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
    /// Represents a log entry to audit events related to
    /// focus change between window elements in the target app window.
    /// For instance, user is navigating a dialog box.
    /// </summary>
    public class AuditEventFocusChanged : AuditEventBase
    {
        /// <summary>
        /// value is unknown
        /// </summary>
        private const String Unknown = "unknown";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventFocusChanged()
            : base("FocusChanged")
        {
            Title = Unknown;
            ProcessName = Unknown;
            ClassName = Unknown;
            ControlType = Unknown;
            AutomationId = Unknown;
            NewFocusElement = Unknown;
            IsMinimized = Unknown;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="title">window title of the app window</param>
        /// <param name="process">active process</param>
        /// <param name="className">window class</param>
        /// <param name="controlType">control type of element</param>
        /// <param name="automationId">automation id of element</param>
        /// <param name="newFocusElement">name of focused element</param>
        /// <param name="isMinimized">window minimized?</param>
        public AuditEventFocusChanged(
                String title,
                String process,
                String className,
                String controlType,
                String automationId,
                String newFocusElement,
                String isMinimized)
            : base("FocusChanged")
        {
            Title = title;
            ProcessName = process;
            ClassName = className;
            ControlType = controlType;
            AutomationId = automationId;
            NewFocusElement = newFocusElement;
            IsMinimized = isMinimized;
        }

        /// <summary>
        /// Gets or sets the automation id
        /// </summary>
        public String AutomationId { get; set; }

        /// <summary>
        /// Gets or sets the windowclass
        /// </summary>
        public String ClassName { get; set; }

        /// <summary>
        /// Gets or sets the control type
        /// </summary>
        public String ControlType { get; set; }

        /// <summary>
        /// Gets or sets whether the window is minimzied
        /// </summary>
        public String IsMinimized { get; set; }

        /// <summary>
        /// Gets or sets the name of the focused element
        /// </summary>
        public String NewFocusElement { get; set; }

        /// <summary>
        /// Gets or sets the process name
        /// </summary>
        public String ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the window title
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(Title, ProcessName, ClassName, ControlType, AutomationId, NewFocusElement, IsMinimized);
        }
    }
}