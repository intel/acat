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

#define PERFORMANCE

using ACAT.Applications;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.AgentManagement.TextControlAgents;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.Utility.Diagnostics;
using ACAT.Core.Utility.Metrics;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extension.UI;
using ACAT.Extensions.UI.Diagnostics;
using ACAT.Extensions.UI.Scanners;
using ACATResources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ACATTalk
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
#if DEBUG
        private static PerformanceDashboard _performanceDashboard = null;
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
#if PERFORMANCE
            PerformanceMonitor.Initialize();
            PerformanceMonitor.StartTimer("TotalStartupTime");
            PerformanceMonitor.LogEvent("Application", "Main entry point");
#endif

#if DEBUG
            _performanceDashboard = new PerformanceDashboard(
                PerformanceMonitor.GetRuntimeCollector(),
                PerformanceMonitor.GetMemoryProfiler());
            _performanceDashboard.Show();
#endif

            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CoreGlobals.AppId = "ACATTalk";
            CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";
            FatalErrorHandler.EvtFatalError += CoreGlobals_EvtFatalError;

#if PERFORMANCE
            PerformanceMonitor.StartTimer("FileUtilsLogAssemblyInfo");
#endif
            FileUtils.LogAssemblyInfo();
#if PERFORMANCE
            PerformanceMonitor.StopTimer("FileUtilsLogAssemblyInfo", PerformanceMonitor.MetricCategory.Startup);
#endif

#if PERFORMANCE
            var swGS = Stopwatch.StartNew();
#endif
            AppCommon.LoadGlobalSettings();
#if PERFORMANCE
            PerformanceMonitor.RecordIoOperation("LoadGlobalSettings", swGS.Elapsed.TotalMilliseconds);
#endif

            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            if (!AppCommon.CreateUserAndProfile())
            {
                return;
            }

#if PERFORMANCE
            var swUP = Stopwatch.StartNew();
#endif
            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }
#if PERFORMANCE
            PerformanceMonitor.RecordIoOperation("LoadUserPreferences", swUP.Elapsed.TotalMilliseconds);
#endif
            if (!AppCommon.SetCulture())
            {
                return;
            }

            User32Interop.SetProcessDPIAware();

            AppCommon.CheckDisplayScalingAndResolution();

            Common.AppPreferences.AppName = "ACAT Talk";

            // Initialize modern logging infrastructure FIRST - before anything else needs it
            // Use the shared singleton logger factory to ensure single log file
            modernLoggingFactory = LoggingConfiguration.GetSharedLoggerFactory();
            LogManager.Initialize(modernLoggingFactory);  // Initialize global logger manager

            _logger = modernLoggingFactory.CreateLogger(typeof(Program));

            _logger.LogDebug("ACAT Talk Application Launch");

            // Set up dependency injection for extension instantiation
            InitializeDependencyInjection();

            AuditLog.Audit(new AuditEvent("Application", "start"));

            CommandDescriptors.Init();

            Common.AppPreferences.PreferredPanelConfigNames = string.Empty;

            if (!AppCommon.DoOnboarding())
            {
                return;
            }

            splash = new Splash(2000);
            splash.Show(StringResources.StartingACAT);

            Context.PreInit();
            Common.PreInit();
            Context.AppAgentMgr.EnableAppAgentContextSwitch = false;

#if PERFORMANCE
            var swCtxInit = Stopwatch.StartNew();
#endif
            if (!Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.AgentManager |
                                Context.StartupFlags.SpellChecker |
                                Context.StartupFlags.WindowsActivityMonitor |
                                Context.StartupFlags.Abbreviations))
            {
                splash.Close();

                splash = null;

                ConfirmBoxOneOption.ShowDialog(Context.GetInitCompletionStatus(), "", StringResources.OK);
                if (Context.IsInitFatal())
                {
                    return;
                }
            }

            Context.ShowTalkWindowOnStartup = false;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = false;
            Context.AppAgentMgr.EnableContextualMenusForMenus = false;
            Context.AppAgentMgr.DefaultAgentForContextSwitchDisable = Context.AppAgentMgr.NullAgent;

#if PERFORMANCE
            PerformanceMonitor.RecordIoOperation("ContextInit", swCtxInit.Elapsed.TotalMilliseconds);
            // Wire prediction and UI callbacks so library code can feed data into PerformanceMonitor
            UserControlWordPredictionCommon.OnPredictionLatencyMs =
                ms => PerformanceMonitor.RecordPredictionLatency(ms);
            UserControlWordPredictionCommon.OnAutoCompleteLatencyMs =
                ms => PerformanceMonitor.RecordAutoCompleteLatency(ms);
            UserControlWordPredictionCommon.OnRefreshLatencyMs =
                ms => PerformanceMonitor.RecordPredictionRefresh(ms);
            TalkApplicationScanner.OnUiKeyPressLatencyMs =
                ms => PerformanceMonitor.RecordUiInputLag(ms);
            // Wire text-input metrics converted from CoreGlobals.Stopwatch
            UserControlKeyboardCommon.OnKeyActuationLatencyMs =
                (type, ms) => PerformanceMonitor.RecordKeyActuationLatency(type, ms);
            TextController.OnAutoCompletePhaseLatencyMs =
                (phase, ms) => PerformanceMonitor.RecordAutoCompletePhaseLatency(phase, ms);
            EditTextControlAgent.OnTextChangeEventLatencyMs =
                ms => PerformanceMonitor.RecordTextChangeEventLatency(ms);
#endif

            splash?.Close();
            splash = null;

            if (!Context.PostInit())
            {
                Context.Dispose();
                return;
            }

            Common.Init();

            Context.AppWindowPosition = Windows.WindowPosition.CenterScreen;

            AuditLog.Audit(new AuditEvent("Application", "Initialiation complete"));

            try
            {
                Context.AppActuatorManager.ShowTryoutDialog(true);

                showTalkInterfaceDescription();

                var startupArg = new StartupArg("TalkApplicationScanner")
                {
                    QuitAppOnFormClose = false
                };

                Form form = PanelManager.Instance.CreatePanel("TalkApplicationScanner", startupArg);
                if (form != null)
                {
                    // Add ad-hoc agent that will handle the form
                    IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
                    if (agent == null)
                    {
                        MessageBox.Show("Could not find application agent for this application.");
                        return;
                    }

                    Context.AppAgentMgr.AddAgent(form.Handle, agent);
                    Context.AppPanelManager.ShowDialog(form as IPanel);
                }
                else
                {
                    MessageBox.Show(string.Format(StringResources.InvalidFormName, "TalkApplicationScanner"));
                    return;
                }

                splash = new Splash();
                splash.Show(StringResources.ExitingACAT);
                AuditLog.Audit(new AuditEvent("Application", "stop"));

#if DEBUG
                // Close PerformanceDashboard before disposing Context to prevent WPF dispatcher from keeping app alive
                if (_performanceDashboard != null)
                {
                    _performanceDashboard.Dispatcher.InvokeShutdown();
                    _performanceDashboard.Close();
                    _performanceDashboard = null;
                }
#endif

                Context.Dispose();

                Common.Uninit();

                splash?.Close();
                splash = null;

                _logger.LogDebug("ACATTalk Application shutdown");

                modernLoggingFactory?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                modernLoggingFactory?.Dispose();
            }
#if PERFORMANCE
            PerformanceMonitor.StopTimer("TotalStartupTime", PerformanceMonitor.MetricCategory.Startup);
            PerformanceMonitor.LogEvent("Application", "Exit");
            PerformanceMonitor.Shutdown();
#endif

            AppCommon.OnExit();

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

            // Make service provider available to Context for extension loading
            Context.ServiceProvider = _serviceProvider;

            _logger.LogDebug("Dependency injection initialized with ACAT services");
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

            Form form = PanelManager.Instance.CreatePanel("DefaultInterfaceScanner", "ACAT Talk Description");
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(form as IPanel);
            }
        }
    }
}