////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Core.WordPredictorManagement
{
    /// <summary>
    /// Represents a response for a word prediction request.
    /// </summary>
    public class WordPredictionResponse
    {
        public WordPredictionResponse(WordPredictionRequest request, IEnumerable<string> results, bool success)
        {
            Results = results;
            Success = success;
            Request = request;
        }

        public WordPredictionRequest Request { get; }
        public IEnumerable<string> Results { get; }

        public bool Success { get; }
    }
}