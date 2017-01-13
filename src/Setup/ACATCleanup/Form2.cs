////////////////////////////////////////////////////////////////////////////
// <copyright file="Form2.cs" company="Intel Corporation">
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

using System;
using System.Windows.Forms;

namespace ACATCleanup
{
    /// <summary>
    /// Form to display prompt to inform the user that Presage
    /// will be uninstalled now
    /// </summary>
    public partial class Form2 : Form
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public Form2()
        {
            InitializeComponent();
            Load += Form2_Load;
        }

        /// <summary>
        /// Handler for the "Next" button
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form loader
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form2_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }
    }
}