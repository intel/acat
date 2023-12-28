////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.Extensions
{
    /// <summary>
    /// Used as arugment to events rasised by the extension invoker
    /// </summary>
    public class ExtensionEventArgs : EventArgs, IExtension
    {
        /// <summary>
        /// The extension invoker object
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="eventType">Type of the event to be raised</param>
        public ExtensionEventArgs(String eventType)
        {
            _invoker = new ExtensionInvoker(this);
            EventType = eventType;
        }

        /// <summary>
        /// Returns the IDescriptor object
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the type of the event
        /// </summary>
        public String EventType { get; private set; }

        /// <summary>
        /// Returns the invoker object
        /// </summary>
        /// <returns>invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }
    }
}