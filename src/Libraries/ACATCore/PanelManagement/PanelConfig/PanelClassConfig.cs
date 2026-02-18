////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Core.PanelManagement.PanelConfig
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
        public string AppId;

        /// <summary>
        /// Name of the application.  Eg ACAT Talk Application
        /// </summary>
        public string AppName;

        /// <summary>
        /// List of panels for this app
        /// </summary>
        public List<PanelClassConfigMap> PanelClassConfigMaps = new();

        /// <summary>
        /// Finds the panel config map entry with the specified config map name
        /// </summary>
        /// <param name="configMapName">name to look for</param>
        /// <returns>panelclassconfigmap object if found null otherwise</returns>

        public PanelClassConfigMap Add(string configMapName, string description, bool isDefault)
        {
            PanelClassConfigMap panelClassConfigMap = Find(configMapName);
            if (panelClassConfigMap == null)
            {
                panelClassConfigMap = new PanelClassConfigMap(configMapName, description, string.Empty, string.Empty, isDefault);
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

        public PanelClassConfigMap Find(string configMapName)
        {
            return PanelClassConfigMaps.FirstOrDefault(panelClassConfigMap =>
                    string.Compare(panelClassConfigMap.Name, configMapName, true) == 0);
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
        public PanelClassConfigMapEntry GetDefaultClassConfigMapEntry(string panelClass)
        {
            foreach (PanelClassConfigMap panelClassConfigMap in PanelClassConfigMaps)
            {
                if (panelClassConfigMap.Default)
                {
                    foreach (PanelClassConfigMapEntry panelClassConfigMapEntry in panelClassConfigMap.PanelClassConfigMapEntries)
                    {
                        if (string.Compare(panelClass, panelClassConfigMapEntry.PanelClass, true) == 0)
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
        public bool SetDefaultClassConfigMap(string configMapName)
        {
            bool retVal = false;

            PanelClassConfigMap panelClassConfigMap = Find(configMapName);

            if (panelClassConfigMap != null)
            {
                foreach (PanelClassConfigMap configMap in PanelClassConfigMaps)
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