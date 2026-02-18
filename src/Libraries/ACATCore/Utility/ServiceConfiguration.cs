////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.CommandManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.SpellCheckManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.WordPredictorManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Configures Microsoft.Extensions.DependencyInjection for ACAT core services
    /// Provides service registration for all managers and core components
    /// Supports Singleton, Scoped, and Transient lifetime management
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Adds ACAT core services to the service collection with proper dependency injection
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        /// <returns>The configured service collection for chaining</returns>
        public static IServiceCollection AddACATServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Register all managers as singletons (matching existing behavior)
            // Managers are registered as their concrete types since interfaces will be added in Task 2
            services.AddSingleton<ActuatorManager>(provider =>
            {
                var logger = provider.GetService<ILogger<ActuatorManager>>();
                return ActuatorManager.Instance;
            });

            services.AddSingleton<AgentManager>(provider =>
            {
                var logger = provider.GetService<ILogger<AgentManager>>();
                return AgentManager.Instance;
            });

            services.AddSingleton<TTSManager>(provider =>
            {
                var logger = provider.GetService<ILogger<TTSManager>>();
                return TTSManager.Instance;
            });

            services.AddSingleton<PanelManager>(provider =>
            {
                var logger = provider.GetService<ILogger<PanelManager>>();
                return PanelManager.Instance;
            });

            services.AddSingleton<ThemeManager>(provider =>
            {
                var logger = provider.GetService<ILogger<ThemeManager>>();
                return ThemeManager.Instance;
            });

            services.AddSingleton<WordPredictionManager>(provider =>
            {
                var logger = provider.GetService<ILogger<WordPredictionManager>>();
                return WordPredictionManager.Instance;
            });

            services.AddSingleton<SpellCheckManager>(provider =>
            {
                var logger = provider.GetService<ILogger<SpellCheckManager>>();
                return SpellCheckManager.Instance;
            });

            services.AddSingleton<AbbreviationsManager>(provider =>
            {
                var logger = provider.GetService<ILogger<AbbreviationsManager>>();
                return AbbreviationsManager.Instance;
            });

            services.AddSingleton<CommandManager>(provider =>
            {
                var logger = provider.GetService<ILogger<CommandManager>>();
                return CommandManager.Instance;
            });

            services.AddSingleton<AutomationEventManager>(provider =>
            {
                var logger = provider.GetService<ILogger<AutomationEventManager>>();
                return AutomationEventManager.Instance;
            });

            return services;
        }

        /// <summary>
        /// Adds complete ACAT infrastructure including logging and core services
        /// This is a convenience method that combines logging and service configuration
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        /// <returns>The configured service collection for chaining</returns>
        public static IServiceCollection AddACATInfrastructure(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Add logging infrastructure
            services.AddACATLogging();

            // Add core services
            services.AddACATServices();

            return services;
        }

        /// <summary>
        /// Creates a fully configured service provider with ACAT infrastructure
        /// This is useful for applications that need a quick setup
        /// </summary>
        /// <returns>Configured service provider</returns>
        public static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddACATInfrastructure();
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Creates a fully configured service provider with custom logger factory
        /// This allows applications to provide their own logging configuration
        /// </summary>
        /// <param name="loggerFactory">Custom logger factory to use</param>
        /// <returns>Configured service provider</returns>
        public static IServiceProvider CreateServiceProvider(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            var services = new ServiceCollection();
            services.AddSingleton(loggerFactory);
            services.AddLogging();
            services.AddACATServices();
            return services.BuildServiceProvider();
        }
    }
}
