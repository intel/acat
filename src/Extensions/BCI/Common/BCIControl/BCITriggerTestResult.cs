////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCITriggerTestResult.cs
//
// Result of the triggertest sent from the actuator to ACAT
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCITriggerTestResult
    {
        /// <summary>
        /// list of duty cycles, one for each pulse
        /// </summary>
        public List<double> DutyCycleList { get; set; }

        /// <summary>
        /// avg duty cycles for all pulses
        /// </summary>
        public double DutyCycleAvg { get; set; }

        /// <summary>
        /// Fail/Success of trigertest
        /// </summary>
        public bool TriggerTestSuccess { get; set; }

        public BCITriggerTestResult()
        {
            DutyCycleList = new List<double>();
        }
    }
}