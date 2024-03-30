////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// BCIMenuOptions.cs
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public class BCIMenuOptions
    {
        public enum Options
        {
            Box,
            Sentence,
            KeyboardL,
            Word,
            KeyboardR,
            EyesCalibration,
            TriggerTest,
            RemapCalibrations,
            SignalCheck,
            Exit,
            Typing,
            None,
        }

        public enum MainMenuOptions
        {
            ExitApplication,
            CalibrateOrShowCalibrationModes,
            TypingOrRecalibrate,
        }
    }
}