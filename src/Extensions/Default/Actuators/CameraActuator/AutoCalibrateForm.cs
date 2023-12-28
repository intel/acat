////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AutoCalibrateForm.cs
//
// A dialog that is displayed when a calibrate request is made by the
// vision subsystem
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    internal partial class AutoCalibrateForm : Form
    {
        private VideoWindowFinder _videoWindowFinder;
        CameraActuator _visionActuator;
        private WindowActiveWatchdog _windowActiveWatchdog;
        private WindowOverlapWatchdog _windowOverlapWatchdog;
        public AutoCalibrateForm(CameraActuator visionActuator)
        {
            InitializeComponent();
            Load += AutoCalibrateForm_Load;
            Shown += AutoCalibrateForm_Shown;
            _visionActuator = visionActuator;
            visionActuator.EvtCalibrationEnd += VisionActuator_EvtCalibrationEnd;
        }

        private void _videoWindowFinder_EvtVideoWindowDisplayed(IntPtr handle)
        {
            try
            {
                _windowOverlapWatchdog = new WindowOverlapWatchdog(handle.ToInt32());

                _videoWindowFinder.DockVideoWindow(this);

            }
            catch
            {

            }
        }

        private void AutoCalibrateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_windowOverlapWatchdog != null)
            {
                _windowOverlapWatchdog.Dispose();
                _windowOverlapWatchdog = null;
            }
        }

        private void AutoCalibrateForm_Load(object sender, EventArgs e)
        {
            Resize += AutoCalibrateForm_Resize;

            FormClosing += AutoCalibrateForm_FormClosing;

            TopMost = false;
            TopMost = true;

            Left = Top = 0;
            CameraSensor.showVideoWindow();
        }

        private void AutoCalibrateForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void AutoCalibrateForm_Shown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            _videoWindowFinder = new VideoWindowFinder();
            _videoWindowFinder.EvtVideoWindowDisplayed += _videoWindowFinder_EvtVideoWindowDisplayed;
            _videoWindowFinder.Start();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CameraSensor.hideVideoWindow();

            _windowActiveWatchdog.Dispose();

            Windows.CloseForm(this);
        }

        private void buttonRecalibrate_Click(object sender, EventArgs e)
        {
            CameraSensor.visionCommand("action=RECALIBRATE", 0);
        }
        private void EndCalibration()
        {
            _visionActuator.EvtCalibrationEnd -= VisionActuator_EvtCalibrationEnd;

            _windowActiveWatchdog.Dispose();

            if (_videoWindowFinder != null)
            {
                _videoWindowFinder.EvtVideoWindowDisplayed -= _videoWindowFinder_EvtVideoWindowDisplayed;
                _videoWindowFinder.Dispose();
            }

            Log.Debug("Hiding video window");

            CameraSensor.hideVideoWindow();

            Log.Debug("Closing calibform");

            Windows.CloseForm(this);
        }

        private void VisionActuator_EvtCalibrationEnd(object sender, EventArgs e)
        {
            EndCalibration();
        }
    }
}
