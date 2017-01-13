////////////////////////////////////////////////////////////////////////////
// <copyright file="VisionActuator.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Extensions.Default.Actuators.VisionActuator.VisionUtils;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.Vision.VisionActuator
{
    /// <summary>
    /// Interacts with ACAT Vision which uses a camera to
    /// detect facial gestures and trigger ACAT.
    /// </summary>
    [DescriptorAttribute("7DA7F870-80DC-47B4-994C-5F46A4DFE538",
                            "Vision Actuator",
                            "Actuator for ACAT Vision which uses facial gestures")]
    internal class VisionActuator : ActuatorBase
    {
        /// <summary>
        /// The settings object for this actuator
        /// </summary>
        public static Settings VisionActuatorSettings;

        /// <summary>
        /// Name of the file that stores the settings for
        /// this actuator
        /// </summary>
        internal const String SettingsFileName = "VisionActuatorSettings.xml";

        /// <summary>
        /// Title text for dialogs
        /// </summary>
        private const String _title = "ACAT Vision";

        /// <summary>
        /// Thread that invokes the ACAT Vision sensor.  This is
        /// done in a separate thread as the entry point into the
        /// vision sensor module is blocking. So we can't have
        /// it on the main thread
        /// </summary>
        private Thread _acatVisionThread;

        /// <summary>
        /// The form that dislays the current status of the camera (eg
        /// "Initializaing camera..."
        /// </summary>
        private CameraStatusForm _cameraStatusForm;

        /// <summary>
        /// Thread to display the camera status form
        /// </summary>
        private Thread _cameraStatusThread;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Is initialization in progress?
        /// </summary>
        private bool _initInProgress;

        /// <summary>
        /// Callback function from ACAT Vision. This function
        /// is called to send gesture trigger and other messages
        /// </summary>
        private VisionSensor.VisionEventCallback _visionSensorCallback;

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public override bool SupportsPreferencesDialog { get { return true;  } }

        /// <summary>
        /// Class factory to create the switch object
        /// </summary>
        /// <returns>the switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new VisionActuatorSwitch();
        }

        /// <summary>
        /// Class factory to create the switch object from an existing
        /// switch object
        /// </summary>
        /// <param name="sourceSwitch">Source switch to clone</param>
        /// <returns>Vision actuator switch object</returns>
        public override IActuatorSwitch CreateSwitch(IActuatorSwitch sourceSwitch)
        {
            return new VisionActuatorSwitch(sourceSwitch);
        }

        /// <summary>
        /// Initialize the vision actuator.  Detect cameras,
        /// let the user select the preferred camera (if reqd),
        /// spawn threads etc.
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public override bool Init()
        {
            Attributions.Add("ACAT Vision",
                "Open Source Computer Vision Library (OpenCV) is licensed under the BSD license");

            if (!Cameras.GetCameraNames().Any())
            {
                MessageBox.Show(R.GetString("NoCamerasFoundACATVisionDisabled"),
                                _title,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                OnInitDone();

                return true;
            }

            _initInProgress = true;

            // load settings from the settings file
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            VisionActuatorSettings = Settings.Load();

            VisionSensor.init();

            bool retVal = detectAndSetPreferredCamera();
            if (retVal)
            {
                showCameraStatus(R.GetString("InitializingCamera"));

                _acatVisionThread = new Thread(visionThread) { IsBackground = true };
                _acatVisionThread.Start();
            }
            else
            {
                _initInProgress = false;
                OnInitDone();
            }

            return retVal;
        }

        /// <summary>
        /// Invoked when the user presses the button on the
        /// calibration dialog
        /// </summary>
        public override void OnCalibrationAction()
        {
            VisionSensor.showVideoWindow();
        }

        /// <summary>
        /// Invoked if the calibration is canceled
        /// </summary>
        public override void OnCalibrationCanceled()
        {
            VisionSensor.hideVideoWindow();

            // if we are in the initialization state, signal that
            // we are done
            if (_initInProgress)
            {
                OnInitDone();
                _initInProgress = false;
            }
        }

        public override void OnRegisterSwitches()
        {
#if unused
            SwitchSetting switchSetting = new SwitchSetting("CT", "Cheek twitch gesture", "CmdMainMenu");
            Context.AppActuatorManager.RegisterSwitch(this, switchSetting);
#endif
        }

        /// <summary>
        /// Pause the actuator
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
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
            var prefChooseForm = new PrefChooseForm {Actuator = this};
            prefChooseForm.ShowDialog();
            return true;
        }

        /// <summary>
        /// Starts calibration of the actuator
        /// </summary>
        public override void StartCalibration()
        {
            try
            {
                Log.Debug("Calling UpdateCalibrationstatus");

                UpdateCalibrationStatus(_title,
                                        R.GetString("CalibratingCameraRemainStill"),
                                        0,
                                        !VisionSensor.isVideoWindowVisible(),
                                        R.GetString("ShowVideo"));
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
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
            switch (switchObj.Action)
            {
                case SwitchAction.Down:
                    OnSwitchActivated(switchObj);
                    break;

                case SwitchAction.Up:
                    OnSwitchDeactivated(switchObj);
                    break;

                case SwitchAction.Trigger:
                    OnSwitchTriggered(switchObj);
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

            IActuatorSwitch actuatorSwitch = parseActuatorMsgAndGetSwitch(text, ref gesture);

            if (actuatorSwitch != null)
            {
                triggerEvent(actuatorSwitch);
            }
            else if (gesture == "CALIB_START") // begin camera calibration
            {
                // if the camera status form is currently displayed, close it
                dismissCameraStatus();

                try
                {
                    Log.Debug("Received CALIB_START");

                    Log.Debug("Calling RequestCalibration");
                    RequestCalibration();
                    Log.Debug("Returned from RequestCalibration");
                }
                catch (Exception ex)
                {
                    Log.Debug("Exception " + ex);
                }
            }
            else if (gesture == "CALIB_END") // end camera calibration
            {
                Log.Debug("CALIB_END");

                // if we are in the initialization state, signal that
                // we are done
                if (_initInProgress)
                {
                    OnInitDone();
                    _initInProgress = false;
                }

                VisionSensor.hideVideoWindow();

                OnEndCalibration();
            }
        }

        /// <summary>
        /// Callback from the status form that the user closed
        /// it manually
        /// </summary>
        private void cameraStatusForm_EvtCancel()
        {
            VisionSensor.hideVideoWindow();

            if (_initInProgress)
            {
                _initInProgress = false;
                OnInitDone();
            }
        }

        /// <summary>
        /// Thread function to displays the current status of the camera
        /// </summary>
        private void cameraStatusThread(String prompt)
        {
            _cameraStatusForm = new CameraStatusForm { Prompt = prompt };
            _cameraStatusForm.EvtCancel += cameraStatusForm_EvtCancel;
            _cameraStatusForm.ShowDialog();
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
            var installedCameras = Cameras.GetCameraNames();
            var preferredCamera = String.Empty;
            bool retVal = true;

            if (hasCameraListChanged() || !isCameraInstalled(VisionActuatorSettings.PreferredCamera))
            {
                VisionActuatorSettings.CameraList = installedCameras.ToArray();

                // if there is more than one camera, let the user pick
                if (installedCameras.Count() > 1)
                {
                    var form = new CameraSelectForm
                    {
                        CameraNames = installedCameras, 
                        Prompt = R.GetString("SelectCamera"), 
                        OKButtonText = R.GetString("OK"),
                        CancelButtonText = R.GetString("Cancel"),
                        Name = _title
                    };

                    var result = form.ShowDialog();

                    if (result == DialogResult.Cancel)
                    {
                        MessageBox.Show(R.GetString("NoCameraSelectedVisionWillNotBeActivated"),
                                            _title,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        retVal = false;
                    }
                    else
                    {
                        MessageBox.Show(String.Format(R.GetString("SelectedCamera"), form.SelectedCamera),
                                        _title,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        preferredCamera = form.SelectedCamera;
                    }
                }
                else
                {
                    preferredCamera = installedCameras.ElementAt(0);
                }
            }
            else
            {
                preferredCamera = VisionActuatorSettings.PreferredCamera;
            }

            VisionActuatorSettings.PreferredCamera = preferredCamera;

            VisionActuatorSettings.Save();

            VisionSensor.selectCamera(preferredCamera);

            return retVal;
        }

        /// <summary>
        /// Dismisses the camera status dialog
        /// </summary>
        private void dismissCameraStatus()
        {
            if (_cameraStatusForm != null)
            {
                _cameraStatusForm.Cancel();
                _cameraStatusForm = null;
            }
        }

        /// <summary>
        /// Detects if the list of installed cameras has changed
        /// since the previous run of ACAT
        /// </summary>
        /// <returns>true if it has</returns>
        private bool hasCameraListChanged()
        {
            var listFromSettings = VisionActuatorSettings.CameraList;
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

        /// <summary>
        /// Checks if the specified camera is currently active
        /// </summary>
        /// <param name="camera">name of the camera</param>
        /// <returns>true if it is</returns>
        private bool isCameraInstalled(String camera)
        {
            var trimmedCamera = camera.Trim();

            if (String.IsNullOrEmpty(trimmedCamera))
            {
                return false;
            }

            var activeCameraList = Cameras.GetCameraNames();

            return activeCameraList.Any(c => String.Compare(c, camera, false) == 0);
        }

        /// <summary>
        /// Displays the camera status dialog
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        private void showCameraStatus(String prompt)
        {
            dismissCameraStatus();

            _cameraStatusThread = new Thread(() => cameraStatusThread(prompt)) { IsBackground = true };
            _cameraStatusThread.Start();
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
            actuatorState = State.Stopped;

            VisionSensor.quit();

            // perform unitialization here
        }

        /// <summary>
        /// Thread function that runs the vision module (which is
        /// a blocking call)
        /// </summary>
        private void visionThread()
        {
            _visionSensorCallback = callbackFromVision;

            VisionSensor.SetVisionEventHandler(_visionSensorCallback);

            VisionSensor.acatVision();
        }
    }
}