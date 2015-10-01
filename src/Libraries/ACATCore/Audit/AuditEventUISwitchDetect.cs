////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventUISwitchDetect.cs" company="Intel Corporation">
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
    /// Represents log entry of audit events related to
    /// switch events received by a scanner during an
    /// animation sequence
    /// </summary>
    public class AuditEventUISwitchDetect : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventUISwitchDetect()
            : base("UISwitchDetect")
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
        public AuditEventUISwitchDetect(String switchName, String panelClass, String widgetType, String widgetName)
            : base("UISwitchDetect")
        {
            SwitchName = switchName;
            PanelName = panelClass;
            WidgetType = widgetType;
            WidgetName = widgetName;
        }

        /// <summary>
        /// Gets or sets name of the scanner
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// Gets or sets name of the switch
        /// </summary>
        public String SwitchName { get; set; }

        /// <summary>
        /// Gets or sets name of the highlighted widget
        /// </summary>
        public String WidgetName { get; set; }

        /// <summary>
        /// Gets or sets type of the highlighted widget when the
        /// switch was detected
        /// </summary>
        public string WidgetType { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(SwitchName, PanelName, WidgetType, WidgetName);
        }
    }
}