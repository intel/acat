////////////////////////////////////////////////////////////////////////////
// <copyright file="InterpreterRunEventArgs.cs" company="Intel Corporation">
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