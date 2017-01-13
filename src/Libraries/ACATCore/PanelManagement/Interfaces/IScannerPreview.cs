////////////////////////////////////////////////////////////////////////////
// <copyright file="IScannerPreview.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Interface used to display the scanner in the preview
    /// mode to enable the user to select the appropriate
    /// size of the scanner.  User can scale the size up
    /// or down and save the desired setting. Alphabet scanners
    /// typically implement this interface to allow the user
    /// to resize the scanner to a preferred size.
    /// </summary>
    public interface IScannerPreview
    {
        /// <summary>
        /// Gets or sets whether the scanner is in preview
        /// mode or not
        /// </summary>
        bool PreviewMode { get; set; }

        /// <summary>
        /// Restores the default size of the scanner
        /// </summary>
        void RestoreDefaults();

        /// <summary>
        /// Saves the current scale level
        /// </summary>
        void SaveScaleSetting();

        /// <summary>
        /// Saves the current setting as default
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Scales the size to the default size
        /// </summary>
        void ScaleDefault();

        /// <summary>
        /// Scales the size down by one step
        /// </summary>
        void ScaleDown();

        /// <summary>
        /// Scales the size up by one step
        /// </summary>
        void ScaleUp();
    }
}