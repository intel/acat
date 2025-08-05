////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIUserInputParameters.cs
//
// Parameters sent from ACAT (updated via form by the user) to the actuator
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public class BCIUserInputParameters
    {
        public BCIModes BciMode { get; set; }

        public BCIScanSections BciCalibrationMode { get; set; }

        public int ScanTime { get; set; }

        public int NumTargets { get; set; }

        public int NumIterationsPerTarget { get; set; }

        public int MinScoreRequired { get; set; }

        public BCIUserInputParameters()
        {
            BciMode = BCIModes.UNKNOWN;
            BciCalibrationMode = BCIScanSections.None;
            NumTargets = 0;
            NumIterationsPerTarget = 0;
            MinScoreRequired = 0;
            ScanTime = 0;
        }
    }
}