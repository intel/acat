////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Stores the mapping between a language
    /// and the TTS Engine for it
    /// </summary>
    [Serializable]
    public class PreferredTTSEngine
    {
        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        public PreferredTTSEngine()
        {
            ID = Guid.Empty;
            Language = String.Empty;
        }

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="guid">Guid of the Engine</param>
        /// <param name="language">Language (culture)</param>
        public PreferredTTSEngine(Guid guid, String language)
        {
            ID = guid;
            Language = language;
        }

        /// <summary>
        /// Gets or sets the guid of the TTS Engine
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Get or sets the language
        /// </summary>
        public String Language { get; set; }
    }
}