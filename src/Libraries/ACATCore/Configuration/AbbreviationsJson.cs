////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AbbreviationsJson.cs
//
// JSON POCO for abbreviations configuration.
// Represents the structure of Abbreviations.json file.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Root configuration object for abbreviations
    /// </summary>
    public class AbbreviationsJson
    {
        /// <summary>
        /// List of abbreviation entries
        /// </summary>
        [JsonPropertyName("abbreviations")]
        public List<AbbreviationJson> Abbreviations { get; set; } = new List<AbbreviationJson>();

        /// <summary>
        /// Creates a default abbreviations configuration
        /// </summary>
        public static AbbreviationsJson CreateDefault()
        {
            return new AbbreviationsJson();
        }
    }

    /// <summary>
    /// Represents a single abbreviation entry
    /// </summary>
    public class AbbreviationJson
    {
        /// <summary>
        /// The abbreviation mnemonic (e.g., "btw")
        /// </summary>
        [JsonPropertyName("word")]
        public string Word { get; set; } = string.Empty;

        /// <summary>
        /// The expansion text (e.g., "by the way")
        /// </summary>
        [JsonPropertyName("replaceWith")]
        public string ReplaceWith { get; set; } = string.Empty;

        /// <summary>
        /// The mode of expansion: "Write" or "Speak"
        /// </summary>
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "Write";
    }
}
