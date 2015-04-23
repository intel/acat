////////////////////////////////////////////////////////////////////////////
// <copyright file="SliderWidget.cs" company="Intel Corporation">
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
using System.Xml;
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
    /// Widget that encapsulates a Track bar with increment and
    /// decrement controls on either side. The min and max values
    /// are displayed along with the current value.  All these
    /// are configurable through the config file
    /// </summary>
    public class SliderWidget : Widget
    {
        /// <summary>
        /// If decimal, what is the step
        /// </summary>
        private const decimal DefaultDecimalStep = 0.5m;

        /// <summary>
        /// Default init value
        /// </summary>
        private const int DefaultInitialSliderValue = 1;

        /// <summary>
        /// How many ticks?
        /// </summary>
        private const int DefaultTickCount = 100;

        /// <summary>
        /// The control that displays the current value (could
        /// be a label or a text box)
        /// </summary>
        private Control _currentValueControl;

        /// <summary>
        /// Step amount if using decimal
        /// </summary>
        private decimal _decimalStep = DefaultDecimalStep;

        /// <summary>
        /// Step amount if integer
        /// </summary>
        private int _intStepAmount = 1;

        /// <summary>
        /// Maximum number of ticks
        /// </summary>
        private int _maxTicks = int.MaxValue;

        /// <summary>
        /// The maximum value
        /// </summary>
        private decimal _maxValue = int.MaxValue;

        /// <summary>
        /// Minimum number of ticks
        /// </summary>
        private int _minTicks = int.MinValue;

        /// <summary>
        /// The minimum value
        /// </summary>
        private decimal _minValue = int.MinValue;

        /// <summary>
        /// The number of ticks in the track bar
        /// </summary>
        private int _numTicks = DefaultTickCount;

        /// <summary>
        /// Caption
        /// </summary>
        private String _sliderCaption = "Caption";

        /// <summary>
        /// The current slider position
        /// </summary>
        private int _sliderTickPosition = DefaultInitialSliderValue;

        /// <summary>
        /// The underlying trackbar UI control
        /// </summary>
        private TrackBar _trackBar;

        /// <summary>
        /// Use decimal?
        /// </summary>
        private bool _useDecimal;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public SliderWidget(Control uiControl)
            : base(uiControl)
        {
            EvtChildAdded += SliderWidget_EvtChildAdded;
        }

        /// <summary>
        /// Gets the current trackbar value
        /// </summary>
        /// <returns>value</returns>
        public decimal GetSliderValue()
        {
            return Convert.ToDecimal(Windows.GetText(_currentValueControl));
        }

        /// <summary>
        /// Reads attributes such as min value, max value, step etc.
        /// Also determines if the track bar displays whole numbers or
        /// decimals
        /// </summary>
        /// <param name="node">the xml node to parse</param>
        public override void Load(XmlNode node)
        {
            String min = XmlUtils.GetXMLAttrString(node, "min");
            String max = XmlUtils.GetXMLAttrString(node, "max");
            String tickFrequency = XmlUtils.GetXMLAttrString(node, "tickfrequency");
            String sliderStep = XmlUtils.GetXMLAttrString(node, "step");
            String sliderCaption = XmlUtils.GetXMLAttrString(node, "caption");

            int initialValue = XmlUtils.GetXMLAttrInt(node, "initialvalue", DefaultInitialSliderValue);

            if (String.IsNullOrEmpty(min)
               || String.IsNullOrEmpty(max)
               || String.IsNullOrEmpty(tickFrequency)
               || String.IsNullOrEmpty(sliderStep))
            {
                return;
            }

            if (sliderStep.Contains("."))
            {
                _useDecimal = true;

                _decimalStep = Convert.ToDecimal(sliderStep);

                if (_decimalStep >= 1)
                {
                    Log.Error("SliderWidget::Load() - Warning!  Decimal step is greater than/equal to 1!");
                }

                _minValue = Convert.ToDecimal(min);
                _maxValue = Convert.ToDecimal(max);

                _numTicks = Convert.ToInt32(Convert.ToDecimal(_maxValue - _minValue) / _decimalStep);

                decimal tempDecimal = Convert.ToDecimal(_minValue) / _decimalStep;
                _minTicks = Convert.ToInt32(tempDecimal);

                _maxTicks = _numTicks;

                tempDecimal = Convert.ToDecimal(initialValue) / _decimalStep;
                _sliderTickPosition = Convert.ToInt32(tempDecimal);
            }
            else
            {
                _useDecimal = false;
                _decimalStep = 1;
                _minValue = Convert.ToDecimal(min);
                _maxValue = Convert.ToDecimal(max);

                _minTicks = Decimal.ToInt32(_minValue);
                _maxTicks = Decimal.ToInt32(_maxValue);

                _intStepAmount = Convert.ToInt32(sliderStep);

                _sliderTickPosition = initialValue;
            }

            _sliderCaption = sliderCaption;
        }

        /// <summary>
        /// Invoked after the config file has been loaded
        /// </summary>
        public override void PostLoad()
        {
            _trackBar.Minimum = _minTicks;
            _trackBar.Maximum = _maxTicks;
            _trackBar.TickStyle = TickStyle.None;
        }

        /// <summary>
        /// Set the trackbar position to the specified value
        /// </summary>
        /// <param name="value">value to set</param>
        /// <param name="units">factor for converson</param>
        public void SetSliderValue(int value, decimal units)
        {
            int convertedValue = Convert.ToInt32((Convert.ToDecimal(value) / units) / _decimalStep);

            _sliderTickPosition = convertedValue;
            if (_sliderTickPosition > _maxTicks)
            {
                _sliderTickPosition = _maxTicks;
            }
            else if (_sliderTickPosition < _minTicks)
            {
                _sliderTickPosition = _minTicks;
            }

            setTickPosition(_sliderTickPosition);
        }

        /// <summary>
        /// The widget that decreases the trackbar value was acutated
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _decrementWidget_EvtActuated(object sender, WidgetEventArgs e)
        {
            _sliderTickPosition = Windows.GetTrackBarValueInt(_trackBar);
            if (_sliderTickPosition > _minTicks)
            {
                _sliderTickPosition -= _intStepAmount;

                // decrease slider and update current value textbox
                setTickPosition(_sliderTickPosition);
                notifyValueChanged();
            }
        }

        /// <summary>
        /// The widget that increases the trackbar value was acutated
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _incrementWidget_EvtActuated(object sender, WidgetEventArgs e)
        {
            _sliderTickPosition = Windows.GetTrackBarValueInt(_trackBar);
            if (_sliderTickPosition < _maxTicks)
            {
                _sliderTickPosition += _intStepAmount;

                // increase slider and update current value textbox
                setTickPosition(_sliderTickPosition);

                notifyValueChanged();
            }
        }

        /// <summary>
        /// Value changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _trackBar_ValueChanged(object sender, EventArgs e)
        {
            Log.Debug("trackbar value: " + _trackBar.Value);
            Log.Debug("Widget: " + _trackBar.Name + ".  Setting value to " + ConvertTicksToRealValue(_trackBar.Value));

            Windows.SetText(_currentValueControl, ConvertTicksToRealValue(_trackBar.Value).ToString());

            notifyValueChanged();
        }

        /// <summary>
        /// Converts the tickCount to its decimal equivalent
        /// </summary>
        /// <param name="tickCount">the tickcount as integer</param>
        /// <returns>decimal value</returns>
        private decimal ConvertTicksToRealValue(int tickCount)
        {
            if (_useDecimal)
            {
                // convert back to "real user" value from ticks
                decimal realValue = Convert.ToDecimal(tickCount * _decimalStep);
                return realValue;
            }

            return tickCount;
        }

        /// <summary>
        /// Quick helper function just to avoid repetition and error when setting the tick position
        /// as it is now coded, you not only must update the _sliderTickPosition variable, you also
        /// need to tell the slider widget to update the screen.  That is what this method does///
        /// </summary>
        /// <param name="tickPosition">The trackbar tick position</param>
        private void setTickPosition(int tickPosition)
        {
            if ((tickPosition > _maxTicks) || (tickPosition < _minTicks))
            {
                Log.Error("SetTickPosition() - tickPosition out of acceptable range!");
            }

            Windows.SetTrackBarValue(_trackBar, tickPosition);
            Windows.SetText(_currentValueControl, ConvertTicksToRealValue(tickPosition).ToString());
        }

        /// <summary>
        /// Invoked when a widget is found on the form.  Use the subclass
        /// to determine what type of widget it is and subscribe to
        /// activation events for the widget
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SliderWidget_EvtChildAdded(object sender, WidgetEventArgs e)
        {
            Widget child = e.SourceWidget;

            Type type = child.UIControl.GetType();
            if (type.ToString().ToLower().Contains("trackbar"))
            {
                _trackBar = (TrackBar)child.UIControl;
                _trackBar.ValueChanged += _trackBar_ValueChanged;
                return;
            }

            var subclass = child.SubClass.ToLower();
            switch (subclass)
            {
                case "sliderdecrement":
                    child.EvtActuated += _decrementWidget_EvtActuated;
                    break;

                case "sliderincrement":
                    child.EvtActuated += _incrementWidget_EvtActuated;
                    break;

                case "slidercaption":
                    var sliderLabel = (Label)child.UIControl;
                    if (sliderLabel != null)
                    {
                        sliderLabel.Text = _sliderCaption;
                    }
                    break;

                case "sliderminvalue":
                    var lblMinValue = (Label)child.UIControl;
                    if (lblMinValue != null)
                    {
                        lblMinValue.Text = _minValue.ToString();
                    }
                    break;

                case "slidermaxvalue":
                    var lblMaxValue = (Label)child.UIControl;
                    if (lblMaxValue != null)
                    {
                        lblMaxValue.Text = _maxValue.ToString();
                    }
                    break;

                case "slidercurrentvalue":
                    _currentValueControl = child.UIControl;
                    break;

                default:
                    Log.Debug("Unrecognized subclass " + subclass);
                    break;
            }
        }
    }
}