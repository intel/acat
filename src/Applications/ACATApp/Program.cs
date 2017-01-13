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
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACATExtension.CommandHandlers;
using System;
using System.Windows.Forms;

namespace ACAT.Applications.ACATApp
{
    /// <summary>
    /// Initializes the various modules in ACAT and activates the default scanner.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Preferred panel config to use
        /// </summary>
        private static String _panelConfig;

        /// <summary>
        /// Active profile name
        /// </summary>
        private static string _profile;

        /// <summary>
        /// Active user name
        /// </summary>
        private static String _userName;

        /// <summary>
        /// Used for parsing the command line
        /// </summary>
        private enum ParseState
        {
            Next,
            Username,
            Profile,
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

            AppCommon.SetUserName(_userName);
            AppCommon.SetProfileName(_profile);

            if (!AppCommon.CreateUserAndProfile())
            {
                return;
            }

            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }

            Common.AppPreferences.AppId = "ACATApp";
            Common.AppPreferences.AppName = "ACAT App";

            CommandDescriptors.Init();

            Log.SetupListeners();

            if (!AppCommon.SetCulture())
            {
                return;
            }

            CommandDescriptors.Init();

            if (!String.IsNullOrEmpty(_panelConfig))
            {
                Common.AppPreferences.PreferredPanelConfigNames = _panelConfig;
            }

            Splash splash = new Splash(2000);
            splash.Show();

            Context.PreInit();
            Common.PreInit();

            if (!Context.Init())
            {
                splash.Close();
                splash = null;

                TimedMessageBox.Show(Context.GetInitCompletionStatus());
                if (Context.IsInitFatal())
                {
                    return;
                }
            }

            if (splash != null)
            {
                splash.Close();
            }

            AuditLog.Audit(new AuditEvent("Application", "start"));

            Context.ShowTalkWindowOnStartup = Common.AppPreferences.ShowTalkWindowOnStartup;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = Common.AppPreferences.EnableContextualMenusForDialogs;
            Context.AppAgentMgr.EnableContextualMenusForMenus = Common.AppPreferences.EnableContextualMenusForMenus;

            if (Context.ShowTalkWindowOnStartup)
            {
                Context.AppTalkWindowManager.ToggleTalkWindow();
                Context.ShowTalkWindowOnStartup = false;
            }

            if (!Context.PostInit())
            {
                MessageBox.Show(Context.GetInitCompletionStatus(), R.GetString("InitializationError"));
                return;
            }

            Common.Init();

            try
            {
                Application.Run();

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
                    case "-user":
                    case "/user":
                        parseState = ParseState.Username;
                        break;

                    case "-profile":
                    case "/profile":
                        parseState = ParseState.Profile;
                        break;

                    case "-panelconfig":
                    case "/panelconfig":
                        parseState = ParseState.PanelConfig;
                        break;
                }

                switch (parseState)
                {
                    case ParseState.Profile:
                        args[index] = args[index].Trim();
                        if (!AppCommon.IsOption(args[index]))
                        {
                            _profile = args[index].Trim();
                        }

                        break;

                    case ParseState.Username:
                        if (!AppCommon.IsOption(args[index]))
                        {
                            _userName = args[index].Trim();
                        }

                        break;

                    case ParseState.PanelConfig:
                        if (!AppCommon.IsOption(args[index]))
                        {
                            _panelConfig = args[index].Trim();
                        }
                        break;
                }
            }
        }
    }
}