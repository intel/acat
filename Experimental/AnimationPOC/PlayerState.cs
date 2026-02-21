////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// PlayerState.cs
//
// Animation player / session state machine states.
// Mirrors the existing PlayerState enum in ACATCore.AnimationManagement
// for backward-compatibility reference.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Experimental.AnimationPOC
{
    /// <summary>
    /// State of an <see cref="Interfaces.IAnimationSession"/>.
    /// </summary>
    public enum PlayerState
    {
        /// <summary>Initial state — session not yet started.</summary>
        Unknown,

        /// <summary>Session has been stopped or has not been started.</summary>
        Stopped,

        /// <summary>Session is paused; timer is disabled; widget remains highlighted.</summary>
        Paused,

        /// <summary>Session is actively scanning.</summary>
        Running,

        /// <summary>Sequence completed all iterations; transitioning to next or stopping.</summary>
        Timeout,

        /// <summary>User interrupted the scan via actuator input.</summary>
        Interrupted
    }
}
