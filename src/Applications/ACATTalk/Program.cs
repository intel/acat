////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.CommandManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACATExtension.CommandHandlers;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ACAT.Applications.ACATTalk
{
    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Preferred panel config to use
        /// </summary>
        private static String _panelConfig;

        /// <summary>
        /// Used for parsing the command line
        /// </summary>
        private enum ParseState
        {
            Next,
            PanelConfig
        }

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

            FileUtils.LogAssemblyInfo();

            parseCommandLine(args);

            AppCommon.LoadGlobalSettings();

            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            if (!AppCommon.CreateUserAndProfile())
            {
                return;
            }

            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }

            Common.AppPreferences.AppId = "ACATTalk";
            Common.AppPreferences.AppName = "ACAT Talk";

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Log.SetupListeners();

            CommandDescriptors.Init();

            setSwitchMapCommands();

            Common.AppPreferences.PreferredPanelConfigNames = !String.IsNullOrEmpty(_panelConfig) ? _panelConfig : "TalkApplicationABC";

            Splash splash = new Splash(1000);
            splash.Show();

            Context.PreInit();
            Common.PreInit();

            Context.AppAgentMgr.EnableAppAgentContextSwitch = false;

            if (!Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.AgentManager |
                                Context.StartupFlags.WindowsActivityMonitor |
                                Context.StartupFlags.Abbreviations))
            {
                splash.Close();
                splash = null;

                TimedMessageBox.Show(Context.GetInitCompletionStatus());
                if (Context.IsInitFatal())
                {
                    return;
                }
            }

            AuditLog.Audit(new AuditEvent("Application", "start"));

            Context.ShowTalkWindowOnStartup = false;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = false;
            Context.AppAgentMgr.EnableContextualMenusForMenus = false;
            Context.AppAgentMgr.DefaultAgentForContextSwitchDisable = Context.AppAgentMgr.NullAgent;

            if (splash != null)
            {
                splash.Close();
            }

            if (!Context.PostInit())
            {
                MessageBox.Show(Context.GetInitCompletionStatus(), R.GetString("InitializationError"));
                return;
            }

            Common.Init();

            Context.AppWindowPosition = Windows.WindowPosition.CenterScreen;

            try
            {
                var form = PanelManager.Instance.CreatePanel("TalkApplicationScanner");
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
                    MessageBox.Show(String.Format(R.GetString("InvalidFormName"), "TalkApplicationScanner"), R.GetString("Error"));
                    return;
                }

                AppCommon.ExitMessageShow();

                AuditLog.Audit(new AuditEvent("Application", "stop"));

                Context.Dispose();

                Common.Uninit();

                AppCommon.ExitMessageClose();

                Log.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            AppCommon.OnExit();
        }

        /// <summary>
        /// Parses the command line arguments. Format of the
        /// arguments are -option <option arg>
        /// </summary>
        /// <param name="args">Args to parse</param>
        private static void parseCommandLine(string[] args)
        {
            var parseState = ParseState.Next;

            for (int index = 0; index < args.Length; index++)
            {
                switch (args[index].ToLower().Trim())
                {
                    case "-panelconfig":
                    case "/panelconfig":
                        parseState = ParseState.PanelConfig;
                        break;
                }

                switch (parseState)
                {
                    case ParseState.PanelConfig:
                        if (!AppCommon.IsOption(args[index]))
                        {
                            _panelConfig = args[index].Trim();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Disable mapping for commands that are not supported by this
        /// app, and cannot be triggerd by an actuator switch
        /// </summary>
        private static void setSwitchMapCommands()
        {
            string[] commands = {
                "CmdToolsMenu",
                "CmdSwitchWindows",
                "CmdSwitchApps",
                "CmdFileBrowserFileOpen",
                "CmdLaunchApp",
                "CmdContextMenu",
                "CmdMainMenu",
                "CmdTalkWindowToggle",
                "CmdTalkWindowShow",
                "CmdTalkWindowClose",
                "CmdTalkApp",
                "CmdAutoPositionScanner",
                "CmdPositionScannerTopRight",
                "CmdPositionScannerTopLeft",
                "CmdPositionScannerBottomRight",
                "CmdPositionScannerBottomLeft",
                "CmdAutoPositionScanner",
                "CmdWindowPosSizeMenu"
            };

            CommandManager.Instance.AppCommandTable.SetEnableSwitchMap(commands, false);
        }
    }
}