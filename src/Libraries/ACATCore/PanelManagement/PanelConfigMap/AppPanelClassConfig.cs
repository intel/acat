////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

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

        [XmlIgnore]
        public String FileName { get; set; }

        public static AppPanelClassConfig Load(String fileName)
        {
            var retVal = AppPanelClassConfig.Load<AppPanelClassConfig>(fileName);

            retVal.FileName = fileName;

            return retVal;
        }

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
            return AppPanelClassConfig.Save<AppPanelClassConfig>(this, FileName);
        }
    }
}