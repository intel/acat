////////////////////////////////////////////////////////////////////////////
// <copyright file="ISupportsStatusBar.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Interface to indicate whether the scanner
    /// supports a status bar.  The status bar
    /// has controls to indicate the current key
    /// pressed status of Shift, Alt, Ctrl, etc.
    /// The ScannerStatusBar class has a collection of
    /// UI controls to display the status of
    /// these keys.
    /// </summary>
    public interface ISupportsStatusBar
    {
        /// <summary>
        /// Gets the status bar
        /// </summary>
        ScannerStatusBar ScannerStatusBar { get; }
    }
}