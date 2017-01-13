////////////////////////////////////////////////////////////////////////////
// <copyright file="OutlookAgentTextInterface.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Extension.AppAgents.Outlook
{
    public class OutlookAgentTextInterface : EditTextControlAgent
    {
        /// <summary>
        /// Initializes a new instance of the class. Disable
        /// abbreviation expansion and spell check.  This is used
        /// for fields such as the "TO" or "CC" fields where we
        /// don't want spellcheck or abbreviations to expand.
        /// </summary>
        /// <param name="handle">handle to the eurdoa window</param>
        /// <param name="editControlElement">element in focus</param>
        /// <param name="handled">true if this was handled</param>
        public OutlookAgentTextInterface(IntPtr handle,
                                        AutomationElement editControlElement,
                                        ref bool handled)
            : base(handle, editControlElement, ref handled)
        {
            Log.Debug();
        }

        /// <summary>
        /// Disables smart punctuations
        /// </summary>
        /// <returns>false</returns>
        public override bool EnableSmartPunctuations()
        {
            return false;
        }

        /// <summary>
        /// Disables abbreviations
        /// </summary>
        /// <returns>false always</returns>
        public override bool ExpandAbbreviations()
        {
            return false;
        }

        /// <summary>
        /// Disables spellchecking
        /// </summary>
        /// <returns>true always</returns>
        public override bool SupportsSpellCheck()
        {
            return true;
        }
    }
}