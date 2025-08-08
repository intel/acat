////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCICalibrationInput.cs
//
// Parameters sent from ACAT after every repetition in calibration
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCICalibrationInput
    {
        public BCICalibrationInput()
        {
            RowColumnIDs = new List<int>();
        }

        public BCICalibrationInput(IEnumerable<int> rowColumnIDs)
        {
            RowColumnIDs = new List<int>(rowColumnIDs);
        }

        /// <summary>
        /// List of IDs of row/columns highlighted
        /// </summary>
        public List<int> RowColumnIDs { get; set; }
    }
}