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

using ACAT.Lib.Core.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;

namespace ACAT.Applications.ACATDashboard
{
    /// <summary>
    /// The ACAT Dashboard app is a launchpad to 
    /// enable the user to launch the various versions of
    /// ACAT Applications
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Settings file name
        /// </summary>
        private const String _settingsFileName = "DashboardSettings.xml";

        /// <summary>
        /// Window title
        /// </summary>
        private const String _title = "ACAT Dashboard";

        /// <summary>
        /// Gets or sets the settings for the Dashboard
        /// </summary>
        public static DashboardSettings Settings { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // if already running, activate it
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1)
            {
                IntPtr handle = User32Interop.FindWindow(null, _title);
                if (handle != IntPtr.Zero)
                {
                    User32Interop.ShowWindow(handle.ToInt32(), User32Interop.SW_SHOW);
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileUtils.LogAssemblyInfo();

            Splash splash = new Splash(1000);
            splash.Show();

            splash.Close();

            DashboardSettings.PreferencesFilePath = FileUtils.GetFullPathRelativeToApp(_settingsFileName);

            Settings = DashboardSettings.Load();

            var form = new DashboardForm { Text = _title };

            Application.Run(form);
        }
    }
}