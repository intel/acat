////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents log entry to audit events related to widget actuated
    /// </summary>
    public class AuditEventWidgetActuate: AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventWidgetActuate()
            : base("WidgetActuate")
        {
            WidgetName = UnknownValue;
            Value = UnknownValue;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
     
        public AuditEventWidgetActuate(String widgetName, String value)
            : base("WidgetActuate")
        {
            WidgetName = widgetName;
            Value = value;
        }

        /// <summary>
        /// Gets or sets  name of the switch
        /// </summary>
        public String WidgetName{ get; set; }

        public String Value { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(WidgetName, Value);
        }
    }
}