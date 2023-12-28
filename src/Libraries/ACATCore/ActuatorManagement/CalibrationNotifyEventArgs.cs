////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CalibrationNotifyEventArgs.cs
//
// Argument for calibration event notifications
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Argument for calibration event notifications
    /// </summary>
    public class CalibrationNotifyEventArgs
    {
        public CalibrationNotifyEventArgs(bool pauseScanner = true, bool hideScanner = false)
        {
            HideScanner = hideScanner;
            PauseScanner = pauseScanner;
        }

        public bool HideScanner { get; set; }

        public bool PauseScanner { get; set; }
    }
}
