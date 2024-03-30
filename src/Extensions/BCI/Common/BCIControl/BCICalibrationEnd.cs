////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCICalibrationEnd.cs
//
// Parameters sent from ACAT when calibration ends
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCICalibrationEnd
    {
        /// <summary>
        /// IDs of the highlighted sequences in the format
        /// Format: [ID of row/column, IDs of buttons highlighted in the row/column]
        /// </summary>
        public Dictionary<int, int[]> FlashingSequence { get; set; }

        /// <summary>
        /// Boolean, true to discard calibration data
        /// </summary>
        public bool DiscardCalibrationData { get; set; }

        public BCICalibrationEnd()
        {
            FlashingSequence = new Dictionary<int, int[]>();
            DiscardCalibrationData = false;
        }

        public BCICalibrationEnd(Dictionary<int, int[]> flashingSequence, bool discardCalibrationData)
        {
            FlashingSequence = flashingSequence;
            DiscardCalibrationData = discardCalibrationData;
        }
    }
}