////////////////////////////////////////////////////////////////////////////
// <copyright file="CheckBoxWidget.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A widget that represents a checkbox.  Everytime the user clicks on
    /// the widget or selects it, it toggles the text in the Control.  On the
    /// form, use a Label as the UI control for this widget.
    /// By default, it assumes that the text for the on/off state comes
    /// from the "ACAT Icon" font ile.  The config file MUST specify "ACAT Icon" as
    /// the font file  for this widget in the WidgetAttribute xml note.
    /// If any other font file is used, the On/Off
    /// state text should be set explicitly in the config xml file
    /// (see Load() function below)
    /// </summary>
    public class CheckBoxWidget : LabelWidget
    {
        /// <summary>
        /// Assuming we are using the "ACAT Icon" font, the
        /// text for the OFF state
        /// </summary>
        private String _offStateText = "M";

        /// <summary>
        /// Assuming we are using the "ACAT Icon" font, the
        /// text for the ON state
        /// </summary>
        private String _onStateText = "L";

        /// <summary>
        /// Current toggle state
        /// </summary>
        private Boolean _toggleState;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public CheckBoxWidget(Control uiControl)
            : base(uiControl)
        {
            EvtActuated += CheckBoxWidget_EvtActuated;
            SetToggleState(_toggleState);
        }

        /// <summary>
        /// Returns the current toggle state
        /// </summary>
        /// <returns></returns>
        public bool GetToggleState()
        {
            return _toggleState;
        }

        /// <summary>
        /// Load button parameters from the xml file. Reads the text for
        ///  the oN/OFF states
        /// </summary>
        /// <param name="node">xml node to parse</param>
        public override void Load(System.Xml.XmlNode node)
        {
            Log.Debug("node=" + node);

            String onOffState = XmlUtils.GetXMLAttrString(node, "onOffState");
            bool toggle;

            toggle = String.Compare(onOffState, "on", true) == 0;

            String state = XmlUtils.GetXMLAttrString(node, "onStateText");
            if (!String.IsNullOrEmpty(state))
            {
                _onStateText = state;
            }

            state = XmlUtils.GetXMLAttrString(node, "offStateText");
            if (!String.IsNullOrEmpty(state))
            {
                _offStateText = state;
            }

            SetToggleState(toggle);
        }

        /// <summary>
        /// Sets the toggle state to on or off. Sets the
        /// text in the control accordingly
        /// </summary>
        /// <param name="isOn">true if on</param>
        public void SetToggleState(Boolean isOn)
        {
            _toggleState = isOn;
            Windows.SetText(UIControl, _toggleState ? _onStateText : _offStateText);
        }

        /// <summary>
        /// Triggered when the toggle button is actuated. Flip state
        /// and notify
        /// </summary>
        /// <param name="child"></param>
        private void CheckBoxWidget_EvtActuated(object sender, WidgetEventArgs e)
        {
            SetToggleState(!_toggleState);
            notifyValueChanged();
        }
    }
}