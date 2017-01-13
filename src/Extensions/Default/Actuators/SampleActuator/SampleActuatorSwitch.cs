////////////////////////////////////////////////////////////////////////////
// <copyright file="SampleActuatorSwitch.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.Default.Actuators.SampleActuator
{
    /// <summary>
    /// Represents an actuator switch.  An actuator can have
    /// multiple switches.  An example would be a camera sensor
    /// that detects facial gestures. It could detect multiple
    /// gestures such as a mouth twitch, cheek twitch, eyebrow raise,
    /// smile, frown etc and raise events indicating which type
    /// of gesture was detected. Each of these gestures can be
    /// treated as an actuator switch.  This class represents
    /// the swtich object
    ///
    /// On Startup, ACAT creates this object and populates
    /// all the properties of the object from the config files
    /// SwtichConfigMap.xml and Actuators.xml.   Refer to
    /// the ACAT Developers Guide for details on these files.
    /// </summary>
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