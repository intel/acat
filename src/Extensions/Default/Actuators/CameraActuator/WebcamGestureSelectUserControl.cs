////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WebcamGestureSelectUserControl.cs
//
// UserControl that enables the user to select the preferred camera and
// preferred gesture to trigger ACAT.  Also displays whether the gesture
// was detected or not.  Allows user to recalibrate. Finds the video window
// and docks alongside.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    public partial class WebcamGestureSelectUserControl : UserControl
    {
        private Color _buttonBackColor;
        private Dictionary<int, IActuatorSwitch> _dict = new Dictionary<int, IActuatorSwitch>();
        private IEnumerable<string> _installedCameras;
        private CameraActuator _cameraActuator;
        private int bothIndex = -1;

        private int cameraIndex = -1;

        private int ctIndex = -1;

        private int erIndex = -1;

        internal WebcamGestureSelectUserControl(CameraActuator cameraActuator)
        {
            InitializeComponent();
            _buttonBackColor = buttonRecalibrate.BackColor;
            _cameraActuator = cameraActuator;
        }

        public delegate void GestureSelected(bool cheekTwitch, bool eyebrowRaise);

        public event GestureSelected EvtGestureSelected;

        public bool CTEnabled
        {
            get
            {
                return dropdownGestureSelect.SelectedIndex == ctIndex ||
                        dropdownGestureSelect.SelectedIndex == bothIndex;
            }
        }

        public bool EREnabled
        {
            get
            {
                return dropdownGestureSelect.SelectedIndex == erIndex ||
                        dropdownGestureSelect.SelectedIndex == bothIndex;
            }
        }

        public void EnableControls(bool enable, bool isCalibrating)
        {
            setControlsEnable(enable, isCalibrating);
        }

        public bool Initialize()
        {
            UnInit();

            return true;
        }

        public void Shown()
        {
            refreshCameraList();
            initializeGestureDropdown();

            dropdownGestureSelect.SelectedIndexChanged += DropdownGestureSelect_SelectedIndexChanged;
            dropdownCameraSelect.SelectedIndexChanged += DropdownCameraSelect_SelectedIndexChanged;
        }

        public void UnInit()
        {
            dropdownGestureSelect.SelectedIndexChanged -= DropdownGestureSelect_SelectedIndexChanged;
            dropdownCameraSelect.SelectedIndexChanged -= DropdownCameraSelect_SelectedIndexChanged;
        }

        private void buttonRecalibrate_Click(object sender, EventArgs e)
        {
            _cameraActuator.Recalibrate();
        }

        private void changeCamera(String camera)
        {
            if (!_cameraActuator.IsCameraInstalled(camera))
            {
                refreshCameraList();
            }
            else
            {
                cameraIndex = dropdownCameraSelect.SelectedIndex;
                _cameraActuator.ChangeCamera(camera);
            }
        }

        private void DropdownCameraSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var camera = dropdownCameraSelect.SelectedItem as String;

            Log.Debug("calling changeCamera to : " + camera);

            if (dropdownCameraSelect.Items.Count > 1 && dropdownCameraSelect.SelectedIndex != cameraIndex)
            {
                Task.Run(() => changeCamera(camera));
            }
        }

        private void DropdownGestureSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EvtGestureSelected == null)
            {
                return;
            }

            if (dropdownGestureSelect.SelectedIndex == ctIndex)
            {
                EvtGestureSelected(true, false);
            }
            if (dropdownGestureSelect.SelectedIndex == erIndex)
            {
                EvtGestureSelected(false, true);
            }
            if (dropdownGestureSelect.SelectedIndex == bothIndex)
            {
                EvtGestureSelected(true, true);
            }
        }

        private void initializeGestureDropdown()
        {
            var ctEnabled = false;
            var erEnabled = false;

            dropdownGestureSelect.Items.Clear();

            foreach (var actuatorSwitch in _cameraActuator.Switches)
            {
                int index = dropdownGestureSelect.Items.Add(actuatorSwitch.Name);

                if (String.Compare(actuatorSwitch.Source, "CT", true) == 0)
                {
                    ctIndex = index;
                    if (actuatorSwitch.IsSelectTriggerSwitch() && actuatorSwitch.Enabled)
                    {
                        ctEnabled = true;
                    }
                }
                else if (String.Compare(actuatorSwitch.Source, "ER", true) == 0)
                {
                    erIndex = index;
                    if (actuatorSwitch.IsSelectTriggerSwitch() && actuatorSwitch.Enabled)
                    {
                        erEnabled = true;
                    }
                }
            }

            bothIndex = dropdownGestureSelect.Items.Add("Either");

            if (!ctEnabled && !erEnabled)
            {
                dropdownGestureSelect.SelectedIndex = ctIndex;
            }
            else if (ctEnabled && erEnabled)
            {
                dropdownGestureSelect.SelectedIndex = bothIndex;
            }
            else if (ctEnabled)
            {
                dropdownGestureSelect.SelectedIndex = ctIndex;
            }
            else if (erEnabled)
            {
                dropdownGestureSelect.SelectedIndex = erIndex;
            }
        }

        private void refreshCameraList()
        {
            Invoke(new MethodInvoker(delegate
            {
                _installedCameras = Cameras.GetCameraNames();

                dropdownCameraSelect.Items.Clear();

                foreach (var im in _installedCameras)
                {
                    dropdownCameraSelect.Items.Add(im);
                }

                if (dropdownCameraSelect.Items.Count > 0)
                {
                    setPreferredCamera();
                }
            }));
        }

        private void setControlsEnable(bool enable, bool isCalibrating = false)
        {
            var buttonEnable = (isCalibrating) ? true : enable;

            Windows.SetEnabled(buttonRecalibrate, buttonEnable);

            Windows.SetBackgroundColor(buttonRecalibrate, buttonEnable ? _buttonBackColor : Color.DimGray);

            Windows.SetEnabled(dropdownCameraSelect, buttonEnable);
        }

        private void setPreferredCamera()
        {
            if (dropdownCameraSelect.Items.Count == 0)
            {
                return;
            }

            if (dropdownCameraSelect.Items.Count == 1)
            {
                dropdownCameraSelect.SelectedIndex = 0;
            }
            else
            {
                var preferredCamera = _cameraActuator.PreferredCamera;

                var index = 0;
                foreach (var item in dropdownCameraSelect.Items)
                {
                    if (String.Compare(item.ToString(), preferredCamera, true) == 0)
                    {
                        dropdownCameraSelect.SelectedIndex = index;
                        cameraIndex = dropdownCameraSelect.SelectedIndex;
                        return;
                    }
                    index++;
                }

                dropdownCameraSelect.SelectedIndex = 0;
            }

            cameraIndex = dropdownCameraSelect.SelectedIndex;
        }
    }
}