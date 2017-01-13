////////////////////////////////////////////////////////////////////////////
// <copyright file="CommandManager.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using System.IO;

namespace ACAT.Lib.Core.CommandManagement
{
    /// <summary>
    /// This class is work-in-progress
    /// </summary>
    public class CommandManager
    {
        private static readonly CommandManager _instance = new CommandManager();

        /// <summary>
        /// The table containing a list of commands with their descriptions
        /// </summary>
        public CmdDescriptorTable AppCommandTable;

        private CommandManager()
        {
        }

        public static CommandManager Instance
        {
            get { return _instance; }
        }

        public bool Init()
        {
            bool retval = true;

            return retval;
        }
    }
}