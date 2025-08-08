////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCICalibrationEnd.cs
//
// Parameters sent from ACAT when calibration ends
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCICalibrationEyesClosedIterationEnd
    {
        public enum BCIEyesClosedModes
        {
            EyesClosed,
            EyesOpened,
            Unknown,
        }

        public BCIEyesClosedModes BciEyesClosedMode;

        public BCICalibrationEyesClosedIterationEnd()
        {
            BciEyesClosedMode = BCIEyesClosedModes.Unknown;
        }

        public BCICalibrationEyesClosedIterationEnd(BCIEyesClosedModes bciEyesClosedMode)
        {
            BciEyesClosedMode = bciEyesClosedMode;
        }
    }
}