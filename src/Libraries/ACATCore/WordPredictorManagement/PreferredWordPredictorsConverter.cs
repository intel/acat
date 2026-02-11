////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferredWordPredictorsConverter.cs
//
// Converts between legacy PreferredWordPredictors XML model and 
// new PreferredWordPredictorsJson model
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Core.WordPredictorManagement
{
    /// <summary>
    /// Converts between legacy XML-based PreferredWordPredictors model
    /// and new JSON-based configuration model
    /// </summary>
    public static class PreferredWordPredictorsConverter
    {
        /// <summary>
        /// Converts JSON model to legacy model
        /// </summary>
        /// <param name="jsonConfig">JSON configuration</param>
        /// <returns>List of PreferredWordPredictor objects</returns>
        public static List<PreferredWordPredictor> FromJson(PreferredWordPredictorsJson jsonConfig)
        {
            if (jsonConfig == null)
            {
                return new List<PreferredWordPredictor>();
            }

            var result = new List<PreferredWordPredictor>();

            foreach (var jsonItem in jsonConfig.WordPredictors)
            {
                if (Guid.TryParse(jsonItem.Id, out Guid guid))
                {
                    result.Add(new PreferredWordPredictor(guid, jsonItem.Language));
                }
            }

            return result;
        }

        /// <summary>
        /// Converts legacy model to JSON model
        /// </summary>
        /// <param name="wordPredictors">List of PreferredWordPredictor objects</param>
        /// <returns>JSON configuration</returns>
        public static PreferredWordPredictorsJson ToJson(List<PreferredWordPredictor> wordPredictors)
        {
            if (wordPredictors == null)
            {
                return PreferredWordPredictorsJson.CreateDefault();
            }

            var jsonConfig = new PreferredWordPredictorsJson();

            foreach (var item in wordPredictors)
            {
                jsonConfig.WordPredictors.Add(new PreferredWordPredictorJson
                {
                    Language = item.Language ?? string.Empty,
                    Id = item.ID.ToString()
                });
            }

            return jsonConfig;
        }
    }
}
