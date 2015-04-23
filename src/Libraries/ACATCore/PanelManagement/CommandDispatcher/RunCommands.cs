////////////////////////////////////////////////////////////////////////////
// <copyright file="RunCommands.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement.CommandDispatcher
{
    /// <summary>
    /// Represents a mapped list of commands and their respective
    /// handlers.
    /// </summary>
    public class RunCommands
    {
        /// <summary>
        /// Command dispatcher object. Caller can set this and the dispatcher
        /// will be called to dispatch the command.
        ///
        /// </summary>
        private readonly IRunCommandDispatcher _dispatcher;

        /// <summary>
        /// Table that maps the command with its handler
        /// </summary>
        private readonly Dictionary<String, IRunCommandHandler> _runCommandLookupTable = new Dictionary<String, IRunCommandHandler>();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="dispatcher">command dispatcher</param>
        public RunCommands(IRunCommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Returns the list of command handlers
        /// </summary>
        public IEnumerable<IRunCommandHandler> Commands
        {
            get { return _runCommandLookupTable.Values; }
        }

        /// <summary>
        /// Add the indicated command to the list of commands. If a
        /// handler for the command already exists, it is replaced
        /// </summary>
        /// <param name="runCommandHandler">Handler to add</param>
        /// <returns>true on success</returns>
        public bool Add(RunCommandHandler runCommandHandler)
        {
            bool ret = true;

            try
            {
                runCommandHandler.Dispatcher = _dispatcher;
                if (!_runCommandLookupTable.ContainsKey(runCommandHandler.Command))
                {
                    _runCommandLookupTable.Add(runCommandHandler.Command, runCommandHandler);
                }
                else
                {
                    _runCommandLookupTable[runCommandHandler.Command] = runCommandHandler;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Looks up the table and returns the command handler for
        /// the indicated command
        /// </summary>
        /// <param name="command">command to look for</param>
        /// <returns>command handler (null if not found)</returns>
        public IRunCommandHandler Get(String command)
        {
            return _runCommandLookupTable.ContainsKey(command) ? _runCommandLookupTable[command] : null;
        }
    }
}