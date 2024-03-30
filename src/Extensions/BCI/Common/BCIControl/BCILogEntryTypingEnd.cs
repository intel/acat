////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILogEntryTypingEnd.cs
//
// Auditlog entry after each typing repetition
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCILogEntryTypingEnd
    {
        public BCILogEntryTypingEnd()
        {
            DecidedFlag = false;
            SelectedButtonLabel = string.Empty;
            SelectedButtonID = 0;
            RepetitionIdx = 0;
            RowColumnIDs = new List<int>();
            NextCharacterProbabilities = new Dictionary<int, double>();
            EegProbabilities = new Dictionary<int, double>();
            PosteriorProbabilities = new Dictionary<int, double>();
            FlashingSequence = new Dictionary<int, int[]>();
            ScanningSection = "";
        }

        public BCILogEntryTypingEnd(string selectedButtonLabel, int selectedButtonID, bool decidedFlag, int repetitionIdx, String scanningSection, List<int> rowColumnIDs, Dictionary<int, int[]> flashingSeq, Dictionary<int, double> nextCharacterProbabilities, Dictionary<int, double> eegProbabilities, Dictionary<int, double> posteriorProbabilities)
        {
            SelectedButtonLabel = selectedButtonLabel;
            SelectedButtonID = selectedButtonID;
            DecidedFlag = decidedFlag;
            RepetitionIdx = repetitionIdx;
            RowColumnIDs = rowColumnIDs;
            FlashingSequence = flashingSeq;
            NextCharacterProbabilities = nextCharacterProbabilities;
            PosteriorProbabilities = posteriorProbabilities;
            EegProbabilities = eegProbabilities;
            ScanningSection = scanningSection;
        }

        /// <summary>
        // sensorError = 1 data not received. Message: "Error, data not received from sensor"
        // sensorError = 2, optical sensor error (# trials != rowcolumnIDs highlighted). Message: "Error from optical sensor"
        // sensorError = 3, calibration error (error while analyzing calibration data)
        // sensorError = 4, data processing error (exception when processing data). Message: "Error processing data"
        // sensorError = 5, sensor not acquiring. Message: "Error, sensor disconnected"
        // sensorError = 6, 3 or more channels KO (go back to signal monitor?)
        /// </summary>
        public int SensorErrorIdx { get; set; }

        /// <summary>
        /// Corresponding scanning section
        /// Options: "Boxes" "Words" "Sentences" "Keyboard" "Menus"
        /// </summary>
        public String ScanningSection { get; set; }

        /// <summary>
        /// Index of the current repetition
        /// </summary>
        public int RepetitionIdx { get; set; }

        /// <summary>
        /// Decided flag. True if a decision has been made by the algorithm
        /// </summary>
        public bool DecidedFlag { get; set; }

        /// <summary>
        /// ID of the selected button label (if a decision has been made)
        /// If no decision has been made, this will be the ID of the button
        /// with maximum probability
        /// </summary>
        public int SelectedButtonID { get; set; }

        // "Boxes" "Words" "Sentences" "Keyboard" "Menus"
        /// <summary>
        /// Label of the selected button label (if a decision has been made)
        /// If no decision has been made, this will be the label of the button
        /// with maximum probability
        /// </summary>
        public String SelectedButtonLabel { get; set; }

        /// <summary>
        /// List of IDs of row/columns highlighted
        /// </summary>
        public List<int> RowColumnIDs { get; set; }

        /// <summary>
        /// IDs of the highlighted sequences in the format
        /// Format: [ID of row/column, IDs of buttons highlighted in the row/column]
        /// </summary>
        public Dictionary<int, int[]> FlashingSequence { get; set; }

        /// <summary>
        /// Language model probabilities
        /// Format: [ID of corresponding button, probability]
        /// </summary>
        public Dictionary<int, double> NextCharacterProbabilities { get; set; }

        /// <summary>
        /// EEG probabilities
        /// Format: [ID of corresponding button, eeg probability]
        /// </summary>
        public Dictionary<int, double> EegProbabilities { get; set; }

        /// <summary>
        /// Posterior probabilities
        /// If languange model is enabled, this fuse the eeg and the LM probability
        /// Format: [ID of corresponding button, posterior probability]
        /// </summary>
        public Dictionary<int, double> PosteriorProbabilities { get; set; }
    }
}