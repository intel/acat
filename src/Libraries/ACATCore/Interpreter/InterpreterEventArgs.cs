////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Lib.Core.Interpreter
{
    /// <summary>
    /// Event args for an interpreter event to execute a PCode
    /// </summary>
    public class InterpreterEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="args"></param>
        public InterpreterEventArgs(List<String> args)
        {
            Args = args;
        }

        /// <summary>
        /// Gets the list of arguments for the PCode
        /// </summary>
        public List<String> Args { get; private set; }
    }
}