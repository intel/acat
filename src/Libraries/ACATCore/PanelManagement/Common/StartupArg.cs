////////////////////////////////////////////////////////////////////////////
// <copyright file="StartupArg.cs" company="Intel Corporation">
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

using System;
using System.Windows.Automation;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Argument used in the call to display a scanner.  Has all
    /// the context information to display the scanner
    /// </summary>
    public class StartupArg
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Class of the scanner (alphabet, numeric etc)</param>
        public StartupArg(String panelClass)
        {
            init();
            PanelClass = panelClass;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public StartupArg()
        {
            init();
        }

        /// <summary>
        /// Optional user-defined arguments
        /// </summary>
        public object Arg { get; set; }

        /// <summary>
        /// Auto-position the scanner to its default position?
        /// </summary>
        public bool AutoPosition { get; set; }

        /// <summary>
        /// Is the scanner being used in as a panel for a
        /// dialog box?
        /// </summary>
        public bool DialogMode { get; set; }

        /// <summary>
        /// The currently focused control in the application window
        /// </summary>
        public AutomationElement FocusedElement { get; set; }

        /// <summary>
        /// Gets or sets the class of the scanner being displayed
        /// </summary>
        public String PanelClass { get; set; }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void init()
        {
            PanelClass = String.Empty;
            FocusedElement = null;
            DialogMode = false;
            Arg = null;
            AutoPosition = true;
        }
    }
}