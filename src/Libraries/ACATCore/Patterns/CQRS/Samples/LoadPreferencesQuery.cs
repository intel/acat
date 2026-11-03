////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// LoadPreferencesQuery.cs
//
// Sample CQRS query that requests an XML preferences object be loaded
// asynchronously from a file path.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Query that requests an XML preferences object of type
    /// <typeparamref name="T"/> be loaded from the supplied file path.
    /// </summary>
    /// <typeparam name="T">Preferences type (XML-serializable reference type).</typeparam>
    public class LoadPreferencesQuery<T> : IQuery<T> where T : class, new()
    {
        /// <summary>
        /// Gets the full path to the preferences file to load.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Initialises a new <see cref="LoadPreferencesQuery{T}"/>.
        /// </summary>
        /// <param name="filePath">Full path to the preferences file.</param>
        public LoadPreferencesQuery(string filePath)
        {
            FilePath = filePath;
        }
    }
}
