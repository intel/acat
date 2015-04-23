////////////////////////////////////////////////////////////////////////////
// <copyright file="IFunctionalAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement;

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
    }
}