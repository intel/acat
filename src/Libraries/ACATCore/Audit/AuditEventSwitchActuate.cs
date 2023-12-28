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