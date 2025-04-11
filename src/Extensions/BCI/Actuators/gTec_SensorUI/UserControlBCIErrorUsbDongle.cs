////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorUsbDongle.cs
//
// User control which displays information on errors related to connecting
// to the BCI board usb dongle which streams data from the BCI board
// through bluetooth
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gtec.Unicorn;
using System.Threading.Tasks;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// User control which displays information on errors related to connecting to the BCI board
    /// usb dongle which streams data from the BCI board through bluetooth
    /// </summary>
    public partial class UserControlBCIErrorUsbDongle : UserControl
    {
        private Timer _updateTimer;

        public UserControlBCIErrorUsbDongle()
        {
            InitializeComponent();

            buttonNext.Enabled = false;

            listViewPairedDevices.SelectedIndexChanged += ListViewPairedDevices_SelectedIndexChanged;
            listViewUnPairedDevices.SelectedIndexChanged += ListViewPairedDevices_SelectedIndexChanged;

            buttonNext.Click += buttonNext_Click;

            _updateTimer = new Timer();
            _updateTimer.Interval = 10000; // 10 seconds
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            updatePairedDeviceList();
            updateUnPairedDeviceList();
        }

        private void buttonNext_Click(object sender, EventArgs e)
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
                Log.Debug($"Selected device: {selectedDevice} , trying to connect...");
                
                Unicorn device = new Unicorn(selectedDevice);
                
                Log.Debug($"Device: {device} is connected...");

                device.Dispose();
                Log.Debug($"Device: {device} is disconnected...");
            }   
            
        }

        private void ListViewPairedDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enableButton = listViewPairedDevices.SelectedItems.Count > 0 || listViewUnPairedDevices.SelectedItems.Count > 0;
            buttonNext.Enabled = enableButton;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            updatePairedDeviceList();
            updateUnPairedDeviceList();
        }

        public async void updatePairedDeviceList()
        {
            try
            {
                IList<string> devices = await Task.Run(() => Unicorn.GetAvailableDevices(true));
                if (devices.Count > 0)
                {
                    listViewPairedDevices.Items.Clear();
                    listViewPairedDevices.Invoke((Action)(() =>
                    {
                        foreach (string device in devices)
                        {
                            var listItem = new ListViewItem(device);
                            listViewPairedDevices.Items.Add(listItem);
                        }
                    }));
                }
            }
            catch (Gtec.Unicorn.DeviceException ex)
            {
                Log.Debug($"Error: {ex.Message}");
            }
        }

        public async void updateUnPairedDeviceList()
        {
            try
            {
                IList<string> devices = await Task.Run(() => Unicorn.GetAvailableDevices(false));
                if (devices.Count > 0)
                {
                    listViewUnPairedDevices.Items.Clear();
                    listViewUnPairedDevices.Invoke((Action)(() =>
                    {
                        foreach (string device in devices)
                        {
                            var listItem = new ListViewItem(device);
                            listViewUnPairedDevices.Items.Add(listItem);
                        }
                    }));
                }
            }
            catch (Gtec.Unicorn.DeviceException ex)
            {
                Log.Debug($"Error: {ex.Message}");
            }
        }
    }
}