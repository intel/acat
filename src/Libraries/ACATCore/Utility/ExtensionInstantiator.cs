using ACAT.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Helper class for creating extension instances with proper dependency injection
    /// </summary>
    public static class ExtensionInstantiator
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
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (extensionTypes == null)
                return Enumerable.Empty<IExtension>();

            var extensions = new List<IExtension>();

            foreach (var type in extensionTypes)
            {
                try
                {
                    logger?.LogDebug("Creating extension instance for {TypeName}", type.FullName);
                    
                    // Use ActivatorUtilities to create instances with proper dependency injection
                    // This automatically resolves ILogger<T> and other registered services
                    var instance = ActivatorUtilities.CreateInstance(serviceProvider, type);
                    
                    if (instance is IExtension extension)
                    {
                        extensions.Add(extension);
                        logger?.LogInformation("Successfully loaded extension: {ExtensionName}", type.Name);
                    }
                    else
                    {
                        logger?.LogWarning("Type {TypeName} does not implement IExtension interface", type.FullName);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Failed to create instance of {TypeName}. This extension will be skipped.", type.FullName);
                }
            }

            return extensions;
        }

        /// <summary>
        /// Creates a single extension instance using dependency injection
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency resolution</param>
        /// <param name="extensionType">The extension type to instantiate</param>
        /// <param name="logger">Optional logger for diagnostics</param>
        /// <returns>The created extension instance, or null if creation fails</returns>
        public static object CreateExtensionInstance(
            IServiceProvider serviceProvider,
            Type extensionType,
            ILogger logger = null)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (extensionType == null)
                return null;

            try
            {
                logger?.LogDebug("Creating extension instance for {TypeName}", extensionType.FullName);
                var instance = ActivatorUtilities.CreateInstance(serviceProvider, extensionType);
                
                if (instance != null)
                {
                    logger?.LogInformation("Successfully loaded extension: {ExtensionName}", extensionType.Name);
                    return instance;
                }
                
                logger?.LogWarning("Failed to create instance of type {TypeName}", extensionType.FullName);
                return null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to create instance of {TypeName}", extensionType.FullName);
                return null;
            }
        }
    }
}
