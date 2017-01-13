////////////////////////////////////////////////////////////////////////////
// <copyright file="AppAgentsPreferencesBase.cs" company="Intel Corporation">
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