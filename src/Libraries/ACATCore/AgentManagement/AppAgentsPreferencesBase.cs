////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ACAT.Core.AgentManagement
{
    /// <summary>
    /// Base class for the settings for all the application agents.
    /// </summary>
    [Serializable]
    public partial class AppAgentsPreferencesBase : PreferencesBase
    {
        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        protected AppAgentsPreferencesBase()
        {
            autoSwitchScannerEnable = true;
        }

        /// <summary>
        /// Set to true to track focus changes in the foreground
        /// window and display the appropriate scanner.  If false,
        /// always displays the Alphabet scanner.
        /// </summary>
        [Descriptor("Auto-display contextual menu when the app window gets focus")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        public bool autoSwitchScannerEnable =  true;
    }
}