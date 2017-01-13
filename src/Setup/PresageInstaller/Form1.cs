////////////////////////////////////////////////////////////////////////////
// <copyright file="Form1.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2016 Intel Corporation 
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
///
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PresageInstaller
{
    /// <summary>
    /// Displays a prompt that Presage will be installed.  Then
    /// installs Presage silently
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Caption to display on the form
        /// </summary>
        private const String caption = "ACAT Setup";

        /// <summary>
        /// Name of the Presage setup exe
        /// </summary>
        private const String presageExeName = "presage-0.9.1-32bit-setup.exe";

        /// <summary>
        /// Full path to the exe
        /// </summary>
        private readonly String exeName;

        /// <summary>
        /// Initializes an instance of the form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            exeName = String.IsNullOrEmpty(Program.LaunchAppName) ? presageExeName : Program.LaunchAppName;
            Load += OnLoad;
        }

        /// <summary>
        /// Handler for the "Next" button.  Installs Presage.  If the user
        /// cancels out, displays a warning message
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    bool retVal = installPresage();
                    if (retVal)
                    {
                        break;
                    }

                    var result = MessageBox.Show(
                                        "You did not install Presage. " +
                                        "Word Prediction will not work in ACAT. Press Retry to install Presage\nPress Cancel to quit.",
                                        caption,
                                        MessageBoxButtons.RetryCancel,
                                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Retry)
                    {
                        var fileName = Process.GetCurrentProcess().MainModule.FileName;
                        var path = Path.GetDirectoryName(fileName);
                        var presageFile = path + "\\" + exeName;
                        MessageBox.Show("Please install Presage manually by running " + presageFile, caption);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error installing Presage. " + ex, caption, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    break;
                }
            }

            Close();
        }

        /// <summary>
        /// Installs presage silently
        /// </summary>
        /// <returns>true if the user installed, false if the user canceled out</returns>
        private bool installPresage()
        {
            const int USER_CANCELLED = 1223;

            var info = new ProcessStartInfo(@exeName)
            {
                UseShellExecute = true,
                Verb = "runas",
                Arguments = "/S /NoNpp"
            };

            try
            {
                Process.Start(info);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == USER_CANCELLED)
                {
                    return false;
                }
                throw;
            }

            return true;
        }

        /// <summary>
        /// Form loader
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void OnLoad(object sender, EventArgs eventArgs)
        {
            TopMost = false;
            TopMost = true;
            ShowInTaskbar = false;
            Text = caption;
            CenterToScreen();
        }
    }
}