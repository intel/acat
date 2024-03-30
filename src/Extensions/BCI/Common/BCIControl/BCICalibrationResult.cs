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

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCICalibrationResult
    {
        public BCICalibrationResult()
        {
            AUC = 0.0f;
            Error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            CalibrationSuccessful = false;
        }

        public BCICalibrationResult(float auc, bool calibrationSuccesful, BCIError error)
        {
            AUC = auc;
            Error = error;
            CalibrationSuccessful = calibrationSuccesful;
        }

        /// <summary>
        /// auc score
        /// </summary>
        public float AUC { get; set; }

        /// <summary>
        /// Boolean, true if calibration succesful
        /// </summary>
        public bool CalibrationSuccessful { get; set; }

        /// <summary>
        /// Error, will send status_ok if no error
        /// </summary>
        public BCIError Error { get; set; }
    }
}