////////////////////////////////////////////////////////////////////////////
// <copyright file="DialogControlAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Extension.AppAgents.DialogControlAgent;

namespace ACAT.Extensions.Default.AppAgents.DialogControlAgent
{
    /// <summary>
    /// This is the application agent to handle application dialogs.
    /// For instance, the "Find" dialog in Notepad, or confirmation
    /// dialog when the user quits Word without saving the file.
    /// Allows the user to navigate the dialog, activate buttons etc.
    /// Base class does all the heavy-lifting.  Override functions
    /// as required customize
    /// </summary>
    [DescriptorAttribute("71175780-0766-4491-AD23-22F0EEF87988",
                            "Dialog Control Agent",
                            "Manages interactions with application dialogs")]
    internal class DialogControlAgent : DialogControlAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static DialogControlAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "DialogControlAgentSettings.xml";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public DialogControlAgent()
        {
            DialogControlAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = DialogControlAgentSettings.Load();

            autoSwitchScanners = Settings.AutoSwitchScannerEnable;
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<DialogControlAgentSettings>();
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