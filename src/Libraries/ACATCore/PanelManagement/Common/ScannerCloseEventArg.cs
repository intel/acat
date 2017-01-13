////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerCloseEventArg.cs" company="Intel Corporation">
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
    /// Delegate for the event raised when the scanner is closed.
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="arg">event args</param>
    public delegate void ScannerClose(object sender, ScannerCloseEventArg arg);

    /// <summary>
    /// Argument for the event raised when the scanner is closed
    /// </summary>
    public class ScannerCloseEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panel">scanner being shown</param>
        public ScannerCloseEventArg(IPanel panel)
        {
            Scanner = panel;
        }

        /// <summary>
        /// Gets the scanner object
        /// </summary>
        public IPanel Scanner { get; internal set; }
    }
}