////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// GetConfigurationValueQuery.cs
//
// Sample CQRS query that retrieves a named configuration override value
// from the environment configuration.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Query that requests the value of a named configuration key.
    /// Returns <c>null</c> when the key is not present.
    /// </summary>
    public class GetConfigurationValueQuery : IQuery<string>
    {
        /// <summary>
        /// Gets the configuration key whose value is requested.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Initialises a new <see cref="GetConfigurationValueQuery"/>.
        /// </summary>
        /// <param name="key">The configuration key to look up.</param>
        public GetConfigurationValueQuery(string key)
        {
            Key = key;
        }
    }
}
