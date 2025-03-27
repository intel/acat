////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorOpticalSensor.cs
//
// User control which displays information on errors related to the optical sensor
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which displays information on errors related to the optical sensor
    /// </summary>
    public partial class UserControlBCIErrorOpticalSensor : UserControl
    {
        /// <summary>
        /// Unique ID for this step
        /// </summary>
        private String _stepId;

        /// <summary>
        /// When form is closed this is to make the Task finish so is able to be disposed
        /// </summary>
        public bool _endTask { get; set; }

        public String resourceButtonVideo; // Resource to set up video resource
        public String resourceButtonSetupGuide; // Resource to set up guide resource

        public System.Threading.Timer timerOpicalSensorError;

        private const int LuxThresholdMinimum = 0;
        private const int LuxThresholdMaximum = 60;
        private const int LuxThresholdDefault = 20;
        private const int LuxIncrement = 1;

        private int _defaultLux;

        private String _htmlText = "<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n  <style>\r\n    a:link{color: rgb(255, 170, 0);}\r\n  </style>\r\n  " +
                                    "</head>\r\n  <body style=\"background-color:#232433;\">\r\n    " +
                                    "<p style=\"font-family:'Montserrat Medium'; font-size:24px; color:white; text-align: center;\">\r\n    " +
                                    "Adjust the slider and/or the brightness of your monitor and click Retry. The waveform must match the sample waveform. \r\n" +
                                     "To fix this error, please review <a href=$ACAT_USER_GUIDE#OpticalSensorError>instructions</a>." +
                                    "</p>\r\n  </body>\r\n</html>\r\n\r\n\r\n\r\n";

        /// <summary>
        /// User Control Form for the output signals from sensor
        /// </summary>
        public UserControlBCIErrorOpticalSensor(String stepId)
        {
            InitializeComponent();

            _stepId = stepId;

            // Connect video and set up guide resources
            resourceButtonVideo = "";
            resourceButtonSetupGuide = "";

            chartSignal.ChartAreas[0].AxisY.Maximum = 1;// 250 for analog 1.2;
            chartSignal.ChartAreas[0].AxisY.Minimum = 0;

            webBrowserTop.DocumentCompleted += WebBrowserDesc_DocumentCompleted;
            var html = _htmlText.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowserTop.DocumentText = html;

            webBrowserBottom.DocumentCompleted += WebBrowserDesc_DocumentCompleted;
            var htmlContent = R.GetString("BCIOnboardingBottomHtmlText");
            html = htmlContent.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowserBottom.DocumentText = html;

            luxSlider.ValueChanged += LuxSlider_ValueChanged;
            luxSlider.MouseUp += LuxSlider_MouseUp;
            luxSlider.Minimum = LuxThresholdMinimum;
            luxSlider.Maximum = LuxThresholdMaximum;
            _defaultLux = BCIActuatorSettings.Settings.DAQ_OpticalSensorLuxThreshold;
            if (_defaultLux < LuxThresholdMinimum || _defaultLux > LuxThresholdMaximum)
            {
                _defaultLux = LuxThresholdDefault;
            }

            luxSlider.Value = _defaultLux;
        }

        private void LuxSlider_MouseUp(object sender, MouseEventArgs e)
        {
            setLuxThresholdValue((int)luxSlider.Value);
        }

        private void LuxSlider_ValueChanged(object sender, EventArgs e)
        {
            setLuxThresholdValue((int)luxSlider.Value);
        }

        private void WebBrowserDesc_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowserBottom.Navigating -= WebBrowserDesc_Navigating;
            webBrowserBottom.Navigating += WebBrowserDesc_Navigating;
            webBrowserTop.Navigating -= WebBrowserDesc_Navigating;
            webBrowserTop.Navigating += WebBrowserDesc_Navigating;
        }

        private void WebBrowserDesc_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Utils.HandleHelpNavigaion(e);
        }

        //// <summary>
        /// Handler for new optical sensor data - called by timer for consistent calls
        /// Only optical sensor data (no EEG) is passed
        /// </summary>
        /// <param name="newData"></param>
        public void updateOpticalSensorDataPlot(double[] newData)
        {
            try
            {
                if (newData != null && newData.Length > 0)
                {
                    int numNewSamples = newData.Length;

                    if (numNewSamples > 0)
                    {
                        // Remove old and add new points all at once instead of individually
                        int numOriginalNewSamples = numNewSamples;

                        while (numNewSamples > 0)
                        {
                            double scaledData = 0.9 * newData[numOriginalNewSamples - numNewSamples];

                            addDataToChart(chartSignal, scaledData);

                            numNewSamples -= 1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        /// <summary>
        /// Adds point to chart
        /// </summary>
        /// <param name="inputSample"></param>
        /// <param name="isGestureDetected"></param>
        private delegate void addDataToChartDelegate(Chart chart, double sample);

        public void addDataToChart(Chart chart, double sample)
        {
            if (chart.InvokeRequired)
            {
                var d = new addDataToChartDelegate(addDataToChart);
                Invoke(d, new object[] { chart, sample });
            }
            else
            {
                // Add signal
                chart.Series["Signal"].Points.Add(sample);

                // Remove points at the end of the graph (for timeseries data)
                if (chart.Series["Signal"].Points.Count > 1250)
                {
                    chart.Series["Signal"].Points.Remove(chart.Series["Signal"].Points.First());
                }
            }
        }

        /// <summary>
        /// Release any resources held by this form
        /// </summary>
        public void close()
        {
            _endTask = true;
        }

        private String getHtmlText(String file)
        {
            var docsPath = SmartPath.ApplicationPath + "\\Docs";

            var htmlFile = docsPath + "\\" + file;

            if (File.Exists(htmlFile))
            {
                return File.ReadAllText(htmlFile);
            }

            return String.Empty;
        }

        private void buttonLuxSliderMinus_Click(object sender, EventArgs e)
        {
            var value = (int)luxSlider.Value - LuxIncrement;
            if (value < LuxThresholdMinimum)
            {
                value = LuxThresholdMinimum;
            }

            luxSlider.Value = value;

            setLuxThresholdValue(value);
        }

        private void buttonLuxSliderPlus_Click(object sender, EventArgs e)
        {
            var value = (int)luxSlider.Value + LuxIncrement;
            if (value > LuxThresholdMaximum)
            {
                value = LuxThresholdMaximum;
            }

            luxSlider.Value = value;

            setLuxThresholdValue(value);
        }

        private void setLuxThresholdValue(int value)
        {
            BCIActuatorSettings.Settings.DAQ_OpticalSensorLuxThreshold = value;
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            luxSlider.Value = _defaultLux;
        }
    }
}