////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IHighlightRenderer.cs
//
// Abstracts the mechanism used to visually highlight a widget during scanning.
// Extensibility point for WinForms (production) and test scenarios.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Rendering;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Abstracts the visual highlighting mechanism used during scanning.
    ///
    /// Thread-safety: Render() and ClearHighlight() are always called on the UI thread.
    /// The AnimationSession is responsible for marshalling before calling these methods.
    ///
    /// The default production implementation is WinFormsHighlightRenderer.
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
