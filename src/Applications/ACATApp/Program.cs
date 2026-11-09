////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// Main entry point into the program. Does onboarding, initializes all
// the extensions and displays the main UI
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Applications;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.Utility.Diagnostics;
using ACAT.Core.Utility.Metrics;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extension.UI;
using ACAT.Extensions.UI.Diagnostics;
using ACATResources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ACATApp
{

    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        private static Splash splash = null;
        private static ILoggerFactory modernLoggingFactory = null;
        private static ILogger _logger;
        private static IServiceProvider _serviceProvider;
        private static bool _commonInitialized;
        private static bool _preferencesInitialized;
        private static bool _contextInitialized;
#if DEBUG
        private static PerformanceDashboard _performanceDashboard = null;
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppCommon.RunApplication(StartApplicationAsync);
        }

        private static async Task StartApplicationAsync()
        {
            try
            {
                await InitializeGlobalsAsync();
                await InitializeUserAsync();
                AppCommon.SetCulture();
                InitializeLogging();
                InitializeDependencyInjection();
                await InitializeContextAsync();

                if (!await AppCommon.DoOnboardingAsync())
                {
                    return;
                }
                AppCommon.SetCulture();

#if DEBUG
                var collector = new RuntimeMetricsCollector();
                var profiler = new MemoryProfiler();
                collector.Start(intervalMs: 5000);

                _performanceDashboard = new PerformanceDashboard(collector, profiler);
                _performanceDashboard.Show();
#endif

                ShowSplashScreen("Starting ACAT");

                var initialized = await InitializeApplicationAsync();

                if (!initialized)
                {
                    return;
                }

                CloseSplashScreen();

                if (!PostInitialization())
                {
                    return;
                }

                ShowMainPanel();
            }
            finally
            {
                ShutdownApplication();
            }
        }

        private static async Task InitializeGlobalsAsync()
        {
            CoreGlobals.AppId = "ACATDashboard";
            CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";
            FatalErrorHandler.EvtFatalError += CoreGlobals_EvtFatalError;
            FileUtils.LogAssemblyInfo();

            await AppCommon.LoadGlobalSettingsAsync();
        }

        private static void InitializeLogging()
        {
            // Initialize modern logging infrastructure
            modernLoggingFactory = LoggingConfiguration.CreateLoggerFactory();
            _logger = modernLoggingFactory.CreateLogger(typeof(Program));

            _logger.LogDebug("ACAT Dashboard Application Launch");

            // Wire up word prediction performance callbacks for diagnostics
            UserControlWordPredictionCommon.OnPredictionLatencyMs = (latencyMs) =>
            {
                if (latencyMs > 100)
                {
                    _logger.LogWarning("Slow word prediction: {LatencyMs:F2}ms", latencyMs);
                }
            };

            UserControlWordPredictionCommon.OnAutoCompleteLatencyMs = (latencyMs) =>
            {
                if (latencyMs > 50)
                {
                    _logger.LogWarning("Slow autocomplete: {LatencyMs:F2}ms", latencyMs);
                }
            };

            UserControlWordPredictionCommon.OnRefreshLatencyMs = (latencyMs) =>
            {
                if (latencyMs > 100)
                {
                    _logger.LogWarning("Slow prediction refresh: {LatencyMs:F2}ms", latencyMs);
                }
            };
        }

        private static void InitializeDependencyInjection()
        {
            // Set up dependency injection using ACAT infrastructure
            var services = new ServiceCollection();

            // Add logging (reuse existing factory)
            services.AddSingleton<ILoggerFactory>(modernLoggingFactory);
            services.AddLogging();

            // Add ACAT core services (managers, factories, etc.)
            services.AddACATServices();

            _serviceProvider = services.BuildServiceProvider();

            // Resolve Context from DI – this sets Context.ServiceProvider automatically
            // so all static Context.AppXXX accessors resolve managers through the container.
            _serviceProvider.GetRequiredService<IContext>();

            _logger.LogDebug("Dependency injection initialized with ACAT services");
        }

        private static async Task InitializeUserAsync()
        {
            await AppCommon.SetUserNameAsync();
            AppCommon.SetProfileName();

            if (!AppCommon.CreateUserAndProfile() ||
                !await AppCommon.LoadUserPreferencesAsync())
            {
                throw new Exception("Failed to initialize user");
            }
            _preferencesInitialized = true;
        }

        private static async Task InitializeContextAsync()
        {
            User32Interop.SetProcessDPIAware();
            await AppCommon.CheckDisplayScalingAndResolutionAsync();
            Common.AppPreferences.AppName = "ACAT Dashboard";

            CommandDescriptors.Init();
            Common.AppPreferences.PreferredPanelConfigNames = string.Empty;

            Context.PreInit();
            Common.PreInit();
            _commonInitialized = true;

            Context.AppAgentMgr.EnableAppAgentContextSwitch = true;
        }

        private static void ShowSplashScreen(string message, int durationMs = 2000)
        {
            splash = new Splash(durationMs);
            splash.Show(message);
        }

        private static void CloseSplashScreen()
        {
            splash?.Close();
            splash = null;
        }

        private static async Task<bool> InitializeApplicationAsync()
        {
            _contextInitialized = await Context.InitAsync(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.AgentManager |
                                Context.StartupFlags.WindowsActivityMonitor |
                                Context.StartupFlags.Abbreviations);
            return _contextInitialized;
        }

        private static bool PostInitialization()
        {
            // Start EventBus activity monitoring (demonstrates new EventBus pattern)
            ActivatePanelActivityMonitor();

            Context.ShowTalkWindowOnStartup = false;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = false;
            Context.AppAgentMgr.EnableContextualMenusForMenus = false;
            Context.AppAgentMgr.DefaultAgentForContextSwitchDisable = Context.AppAgentMgr.NullAgent;

            return Context.PostInit();
        }

        private static void ShowMainPanel()
        {
            var startupArg = new StartupArg("DashboardAppScanner")
            {
                QuitAppOnFormClose = false
            };

            // CQRS: Use command handler instead of direct singleton access
            var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
            var command = new CreatePanelCommand("DashboardAppScanner", null, startupArg);
            createPanelHandler.Handle(command);

            Form form = command.CreatedPanel as Form;
            if (form == null)
            {
                MessageBox.Show(string.Format(StringResources.InvalidFormName, startupArg.ToString()));
                return;
            }

            IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("ACAT Agent");
            if (agent == null)
            {
                MessageBox.Show("Could not find application agent for this application.");
                return;
            }


            Context.AppAgentMgr.AddAgent(form.Handle, agent);
            Context.AppPanelManager.ShowDialog(form as IPanel);
            //Application.Run(form as Form);
        }

        /// <summary>
        /// Activates the PanelActivityMonitor to demonstrate EventBus pattern
        /// This shows real-time panel and actuator activity via EventBus subscriptions
        /// </summary>
        private static void ActivatePanelActivityMonitor()
        {
            try
            {
                if (_serviceProvider != null)
                {
                    var monitor = _serviceProvider.GetRequiredService<ACAT.Core.Diagnostics.PanelActivityMonitor>();
                    _logger.LogInformation("✅ PanelActivityMonitor activated - EventBus subscriptions active");
                    _logger.LogInformation("📊 You will now see real-time panel and actuator activity logs!");
                }
                else
                {
                    _logger.LogWarning("ServiceProvider not available - PanelActivityMonitor not activated");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to activate PanelActivityMonitor");
            }
        }

        private static void ShutdownApplication()
        {
            if (_preferencesInitialized)
            {
                TryCleanup(() => AuditLog.Audit(new AuditEvent("Application", "stop")));
            }

#if DEBUG
            if (_performanceDashboard != null)
            {
                TryCleanup(() => _performanceDashboard.Dispatcher.InvokeShutdown());
                TryCleanup(() => _performanceDashboard.Close());
                _performanceDashboard = null;
            }
#endif

            if (_contextInitialized)
            {
                TryCleanup(Context.Dispose);
            }
            if (_commonInitialized)
            {
                TryCleanup(Common.Uninit);
            }
            TryCleanup(CloseSplashScreen);
            TryCleanup(() => FatalErrorHandler.EvtFatalError -= CoreGlobals_EvtFatalError);
            TryCleanup(() => _logger?.LogDebug("ACAT Dashboard Application shutdown"));
            TryCleanup(() => (_serviceProvider as IDisposable)?.Dispose());
            TryCleanup(() => modernLoggingFactory?.Dispose());
        }

        private static void TryCleanup(Action cleanup)
        {
            try
            {
                cleanup();
            }
            catch (Exception ex)
            {
                // Cleanup must not replace a startup exception or skip later resources.
                try
                {
                    _logger?.LogError(ex, "Application cleanup failed");
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// A fatal error has occurred.  Try and gracefully exit ACAT
        /// </summary>
        /// <param name="reason"></param>
        private static void CoreGlobals_EvtFatalError(string reason)
        {
            splash?.Close();


            if (Context.AppPanelManager != null && Context.AppPanelManager.GetCurrentForm() != null &&
                Context.AppPanelManager.GetCurrentForm().PanelCommon != null && Context.AppPanelManager.GetCurrentForm().PanelCommon.RootWidget != null)
            {
                Context.AppPanelManager.GetCurrentForm().OnPause();
                var form = Context.AppPanelManager.GetCurrentForm().PanelCommon.RootWidget.UIControl as Form;
                ConfirmBoxLargeSingleOption.ShowDialog(reason, "OK", form);
            }
            else
            {
                ConfirmBoxLargeSingleOption.ShowDialog(reason, "OK");
            }

            Application.ExitThread();

            Environment.FailFast(reason);
        }

        private static void showTalkInterfaceDescription()
        {
            if (!Common.AppPreferences.ShowTalkInterfaceDescOnStartup)
            {
                return;
            }

            // CQRS: Use command handler instead of direct singleton access
            var createPanelHandler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePanelCommand>>();
            var command = new CreatePanelCommand("DefaultInterfaceScanner", "ACAT Talk Description");
            createPanelHandler.Handle(command);

            if (command.CreatedPanel != null)
            {
                Context.AppPanelManager.ShowDialog(command.CreatedPanel);
            }
        }
    }
}