////////////////////////////////////////////////////////////////////////////
// <copyright file="IFunctionalAgent.cs" company="Intel Corporation">
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