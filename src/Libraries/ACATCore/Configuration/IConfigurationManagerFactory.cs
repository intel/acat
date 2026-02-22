////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Factory interface for creating IConfigurationManager instances.
    /// Provides abstraction for manager creation to support testing and
    /// dependency injection.
    /// </summary>
    public interface IConfigurationManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the IConfigurationManager instance
        /// </summary>
        /// <returns>The IConfigurationManager instance</returns>
        IConfigurationManager Create();
    }

    /// <summary>
    /// Default factory implementation that returns the <see cref="EnvironmentConfiguration"/>
    /// instance provided at construction time.
    /// </summary>
    /// <remarks>
    /// When used with the Microsoft DI container (see
    /// <c>ServiceCollectionExtensions.AddACATConfiguration</c>), the container
    /// injects the registered singleton so that <see cref="Create"/> always
    /// returns that same instance.
    /// </remarks>
    public class ConfigurationManagerFactory : IConfigurationManagerFactory
    {
        private readonly IConfigurationManager _instance;

        /// <summary>
        /// Initialises the factory with the configuration manager instance to vend.
        /// </summary>
        /// <param name="instance">The <see cref="IConfigurationManager"/> to return from <see cref="Create"/>.</param>
        public ConfigurationManagerFactory(IConfigurationManager instance)
        {
            _instance = instance ?? throw new System.ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// Returns the <see cref="IConfigurationManager"/> instance provided at construction time.
        /// </summary>
        /// <returns>The IConfigurationManager instance</returns>
        public IConfigurationManager Create()
        {
            return _instance;
        }
    }
}
