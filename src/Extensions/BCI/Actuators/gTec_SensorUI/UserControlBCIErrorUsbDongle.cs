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

            _updateTimer = new Timer();
            _updateTimer.Interval = 5000; // 5 seconds
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            updatePairedDeviceList();
            updateUnPairedDeviceList();
        }

        private void ListViewPairedDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonNext.Enabled = listViewPairedDevices.SelectedItems.Count > 0;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            updatePairedDeviceList();
            updateUnPairedDeviceList();
        }

        public async void updatePairedDeviceList()
        {
            listViewPairedDevices.Items.Clear();
            try
            {
                IList<string> devices = await Task.Run(() => Unicorn.GetAvailableDevices(true));
                if (devices.Count > 0)
                {
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
                // Log or handle the exception
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public async void updateUnPairedDeviceList()
        {
            listViewUnPairedDevices.Items.Clear();
            try
            {
                IList<string> devices = await Task.Run(() => Unicorn.GetAvailableDevices(false));
                if (devices.Count > 0)
                {
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
                // Log or handle the exception
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}