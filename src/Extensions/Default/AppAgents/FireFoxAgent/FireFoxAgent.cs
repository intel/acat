////////////////////////////////////////////////////////////////////////////
// <copyright file="FirefoxAgent.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension.AppAgents.Firefox;

namespace ACAT.Extensions.Default.AppAgents.FireFox
{
    /// <summary>
    /// Application agent for the Firefox browser.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [DescriptorAttribute("F59BDA2F-A0A7-4AFE-B546-7EF3B6F8A1D0",
                            "Firefox Agent",
                            "Manages interactions with the Firefox Browser")]
    internal class FireFoxAgent : FireFoxAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static FireFoxAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "FireFoxAgentSettings.xml";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public FireFoxAgent()
        {
            FireFoxAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = FireFoxAgentSettings.Load();

            autoSwitchScanners = Settings.AutoSwitchScannerEnable;
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<FireFoxAgentSettings>();
        }

        /// <summary>
        /// Returns the settings for this agent
        /// </summary>
        /// <returns>The settings object</returns>
        public override IPreferences GetPreferences()
        {
            return Settings;
        }
    }
}