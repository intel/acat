////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;

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
        /// Adds the indicated command to the list of commands. If a
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