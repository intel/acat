////////////////////////////////////////////////////////////////////////////
// <copyright file="RunCommandDispatcher.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement.CommandDispatcher
{
    /// <summary>
    /// Dispatches (executes) the command
    /// </summary>
    public class RunCommandDispatcher : IRunCommandDispatcher
    {
        /// <summary>
        /// Initializes a new instance of the class..
        /// </summary>
        /// <param name="scanner">Parent scanner</param>
        public RunCommandDispatcher(IScannerPanel scanner)
        {
            Scanner = scanner;
            Commands = new RunCommands(this);
        }

        /// <summary>
        /// Gets set of commands
        /// </summary>
        public RunCommands Commands { get; private set; }

        /// <summary>
        /// Gets parent scanner
        /// </summary>
        public IScannerPanel Scanner { get; set; }

        /// <summary>
        /// Dispatches the command by invoking the Execute
        /// function
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <returns>return value of the Execute function</returns>
        public static bool Dispatch(IRunCommandHandler command)
        {
            bool retVal = true;
            bool handled = false;

            if (command != null)
            {
                retVal = command.Execute(ref handled);
            }

            return retVal;
        }

        /// <summary>
        /// Executes the command indicated by the 'command' verb.
        /// Looks up the command table, if command if found, invokes
        /// the command handler's execute function
        /// </summary>
        /// <param name="command">command verb</param>
        /// <param name="handled">was it handled?</param>
        /// <returns>Result of Execute()</returns>
        public virtual bool Dispatch(String command, ref bool handled)
        {
            IRunCommandHandler runCommand = Commands.Get(command);

            bool retVal = runCommand != null && runCommand.Execute(ref handled);

            return retVal;
        }
    }
}