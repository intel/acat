////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlErrorBluetoothDisconnected.cs
//
// User control which handles Unicorn bluetooth device connection
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// User control which handles Unicorn bluetooth device connection
    /// This screen shows when bluetooth connection cannot be established with a gTec device
    /// </summary>
    public partial class UserControlErrorBluetoothDisconnected : UserControl
    {
        // Timer to update the lists of paired / unpaired devices
        private Timer _updateTimer;

        // Bluetooth event to handle requests
        public event DAQ_gTecBCI.DelegateBluetoothUpdate EvtBluetoothRequest;

        /// <summary>
        /// Constructor for user control which handles Unicorn bluetooth device connection
        /// </summary>
        public UserControlErrorBluetoothDisconnected()
        {
            InitializeComponent();

            // Disable Next button until something in list is selected
            buttonNext_userControlErrorBluetoothDisconnected.Enabled = false;

            // Add handlers for selecting items in the lists
            listViewDevices.SelectedIndexChanged += ListViewDevices_SelectedIndexChanged;
        }

        /// <summary>
        /// Handler for saving gTec device name in settings when something is selected in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable Next button if something is selected in list
            bool enableButton = listViewDevices.SelectedItems.Count > 0;
            buttonNext_userControlErrorBluetoothDisconnected.Enabled = enableButton;

            string selectedDevice = "";

            if (listViewDevices.SelectedItems.Count > 0)
            {
                selectedDevice = listViewDevices.SelectedItems[0].ToString();
            }

            // Save selected device in settings
            if (!string.IsNullOrEmpty(selectedDevice))
            {
                BCIActuatorSettings.Settings.GTecDeviceName = selectedDevice;
                BCIActuatorSettings.Save();
                Log.Debug("Saved BCIGtecActuatorSettings.Settings.GTecDeviceName to ACAT settings: " + BCIActuatorSettings.Settings.GTecDeviceName);
            }
        }

        /// <summary>
        /// Function to start / top timer to update the list of paired / unpaired bluetooth devices
        /// </summary>
        /// <param name="start">Start/stop the timer which update the lists</param>
        public void startStopUpdateBluetoothListTimer(bool start)
        {
            if (start)
            {
                // Start timer
                try
                {
                    _updateTimer = new Timer
                    {
                        Interval = 5000 // 5 seconds
                    };
                    _updateTimer.Tick += UpdateTimer_Tick;
                    _updateTimer.Start();
                }
                catch (Exception e)
                {
                    Log.Exception("startStopUpdateBluetoothListTimer | Exception: " + e.ToString());
                }
            }
            else
            {
                // Stop timer
                try
                {
                    if (_updateTimer != null && _updateTimer.Enabled)
                    {
                        _updateTimer.Stop();
                        _updateTimer.Enabled = false;
                        _updateTimer.Dispose();
                        _updateTimer = null;
                    }
                }
                catch (Exception e)
                {
                    Log.Exception("startStopUpdateBluetoothListTimer | Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// Function executed during each timer tick. Sends bluetooth requests to scan for paired/unpaired devices
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            Dictionary<String, object> requestParams = new()
            {
                ["paired"] = true
            };
            EvtBluetoothRequest(DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_REQUEST, requestParams);

            requestParams["paired"] = false;
            EvtBluetoothRequest(DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_REQUEST, requestParams);
        }

        /// <summary>
        /// Handler for processing the results of bluetooth requests
        /// </summary>
        /// <param name="bluetoothEvent">Type of bluetooth request to handle</param>
        /// <param name="eventParams">Any extra params sent with bluetooth event request</param>
        public void bluetoothResultHandler(DAQ_gTecBCI.BluetoothEvent bluetoothEvent, Dictionary<String, object> eventParams)
        {
            Log.Debug("UserControlErrorBluetoothDisconnected | bluetoothResultHandler | bluetoothEvent: " + bluetoothEvent.ToString());

             if (!this.IsHandleCreated || this.IsDisposed) { return; }

            switch (bluetoothEvent)
            {
                case DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_RESULT:

                    Invoke(new Action(() =>
                    {
                        try
                        {
                            // Get devices from eventParams dict
                            IList<string> devices = (IList<string>)eventParams["devices"];
                            if (devices.Count > 0)
                            {
                                foreach (string device in devices)
                                {
                                    // check if the item is not in the list
                                    if (!listViewDevices.Items.Contains(device))
                                    {
                                        listViewDevices.Items.Add(device);
                                    }
                                }

                                // Also want to remove the devices that are not discoverable
                                foreach (ListViewItem item in listViewDevices.Items)
                                {
                                    if (!devices.Contains(item.Text))
                                    {
                                        listViewDevices.Items.Remove(item);
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Exception("UserControlErrorBluetoothDisconnected | bluetoothResultHandler | Exception: " + ex.Message);
                        }
                    }));

                    break;

                default:
                    break;
            }
        }
    }
}