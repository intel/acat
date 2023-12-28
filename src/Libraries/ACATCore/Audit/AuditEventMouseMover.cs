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
    /// Represents log entry to audit events related to
    /// mouse navigation using the mouse mover
    /// </summary>
    public class AuditEventMouseMover : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventMouseMover()
            : base("MouseMover")
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="direction">which way is the mover moving?</param>
        public AuditEventMouseMover(String direction)
            : base("MouseMover")
        {
            Direction = direction;
        }

        /// <summary>
        /// Gets or sets the direction of the movement
        /// </summary>
        public String Direction { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(Direction);
        }
    }
}