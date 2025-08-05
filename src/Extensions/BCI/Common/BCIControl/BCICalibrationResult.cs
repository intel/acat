////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCICalibrationResult.cs
//
// Parameters sent to ACAT after calibration finalized
//
////////////////////////////////////////////////////////////////////////////

using ControlzEx.Standard;
using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCICalibrationResult
    {
        public BCICalibrationResult()
        {
        }

        /// <summary>
        /// auc score
        /// </summary>
        public float AUC { get; set; } = 0.0f;

        /// <summary>
        /// Boolean, true if calibration succesful
        /// </summary>
        public bool CalibrationSuccessful { get; set; } = false;

        /// <summary>
        /// Error, will send status_ok if no error
        /// </summary>
        public BCIError Error { get; set; } = new BCIError() { ErrorCode = BCIErrorCodes.Status_Ok, ErrorMessage = BCIMessages.Status_Ok };
    }
}