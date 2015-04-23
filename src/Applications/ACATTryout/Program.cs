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
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            //Disallow multiple instances
            if (FileUtils.CheckAppExistingInstance("ACATMutex"))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // get appname and copyright information
            var assembly = Assembly.GetExecutingAssembly();

            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var appName = (attributes.Length != 0) ? ((AssemblyTitleAttribute)attributes[0]).Title : String.Empty;

            var appVersion = "Version " + assembly.GetName().Version.ToString();
            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var appCopyright = (attributes.Length != 0) ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : String.Empty;

            Log.Info("***** " + appName + ". " + appVersion + ". " + appCopyright + " *****");

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

            Context.PreInit();
            Common.PreInit();

            try
            {
                if (!Context.Init(Context.StartupFlags.Minimal))
                {
                    MessageBox.Show("Context initialization error");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Context Init exception " + ex);
                return;
            }

            Common.Init();

            var form = PanelManager.Instance.CreatePanel("ACATGettingStartedForm");
            if (form != null)
            {
                Context.AppPanelManager.Show(null, form as IPanel);
            }
            try
            {
                Application.Run();

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
                MessageBox.Show("Unable to create user. Error executing batchfile " +
                                    batchFileName + 
                                    ".\nError: " + 
                                    ex);
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

            ACATPreferences.ApplicationAssembly = Assembly.GetExecutingAssembly(); ;

            return true;
        }

        /// <summary>
        /// Sets active profile name
        /// </summary>
        private static void setProfileName()
        {
            ProfileManager.CurrentProfile = CoreGlobals.AppGlobalPreferences.CurrentProfile.Trim();
            if (String.IsNullOrEmpty(ProfileManager.CurrentProfile))
            {
                ProfileManager.CurrentProfile = ProfileManager.DefaultProfileName;
            }
        }

        /// <summary>
        /// Sets the active user name
        /// </summary>
        private static void setUserName()
        {
            UserManager.CurrentUser = CoreGlobals.AppGlobalPreferences.CurrentUser.Trim();
            if (String.IsNullOrEmpty(UserManager.CurrentUser))
            {
                UserManager.CurrentUser = UserManager.DefaultUserName;
            }
        }
    }
}