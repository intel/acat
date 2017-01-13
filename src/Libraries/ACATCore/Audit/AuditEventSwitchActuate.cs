////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventSwitchActuate.cs" company="Intel Corporation">
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
    /// Represents log entry to audit events related to actuator
    /// switch triggers
    /// </summary>
    public class AuditEventSwitchActuate : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventSwitchActuate()
            : base("SwitchActuate")
        {
            SwitchName = UnknownValue;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="switchName">name of the actuator switch</param>
        /// <param name="action">switch action</param>
        /// <param name="actuator">the parent actuator</param>
        /// <param name="tag">user defined data</param>
        /// <param name="elapsedMilliSeconds">how long was the switch engaged?</param>
        public AuditEventSwitchActuate(String switchName, String action, String actuator, String tag, long elapsedMilliSeconds)
            : base("SwitchActuate")
        {
            SwitchName = switchName;
            Action = action;
            Actuator = actuator;
            Tag = tag;
            ElapsedMilliSeconds = elapsedMilliSeconds;
        }

        /// <summary>
        /// Gets or sets the switch action
        /// </summary>
        public String Action { get; set; }

        /// <summary>
        /// Gets or sets the parent actuator
        /// </summary>
        public String Actuator { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time
        /// </summary>
        public long ElapsedMilliSeconds { get; set; }

        /// <summary>
        /// Gets or sets  name of the switch
        /// </summary>
        public String SwitchName { get; set; }

        /// <summary>
        /// Gets or sets user defined data
        /// </summary>
        public String Tag { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(SwitchName, Action, Actuator, ElapsedMilliSeconds, Tag);
        }
    }
}