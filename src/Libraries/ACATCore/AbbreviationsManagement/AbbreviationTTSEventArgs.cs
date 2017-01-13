////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationTTSEventArgs.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.AbbreviationsManagement
{
    /// <summary>
    /// Represents the event args for a text to speech request for abbreviation
    /// expansion
    /// </summary>
    public class AbbreviationTTSEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="text">Text to convert to speech</param>
        public AbbreviationTTSEventArgs(String text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets the string to be converted to speech
        /// </summary>
        public String Text { get; private set; }
    }
}