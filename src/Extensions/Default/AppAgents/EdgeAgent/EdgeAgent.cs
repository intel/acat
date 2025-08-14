////////////////////////////////////////////////////////////////////////////
// <copyright file="InternetExplorerAgent.cs" company="Intel Corporation">
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

using ACAT.Core.PreferencesManagement;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Extension.AppAgents.InternetExplorer;

namespace ACAT.Extensions.Default.AppAgents.EdgeAgent
{
    /// <summary>
    /// Application agent for the Internet Explorer.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [ClassDescriptor("0B183771-C3E7-4ED2-9886-741526343140",
                        "Edge Agent",
                        "Manages interactions with Edge")]
    internal class InternetExplorerAgent : InternetExplorerAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static EdgeAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "EdgeAgentSettings.xml";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public InternetExplorerAgent()
        {
            EdgeAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = EdgeAgentSettings.Load();

            autoSwitchScanners = Settings.AutoSwitchScannerEnable;
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<EdgeAgentSettings>();
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