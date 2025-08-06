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
        public BCIUserInputParameters() { }

        public BCIModes BciMode { get; set; } = BCIModes.UNKNOWN;

        public BCIScanSections BciCalibrationMode { get; set; } = BCIScanSections.None;

        public int ScanTime { get; set; } = 0;

        public int NumTargets { get; set; } = 0;

        public int NumIterationsPerTarget { get; set; } = 0;

        public int MinScoreRequired { get; set; } = 0;
    }
}