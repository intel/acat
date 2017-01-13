////////////////////////////////////////////////////////////////////////////
// <copyright file="AcrobatReaderAgentSettings.cs" company="Intel Corporation">
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

namespace ACAT.Extensions.Default.AppAgents.AcrobatReaderAgent
{
    /// <summary>
    /// Settings for the Adobe Acrobat Reader Agent.
    /// </summary>
    [Serializable]
    public class AcrobatReaderAgentSettings : AppAgentsPreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AcrobatReaderAgentSettings()
        {
            AutoSwitchScannerEnable = true;
        }

        /// <summary>
        /// Load settings from the settings file
        /// </summary>
        /// <returns>settings object</returns>
        public static AcrobatReaderAgentSettings Load()
        {
            return PreferencesBase.Load<AcrobatReaderAgentSettings>(PreferencesFilePath);
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
