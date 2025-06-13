using ACAT.Lib.Core.PreferencesManagement;
using System;
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
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.BCI.Common
{
    public abstract class BCISensorSettings<TSettings>
        where TSettings : PreferencesBase, new()
    {
        public String SettingsFileName { get; set; }

        public TSettings Settings { get; set; }

        public BCISettingsFixed SettingsFixed { get;  } = new BCISettingsFixed();

        public abstract void Load();

        public void Save()
        {
            Settings?.Save();
        }
    }
}