////////////////////////////////////////////////////////////////////////////
// <copyright file="AppPanelClassConfig.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents a persistent class that contains a list
    /// of panel configuration entries for ACAT applications,
    /// Each ACAT app can have a set of scanners. For instance,
    /// ACATApp(English) may have the AlphabetScanner Qwerty,
    /// AlphabetScanner (Abc), whereas ACATApp (French) may have
    /// a different alphabet scanner such as AlphabetScanner Azerty
    ///
    /// Hierarchy is
    ///   AppPanelClassConfig
    ///           |
    ///           |
    ///  List of PanelClassConfig (one for each app)
    ///                 |
    ///                 |
    ///         List of PanelClassConfigMap (one for each config in the app)
    ///                          |
    ///                          |
    ///           List of PanelClassConfigMapEntry  (one for each panel in the config)
    /// </summary>
    [Serializable]
    public class AppPanelClassConfig : PreferencesBase
    {
        /// <summary>
        /// List of panelclassconfig's for the different ACAT Apps
        /// </summary>
        public List<PanelClassConfig> PanelClassConfigs = new List<PanelClassConfig>();

        /// <summary>
        /// Adds the specified app info to the PanelClasConfig collection.  If the
        /// entry already exists, replaces it if replaceIfExists is true
        /// </summary>
        /// <param name="appId">the application id</param>
        /// <param name="appName">user friendly app name</param>
        /// <param name="appDescription">a brief description</param>
        /// <param name="replaceIfExists">set to true if existing entry should be replaced</param>
        /// <returns></returns>
        public PanelClassConfig Add(String appId, String appName, String appDescription, bool replaceIfExists = false)
        {
            var panelClassConfig = Find(appId);
            if (panelClassConfig == null)
            {
                panelClassConfig = new PanelClassConfig();
                PanelClassConfigs.Add(panelClassConfig);
            }
            else if (replaceIfExists)
            {
                PanelClassConfigs.Remove(panelClassConfig);
                panelClassConfig = new PanelClassConfig();
                PanelClassConfigs.Add(panelClassConfig);
            }
            else
            {
                return panelClassConfig;
            }

            panelClassConfig.AppId = appId;
            panelClassConfig.AppName = appName;
            panelClassConfig.AppDescription = appDescription;

            return panelClassConfig;
        }

        /// <summary>
        /// Adds the panelClassConfig entry to the collection.   If the
        /// entry already exists, replaces it if replaceIfExists is true
        /// </summary>
        /// <param name="panelClassConfig">the entry to add</param>
        /// <param name="replaceIfExists">set to true if existing entry should be replaced</param>
        public void Add(PanelClassConfig panelClassConfig, bool replaceIfExists = false)
        {
            var existingEntry = Find(panelClassConfig.AppId);
            if (existingEntry == null)
            {
                PanelClassConfigs.Add(panelClassConfig);
            }
            else if (replaceIfExists)
            {
                PanelClassConfigs.Remove(existingEntry);
                PanelClassConfigs.Add(panelClassConfig);
            }
        }

        /// <summary>
        /// Find the panel class config for the specified application Id
        /// </summary>
        /// <param name="appId">application id</param>
        /// <returns>Panelclassconfig entry, null if not found</returns>
        public PanelClassConfig Find(String appId)
        {
            return PanelClassConfigs.FirstOrDefault(panelClassConfig => String.Compare(panelClassConfig.AppId, appId, true) == 0);
        }

        /// <summary>
        /// Saves settings.  No Op.  Doesn't save.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            return false;
        }
    }
}