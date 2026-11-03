////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Core.AgentManagement
{
    /// <summary>
    /// Interface for AgentManager to support dependency injection.
    /// Manages all application and functional agents in ACAT.
    /// </summary>
    public interface IAgentManager : IDisposable
    {
        /// <summary>
        /// Event raised when focus changes on the windows desktop
        /// </summary>
        event FocusChanged EvtFocusChanged;

        /// <summary>
        /// Event raised when mouse is clicked on a non-scanner window
        /// </summary>
        event MouseEventHandler EvtNonScannerMouseDown;

        /// <summary>
        /// Event raised to request display of a scanner
        /// </summary>
        event PanelRequest EvtPanelRequest;

        /// <summary>
        /// Event raised before an agent is activated
        /// </summary>
        event EventHandler EvtPreActivateAgent;

        /// <summary>
        /// Event raised for scanner hit test
        /// </summary>
        event ScannerHitTest EvtScannerHitTest;

        /// <summary>
        /// Event raised when text changes in the active window
        /// </summary>
        event EventHandler EvtTextChanged;

        /// <summary>
        /// Gets the currently active agent
        /// </summary>
        IApplicationAgent ActiveAgent { get; }

        /// <summary>
        /// Gets or sets the current editing mode
        /// </summary>
        EditingMode CurrentEditingMode { get; set; }

        /// <summary>
        /// Gets or sets the default agent for context switch disable
        /// </summary>
        IApplicationAgent DefaultAgentForContextSwitchDisable { get; set; }

        /// <summary>
        /// Gets or sets whether app agent context switch is enabled
        /// </summary>
        bool EnableAppAgentContextSwitch { get; set; }

        /// <summary>
        /// Gets or sets whether contextual menus are enabled for dialogs
        /// </summary>
        bool EnableContextualMenusForDialogs { get; set; }

        /// <summary>
        /// Gets or sets whether contextual menus are enabled for menus
        /// </summary>
        bool EnableContextualMenusForMenus { get; set; }

        /// <summary>
        /// Gets the generic app agent
        /// </summary>
        IApplicationAgent GenericAppAgent { get; }

        /// <summary>
        /// Gets the keyboard interface
        /// </summary>
        IKeyboard Keyboard { get; }

        /// <summary>
        /// Gets the null agent
        /// </summary>
        IApplicationAgent NullAgent { get; }

        /// <summary>
        /// Gets the text changed notifications trigger lock
        /// </summary>
        TriggerLock TextChangedNotifications { get; }

        /// <summary>
        /// Activates the specified functional agent
        /// </summary>
        /// <param name="caller">calling agent</param>
        /// <param name="agent">functional agent to activate</param>
        /// <returns>task</returns>
        Task ActivateAgent(IApplicationAgent caller, IFunctionalAgent agent);

        /// <summary>
        /// Activates the specified functional agent
        /// </summary>
        /// <param name="agent">functional agent to activate</param>
        /// <returns>task</returns>
        Task ActivateAgent(IFunctionalAgent agent);

        /// <summary>
        /// Gets the context of the active agent
        /// </summary>
        /// <returns>agent context</returns>
        AgentContext ActiveContext();

        /// <summary>
        /// Adds an agent for the specified window handle
        /// </summary>
        /// <param name="handle">window handle</param>
        /// <param name="agent">agent to add</param>
        void AddAgent(IntPtr handle, IApplicationAgent agent);

        /// <summary>
        /// Checks if a functional agent can be activated
        /// </summary>
        /// <returns>true if can activate</returns>
        bool CanActivateFunctionalAgent();

        /// <summary>
        /// Checks if the specified command is enabled
        /// </summary>
        /// <param name="arg">command argument</param>
        void CheckCommandEnabled(CommandEnabledArg arg);

        /// <summary>
        /// Gets an agent by category
        /// </summary>
        /// <param name="category">category name</param>
        /// <returns>agent, null if not found</returns>
        IApplicationAgent GetAgentByCategory(String category);

        /// <summary>
        /// Gets an agent by name
        /// </summary>
        /// <param name="name">agent name</param>
        /// <returns>agent, null if not found</returns>
        IApplicationAgent GetAgentByName(String name);

        /// <summary>
        /// Gets a functional agent by name
        /// </summary>
        /// <param name="name">agent name</param>
        /// <returns>functional agent, null if not found</returns>
        IFunctionalAgent GetFunctionalAgentByName(String name);

        /// <summary>
        /// Gets the name of the current agent
        /// </summary>
        /// <returns>agent name</returns>
        String GetCurrentAgentName();

        /// <summary>
        /// Gets the collection of discovered agent extensions
        /// </summary>
        /// <returns>collection of extensions</returns>
        IEnumerable<object> GetExtensions();

        /// <summary>
        /// Initializes the agent manager
        /// </summary>
        /// <param name="extensionDirs">directories to search for agents</param>
        /// <returns>true on success</returns>
        bool Init(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Checks if the specified agent is the current agent
        /// </summary>
        /// <param name="agentName">agent name</param>
        /// <returns>true if current</returns>
        bool IsCurrentAgent(String agentName);

        /// <summary>
        /// Loads agent extensions from the specified directories
        /// </summary>
        /// <param name="extensionDirs">directories to search</param>
        /// <returns>true on success</returns>
        bool LoadExtensions(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Called when a panel is closed
        /// </summary>
        /// <param name="panelClass">panel class that was closed</param>
        void OnPanelClosed(String panelClass);

        /// <summary>
        /// Pauses panel change requests
        /// </summary>
        void PausePanelChangeRequests();

        /// <summary>
        /// Performs second phase initialization
        /// </summary>
        /// <returns>true on success</returns>
        bool PostInit();

        /// <summary>
        /// Removes an agent by name
        /// </summary>
        /// <param name="AgentName">agent name to remove</param>
        void RemoveAgent(string AgentName);

        /// <summary>
        /// Removes an agent by window handle
        /// </summary>
        /// <param name="handle">window handle</param>
        void RemoveAgent(IntPtr handle);

        /// <summary>
        /// Resumes panel change requests
        /// </summary>
        /// <param name="getActiveWindow">get active window after resume?</param>
        void ResumePanelChangeRequests(bool getActiveWindow = true);

        /// <summary>
        /// Runs the specified command
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was command handled?</param>
        void RunCommand(String command, ref bool handled);

        /// <summary>
        /// Runs the specified command with argument
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="arg">command argument</param>
        /// <param name="handled">was command handled?</param>
        void RunCommand(String command, object arg, ref bool handled);

        /// <summary>
        /// Shows the context menu
        /// </summary>
        void ShowContextMenu();
    }
}
