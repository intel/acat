////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CalibrationParametersForSection.cs
//
// Parameters for a particular Calibration sent to ACAT
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class CalibrationParametersForSection
    {
        public BCIScanSections CalibrationMode { get; set; }

        public int ScanTime { get; set; }

        public int TargetCount { get; set; }

        public int IterationsPerTarget { get; set; }

        public bool UseRandomTargetsFlag { get; set; }

        public String Sequence { get; set; }

        public int MinimumScoreRequired { get; set; }

        public CalibrationParametersForSection(BCIScanSections calibrationMode, int scanTime, int targetCount, int iterationsPerTarget, int minimumScoreRequired, bool useRandomTargetsFlag = true, String sequence = null)
        {
            CalibrationMode = calibrationMode;
            ScanTime = scanTime;
            TargetCount = targetCount;
            IterationsPerTarget = iterationsPerTarget;
            UseRandomTargetsFlag = useRandomTargetsFlag;
            Sequence = sequence;
            MinimumScoreRequired = minimumScoreRequired;
        }
    }
}