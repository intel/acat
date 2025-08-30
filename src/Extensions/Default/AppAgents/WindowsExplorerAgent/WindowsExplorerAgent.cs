// Copyright (c) 2013-2017, 2025 Intel Corporation 
// SPDX-License-Identifier: Apache-2.0

using ACAT.Core.PreferencesManagement;
using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Extension.AppAgents.WindowsExplorer;

namespace ACAT.Extensions.AppAgents.WindowsExplorerAgent
{
    /// <summary>
    /// This is the application agent for Windows Explorer.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [ClassDescriptor("27A45570-FC5A-4FD7-8B07-63A3EF391A9C",
                            "Windows Explorer Agent",
                            "Manages interactions with Windows Explorer")]
    internal class WindowsExplorerAgent : WindowsExplorerAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static WindowsExplorerAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "WindowsExplorerAgentSettings.xml";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public WindowsExplorerAgent()
        {
            WindowsExplorerAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = WindowsExplorerAgentSettings.Load();

            //autoSwitchScanners = Settings.AutoSwitchScannerEnable;
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<WindowsExplorerAgentSettings>();
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