////////////////////////////////////////////////////////////////////////////
// <copyright file="FoxitReaderAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Extension.AppAgents.FoxitReader;

namespace ACAT.Extensions.Default.AppAgents.FoxitReader
{
    /// <summary>
    /// This is the application agent for Foxit PDF reader.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [DescriptorAttribute("3EA64BB0-6247-4E9C-9EAD-924738F4E7D2",
                            "Foxit Reader Agent",
                            "Manages interactions with the Foxit PDF Reader application")]
    internal class FoxitReaderAgent : FoxitReaderAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static FoxitReaderAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "FoxitReaderAgentSettings.xml";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public FoxitReaderAgent()
        {
            FoxitReaderAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = FoxitReaderAgentSettings.Load();

            autoSwitchScanners = Settings.AutoSwitchScannerEnable;
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<FoxitReaderAgentSettings>();
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