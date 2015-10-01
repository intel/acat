////////////////////////////////////////////////////////////////////////////
// <copyright file="IApplicationAgent.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Delegate for the event that is raised when the text changes in
    /// the active window
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event args</param>
    public delegate void TextChangedDelegate(object sender, TextChangedEventArgs e);

    /// <summary>
    /// All application agents must derive from this interface.  Application agents
    /// handle all interactions with an application, such as notepad, ms word, ie etc.
    /// </summary>
    public interface IApplicationAgent : IDisposable, IExtension
    {
        /// <summary>
        /// Raised when an application agent is deactivated
        /// </summary>
        event AgentClose EvtAgentClose;

        /// <summary>
        /// Raised when the application agent wants to request for
        /// a specific scanner to be activated.
        /// </summary>
        event PanelRequest EvtPanelRequest;

        /// <summary>
        /// Raised when text or cursor position changes in the active
        /// application window.
        /// </summary>
        event TextChangedDelegate EvtTextChanged;

        /// <summary>
        /// Returns the ACAT descriptor for the agent
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Gets or sets the name of the agent
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Gets or sets the parent agent
        /// </summary>
        IApplicationAgent Parent { get; set; }

        /// <summary>
        /// Gets the processes supported by this agent
        /// </summary>
        IEnumerable<AgentProcessInfo> ProcessesSupported { get; }

        /// <summary>
        /// Gets the text control agent object. This object manages
        /// all text manipulation in the active window
        /// </summary>
        ITextControlAgent TextControlAgent { get; }

        /// <summary>
        /// Check to see if a widget (a button on the scanner) should be
        /// enabled or not. This depends on the context.   The arg parameter
        /// contains the widget object in question.  For instance, if the
        /// talk window is empty, the "Clear talk window" button should be disabled.
        /// </summary>
        /// <param name="arg">Argument</param>
        void CheckWidgetEnabled(CheckEnabledArgs arg);

        /// <summary>
        /// Invoked when there is a request to display a contexutal menu for
        /// the currently active process
        /// </summary>
        /// <param name="monitorInfo">Info  about the active process/window</param>
        void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// This function is invoked whenever the focus changes in the
        /// application associated the agent.  Focus could change from
        /// window to another, focus could change from say, an edit
        /// box to another etc.
        /// </summary>
        /// <param name="monitorInfo"></param>
        /// <param name="handled"></param>
        void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled);

        /// <summary>
        /// Invoked when the agent is deactivated because there was a
        /// context switch from the currently active process to another process
        /// </summary>
        void OnFocusLost();

        /// <summary>
        /// Invoked when the currently active scanner is closed
        /// </summary>
        /// <param name="panelClass">name/class of the scanner</param>
        /// <param name="monitorInfo">Active focused window info</param>
        void OnPanelClosed(String panelClass, WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Invoked to pause the agent
        /// </summary>
        void OnPause();

        /// <summary>
        /// Invoked to resume a paused agent
        /// </summary>
        void OnResume();

        /// <summary>
        /// Invoked when there is a request to run a command.  Set handled
        /// to true if the command was handled, false otherwise
        /// </summary>
        /// <param name="command">The command verb</param>
        /// <param name="arg">optional arguments</param>
        /// <param name="handled">set appropriately</param>
        void OnRunCommand(String command, object arg, ref bool handled);

        /// <summary>
        /// Invoked before the agent is deactivated.  Return true if it is
        /// OK to deactivate the agent, false otherwise
        /// </summary>
        /// <param name="newAgent">Agent that will be activated</param>
        /// <returns>true/false</returns>
        bool QueryAgentSwitch(IApplicationAgent newAgent);
    }
}