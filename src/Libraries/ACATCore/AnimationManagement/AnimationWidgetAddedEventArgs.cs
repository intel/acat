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
    /// Event args for the event that is raised when an animation
    /// widget is added to the animation sequence. This gives the
    /// app a chance to do its own initialization.
    /// </summary>
    public class AnimationWidgetAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializaes the object
        /// </summary>
        /// <param name="widget">the animation widget</param>
        public AnimationWidgetAddedEventArgs(AnimationWidget widget)
        {
            AnimWidget = widget;
        }

        /// <summary>
        /// Gets the animation widget object that was added.
        /// </summary>
        public AnimationWidget AnimWidget { get; private set; }
    }
}