////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowActivityMonitorInfo.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Security.Permissions;
using System.Windows.Automation;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Encapsulates information about the currently active
    /// foreground window. Used by WindowActivityMonitor class
    /// to notify event subscribers about the currently focused
    /// window
    /// </summary>
    public class WindowActivityMonitorInfo
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WindowActivityMonitorInfo()
        {
            FgHwnd = IntPtr.Zero;
            Title = String.Empty;
            FgProcess = null;
            FocusedElement = null;
            IsNewFocusedElement = false;
            IsNewWindow = false;
        }

        /// <summary>
        /// Gets or sets the handle of the foreground window
        /// </summary>
        public IntPtr FgHwnd { get; set; }

        /// <summary>
        /// Gets or sets the process that owns the foreground window
        /// </summary>
        public Process FgProcess { get; set; }

        /// <summary>
        /// Gets or sets the element that has focus
        /// </summary>
        public AutomationElement FocusedElement { get; set; }

        /// <summary>
        /// Gets or sets whether focus has shifted to another
        /// control in the currently active window
        /// </summary>
        public bool IsNewFocusedElement { get; set; }

        /// <summary>
        /// Gets or sets whether the focus has shifted from the
        /// currently active window to a new window
        /// </summary>
        public bool IsNewWindow { get; set; }

        /// <summary>
        /// Gets or sets the title of the window
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>String represntation of the object</returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public override String ToString()
        {
            try
            {
                return "FgHwnd: " + FgHwnd +
                    ", title: " + Title +
                    ", fgProcess: " + FgProcess.ProcessName +
                    ", focusedClass: " + FocusedElement.Current.ClassName +
                    ", newWindow: " + IsNewWindow +
                    ", newFocus: " + IsNewFocusedElement;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}