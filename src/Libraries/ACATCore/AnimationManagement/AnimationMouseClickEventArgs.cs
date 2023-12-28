////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Interpreter;
using System;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Event args for the mouse click event
    /// </summary>
    public class AnimationMouseClickEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the object
        /// </summary>
        /// <param name="animationWidget">animation widget</param>
        /// <param name="onMouseClick">the code for handling the click</param>
        public AnimationMouseClickEventArgs(AnimationWidget animationWidget, PCode onMouseClick)
        {
            AnimationWidget = animationWidget;
            OnMouseClick = onMouseClick;
        }

        /// <summary>
        /// Gets the animation widget which rasied the event
        /// </summary>
        public AnimationWidget AnimationWidget { get; private set; }

        /// <summary>
        /// Gets the PCode associated with the mouse click event
        /// </summary>
        public PCode OnMouseClick { get; private set; }
    }
}