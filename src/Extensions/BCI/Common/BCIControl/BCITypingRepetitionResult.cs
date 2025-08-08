////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCITypingRepetitionResult.cs
//
// Parameters sent from BCIActutor to ACAT after data from a typing repetition
// has been processed. This includes the decision, probabilities, etc
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    /// <summary>
    /// Signal status. Only 2 status statuses are used by ACAT: SIGNAL_OK and SIGNAL_KO
    /// The rest are internal to the BCI Actuator
    /// </summary>
    public enum SignalStatus
    {
        SIGNAL_ERROR,
        SIGNAL_OK,
        SIGNAL_ACCEPTABLE,
        SIGNAL_KO,
    };

    [Serializable]
    public class BCITypingRepetitionResult
    {
        public BCITypingRepetitionResult()
        {
            PosteriorProbs = new SortedDictionary<int, double>();
            DecidedFlag = false;
            DecidedId = 0;
            Error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            ReturnToBoxScanningFlag = false;
            StatusSignal = SignalStatus.SIGNAL_OK;
        }

        /// <summary>
        /// Boolean, true if a decision has been made
        /// </summary>
        public bool DecidedFlag { get; set; }

        /// <summary>
        /// ID of the selected button label (if a decision has been made)
        /// If no decision has been made, this will be the ID of the button
        /// with maximum probability
        /// </summary>
        public int DecidedId { get; set; }

        /// <summary>
        /// Probabilities of each button  / box
        /// Format: [ID, probability]
        /// </summary>
        public SortedDictionary<int, double> PosteriorProbs { get; set; }

        /// <summary>
        /// Boolean, true if eyes closed detected and should return to box scanning
        /// </summary>
        public bool ReturnToBoxScanningFlag { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        public BCIError Error { get; set; }

        /// <summary>
        /// Signal status
        /// </summary>
        public SignalStatus StatusSignal { get; set; }
    }
}