////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Extension class for slider widget.  This consists of a
    /// slider (TrackBar) control with increment and decrement
    /// controls on either side.  The user increments/decrements
    /// the slider value by actuating the increment/decrement
    /// controls.
    /// </summary>
    public static class SliderWidgetExtension
    {
        /// <summary>
        /// Returns the current value of the slider
        /// </summary>
        /// <param name="sliderWidget">The slider widget</param>
        /// <param name="units">conversion units</param>
        /// <returns>slider value</returns>
        public static int GetState(this SliderWidget sliderWidget, decimal units)
        {
            if (sliderWidget != null)
            {
                decimal unconvertedValue = sliderWidget.GetSliderValue();
                return Convert.ToInt32(unconvertedValue / units);
            }

            return 0;
        }

        /// <summary>
        /// Sets the slider position to the indicated value. The 'units' parameter
        /// is used to normalize the value for the number of ticks in the track bar
        /// </summary>
        /// <param name="sliderWidget">the slider widget</param>
        /// <param name="sliderPosition">slider value</param>
        /// <param name="units">conversion units</param>
        public static void SetState(this SliderWidget sliderWidget, int sliderPosition, decimal units)
        {
            if (sliderWidget != null)
            {
                sliderWidget.SetSliderValue(sliderPosition, 1 / units);
            }
        }
    }

    /// <summary>
    /// Widget that encapsulates a Track bar with increment and
    /// decrement controls on either side. The min and max values
    /// are displayed along with the current value.  All these
    /// are configurable through the scanner config file
    /// </summary>
    public class SliderWidget : Widget
    {
        public const decimal SliderUnitsHundredths = 0.01M;

        /// <summary>
        /// Slider track bar tick units.
        /// </summary>
        public const decimal SliderUnitsOnes = 1;

        public const decimal SliderUnitsTenths = 0.1M;
        public const decimal SliderUnitsThousandths = 0.001M;

        /// <summary>
        /// If decimal, what is the step
        /// </summary>
        private const decimal DefaultDecimalStep = 0.5m;

        /// <summary>
        /// Default init value
        /// </summary>
        private const int DefaultInitialSliderValue = 1;

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
            sliderCaption = R.GetString(sliderCaption);

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

                _decimalStep = 1;

                stringToDecimal(sliderStep, ref _decimalStep);

                if (_decimalStep >= 1)
                {
                    Log.Error("SliderWidget::Load() - Warning!  Decimal step is greater than/equal to 1!");
                }

                _minValue = 0;
                _maxValue = 0;

                stringToDecimal(min, ref _minValue);
                stringToDecimal(max, ref _maxValue);

                decimal tempDecimal = Convert.ToDecimal(_minValue) / _decimalStep;
                _minTicks = Convert.ToInt32(tempDecimal);

                _maxTicks = Convert.ToInt32(Convert.ToDecimal(_maxValue) / _decimalStep);

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
        /// Sets the trackbar position to the specified value
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
        private void _decrementWidget_EvtActuated(object sender, WidgetActuatedEventArgs e)
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
        private void _incrementWidget_EvtActuated(object sender, WidgetActuatedEventArgs e)
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
        /// Trackbar Value changed.  Update current value and notify
        /// subscribers
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
        /// Quick helper function just to avoid repetition and
        /// error when setting the tick position as it is now coded,
        /// you not only must update the _sliderTickPosition variable,
        /// you also need to tell the slider widget to update the
        /// form.  That is what this method does.
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

        /// <summary>
        /// Convert string to decimal
        /// </summary>
        /// <param name="inputString">string to convert</param>
        /// <param name="value">converted value</param>
        /// <returns>true on success</returns>
        private bool stringToDecimal(String inputString, ref decimal value)
        {
            bool retVal = true;

            try
            {
                value = decimal.Parse(inputString, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Log.Debug("Error parsing decimal " + inputString + ", ex: " + ex.ToString());
                retVal = false;
            }

            return retVal;
            ;
        }
    }
}