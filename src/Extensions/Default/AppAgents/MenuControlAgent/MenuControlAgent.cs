////////////////////////////////////////////////////////////////////////////
// <copyright file="MenuControlAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Extension.AppAgents.MenuControlAgent;

namespace ACATCore.Extensions.Base.AppAgents.MenuControl
{
    /// <summary>
    /// Agent to handle menus.  Allows the user to navigate
    /// the menu and make selections.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [DescriptorAttribute("72D527C4-449C-42FE-8FD1-0A176484F690", 
                            "Menu Control Agent",
                            "Manages interactions with application menus")]
    internal class MenuControlAgent : MenuControlAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static MenuControlAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "MenuControlAgentSettings.xml";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public MenuControlAgent()
        {
            MenuControlAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = MenuControlAgentSettings.Load();

            autoSwitchScanners = Settings.AutoSwitchScannerEnable;
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<MenuControlAgentSettings>();
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