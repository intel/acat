////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowCreatedEventArgs.cs" company="Intel Corporation">
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
using System.Windows.Forms;

namespace ACAT.Lib.Core.TalkWindowManagement
{
    /// <summary>
    /// Event argument for event raised when the talk window is created
    /// </summary>
    public class TalkWindowCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="talkWindowForm">The talk window form</param>
        public TalkWindowCreatedEventArgs(Form talkWindowForm)
        {
            TalkWindowForm = talkWindowForm;
        }

        /// <summary>
        /// Gets the talk window form
        /// </summary>
        public Form TalkWindowForm { get; private set; }
    }
}