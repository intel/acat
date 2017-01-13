////////////////////////////////////////////////////////////////////////////
// <copyright file="PostExitCommand.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PanelManagement.CommandDispatcher;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents a command that should be executed AFTER a
    /// functional agent has exited.  For instance if a "File
    /// Browser" functional agent is active and the user wants to
    /// activate the Talk window.  The browser functional agent
    /// must exit first, and then request ACAT to activate the
    /// Talk window after the browser agent has exited
    /// This command facitilates this.
    /// </summary>
    public class PostExitCommand
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PostExitCommand()
        {
            Command = null;
            ContextSwitch = false;
        }

        /// <summary>
        /// Gets or sets the command to run
        /// </summary>
        public RunCommandHandler Command { get; set; }

        /// <summary>
        /// Gets or sets whether the command results in a context
        /// switch to a new window (eg activate the talk window)
        /// </summary>
        public bool ContextSwitch { get; set; }
    }
}