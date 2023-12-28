////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SampleActuatorSwitch.cs
//
// Represents an actuator switch.  An actuator can have
// multiple switches.  An example would be a camera sensor
// that detects facial gestures. It could detect multiple
// gestures such as a mouth twitch, cheek twitch, eyebrow raise,
// smile, frown etc and raise events indicating which type
// of gesture was detected. Each of these gestures can be
// treated as an actuator switch.  This class represents
// the swtich object
//
// On Startup, ACAT creates this object and populates
// all the properties of the object from the config files
// SwtichConfigMap.xml and Actuators.xml.   Refer to
// the ACAT Developers Guide for details on these files.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.Default.Actuators.SampleActuator
{
    public class SampleActuatorSwitch : ActuatorSwitchBase
    {
        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public SampleActuatorSwitch()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.  Copies
        /// members over from switchObj
        /// </summary>
        /// <param name="switchObj">Switch object to clone</param>
        public SampleActuatorSwitch(IActuatorSwitch switchObj)
            : base(switchObj)
        {
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
        /// Release resources
        /// </summary>
        private void unInit()
        {
        }
    }
}