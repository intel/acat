////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents log entry for auditing context switch to
    /// another window.
    /// </summary>
    public class AuditEventActiveWindowChange : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventActiveWindowChange()
            : base("ActiveWindowChange")
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="processName">name of the new process</param>
        /// <param name="windowTitle">title of the new windw</param>
        public AuditEventActiveWindowChange(string processName, string windowTitle)
            : base("ActiveWindowChange")
        {
            ProcessName = processName;
            WindowTitle = windowTitle;
        }

        /// <summary>
        /// Gets name of the process
        /// </summary>
        public String ProcessName { get; set; }

        /// <summary>
        /// Gets title of the window
        /// </summary>
        public String WindowTitle { get; set; }

        /// <summary>z
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(ProcessName, WindowTitle);
        }
    }
}