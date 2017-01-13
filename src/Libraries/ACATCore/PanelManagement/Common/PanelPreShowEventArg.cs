////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerShowEventArg.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Delegate for the event raised when the
    /// scanner is displayed
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="arg">event args</param>
    public delegate void PanelPreShow(object sender, PanelPreShowEventArg arg);

    /// <summary>
    /// Argument for the event raised just before when the panel is shown
    /// </summary>
    public class PanelPreShowEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panel">scanner being shown</param>
        public PanelPreShowEventArg(IPanel panel, DisplayModeTypes dislayMode)
        {
            Panel = panel;
            DisplayMode = dislayMode;
        }

        /// <summary>
        /// Gets DisplayMode for the panel
        /// </summary>
        public DisplayModeTypes DisplayMode { get; private set; }

        /// <summary>
        /// Gets the scanner object
        /// </summary>
        public IPanel Panel { get; internal set; }
    }
}