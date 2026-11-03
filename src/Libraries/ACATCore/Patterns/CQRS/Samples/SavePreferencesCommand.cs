////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SavePreferencesCommand.cs
//
// Sample CQRS command that encapsulates a request to save an XML
// preferences object to a file path asynchronously.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Command that requests an XML preferences object of type
    /// <typeparamref name="T"/> be saved asynchronously to the supplied file path.
    /// </summary>
    /// <typeparam name="T">Preferences type (XML-serializable reference type).</typeparam>
    public class SavePreferencesCommand<T> : ICommand where T : class, new()
    {
        /// <summary>
        /// Gets the preferences object to save.
        /// </summary>
        public T Preferences { get; }

        /// <summary>
        /// Gets the full path of the file to save to.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Initialises a new <see cref="SavePreferencesCommand{T}"/>.
        /// </summary>
        /// <param name="preferences">The preferences object to save.</param>
        /// <param name="filePath">Full path of the file to write.</param>
        public SavePreferencesCommand(T preferences, string filePath)
        {
            Preferences = preferences;
            FilePath = filePath;
        }
    }
}
