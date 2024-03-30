////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIParameters.cs
//
// Parameters (read from BCIActuatorSettings) sent from the BCIActuator to ACAT
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCIParameters
    {
        public BCIParameters(Dictionary<BCIScanSections, CalibrationParametersForSection> calibrationParameters, bool calibrationRequiredFlag, float lastCalibrationAUC,
                    int scanning_PauseTime, int scanning_ShortPauseTime, int scanning_showDecisionTime, int scanning_delayAfterDecision, int scanning_delayToGetReady, int minProbablityToDisplayBar,
                    string focalCircleColor, bool isFocalCircleFilled,
                    BCIError error)
        {
            CalibrationParameters = calibrationParameters;
            CalibrationRequiredFlag = calibrationRequiredFlag;
            MinProbablityToDisplayBarOnTyping = minProbablityToDisplayBar;
            LastCalibrationAUC = lastCalibrationAUC;
            Scanning_PauseTime = scanning_PauseTime;
            Scanning_ShortPauseTime = scanning_ShortPauseTime;
            Scanning_ShowDecisionTime = scanning_showDecisionTime;
            Scanning_DelayAfterDecision = scanning_delayAfterDecision;
            Scanning_DelayToGetReady = scanning_delayToGetReady;
            Scanning_FocalCircleColor = focalCircleColor;
            Scanning_IsFocalCircleFilled = isFocalCircleFilled;
            Error = error;
        }

        public Dictionary<BCIScanSections, CalibrationParametersForSection> CalibrationParameters;

        /// <summary>
        /// True if calibration is required. This is controlled by the actuator
        /// </summary>
        public bool CalibrationRequiredFlag { get; set; }

        /// <summary>
        /// AUC score from the last sucessful calibration
        /// </summary>
        public float LastCalibrationAUC { get; set; }

        /// <summary>
        /// Delay added after decision
        /// </summary>
        public int Scanning_DelayAfterDecision { get; set; }

        /// <summary>
        /// Pause time (in ms) (read from BCIActuatorSettings.xml)
        /// </summary>
        public int Scanning_PauseTime { get; set; }

        /// <summary>
        ///  Pause time (in ms) (read from BCIActuatorSettings.xml)
        /// </summary>
        public int Scanning_ShortPauseTime { get; set; }

        /// <summary>
        /// Duration when decision is shown (in ms) (read from BCIActuatorSettings.xml)
        /// </summary>
        public int Scanning_ShowDecisionTime { get; set; }

        /// <summary>
        /// Delay before the Get ready 3,2,1 message is displayed before starting typing
        /// </summary>
        public int Scanning_DelayToGetReady { get; set; }

        /// <summary>
        /// Boolean, true if the focal circle should be filled
        /// </summary>
        public bool Scanning_IsFocalCircleFilled { get; set; }

        /// <summary>
        /// Color of the focal circle
        /// </summary>
        public String Scanning_FocalCircleColor { get; set; }

        /// <summary>
        /// Minimum probabliity value to display probablity bar while typing
        /// </summary>
        public int MinProbablityToDisplayBarOnTyping { get; set; }

        /// <summary>
        ///Error, will send status_ok if no error
        /// </summary>
        public BCIError Error { get; set; }
    }
}