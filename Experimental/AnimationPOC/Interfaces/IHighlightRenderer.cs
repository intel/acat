////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IHighlightRenderer.cs
//
// Abstracts the mechanism used to visually highlight a widget during scanning.
// Extensibility point for WinForms (Phase A), DirectX BCI (Phase C), WinUI 3 (Phase 4).
// Designed per Issue #207 design spec §5.5.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Experimental.AnimationPOC.Interfaces
{
    /// <summary>Visual parameters for a single highlight step.</summary>
    public class HighlightStyle
    {
        /// <summary>If true, a beep sound should play when this widget is highlighted.</summary>
        public bool PlayBeep { get; set; }

        /// <summary>Optional color scheme identifier (drives future WinUI 3 brush).</summary>
        public string ColorScheme { get; set; }

        /// <summary>Duration in milliseconds for immediate-mode renderers (future use).</summary>
        public double DurationMs { get; set; }
    }

    /// <summary>
    /// Abstracts the visual highlighting mechanism used during scanning.
    ///
    /// Thread-safety: Render() and ClearHighlight() are always called on the UI thread.
    /// The AnimationSession is responsible for marshalling before calling these methods.
    ///
    /// The default POC implementation calls Action delegates provided by the demo form.
    /// </summary>
    public interface IHighlightRenderer
    {
        /// <summary>Applies the highlight visual to the named widget.</summary>
        void Render(string widgetName, HighlightStyle style);

        /// <summary>Removes the highlight visual from the named widget.</summary>
        void ClearHighlight(string widgetName);

        /// <summary>Removes highlight from all widgets in the panel.</summary>
        void ClearAll();
    }
}
