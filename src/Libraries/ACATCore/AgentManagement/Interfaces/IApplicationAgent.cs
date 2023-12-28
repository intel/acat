////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;

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
    public interface IApplicationAgent : IDisposable, IExtension, ISupportsPreferences
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
        /// Check to see if a command should be
        /// enabled or not. This depends on the context.   The arg parameter
        /// contains the widget/command object in question.  For instance, if the
        /// talk window is empty, the "Clear talk window" button should be disabled.
        /// </summary>
        /// <param name="arg">Argument</param>
        void CheckCommandEnabled(CommandEnabledArg arg);

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