////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelClassConfig.cs" company="Intel Corporation">
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents a list of panel configurations for an
    /// ACAT application.
    /// Each ACAT app can have a set of scanners. For instance,
    /// ACATApp(English) may have the AlphabetScanner Qwerty,
    /// AlphabetScanner (Abc), whereas ACATApp (French) may have
    /// a different alphabet scanner such as AlphabetScanner Azerty
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
    public class PanelClassConfig
    {
        /// <summary>
        /// User friendly description of the app
        /// </summary>
        public string AppDescription;

        /// <summary>
        /// A moniker for the app. Can be anything unique
        /// </summary>
        public String AppId;

        /// <summary>
        /// Name of the application.  Eg ACAT Talk Application
        /// </summary>
        public String AppName;

        /// <summary>
        /// List of panels for this app
        /// </summary>
        public List<PanelClassConfigMap> PanelClassConfigMaps = new List<PanelClassConfigMap>();

        /// <summary>
        /// Finds the panel config map entry with the specified config map name
        /// </summary>
        /// <param name="configMapName">name to look for</param>
        /// <returns>panelclassconfigmap object if found null otherwise</returns>

        public PanelClassConfigMap Find(string configMapName)
        {
            return PanelClassConfigMaps.FirstOrDefault(panelClassConfigMap =>
                    String.Compare(panelClassConfigMap.Name, configMapName, true) == 0);
        }

        public PanelClassConfigMap Add(String configMapName, String description, bool isDefault)
        {
            var panelClassConfigMap = Find(configMapName);
            if (panelClassConfigMap == null)
            {
                panelClassConfigMap = new PanelClassConfigMap(configMapName, description, isDefault);
                PanelClassConfigMaps.Add(panelClassConfigMap);
            }
            else
            {
                panelClassConfigMap.Name = configMapName;
                panelClassConfigMap.Description = description;
                panelClassConfigMap.Default = isDefault;
            }

            return panelClassConfigMap;
        }



        /// <summary>
        /// Returns the PanelClassConfigMap object for the ap that is marked as 
        /// default. 
        /// </summary>
        /// <returns>object if found null otherwise</returns>
        public PanelClassConfigMap GetDefaultClassConfigMap()
        {
            return PanelClassConfigMaps.FirstOrDefault(panelClassConfigMap => panelClassConfigMap.Default);
        }

        /// <summary>
        /// Looks through the default panel class config map for the app and within that,
        /// looks for the entry for the specified panelClass (Eg Alphabet or Punctuation)
        /// </summary>
        /// <param name="panelClass">PanelClass to look for</param>
        /// <returns>object if found, null if not</returns>
        public PanelClassConfigMapEntry GetDefaultClassConfigMapEntry(String panelClass)
        {
            foreach (var panelClassConfigMap in PanelClassConfigMaps)
            {
                if (panelClassConfigMap.Default)
                {
                    foreach (var panelClassConfigMapEntry in panelClassConfigMap.PanelClassConfigMapEntries)
                    {
                        if (String.Compare(panelClass, panelClassConfigMapEntry.PanelClass, true) == 0)
                        {
                            return panelClassConfigMapEntry;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the specified configMapName as the default 
        /// configuration for the app
        /// </summary>
        /// <param name="configMapName">name of the config map</param>
        /// <returns>true on success</returns>
        public bool SetDefaultClassConfigMap(String configMapName)
        {
            bool retVal = false;

            var panelClassConfigMap = Find(configMapName);

            if (panelClassConfigMap != null)
            {
                foreach (var configMap in PanelClassConfigMaps)
                {
                    configMap.Default = false;
                }

                panelClassConfigMap.Default = true;

                retVal = true;
            }

            return retVal; ;
        }


    }
}