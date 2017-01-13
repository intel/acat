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

using ACAT.Applications;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACATExtension.CommandHandlers;
using System;
using System.Windows.Forms;

namespace ACATConfig
{
    /// <summary>
    /// Entry point into the app that enables the user to configure ACAT
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Event handler to indicate language changed. Save
        /// the settings.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private static void form_EvtLanguageChanged(object sender, PreferencesCategoriesForm.PreferencesLanguageChanged arg)
        {
            Common.AppPreferences.Language = arg.CI.Name;
            ResourceUtils.SetCulture(Common.AppPreferences.Language);
            if (arg.IsDefault)
            {
                Common.AppPreferences.Save();
            }
        }

        /// <summary>
        /// Event handler for theme change
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="selectedTheme">theme selected</param>
        private static void Form_EvtThemeChanged(object sender, string selectedTheme)
        {
            Common.AppPreferences.Theme = selectedTheme;
            Common.AppPreferences.Save();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
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

            Log.SetupListeners();

            CommandDescriptors.Init();

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Common.PreInit();
            Common.Init();

            Splash splash = new Splash(1000);
            splash.Show();

            splash.Close();

            var form = new PreferencesCategoriesForm();
            form.EvtLanguageChanged += form_EvtLanguageChanged;
            form.EvtThemeChanged += Form_EvtThemeChanged;
            Application.Run(form);
        }
    }
}