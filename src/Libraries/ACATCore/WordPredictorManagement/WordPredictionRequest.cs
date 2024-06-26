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
        Sentences,
        Keywords,
        CRGResponses
    }

    /// <summary>
    /// Represents a request for async word prediction 
    /// </summary>
    public class WordPredictionRequest
    {
        public WordPredictionRequest(String context, PredictionTypes predictionType, WordPredictionModes mode, bool crg = false, String keyword = null)
        {
            Context = context;
            PredictionType = predictionType;
            WordPredictionMode = mode;
            Keyword = keyword;
            CRG = crg;
        }

        public String Context { get; }
        public PredictionTypes PredictionType { get; }
        public WordPredictionModes WordPredictionMode { get; }
        
        public String Keyword { get; }

        public bool CRG { get;  }


    }
}