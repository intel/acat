﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardActuator.cs
//
// A keyboard actuator looks for specific keydown and keyup events
// and rasies switch activation events.  Each key (Eg F5, F6 etc) has a
// corresponding switch object and the event is propogated through the
// switch object.
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.InputActuators
{
    [DescriptorAttribute("D91A1877-C92B-4D7E-9AB6-F01F30B12DF9",
                        "Keyboard Actuator",
                        "Handles Keyboard and Mouse input")]
    public class KeyboardActuator : ActuatorBase
    {
        /// <summary>
        /// Indicated whetherthis object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Low-level keyboard hook callback function
        /// </summary>
        private KeyboardHook _keyboardHook;

        /// <summary>
        /// Low-level mouse hook callback function
        /// </summary>
        private MouseHook _mouseHook;

        /// <summary>
        /// Raised when a keydown is detected
        /// </summary>
        public event KeyEventHandler EvtKeyDown;

        /// <summary>
        /// Raised when a keypress is detected
        /// </summary>
        public event KeyPressEventHandler EvtKeyPress;

        /// <summary>
        /// Raised when a keyup is detected
        /// </summary>
        public event KeyEventHandler EvtKeyUp;

        /// <summary>
        /// Raised when a Mouse down is detected
        /// </summary>
        public event MouseEventHandler EvtMouseDown;

        public override String OnboardingImageFileName
        {
            get
            {
                return FileUtils.GetImagePath("KeyboardSwitch.png");
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
        {
            get { return true; }
        }

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

        /// <summary>
        /// Class factory to create a keyboard switch object
        /// </summary>
        /// <returns>The keyboard switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new KeyboardSwitch();
        }

        public override IOnboardingExtension GetOnboardingExtension()
        {
            return new OnboardingHardwareSwitchSetup(OnboardingHardwareSwitchSetup.SwitchType.Keyboard);
        }

        public override IEnumerable<String> GetSupportedKeyboardConfigs()
        {
            return new List<String>() { "TalkApplicationAbc", "TalkApplicationQwerty", "TalkApplicationAbcLnR", "TalkApplicationQwertyLnR" };
        }

        /// <summary>
        /// Initializes the keyboard actuator
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Init()
        {
            subscribeToHookManager();
            actuatorState = State.Running;

            OnInitDone();
            return true;
        }

        /// <summary>
        /// Pause actuator.  No events will be raised
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
        }

        /// <summary>
        /// Resume actuators.  Events will be raised
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
            // var prefChooseForm = new ConfigureKeyboardActuatorForm {Actuator = this};
            var prefChooseForm = new ConfigureKeyboardActuatorForm { Actuator = this };

            prefChooseForm.ShowDialog();

            prefChooseForm.Dispose();

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
        /// Indicates whether the actuator supports calibration or not
        /// </summary>
        /// <returns>true if it does, false otherwise</returns>
        public override bool SupportsCalibration()
        {
            return false;
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
        /// Looks up the list of switch objects and checks to see
        /// if the key is supported by one of them. Hotkey could
        /// be something like Ctrl+T
        /// </summary>
        /// <param name="hotKey">The key we are interested in</param>
        /// <returns>The switch object for the key</returns>
        private IActuatorSwitch findActuatorSwitch(String hotKey)
        {
            try
            {
                foreach (var obj in Switches)
                {
                    var keySwitch = obj as KeyboardSwitch;
                    if (keySwitch != null)
                    {
                        if (String.Compare(keySwitch.HotKey, hotKey, true) == 0)
                        {
                            return new KeyboardSwitch(keySwitch);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            return null;
        }

        /// <summary>
        /// A keydown event was detected.  Check if this is one of the keys
        /// we are interested in.  The valid keys are defined as the 'source'
        /// attribute in a switch object
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void KeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (actuatorState != State.Running)
            {
                return;
            }

            
            Log.Debug("Keydown: " + e.KeyCode.ToString());

            // check if this is one of the keys we recognize.  If so, trigger
            // a switch-activated event
            var actuatorSwitch = findActuatorSwitch(e.KeyCode.ToString());
            if (actuatorSwitch != null)
            {
                e.Handled = true;
                actuatorSwitch.Action = SwitchAction.Down;
                actuatorSwitch.Confidence = 100;
                OnSwitchActivated(actuatorSwitch);
            }
            else
            {
                KeyStateTracker.KeyDown(e.KeyCode);

                // Get the status of ctrl, shift and alt keys from
                // the tracker.  Then format the final key. For
                // eg:  Ctrl+Alt+T
                string hotKey = KeyStateTracker.GetKeyPressedStatus();
                if (String.IsNullOrEmpty(hotKey))
                {
                    hotKey = e.KeyCode.ToString();
                }
                else
                {
                    hotKey += "+" + e.KeyCode;
                }

                Log.Debug("KeyStateTracker.KeyString: " + hotKey);

                // check which switch handles this hotkey and trigger it
                actuatorSwitch = findActuatorSwitch(hotKey);
                if (actuatorSwitch != null)
                {
                    actuatorSwitch.Action = SwitchAction.Trigger;
                    actuatorSwitch.Confidence = 100;
                    OnSwitchTriggered(actuatorSwitch);
                }
                else
                {
                    if (EvtKeyDown != null)
                    {
                        EvtKeyDown(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// A key up to the corresponding press event was detected.  Notify callers
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void KeyboardHook_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (EvtKeyPress != null)
                {
                    EvtKeyPress(sender, e);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// A key up to the corresponding keydown event was detected.  Notify callers
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void KeyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (actuatorState != State.Running)
            {
                return;
            }

            var s = String.Format("Keyup{0}.  Alt: {1} Ctrl: {2}", e.KeyCode, e.Alt, e.Control);
            Log.Debug(s);

            KeyStateTracker.KeyUp(e.KeyCode);

            var actuatorSwitch = findActuatorSwitch(e.KeyCode.ToString());
            if (actuatorSwitch != null)
            {
                e.Handled = true;
                actuatorSwitch.Action = SwitchAction.Up;
                actuatorSwitch.Confidence = 100;
                OnSwitchDeactivated(actuatorSwitch);
            }
            else
            {
                if (EvtKeyUp != null)
                {
                    EvtKeyUp(this, e);
                }
            }
        }

        /// <summary>
        /// Event handler for a mouse down event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void MouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            if (EvtMouseDown != null)
            {
                EvtMouseDown(sender, e);
            }
        }

        /// <summary>
        /// Subscribe to keyboard events
        /// </summary>
        private void subscribeToHookManager()
        {
            // Trap the keydown and key up events. Note
            // that the HookManager is a systemwide keyboard
            // trapper, not just for this application
            if (actuatorState != State.Running)
            {
                _keyboardHook = new KeyboardHook();
                _keyboardHook.EvtKeyDown += KeyboardHook_KeyDown;
                _keyboardHook.EvtKeyUp += KeyboardHook_KeyUp;
                _keyboardHook.EvtKeyPress += KeyboardHook_KeyPress;
                _keyboardHook.SetHook();

                _mouseHook = new MouseHook();
                _mouseHook.EvtMouseDown += MouseHook_MouseDown;
                _mouseHook.SetHook();
            }
        }

        /// <summary>
        /// Deallocate resources
        /// </summary>
        private void unInit()
        {
            unsubscribeToHookManager();
            actuatorState = State.Stopped;
        }

        /// <summary>
        /// Unsubscribe keyboard events
        /// </summary>
        private void unsubscribeToHookManager()
        {
            if (actuatorState == State.Running)
            {
                if (_keyboardHook != null)
                {
                    _keyboardHook.EvtKeyDown -= KeyboardHook_KeyDown;
                    _keyboardHook.EvtKeyUp -= KeyboardHook_KeyUp;
                    _keyboardHook.EvtKeyPress -= KeyboardHook_KeyPress;
                    _keyboardHook.RemoveHook();
                }

                _mouseHook.EvtMouseDown -= MouseHook_MouseDown;
                _mouseHook.RemoveHook();
            }
        }
    }
}