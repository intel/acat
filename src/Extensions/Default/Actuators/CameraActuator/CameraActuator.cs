////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CameraActuator.cs
//
// Interacts with ACAT Vision which uses a camera to
// detect facial gestures and trigger ACAT.
//
////////////////////////////////////////////////////////////////////////////

//#define ACAT_ACTUATE

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    /// <summary>
    /// Uses a camera to detect facial gestures and trigger ACAT.
    /// </summary>
    [DescriptorAttribute("7DA7F870-80DC-47B4-994C-5F46A4DFE538",
                            "Camera Actuator",
                            "Actuator which uses a webcam to detect facial gestures and trigger ACAT")]
    internal class CameraActuator : ActuatorBase, ISupportsPreferences
    {
        /// <summary>
        /// The settings object for this actuator
        /// </summary>
        public static Settings CameraActuatorSettings = null;

        public String PreferredCamera;

        
        /// <summary>
        /// Name of the file that stores the settings for
        /// this actuator
        /// </summary>
        internal const String SettingsFileName = "VisionActuatorSettings.xml";

        internal volatile bool IsCalibrating = false;
        internal volatile bool CameraSensorRunning = false;

        /// <summary>
        /// Thread that invokes the ACAT Camera sensor.  This is
        /// done in a separate thread as the entry point into the
        /// vision sensor module is blocking. So we can't have
        /// it on the main thread
        /// </summary>
        private Thread _acatVisionThread;

        private AutoCalibrateForm _autoCalibrateForm;

        private bool _calibrateAndTestInProgress;

        private ConfigureActuatorForm _configureActuatorForm;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Is initialization in progress?
        /// </summary>
        private bool _cameraActuatorInitInProgress;

        /// <summary>
        /// Callback function from ACAT Vision. This function
        /// is called to send gesture trigger and other messages
        /// </summary>
        private CameraSensor.CameraEventCallback _visionCallback;

        internal delegate void changeCameraEndDelegate();

        internal delegate void changeCameraStartDelegate(String camera);

        internal delegate void gestureDetectedDelegate(String gesture);

        internal event EventHandler EvtCalibrationEnd;

        internal event EventHandler EvtCalibrationStart;

        internal event changeCameraEndDelegate EvtChangeCameraEnd;

        internal event changeCameraStartDelegate EvtChangeCameraStart;

        internal event gestureDetectedDelegate EvtGestureDetected;

        internal event EventHandler EvtCameraSensorInitDone;

        public override String OnboardingImageFileName
        {
            get
            {
                return FileUtils.GetImagePath("WebcamSwitch.jpg");
            }
        }

        public override bool ShowTryoutOnStartup
        {
            get
            {
                return CoreGlobals.AppPreferences.ShowSwitchTryoutOnStartup;
            }
        }

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public override bool SupportsPreferencesDialog
        { get { return false; } }

        public override bool SupportsScanTimingsConfigureDialog
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsTryout
        {
            get
            {
                return true;
            }
        }

        internal bool CameraActuatorInitInProgress
        {
            get
            {
                return _cameraActuatorInitInProgress;
            }
        }

        /// <summary>
        /// Class factory to create the switch object
        /// </summary>
        /// <returns>the switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new CameraActuatorSwitch();
        }

        /// <summary>
        /// Class factory to create the switch object from an existing
        /// switch object
        /// </summary>
        /// <param name="sourceSwitch">Source switch to clone</param>
        /// <returns>Camera actuator switch object</returns>
        public override IActuatorSwitch CreateSwitch(IActuatorSwitch sourceSwitch)
        {
            return new CameraActuatorSwitch(sourceSwitch);
        }

        /// <summary>
        /// Returns the default preferences object
        /// </summary>
        /// <returns>default preferences object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            if (CameraActuatorSettings == null)
            {
                Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
                CameraActuatorSettings = Settings.Load();
            }

            return PreferencesBase.LoadDefaults<Settings>();
        }

        /// <summary>
        /// Returns the preferences object
        /// </summary>
        /// <returns>preferences object</returns>
        public override IPreferences GetPreferences()
        {
            if (CameraActuatorSettings == null)
            {
                Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
                CameraActuatorSettings = Settings.Load();
            }

            return CameraActuatorSettings;
        }

        /// <summary>
        /// Initialize the Camera actuator.  Detect cameras,
        /// let the user select the preferred camera (if reqd),
        /// spawn threads etc.
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public override bool Init()
        {
            Attributions.Add("ACAT Camera Actuator",
                "Open Source Computer Vision Library (OpenCV) is licensed under the BSD license");

            // load settings from the settings file
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            CameraActuatorSettings = Settings.Load();

            CalibrationDoneAtleastOnce = false;

            CameraSensor.init();

            OnInitDone();

            return true;
        }

        /// <summary>
        /// Pause the actuator
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
        }

        public override bool PostInit()
        {
            if (!Cameras.GetCameraNames().Any())
            {
                //ConfirmBoxSingleOption.ShowDialog(R.GetString("NoCamerasFoundACATVisionDisabled"), "OK");
                ConfirmBoxSingleOption.ShowDialog("You have selected Camera as the input switch, but no cameras were found. Please connect a camera and retry.\nExiting.", "OK");
                OnPostInitDone();
                return false;
            }

            _cameraActuatorInitInProgress = true;

            bool retVal = detectAndSetPreferredCamera();
            if (retVal)
            {
                _acatVisionThread = new Thread(visionThread) { IsBackground = true };
                _acatVisionThread.Start();

                showCalibrationDialogOnStartup();
            }
            else
            {
                _cameraActuatorInitInProgress = false;
                OnPostInitDone();
            }

            return retVal;
        }

        /// <summary>
        /// Resume the actuator
        /// </summary>
        public override void Resume()
        {
            actuatorState = State.Running;
        }

        /// <summary>
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        public override bool ShowPreferencesDialog()
        {
            return true;
        }

        public override bool ShowScanTimingsConfigureDialog()
        {
            return ShowDefaultScanTimingsConfigureDialog();
        }

        public override bool ShowTryoutDialog()
        {
            return ShowDefaultTryoutDialog();
        }

        /// <summary>
        public override void StartCalibration(RequestCalibrationReason reason)
        {
            try
            {
                Log.Debug("Calling UpdateCalibrationstatus");

                if (_autoCalibrateForm != null)
                {
                    Windows.CloseForm(_autoCalibrateForm);
                    _autoCalibrateForm = null;
                }

                Log.Debug("Calling NotifyStartCalibration");

                Context.AppActuatorManager.NotifyStartCalibration(new CalibrationNotifyEventArgs(true, false));

                if (reason == RequestCalibrationReason.SensorInitiated)
                {
                    Log.Debug("Calling new Calibform");
                    _autoCalibrateForm = new AutoCalibrateForm(this);

                    Log.Debug("Calling new Calibform show dialog");

                    var form = Context.AppPanelManager.GetCurrentForm() as Form;
                    if (form != null)
                    {
                        form.Invoke(new MethodInvoker(delegate
                        {
                            _autoCalibrateForm.ShowDialog(form);
                        }));
                    }
                    else
                    {
                        _autoCalibrateForm.ShowDialog();
                    }
                    Log.Debug("Returned from Calibform show dialog");

                    _autoCalibrateForm = null;
                }
                else
                {
                    CameraSensor.showVideoWindow();

                    if (!_calibrateAndTestInProgress)
                    {
                        showCalibrationAndTestForm();
                    }
                }

                Log.Debug("Calling NotifyEndCalibration");
                Context.AppActuatorManager.NotifyEndCalibration();

                Log.Debug("Calling OnEndCalibration");
                OnEndCalibration();

                if (_cameraActuatorInitInProgress)
                {
                    _cameraActuatorInitInProgress = false;

                    OnPostInitDone();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Indicates whether the actuator supports calibration or not
        /// </summary>
        /// <returns>true if it does, false otherwise</returns>
        public override bool SupportsCalibration()
        {
            return true;
        }

        internal void ChangeCamera(String camera)
        {
            CameraSensorRunning = false;

            EvtChangeCameraStart?.Invoke(camera);

            unInit();

            Thread.Sleep(5000);

            CameraSensor.init();

            CameraSensor.selectCamera(camera);

            PreferredCamera = camera;

            CameraActuatorSettings.PreferredCamera = camera;

            CameraActuatorSettings.Save();

            _acatVisionThread = new Thread(visionThread) { IsBackground = true };
            _acatVisionThread.Start();

            EvtChangeCameraEnd?.Invoke();
        }

        /// <summary>
        /// Checks if the specified camera is currently active
        /// </summary>
        /// <param name="camera">name of the camera</param>
        /// <returns>true if it is</returns>
        internal bool IsCameraInstalled(String camera)
        {
            var trimmedCamera = camera.Trim();

            if (String.IsNullOrEmpty(trimmedCamera))
            {
                return false;
            }

            var activeCameraList = Cameras.GetCameraNames();

            return activeCameraList.Any(c => String.Compare(c, camera, false) == 0);
        }

        internal void Recalibrate()
        {
            CameraSensor.visionCommand("action=RECALIBRATE", 0);
        }

        internal void SaveCameraList()
        {
            CameraActuatorSettings.CameraList = Cameras.GetCameraNames().ToArray();
            CameraActuatorSettings.Save();
        }

        internal void setCheekTwitchHoldTime(int value)
        {
            CameraSensor.visionCommand("action=CHEEK_HOLD_MSEC", value);
        }

        internal void setCheekTwitchSensitivity(int value)
        {
            CameraSensor.visionCommand("action=CHEEK_MOVEMENT_SENSITIVITY", value);
        }

        internal void setEyebrowRaiseHoldTime(int value)
        {
            CameraSensor.visionCommand("action=EYEBROW_HOLD_MSEC", value);
        }

        internal void setEyebrowRaiseSensitivity(int value)
        {
            CameraSensor.visionCommand("action=EYEBROW_MOVEMENT_SENSITIVITY", value);
        }

        internal void setHeadMovementSensitivity(int value)
        {
            CameraSensor.visionCommand("action=HEAD_MOVEMENT_SENSITIVITY", value);
        }

        internal void setVisionSettings()
        {
            Log.Debug("Setting vision parameters...");

            Log.Debug("HeadMovementSensitivity: " + CameraActuatorSettings.HeadMovementSensitivity);
            setHeadMovementSensitivity(CameraActuatorSettings.HeadMovementSensitivity);

            Log.Debug("CheekTwitchSensitivity: " + CameraActuatorSettings.CheekTwitchSensitivity);
            setCheekTwitchSensitivity(CameraActuatorSettings.CheekTwitchSensitivity);

            Log.Debug("EyebrowRaiseSensitivity: " + CameraActuatorSettings.EyebrowRaiseSensitivity);
            setEyebrowRaiseSensitivity(CameraActuatorSettings.EyebrowRaiseSensitivity);

            Log.Debug("CheekTwitchHoldTime: " + CameraActuatorSettings.CheekTwitchHoldTime);
            setCheekTwitchHoldTime(CameraActuatorSettings.CheekTwitchHoldTime);

            Log.Debug("EyebrowRaiseHoldTime: " + CameraActuatorSettings.EyebrowRaiseHoldTime);
            setEyebrowRaiseHoldTime(CameraActuatorSettings.EyebrowRaiseHoldTime);
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources

                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Depending on the switch action, raises events such as
        /// switch engaged, switch disengaged etc
        /// </summary>
        /// <param name="switchObj"></param>
        protected void triggerEvent(IActuatorSwitch switchObj)
        {
            if (!switchObj.Enabled)
            {
                return;
            }

            if (switchObj.Action == SwitchAction.Trigger)
            {
                EvtGestureDetected?.Invoke(switchObj.Name);
            }

            if (_calibrateAndTestInProgress)
            {
                return;
            }

            switch (switchObj.Action)
            {
                case SwitchAction.Down:
                    OnSwitchActivated(switchObj);
                    break;

                case SwitchAction.Up:
                    OnSwitchDeactivated(switchObj);
                    break;

                case SwitchAction.Trigger:
#if  !ACAT_ACTUATE
                    OnSwitchTriggered(switchObj);
#endif
                    break;
            }
        }

        /// <summary>
        /// This is the callback function from ACAT vision. This is
        /// in form of a string.
        /// Format of the string is:
        ///    gesture=gesturetype;action=gestureevent;conf=confidence;time=timestamp;actuate=flag;tag=userdata
        /// where
        ///  gesturetype    is a string representing the gesture. This is used as
        ///                 the 'source' field in the actuator switch object
        ///  gestureevent   should be a valid value from the SwitchAction enum
        ///  confidence     Integer representing the confidence level, for future use
        ///  timestamp      Timestamp of when the switch event triggered (in ticks)
        ///  flag           true/false.  If false, the switch trigger event will be ignored
        ///  userdata       Any user data
        /// Eg
        ///    gesture=G1;action=trigger;conf=75;time=3244394443
        /// </summary>
        /// <param name="text"></param>
        private void callbackFromVision(string text)
        {
            var gesture = String.Empty;

            Log.Debug("Received msg: " + text);

            IActuatorSwitch actuatorSwitch = parseActuatorMsgAndGetSwitch(text, ref gesture);

            //Log.Debug("Action: " + actuatorSwitch.Action);

            if (actuatorSwitch != null)
            {
#if ACAT_ACTUATE
                actuatorSwitch.Actuate = true;
#endif

                //Log.Debug("actuator switch != null. Calling trigger event, acutate = " + actuatorSwitch.Actuate);
                triggerEvent(actuatorSwitch);
            }
            else if (gesture == "INIT_DONE")
            {
                CameraSensorRunning = true;

                IsCalibrating = false;

                Task.Run(() => setVisionSettings());

                Task.Run(() => EvtCameraSensorInitDone?.Invoke(this, null));
            }
            else if (gesture == "CALIB_START") // begin camera calibration
            {
                try
                {
                    IsCalibrating = true;

                    Log.Debug("Received CALIB_START");

                    EvtCalibrationStart?.Invoke(this, new EventArgs());

                    if (!_cameraActuatorInitInProgress && !_calibrateAndTestInProgress)
                    {
                        Log.Debug("Calling RequestCalibration");
                        RequestCalibration(_calibrateAndTestInProgress ?
                                                RequestCalibrationReason.AppRequested :
                                                RequestCalibrationReason.SensorInitiated);
                        Log.Debug("Returned from RequestCalibration");
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("Exception " + ex);
                }
            }
            else if (gesture == "CALIB_END") // end camera calibration
            {
                Log.Debug("CALIB_END");

                IsCalibrating = false;

                CalibrationDoneAtleastOnce = true;

                EvtCalibrationEnd?.Invoke(this, new EventArgs());

                // if we are in the initialization state, signal that we are done
                //if (_initInProgress)
                {
                    actuatorState = State.Running;
                }
                //else
                if (!_cameraActuatorInitInProgress)
                {
                    OnEndCalibration();
                }
            }
        }

        /// <summary>
        /// Detects cameras installed on the computer and if reqd,
        /// asks the user to select from the list.  Saves the
        /// list of currently active cameras so if any change is detected
        /// the next time ACAT is run, we can display the list again.
        /// Also instructs ACAT Vision to use the selected camera
        /// </summary>
        /// <returns></returns>
        private bool detectAndSetPreferredCamera()
        {
            bool retVal = true;
            var installedCameras = Cameras.GetCameraNames();
            PreferredCamera = CameraActuatorSettings.PreferredCamera;

            if (!IsCameraInstalled(PreferredCamera))
            {
                // if there is more than one camera, just select the first one
                if (installedCameras.Count() >= 1)
                {
                    PreferredCamera = installedCameras.ElementAt(0);
                }
                else
                {
                    return false;
                }
            }

            CameraSensor.selectCamera(PreferredCamera);

            return retVal;
        }

        /// <summary>
        /// Detects if the list of installed cameras has changed
        /// since the previous run of ACAT
        /// </summary>
        /// <returns>true if it has</returns>
        private bool hasCameraListChanged()
        {
            var listFromSettings = CameraActuatorSettings.CameraList;
            var activeCameraList = Cameras.GetCameraNames();

            if (listFromSettings == null || activeCameraList == null)
            {
                return true;
            }

            if (listFromSettings.Length != activeCameraList.Count())
            {
                return true;
            }

            foreach (var cameraInstalled in activeCameraList)
            {
                bool found = listFromSettings.Any(cameraInSettings => String.Compare(cameraInstalled, cameraInSettings, false) == 0);

                if (!found)
                {
                    return true;
                }
            }

            return false;
        }

        private void showCalibrationAndTestForm()
        {
            if (_calibrateAndTestInProgress)
            {
                return;
            }

            _calibrateAndTestInProgress = true;

            _configureActuatorForm = new ConfigureActuatorForm(this);

            CameraSensor.showVideoWindow();

            var form = Context.AppPanelManager.GetCurrentForm() as Form;

            if (form != null)
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    _configureActuatorForm.ShowDialog(form);
                }));
            }
            else
            {
                _configureActuatorForm.ShowDialog();
            }

            CameraActuatorSettings.PreferredCamera = PreferredCamera;

            CameraActuatorSettings.Save();

            _calibrateAndTestInProgress = false;
        }

        /// <summary>
        /// Thread function to displays the current status of the camera
        /// </summary>
        private void showCalibrationDialogOnStartup()
        {
            if (hasCameraListChanged() || !IsCameraInstalled(CameraActuatorSettings.PreferredCamera))
            {
                SaveCameraList();
            }

            showCalibrationAndTestForm();

            _cameraActuatorInitInProgress = false;

            OnPostInitDone();
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
            actuatorState = State.Stopped;

            IsCalibrating = false;

            CameraSensor.visionCommand("action=EXITAPP", 0);

            CameraSensor.quit();
        }

        /// <summary>
        /// Thread function that runs the vision module (which is
        /// a blocking call)
        /// </summary>
        private void visionThread()
        {
            _visionCallback = callbackFromVision;

            CameraSensor.SetVisionEventHandler(_visionCallback);

            try
            {
                CameraSensor.acatVision();
            }
            catch (Exception ex)
            {
                Log.Debug("acatVision threw an exception:   " + ex.ToString());
                Log.Exception(ex);
            }

            Log.Debug("ACATvision quit");
        }

        public bool CalibrationDoneAtleastOnce
        {
            get;
            private set;
        }
    }
}