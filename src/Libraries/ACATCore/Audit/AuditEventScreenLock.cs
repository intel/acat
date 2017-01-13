////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventScreenLock.cs" company="Intel Corporation">
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
    /// Represents log entry to audit events related to screen-lock where
    /// the user can lock the display and unlock it by typing in a pin
    /// </summary>
    public class AuditEventScreenLock : AuditEventBase
    {
        /// <summary>
        /// Type of event associated with the Screen Lock feature
        /// </summary>
        private readonly String _eventType;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventScreenLock()
            : base("ScreenLock")
        {
            _eventType = "unknown";
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="eventType">what happened on the Screen lock?</param>
        public AuditEventScreenLock(String eventType)
            : base("ScreenLock")
        {
            _eventType = eventType;
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(_eventType);
        }
    }
}