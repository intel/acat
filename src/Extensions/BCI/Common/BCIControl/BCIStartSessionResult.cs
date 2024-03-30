////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIStartSessionResult.cs
//
// Result of start session (sensorReady?, sessionDirectory, errors...) sent by the actuator
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCIStartSessionResult
    {
        public BCIStartSessionResult()
        {
            SensorReady = true;
            Error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            SessionDirectory = "";
            SessionId = "";
        }

        public BCIStartSessionResult(bool sensorReady, string sessionDirectory, string sessionId, BCIError error)
        {
            SensorReady = sensorReady;
            Error = error;
            SessionDirectory = sessionDirectory;
        }

        public BCIError Error { get; set; }

        /// <summary>
        /// True if sensor is ready
        /// </summary>
        public bool SensorReady { get; set; }

        /// <summary>
        /// Directory of the session
        /// </summary>
        public string SessionDirectory { get; set; }

        /// <summary>
        /// ID of the session
        /// </summary>
        public string SessionId { get; set; }
    }
}