////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCICalibrationEyesClosedParameters.cs
//
// Parameters sent from ACAT when calibration ends
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCITriggerTestParameters
    {
        /// <summary>
        /// number of repetitions. One repeition is considered as eyes open -> eyes closed
        /// </summary>
        public int NumRepetitions;

        /// <summary>
        /// Duration where eyes are open or closed
        /// </summary>
        public int ScanTime;

        public BCITriggerTestParameters(int numRepetitions, int scanTime)
        {
            NumRepetitions = numRepetitions;
            ScanTime = scanTime;
        }
    }
}