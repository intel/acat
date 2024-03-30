////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DecisionMaker.cs
//
// Handles all the decision maker processes by processing input data and predicting
// a selection and calculating the probabilities for all highlighted options
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public class DecisionMaker
    {
        // ************ Params and objects loaded at init

        /// <summary>
        /// maximum number of sequences to make a selection
        /// (this is typically read from settings)
        /// </summary>
        public int maxNumberOfSequences;

        /// <summary>
        /// Confidence threshold to make a selection
        /// (this is typically read from settings)
        /// </summary>
        public float confidenceThreshold;

        /// <summary>
        /// Trained classifiers
        /// </summary>
        public FeatureExtraction TrainedClassifiersObj;

        // *************** Variables updated at every iteration

        /// <summary>
        /// likelihods calculated if targets
        /// Format [symbol][seq1, seq2...]
        /// </summary>
        private Dictionary<int, List<double>> _likelihoodsTarget = null;

        /// <summary>
        /// likelihods calculated if non-taargets
        /// Format [symbol][seq1, seq2...]
        /// </summary>
        private Dictionary<int, List<double>> _likelihoodsNontarget = null;

        /// <summary>
        /// Boolean, true to enable the probabilities from the language model
        /// </summary>
        public bool enableLanguageModelProbabilities;

        /// <summary>
        /// probabilities from language model
        /// Format: [symbol][probability]
        /// </summary>
        private Dictionary<int, double> _languageModelProbabilities = null; //(symbol, prob)

        /// <summary>
        /// index corresponding the the actual sequence
        /// </summary>
        private int _sequenceCount;

        /// <summary>
        /// Incomplete scores from previous repetition
        /// </summary>
        private List<double> incompleteTrialScores;

        /// <summary>
        /// Incomplete data from previous repetition (less than 500ms)
        /// </summary>
        private double[,] incompleteData;

        /// <summary>
        /// Incomplete markers from previous repetition (less than 500ms)
        /// </summary>
        private List<int> incompleteMarkers;

        /// <summary>
        /// Constructor (Params read from settings)
        /// </summary>
        public DecisionMaker(string TrainedClassifiersFilePath)
        {
            enableLanguageModelProbabilities = false; // by default (actuator will set flag for different type of LM probabilities)
            maxNumberOfSequences = BCIActuatorSettings.Settings.Classifier_MaxDecisionSequences;
            confidenceThreshold = BCIActuatorSettings.Settings.Classifier_ConfidenceThreshold;
            _sequenceCount = 0;

            // Load trained classifier (from file)d
            try
            {
                //String filePath = Path.Combine(UserManager.CurrentUserDir, settings.Calibration_TrainedClassifiersFilePath);

                _likelihoodsTarget = new Dictionary<int, List<double>>();
                _likelihoodsNontarget = new Dictionary<int, List<double>>();

                if (File.Exists(TrainedClassifiersFilePath))
                {
                    TrainedClassifiersObj = Utilities.ReadFromBinaryFile<FeatureExtraction>(TrainedClassifiersFilePath);

                    string logTxt = "Decision maker created with calibration file " + TrainedClassifiersObj.trainedClassifiersSessionID + ". AUC: " + TrainedClassifiersObj.meanAUC + " Max number of sequences: " + maxNumberOfSequences + ". Confidence threshold: " + confidenceThreshold;
                    Log.Debug(logTxt);
                }
                else
                    Log.Debug("Calibration file does not exist");
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Constructor (Params as input)
        /// </summary>
        public DecisionMaker(int maxNumberOfSeqs, float confThreshold)
        {
            maxNumberOfSequences = maxNumberOfSeqs;
            confidenceThreshold = confThreshold;

            // Load trained classifier
            String filePath = Path.Combine(UserManager.CurrentUserDir, "EEGData", "TrainedClassifiers");
            if (File.Exists(filePath))
                TrainedClassifiersObj = Utilities.ReadFromBinaryFile<FeatureExtraction>(filePath);
            else
                Log.Debug("Calibration file does not exist");

            _likelihoodsTarget = new Dictionary<int, List<double>>();
            _likelihoodsNontarget = new Dictionary<int, List<double>>();
            _sequenceCount = 0;
        }

        public void AddNextCharacterProbabilities(Dictionary<int, double> pNextCharProbs)
        {
            _languageModelProbabilities = pNextCharProbs;
        }

        /// <summary>
        /// Compute posterior probs with trial scores generated from KDEs
        /// </summary>
        /// <param name="trialButtons"></param>
        /// <param name="trialTargetness"></param>
        /// <param name="decided"></param>
        /// <param name="decidedButtonID"></param>
        /// <param name="posteriorProbs"></param>
        //public void ComputePosteriorProbsFromAutogeneratedSamples(List<int[]> trialButtons, List<bool> trialTargetness,
        // Sai-EEG
        public void ComputePosteriorProbsFromAutogeneratedSamples(Dictionary<int, int[]> trialButtons, List<bool> trialTargetness,
           out bool decided, out int decidedButtonID, out int repetition, out SortedDictionary<int, double> posteriorProbs, out Dictionary<int, double> eegProbs,
            out Dictionary<int, double> nextCharacterProbs)
        {
            List<double> trialScores = TrainedClassifiersObj.GenerateScoresFromDistributions(trialButtons.Count, trialTargetness);
            ComputePosteriorProbs(trialScores.ToArray(), trialButtons, out decided, out decidedButtonID, out repetition, out posteriorProbs, out eegProbs, out nextCharacterProbs);
        }

        /// <summary>
        /// Calculates the posterior probabilities given the data read from teh device (needs to be parsed)
        /// </summary>
        /// <param name="data2parse"></param>
        /// <param name="markerValues"></param>
        /// <param name="trialButtons"></param>
        /// <param name="decided"></param>
        /// <param name="decidedButtonID"></param>
        /// <param name="posteriorProbs"></param>
        // Sai-EEG
        //public void ComputePosteriorProbs(List<double[]> data2parse, List<int> markerValues, List<int[]> trialButtons,
        public void ComputePosteriorProbs(double[,] data2parse, List<int> markerValues, Dictionary<int, int[]> flashingSequence, bool[] availableChannels,
           out bool decided, out int decidedButtonID, out int repetition, out SortedDictionary<int, double> posteriorProbs, out Dictionary<int, double> eegProbs,
            out Dictionary<int, double> nextCharacterProbs)
        {
            decided = false;
            decidedButtonID = 0;
            repetition = 0;
            posteriorProbs = null;
            eegProbs = null;
            nextCharacterProbs = null;

            try
            {
                // Update channel subset
                TrainedClassifiersObj.UpdateChannelSubset(availableChannels);

                List<string> markerValuesString = markerValues.ConvertAll<string>(x => x.ToString());
                Log.Debug("Reduce data w/ " + data2parse.Length + " samples and " + String.Join(", ", markerValuesString) + " markers");

                // Append incomplete data and markers
                double[,] allDataToParse;
                List<int> allMarkersToParse;
                if (incompleteData != null && incompleteData.Length > 0)
                {
                    // Concatenate markers
                    allMarkersToParse = incompleteMarkers;
                    allMarkersToParse.AddRange(markerValues);
                    Log.Debug("Markers concatenated. Total markers: " + allMarkersToParse.Count);

                    // Concatenate data
                    int numColumns = data2parse.GetLength(0);
                    int numSamples = data2parse.GetLength(1) + incompleteData.GetLength(1);
                    allDataToParse = new double[numColumns, numSamples];

                    for (int columnIdx = 0; columnIdx < numColumns; columnIdx++)
                    {
                        for (int sampleIdx = 0; sampleIdx < incompleteData.GetLength(1); sampleIdx++)
                            allDataToParse[columnIdx, sampleIdx] = incompleteData[columnIdx, sampleIdx];
                        for (int sampleIdx = 0; sampleIdx < data2parse.GetLength(1); sampleIdx++)
                            allDataToParse[columnIdx, sampleIdx + incompleteData.GetLength(1)] = data2parse[columnIdx, sampleIdx];
                    }
                    Log.Debug("Data concatenated. Total data: " + allDataToParse.GetLength(1) + " containing " + incompleteData.GetLength(1) + " from previous iterations and " + data2parse.GetLength(1) + " from current iteration");
                }
                else
                {
                    allDataToParse = data2parse;
                    allMarkersToParse = markerValues;
                }

                //TODO: add out trialFlashingSequence
                List<double> trialScores = TrainedClassifiersObj.Reduce(allDataToParse, allMarkersToParse, flashingSequence, out incompleteData, out incompleteMarkers);

                if (trialScores != null)
                    Log.Debug("Data reduced " + trialScores.Count + " trial scores found");
                else
                    Log.Debug("Data not reduced. Scores returned null. Will reduce in new iteration");

                List<double> appendedTrialScores = new List<double>();
                if (incompleteTrialScores != null)
                {
                    Log.Debug("Creating appended list with " + incompleteTrialScores.Count + " scores");
                    appendedTrialScores = new List<double>(incompleteTrialScores);
                }
                if (trialScores != null && trialScores.Count > 0)
                {
                    // Append trial scores
                    if (appendedTrialScores != null && appendedTrialScores.Count > 0)
                    {
                        Log.Debug("Adding current " + trialScores.Count + " scores to appended trialscores");
                        appendedTrialScores.AddRange(trialScores);

                        if (incompleteTrialScores != null)
                            Log.Debug("Appended trialScores. Total scores: " + appendedTrialScores.Count + " with " + incompleteTrialScores.Count + " from previous iteration and " + trialScores.Count + " from current iteration");
                        else
                            Log.Debug("Appended trialScores. Total scores: " + appendedTrialScores.Count + " with 0 from previous iteration and " + trialScores.Count + " from current iteration");
                    }
                    else
                    {
                        appendedTrialScores = trialScores;
                        Log.Debug("No trial scores from previous iterations. Current " + trialScores.Count + " will be used");
                    }
                }

                // Compute posterior probabilities
                if (appendedTrialScores != null)
                {
                    if (appendedTrialScores != null && appendedTrialScores.Count >= flashingSequence.Count)
                    {
                        // Get only trialscores corresponding to the flashingSequence
                        // List<double> completeTrialScores = new List<double>(appendedTrialScores);
                        List<double> completeTrialScores = new List<double>(appendedTrialScores.GetRange(0, flashingSequence.Count));

                        Log.Debug("Calculating probabilities for " + completeTrialScores.Count + " scores corresponding to " + flashingSequence.Count + " trials");
                        ComputePosteriorProbs(completeTrialScores.ToArray(), flashingSequence, out decided, out decidedButtonID, out repetition, out posteriorProbs, out eegProbs, out nextCharacterProbs);
                        Log.Debug("Posterior probabilities calculated. Repetition: " + repetition + " , Decided: " + decided + " , Decided button ID: " + decidedButtonID);

                        if (appendedTrialScores.Count > flashingSequence.Count)
                        {
                            incompleteTrialScores = new List<double>(appendedTrialScores.GetRange(flashingSequence.Count, appendedTrialScores.Count - flashingSequence.Count));
                            Log.Debug("Incomplete Trial scores " + incompleteTrialScores.Count + " saved for next iteration");
                        }
                    }
                    else
                    {
                        if(trialScores!=null)
                            Log.Debug("Incomplete trial scores. Expected: " + flashingSequence.Count + " Calculated: " + trialScores.Count + " Waiting for new repetition");
                        else
                            Log.Debug("Incomplete trial scores. Expected: " + flashingSequence.Count + " Calculated: 0  Waiting for new repetition");

                        if (incompleteTrialScores == null)
                            incompleteTrialScores = appendedTrialScores;
                        else
                            incompleteTrialScores.AddRange(appendedTrialScores);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Calculates the posterior probabilities given the scores for each trial
        /// </summary>
        /// <param name="trialScores"></param>
        /// <param name="trialButtons"></param>
        /// <param name="decided"></param>
        /// <param name="decidedButtonID"></param>
        /// <param name="posteriorProbs"></param>
        public void ComputePosteriorProbs(double[] trialScores, Dictionary<int, int[]> trialButtons,
            out bool decided, out int decidedButtonID, out int repetition,
            out SortedDictionary<int, double> posteriorProbs,
            out Dictionary<int, double> eegProbs,
            out Dictionary<int, double> nextCharacterProbs)
        {
            // ======================== 1. Calculate trial probabilities =====================

            decided = false;
            decidedButtonID = -1;
            posteriorProbs = null;
            nextCharacterProbs = null;
            eegProbs = null;
            repetition = 0;

            try
            {
                if (trialScores != null && trialScores.Length > 0)
                {
                    // 1.1 Calculate probabilities
                    double[] probsTarget = TrainedClassifiersObj.KDETarget.CalculateProbabilities(trialScores);
                    double[] probsNontarget = TrainedClassifiersObj.KDENontarget.CalculateProbabilities(trialScores);

                    //1.2 Append probabilities to
                    //for(int trialIdx = 0; trialIdx< trialButtons.Count; trialIdx++)
                    int trialIdx = 0;
                    foreach (var rowColumnIDs in trialButtons.Keys)
                    {
                        //int[] buttonsInTrial = trialButtons[trialIdx];
                        int[] buttonsInTrial = trialButtons[rowColumnIDs];
                        double pT = probsTarget[trialIdx];
                        double pN = probsNontarget[trialIdx];

                        foreach (int buttonID in buttonsInTrial)
                        {
                            if (!_likelihoodsNontarget.ContainsKey(buttonID))
                                _likelihoodsNontarget.Add(buttonID, new List<double>());
                            if (!_likelihoodsTarget.ContainsKey(buttonID))
                                _likelihoodsTarget.Add(buttonID, new List<double>());

                            _likelihoodsTarget[buttonID].Add(Math.Log(pT));
                            _likelihoodsNontarget[buttonID].Add(Math.Log(pN));
                        }
                        trialIdx++;
                    }

                    // ======================== 1. Calculate posterior probabilities for each label =====================
                    Dictionary<int, double> logPosteriors = new Dictionary<int, double>();
                    eegProbs = new Dictionary<int, double>();
                    repetition = 0;
                    double maxLogPosteriors = Double.MinValue;
                    foreach (int buttonID in _likelihoodsNontarget.Keys)
                    {
                        double buttonLogPosterior = 0;
                        for (int repetitionIdx = 0; repetitionIdx < _likelihoodsTarget[buttonID].Count; repetitionIdx++)
                        {
                            double tmpLogPosterior = _likelihoodsTarget[buttonID][repetitionIdx] - _likelihoodsNontarget[buttonID][repetitionIdx];

                            buttonLogPosterior += tmpLogPosterior;
                        }
                        eegProbs.Add(buttonID, buttonLogPosterior);

                        // Add next character probabilities
                        if (enableLanguageModelProbabilities && _languageModelProbabilities != null)
                        {
                            Log.Debug("Adding language model probabilities");
                            if (_languageModelProbabilities.ContainsKey(buttonID))
                            {
                                buttonLogPosterior += Math.Log(Math.Sqrt(_languageModelProbabilities[buttonID]));
                            }
                            // else
                            //     buttonLogPosterior = buttonLogPosterior + Math.Log(Math.Sqrt(0.05));
                        }

                        if (Double.IsInfinity(buttonLogPosterior))
                            buttonLogPosterior = Math.Log(Double.MaxValue);

                        if (Double.IsNaN(buttonLogPosterior))
                            buttonLogPosterior = 0;

                        // Get maximum value to normalize
                        if (buttonLogPosterior > maxLogPosteriors)
                            maxLogPosteriors = buttonLogPosterior;

                        logPosteriors[buttonID] = buttonLogPosterior;
                    }

                    posteriorProbs = new SortedDictionary<int, double>();
                    double normFactor = 0;
                    foreach (int buttonID in _likelihoodsNontarget.Keys)
                    {
                        double buttonPosterior = Math.Exp(logPosteriors[buttonID] - maxLogPosteriors);
                        posteriorProbs[buttonID] = buttonPosterior;

                        normFactor += buttonPosterior;
                    }

                    // Normalize probabilities
                    double maxProb = 0;
                    decidedButtonID = 0;
                    foreach (int buttonID in _likelihoodsNontarget.Keys)
                    {
                        posteriorProbs[buttonID] = posteriorProbs[buttonID] / normFactor;

                        if (posteriorProbs[buttonID] > maxProb)
                        {
                            maxProb = posteriorProbs[buttonID];
                            decidedButtonID = buttonID;
                        }
                    }

                    // Outputs
                    nextCharacterProbs = _languageModelProbabilities;

                    _sequenceCount++;
                    repetition = _sequenceCount;

                    // Make deicision if the maximum probability is bigger than the confidence threshold
                    decided = false;
                    if (maxProb > confidenceThreshold || _sequenceCount == maxNumberOfSequences)
                    {
                        string txtLog = "Decision made. Repetition: " + _sequenceCount + " Probability: " + maxProb;
                        Log.Debug(txtLog);
                        decided = true;
                        _sequenceCount = 0;
                        _likelihoodsTarget = new Dictionary<int, List<double>>();
                        _likelihoodsNontarget = new Dictionary<int, List<double>>();
                        _languageModelProbabilities = null;
                        incompleteData = null;
                        incompleteMarkers = null;
                        incompleteTrialScores = null;
                    }

                    if (posteriorProbs == null || posteriorProbs.Count == 0)
                        Log.Debug("Zero posteriorProbs");
                }
                else
                    Log.Debug("Error when computing probabilities, trialScore is null or 0, returning null and restarting algorithm");
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Restart Probabilities: sets likelihoodsTarget and likelihoodsTarget to 0 and restarts sequenceCount to 0
        /// </summary>
        public void RestartProbabilities()
        {
            Log.Debug("Restarting probabilities");
            _sequenceCount = 0;
            _likelihoodsTarget = new Dictionary<int, List<double>>();
            _likelihoodsNontarget = new Dictionary<int, List<double>>();
            incompleteData = null;
            incompleteTrialScores = null;
            incompleteMarkers = null;
            //_nextCharacterProbabilities = new Dictionary<int, double>(); ///??
        }
    }
}