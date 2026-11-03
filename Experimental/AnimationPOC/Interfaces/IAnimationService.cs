////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IAnimationService.cs
//
// Root service for the animation engine. Singleton: one per application lifetime.
// Designed per the Issue #207 design spec §5.1.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Config;

namespace ACAT.Experimental.AnimationPOC.Interfaces
{
    /// <summary>
    /// Root service for the animation engine.
    /// Singleton: one instance per application lifetime.
    /// Registered in DI via AddAnimationPOCServices().
    /// </summary>
    public interface IAnimationService
    {
        /// <summary>
        /// Creates and starts a new scan session for the given panel.
        /// The session is owned by the caller; Dispose() releases all resources.
        /// Thread-safety: safe to call from any thread.
        /// </summary>
        /// <param name="config">Animation configuration loaded by the caller.</param>
        /// <param name="strategyName">
        ///   Name of the IScanModeStrategy to use ("auto", "manual", "bci").
        ///   Defaults to "auto" if null.
        /// </param>
        IAnimationSession CreateSession(AnimationConfig config, string strategyName = null);

        /// <summary>
        /// Disposes all active sessions created by this service instance.
        /// Called during application shutdown.
        /// </summary>
        void Shutdown();
    }
}
