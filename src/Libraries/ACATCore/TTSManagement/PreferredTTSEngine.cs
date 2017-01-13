////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferredTTSEngine.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
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