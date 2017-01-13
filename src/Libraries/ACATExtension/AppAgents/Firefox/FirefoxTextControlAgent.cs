////////////////////////////////////////////////////////////////////////////
// <copyright file="FirefoxTextControlAgent.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Automation;

namespace ACAT.Lib.Extension.AppAgents.Firefox
{
    /// <summary>
    /// Text control agent for the FireFox browser. Disable expansion of
    /// abbreviations and auto-correct
    /// </summary>
    public class FireFoxTextControlAgent
        : EditTextControlAgent
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="handle">Handle to the focused control</param>
        /// <param name="editControlElement">The automation element</param>
        /// <param name="handled">true if handled</param>
        public FireFoxTextControlAgent(IntPtr handle, AutomationElement editControlElement, ref bool handled)
            : base(handle, editControlElement, ref handled)
        {
            Log.Debug();
        }

        /// <summary>
        /// Disables expansion of abbreviations
        /// </summary>
        /// <returns>false</returns>
        public override bool ExpandAbbreviations()
        {
            return false;
        }

        /// <summary>
        /// Disables spellcheck/autocorrect
        /// </summary>
        /// <returns>true</returns>
        public override bool SupportsSpellCheck()
        {
            return true;
        }
    }
}