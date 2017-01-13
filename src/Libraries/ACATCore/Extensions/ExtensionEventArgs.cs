////////////////////////////////////////////////////////////////////////////
// <copyright file="ExtensionEventArgs.cs" company="Intel Corporation">
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