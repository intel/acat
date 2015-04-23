////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerStatusBar.cs" company="Intel Corporation">
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
using System.Windows.Forms;

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
    /// Represents the UI controls in the scanner form that
    /// will display the states of keys such as Ctrl, Alt, Shift.
    /// Typically, these could Label object that displays whether
    /// Ctrl is currently pressed or not.
    /// </summary>
    public class ScannerStatusBar
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ScannerStatusBar()
        {
            AltStatus = null;
            CtrlStatus = null;
            FuncStatus = null;
            ShiftStatus = null;
            LockStatus = null;
        }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Alt key
        /// </summary>
        public Control AltStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Ctrl key
        /// </summary>
        public Control CtrlStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the the Function keys
        /// </summary>
        public Control FuncStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Shift lock (Caps lock)
        /// </summary>
        public Control LockStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Shift key
        /// </summary>
        public Control ShiftStatus { get; set; }
    }
}