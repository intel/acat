////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCISensorStatus.cs
//
// Sensor status sent from the BCIActuator to ACAT
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCISensorStatus
    {
        public BCISensorStatus()
        {
            Error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            StatusSignal = SignalStatus.SIGNAL_KO;
        }

        public BCISensorStatus(BCIError error, SignalStatus statusSignal)
        {
            Error = error;
            StatusSignal = statusSignal;
        }

        /// <summary>
        /// Sensor error
        /// </summary>
        public BCIError Error { get; set; }

        /// <summary>
        /// Status of the signal
        /// </summary>
        public SignalStatus StatusSignal { get; set; }
    }
}