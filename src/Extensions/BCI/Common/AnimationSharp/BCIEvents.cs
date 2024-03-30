////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIEvents.cs
//
/// Contains events from BCI
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.AnimationSharp
{
    public  class BCIEvents
    {
        /// <summary>
        /// Event triggerd when calibration finishes
        /// </summary>
        public delegate void BCICalibrationComplete(string message);

        /// <summary>
        /// Event triggered when is requested to exit the App
        /// </summary>
        public delegate void BCIExitApplication();

        /// <summary>
        /// Event triggerd when the calibration sesion finished
        /// </summary>
        public delegate void BCIParametersResult();

        /// <summary>
        /// Event triggerd when is requested to resume Watchdog
        /// </summary>
        /// 
        public delegate void BCIResumeWatchdog();

        /// <summary>
        /// Event triggerd when is requested to start a calibration sesion
        /// </summary>
        public delegate void BCIStartCalibration();

        /// <summary>
        /// Event triggerd when is required to update the text in the TextBox Control
        /// </summary>
        public delegate void BCIUpdateTextBox(string stringMessage);

    }
}
