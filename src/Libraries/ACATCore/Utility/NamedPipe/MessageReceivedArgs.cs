////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Utility.NamedPipe
{
    /// <summary>
    /// Message received event arguments.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">
        /// The message received.
        /// </param>
        public MessageReceivedEventArgs(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.Message = message;
            }
        }

        /// <summary>
        /// Gets the message received from the named-pipe.
        /// </summary>
        public string Message { get; private set; }
    }
}