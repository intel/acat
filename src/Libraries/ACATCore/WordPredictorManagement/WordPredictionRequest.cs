////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.WordPredictorManagement.Interfaces;

namespace ACAT.Core.WordPredictorManagement
{
    public enum PredictionTypes
    {
        Words,
        Sentences
    }

    /// <summary>
    /// Represents a request for async word prediction
    /// </summary>
    public class WordPredictionRequest
    {
        public WordPredictionRequest(string prevWords, string currentWord, PredictionTypes predictionType, WordPredictionModes mode)
        {
            PrevWords = prevWords;
            CurrentWord = currentWord;
            PredictionType = predictionType;
            WordPredictionMode = mode;
        }

        public string CurrentWord { get; }
        public PredictionTypes PredictionType { get; }
        public string PrevWords { get; }
        public WordPredictionModes WordPredictionMode { get; }
    }
}