using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using Accord.Math;
using EEGUtils;

namespace ACAT.Extensions.Default.Actuators.EEG.EEGDataAcquisition
{
    public partial class SignalMonitorForm : Form
    {
        #region Properties
        public static String SettingsFileName = "EEGSettings.xml";

        static Color[] ColorValues = new Color[] { Color.Gray, Color.Purple, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Red, Color.Brown, Color.Black };

        public ConsoleForm consoleForm;

        public List<Chart> chSignalList;
        public List<Panel> pnlSignalQualityList;
        public List<Label> lblUVrms;
        public Filter NotchFilter;
        public Filter Frontend;

        // Acquisition parameters
        private int _notchIdx;
        private int _frontendIdx;
        private int _scaleIdx;
        private string _comPort;
        private int _numChannels;
        private bool _saveDataToFileFlag;
        private int _opticalSensorThreshold;

        private bool _isAcquiringData;
        private bool _initInProcess = false;
        private bool _settingsSavedFlag = true; //Sets to false when a setting updated and not saved

        #endregion

        #region Init

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="startDevice"></param>
        public SignalMonitorForm(bool startDevice = false)
        {
            InitializeComponent();
            _initInProcess = true;

            Load += SignalMonitorSimpleForm_Load;
            Log.Debug("Initializing Signal Monitor Form...");
            Log.Debug("Starting Device: " + startDevice);

            TopMost = true;

            // Load settings
            LoadSettings();

            // Add series to chart and add signals
            chSignalList = new List<Chart> { chartSignal1, chartSignal2, chartSignal3, chartSignal4, chartSignal5, chartSignal6, chartSignal7, chartSignal8 };
           
            //    pnlSignalQualityList = new List<Panel> { bSignalQuality1, bSignalQuality2, bSignalQuality3, bSignalQuality4, bSignalQuality5, bSignalQuality6, bSignalQuality7, bSignalQuality8 };
            // lblUVrms = new List<Label> { lblUVrmsCh1, lblUVrmsCh2, lblUVrmsCh3, lblUVrmsCh4, lblUVrmsCh5, lblUVrmsCh6, lblUVrmsCh7, lblUVrmsCh8 };

            for (int chIdx = 0; chIdx < chSignalList.Count; chIdx++)
            {
                chSignalList[chIdx].Series.Clear();
                chSignalList[chIdx].Series.Add("Signal");
                chSignalList[chIdx].Series["Signal"].ChartType = SeriesChartType.FastLine;
                chSignalList[chIdx].Series["Signal"].Color = ColorValues[chIdx];
                chSignalList[chIdx].Series["Signal"].BorderWidth = 1;
            }

            // Add series to marker chart
            chMarker.Series.Add("Signal");
            chMarker.Series["Signal"].ChartType = SeriesChartType.FastLine;
            chMarker.Series["Signal"].Color = ColorValues[ColorValues.Length-1];
            chMarker.ChartAreas[0].AxisY.Maximum = 1;// 250 for analog 1.2;
            chMarker.ChartAreas[0].AxisY.Minimum = 0;

            // Add port names to dropdown list
            List<String> portsFound = DAQ_OpenBCI.GetSerialPorts();
            bool defaultPortFound = false;
            int idx = 0;
            foreach (string s in portsFound)
            {
                Log.Debug("Port " + s + " detected");
                cbPorts.Items.Add(s);
                if (s == _comPort)
                {
                    defaultPortFound = true;
                    cbPorts.SelectedIndex = idx;
                }
                idx++;
            }
            if (!defaultPortFound)
                cbPorts.SelectedIndex = 0;


            // Update form
            UpdaFormWithSettings();

            // Create timer to plot samples (will be started if device is acquiring data)
            _timerPlotData.Enabled = true;
            _timerPlotData.Interval = 4;
            _timerPlotData.Tick += new System.EventHandler(this.TimerPlotData_Tick);
            _timerPlotData.Stop();

            // Check if EmgGlobals.OpenBCI is already acquiring data
            _isAcquiringData = DAQ_OpenBCI.IsAcquiring();

            // If the device is already acquiring data, plot it. 
            // Otherwise, user will start it manually
            if (_isAcquiringData)
            {
                Log.Debug("Device is already acquiring data, start plotting");
                btnStartStopDevice.Text = "Stop Device";
                _timerPlotData.Start();
            }
            else if (startDevice)
            {
                BtnStartStopDevice_Click(btnStartStopDevice, EventArgs.Empty);
            }

            NotchFilter = new Filter(_notchIdx, Filter.FilterTypes.Notch);
            Frontend = new Filter(_frontendIdx, Filter.FilterTypes.Frontend);

            //// Init in process finalizes when 
            //// a)  if startDevice=true -> device gets firts sample
            //// b) if startDevice=false -> everything is initialized
            //if (!startDevice)
            //    _initInProcess = false;

            _isAcquiringData = DAQ_OpenBCI.IsAcquiring();
            _settingsSavedFlag = true;

        }


        private void SignalMonitorSimpleForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            Top = 0;
            Left = 0;
        }

        #endregion

        #region Start/Stop
        private bool StartAcquisition()
        {
            bool success = DAQ_OpenBCI.Start(_comPort);
            Frontend.Restart();
            NotchFilter.Restart();

            if (success)
            {
                EEGUtils.FormInvokers.setButtonText(btnStartStopDevice, "Stop Device");
                _isAcquiringData = true;

                //for (int channelIdx=0; channelIdx<_numChannels; channelIdx++)
                //    pnlSignalQualityList[channelIdx].BackColor = Color.Yellow;

                _timerPlotData.Start();

                string txt = "Device started \n";
                AddMessageToConsole(txt);
            }
            return success;
        }

        private void StopAcquisition()
        {
            // Stop acquisition
            DAQ_OpenBCI.Stop();
            _timerPlotData.Stop();

            FormInvokers.setButtonText(btnStartStopDevice, "Start Device");

            _isAcquiringData = false;
            //for (int channelIdx = 0; channelIdx < _numChannels; channelIdx++)
            //    pnlSignalQualityList[channelIdx].BackColor = Color.Gray;

            string txt = "Device stopped \n";
            AddMessageToConsole(txt);
        }

        #endregion

        #region PlotSamples
        /// <summary>
        /// Plot samples at every tick (this simulates data is received continuosly)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerPlotData_Tick(object sender, EventArgs e)
        {
            // REMOVE ONLY FOR TESTING
            DAQ_OpenBCI.InsertMarker(5.89f);

            //List<Data> processedData = EegDataProcess.getProcessedData();
            double[,] data = null;// DAQ_OpenBCI.GetData();

            if (data != null && data.Length > 0)
            {
                _initInProcess = false;
                int numSamples = data.GetLength(1);
                int numChannels = data.GetLength(0);
                double unfilteredSample, filteredSample;

                for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                {
                    for (int channelIdx = 0; channelIdx < DAQ_OpenBCI.indEegChannels.Length; channelIdx++)
                    {
                        unfilteredSample = data[DAQ_OpenBCI.indEegChannels[channelIdx], sampleIdx];
                        filteredSample = NotchFilter.FilterData(unfilteredSample, channelIdx);
                        filteredSample = Frontend.FilterData(filteredSample, channelIdx);

                        // NOTE: Working on adding this in the new DAQ (this was done in the old one)
                       //// Update uVrms label
                       //lblUVrms[channelIdx].Text = dataAtTimeT.uVrms[channelIdx].ToString("0.00") + " uVrms";

                        //// Update panel signal quality

                        // if (dataAtTimeT.isSignalQualityOk[channelIdx])
                        //     pnlSignalQualityList[channelIdx].BackColor = Color.Green;
                        // else
                        // {
                        //     pnlSignalQualityList[channelIdx].BackColor = Color.Red;
                        //     _isSignalQualityAllChannelsOK = false;
                        // }

                        // Plot samples
                        addDataToChart(chSignalList[channelIdx], filteredSample);
                    }

                    BCIData.OpticalSensorStatus = DAQ_OpenBCI.indOpticalSensorChannel;
                    // Plot marker
                    addDataToChart(chMarker, data[BCIData.OpticalSensorStatus, sampleIdx]); // optical sensor
                }

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
            Invoke(d, new object[] { chart, sample});
        }
        else
        {
                // Add signal
                chart.Series["Signal"].Points.Add(sample);

                // Remove poits at the end of the graph (for timeseries data)
                if (chart.Series["Signal"].Points.Count > 1250)
                {
                    chart.Series["Signal"].Points.SuspendUpdates();
                    chart.Series["Signal"].Points.Remove(chart.Series["Signal"].Points.First());
                    chart.Series["Signal"].Points.ResumeUpdates();
                }
            }
    }

        #endregion        

        #region Callbacks from Form elements

        /// <summary>
        /// Selects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update port in settings
            ComboBox cb = (ComboBox)sender;
            _comPort = (string)cb.SelectedItem;

            DAQ_OpenBCI.SetPort(_comPort);

            _settingsSavedFlag = false;

        }

         /// <summary>
        /// Change the notch filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbNotch_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            _notchIdx = cb.SelectedIndex;
            _settingsSavedFlag = false;

            //EegDataProcess.setNotchFilter(_notchIdx);

        }

        /// <summary>
        /// Change the scale of the signals being displayed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            _scaleIdx = cb.SelectedIndex;
            _settingsSavedFlag = false;

            int yLimMin, yLimMax;
            GetGraphYLims(_scaleIdx, out yLimMin, out yLimMax);

            // Update scale in chaon ho rt
            for (int chIdx = 0; chIdx < chSignalList.Count; chIdx++)
            {
                chSignalList[chIdx].ChartAreas[0].AxisY.Maximum = yLimMax;
                chSignalList[chIdx].ChartAreas[0].AxisY.Minimum = yLimMin;
            }
        }        
       

        /// <summary>
        /// Start/Stop the device when the button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartStopDevice_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (_isAcquiringData) // Stop the device
                StopAcquisition();
            else // Start the device
                StartAcquisition();
        }


        /// <summary>
        /// Display console and locate it next to the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShowConsole_Click(object sender, EventArgs e)
        {
            if (consoleForm == null || consoleForm.IsDisposed)
                consoleForm = new ConsoleForm();

            consoleForm.Show();
            consoleForm.Location = new Point(this.Location.X + this.Width + 5, this.Location.Y);
        }
        
        #endregion

        #region Utils

        /// <summary>
        /// Clear all samples in chart
        /// </summary>
        private void ClearCharts()
        {
            for (int chIdx = 0; chIdx < chSignalList.Count; chIdx++)
                chSignalList[chIdx].Series["Signal"].Points.Clear();
        }

        /// <summary>
        /// Get the minimum and maximum values of the chart using _scaleIdx
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void GetGraphYLims(int scaleIdx, out int yLimMin, out int yLimMax)
        {
            int scale = 0;
            yLimMin = 0;
            yLimMax = 0;

            switch (scaleIdx)
            {
                case 0:
                    //50uV
                    scale = 50;
                    break;
                case 1:
                    // 100uV
                    scale = 100;
                    break;
                case 2:
                    // 200uV
                    scale = 200;
                    break;
                case 3:
                    // 500uV
                    scale = 500;
                    break;
                case 4:
                    // 1mV
                    scale = 1000;
                    break;
               
            }
                    yLimMax = scale;
                    yLimMin = -1 * scale;
        }

        private void LoadSettings()
        {
            // Load settings
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            var settings = Settings.Load();

            // Copy settings 2 parameters
            _scaleIdx = settings.SignalMonitor_ScaleIdx;
            _comPort = settings.DAQ_ComPort;
            _notchIdx = settings.DAQ_NotchFilterIdx;
            _frontendIdx = settings.DAQ_FrontendFilterIdx; // cannot be changed in the form
            _numChannels = settings.DAQ_NumChannels;
            //_opticalSensorThreshold = settings.DAQ_OpticalSensorThreshold;
            _saveDataToFileFlag = settings.DAQ_SaveToFileFlag;

            _settingsSavedFlag = true; //when a settings is changed in the form, it will change to false
            AddMessageToConsole("Settings loaded \n");
        }

        private void UpdaFormWithSettings()
        {
            //LoadSettings(); 

            // Update form
            cbScale.SelectedIndex = _scaleIdx;
            cbNotch.SelectedIndex = _notchIdx;
            //chSaveToFile.Checked = _saveDataToFileFlag; //removed to simplify file
          
            //EegDataProcess.setNotchFilter(_notchIdx);
        }


        private void SaveSettings()
        {
            
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            var settings = Settings.Load();
        
            settings.DAQ_ComPort = _comPort;
            settings.DAQ_NotchFilterIdx = _notchIdx;
            settings.SignalMonitor_ScaleIdx = _scaleIdx;
           //settings.DAQ_OpticalSensorThreshold = _opticalSensorThreshold;
            settings.DAQ_SaveToFileFlag = _saveDataToFileFlag;

            settings.Save();

            _settingsSavedFlag = true; 
            string txt = "Settings saved \n";
            AddMessageToConsole(txt);

        }

        /// <summary>
        /// Add message 2 console
        /// </summary>
        /// <param name="message"></param>
        private void AddMessageToConsole(String message)
        {
            /*
            // If richtextbox is embbeded in the form
            rtbConsole.AppendText(message);
            rtbConsole.ScrollToCaret();
            */

            // New form with console
            if (consoleForm == null)
                consoleForm = new ConsoleForm();

            if (consoleForm.Visible)
                consoleForm.AddTextToConsole(message);
        }

        #endregion

     
        
        public class BCIData
        {
            public static int OpticalSensorStatus { get; set; }
        }


    }
}
