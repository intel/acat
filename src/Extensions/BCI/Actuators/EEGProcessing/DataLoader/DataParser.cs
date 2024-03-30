////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DataParser.cs
//
// Handles parsing of the data
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    [Serializable]
    public class DataParser
    {
        /// <summary>
        /// Offset added to targets
        /// (this parameter is typically set in settings)
        /// </summary>
        private int offsetTarget;

        /// <summary>
        /// Sample rate of the sensor
        /// </summary>
        private int sampleRate;

        /// <summary>
        /// Duration of teh window for feature extraction
        /// </summary>
        private int windowDuration;

        /// <summary>
        /// symbols in each group
        /// Format: [ID group, IDs symbols in groups]
        /// Eg: [1, 1 2 3 4] = row 1: buttons 1, 2, 3, 4
        /// </summary>
        private Dictionary<int, int[]> _symbolsInGroups;

        public DataParser(int pSampleRate, int pWindowDuration, int pOffsetTarget, Dictionary<int, int[]> pSymbolsInGroups)
        {
            offsetTarget = pOffsetTarget;
            sampleRate = pSampleRate;
            windowDuration = pWindowDuration;
            _symbolsInGroups = pSymbolsInGroups;
        }

        /// <summary>
        /// Parses data to the correct format
        /// </summary>
        /// <param name="inputData"> BrainflowFormat[allChannels (including timestamps, etc x numSamples ] (24xnSamples),
        /// <param name="markerValues"> Format: [int numStimuli] containing the values of each marker
        /// <param name="trialData"></param>
        /// <param name="trialTargetness"></param>
        public void ParseDataFromBrainflow(double[,] allData, out double[,] rawData, out int[] triggerData, out int numTriggerPulses)
        {
            int numSamples = allData.GetLength(1);
            int numColumns = allData.GetLength(0);

            rawData = new double[BCIActuatorSettings.Settings.DAQ_NumEEGChannels, numSamples];
            triggerData = new int[numSamples];
            numTriggerPulses = 0;
            double tmpTriggerSample;
            int valueNextSamples = 0;
            bool startPulseFound = false;
            for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
            {
                if (BCIActuatorSettings.Settings.DataParser_UseSoftwareTrigers)
                {
                    tmpTriggerSample = Convert.ToInt32(allData[BCISettingsFixed.DataParser_IdxTriggerSignal_Sw - 1, sampleIdx]); // sw trigger is stored in last channel
                    //Generate signal of 0 and 1 pulses (sw trigger is 1=on, 2=off, 0=no marker sent)
                    switch (tmpTriggerSample)
                    {
                        case 1: valueNextSamples = 0; break;
                        case 2: valueNextSamples = 1; numTriggerPulses++; break;
                    }
                    triggerData[sampleIdx] = valueNextSamples;
                }
                else
                {
                    // triggerData
                    tmpTriggerSample = Convert.ToDouble(allData[BCISettingsFixed.DataParser_IdxTriggerSignal_Hw - 1, sampleIdx]);
                    if (tmpTriggerSample == 0)
                    {
                        triggerData[sampleIdx] = 1; //digital sensor inverts it
                        startPulseFound = true;
                    }
                    else
                    {
                        triggerData[sampleIdx] = 0; // digital sensor inverts it

                        // Pulse detected
                        if (startPulseFound)
                            numTriggerPulses++;

                        startPulseFound = false;
                    }
                }

                // rawData
                double[] chData = new double[BCIActuatorSettings.Settings.DAQ_NumEEGChannels];
                for (int ch = 0; ch < BCIActuatorSettings.Settings.DAQ_NumEEGChannels; ch++)
                {
                    rawData[ch, sampleIdx] = Convert.ToDouble(allData[BCISettingsFixed.DataParser_IdxStartEEGData - 1 + ch, sampleIdx]); //Indexed from 0, value given from 1
                }
            }
        }

        /// <summary>
        /// Parses data to the correct format
        /// </summary>
        /// <param name="inputData"> Format: [numChannels (rows) x numSamples (columns)],
        /// <param name="triggerData"> Format:[(int) numSamples] containign 1 for onset, 0 for offset
        /// <param name="markerValues"> Format: [int numStimuli] containing the values of each marker
        /// <param name="trialData"></param>
        /// <param name="trialTargetness"></param>
        public void ParseData(double[,] inputData, int[] triggerData, List<int> markerValues, Dictionary<int, int[]> pSymbolsInGroups,
        out List<double[,]> trialData, out List<int> trialTargetness,
        out List<List<int>> trialLabels, out List<int> trialGroups, out List<int> targetLabels, out int sampleIdxIncompleteData, out List<int> incompleteMarkerValues, bool triggerIsOnForTargets = false)
        {
            // Init outputs
            trialData = new List<double[,]>();
            trialTargetness = new List<int>();
            trialLabels = new List<List<int>>();
            trialGroups = new List<int>();
            targetLabels = new List<int>();

            if (pSymbolsInGroups != null && pSymbolsInGroups.Count > 0)
                _symbolsInGroups = pSymbolsInGroups;

            // Get params and dimensions
            int windowLength = (int)(sampleRate * windowDuration / 1000);
            int numSamples = inputData.GetLength(1);
            int numChannels = inputData.GetLength(0);

            sampleIdxIncompleteData = -1;
            incompleteMarkerValues = null;
            bool incompleteDataFound = false;

            // Split data in trials
            bool OnsetFound = false;
            int stimulusID = 0;
            int targetID = 0;
            int stimulusIdx = 0;
            bool hasTargets = false;
            int idxFirstMarket = markerValues.FindIndex(element => element > offsetTarget);
            if (idxFirstMarket >= 0)
                hasTargets = true;

            try
            {
                int sampleIdx = 0;
                while (!incompleteDataFound || sampleIdx < numSamples)
                {
                    // Find onset
                    if (!incompleteDataFound && triggerData[sampleIdx] == 1 && !OnsetFound) //Onset found
                    {
                        OnsetFound = true;
                        stimulusID = markerValues[stimulusIdx]; //stimulusIDx is incremented accordingly

                        if (stimulusID > offsetTarget) //Target found
                        {
                            targetID = stimulusID - offsetTarget;

                            if (!triggerIsOnForTargets)
                                stimulusIdx++;
                        }

                        stimulusID = markerValues[stimulusIdx]; //stimulusIDx is incremented accordingly

                        if ((triggerIsOnForTargets && stimulusID > 0 && stimulusID < offsetTarget) ||
                            (!triggerIsOnForTargets && hasTargets && targetID > 0 && stimulusID > 0 && stimulusID < offsetTarget) ||
                            (!triggerIsOnForTargets && !hasTargets && stimulusID > 0 && stimulusID < offsetTarget))
                        {
                            //stimulusID = stimulusID;///// - 20;  ///!!!!!!!!!!!!!!! DEPENDING ON

                            // Get windowed data
                            if (sampleIdx + windowLength < numSamples)
                            {
                                double[,] windowedData = new double[windowLength, numChannels];
                                for (int i = 0; i < windowLength; i++)
                                    for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
                                        windowedData[i, channelIdx] = inputData[channelIdx, sampleIdx + i];

                                // Add trialData
                                trialData.Add(windowedData);

                                // Add target (only if it was displayed)
                                if (targetID != 0)
                                    targetLabels.Add(targetID);

                                // Add group
                                trialGroups.Add(stimulusID);

                                // Add symbols IDs (symbol corresponding to the group (row or column)
                                if (_symbolsInGroups != null) // Row/Column mode (varoious symbols / stimuli
                                {
                                    int[] symbolsInThisGroup = _symbolsInGroups[stimulusID]; // symbolsInGroups.GetRow(stimulusID - 1); (for [groupIdx, symbols])
                                    trialLabels.Add(symbolsInThisGroup.ToList());
                                }
                                else // Single mode (one symbol/stimuli)
                                    trialLabels.Add(new List<int> { stimulusID });

                                // Add targetness
                                if (targetID != 0)
                                {
                                    if (_symbolsInGroups != null) // Row/Colum mode (various symbols / stimuli)
                                    {
                                        int[] symbolsInThisGroup = _symbolsInGroups[stimulusID]; // symbolsInGroups.GetRow(stimulusID - 1); (for [groupIdx, symbols])
                                        if (Array.Exists(symbolsInThisGroup, element => element == targetID))
                                            trialTargetness.Add(1); //target
                                        else
                                            trialTargetness.Add(0); //nontarget
                                    }
                                    else // Single mode (one symbol/stimuli)
                                    {
                                        if (stimulusID == targetID)
                                            trialTargetness.Add(1); //target
                                        else
                                            trialTargetness.Add(0); //nontarget
                                    }
                                }
                            }
                            else
                            {
                                // Save incomplete data
                                incompleteDataFound = true;
                                int numRemainingMarkers = markerValues.Count - stimulusIdx;
                                incompleteMarkerValues = new List<int>(numRemainingMarkers);
                                incompleteMarkerValues.AddRange(markerValues.GetRange(stimulusIdx, numRemainingMarkers));

                                sampleIdxIncompleteData = sampleIdx - 1; //from onset -1 sample

                                /*
                                int numRemainingSamples = numSamples - sampleIdx-2; // from onset - 1 sample (to get the 0 before the pulse) -1 sample (indexed from 0)
                                incompleteData = new double[numChannels, numRemainingSamples];
                                incompleteTriggerData = new int[numRemainingSamples];
                                incompleteMarkerValues = new List<int>(numRemainingMarkers);
                                for (int i = 0; i < numRemainingSamples; i++)
                                {
                                    incompleteTriggerData[i] = triggerData[sampleIdx+i];
                                    for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
                                        incompleteData[channelIdx, i] = inputData[channelIdx, sampleIdx + i];
                                }
                                incompleteMarkerValues.AddRange(markerValues.GetRange(stimulusIdx, numRemainingMarkers));
                                */
                            }
                        }

                        stimulusIdx++;
                    }
                    if (triggerData[sampleIdx] == 0 && OnsetFound) //Offset found
                        OnsetFound = false;

                    sampleIdx++;
                }
            }
            catch (Exception e)
            {
                //Log.Debug(e.getClass().getName()); e)
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Get remaining data given a starting sample
        /// </summary>
        /// <param name="allData"></param>
        /// <param name="startingSampleIdx"></param>
        /// <returns></returns>
        public double[,] GetRemaining(double[,] allData, int startingSampleIdx)
        {
            double[,] remainingData = null;

            if (allData != null)
            {
                int numSamples = allData.GetLength(1);
                int numColumns = allData.GetLength(0);

                int numRemainingSamples = numSamples - startingSampleIdx; // from onset - 1 sample
                remainingData = new double[numColumns, numRemainingSamples];

                String txtLog = "Get " + numRemainingSamples + " remaining data. Input data with Num Columns: " + numColumns + " Num Samples: " + numSamples;
                Log.Debug(txtLog);
                try
                {
                    for (int columnIdx = 0; columnIdx < numColumns; columnIdx++)
                        for (int i = 0; i < numRemainingSamples; i++)
                            remainingData[columnIdx, i] = allData[columnIdx, startingSampleIdx + i];

                    txtLog = "Remaining dat. Num Columns: " + remainingData.GetLength(0) + " Num samples: " + remainingData.GetLength(1);
                    Log.Debug(txtLog);
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message);
                }
            }

            return remainingData;
        }
    }
}