////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILogEntrySignalQuality.cs
//
// Auditlog entry of signal quality:
// channel names, enabled channels, impedance values and railing values
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCILogEntrySignalQuality

    {
        public BCILogEntrySignalQuality(string[] channelNames, bool[] enabledChannels, int[] railingValues, int[] impedanceValues, bool passedSignalCheck)
        {
            ChannelNames = channelNames;
            EnabledChannels = enabledChannels;
            ImpedanceValues = impedanceValues;
            RailingValues = railingValues;
            PassedSignalCheck = passedSignalCheck;
        }

        /// <summary>
        /// Names of the channels
        /// </summary>
        public String[] ChannelNames { get; set; }

        /// <summary>
        /// Array of booleans, true if a channel is enabled
        /// </summary>
        public bool[] EnabledChannels { get; set; }

        /// <summary>
        /// Impedance
        /// </summary>
        public int[] ImpedanceValues { get; set; }

        /// <summary>
        /// Railing values
        /// </summary>
        public int[] RailingValues { get; set; }

        /// <summary>
        /// Whether or not signal check exited at time of log entry
        /// </summary>
        public bool PassedSignalCheck { get; set; }
    }
}