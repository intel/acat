////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIMode.cs
//
// Parameters send from ACAT to indicate the current BCI Mode.
// Supported modes: Signal Monitor, Calibration, Tryout, Typing and Unknown
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public enum BCIModes
    {
        SIGNALMONITOR,
        CALIBRATION_EYESOPENCLOSE,
        CALIBRATION,
        TRYOUT,
        TYPING,
        TRIGGERTEST,
        UNKNOWN
    }

    public class BCIMode
    {
        public BCIModes BciMode;

        public BCIScanSections BciCalibrationMode;

        public BCIMode()
        {
            BciMode = BCIModes.UNKNOWN;
            BciCalibrationMode = BCIScanSections.None;
        }

        public BCIMode(BCIModes bciMode, BCIScanSections bciCalibrationMode)
        {
            BciMode = bciMode;
            BciCalibrationMode = bciCalibrationMode;
        }
    }
}