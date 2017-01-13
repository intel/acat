////////////////////////////////////////////////////////////////////////////
// <copyright file="PlayerStateChangedEventArgs.cs" company="Intel Corporation">
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