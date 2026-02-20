////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// GetConfigurationValueQueryHandler.cs
//
// Sample CQRS query handler that reads a named override value from
// EnvironmentConfiguration without causing any side effects.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Handles <see cref="GetConfigurationValueQuery"/> by reading the
    /// requested key from <see cref="EnvironmentConfiguration"/>.
    /// Returns <c>null</c> when the key is not present.
    /// </summary>
    public class GetConfigurationValueQueryHandler
        : IQueryHandler<GetConfigurationValueQuery, string>
    {
        private readonly EnvironmentConfiguration _configuration;

        /// <summary>
        /// Initialises a new <see cref="GetConfigurationValueQueryHandler"/>.
        /// </summary>
        /// <param name="configuration">The environment configuration to query.</param>
        public GetConfigurationValueQueryHandler(EnvironmentConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public string Handle(GetConfigurationValueQuery query)
        {
            return _configuration.GetOverride(query.Key);
        }
    }
}
