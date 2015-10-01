////////////////////////////////////////////////////////////////////////////
// <copyright file="NotepadTextControlAgent.cs" company="Intel Corporation">
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;
using ACAT.Lib.Core.AgentManagement.TextInterface;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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