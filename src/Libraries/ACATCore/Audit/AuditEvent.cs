////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEvent.cs" company="Intel Corporation">
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
    /// Represents a general audit log event. Use this when an
    /// event-specific class does not exist for the event being
    /// audited.
    /// </summary>
    public class AuditEvent : AuditEventBase
    {
        /// <summary>
        /// Data for the audit log
        /// </summary>
        private readonly object[] _args;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEvent()
            : base(UnknownValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="eventType">type of the audit event</param>
        /// <param name="args">event args</param>
        public AuditEvent(String eventType, params object[] args)
            : base(eventType)
        {
            _args = args;
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(_args);
        }
    }
}