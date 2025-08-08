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
        public BCIModes BciMode;

        public BCIScanSections BciCalibrationMode;

        public int ScanTime;

        public int NumTargets;

        public int NumIterationsPerTarget;

        public int MinScoreRequired;

        public BCIUserInputParameters()
        {
            BciMode = BCIModes.UNKNOWN;
            BciCalibrationMode = BCIScanSections.None;
            NumTargets = 0;
            NumIterationsPerTarget = 0;
            MinScoreRequired = 0;
            ScanTime = 0;
        }

        public BCIUserInputParameters(BCIModes bciMode, BCIScanSections bciCalibrationMode, int scanTime, int numTargets, int numIterationsPerTarget, int minScoreRequired)
        {
            BciMode = bciMode;
            BciCalibrationMode = bciCalibrationMode;
            NumTargets = numTargets;
            NumIterationsPerTarget = numIterationsPerTarget;
            ScanTime = scanTime;
            MinScoreRequired = minScoreRequired;
        }
    }
}