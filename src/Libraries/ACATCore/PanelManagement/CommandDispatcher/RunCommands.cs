////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.PanelManagement.CommandDispatcher
{
    /// <summary>
    /// Represents a mapped list of commands and their respective
    /// handlers.
    /// </summary>
    public class RunCommands
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(RunCommands).Name);

        /// <summary>
        /// Command dispatcher object. Caller can set this and the dispatcher
        /// will be called to dispatch the command.
        ///
        /// </summary>
        private readonly IRunCommandDispatcher _dispatcher;

        /// <summary>
        /// Table that maps the command with its handler
        /// </summary>
        private readonly Dictionary<String, IRunCommandHandler> _runCommandLookupTable = new();

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
        public bool Add(RunCommandHandler handler)
        {
            bool ret = true;

            try
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                if (string.IsNullOrWhiteSpace(handler.Command))
                    throw new ArgumentException("Command name cannot be null or whitespace.", nameof(handler.Command));

                handler.Dispatcher = _dispatcher;
                _runCommandLookupTable[handler.Command] = handler;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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