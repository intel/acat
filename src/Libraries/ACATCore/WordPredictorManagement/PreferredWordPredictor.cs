////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.WordPredictorManagement
{
    /// <summary>
    /// Stores the mapping between a language
    /// and the word predictor for it
    /// </summary>
    [Serializable]
    public class PreferredWordPredictor
    {
        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        public PreferredWordPredictor()
        {
            ID = Guid.Empty;
            Language = String.Empty;
        }

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="guid">Guid of the word predictor</param>
        /// <param name="language">Language (culture)</param>
        public PreferredWordPredictor(Guid guid, String language)
        {
            ID = guid;
            Language = language;
        }

        /// <summary>
        /// Gets or sets the guid of the word predictor
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Get or sets the language
        /// </summary>
        public String Language { get; set; }
    }
}