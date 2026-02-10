////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PronunciationsJson.cs
//
// JSON POCO for pronunciations configuration.
// Represents the structure of Pronunciations.json file.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Root configuration object for pronunciations
    /// </summary>
    public class PronunciationsJson
    {
        /// <summary>
        /// List of pronunciation entries
        /// </summary>
        [JsonPropertyName("pronunciations")]
        public List<PronunciationJson> Pronunciations { get; set; } = new List<PronunciationJson>();

        /// <summary>
        /// Creates a default pronunciations configuration
        /// </summary>
        public static PronunciationsJson CreateDefault()
        {
            return new PronunciationsJson();
        }
    }

    /// <summary>
    /// Represents a single pronunciation entry
    /// </summary>
    public class PronunciationJson
    {
        /// <summary>
        /// The original word
        /// </summary>
        [JsonPropertyName("word")]
        public string Word { get; set; } = string.Empty;

        /// <summary>
        /// The phonetic pronunciation
        /// </summary>
        [JsonPropertyName("pronunciation")]
        public string Pronunciation { get; set; } = string.Empty;
    }
}
