////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Interpreter
{
    /// <summary>
    /// Arguments for event raised to "Run" a command
    /// </summary>
    public class InterpreterRunEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="script">the command to execute</param>
        public InterpreterRunEventArgs(String script)
        {
            Script = script;
        }

        /// <summary>
        /// Gets the script
        /// </summary>
        public String Script { get; private set; }
    }
}