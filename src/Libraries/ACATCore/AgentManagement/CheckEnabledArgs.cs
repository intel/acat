////////////////////////////////////////////////////////////////////////////
// <copyright file="CheckEnabledArgs.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Argument for the CheckWidgetEnabled function.  This class
    /// contains contextual info about the currently active window and
    /// also the widget that needs to be enabled or disabled.  Depending
    /// on the context, the application agent decides whether to enable
    /// or disable the widget.  If Handled is set to false, it means
    /// that agent does not care for the widget so someone else can handle it.
    /// </summary>
    public class CheckEnabledArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="widget">Scanner button that needs to be enabled/disabled</param>
        public CheckEnabledArgs(WindowActivityMonitorInfo monitorInfo, Widget widget)
        {
            Handled = false;
            Enabled = false;
            Widget = widget;
            MonitorInfo = monitorInfo;
        }

        /// <summary>
        /// Gets or sets the Enabled state of the widget
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets whether this was handled or not
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets active window info
        /// </summary>
        public WindowActivityMonitorInfo MonitorInfo { get; private set; }

        /// <summary>
        /// Gets or sets the scanner button control
        /// </summary>
        public Widget Widget { get; private set; }
    }
}