////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigureActuatorForm.cs
//
// Allows user to select the camera, preferred gesture and change
// gesture settings.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    public partial class ConfigureActuatorForm : Form
    {
        public bool IsClosing;
        // TODO - Localize Me
        private const String _calibrating = "Calibrating...\nPlease be still";
        // TODO - Localize Me
        private const String _initializing = "Initializing\nPlease wait...";
        // TODO - Localize Me
        private const String _switchingCamera = "Switching camera\nPlease wait...";
        // TODO - Localize Me
        private const String textCalibratingNeutralFace = "In Calibration - System is calibrating for NEUTRAL face\n\nDO NOT do any head moverments or facial gestures\n\nStay still";
        // TODO - Localize Me
        private const String textSelectCamera = "Practice the selected gestures a few times.\n\nACAT will indicate below if it detects a successful gesture.\n\nClick on Settings to adjust gesture sensitivity and hold times";
        // TODO - Localize Me
        private const String textSetParameters = "1. Adjust settings.\n2. Click on Apply Changes\n3. Test your gestures until you’re comfortable with the settings.\n\nACAT will indicate below if it detects a successful gesture";
        private Color _buttonBackColor;
        private bool _gestureDetectedAtleastOnce = false;
        private CameraActuator _cameraActuator;
        private int _gestureCount = 0;
        private Mode _mode = Mode.SelectCamera;
        private SampleImageForm _sampleImageForm = null;
        private Timer _textTimer;
        private Timer _timer;
        private VideoWindowFinder _videoWindowFinder;
        private WebcamGestureSelectUserControl _webcamGestureSelectUserControl;
        private WebcamGestureSettingsUserControl _webcamGestureSettingsUserControl;
        private WindowActiveWatchdog _windowActiveWatchdog;
        private WindowOverlapWatchdog _windowOverlapWatchdog;

        internal ConfigureActuatorForm(CameraActuator cameraActuator)
        {
            InitializeComponent();

            _buttonBackColor = buttonDone.BackColor;

            _cameraActuator = cameraActuator;

            Load += ConfigureActuatorForm_Load;
            Shown += ConfigureActuatorForm_Shown;
            FormClosing += ConfigureActuatorForm_FormClosing;

            cameraActuator.EvtCalibrationStart += CameraActuator_EvtCalibrationStart;
            cameraActuator.EvtCalibrationEnd += CameraActuator_EvtCalibrationEnd;
            cameraActuator.EvtGestureDetected += CameraActuator_EvtGestureDetected;

            cameraActuator.EvtChangeCameraStart += CameraActuator_EvtChangeCameraStart;
            cameraActuator.EvtChangeCameraEnd += CameraActuator_EvtChangeCameraEnd;
        }

        private enum Mode
        {
            SelectCamera,
            SetParameters
        }

        private void _textTimer_Tick(object sender, EventArgs e)
        {
            _textTimer.Stop();

            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    labellMessage.Text = String.Empty;
                }));
            }
            catch
            {
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            Log.Debug("_cameraActuator.CameraSensorRunning: " + _cameraActuator.CameraSensorRunning);

            if (_cameraActuator.CameraSensorRunning)
            {
                stopTimer();
                setNotificationText();

                setLabelPromptDefaultText();

                Log.Debug("Camera sensor is is running. Calling _videoWindowFinder.Start()");
                //_videoWindowFinder.Start();
            }
        }

        private void _videoWindowFinder_EvtVideoWindowDisplayed(IntPtr handle)
        {
            Log.Debug();

            try
            {
                _windowOverlapWatchdog = new WindowOverlapWatchdog(handle.ToInt32());

                Log.Debug("Docking calibration window");

                _videoWindowFinder.DockVideoWindow(this);

                setControlsEnable(true);

                //setNotificationText();

                if (_sampleImageForm == null)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        _sampleImageForm = new SampleImageForm();

                        User32Interop.RECT rect = new User32Interop.RECT();
                        User32Interop.GetWindowRect(handle, out rect);

                        _sampleImageForm.Show(this);
                        _sampleImageForm.Location = new Point(rect.left, rect.bottom);
                        _sampleImageForm.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
                    }));
                }
            }
            catch
            {
            }
        }

        private void _videoWindowFinder_EvtVideoWindowFindStart(object sender, EventArgs e)
        {
            setNotificationText(_initializing);
        }

        private void _webcamGestureSelectUserControl_EvtGestureSelected(bool cheekTwitch, bool eyebrowRaise)
        {
            var switches = _cameraActuator.Switches;

            if (cheekTwitch && eyebrowRaise)
            {
                enableSwitch("Either");
            }
            else if (cheekTwitch)
            {
                enableSwitch("CT");
            }
            else if (eyebrowRaise)
            {
                enableSwitch("ER");
            }
        }

        private void _webcamGestureSettingsUserControl_EvtPause()
        {
            pause();
        }

        private void _webcamGestureSettingsUserControl_EvtResume()
        {
            resume();
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (!_cameraActuator.CameraSensorRunning)
            {
                if (!showDoneActionConfirm("Camera switch not initialized. The selected facial gestures may not be detected. Exit Webcam Calibration?"))
                {
                    return;
                }
            }
            else if (_cameraActuator.IsCalibrating)
            {
                if (!showDoneActionConfirm("Calibration is in progress. Exit Webcam calibration?"))
                {
                    return;
                }
            }
            else if (_cameraActuator.CameraActuatorInitInProgress && !_gestureDetectedAtleastOnce )
            {
                if (!showDoneActionConfirm("You have not tested your facial gestures to check if ACAT can detect them. Exit Webcam Calibration?"))
                {
                    return;
                }
            }

            IsClosing = true;

            if (_videoWindowFinder != null)
            {
                _videoWindowFinder.EvtVideoWindowDisplayed -= _videoWindowFinder_EvtVideoWindowDisplayed;
                _videoWindowFinder.EvtVideoWindowFindStart -= _videoWindowFinder_EvtVideoWindowFindStart;

                Log.Debug("Calling dockvideo dispose");
                _videoWindowFinder.Dispose();
                Log.Debug("Returned from dockvideo dispose");
            }

            CameraSensor.hideVideoWindow();

            _textTimer.Stop();

            _windowActiveWatchdog.Dispose();

            saveActuatorConfig();

            _webcamGestureSelectUserControl.UnInit();
            _webcamGestureSettingsUserControl.UnInit();

            Close();
        }

        private void buttonSettingsOrGoBack_Click(object sender, EventArgs e)
        {
            switch (_mode)
            {
                case Mode.SelectCamera:
                    setMode(Mode.SetParameters);
                    break;

                case Mode.SetParameters:
                    if (panelContainer.Controls[0] == _webcamGestureSettingsUserControl)
                    {
                        _webcamGestureSettingsUserControl.OnClose();
                    }

                    setMode(Mode.SelectCamera);
                    break;
            }
        }

        private void CalibrateForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void CameraActuator_EvtCalibrationEnd(object sender, EventArgs e)
        {
            if (!IsClosing)
            {
                Invoke(new MethodInvoker(delegate
                {
                    setNotificationText("Calibration Done");

                    setLabelPromptDefaultText();

                    if (_textTimer != null)
                    {
                        _textTimer.Stop();
                        _textTimer.Start();
                    }
                }));

                setControlsEnable(true);
            }
        }

        private void CameraActuator_EvtCalibrationStart(object sender, EventArgs e)
        {
            if (!IsClosing)
            {
                _gestureCount = 0;
                setControlsEnable(false, true);
                setNotificationText(_calibrating);
            }
        }

        private void CameraActuator_EvtChangeCameraEnd()
        {
            var form = Context.AppPanelManager.GetCurrentForm() as Form;
            form.Invoke(new MethodInvoker(delegate
            {
                setLabelPromptDefaultText();
            }));
        }

        private void CameraActuator_EvtChangeCameraStart(String camera)
        {
            _gestureDetectedAtleastOnce = false;

            setControlsEnable(false);

            var form = Context.AppPanelManager.GetCurrentForm() as Form;
            if (form == null)
            {
                form = this;
            }

            try
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    labelPrompt.Text = String.Empty;
                    Log.Debug("Calling startTimer()");
                    startTimer(_switchingCamera);
                }));
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        private void CameraActuator_EvtGestureDetected(string gesture)
        {
            _gestureDetectedAtleastOnce = true;
            updateGesture(gesture);
        }

        private void ConfigureActuatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_windowOverlapWatchdog != null)
            {
                _windowOverlapWatchdog.Dispose();
                _windowOverlapWatchdog = null;
            }

            if (_sampleImageForm != null)
            {
                Invoke(new MethodInvoker(delegate
                {
                    _sampleImageForm.Close();
                }));
            }
        }

        private void ConfigureActuatorForm_Load(object sender, EventArgs e)
        {
            Left = Top = 0;

            Log.Debug("ENtered ConfigureActuatorForm_Load");

            Resize += CalibrateForm_Resize;
            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            _webcamGestureSelectUserControl = new WebcamGestureSelectUserControl(_cameraActuator);
            _webcamGestureSettingsUserControl = new WebcamGestureSettingsUserControl(_cameraActuator);

            _webcamGestureSettingsUserControl.EvtPause += _webcamGestureSettingsUserControl_EvtPause;
            _webcamGestureSettingsUserControl.EvtResume += _webcamGestureSettingsUserControl_EvtResume;

            _webcamGestureSelectUserControl.EvtGestureSelected += _webcamGestureSelectUserControl_EvtGestureSelected;

            setMode(Mode.SelectCamera);

            labelPrompt.Text = String.Empty;

            setControlsEnable(false);
        }

        private void ConfigureActuatorForm_Shown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);

            _textTimer = new Timer
            {
                Interval = 1000
            };
            _textTimer.Tick += _textTimer_Tick;

            _videoWindowFinder = new VideoWindowFinder();
            _videoWindowFinder.EvtVideoWindowDisplayed += _videoWindowFinder_EvtVideoWindowDisplayed;
            _videoWindowFinder.EvtVideoWindowFindStart += _videoWindowFinder_EvtVideoWindowFindStart;

            startTimer(_initializing);

            _videoWindowFinder.Start();
        }

        private void enableSwitch(String source)
        {
            foreach (var actuatorSwitch in _cameraActuator.Switches)
            {
                actuatorSwitch.Enabled = String.Compare(actuatorSwitch.Source, source, true) == 0 || String.Compare(source, "Either", true) == 0;
            }
        }

        private List<String> getSelectedSwitchList()
        {
            var list = new List<String>();
            foreach (var actuatorSwitch in _cameraActuator.Switches)
            {
                if (actuatorSwitch.Enabled)
                {
                    list.Add(actuatorSwitch.Source);
                }
            }

            return list;
        }

        private bool isSwitchEnabled(String source)
        {
            foreach (var actuatorSwitch in _cameraActuator.Switches)
            {
                if (String.Compare(actuatorSwitch.Source, source, true) == 0)
                {
                    return actuatorSwitch.Enabled;
                }
            }

            return false;
        }

        private void pause()
        {
            _windowActiveWatchdog.Pause();
        }

        private void resume()
        {
            _windowActiveWatchdog.Resume();
        }
        private void saveActuatorConfig()
        {
            var actuatorConfig = Context.AppActuatorManager.GetActuatorConfig();

            var actuatorSetting = actuatorConfig.Find(_cameraActuator.Descriptor.Id);
            foreach (var switchSetting in actuatorSetting.SwitchSettings)
            {
                switchSetting.Enabled = isSwitchEnabled(switchSetting.Source);
            }

            actuatorConfig.Save();
        }

        private void setControlsEnable(bool enable, bool isCalibrating = false)
        {
            var buttonEnable = (isCalibrating) ? false : enable;

            //Windows.SetEnabled(buttonDone, buttonEnable);

            Windows.SetEnabled(buttonSettingsOrGoBack, buttonEnable);

            //Windows.SetBackgroundColor(buttonDone, buttonEnable ? _buttonBackColor : Color.DimGray);

            Windows.SetBackgroundColor(buttonSettingsOrGoBack, buttonEnable ? _buttonBackColor : Color.DimGray);

            switch (_mode)
            {
                case Mode.SelectCamera:
                    _webcamGestureSelectUserControl.EnableControls(enable, isCalibrating);
                    break;

                case Mode.SetParameters:
                    break;
            }
        }

        private void setLabelPromptDefaultText()
        {
            switch (_mode)
            {
                case Mode.SelectCamera:
                    labelPrompt.Text = textSelectCamera;
                    break;

                case Mode.SetParameters:
                    labelPrompt.Text = textSetParameters;
                    break;
            }
        }

        private void setMode(Mode mode)
        {
            _mode = mode;

            panelContainer.Controls.Clear();

            switch (mode)
            {
                case Mode.SelectCamera:
                    _webcamGestureSelectUserControl.Initialize();
                    _webcamGestureSelectUserControl.Dock = DockStyle.Fill;
                    labelPrompt.Text = textSelectCamera;
                    panelContainer.Controls.Add(_webcamGestureSelectUserControl);
                    _webcamGestureSelectUserControl.Shown();
                    buttonSettingsOrGoBack.Text = "Settings";
                    buttonDone.Visible = true;
                    labelTitle.Text = "Camera Calibration";
                    break;

                case Mode.SetParameters:
                    var list = getSelectedSwitchList();
                    _webcamGestureSettingsUserControl.Initialize(list);
                    _webcamGestureSettingsUserControl.Dock = DockStyle.Fill;
                    labelPrompt.Text = textSetParameters;
                    panelContainer.Controls.Add(_webcamGestureSettingsUserControl);
                    _webcamGestureSettingsUserControl.Shown();
                    buttonSettingsOrGoBack.Text = "Go Back";
                    buttonDone.Visible = false;
                    labelTitle.Text = "Camera Calibration: Settings";
                    break;
            }
        }
        private void setNotificationText(String text = "")
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    if (_cameraActuator.IsCalibrating)
                    {
                        text = _calibrating;
                        labelPrompt.Text = textCalibratingNeutralFace;
                    }
                    labellMessage.Text = text;
                    labellMessage.Refresh();
                }));
            }
            catch
            {
            }
        }

        private bool showDoneActionConfirm(String message)
        {
            pause();

            var msgBox = new ConfirmBoxTwoOptions
            {
                Prompt = message,
                Op1Prompt = "Yes",
                Op2Prompt = "No"
            };

            msgBox.ShowDialog(this);

            var result = msgBox.OptionsResult;

            msgBox.Dispose();

            resume();

            return result == ConfirmBoxTwoOptions.Options.Option1;
        }

        private void startTimer(String message)
        {
            setNotificationText(message);
            if (_timer != null && _timer.Enabled)
            {
                stopTimer();
            }

            _timer = new Timer
            {
                Interval = 500
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void stopTimer()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        private void updateGesture(String gesture)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    setNotificationText(gesture + " detected (#" + (++_gestureCount) + ")");

                    if (_textTimer != null)
                    {
                        _textTimer.Stop();
                        _textTimer.Start();
                    }
                }));
            }
            catch
            {
            }
        }
    }
}