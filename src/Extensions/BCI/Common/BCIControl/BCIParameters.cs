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
        public Dictionary<BCIScanSections, CalibrationParametersForSection> CalibrationParameters { get; set; } = new Dictionary<BCIScanSections, CalibrationParametersForSection>();

        /// <summary>
        /// True if calibration is required. This is controlled by the actuator
        /// </summary>
        public bool CalibrationRequiredFlag { get; set; } = false;

        /// <summary>
        /// AUC score from the last sucessful calibration
        /// </summary>
        public float LastCalibrationAUC { get; set; } = 0.0f;

        /// <summary>
        /// Delay added after decision
        /// </summary>
        public int Scanning_DelayAfterDecision { get; set; } = 0;

        /// <summary>
        /// Pause time (in ms) (read from BCIActuatorSettings.xml)
        /// </summary>
        public int Scanning_PauseTime { get; set; } = 0;

        /// <summary>
        ///  Pause time (in ms) (read from BCIActuatorSettings.xml)
        /// </summary>
        public int Scanning_ShortPauseTime { get; set; } = 0;

        /// <summary>
        /// Duration when decision is shown (in ms) (read from BCIActuatorSettings.xml)
        /// </summary>
        public int Scanning_ShowDecisionTime { get; set; } = 0;

        /// <summary>
        /// Delay before the Get ready 3,2,1 message is displayed before starting typing
        /// </summary>
        public int Scanning_DelayToGetReady { get; set; } = 0;

        /// <summary>
        /// Boolean, true if the focal circle should be filled
        /// </summary>
        public bool Scanning_IsFocalCircleFilled { get; set; } = false;

        /// <summary>
        /// Color of the focal circle
        /// </summary>
        public String Scanning_FocalCircleColor { get; set; } = "#FF0000"; // Red

        /// <summary>
        /// Minimum probabliity value to display probablity bar while typing
        /// </summary>
        public int MinProbablityToDisplayBarOnTyping { get; set; } = 0;

        /// <summary>
        ///Error, will send status_ok if no error
        /// </summary>
        public BCIError Error { get; set; } = new BCIError
        {
            ErrorCode = BCIErrorCodes.Status_Ok,
            ErrorMessage = BCIMessages.Status_Ok
        };
    }
}