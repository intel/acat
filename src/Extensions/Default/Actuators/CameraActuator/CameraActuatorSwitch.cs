////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CameraActuatorSwitch.cs
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
// all the properties of the object from the Actuator
// settings file.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    public class CameraActuatorSwitch : ActuatorSwitchBase
    {
        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public CameraActuatorSwitch()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.  Copies
        /// members over from switchObj
        /// </summary>
        /// <param name="switchObj">Switch object to clone</param>
        public CameraActuatorSwitch(IActuatorSwitch switchObj)
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