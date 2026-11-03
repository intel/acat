////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// Main entry point into the program.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Applications;
using ACAT.Applications.ACATWatch;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ACATWatch
{
    internal static class Program
    {
        private static ILogger _logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (FileUtils.CheckAppExistingInstance("ACATWatchMutex"))
            {
                return;
            }

            if (AppCommon.LoadUserPreferences())
            {
                CoreGlobals.AppId = "ACATWatcher";
                Common.AppPreferences.AppName = "ACAT Watcher";

                CoreGlobals.AppPreferences.DebugLogMessagesToFile = true;
                CoreGlobals.AppPreferences.DebugMessagesEnable = true;

                // Initialize dependency injection with ACAT infrastructure
                var services = new ServiceCollection();
                services.AddACATInfrastructure();
                var serviceProvider = services.BuildServiceProvider();
                Context.ServiceProvider = serviceProvider;
                
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                _logger = loggerFactory.CreateLogger(typeof(Program));

                FileUtils.LogAssemblyInfo(Assembly.GetExecutingAssembly());

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ACATWatchForm(_logger));

                _logger.LogInformation("**** Exit " + Common.AppPreferences.AppName + " " + DateTime.Now.ToString() + " ****");

                (serviceProvider as IDisposable)?.Dispose();
            }
            else
            {
                var tempFactory = LoggingConfiguration.CreateLoggerFactory();
                var tempLogger = tempFactory.CreateLogger(typeof(Program));
                tempLogger.LogError("Failed to load user preferences. Exiting application.");
                tempFactory.Dispose();
            }
        }
    }
}