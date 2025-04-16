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

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// User control which handles Unicorn bluetooth device connection
    /// </summary>
    public partial class UserControlErrorBluetoothDisconnected : UserControl
    {
        private DAQ_gTecBCI gtecBCI;
        private Timer _updateTimer;

        public UserControlErrorBluetoothDisconnected(DAQ_gTecBCI device)
        {
            InitializeComponent();

            gtecBCI = device;

            // buttonNext_userControlErrorBluetoothDisconnected.Enabled = false;

            listViewPairedDevices.SelectedIndexChanged += ListViewPairedDevices_SelectedIndexChanged;
            listViewUnPairedDevices.SelectedIndexChanged += ListViewPairedDevices_SelectedIndexChanged;

            // buttonNext_userControlErrorBluetoothDisconnected.Click += buttonNext_Click;

            _updateTimer = new Timer();
            _updateTimer.Interval = 5000; // 5 seconds
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

        }

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

        private void ListViewPairedDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enableButton = listViewPairedDevices.SelectedItems.Count > 0 || listViewUnPairedDevices.SelectedItems.Count > 0;
            buttonNext_userControlErrorBluetoothDisconnected.Enabled = enableButton;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            updatePairedDeviceList();
            updateUnPairedDeviceList();
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
    }
}