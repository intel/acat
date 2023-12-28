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
    /// Represents log entry to audit events related to the scanner
    /// - show, close, hide etc.
    /// </summary>
    public class AuditEventScannerActivity : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventScannerActivity()
            : base("ScannerActivity")
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="eventType">type of the scanner event</param>
        /// <param name="panelClass">scanner class</param>
        /// <param name="panelName">scanner name</param>
        /// <param name="title">scanner title</param>
        public AuditEventScannerActivity(String eventType, String panelClass, String panelName, String title = "null")
            : base("ScannerActivity")
        {
            EventType = eventType;
            PanelClass = panelClass;
            PanelName = panelName;
            Title = title;
        }

        /// <summary>
        /// Gets or sets scanner class
        /// </summary>
        public string PanelClass { get; set; }

        /// <summary>
        /// Gets or sets scanner name
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// Gets or sets scanner title
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(EventType, PanelClass, PanelName, Title);
        }
    }
}