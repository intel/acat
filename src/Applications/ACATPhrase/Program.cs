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
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ACAT.Lib.Core.CommandManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.UserManagement;
using ACATExtension.CommandHandlers;

namespace ACAT.Applications.ACATTalk
{
    /// <summary>
    /// ACAT Talk is an application customized for conversations.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Name of the class config for this application
        /// </summary>
        private static String _panelConfigClassName = "PhraseSpeakScannerAlt";

        /// <summary>
        /// Name of the phrase speak scanner
        /// </summary>
        private static String _phraseSpeakScannerName = "PhraseSpeakScanner";

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

            Common.AppPreferences.AppId = "ACATPhrase";
            Common.AppPreferences.AppName = "ACAT Phrases";

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Log.SetupListeners();

            CommandDescriptors.Init();

            setSwitchMapCommands();

            if (!createPanelClassConfig())
            {
                return;
            }

            Common.AppPreferences.PreferredPanelConfigNames = _panelConfigClassName;

            Splash splash = new Splash(1000);
            splash.Show();

            Context.PreInit();
            Common.PreInit();

            Context.AppAgentMgr.EnableAppAgentContextSwitch = false;

            if (!Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
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
                var phraseSpeakScanner = Context.AppPanelManager.CreatePanel(_phraseSpeakScannerName);
                if (phraseSpeakScanner != null)
                {
                    var invoker = (phraseSpeakScanner as IExtension).GetInvoker();
                    invoker.SetValue("ShowSearchButton", false);
                    Context.AppPanelManager.ShowDialog(phraseSpeakScanner as IPanel);
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
        /// Creates the PanelClassConfig entry for this application and for the 
        /// PhraseSpeakScanner.
        /// </summary>
        /// <returns>true if successful</returns>
        private static bool createPanelClassConfig()
        {
            var panelClassConfigFilePath = PanelConfigMap.GetDefaultPanelClassConfigFileName();
            if (!File.Exists(panelClassConfigFilePath))
            {
                MessageBox.Show("Could not find PanelClassConfig file " + panelClassConfigFilePath);
                return false;
            }

            var appPanelClassConfig = AppPanelClassConfig.Load<AppPanelClassConfig>(panelClassConfigFilePath);
            var panelClassConfig = appPanelClassConfig.Find(Common.AppPreferences.AppId);
            if (panelClassConfig == null)
            {
                panelClassConfig = appPanelClassConfig.Add(Common.AppPreferences.AppId, 
                                                            "ACAT Phrases Application",
                                                            "Displays a list of phrases that can be converted to speech");

                var panelClassConfigMap = panelClassConfig.Add(_panelConfigClassName, 
                                                                "Phrases Speak scanner without the search button", 
                                                                true);

                panelClassConfigMap.Add(_phraseSpeakScannerName, new Guid("2BE12ADA-88A2-4029-B8F7-8E74209B585B"));

            }

            AppPanelClassConfig.Save(appPanelClassConfig, panelClassConfigFilePath);

            return true;
        }

        /// <summary>
        /// Enable commands that can be triggerd with a switch trigger
        /// </summary>
        private static void setSwitchMapCommands()
        {
            var commands = CommandManager.Instance.AppCommandTable;

            foreach (var command in commands.CmdDescriptors)
            {
                command.EnableSwitchMap = (command.Command == "CmdGoBack");
            }
        }
    }
}