////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EEGProcessingGlobals.cs
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public class EEGProcessingGlobals
    {
       
        /// <summary>
        /// Decision maker object
        /// </summary>
        public static Dictionary<BCIScanSections, DecisionMaker> DecisionMakerDict { get; set; }

        /// <summary>
        /// Restarts all probabilities
        /// </summary>
        /// <returns></returns>
        public static bool RestartAllDecisionMakerProbabilities()
        {
            if (DecisionMakerDict != null)
            {
                foreach (var item in DecisionMakerDict)
                    item.Value.RestartProbabilities();
            }
            return true;
        }
    }
}