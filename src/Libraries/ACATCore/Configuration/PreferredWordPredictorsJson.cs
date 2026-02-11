////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferredWordPredictorsJson.cs
//
// JSON-serializable POCO classes for preferred word predictors configuration
// with System.Text.Json attributes for modern JSON serialization
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Root configuration for preferred word predictors in ACAT
    /// Maps languages/cultures to their preferred word predictor
    /// </summary>
    public class PreferredWordPredictorsJson
    {
        /// <summary>
        /// List of preferred word predictors for different languages
        /// </summary>
        [JsonPropertyName("wordPredictors")]
        [Required]
        public List<PreferredWordPredictorJson> WordPredictors { get; set; } = new();

        /// <summary>
        /// Factory method to create default empty configuration
        /// </summary>
        public static PreferredWordPredictorsJson CreateDefault()
        {
            return new PreferredWordPredictorsJson
            {
                WordPredictors = new List<PreferredWordPredictorJson>()
            };
        }
    }

    /// <summary>
    /// Configuration mapping a language to its preferred word predictor
    /// </summary>
    public class PreferredWordPredictorJson
    {
        /// <summary>
        /// Language code (e.g., "en", "fr", "es")
        /// </summary>
        [JsonPropertyName("language")]
        [Required(ErrorMessage = "Language is required")]
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// GUID of the preferred word predictor for this language
        /// </summary>
        [JsonPropertyName("id")]
        [Required(ErrorMessage = "Word predictor ID is required")]
        public string Id { get; set; } = Guid.Empty.ToString();
    }
}
