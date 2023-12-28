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