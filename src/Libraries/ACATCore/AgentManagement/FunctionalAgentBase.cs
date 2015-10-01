////////////////////////////////////////////////////////////////////////////
// <copyright file="FunctionalAgentBase.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using ACAT.Lib.Core.PanelManagement;
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
        /// Gets or sets whether the functional agent is in
        /// the process of exiting or not
        /// </summary>
        public bool IsClosing { get; set; }

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
    }
}