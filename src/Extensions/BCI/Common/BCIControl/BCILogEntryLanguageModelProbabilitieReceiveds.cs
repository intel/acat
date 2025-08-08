////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILogEntryLanguageModelProbabilitiesReceived.cs
//
// Auditlog entry when language model probabilities are received from ACAT
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCILogEntryLanguageModelProbabilitiesReceived
    {
        public BCILogEntryLanguageModelProbabilitiesReceived()
        {
            LanguageModelProbabilityType = "";
            LanguageModelProbabilitiesEnabled = false;
            LanguageModelProbabilities = new Dictionary<int, double>();
        }

        public BCILogEntryLanguageModelProbabilitiesReceived(String languageModelProbabilityType, Dictionary<int, double> languageModelProbabilities, bool languageModelProbabilitiesEnabled)
        {
            LanguageModelProbabilityType = languageModelProbabilityType;
            LanguageModelProbabilitiesEnabled = languageModelProbabilitiesEnabled;
            LanguageModelProbabilities = languageModelProbabilities;
        }

        /// <summary>
        /// Boolean, true if LM probabilities enabled
        /// </summary>
        public bool LanguageModelProbabilitiesEnabled { get; set; }

        /// <summary>
        /// Type of LM probability.
        /// </summary>
        public String LanguageModelProbabilityType { get; set; }

        /// <summary>
        /// Language model probabilities in the format: [ID, probability]
        /// </summary>
        public Dictionary<int, double> LanguageModelProbabilities { get; set; }
    }
}