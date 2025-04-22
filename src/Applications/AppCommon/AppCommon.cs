////////////////////////////////////////////////////////////////////////////
// <copyright file="AppCommon.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Applications
{
    public class AppCommon
    {
        /// <summary>
        /// Form to display the exit message
        /// </summary>
        private static ToastForm _exitMessageToastForm;

        /// <summary>
        /// Creates the user and profile directories if they
        /// don't exist
        /// </summary>
        /// <returns></returns>
        public static bool CreateUserAndProfile()
        {
            if (!UserManager.CreateUser(UserManager.CurrentUser))
            {
                MessageBox.Show(String.Format(R.GetString("CouldNotCreateUserError"), UserManager.CurrentUser),
                    R.GetString("ACATError"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!ProfileManager.ProfileExists(ProfileManager.CurrentProfile))
            {
                ProfileManager.CreateProfileDir(ProfileManager.CurrentProfile);
            }

            if (!ProfileManager.ProfileExists(ProfileManager.CurrentProfile))
            {
                MessageBox.Show("Could not find profile " + ProfileManager.CurrentProfile);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closing the exit message form
        /// </summary>
        public static void ExitMessageClose()
        {
            if (_exitMessageToastForm != null)
            {
                try
                {
                    _exitMessageToastForm.Close();
                    _exitMessageToastForm = null;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Displays message that the app is exiting
        /// </summary>
        public static void ExitMessageShow()
        {
            if (_exitMessageToastForm == null)
            {
                try
                {
                    _exitMessageToastForm = new ToastForm(R.GetString("ExitingACAT"), -1);
                    Windows.SetWindowPosition(_exitMessageToastForm, Windows.WindowPosition.CenterScreen);
                    _exitMessageToastForm.Show();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Checks if the specified string is an option flag.
        /// it should start with a - or a /
        /// </summary>
        /// <param name="arg">arg to check</param>
        /// <returns>true if it is</returns>
        public static bool IsOption(String arg)
        {
            if (!String.IsNullOrEmpty(arg))
            {
                return (arg[0] == '/' || arg[0] == '-');
            }

            return false;
        }

        /// <summary>
        /// Loads app global preferences
        /// </summary>
        public static void LoadGlobalSettings()
        {
            GlobalPreferences.PreferencesFilePath = FileUtils.GetFullPathRelativeToHome("Settings.xml");
            GlobalPreferences.DefaultPreferencesFilePath = FileUtils.GetFullPathRelativeToHome("DefaultSettings.xml");

            CoreGlobals.AppGlobalPreferences = GlobalPreferences.Load();
        }

        /// <summary>
        /// Loads user settings from the user's profile directory
        /// </summary>
        /// <returns>true on success</returns>
        public static bool LoadUserPreferences()
        {
            setPreferencesPaths();

            FileUtils.AppPreferencesDir = ProfileManager.CurrentProfileDir;

            Common.AppPreferences = ACATPreferences.Load();
            if (Common.AppPreferences == null)
            {
                MessageBox.Show(String.Format(R.GetString("UnableToReadPreferences")), FileUtils.AppPreferencesDir);
                return false;
            }

            Common.AppPreferences.Save();

            CoreGlobals.AppPreferences = Common.AppPreferences;

            CoreGlobals.AppDefaultPreferences = ACATPreferences.LoadDefaultSettings();

            ACATPreferences.SaveDefaults<ACATPreferences>(ACATPreferences.DefaultPreferencesFilePath);

            Common.AppPreferences.DebugAssertOnError = false;

            ACATPreferences.ApplicationAssembly = Assembly.GetEntryAssembly();

            return true;
        }

        /// <summary>
        /// Invoke this at the end of the Main function.
        /// </summary>
        public static void OnExit()
        {
            // let's kill the app, in case there are
            // bad actors (mis-behaving plugins, lingering
            // threads etc.
            Process.GetCurrentProcess().Kill();
        }

        public static bool OtherInstancesRunning()
        {
            // Disallow multiple instances
            if (FileUtils.IsACATRunning())
            {
                return true;
            }

            if (FileUtils.AreMultipleInstancesRunning())
            {
                MessageBox.Show("Another instance of " +
                                Process.GetCurrentProcess().ProcessName +
                                " is running. Please exit the app or terminate it from Task Manager and retry",
                    Common.AppPreferences.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Prompts user (if reqd) to select preferred language
        /// </summary>
        public static bool SetCulture()
        {
            bool isDefault = false;

            if (!String.IsNullOrEmpty(Common.AppPreferences.Language))
            {
                ResourceUtils.SetCulture(Common.AppPreferences.Language);
                ResourceUtils.InstallLanguageForUser();
                return true;
            }

            var installedLanguages = ResourceUtils.EnumerateInstalledLanguages();

            if (installedLanguages.Count() == 0)
            {
                MessageBox.Show("No language extensions found.  Exiting", Common.AppPreferences.AppName);
                return false;
            }

            if (installedLanguages.Count() > 1)
            {
                var cultureInfo = LanguageSelectForm.SelectLanguage();
                if (cultureInfo == null)
                {
                    MessageBox.Show("English will be set as default", Common.AppPreferences.AppName);
                    ResourceUtils.SetEnglishCulture();
                    Common.AppPreferences.Language = "en";
                }
                else
                {
                    ResourceUtils.SetCulture(cultureInfo.Name);
                    Common.AppPreferences.Language = cultureInfo.Name;
                    isDefault = LanguageSelectForm.IsDefault;
                }

                if (isDefault)
                {
                    Common.AppPreferences.Save();
                }
            }
            else
            {
                ResourceUtils.SetCulture(installedLanguages.ElementAt(0));
            }

            ResourceUtils.InstallLanguageForUser();

            return true;
        }

        /// <summary>
        /// Sets the active profile name
        /// </summary>
        public static void SetProfileName(String profile = null)
        {
            // if the profile has not been specified in the
            // command line, use the one from GlobalPreferences
            if (string.IsNullOrEmpty(profile))
            {
                ProfileManager.CurrentProfile = CoreGlobals.AppGlobalPreferences.CurrentProfile.Trim();
                if (String.IsNullOrEmpty(ProfileManager.CurrentProfile))
                {
                    ProfileManager.CurrentProfile = ProfileManager.DefaultProfileName;
                }
            }
            else
            {
                ProfileManager.CurrentProfile = profile;
            }
        }

        /// <summary>
        /// Sets the active user name
        /// </summary>
        public static void SetUserName(String userName = null)
        {
            // if username has not been specified in the
            // command line, use the one from GlobalPreferences
            UserManager.CurrentUser = string.IsNullOrEmpty(userName)
                ? CoreGlobals.AppGlobalPreferences.CurrentUser.Trim()
                : userName;

            if (String.IsNullOrEmpty(UserManager.CurrentUser) ||
                String.Compare(UserManager.CurrentUser, "acat", true) == 0 ||
                String.Compare(UserManager.CurrentUser, "default", true) == 0)
            {
                UserManager.CurrentUser = UserManager.DefaultUserName;
                CoreGlobals.AppGlobalPreferences.CurrentUser = UserManager.CurrentUser;
                CoreGlobals.AppGlobalPreferences.Save();
            }
        }

        public static void CheckDisplayScalingAndResolution()
        {
            if (!Common.AppPreferences.ShowDisplayScaleMessageOnStartup)
            {
                return;
            }

            var tuple = DualMonitor.GetDisplayWidthAndScaling();

            String prompt = String.Empty;

            if (tuple.Item1 != 1920)
            {
                prompt = String.Format("The monitor's horizontal resolution is currently set to {0} pixels, which may " +
                                        " cause display issues with the ACAT user interface. The ACAT UI is tailored " +
                                        "for Full HD monitors with a horizontal resolution of 1920 pixels.\r\n\r\n" +
                                        "To resolve any potential display inconsistencies, we recommend adjusting " +
                                        "your monitor's resolution to 1920 pixels wide, if supported.\r\n\r\n" +
                                        "For guidance on changing the display resolution, please refer to the ACAT User Guide.", tuple.Item1);
            }
            else if (tuple.Item2 != 100 && tuple.Item2 != 125)
            {
                prompt = String.Format("The monitor's display scaling is currently set to {0}% which may cause display issues " +
                                        " with the ACAT user interface.  The ACAT UI is tailored for display scaling " +
                                        "values of 100% and 125%.\r\n\r\n To resolve any potential display inconsistencies, " +
                                        "we recommend adjusting your monitor's display scaling to 100% or 125%.\r\n\r\n" + "" +
                                        "For guidance on changing the display scaling, please refer to the ACAT User Guide.", tuple.Item2); ;
            }

            if (!String.IsNullOrEmpty(prompt))
            {
                Common.AppPreferences.ShowDisplayScaleMessageOnStartup = !ConfirmBoxLargeSingleOption.ShowDialog(prompt, "OK", null, true);

                Common.AppPreferences.Save();
            }
        }

        public static bool CheckFontsInstalled()
        {
            string fontPath = SmartPath.ApplicationPath + "\\Assets\\Fonts";

            if (!FontCheck.IsMontserratFontInstalled())
            {
                MessageBox.Show("Montserrat fonts are not installed on this system.\nPlease install them and restart ACAT.\nThe fonts can be found here: " + fontPath,
                                    "ACAT",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                return false;
            }

            String fontName = "ACAT Font 1";
            if (!FontCheck.IsFontInstalled(fontName))
            {
                MessageBox.Show("Font \"" + fontName + "\" is not installed on this system.\nPlease install it and restart ACAT.\nThe font can be found here: " + fontPath,
                                    "ACAT",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Sets the paths to the settings file for the app
        /// </summary>
        private static void setPreferencesPaths()
        {
            ACATPreferences.PreferencesFilePath = ProfileManager.GetFullPath("Settings.xml");
            ACATPreferences.DefaultPreferencesFilePath = ProfileManager.GetFullPath("DefaultSettings.xml");
        }


        public static void addBCIActuatorSetting()
        {
            var config = ActuatorManager.Instance.GetActuatorConfig();

            var actuatorSettings = config.ActuatorSettings;

            var guid = new Guid("77809D19-F450-4D36-A633-D818400B3D9A");
            foreach (var setting in actuatorSettings)
            {
                if (Guid.Equals(guid, setting.Id))
                {
                    return;
                }
            }

            var bciActuatorSetting = new ActuatorSetting
            {
                Description = "Brain Computer Interface (BCI) is technology that reads brain waves to help you interact " +
                                                "with your computer. Click &lt;a href=$ASSETS_VIDEOS_DIR#ACATOverviewBCI.mp4&gt;here&lt;/a&gt; for " +
                                                "a demonstration of BCI. You can find more details &lt;a href=$ACAT_USER_GUIDE#BCISwitch&gt;here&lt;/a&gt;",
                Enabled = false,
                Id = new Guid("77809D19-F450-4D36-A633-D818400B3D9A"),
                ImageFileName = "BCISwitch.png",
                Name = "BCI EEG Actuator"
            };

            var switchSetting = new SwitchSetting
            {
                Actuate = true,
                Command = "@Trigger",
                Description = String.Empty,
                MinHoldTime = "@MinActuationHoldTime",
                Name = "R1",
                Source = "R1"
            };

            bciActuatorSetting.SwitchSettings.Add(switchSetting);

            actuatorSettings.Add(bciActuatorSetting);

            config.Save();
        }

        public static void addPanelClassConfigMapForBCI()
        {
            var panelClassConfigMap = new PanelClassConfigMap();
            panelClassConfigMap.Default = false;
            panelClassConfigMap.Description = "An alphabetically arranged keyboard with predictive text/sentences to help you communicate";
            panelClassConfigMap.Name = "TalkApplicationBCIScannerABC";
            panelClassConfigMap.ScreenshotFileName = "ABCKeyboardLayout.png";
            panelClassConfigMap.DisplayNameShort = "Alphabetical";
            panelClassConfigMap.DisplayNameLong = "ABC Keyboard Layout";

            var panelClassConfigMapEntry = new PanelClassConfigMapEntry();
            panelClassConfigMapEntry.ConfigId = new Guid("18f8796a-c0e2-4d4b-ac20-1e76e0a57bcd");
            panelClassConfigMapEntry.PanelClass = "TalkApplicationScanner";

            panelClassConfigMap.PanelClassConfigMapEntries.Add(panelClassConfigMapEntry);


            PanelConfigMap.AddPanelClassConfigMap(CoreGlobals.AppId, "en", panelClassConfigMap);
        }
    }
}