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
using ACAT.Core.EventManagement;
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
        /// Registers both interface and concrete types for all managers
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        /// <returns>The configured service collection for chaining</returns>
        public static IServiceCollection AddACATServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Register all managers as singletons (matching existing behavior)
            // Each manager is registered as both its interface and concrete type
            // This allows dependency injection via interface while maintaining backward compatibility
            
            // ActuatorManager
            services.AddSingleton<ActuatorManager>(provider => ActuatorManager.Instance);
            services.AddSingleton<IActuatorManager>(provider => provider.GetRequiredService<ActuatorManager>());
            services.AddSingleton<IActuatorManagerFactory, ActuatorManagerFactory>();

            // AgentManager
            services.AddSingleton<AgentManager>(provider => AgentManager.Instance);
            services.AddSingleton<IAgentManager>(provider => provider.GetRequiredService<AgentManager>());
            services.AddSingleton<IAgentManagerFactory, AgentManagerFactory>();

            // TTSManager
            services.AddSingleton<TTSManager>(provider => TTSManager.Instance);
            services.AddSingleton<ITTSManager>(provider => provider.GetRequiredService<TTSManager>());
            services.AddSingleton<ITTSManagerFactory, TTSManagerFactory>();

            // PanelManager
            services.AddSingleton<PanelManager>(provider => PanelManager.Instance);
            services.AddSingleton<IPanelManager>(provider => provider.GetRequiredService<PanelManager>());
            services.AddSingleton<IPanelManagerFactory, PanelManagerFactory>();

            // ThemeManager
            services.AddSingleton<ThemeManager>(provider => ThemeManager.Instance);
            services.AddSingleton<IThemeManager>(provider => provider.GetRequiredService<ThemeManager>());
            services.AddSingleton<IThemeManagerFactory, ThemeManagerFactory>();

            // WordPredictionManager
            services.AddSingleton<WordPredictionManager>(provider => WordPredictionManager.Instance);
            services.AddSingleton<IWordPredictionManager>(provider => provider.GetRequiredService<WordPredictionManager>());
            services.AddSingleton<IWordPredictionManagerFactory, WordPredictionManagerFactory>();

            // SpellCheckManager
            services.AddSingleton<SpellCheckManager>(provider => SpellCheckManager.Instance);
            services.AddSingleton<ISpellCheckManager>(provider => provider.GetRequiredService<SpellCheckManager>());
            services.AddSingleton<ISpellCheckManagerFactory, SpellCheckManagerFactory>();

            // AbbreviationsManager
            services.AddSingleton<AbbreviationsManager>(provider => AbbreviationsManager.Instance);
            services.AddSingleton<IAbbreviationsManager>(provider => provider.GetRequiredService<AbbreviationsManager>());
            services.AddSingleton<IAbbreviationsManagerFactory, AbbreviationsManagerFactory>();

            // CommandManager
            services.AddSingleton<CommandManager>(provider => CommandManager.Instance);
            services.AddSingleton<ICommandManager>(provider => provider.GetRequiredService<CommandManager>());
            services.AddSingleton<ICommandManagerFactory, CommandManagerFactory>();

            // AutomationEventManager
            services.AddSingleton<AutomationEventManager>(provider => AutomationEventManager.Instance);
            services.AddSingleton<IAutomationEventManager>(provider => provider.GetRequiredService<AutomationEventManager>());
            services.AddSingleton<IAutomationEventManagerFactory, AutomationEventManagerFactory>();

            // EventBus
            services.AddSingleton<IEventBus, EventBus>();

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
