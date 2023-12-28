////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Event argument for the event that is raised when an
    /// application agent is deactivated
    /// </summary>
    public class AgentCloseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="result">Exit code</param>
        public AgentCloseEventArgs(bool result)
        {
            Result = result;
        }

        /// <summary>
        /// Gets the exit code
        /// </summary>
        public bool Result { get; private set; }
    }
}