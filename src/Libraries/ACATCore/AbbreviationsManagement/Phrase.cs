////////////////////////////////////////////////////////////////////////////
// <copyright file="Phrase.cs" company="Intel Corporation">
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
    /// Represents an phrase that will be converted to speech
    /// </summary>
    [Serializable]
    public class Phrase
    {
        /// <summary>
        /// Is this a favorite?
        /// </summary>
        public bool Favorite = false;

        /// <summary>
        /// Text of the phrase
        /// </summary>
        public String Text = String.Empty;

        /// <summary>
        /// Initalizes a new instance of the class
        /// </summary>
        /// <param name="text">Text of the phrase</param>
        /// <param name="favorite">Is this a favorite phrase</param>
        public Phrase(String text, bool favorite = false)
        {
            Text = text;
            Favorite = favorite;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public Phrase()
        {
        }
    }
}