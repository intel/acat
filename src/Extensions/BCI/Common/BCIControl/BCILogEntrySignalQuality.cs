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
        /// <summary>
        /// Names of the channels
        /// </summary>
        public String[] ChannelNames { get; set; } = new String[0];

        /// <summary>
        /// Array of booleans, true if a channel is enabled
        /// </summary>
        public bool[] EnabledChannels { get; set; } = new bool[0];

        /// <summary>
        /// Impedance
        /// </summary>
        public int[] ImpedanceValues { get; set; } = new int[0];

        /// <summary>
        /// Railing values
        /// </summary>
        public int[] RailingValues { get; set; } = new int[0];

        /// <summary>
        /// Whether or not signal check exited at time of log entry
        /// </summary>
        public bool PassedSignalCheck { get; set; } = false;
    }
}