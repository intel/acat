////////////////////////////////////////////////////////////////////////////
// <copyright file="TextChangedEventArgs.cs" company="Intel Corporation">
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