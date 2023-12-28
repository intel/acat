////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

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