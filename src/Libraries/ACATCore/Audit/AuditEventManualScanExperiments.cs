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
    /// Represents log entry of audit events related to
    /// switch events received by a scanner during an
    /// animation sequence
    /// </summary>
    public class AuditEventManualScanExperiments : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventManualScanExperiments()
            : base("ManualScanExperiments")
        {
            SwitchName = UnknownValue;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="switchName">name of the actuator switch</param>
        /// <param name="panelClass">name/class of the active scanner</param>
        /// <param name="widgetType">type of the highlighted widget</param>
        /// <param name="widgetName">name of the highlighted widget</param>
        public AuditEventManualScanExperiments(String switchName, String panelClass, String widgetName, String widgetValue, String widgetCommand, long elapsedMs)
            : base("ManualScanExperiments")
        {
            SwitchName = switchName;
            PanelName = panelClass;
            WidgetName = widgetName;
            WidgetValue = widgetValue;
            WidgetCommand = widgetCommand;
            ElapsedMs = elapsedMs;
        }

        /// <summary>
        /// Miliseconds elapsed since last instruction
        /// </summary>
        public long ElapsedMs { get; set; }

        /// <summary>
        /// Gets or sets name of the scanner
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// Gets or sets name of the switch
        /// </summary>
        public String SwitchName { get; set; }

        /// <summary>
        /// Gets or sets the command of the highlighted widget when the
        /// switch was detected
        /// </summary>
        public string WidgetCommand { get; set; }

        /// <summary>
        /// Gets or sets name of the highlighted widget
        /// </summary>
        public String WidgetName { get; set; }

        /// <summary>
        /// Gets or sets value of the highlighted widget when the
        /// switch was detected
        /// </summary>
        public string WidgetValue { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(SwitchName, PanelName, WidgetName, WidgetValue, WidgetCommand, ElapsedMs.ToString());
        }
    }
}