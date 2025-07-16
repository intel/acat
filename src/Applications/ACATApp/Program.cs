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

using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACATExtension.CommandHandlers;
using ACATResources;
using System;
using System.Windows.Forms;

namespace ACAT.Applications.ACATApp
{
    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        private static Splash splash = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!AppCommon.CheckFontsInstalled())
            {
                return;
            }

            CoreGlobals.AppId = "ACATDashboard";
            CoreGlobals.ACATUserGuideFileName = "ACAT User Guide.pdf";
            global::ACAT.Core.Utility.FatalErrorHandler.EvtFatalError += CoreGlobals_EvtFatalError;

            FileUtils.LogAssemblyInfo();

            AppCommon.LoadGlobalSettings();

            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            bool freshInstallForUser = !UserManager.UserExists(UserManager.CurrentUser);

            if (!AppCommon.CreateUserAndProfile() || !AppCommon.LoadUserPreferences() || !AppCommon.SetCulture())
            {
                return;
            }

            //User32Interop.SetProcessDPIAware();

            //AppCommon.CheckDisplayScalingAndResolution();

            Common.AppPreferences.AppName = "ACAT App";

            Log.SetupListeners();
            Log.Debug("ACAT App Application Launch");

            //AuditLog.Audit(new AuditEvent("Application", "start"));

            //AppCommon.addBCIActuatorSetting();
            //AppCommon.addPanelClassConfigMapForBCI();

            CommandDescriptors.Init();

            Common.AppPreferences.PreferredPanelConfigNames = String.Empty;

            //if (!AppCommon.DoOnboarding())
            //{
            //    return;
            //}

            splash = new Splash(2000);
            splash.Show();

            Context.PreInit();
            Common.PreInit();

            Context.AppAgentMgr.EnableAppAgentContextSwitch = true;

            if (!Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.AgentManager |
                                Context.StartupFlags.SpellChecker |
                                Context.StartupFlags.WindowsActivityMonitor |
                                Context.StartupFlags.Abbreviations
                ))
            {
                splash.Close();
                splash = null;

                ConfirmBoxOneOption.ShowDialog("ACAT Fatal Error", Context.GetInitCompletionStatus(), StringResources.OK);
                //return;
            }

            else
            {
                Context.ShowTalkWindowOnStartup = false;
                Context.AppAgentMgr.EnableContextualMenusForDialogs = false;
                Context.AppAgentMgr.EnableContextualMenusForMenus = false;
                Context.AppAgentMgr.DefaultAgentForContextSwitchDisable = Context.AppAgentMgr.NullAgent;

                splash?.Close();

                splash = null;

                if (!Context.PostInit())
                {
                    Context.Dispose();
                    return;
                }

                Common.Init();

                //Context.AppWindowPosition = Windows.WindowPosition.CenterScreen;

                //AuditLog.Audit(new AuditEvent("Application", "Initialiation complete"));

                try
                {
                    Context.AppActuatorManager.ShowTryoutDialog(true);

                    // showTalkInterfaceDescription();

                    var startupArg = new StartupArg("DashboardAppScanner")
                    {
                        QuitAppOnFormClose = false
                    };

                    var form = PanelManager.Instance.CreatePanel("DashboardAppScanner", startupArg);
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
                        MessageBox.Show(String.Format(StringResources.InvalidFormName, startupArg.ToString()));
                        return;
                    }

                    AppCommon.ExitMessageShow();

                    AuditLog.Audit(new AuditEvent("Application", "stop"));

                    Context.Dispose();

                    Common.Uninit();

                    ScannerFocus.Stop();

                    AppCommon.ExitMessageClose();

                    Log.Debug("ACATTalk Application shutdown");

                    Log.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            AppCommon.OnExit();

        }

        /// <summary>
        /// A fatal error has occurred.  Try and gracefully exit ACAT
        /// </summary>
        /// <param name="reason"></param>
        private static void CoreGlobals_EvtFatalError(string reason)
        {
            splash?.Close();

            ScannerFocus.Stop();

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