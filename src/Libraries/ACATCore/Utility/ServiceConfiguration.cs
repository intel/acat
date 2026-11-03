////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.DependencyInjection;
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

            // Delegate to the per-module extension methods defined in ServiceCollectionExtensions
            services.AddACATCoreModules();

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
