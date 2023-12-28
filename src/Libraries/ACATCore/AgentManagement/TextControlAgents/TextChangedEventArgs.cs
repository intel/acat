////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.AgentManagement.TextInterface
{
    /// <summary>
    /// Argument to the event that's raised when the text changes
    /// are detected in the target text control
    /// </summary>
    public class TextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="textAgent">The objet that triggered the event</param>
        public TextChangedEventArgs(ITextControlAgent textAgent)
        {
            TextInterface = textAgent;
        }

        /// <summary>
        /// Returns the source text control interface that raised
        /// the event
        /// </summary>
        public ITextControlAgent TextInterface { get; private set; }
    }
}