////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCISimpleParameters.cs
//
// Parameters to define the amount of repetitions for calibration and the scann time
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public class BCISimpleParameters
    {
        /// <summary>
        /// Number of iterations per target
        /// </summary>
        public int IterationsPertarget;

        /// <summary>
        ///  Minimum score to pass calibration
        /// </summary>
        public int MinScore;

        /// <summary>
        /// Scanning time
        /// </summary>
        public int ScannTime;

        /// <summary>
        /// Number of targets
        /// </summary>
        public int Targets;

        public BCISimpleParameters()
        {
            Targets = 0;
            ScannTime = 0;
            IterationsPertarget = 0;
            MinScore = 0;
        }

        public BCISimpleParameters(int repetitions, int scannTime)
        {
            Targets = repetitions;
            ScannTime = scannTime;
        }
    }
}