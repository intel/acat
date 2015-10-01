////////////////////////////////////////////////////////////////////////////
// <copyright file="StatusBarController.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Utility;

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
    /// Use this class to display a status bar in the scanner
    /// form.  The status bar can be used to display the status
    /// of the various modifier keys such as Shift, Ctrl, Alt etc.
    /// For example, when the shift key is pressed, an up arrow
    /// can be displayed in the UI control that is assigned to
    /// the shift key.
    /// Each modifier key has a UI control (like a Forms.Label object)
    /// which can display the status of the modifier key.
    /// </summary>
    public class StatusBarController
    {
        /// <summary>
        /// The status bar object that holds the UI controls that
        /// represent the status of the modifier keys
        /// </summary>
        private ScannerStatusBar _statusBar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusBar"></param>
        public StatusBarController(ScannerStatusBar statusBar = null)
        {
            _statusBar = statusBar;
        }

        /// <summary>
        /// Get/sets the statusbar object
        /// </summary>
        public ScannerStatusBar StatusBar
        {
            get { return _statusBar; }
            set { _statusBar = value; }
        }

        /// <summary>
        /// Updates the control that displays the status of the
        /// Alt key.
        /// </summary>
        public void UpdateAltStatus()
        {
            if (_statusBar != null && _statusBar.AltStatus != null)
            {
                String label = String.Empty;
                if (KeyStateTracker.IsStickyAltOn())
                {
                    label = "ALT LOCK";
                }
                else if (KeyStateTracker.IsAltOn())
                {
                    label = "ALT";
                }

                setText(_statusBar.AltStatus, label);
            }
        }

        /// <summary>
        /// Updates the control that displays the status of the
        /// Ctrl key.
        /// </summary>
        public void UpdateCtrlStatus()
        {
            if (_statusBar != null && _statusBar.CtrlStatus != null)
            {
                String label = String.Empty;
                if (KeyStateTracker.IsStickyCtrlOn())
                {
                    label = "CTRL LOCK";
                }
                else if (KeyStateTracker.IsCtrlOn())
                {
                    label = "CTRL";
                }

                setText(_statusBar.CtrlStatus, label);
            }
        }

        /// <summary>
        /// Updates the control that displays the status of the
        /// shift key.
        /// </summary>
        public void UpdateShiftStatus()
        {
            if (_statusBar != null && _statusBar.ShiftStatus != null)
            {
                String label = String.Empty;
                if (KeyStateTracker.IsStickyShiftOn())
                {
                    label = "CAPS";
                }
                else if (KeyStateTracker.IsShiftOn())
                {
                    label = "SHIFT";
                }

                setText(_statusBar.ShiftStatus, label);
            }
        }

        /// <summary>
        /// Updates the status bar with the current status
        /// of the modifier keys
        /// </summary>
        public void UpdateStatusBar()
        {
            if (_statusBar != null)
            {
                UpdateShiftStatus();
                UpdateCtrlStatus();
                UpdateAltStatus();
            }
        }

        private void setText(object control, String text)
        {
            if (control is Control)
            {
                Windows.SetText(control as Control, text);
            }
            else if (control is StatusBarPanel)
            {
                (control as StatusBarPanel).Text = text;
            }
        }
    }
}