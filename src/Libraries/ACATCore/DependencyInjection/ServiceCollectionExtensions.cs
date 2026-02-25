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
using ACAT.Core.AnimationManagement;
using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Strategies;
using ACAT.Core.CommandManagement;
using ACAT.Core.Configuration;
using ACAT.Core.DataAccess;
using ACAT.Core.Diagnostics;
using ACAT.Core.EventManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
using ACAT.Core.SpellCheckManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.WidgetManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.Utility.TypeLoader;
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
    ///   <item>Configuration services (JsonSchemaValidator, ConfigurationReloadService,
    ///         EnvironmentConfiguration) are Singletons – shared across the application.</item>
    /// </list>
    /// </remarks>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="Context"/> as a singleton and exposes it through
        /// <see cref="IContext"/>. The factory sets <see cref="Context.ServiceProvider"/>
        /// from the DI container so that all static <c>Context.AppXXX</c> accessor
        /// properties resolve managers through the DI container automatically.
        /// </summary>
        public static IServiceCollection AddContextService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IContext>(provider => 
            {
                // The provider parameter might be a scope (ServiceProviderEngineScope).
                // We need to ensure Context.ServiceProvider is set to the root provider
                // so that static accessors always use the root, not a transient scope.
                var rootProvider = GetRootProvider(provider);
                return new Context(rootProvider);
            });
            services.AddSingleton<Context>(provider => (Context)provider.GetRequiredService<IContext>());
            return services;
        }

        /// <summary>
        /// Gets the root service provider from a potentially scoped provider.
        /// </summary>
        private static IServiceProvider GetRootProvider(IServiceProvider provider)
        {
            // Navigate through scopes to find the root provider
            var current = provider;
            var scopeType = Type.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceProviderEngineScope, Microsoft.Extensions.DependencyInjection");

            if (scopeType != null)
            {
                while (scopeType.IsInstanceOfType(current))
                {
                    var rootProviderProperty = scopeType.GetProperty("RootProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (rootProviderProperty != null)
                    {
                        var root = rootProviderProperty.GetValue(current) as IServiceProvider;
                        if (root != null && root != current)
                        {
                            current = root;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return current;
        }

        /// <summary>
        /// Registers the new animation engine services.
        /// </summary>
        /// <remarks>
        /// Service lifetimes:
        /// <list type="bullet">
        ///   <item><see cref="IScanTimer"/> → <see cref="SystemScanTimer"/> (Transient — one per session).</item>
        ///   <item><see cref="IAnimationConfigProvider"/> → <see cref="AnimationConfigProvider"/> (Singleton).</item>
        ///   <item><see cref="IAnimationService"/> → <see cref="AnimationService"/> (Singleton).</item>
        ///   <item>Scan strategies (<see cref="AutoScanStrategy"/>, <see cref="ManualScanStrategy"/>,
        ///         <see cref="StepScanStrategy"/>) registered as Transient.</item>
        ///   <item><see cref="IScanStrategyFactory"/> → <see cref="DefaultScanStrategyFactory"/> (Singleton).</item>
        /// </list>
        /// Note: <see cref="IHighlightRenderer"/> is not registered here because it requires
        /// host-specific callbacks. The host must register its own implementation.
        /// </remarks>
        public static IServiceCollection AddAnimationEngine(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IScanTimer, SystemScanTimer>();
            services.AddSingleton<IAnimationConfigProvider, AnimationConfigProvider>();
            services.AddSingleton<IScanStrategyFactory, DefaultScanStrategyFactory>();
            services.AddTransient<AutoScanStrategy>();
            services.AddTransient<ManualScanStrategy>();
            services.AddTransient<StepScanStrategy>();
            services.AddSingleton<IAnimationService>(provider =>
                new AnimationService(
                    provider.GetRequiredService<IEventBus>(),
                    provider.GetService<IHighlightRenderer>(),
                    provider.GetRequiredService<IScanStrategyFactory>(),
                    null));
            return services;
        }

        // ---------------------------------------------------------------
        // Individual module registration methods
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers the <see cref="ActuatorManager"/> and its related factories.
        /// </summary>
        public static IServiceCollection AddActuatorManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ActuatorManager>(provider => ActuatorManager.Instance);
            services.AddSingleton<IActuatorManager>(provider => provider.GetRequiredService<ActuatorManager>());
            services.AddSingleton<IActuatorManagerFactory, ActuatorManagerFactory>();
            services.AddSingleton<IActuatorFactory, ActuatorFactory>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="AgentManager"/> and its related factories.
        /// </summary>
        public static IServiceCollection AddAgentManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<AgentManager>(provider => AgentManager.Instance);
            services.AddSingleton<IAgentManager>(provider => provider.GetRequiredService<AgentManager>());
            services.AddSingleton<IAgentManagerFactory, AgentManagerFactory>();
            services.AddSingleton<IAgentFactory, AgentFactory>();
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
        /// Registers the <see cref="PanelManager"/> and its related factories,
        /// including <see cref="IScannerFactory"/> for creating individual scanner panels.
        /// </summary>
        public static IServiceCollection AddPanelManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<PanelManager>(provider => PanelManager.Instance);
            services.AddSingleton<IPanelManager>(provider => provider.GetRequiredService<PanelManager>());
            services.AddSingleton<IPanelManagerFactory, PanelManagerFactory>();
            services.AddSingleton<IScannerFactory, ScannerFactory>();
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
            services.AddSingleton<IAsyncRepository<Theme>>(provider => (ThemeRepository)provider.GetRequiredService<IRepository<Theme>>());
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

        /// <summary>
        /// Registers the <see cref="IWidgetFactory"/> for creating individual widget instances.
        /// </summary>
        public static IServiceCollection AddWidgetManagement(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IWidgetFactory, WidgetFactory>();
            return services;
        }

        /// <summary>
        /// Registers <see cref="ExtensionLoader{TExtension}"/> as a singleton
        /// <see cref="IExtensionLoader{TExtension}"/> for the specified extension type.
        /// </summary>
        /// <typeparam name="TExtension">
        /// The plugin-extension interface type (must implement <see cref="IPluginExtension"/>).
        /// </typeparam>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddExtensionLoader<TExtension>(this IServiceCollection services)
            where TExtension : class, IPluginExtension
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IExtensionLoader<TExtension>>(
                provider => new ExtensionLoader<TExtension>(provider));
            return services;
        }

        /// <summary>
        /// Registers ACAT configuration services including JSON schema validation,
        /// hot-reload support, and environment-specific configuration.
        /// </summary>
        /// <remarks>
        /// Service lifetimes:
        /// <list type="bullet">
        ///   <item><see cref="JsonSchemaValidator"/> is Singleton – schemas are loaded once and reused.</item>
        ///   <item><see cref="ConfigurationReloadService"/> is Singleton – a single watcher per application.</item>
        ///   <item><see cref="EnvironmentConfiguration"/> is Singleton – environment is fixed at startup.</item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddACATConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<JsonSchemaValidator>();
            services.AddSingleton<ConfigurationReloadService>();
            services.AddSingleton<EnvironmentConfiguration>();
            services.AddSingleton<IConfigurationManager>(provider => provider.GetRequiredService<EnvironmentConfiguration>());
            services.AddSingleton<IConfigurationManagerFactory>(provider =>
                new ConfigurationManagerFactory(provider.GetRequiredService<IConfigurationManager>()));
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
                .AddContextService()
                .AddACATConfiguration()
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
                .AddDiagnostics()
                .AddWidgetManagement()
                .AddAnimationEngine();

            return services;
        }

        /// <summary>
        /// Registers all ACAT services with the service container.
        /// This is an alias for <see cref="AddACATCoreModules"/>.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddACATServices(this IServiceCollection services)
        {
            return AddACATCoreModules(services);
        }
    }
}
