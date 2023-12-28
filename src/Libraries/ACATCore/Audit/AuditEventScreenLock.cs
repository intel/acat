////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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