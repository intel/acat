////////////////////////////////////////////////////////////////////////////
// <copyright file="FocusChangedEventArgs.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Event args for when the focus changes in Windows.r Focus
    /// can change because active window changed, or within a window,
    /// focus changed to a different control
    /// </summary>
    public class FocusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialzies an instance of the class
        /// </summary>
        /// <param name="monitorInfo">Focused element info</param>
        public FocusChangedEventArgs(WindowActivityMonitorInfo monitorInfo)
        {
            WindowActivityInfo = monitorInfo;
        }

        /// <summary>
        /// Gets the focused element information
        /// </summary>
        public WindowActivityMonitorInfo WindowActivityInfo { get; private set; }
    }
}