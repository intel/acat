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
        public BCIScanSections CalibrationMode { get; set; } = BCIScanSections.None;

        public int ScanTime { get; set; } = 0;

        public int TargetCount { get; set; } = 0;

        public int IterationsPerTarget { get; set; } = 0;

        public bool UseRandomTargetsFlag { get; set; } = 0;

        public String Sequence { get; set; } = "";

        public int MinimumScoreRequired { get; set; } = 0;
    }
}