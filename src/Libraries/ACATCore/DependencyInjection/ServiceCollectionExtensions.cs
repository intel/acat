////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ServiceCollectionExtensions.cs
//
// Extension methods for registering individual ACAT core modules with the
// Microsoft.Extensions.DependencyInjection service container.
//
// Each module has its own registration method so that host applications can
// opt-in to only the modules they require, or call AddACATServices() to
// register every module at once.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.CommandManagement;
using ACAT.Core.DataAccess;
using ACAT.Core.Diagnostics;
using ACAT.Core.EventManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
using ACAT.Core.SpellCheckManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ACAT.Core.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> that register
    /// individual ACAT core modules.
    /// </summary>
    /// <remarks>
    /// Service registration patterns:
    /// <list type="bullet">
    ///   <item>All managers are Singletons – matching the existing static-singleton behaviour.</item>
    ///   <item>Each manager is registered under both its concrete type and its interface type
    ///         so that callers can resolve either.</item>
    ///   <item>Factory helpers are registered as Singletons for advanced / test scenarios.</item>
    ///   <item>CQRS command / query handlers are Transient because they are stateless.</item>
    /// </list>
    /// </remarks>
    public static class ServiceCollectionExtensions
    {
        // ---------------------------------------------------------------
        // Individual module registration methods
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers the <see cref="ActuatorManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddActuatorManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ActuatorManager>(provider => ActuatorManager.Instance);
            services.AddSingleton<IActuatorManager>(provider => provider.GetRequiredService<ActuatorManager>());
            services.AddSingleton<IActuatorManagerFactory, ActuatorManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="AgentManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddAgentManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<AgentManager>(provider => AgentManager.Instance);
            services.AddSingleton<IAgentManager>(provider => provider.GetRequiredService<AgentManager>());
            services.AddSingleton<IAgentManagerFactory, AgentManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="TTSManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddTTSManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<TTSManager>(provider => TTSManager.Instance);
            services.AddSingleton<ITTSManager>(provider => provider.GetRequiredService<TTSManager>());
            services.AddSingleton<ITTSManagerFactory, TTSManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="PanelManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddPanelManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<PanelManager>(provider => PanelManager.Instance);
            services.AddSingleton<IPanelManager>(provider => provider.GetRequiredService<PanelManager>());
            services.AddSingleton<IPanelManagerFactory, PanelManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="ThemeManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddThemeManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ThemeManager>(provider => ThemeManager.Instance);
            services.AddSingleton<IThemeManager>(provider => provider.GetRequiredService<ThemeManager>());
            services.AddSingleton<IThemeManagerFactory, ThemeManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="WordPredictionManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddWordPrediction(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<WordPredictionManager>(provider => WordPredictionManager.Instance);
            services.AddSingleton<IWordPredictionManager>(provider => provider.GetRequiredService<WordPredictionManager>());
            services.AddSingleton<IWordPredictionManagerFactory, WordPredictionManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="SpellCheckManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddSpellChecking(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<SpellCheckManager>(provider => SpellCheckManager.Instance);
            services.AddSingleton<ISpellCheckManager>(provider => provider.GetRequiredService<SpellCheckManager>());
            services.AddSingleton<ISpellCheckManagerFactory, SpellCheckManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="AbbreviationsManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddAbbreviationsManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<AbbreviationsManager>(provider => AbbreviationsManager.Instance);
            services.AddSingleton<IAbbreviationsManager>(provider => provider.GetRequiredService<AbbreviationsManager>());
            services.AddSingleton<IAbbreviationsManagerFactory, AbbreviationsManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="CommandManager"/> and its related factory.
        /// </summary>
        public static IServiceCollection AddCommandManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<CommandManager>(provider => CommandManager.Instance);
            services.AddSingleton<ICommandManager>(provider => provider.GetRequiredService<CommandManager>());
            services.AddSingleton<ICommandManagerFactory, CommandManagerFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="AutomationEventManager"/>, <see cref="IEventBus"/>,
        /// and their related factory.
        /// </summary>
        public static IServiceCollection AddEventManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<AutomationEventManager>(provider => AutomationEventManager.Instance);
            services.AddSingleton<IAutomationEventManager>(provider => provider.GetRequiredService<AutomationEventManager>());
            services.AddSingleton<IAutomationEventManagerFactory, AutomationEventManagerFactory>();
            services.AddSingleton<IEventBus, EventBus>();
            return services;
        }

        /// <summary>
        /// Registers CQRS command and query handlers.
        /// Handlers are Transient because they are stateless and lightweight.
        /// </summary>
        public static IServiceCollection AddCQRSHandlers(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<ICommandHandler<CreatePanelCommand>, CreatePanelCommandHandler>();
            services.AddTransient<ICommandHandler<HandleActuatorSwitchCommand>, HandleActuatorSwitchCommandHandler>();
            services.AddTransient<IQueryHandler<GetActiveAgentNameQuery, string>, GetActiveAgentNameQueryHandler>();
            services.AddTransient<IQueryHandler<GetConfigurationValueQuery, string>, GetConfigurationValueQueryHandler>();
            return services;
        }

        /// <summary>
        /// Registers data-access repositories.
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IRepository<Theme>, ThemeRepository>();
            return services;
        }

        /// <summary>
        /// Registers diagnostics and monitoring services.
        /// </summary>
        public static IServiceCollection AddDiagnostics(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<PanelActivityMonitor>();
            return services;
        }

        // ---------------------------------------------------------------
        // Convenience aggregate method
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers all ACAT core modules with the service container.
        /// This is equivalent to calling each individual module registration
        /// method in sequence.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddACATCoreModules(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddActuatorManagement()
                .AddAgentManagement()
                .AddTTSManagement()
                .AddPanelManagement()
                .AddThemeManagement()
                .AddWordPrediction()
                .AddSpellChecking()
                .AddAbbreviationsManagement()
                .AddCommandManagement()
                .AddEventManagement()
                .AddCQRSHandlers()
                .AddRepositories()
                .AddDiagnostics();

            return services;
        }
    }
}
