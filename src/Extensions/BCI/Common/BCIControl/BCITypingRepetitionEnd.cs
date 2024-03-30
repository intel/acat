////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCITypingRepetitionEnd.cs
//
// Parameters sent from ACAT to BCIActuator after each typing repetition
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCITypingRepetitionEnd
    {
        public BCITypingRepetitionEnd()
        {
            RowColumnIDs = new List<int>();
            FlashingSequence = new Dictionary<int, int[]>();
            ButtonTextValues = new Dictionary<int, String>();
            ScanningSection = BCIScanSections.None;
        }

        /// <summary>
        /// Lookup table with ID and value of each button
        /// Format: [ID of button, value]
        /// </summary>
        public Dictionary<int, String> ButtonTextValues { get; set; }

        /// <summary>
        /// IDs of the highlighted sequences in the format
        /// Format: [ID of row/column, IDs of buttons highlighted in the row/column]
        /// </summary>
        public Dictionary<int, int[]> FlashingSequence { get; set; }

        /// <summary>
        /// List of IDs of row/columns highlighted
        /// </summary>
        public List<int> RowColumnIDs { get; set; }

        /// <summary>
        /// Corresponding scanning section
        /// Options: "Boxes" "Words" "Sentences" "Keyboard" "Menus"
        /// </summary>
        public BCIScanSections ScanningSection { get; set; }
    }
}