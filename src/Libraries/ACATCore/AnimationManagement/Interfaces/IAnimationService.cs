////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IAnimationService.cs
//
// Root service for the animation engine. Singleton: one per application lifetime.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Root service for the animation engine.
    /// Singleton: one instance per application lifetime.
    /// Registered in DI via AddAnimationEngine().
    /// </summary>
    public interface IAnimationService
    {
        /// <summary>
        /// Creates and starts a new scan session for the given panel.
        /// The session is owned by the caller; Dispose() releases all resources.
        /// Thread-safety: safe to call from any thread.
        /// </summary>
        /// <param name="rootWidget">The root widget object for the panel (used by the renderer).</param>
        /// <param name="config">Animation configuration loaded by the caller.</param>
        /// <param name="strategyName">
        ///   Name of the IScanModeStrategy to use ("auto", "manual", "step").
        ///   Defaults to "auto" if null.
        /// </param>
        /// <param name="renderer">
        ///   Optional per-session highlight renderer. When provided, overrides the
        ///   singleton renderer registered in DI. Allows each panel to supply its
        ///   own renderer with panel-specific widget-lookup callbacks.
        ///   When null the singleton renderer is used; if that is also null an
        ///   <see cref="InvalidOperationException"/> is thrown.
        /// </param>
        IAnimationSession CreateSession(object rootWidget, AnimationConfig config, string strategyName, IHighlightRenderer renderer = null);

        /// <summary>
        /// Disposes all active sessions created by this service instance.
        /// Called during application shutdown.
        /// </summary>
        void Shutdown();
    }
}
