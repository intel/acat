////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaPlayerAgentSettings.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.AppAgents.MediaPlayer
{
    /// <summary>
    /// Settings for the Windows Media Player Agent
    /// </summary>
    [Serializable]
    public class MediaPlayerAgentSettings : AppAgentsPreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Set to true to hide the menu if the media player
        /// window is maximized
        /// </summary>
        public bool HideContextualMenuOnPlayerWindowMaximized;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MediaPlayerAgentSettings()
        {
            AutoSwitchScannerEnable = true;
            HideContextualMenuOnPlayerWindowMaximized = true;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns>settings object</returns>
        public static MediaPlayerAgentSettings Load()
        {
            return PreferencesBase.Load<MediaPlayerAgentSettings>(PreferencesFilePath);
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