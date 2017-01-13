////////////////////////////////////////////////////////////////////////////
// <copyright file="CheckBoxWidget.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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

using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Extension class for checkbox widget
    /// </summary>
    public static class CheckBoxWidgetExtensionMethods
    {
        /// <summary>
        /// Returns the toggle state of the checkbox widget
        /// </summary>
        /// <param name="widget">checkboxwidget object</param>
        /// <returns>the current state, true if checked</returns>
        public static bool GetState(this CheckBoxWidget widget)
        {
            if (widget != null)
            {
                return widget.GetToggleState();
            }

            return false;
        }

        /// <summary>
        /// Sets the toggle state of the checkbox widget
        /// </summary>
        /// <param name="widget">Checkbox widget</param>
        /// <param name="state">the state to set to</param>
        public static void SetState(this CheckBoxWidget widget, Boolean state)
        {
            if (widget != null)
            {
                widget.SetToggleState(state);
            }
        }
    }

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
        /// Loads widget specific parameters from the xml file.
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
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void CheckBoxWidget_EvtActuated(object sender, WidgetEventArgs e)
        {
            SetToggleState(!_toggleState);
            notifyValueChanged();
        }
    }
}