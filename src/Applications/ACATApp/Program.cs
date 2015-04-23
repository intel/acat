////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Applications.ACATApp
{
    /// <summary>
    /// Initializes the various modules in ACAT and activates the default scanner.
    /// </summary>
    internal static class Program
    {
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
            Profile
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            // Disallow multiple instances
            if (FileUtils.CheckAppExistingInstance("ACATMutex"))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var assembly = Assembly.GetExecutingAssembly();

            // get appname and copyright information
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var appName = (attributes.Length != 0) ? 
                            ((AssemblyTitleAttribute)attributes[0]).Title : 
                            String.Empty;

            var appVersion = "Version " + assembly.GetName().Version;
            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var appCopyright = (attributes.Length != 0) ? 
                                ((AssemblyCopyrightAttribute)attributes[0]).Copyright : 
                                String.Empty;

            Log.Info("***** " + appName + ". " + appVersion + ". " + appCopyright + " *****");

            parseCommandLine(args);

            CoreGlobals.AppGlobalPreferences = GlobalPreferences.Load(FileUtils.GetPreferencesFileFullPath(GlobalPreferences.FileName));

            //Set the active user/profile information
            setUserName();
            setProfileName();

            //Create user and profile if they don't already exist
            if (!createUserAndProfile())
            {
                return;
            }

            if (!loadUserPreferences())
            {
                return;
            }

            Log.SetupListeners();

            // Display splash screen and initialize
            Splash splash = new Splash(FileUtils.GetImagePath("SplashScreenImage.png"), appName, appVersion, appCopyright, 5000);
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

            AuditLog.Audit(new AuditEvent("Application", "start"));

            Context.ShowTalkWindowOnStartup = Common.AppPreferences.ShowTalkWindowOnStartup;
            Context.AppAgentMgr.EnableContextualMenusForDialogs = Common.AppPreferences.EnableContextualMenusForDialogs;
            Context.AppAgentMgr.EnableContextualMenusForMenus = Common.AppPreferences.EnableContextualMenusForMenus;

            Context.PostInit();

            if (splash != null)
            {
                splash.Close();
            }

            Common.Init();

            try
            {
                Application.Run();

                AuditLog.Audit(new AuditEvent("Application", "stop"));

                Context.Dispose();

                Common.Uninit();

                //Utils.Dispose();

                Log.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Creates the specified user using the batchFileName.
        /// Executes the batchfile which creates the user
        /// folder and copies the initialization files
        /// </summary>
        /// <param name="batchFileName">Name of the batchfile to run</param>
        /// <param name="userName">Name of the user</param>
        /// <returns>true on success</returns>
        private static bool createUser(String batchFileName, String userName)
        {
            bool retVal = true;
            try
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                Process proc = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(dir, batchFileName),
                        WorkingDirectory = dir,
                        Arguments = userName,
                        UseShellExecute = true
                    }
                };

                proc.Start();
                proc.WaitForExit();
                proc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create user. Error executing batchfile " + batchFileName + ".\nError: " + ex.ToString());
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Creates the user and profile directories if they
        /// don't exist
        /// </summary>
        /// <returns></returns>
        private static bool createUserAndProfile()
        {
            if (!UserManager.CurrentUserExists())
            {
                const string batchFile = "CreateUser.bat";

                if (!createUser(batchFile, UserManager.CurrentUser))
                {
                    return false;
                }
            }

            if (!ProfileManager.ProfileExists(ProfileManager.CurrentProfile))
            {
                ProfileManager.CreateProfile(ProfileManager.CurrentProfile);
            }

            if (!ProfileManager.ProfileExists(ProfileManager.CurrentProfile))
            {
                MessageBox.Show("Could not find profile " + ProfileManager.CurrentProfile);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the specified string is an option flag.
        /// it should start with a - or a /
        /// </summary>
        /// <param name="arg">arg to check</param>
        /// <returns>true if it is</returns>
        private static bool isOption(String arg)
        {
            if (!String.IsNullOrEmpty(arg))
            {
                return (arg[0] == '/' || arg[0] == '-');
            }

            return false;
        }

        /// <summary>
        /// Loads user settings from the user's profile directory
        /// </summary>
        /// <returns>true on success</returns>
        private static bool loadUserPreferences()
        {
            ACATPreferences.PreferencesFilePath = ProfileManager.GetFullPath("Settings.xml");
            ACATPreferences.DefaultPreferencesFilePath = ProfileManager.GetFullPath("DefaultSettings.xml");

            FileUtils.AppPreferencesDir = ProfileManager.CurrentProfileDir;

            Common.AppPreferences = ACATPreferences.Load();
            if (Common.AppPreferences == null)
            {
                MessageBox.Show("Unable to read preferences from " + FileUtils.AppPreferencesDir);
                return false;
            }

            Common.AppPreferences.Save();

            CoreGlobals.AppPreferences = Common.AppPreferences;

            ACATPreferences.SaveDefaults<ACATPreferences>(ACATPreferences.DefaultPreferencesFilePath);

            Common.AppPreferences.DebugAssertOnError = false;

            ACATPreferences.ApplicationAssembly = Assembly.GetExecutingAssembly();

            return true;
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
                }

                switch (parseState)
                {
                    case ParseState.Profile:
                        args[index] = args[index].Trim();
                        if (!isOption(args[index]))
                        {
                            _profile = args[index].Trim();
                        }

                        break;

                    case ParseState.Username:
                        if (!isOption(args[index]))
                        {
                            _userName = args[index].Trim();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Sets the active profile name
        /// </summary>
        private static void setProfileName()
        {
            // if the profile has not been specified in the
            // command line, use the one from GlobalPreferences
            if (string.IsNullOrEmpty(_profile))
            {
                ProfileManager.CurrentProfile = CoreGlobals.AppGlobalPreferences.CurrentProfile.Trim();
                if (String.IsNullOrEmpty(ProfileManager.CurrentProfile))
                {
                    ProfileManager.CurrentProfile = ProfileManager.DefaultProfileName;
                }
            }
            else
            {
                ProfileManager.CurrentProfile = _profile;
            }
        }

        private static void setUserName()
        {
            // if username has not been specified in the
            // command line, use the one from GlobalPreferences
            if (string.IsNullOrEmpty(_userName))
            {
                UserManager.CurrentUser = CoreGlobals.AppGlobalPreferences.CurrentUser.Trim();
                if (String.IsNullOrEmpty(UserManager.CurrentUser))
                {
                    UserManager.CurrentUser = UserManager.DefaultUserName;
                }
            }
            else
            {
                UserManager.CurrentUser = _userName;
            }
        }
    }
}