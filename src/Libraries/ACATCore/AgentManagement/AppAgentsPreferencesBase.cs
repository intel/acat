////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using System;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Base class for the settings for all the application agents.
    /// </summary>
    [Serializable]
    public abstract class AppAgentsPreferencesBase : PreferencesBase
    {
        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        protected AppAgentsPreferencesBase()
        {
            AutoSwitchScannerEnable = true;
        }

        /// <summary>
        /// Set to true to track focus changes in the foreground
        /// window and display the appropriate scanner.  If false,
        /// always displays the Alphabet scanner.
        /// </summary>
        [BoolDescriptor("Auto-display contextual menu when the app window gets focus", true)]
        public bool AutoSwitchScannerEnable { get; set; }
    }
}