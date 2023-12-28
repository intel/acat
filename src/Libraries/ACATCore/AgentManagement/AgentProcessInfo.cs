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
    /// Represents process information used to identify which processes
    /// an agent supports
    /// </summary>
    public class AgentProcessInfo
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">name of the process</param>
        public AgentProcessInfo(String name)
        {
            Name = name;
            ExecutablePath = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">name of the process</param>
        /// <param name="exe">full path to the exe</param>
        public AgentProcessInfo(String name, String exe)
        {
            Name = name;
            ExecutablePath = exe;
        }

        /// <summary>
        /// Optional.  Path to the executable.  (What if there
        /// is another app called notepad?  Use this to ensure
        /// that it is the Windows notepad.
        /// </summary>
        public String ExecutablePath { get; set; }

        /// <summary>
        /// Name of the process. Eg. winword, notepad etc
        /// </summary>
        public String Name { get; set; }
    }
}