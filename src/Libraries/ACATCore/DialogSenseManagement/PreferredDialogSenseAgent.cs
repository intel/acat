////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    /// <summary>
    /// Stores the mapping between a language
    /// and the spellchecker for it
    /// </summary>
    [Serializable]
    public class PreferredDialogSenseAgent
    {
        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        public PreferredDialogSenseAgent()
        {
            ID = Guid.Empty;
            Language = String.Empty;
        }

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="guid">Guid of the spellchecker</param>
        /// <param name="language">Language (culture)</param>
        public PreferredDialogSenseAgent(Guid guid, String language)
        {
            ID = guid;
            Language = language;
        }

        /// <summary>
        /// Gets or sets the guid of the spellchecker
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Get or sets the language
        /// </summary>
        public String Language { get; set; }
    }
}