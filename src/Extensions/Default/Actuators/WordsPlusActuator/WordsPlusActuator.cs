////////////////////////////////////////////////////////////////////////////
// <copyright file="WordsPlusActuator.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Timers;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Extensions.Default.Actuators.WordsPlus
{
    /// <summary>
    /// Talks to the USB interface driver to the Words+
    /// hardware and registers for events triggered when
    /// the switch is engaged/disengaged.
    ///
    ///This class encapsulates the following xml fragment (shown here as an example)
    /// <Actuator name="EZKeys" enabled="true">
    ///   <Switch name="S1" source="1" enabled="true" acceptTime="@AcceptTime"/>
    ///   <!-- switch 1 on the EZ Keys HW-->
    ///   <Switch name="S2" source="2" enabled="true" acceptTime="@AcceptTime"/>
    ///   <!-- switch 2 on the EZ Keys HW -->
    /// </Actuator>
    /// </summary>
    [DescriptorAttribute("6375D1EC-6D70-4376-8B89-E09A1DD1AED4", "WordsPlus Actuator", "Switch actuation through the WordsPlus hardware switch")]
    public class WordsPlusActuator : ActuatorBase
    {
        /// <summary>
        /// The settings object for this actuator
        /// </summary>
        public static Settings WordsPlusActuatorSettings;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Used to detect is plugged in
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// USB device interface object
        /// </summary>
        private USBDevice _usbDevice;

        /// <summary>
        /// Product ID of the words+ HW. Not needed
        /// </summary>
        private readonly String _wordsPlusPID = String.Empty;

        /// <summary>
        /// Vendor ID of the Words+ hardware
        /// </summary>
        private const String WordsPlusVid = "vid_05f3";

        private const String SettingsFileName = "WordsPlusActuatorSettings.xml";

        /// <summary>
        /// Constructor
        /// </summary>
        public WordsPlusActuator()
            : base()
        {
            WordsPlusActuatorSettings = new Settings();
        }

        /// <summary>
        /// Class factory to create a EZKeys switch object
        /// </summary>
        /// <returns></returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new WordsPlusSwitch();
        }

        /// <summary>
        /// Initialize resources, open usb driver, register for
        /// USB events
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public override bool Init()
        {
            Settings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            WordsPlusActuatorSettings = Settings.Load();

            // register for USB events "PID_0303";
            _usbDevice = new USBDevice(WordsPlusVid, _wordsPlusPID);
            _usbDevice.EvtReadDataNotify += _usbDevice_EvtReadDataNotify;
            _usbDevice.EvtDeviceDisconnected += _usbDevice_EvtDeviceDisconnected;

            // timer is used, in case the USB device is
            // not connected.  Let's poll periodically to
            // see if it is plugged in
            _timer = new Timer {Interval = WordsPlusActuatorSettings.WordsPlusKeyCheckInterval};
            _timer.Elapsed += _timer_Elapsed;

            // open usb device and begin reading data
            if (!openAndBeginRead())
            {
                _timer.Start();
            }

            return true;
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
        /// Check to see if the device is plugged in.  Otherwise,
        /// just check back periodically
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!openAndBeginRead())
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Triggered when the device is unplugged
        /// </summary>
        private void _usbDevice_EvtDeviceDisconnected()
        {
            Log.Debug("Words+ Key has been unplugged");

            _usbDevice.Close();
            actuatorState = State.Stopped;

            // start the timer to periodically check
            _timer.Start();
        }

        /// <summary>
        /// Event triggered when the USB device receives data
        /// from the Words+ hardware.
        /// The data is a string of bytes. The HW has two switches
        /// - S1 and S2.  Here's how it's decoded:
        /// If byte[1] == 0, switch S1 is disengaged
        /// if byte[1] == 1, switch S1 is engaged
        /// if byte[2] == 0, switch S2 is disengaged
        /// if byte[2] == 1, switch S2 is engaged
        /// </summary>
        /// <param name="rawData">USB data</param>
        private void _usbDevice_EvtReadDataNotify(byte[] rawData)
        {
            Log.Debug();

            if (actuatorState != State.Running)
            {
                Log.Debug("EZKEys actuator is not running. returning");
                return;
            }

            var sb = new StringBuilder();

            // go through the data and check each index position
            // to see if it is set to 1 or 0
            for (int index = 0; index < rawData.Length; index++)
            {
                sb.Append(rawData[index].ToString() + " ");
                var switchObj = find(index);
                if (switchObj != null)
                {
                    // trigger an event showing the switch status
                    if (rawData[index] == 1)
                    {
                        OnSwitchActivated(switchObj);
                    }
                    else if (rawData[index] == 0)
                    {
                        OnSwitchDeactivated(switchObj);
                    }
                }
            }

            Log.Debug("Received Words+ data: " + sb.ToString());
        }

        /// <summary>
        /// Find the switch that deals with the input detected
        /// </summary>
        /// <param name="switchSource">The source name of the switch</param>
        /// <returns>Switch object, null if not found</returns>
        private IActuatorSwitch find(int switchSource)
        {
            String str = switchSource.ToString();
            foreach (IActuatorSwitch switchObj in Switches)
            {
                if (switchObj is WordsPlusSwitch)
                {
                    var wordsPlusSwitch = (WordsPlusSwitch)switchObj;
                    if (wordsPlusSwitch.Source == str)
                    {
                        return new WordsPlusSwitch(switchObj);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Opens the USB driver and start reading data from it.  Reading
        /// is asynchronous and events are triggered whenever data is received
        /// </summary>
        /// <returns></returns>
        private bool openAndBeginRead()
        {
            bool retVal = _usbDevice.Open();
            if (retVal)
            {
                Log.Debug("Detected Words+ key.  Opened successfully!");
                actuatorState = State.Running;
                _usbDevice.BeginReadAsync();
            }

            return retVal;
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
            _timer.Stop();
            _timer.Dispose();
            _usbDevice.Close();
            actuatorState = State.Stopped;
        }
    }
}