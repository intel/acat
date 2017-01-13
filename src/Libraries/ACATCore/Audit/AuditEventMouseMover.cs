////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventMouseMover.cs" company="Intel Corporation">
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