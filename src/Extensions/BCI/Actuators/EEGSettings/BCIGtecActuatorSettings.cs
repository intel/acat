////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIGtecActuatorSettings.cs
//
// Handles load/save of settings for BCI Actuator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.UserManagement;
using System;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    public static class BCIGtecActuatorSettings
    {
        public static BCIGtecSettings Settings;
        public const String SettingsFileName = "BCIGtecActuatorSettings.xml";
        public static void Load()
        {
            BCIGtecSettings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);

            Settings = BCIGtecSettings.Load();
        }

        public static void Save()
        {
            Settings?.Save();
        }
    }
}