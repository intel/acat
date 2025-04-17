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

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gtec.Unicorn;
using System.Threading.Tasks;
using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// User control which handles Unicorn bluetooth device connection
    /// </summary>
    public partial class UserControlErrorBluetoothDisconnected : UserControl
    {
        private Timer _updateTimer;

        // public String selectedDevice = null;

        public event DAQ_gTecBCI.DelegateBluetoothUpdate EvtBluetoothRequest;

        public UserControlErrorBluetoothDisconnected()
        {
            InitializeComponent();

            buttonNext_userControlErrorBluetoothDisconnected.Enabled = false;

            listViewPairedDevices.SelectedIndexChanged += ListViewPairedDevices_SelectedIndexChanged;
            listViewUnPairedDevices.SelectedIndexChanged += ListViewPairedDevices_SelectedIndexChanged;

            // buttonNext_userControlErrorBluetoothDisconnected.Click += buttonNext_Click;

            

        }


        // Save gTec device name in settings if something is selected in the list
        private void ListViewPairedDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enableButton = listViewPairedDevices.SelectedItems.Count > 0 || listViewUnPairedDevices.SelectedItems.Count > 0;
            buttonNext_userControlErrorBluetoothDisconnected.Enabled = enableButton;

            string selectedDevice = "";

            if (listViewPairedDevices.SelectedItems.Count > 0)
            {
                selectedDevice = listViewPairedDevices.SelectedItems[0].Text;
            }

            if (listViewUnPairedDevices.SelectedItems.Count > 0)
            {
                selectedDevice = listViewUnPairedDevices.SelectedItems[0].Text;
            }

            if (!string.IsNullOrEmpty(selectedDevice))
            {
                BCIActuatorSettings.Settings.GTecDeviceName = selectedDevice;
                BCIActuatorSettings.Save();
                Log.Debug("Saved BCIActuatorSettings.Settings.GTecDeviceName to ACAT settings: " + BCIActuatorSettings.Settings.GTecDeviceName);
            }
        }

        public void startStopUpdateBluetoothListTimer(bool start)
        {
            if (start)
            {
                try
                {
                    _updateTimer = new Timer();
                    _updateTimer.Interval = 5000; // 5 seconds
                    _updateTimer.Tick += UpdateTimer_Tick;
                    _updateTimer.Start();
                }
                catch (Exception e)
                {
                    Log.Debug("startStopUpdateBluetoothListTimer | Exception: " + e.ToString());
                }
            }
            else
            {
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
                    Log.Debug("startStopUpdateBluetoothListTimer | Exception: " + e.ToString());
                }
            }

        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            //updatePairedDeviceList();
            //updateUnPairedDeviceList();

            Dictionary<String, object> requestParams = new Dictionary<String, object>();
            requestParams["paired"] = true;
            EvtBluetoothRequest(DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_REQUEST, requestParams);

            requestParams["paired"] = false;
            EvtBluetoothRequest(DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_REQUEST, requestParams);
        }


        public void bluetoothResultHandler(DAQ_gTecBCI.BluetoothEvent bluetoothEvent, Dictionary<String, object> eventParams)
        {
            Log.Debug("UserControlErrorBluetoothDisconnected | bluetoothResultHandler | bluetoothEvent: " + bluetoothEvent.ToString());

            switch (bluetoothEvent)
            {
                /*
                case DAQ_gTecBCI.BluetoothEvent.DEVICE_DISCONNECTED:
                    break;
                case DAQ_gTecBCI.BluetoothEvent.SUCCESSFUL_CONNECTION:
                    break;
                */

                case DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_RESULT:

                    Invoke(new Action(() =>
                    {
                        // Updated paired / unpaired devices list
                        ListView listViewUpdate = listViewUnPairedDevices;
                        try
                        {
                            if ((bool)eventParams["paired"])
                            {
                                listViewUpdate = listViewPairedDevices;
                            }

                            IList<string> devices = (IList<string>)eventParams["devices"];
                            if (devices.Count > 0)
                            {
                                listViewUpdate.Items.Clear();
                                listViewUpdate.Invoke((Action)(() =>
                                {
                                    foreach (string device in devices)
                                    {
                                        var listItem = new ListViewItem(device);
                                        listViewUpdate.Items.Add(listItem);
                                    }
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("UserControlErrorBluetoothDisconnected | bluetoothResultHandler | Exception: " + ex.Message);
                        }
                    }));

                    break;

                default:
                    break;

            }

        }


/*        
        private async void buttonNext_Click(object sender, EventArgs e)
        {
            string selectedDevice = "";

            if (listViewPairedDevices.SelectedItems.Count > 0)
            {
                selectedDevice = listViewPairedDevices.SelectedItems[0].Text;
            }

            if (listViewUnPairedDevices.SelectedItems.Count > 0)
            {
                selectedDevice = listViewUnPairedDevices.SelectedItems[0].Text;
            }

            if (!string.IsNullOrEmpty(selectedDevice))
            {
                await gtecBCI.connectionTestAsync(selectedDevice);
            }   
        }

        public async void updatePairedDeviceList()
        {
            IList<string> pairedDevices = await gtecBCI.scanDevicesAsync(true);

            if (pairedDevices.Count > 0)
            {
                listViewPairedDevices.Items.Clear();
                listViewPairedDevices.Invoke((Action)(() =>
                {
                    foreach (string device in pairedDevices)
                    {
                        var listItem = new ListViewItem(device);
                        listViewPairedDevices.Items.Add(listItem);
                    }
                }));
            }
        }

        public async void updateUnPairedDeviceList()
        {
            IList<string> unPairedDevices = await gtecBCI.scanDevicesAsync(false);

            if (unPairedDevices.Count > 0)
            {
                listViewUnPairedDevices.Items.Clear();
                listViewUnPairedDevices.Invoke((Action)(() =>
                {
                    foreach (string device in unPairedDevices)
                    {
                        var listItem = new ListViewItem(device);
                        listViewUnPairedDevices.Items.Add(listItem);
                    }
                }));
            }
        }

 */


    }
}