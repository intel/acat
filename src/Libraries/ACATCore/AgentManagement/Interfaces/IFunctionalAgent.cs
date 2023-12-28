////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// All functional agents must derive from this interface.  Functional agents
    /// provide specific functionality or tasks, such as, browsing files, switching
    /// windows etc.
    /// </summary>
    public interface IFunctionalAgent : IApplicationAgent
    {
        /// <summary>
        /// The completion code
        /// </summary>
        CompletionCode ExitCode { get; set; }

        /// <summary>
        /// Command to execute after the functional agent exits.
        /// The command is executed by ACAT. For eg, the user wants
        /// to activate the talk window from within a functional agent.
        /// </summary>
        PostExitCommand ExitCommand { get; set; }

        /// <summary>
        /// Is the agent now active?  Set this to true in the Activate()
        /// function after the initialization has completed
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Is the agent closing?
        /// </summary>
        bool IsClosing { get; }

        /// <summary>
        /// Invoked when the functional agent is activated.
        /// </summary>
        /// <returns>true on success</returns>
        bool Activate();

        /// <summary>
        /// Call this function to exit the functional agent
        /// </summary>
        /// <returns>true on success</returns>
        bool Close();

        /// <summary>
        /// Invoked when there is a request to quit the functional agent.
        /// The agent MUST cleanup and quit when this is invoked
        /// </summary>
        /// <returns>true on success</returns>
        bool OnRequestClose();

        /// <summary>
        /// Invoked after the functional agent has
        /// closed to do post-close cleanup
        /// </summary>
        void PostClose();
    }
}