////////////////////////////////////////////////////////////////////////////
// <copyright file="Form1.cs" company="Intel Corporation">
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
/// 
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ACATCleanup
{
    /// <summary>
    /// Prompts the user whether to delete the contents
    /// of the ACAT install dir
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
            TopMost = true;
        }

        /// <summary>
        /// User doesn't want to delete the ACAT install 
        /// dir.  Just remove presage and quit
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonNo_Click(object sender, EventArgs e)
        {
            Program.KillPresage();
            Program.RemovePresage();
            Close();
        }

        /// <summary>
        /// User wants to delete the ACAT install 
        /// dir.  Make a copy of ourselves to the temp folder
        /// and spawn that exe
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonYes_Click(object sender, EventArgs e)
        {
            var tmpDir = Path.GetTempPath();

            var thisExe = Process.GetCurrentProcess().MainModule.FileName;

            var tempPath = Path.Combine(tmpDir, Path.GetFileName(thisExe));

            File.Copy(thisExe, tempPath, true);

            var info = new ProcessStartInfo {Arguments = "blah", FileName = tempPath};

            Process.Start(info);

            Close();
        }
    }
}