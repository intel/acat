////////////////////////////////////////////////////////////////////////////
// <copyright file="CheckEnabledArgs.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Argument for the CheckCommandEnabled function.  This class
    /// contains contextual info about the currently active window and
    /// also the widget that needs to be enabled or disabled.  Depending
    /// on the context, the application agent decides whether to enable
    /// or disable the widget.  If Handled is set to false, it means
    /// that agent does not care for the widget so someone else can handle it.
    /// </summary>
    public class CommandEnabledArg
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="widget">Scanner button that needs to be enabled/disabled</param>
        public CommandEnabledArg(WindowActivityMonitorInfo monitorInfo, String command, Widget widget = null)
        {
            Handled = false;
            Enabled = false;
            Command  = command;
            MonitorInfo = monitorInfo;
            Widget = widget;
        }
        
        /// <summary>
        /// Gets or sets the Enabled state of the widget
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets whether this was handled or not
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets active window info
        /// </summary>
        public WindowActivityMonitorInfo MonitorInfo { get; private set; }

        /// <summary>
        /// Gets the command
        /// </summary>
        public String Command{ get; private set; }

        /// <summary>
        /// Optional, if a widget is associated with the command
        /// </summary>
        public Widget Widget { get; private set; }
    }
}