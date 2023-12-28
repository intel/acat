////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.CommandManagement
{
    /// <summary>
    /// This class is work-in-progress
    /// </summary>
    public class CommandManager
    {
        /// <summary>
        /// The table containing a list of commands with their descriptions
        /// </summary>
        public CmdDescriptorTable AppCommandTable;

        private static readonly CommandManager _instance = new CommandManager();

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