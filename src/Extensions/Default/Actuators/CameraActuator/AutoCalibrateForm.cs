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

using ACAT.Core.PanelManagement.Utils;
using ACAT.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Actuators.CameraActuator
{
    internal partial class AutoCalibrateForm : Form
    {
        private VideoWindowFinder _videoWindowFinder;
        private readonly CameraActuator _visionActuator;

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
                _videoWindowFinder.DockVideoWindow(this);
            }
            catch
            {
            }
        }


        private void AutoCalibrateForm_Load(object sender, EventArgs e)
        {
            Resize += AutoCalibrateForm_Resize;

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

            _videoWindowFinder = new VideoWindowFinder();
            _videoWindowFinder.EvtVideoWindowDisplayed += _videoWindowFinder_EvtVideoWindowDisplayed;
            _videoWindowFinder.Start();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CameraSensor.hideVideoWindow();


            Windows.CloseForm(this);
        }

        private void buttonRecalibrate_Click(object sender, EventArgs e)
        {
            CameraSensor.visionCommand("action=RECALIBRATE", 0);
        }

        private void EndCalibration()
        {
            _visionActuator.EvtCalibrationEnd -= VisionActuator_EvtCalibrationEnd;


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