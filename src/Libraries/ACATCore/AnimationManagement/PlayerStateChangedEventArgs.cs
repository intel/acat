////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Event argument for the event raised when the player state changes
    /// </summary>
    public class PlayerStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the object
        /// </summary>
        /// <param name="oldState">previous state</param>
        /// <param name="newState">ne state</param>
        public PlayerStateChangedEventArgs(PlayerState oldState, PlayerState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        /// <summary>
        /// Gets the new player state
        /// </summary>
        public PlayerState NewState { get; private set; }

        /// <summary>
        /// Gets the previous player state
        /// </summary>
        public PlayerState OldState { get; private set; }
    }
}