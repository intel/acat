////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventFocusChanged.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics.CodeAnalysis;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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