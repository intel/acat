////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// WinFormsHighlightRenderer.cs
//
// Production WinForms implementation of IHighlightRenderer.
// Uses callback actions provided by the host form/control to apply and
// remove visual highlights on widgets.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Interfaces;
using System;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement.Rendering
{
    /// <summary>
    /// WinForms implementation of <see cref="IHighlightRenderer"/>.
    ///
    /// The host (scanner form or user control) supplies render and clear callbacks
    /// so that the animation engine remains decoupled from WinForms widget types.
    ///
    /// Thread-safety: all methods must be called on the UI thread. AnimationSession
    /// is responsible for marshalling to the UI thread before calling these methods.
    /// </summary>
    public class WinFormsHighlightRenderer : IHighlightRenderer
    {
        private readonly Action<string, HighlightStyle> _renderCallback;
        private readonly Action<string> _clearCallback;
        private readonly Action _clearAllCallback;
        private readonly HashSet<string> _highlighted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new WinFormsHighlightRenderer with the given callbacks.
        /// </summary>
        /// <param name="renderCallback">
        ///   Invoked with (widgetName, style) when a widget should be highlighted.
        /// </param>
        /// <param name="clearCallback">
        ///   Invoked with (widgetName) when a widget's highlight should be removed.
        /// </param>
        /// <param name="clearAllCallback">
        ///   Invoked when all highlights should be removed from the panel.
        /// </param>
        public WinFormsHighlightRenderer(
            Action<string, HighlightStyle> renderCallback,
            Action<string> clearCallback,
            Action clearAllCallback)
        {
            _renderCallback = renderCallback ?? throw new ArgumentNullException(nameof(renderCallback));
            _clearCallback = clearCallback ?? throw new ArgumentNullException(nameof(clearCallback));
            _clearAllCallback = clearAllCallback ?? throw new ArgumentNullException(nameof(clearAllCallback));
        }

        /// <inheritdoc/>
        public void Render(string widgetName, HighlightStyle style)
        {
            if (string.IsNullOrEmpty(widgetName)) return;
            _highlighted.Add(widgetName);
            _renderCallback(widgetName, style ?? new HighlightStyle());
        }

        /// <inheritdoc/>
        public void ClearHighlight(string widgetName)
        {
            if (string.IsNullOrEmpty(widgetName)) return;
            _highlighted.Remove(widgetName);
            _clearCallback(widgetName);
        }

        /// <inheritdoc/>
        public void ClearAll()
        {
            _highlighted.Clear();
            _clearAllCallback();
        }
    }
}
