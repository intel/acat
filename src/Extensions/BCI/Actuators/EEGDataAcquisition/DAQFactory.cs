////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DAQFactory.cs
//
// Factory for creating DAQ (Data Acquisition) implementations
//
////////////////////////////////////////////////////////////////////////////

using System;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    /// <summary>
    /// DAQ device types
    /// </summary>
    public enum DAQDeviceType
    {
        GTecBCI,
        OpenBCI
    }

    /// <summary>
    /// Factory for creating DAQ (Data Acquisition) implementations
    /// </summary>
    public static class DAQFactory
    {
        /// <summary>
        /// Create a DAQ instance based on the device type
        /// </summary>
        /// <param name="deviceType">Type of the device</param>
        /// <returns>Instance of BaseDAQ</returns>
        public static BaseDAQ CreateDAQ(DAQDeviceType deviceType)
        {
            switch (deviceType)
            {
                case DAQDeviceType.OpenBCI:
                    return new DAQ_OpenBCI();
                case DAQDeviceType.GTecBCI:
                    return new DAQ_gTecBCI();
                default:
                    throw new ArgumentException($"Unsupported device type: {deviceType}");
            }
        }

        /// <summary>
        /// Create a DAQ instance based on settings
        /// </summary>
        /// <returns>Instance of BaseDAQ</returns>
        public static BaseDAQ CreateDAQFromSettings()
        {
            // Read from settings to determine which DAQ implementation to use
            var settings = BCIActuatorSettings.Settings;


            if (settings.DAQ_DeviceType == "GTecBCI")
            {
                return new DAQ_gTecBCI();

            }
            else if (settings.DAQ_DeviceType == "OpenBCI")
            {
                return new DAQ_OpenBCI();
            }
            else
            {
                // Default to GTecBCI
                return new DAQ_gTecBCI();
            }
        }
    }
}
