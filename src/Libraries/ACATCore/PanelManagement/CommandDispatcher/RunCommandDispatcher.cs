////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

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

        public virtual bool Dispatch2(Object source, String command, ref bool handled)
        {
            IRunCommandHandler runCommand = Commands.Get(command);

            bool retVal = runCommand != null && runCommand.Execute2(source, ref handled);

            return retVal;
        }
    }
}