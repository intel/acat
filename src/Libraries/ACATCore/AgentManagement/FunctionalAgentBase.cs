////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System.Threading;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Base class for all functional agents
    /// </summary>
    public abstract class FunctionalAgentBase : GenericAppAgentBase, IFunctionalAgent
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected FunctionalAgentBase()
        {
            CloseEvent = new AutoResetEvent(false);
            ExitCode = CompletionCode.None;
        }

        /// <summary>
        /// Gets the event that is set when the functional
        /// agent exits.  After activating the functional
        /// agent, the caller can wait on this event for
        /// the fucntional agent to quit
        /// </summary>
        public AutoResetEvent CloseEvent { get; private set; }

        /// <summary>
        /// Exit code of the functional agent.
        /// </summary>
        public virtual CompletionCode ExitCode { get; set; }

        /// <summary>
        /// Command to execute after the functional agent exits.
        /// The command is executed by ACAT. For eg, the user wants
        /// to activate the talk window from within a functional agent.
        /// </summary>
        public PostExitCommand ExitCommand { get; set; }

        /// <summary>
        /// Is the agent now active?  Set this to true in the Activate()
        /// function after the initialization has completed
        /// </summary>
        public bool IsActive { get; protected set; }

        /// <summary>
        /// Gets or sets whether the functional agent is in
        /// the process of exiting or not
        /// </summary>
        public bool IsClosing { get; protected set; }

        /// <summary>
        /// Invoked when the functional agent is activated.
        /// </summary>
        /// <returns>trye b success</returns>
        public abstract bool Activate();

        /// <summary>
        /// Call this function from within the functional agent
        /// to exit.
        /// </summary>
        /// <returns>true on success</returns>
        public bool Close()
        {
            IsClosing = true;
            CloseEvent.Set();
            return true;
        }

        /// <summary>
        /// Invoked when there is a request to display a contexutal menu for
        /// the currently active process
        /// </summary>
        /// <param name="monitorInfo">Info  about the active process/window</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Invoked when there is a request to quit the functional agent.
        /// The agent MUST cleanup and quit when this is invoked
        /// </summary>
        /// <returns>true on success</returns>
        public abstract bool OnRequestClose();

        /// <summary>
        /// Invoked after the functional agent has
        /// closed to do post-close cleanup
        /// </summary>
        public virtual void PostClose()
        {
            IsClosing = false;
            IsActive = false;
        }
    }
}