////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardSwitch.cs
//
// Represents a keyboard switch object. The user can drive the
// UI using the keyboard.  Each keyboard switch object encapsulates
// a short cut or hotkey such as Ctrl-T. When this key combination
//  is detected, the switch event is raised.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// Represents a keyboard switch object. The user can drive the
    /// UI using the keyboard.  Each keyboard switch object encapsulates
    /// a short cut or hotkey such as Ctrl-T. When this key combination
    ///  is detected, the switch event is raised.
    /// </summary>
    internal class KeyboardSwitch : ActuatorSwitchBase
    {
        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes the keyboardswitch object
        /// </summary>
        public KeyboardSwitch()
        {
        }

        /// <summary>
        /// Initialize the keyboard actuator object
        /// </summary>
        /// <param name="switchObj"></param>
        public KeyboardSwitch(IActuatorSwitch switchObj)
            : base(switchObj)
        {
        }

        /// <summary>
        /// The keyboard hotkey this switch represents (e.g F5).  This
        /// is the 'source' attribute of a keyboard switch in the xml file
        /// </summary>
        public String HotKey { get; set; }

        /// <summary>
        /// Perform initialization
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            HotKey = Source;

            return true;
        }

        public override bool Load(SwitchSetting switchSetting)
        {
            base.Load(switchSetting);

            return true;
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
        /// De-allocate resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
        }
    }
}