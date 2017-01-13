////////////////////////////////////////////////////////////////////////////
// <copyright file="NotepadTextControlAgent.cs" company="Intel Corporation">
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
using System;
using System.Windows.Automation;

namespace ACAT.Lib.Extension.AppAgents.Notepad
{
    /// <summary>
    /// The text interface for the Notepad window. The base class
    /// does all the heavy lifting for us a the notepad is just
    /// an edit control
    /// </summary>
    public class NotepadTextControlAgent : EditTextControlAgent
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="handle">Handle of the notpad window</param>
        /// <param name="editControlElement">automation element of the text control</param>
        /// <param name="handled">set to true if handled</param>
        public NotepadTextControlAgent(IntPtr handle, AutomationElement editControlElement, ref bool handled)
            : base(handle, editControlElement, ref handled)
        {
        }

        /// <summary>
        /// If the user hit a sentence terminator, learn the current
        /// sentence i.e., add it to the word prediction model for
        /// better predictions in the future
        /// </summary>
        public override void OnSentenceTerminator()
        {
            learn();
        }
    }
}