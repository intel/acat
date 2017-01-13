////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventTextToSpeech.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents log entry of audit events related to
    /// text to speech conversions
    /// </summary>
    public class AuditEventTextToSpeech : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventTextToSpeech()
            : base("TextToSpeech")
        {
            SpeechEngine = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="speechEngine">name of the active speech engine</param>
        public AuditEventTextToSpeech(String speechEngine)
            : base("TextToSpeech")
        {
            SpeechEngine = speechEngine;
        }

        /// <summary>
        /// Gets or sets name of the active speech engine
        /// </summary>
        public String SpeechEngine { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(SpeechEngine);
        }
    }
}