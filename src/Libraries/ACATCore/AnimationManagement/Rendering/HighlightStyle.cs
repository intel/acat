////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// HighlightStyle.cs
//
// Data class carrying visual parameters for a single widget highlight step.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.AnimationManagement.Rendering
{
    /// <summary>
    /// Visual parameters for a single highlight step.
    /// Passed to <see cref="Interfaces.IHighlightRenderer.Render"/> when advancing
    /// to a widget during scanning.
    /// </summary>
    public class HighlightStyle
    {
        /// <summary>If true, a beep sound should play when this widget is highlighted.</summary>
        public bool PlayBeep { get; set; }

        /// <summary>Optional color scheme identifier (drives future theme brush).</summary>
        public string ColorScheme { get; set; }

        /// <summary>Duration in milliseconds for immediate-mode renderers (future use).</summary>
        public double DurationMs { get; set; }
    }
}
