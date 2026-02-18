////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.AbbreviationsManagement
{
    /// <summary>
    /// Interface for AbbreviationsManager to support dependency injection.
    /// Manages list of abbreviations.
    /// </summary>
    public interface IAbbreviationsManager : IDisposable
    {
        /// <summary>
        /// Gets or sets the list of abbreviations
        /// </summary>
        Abbreviations Abbreviations { get; }

        /// <summary>
        /// Loads abbreviations from the abbreviations file
        /// </summary>
        /// <param name="abbreviationsFile">optional abbreviations file path</param>
        /// <returns>true on success</returns>
        bool Init(String abbreviationsFile = null);
    }
}
