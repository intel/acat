// Copyright (c) 2013-2017, 2025 Intel Corporation 
// SPDX-License-Identifier: Apache-2.0

using ACAT.Core.AgentManagement;
using ACAT.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;

namespace ACAT.Extensions.AppAgents.WindowsExplorerAgent
{
    /// <summary>
    /// Settings for the Windows Explorer Agent.
    /// </summary>
    [Serializable]
    public class WindowsExplorerAgentSettings : AppAgentsPreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WindowsExplorerAgentSettings()
        {
            AutoSwitchScannerEnable = true;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns>settings object</returns>
        public static WindowsExplorerAgentSettings Load()
        {
            return PreferencesBase.Load<WindowsExplorerAgentSettings>(PreferencesFilePath);
        }

        /// <summary>
        /// Save settings to the preferences file (PreferencesFilePath)
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Save()
        {
            return Save(this, PreferencesFilePath);
        }
    }
}