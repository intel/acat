////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FeatureExtractions.cs
//
// Handles feature extraction from EEGData
// Includes Learn() that trains the classifiers from input data
//  and Reduce() that outputs classifier scores using the trained classifiers
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class FeatureExtraction
    {
        /// <summary>
        /// Path of the trained classifiers
        /// </summary>
        public string trainedClassifiersFilePath;

        /// <summary>
        /// session ID for the trained classifiers
        /// </summary>
        public string trainedClassifiersSessionID;

        /// <summary>
        /// calibration parameters for the corresponding section
        /// </summary>
        public CalibrationParametersForSection calibrationParameters;

        /// <summary>
        /// Subset of channels
        /// </summary>
        public int[] channelSubset = null;

        /// <summary>
        /// Offset for targets
        /// </summary>
        public int offsetTarget;

        /// <summary>
        /// Sample rate of the sensor
        /// </summary>
        public int sampleRate;

        /// <summary>
        /// Duration for feature extraction
        /// (this is a parameter stored in settings)
        /// </summary>
        public float windowDuration;

        /// <summary>
        /// mean AUC of the trained classifier
        /// </summary>
        public float meanAUC;

        // min AUC required (this is a parameter stored in settings)
        public float minAUCRequired;

        /// <summary>
        /// timestamp when calibration was performed
        /// </summary>
        public DateTime calibrationTime;

        /// <summary>
        /// Boolean, true if plots will be displayed after training the classifiers
        /// (this is a parameter store in settings)
        /// </summary>
        public bool displayPlots;

        /// <summary>
        /// average alpha values stored during calibration
        /// </summary>
        public List<double> avgAlphaValuesCalibration;

        /// <summary>
        /// multiplier of standard deviation for eyes closed detection
        /// (this is a parameter stored in settings)
        /// </summary>
        public float eyesClosed_stdMultiplier;

        /// <summary>
        /// threshold for eyes closed detection
        /// </summary>
        public double eyesClosed_thresholdDetection;

        // Objects used in this class

        /// <summary>
        /// symbols in groups in teh format
        /// [ID group, simbols in the group]
        /// Eg: [1, 1 2 3 4 5] rowID 1, symbols/buttons: 1, 2, 3, 4, 5
        /// </summary>
        private readonly Dictionary<int, int[]> _symbolsInGroups = null;

        /// <summary>
        /// Bandpass filter object
        /// </summary>
        private readonly BandpassFilter _BandPassFilter = null;

        /// <summary>
        /// Data parser object
        /// </summary>
        private readonly DataParser _DataParserObj = null;

        /// <summary>
        /// Channel selection (to reduce number of channels) object
        /// </summary>
        private DimReductChanSel _DimReductChannelSelectionObj = null;

        /// <summary>
        /// Object for data downsampling
        /// </summary>
        private readonly DimReductDownSample _DimReductDownSampleObj = null;

        /// <summary>
        /// Object for PCA reduction
        /// </summary>
        private readonly DimReductPCA _DimReductPCAObj = null;

        // Object for RDA reduction
        private readonly DimReductRDA _DimReductRDAObj = null;

        /// <summary>
        /// KDE for target samples
        /// </summary>
        public NormalKDE KDETarget = null; //add boundaries

        /// <summary>
        /// KDE for non-target samples
        /// </summary>
        public NormalKDE KDENontarget = null; //add boundaries

        /// <summary>
        /// Object for crossvalidation
        /// </summary>
        private readonly KFoldCrossValidate _CrossValidationObj = null;

        /// <summary>
        /// Constructor (parameters read from settings)
        /// </summary>
        /// <param name="isTraining"></param>
        /// <param name="numGroups"></param>
        public FeatureExtraction(Dictionary<int, int[]> pSymbolsInGroups, CalibrationParametersForSection pCalibrationParameters, int numRows = 6, int numCols = 6, bool isRowCol = true)
        {
            // Get parameters from settings
            offsetTarget = BCIActuatorSettings.Settings.Calibration_OffsetTarget;
            windowDuration = BCIActuatorSettings.Settings.FeatureExtraction_WindowDurationInMs / 1000f;
            sampleRate = BCISettingsFixed.DAQ_SampleRate;
            displayPlots = BCIActuatorSettings.Settings.Calibration_DisplaySignalsAfterCalibrationFlag;
            eyesClosed_stdMultiplier = BCIActuatorSettings.Settings.EyesClosed_AdaptiveThreshold_StandardDeviationMultiplier;

            // 2. Create symbols in group (= flashing sequence)
            if (pSymbolsInGroups != null && pSymbolsInGroups.Count > 0)
            {
                _symbolsInGroups = pSymbolsInGroups;
            }
            else
            {
                _symbolsInGroups = new Dictionary<int, int[]>();

                if (isRowCol)
                {
                    //Row-column 6x6
                    // Create flashing sequence (row 1 row 2 row 3 column 1 column2 ... column 7)
                    _symbolsInGroups = new Dictionary<int, int[]>();
                    for (int seqIdx = 0; seqIdx < numRows; seqIdx++)
                    {
                        List<int> seq = new List<int>();
                        for (int c = 1; c <= numCols; c++)
                            seq.Add(seqIdx * numCols + c);
                        _symbolsInGroups.Add(seqIdx + 1, seq.ToArray());
                    }
                    for (int seqIdx = 0; seqIdx < numCols; seqIdx++)
                    {
                        List<int> seq = new List<int>();
                        for (int r = 0; r < numRows; r++)
                            seq.Add(r * numCols + seqIdx + 1);

                        _symbolsInGroups.Add(seqIdx + 1 + numRows, seq.ToArray());
                    }
                }
                else
                {
                    int numGroups = numRows + numCols;
                    //Single
                    for (int groupIdx = 0; groupIdx < numGroups; groupIdx++)
                    {
                        int[] symbolsGroupIdx = new int[] { groupIdx + 1 };
                        _symbolsInGroups.Add(groupIdx + 1, symbolsGroupIdx);
                    }
                }
            }

            // 3. Build objects
            // Channel selection
            List<int> tmpChannelSubset = new List<int>();
            bool[] channelSubsetEnabled = new bool[]
                                            { BCIActuatorSettings.Settings.Classifier_EnableChannel1,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel2,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel3,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel4,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel5,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel6,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel7,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel8,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel9,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel10,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel11,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel12,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel13,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel14,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel15,
                                                BCIActuatorSettings.Settings.Classifier_EnableChannel16
                                            };

            for (int chIdx = 0; chIdx < channelSubsetEnabled.Length; chIdx++)
            {
                if (channelSubsetEnabled[chIdx])
                    tmpChannelSubset.Add(chIdx + 1);
            }
            channelSubset = tmpChannelSubset.ToArray();
            _DimReductChannelSelectionObj = new DimReductChanSel(channelSubset);

            _BandPassFilter = new BandpassFilter(BCISettingsFixed.DAQ_SampleRate);

            // DataParser
            _DataParserObj = new DataParser(BCISettingsFixed.DAQ_SampleRate, BCIActuatorSettings.Settings.FeatureExtraction_WindowDurationInMs, BCIActuatorSettings.Settings.Calibration_OffsetTarget, _symbolsInGroups);

            // Downsample
            _DimReductDownSampleObj = new DimReductDownSample(BCISettingsFixed.DimReduct_DownsampleRate);

            // PCA (same will be applied to each channel)
            _DimReductPCAObj = new DimReductPCA(BCIActuatorSettings.Settings.DimReductPCA_ComponentSortMethod, BCIActuatorSettings.Settings.DimReductPCA_MinEigenvalue);

            // RDA
            _DimReductRDAObj = new DimReductRDA(BCIActuatorSettings.Settings.DimReductRDA_ShrinkParam, BCIActuatorSettings.Settings.DimReductRDA_RegParam);

            // KDE
            KDENontarget = new NormalKDE();
            KDETarget = new NormalKDE();

            // Crossvalidation
            _CrossValidationObj = new KFoldCrossValidate(BCIActuatorSettings.Settings.CrossValidation_NumFolds, BCIActuatorSettings.Settings.CrossaValidation_SortMethod);

            string trainedClassifiersFileID;

            if (pCalibrationParameters != null)
            {
                calibrationParameters = pCalibrationParameters;
                minAUCRequired = (float)pCalibrationParameters.MinimumScoreRequired / (float)100;
                trainedClassifiersFileID = BCIActuatorSettings.Settings.Calibration_TrainedClassifiersFilePath + "_" + calibrationParameters.CalibrationMode.ToString();
            }
            else
            {
                trainedClassifiersFileID = BCIActuatorSettings.Settings.Calibration_TrainedClassifiersFilePath;
                minAUCRequired = 0;
            }

            trainedClassifiersFilePath = Path.Combine(UserManager.CurrentUserDir, trainedClassifiersFileID);
        }

        /// <summary>
        /// Updates subset of channels
        /// </summary>
        /// <param name="availableChannels"></param>
        public void UpdateChannelSubset(bool[] availableChannels)
        {
            List<int> tmpChannelSubset = new List<int>();
            for (int chIdx = 0; chIdx < availableChannels.Length; chIdx++)
            {
                if (availableChannels[chIdx])
                    tmpChannelSubset.Add(chIdx + 1);
            }
            channelSubset = tmpChannelSubset.ToArray();
            _DimReductChannelSelectionObj = new DimReductChanSel(channelSubset);
        }

        /// <summary>
        /// Learns adaptive threshold for eyes closed detection
        /// </summary>
        /// <param name="avgAlphaValues"></param>
        /// <returns></returns>
        public float LearnEyesClosedAdaptiveThreshold(List<double> avgAlphaValues)
        {
            avgAlphaValuesCalibration = avgAlphaValues;

            // Estimate threshold for eyes closed detection
            if (avgAlphaValues != null && avgAlphaValues.Count > 0)
            {
                // Sort values
                var sortedAvgAlphaValues = avgAlphaValues.OrderBy(d => d);

                // Remove 3 outliers
                int numOutliers = 3;
                int numValuesForEstimation = sortedAvgAlphaValues.Count() - 2 * numOutliers;
                var avgAlphaValuesForEstimaton = sortedAvgAlphaValues.ToList().GetRange(numOutliers, numValuesForEstimation);

                // Calculate average = baseline
                var avg = avgAlphaValuesForEstimaton.Average();

                // Calculate std
                var std = Math.Sqrt(avgAlphaValuesForEstimaton.Average(v => Math.Pow(v - avg, 2)));

                // Estimate alphaEyesOpen as avg+N*std
                eyesClosed_thresholdDetection = avg + eyesClosed_stdMultiplier * std;
            }
            return (float)eyesClosed_thresholdDetection;
        }

        /// <summary>
        /// Learn: Extract features + Train KDE Classifier
        /// The trained classifiers are stored into a binary file
        /// </summary>
        /// <param name="filePathMarkers"></param>
        /// <param name="filePathEEG"></param>
        /// <returns></returns>
        public float Learn(String sessionID)
        {
            // 0. Init outputs
            List<double[,]> inputData = null; // input data
            List<double[]> trialData = null; // features (after PCA)
            List<double> scores = null;  // 1D scores (after RDA)
            List<int> trialTargetness = null;
            List<List<int>> trialLabels = null;
            List<double[,]> avgTrialData;

            // ===================== 1. Load data ==========================
            // 1.1 Load file
            List<int> markerValues = null;
            double[,] rawData = null;
            int[] triggerSignal = null;
            String sessionDirectory = null;

            try
            {
                // Train classifiers
                trainedClassifiersSessionID = sessionID;

                string txtLog = "Read data " + sessionID;
                Log.Debug(txtLog);
                FileReader fileReaderObj = new FileReader();
                fileReaderObj.ReadDataAndMarkersFromFiles(sessionID, out rawData, out triggerSignal, out markerValues, out sessionDirectory);
                //fileReaderObj.ReadDataAndMarkersFromTestFiles(out rawData, out triggerSignal, out markerValues, out sessionDirectory,
                //    trainedClassifiersFilePath, filePathMarkers, filePathEEG);

                int numSamples = rawData.GetLength(1);
                int numColumns = rawData.GetLength(0);
                txtLog = "Raw data read " + sessionID + " Num samples: " + numSamples + " Num channels: " + numColumns;
                Log.Debug(txtLog);

                //Plots.plotTriggerSignal(triggerSignal);

                // 1.2 Filter
                Log.Debug("Filtering data");
                _BandPassFilter.FilterData(rawData, triggerSignal, out double[,] filteredData, out int[] delayedTriggerSignal);
                Log.Debug("Data filtered");

                //Plots.plotSignal(filteredData, 1);

                // 1.2 Parse file
                Log.Debug("Parsing data");
                _DataParserObj.ParseData(filteredData, delayedTriggerSignal, markerValues, _symbolsInGroups, out inputData, out trialTargetness, out trialLabels, out List<int> trialGroups, out List<int> targetLabels, out _, out _, true);

                // ===================== 2. Preprocessing ==========================
                if (inputData.Count > 0)
                {
                    int[] trialTargetnessArray = trialTargetness.ToArray();

                    txtLog = "Data parsed. Num trials: " + inputData.Count;
                    Log.Debug(txtLog);

                    // 2.1 Select subset of channels (if applicable)
                    _DimReductChannelSelectionObj.Reduce(inputData, out inputData);
                    avgTrialData = inputData;

                    // 2.2 Downsample
                    _DimReductDownSampleObj.Reduce(inputData, out inputData);

                    // ===================== 3. Feature selection ==========================

                    // 3.1 PCA (reduce dimensions)
                    Log.Debug("Applying PCA");
                    _DimReductPCAObj.Learn(inputData);
                    _DimReductPCAObj.Reduce(inputData, out trialData);

                    // 3.2 RDA (transform to scores using crossValidation)
                    txtLog = "Crossvalidation with " + trialData.Count + " trials";
                    Log.Debug(txtLog);
                    scores = _CrossValidationObj.CrossValidate(_DimReductRDAObj, trialData, trialTargetness);

                    // Calculate performance
                    txtLog = "Calculating AUC for " + scores.Count + " trials";
                    Log.Debug(txtLog);
                    meanAUC = ClassifierUtils.CalculateAUC(scores, trialTargetnessArray, out double[] TPrate, out double[] FPrate);
                    txtLog = "AUC " + meanAUC;
                    Log.Debug(txtLog);
                    // 3.3 Train RDA with all data (scores for target/nontarget class distributions are calculated with crossV)
                    _DimReductRDAObj.Learn(trialData, trialTargetness.ToList());

                    // ===================== 4. Build class distributions ==========================

                    // 4.1 Separate in targets and non-targets
                    int[] indNonTarget = Matrix.Find(trialTargetnessArray, element => element == 0);
                    List<double> nonTargetScores = Matrix.Get(scores, indNonTarget); //<trial0>[sample0, sample1, sample2...], <trial1[sample0, sample1, sample2]...

                    int[] indTarget = Matrix.Find(trialTargetnessArray, element => element == 1);
                    List<double> targetScores = Matrix.Get(scores, indTarget); //<trial0>[sample0, sample1, sample2...], <trial1[sample0, sample1, sample2]...

                    // 4.2 Construct KDEs
                    KDETarget.BuildKDE(targetScores.ToArray());
                    KDENontarget.BuildKDE(nonTargetScores.ToArray());

                    // ===================== 5. Plot results ==========================

                    if (displayPlots)
                    {
                        try
                        {
                            Plots.plotERPs(avgTrialData, trialTargetnessArray, _DimReductChannelSelectionObj.channelSubset, sampleRate, meanAUC.ToString());
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.Message);
                        }

                        /* Display plots with class distributions and AUC
                         * GraphDisplayerForm1x2 graphFormResultsCross = new GraphDisplayerForm1x2();
                        GraphPane graphPaneCrossLeft = graphFormResultsCross.graphControlLeft.GraphPane;
                        GraphPane graphPaneCrossRight = graphFormResultsCross.graphControlRight.GraphPane;

                        Plots.plotClassDistributions(scores, trialTargetnessArray, graphPaneCrossLeft);
                        Plots.plotRoC(TPrate, FPrate, meanAUC, graphPaneCrossRight);

                        graphFormResultsCross.TopMost = true;
                        graphFormResultsCross.ShowDialog(); //In ACAT, dont show it

                        graphFormResultsCross.ShowDialog(Context.AppPanelManager.GetCurrentForm() as Form);
                        */
                    }

                    // ===================== 6. Save results ==========================
                    // Save AUC
                    String filePath = Path.Combine(sessionDirectory, "CalibrationResults.txt");
                    using (StreamWriter sw = new StreamWriter(filePath))
                        sw.WriteLine("AUC = " + meanAUC);

                    // Save classifier in session directory (this will be saved regardless of the AUC)
                    String filePathClassifier = Path.Combine(sessionDirectory, System.IO.Path.GetFileName(trainedClassifiersFilePath));
                    Utilities.WriteToBinaryFile(filePathClassifier, this);

                    // Save classifier in actuator folder (this is the classifier that will be used on typing, only saved if AUC>minAUC)
                    calibrationTime = DateTime.Now;
                    if (meanAUC >= minAUCRequired)
                        Utilities.WriteToBinaryFile(trainedClassifiersFilePath, this);
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                meanAUC = -1;

                // Save Error in file
                if (sessionDirectory != null)
                {
                    String filePath = Path.Combine(sessionDirectory, "CalibrationResults.txt");
                    using (StreamWriter sw = new StreamWriter(filePath))
                        sw.WriteLine("AUC = " + "ERROR");
                }
            }
            return meanAUC;
        }

        /// <summary>
        /// Reduce with data already in correct format
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public List<double> Reduce(List<double[,]> inputData)
        {
            // ===================== 2. Preprocessing ==========================
            // 2.1 Select subset of channels (if applicable)
            Log.Debug("Reducing number of channels for " + inputData.Count + " trials");
            _DimReductChannelSelectionObj.Reduce(inputData, out inputData);
            Log.Debug("Number of channels reduced to " + _DimReductChannelSelectionObj.channelSubset.Length);

            // 2.2 Downsample
            Log.Debug("Downsampling");
            _DimReductDownSampleObj.Reduce(inputData, out inputData);
            Log.Debug("Data downsampled by " + _DimReductDownSampleObj.downsampleRate + " for " + inputData.Count + " trials");

            // ===================== 3. Feature selection ==========================

            // PCA (reduce dimensions)"
            Log.Debug("Starting PCA.reduce for " + inputData.Count + " trials");
            _DimReductPCAObj.Reduce(inputData, out List<double[]> trialData);
            Log.Debug("PCA reduced for " + trialData.Count);

            // RDA (obtain 1-dimensional scores)
            Log.Debug("Starting RDA.reduce for " + trialData.Count + " trials");
            _DimReductRDAObj.Reduce(trialData, out List<double> scores);
            Log.Debug("RDA reduced " + scores.Count + " scores");

            return scores;
        }

        /// <summary>
        /// Reduce with data needed to be parsed
        /// </summary>
        /// <param name="allData"></param> all th data (from brainflow, including sampleIdx, trigger, channel, etc
        /// <param name="markerValues"></param>
        /// <returns></returns>
        public List<double> Reduce(double[,] allData, List<int> markerValues, Dictionary<int, int[]> symbolsInGroups, out double[,] incompleteData, out List<int> incompleteMarkerValues)//eegData timestamps
        {
            List<double> trialScores = null;

            // ===================== 1. Parse data ==========================

            int numSamples = allData.GetLength(1);
            int numColumns = allData.GetLength(0);
            incompleteData = null;
            Log.Debug("Parsing data from brainflow " + numSamples + " samples and " + numColumns + " columns");
            _DataParserObj.ParseDataFromBrainflow(allData, out double[,] rawData, out int[] triggerSignal, out _);

            numSamples = rawData.GetLength(1);
            Log.Debug("Data parsed " + numSamples + " samples.");

            _BandPassFilter.FilterData(rawData, triggerSignal, out double[,] filteredData, out int[] delayedTriggerSignal);

            // 1.2 Parse file
            Log.Debug("Parsing data");
            _DataParserObj.ParseData(filteredData, delayedTriggerSignal, markerValues, symbolsInGroups, out List<double[,]> trialData, out _, out _, out List<int> trialMarkers, out _, out int incompleteSampleIdx, out incompleteMarkerValues);
            Log.Debug("Data parsed for " + trialMarkers.Count + " markers and" + trialData.Count + " trials");

            // trialLabels: if singleBox paradigm: trialLabels[trial1][boxID]
            //              if RC paradigm: trialLabels[trial1][box1 box2 box3] containing all highlighted boxes

            if (incompleteSampleIdx != -1)
            {
                int startSampleIdx = incompleteSampleIdx - _BandPassFilter.GetGroupDelay();
                incompleteData = _DataParserObj.GetRemaining(allData, startSampleIdx);
                Log.Debug("Incomplete data remaining with " + incompleteData.GetLength(1) + " columns and " + incompleteData.GetLength(0) + " samples for " + trialMarkers.Count + " markers");
            }

            if (trialData.Count > 0 && trialData.Count == trialMarkers.Count)
            {
                trialScores = Reduce(trialData);
                Log.Debug("Reduced " + trialScores.Count + " trials found");
            }
            return trialScores; // will return null if can't be computed
        }

        /// <summary>
        /// Gemerate scores using KDE class-conditional distributions
        /// </summary>
        /// <param name="numTrials"></param>
        /// <param name="trialTargetness"></param>
        /// <returns></returns>
        public List<double> GenerateScoresFromDistributions(int numTrials, List<bool> trialTargetness)
        {
            // Generate samples for target and non-targets
            double[] scoresT = KDENontarget.generateSamples(numTrials);
            double[] scoresNT = KDETarget.generateSamples(numTrials);

            // Save target/non-target generated samples in target/non-target indices
            List<double> scores = scoresNT.ToList();
            for (int trialIdx = 0; trialIdx < numTrials; trialIdx++)
            {
                if (trialTargetness[trialIdx])
                    scores[trialIdx] = scoresT[trialIdx];
                else
                    scores[trialIdx] = scoresNT[trialIdx];
            }
            return scores;
        }
    }
}