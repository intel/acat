////////////////////////////////////////////////////////////////////////////
// <copyright file="IScannerPanel.cs" company="Intel Corporation">
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
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// All scanners must implement the IScannerPanel interface.
    /// </summary>
    public interface IScannerPanel : IPanel
    {
        RunCommandDispatcher CommandDispatcher { get; }

        /// <summary>
        /// Gets the scanner Window form
        /// </summary>
        Form Form { get; }

        /// <summary>
        /// Return the panel type
        /// </summary>
        /// <returns></returns>
        String PanelClass { get; }

        /// <summary>
        /// Gets the ScannerCommon object.  Every scanner
        /// must create an instance of this object
        /// </summary>
        ScannerCommon ScannerCommon { get; }

        /// <summary>
        /// Gets the text controller object. Every scanner
        /// must create an instance of this object
        /// </summary>
        ITextController TextController { get; }

        /// <summary>
        /// Invoked to check if a widget on a scanner needs to
        /// be enabled or not.  This depends on the context.
        /// </summary>
        /// <param name="arg">Contextual information</param>
        /// <returns>true on success</returns>
        bool CheckWidgetEnabled(CheckEnabledArgs arg);

        /// <summary>
        /// Performs initialization.  This is invoked by
        /// the Screen manager after instatiating the panel, but
        /// before the panel is shown.
        /// </summary>
        /// <param name="initArg"></param>
        /// <returns></returns>
        bool Initialize(StartupArg initArg);

        /// <summary>
        /// Invoked when the focus changes in the current
        /// foreground application
        /// </summary>
        /// <param name="monitorInfo"></param>
        void OnFocusChanged(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Invoked when a request for a new scanner panel is made.  Return
        /// true to de-activate this panel and activate the new panel.
        /// Return false to dissallow changing of panels.
        /// </summary>
        /// <param name="eventArg"></param>
        /// <returns></returns>
        bool OnQueryPanelChange(PanelRequestEventArgs eventArg);

        /// <summary>
        /// A widget was actuated by the user either clicking on it
        /// or by using one of the input switches
        /// </summary>
        /// <param name="widget">the source widget</param>
        /// <param name="handled">set to true if handled</param>
        void OnWidgetActuated(Widget widget, ref bool handled);

        /// <summary>
        /// Inovked when the scanner is being used as an input
        /// panel to a control in a dialog box.  E.g, the
        /// alphabet scanner may be used to enter text in an edit
        /// control
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        void SetTargetControl(Form parent, Widget widget);
    }
}