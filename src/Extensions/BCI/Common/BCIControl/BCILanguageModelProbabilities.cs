////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILanguageModelProbabilities.cs
//
// Parameters sent from ACAT corresponding to the Language Model
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public enum ProbabilityType
    {
        NextCharacterProbabilities,
        NextWordProbabilities,
        NextSentenceProbabilities,
        Other
    }

    public class BCILanguageModelProbabilities
    {
        public BCILanguageModelProbabilities()
        {
            LanguageModelProbabilities = new Dictionary<int, double>();
            LanguageModelProbabilityType = ProbabilityType.Other;
        }

        /// <summary>
        /// Languae model probabilities
        /// Format: [ID of corresponding button, probability]
        /// </summary>
        public Dictionary<int, double> LanguageModelProbabilities { get; set; }

        /// <summary>
        /// Type of probability.
        /// Supported types: NextCharacter, NextWord, NextSentence, Other
        /// </summary>
        public ProbabilityType LanguageModelProbabilityType { get; set; }
    }
}