////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.WordPredictionManagement
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
        public WordPredictionRequest(String prevWords, String currentWord, PredictionTypes predictionType, WordPredictionModes mode)
        {
            PrevWords = prevWords;
            CurrentWord = currentWord;
            PredictionType = predictionType;
            WordPredictionMode = mode;
        }

        public String CurrentWord { get; }
        public PredictionTypes PredictionType { get; }
        public String PrevWords { get; }
        public WordPredictionModes WordPredictionMode { get; }
    }
}