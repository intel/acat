using ACAT.Core.Extensions;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Applications
{
    /// <summary>
    /// Helper class for creating extension instances with proper dependency injection
    /// Wrapper around ExtensionInstantiator from ACAT.Core for backward compatibility
    /// </summary>
    public static class ExtensionHelper
    {
        /// <summary>
        /// Creates instances of extension types using dependency injection.
        /// This ensures extensions receive proper logger instances and other registered services.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency resolution</param>
        /// <param name="extensionTypes">Collection of extension types to instantiate</param>
        /// <param name="logger">Optional logger for diagnostics</param>
        /// <returns>Collection of successfully created extension instances</returns>
        public static IEnumerable<IExtension> CreateExtensionInstances(
            IServiceProvider serviceProvider,
            IEnumerable<Type> extensionTypes,
            ILogger logger = null)
        {
            return ExtensionInstantiator.CreateExtensionInstances(serviceProvider, extensionTypes, logger);
        }

        /// <summary>
        /// Creates a single extension instance using dependency injection
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency resolution</param>
        /// <param name="extensionType">The extension type to instantiate</param>
        /// <param name="logger">Optional logger for diagnostics</param>
        /// <returns>The created extension instance, or null if creation fails</returns>
        public static IExtension CreateExtensionInstance(
            IServiceProvider serviceProvider,
            Type extensionType,
            ILogger logger = null)
        {
            return ExtensionInstantiator.CreateExtensionInstance(serviceProvider, extensionType, logger) as IExtension;
        }
    }
}
