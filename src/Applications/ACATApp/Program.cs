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
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACATResources;
using System;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace ACATApp
{

    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        private static Splash splash = null;
        private static Microsoft.Extensions.Logging.ILoggerFactory modernLoggingFactory = null;

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

            InitializeGlobals();
            InitializeUser();
            InitializeLogging();
            InitializeContext();

            if (!PerformOnboarding())
            {
                return;
            }

            ShowSplashScreen("Starting ACAT");

            var initialized = InitializeApplication();

            if (!initialized)
            {
                Context.Dispose();
                return;
            }

            CloseSplashScreen();

            if (!PostInitialization())
            {
                Context.Dispose();
                return;
            }

            ShowMainPanel();

            ShowSplashScreen("Closing ACAT");
            ShutdownApplication();
        }

        private static void InitializeGlobals()
        {
            CoreGlobals.AppId = "ACATDashboard";
            CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";
            FatalErrorHandler.EvtFatalError += CoreGlobals_EvtFatalError;
            FileUtils.LogAssemblyInfo();

            AppCommon.LoadGlobalSettings();
        }

        private static void InitializeLogging()
        {
            // Initialize legacy logging
            Log.SetupListeners();
            
            // Initialize modern logging infrastructure (ticket #3)
            modernLoggingFactory = LoggingConfiguration.CreateLoggerFactory();

            Log.Debug("ACAT Dashboard Application Launch");
        }

        private static void InitializeUser()
        {
            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            if (!AppCommon.CreateUserAndProfile() ||
                !AppCommon.LoadUserPreferences() ||
                !AppCommon.SetCulture())
            {
                throw new Exception("Failed to initialize user");
            }
        }

        private static void InitializeContext()
        {
            User32Interop.SetProcessDPIAware();
            AppCommon.CheckDisplayScalingAndResolution();
            Common.AppPreferences.AppName = "ACAT Dashboard";

            CommandDescriptors.Init();
            Common.AppPreferences.PreferredPanelConfigNames = string.Empty;

            Context.PreInit();
            Common.PreInit();

            Context.AppAgentMgr.EnableAppAgentContextSwitch = true;
        }

        private static bool PerformOnboarding()
        {
            return AppCommon.DoOnboarding();
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

        private static bool InitializeApplication()
        {
            return Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.AgentManager |
                                Context.StartupFlags.WindowsActivityMonitor |
                                Context.StartupFlags.Abbreviations);
        }

        private static bool PostInitialization()
        {
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

            var form = PanelManager.Instance.CreatePanel("DashboardAppScanner", startupArg);
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


            form.FormClosed += (s, e) =>
            {
                ShutdownApplication();
            };


            Context.AppAgentMgr.AddAgent(form.Handle, agent);
            Context.AppPanelManager.ShowDialog(form as IPanel);
            //Application.Run(form as Form);
        }

        private static void ShutdownApplication()
        {
            AuditLog.Audit(new AuditEvent("Application", "stop"));
            Context.Dispose();
            Common.Uninit();
            CloseSplashScreen();
            Log.Debug("ACATTalk Application shutdown");
            Log.Close();
            modernLoggingFactory?.Dispose();
            AppCommon.OnExit();
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

            var form = PanelManager.Instance.CreatePanel("DefaultInterfaceScanner", "ACAT Talk Description");
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(form as IPanel);
            }
        }
    }
}