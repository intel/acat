////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIActuatorSettings.cs
//
// Handles load/save of settings for BCI Actuator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.UserManagement;
using System;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    public static class BCIActuatorSettings
    {
        public static BCISettings Settings;
        public const String SettingsFileName = "BCIActuatorSettings.xml";
        public static void Load()
        {
            BCISettings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);

            Settings = BCISettings.Load();
        }

        public static void Save()
        {
            if (Settings != null)
            {
                Settings.Save();
            }
        }
    }
}