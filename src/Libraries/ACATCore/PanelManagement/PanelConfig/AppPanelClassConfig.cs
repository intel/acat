////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Core.PanelManagement.PanelConfig
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
        /// List of AppPanelClassConfig's for the different ACAT Apps
        /// </summary>
        public List<PanelClassConfig> PanelClassConfigs = new();

        [XmlIgnore]
        public string FileName { get; set; }

        public static AppPanelClassConfig Load(string fileName)
        {
            AppPanelClassConfig retVal = Load<AppPanelClassConfig>(fileName, false, false);

            retVal.FileName = fileName;

            return retVal;
        }

        /// <summary>
        /// Adds the specified app info to the PanelClassConfig collection.  If the
        /// entry already exists, replaces it if replaceIfExists is true
        /// </summary>
        /// <param name="appId">the application id</param>
        /// <param name="appName">user friendly app name</param>
        /// <param name="appDescription">a brief description</param>
        /// <param name="replaceIfExists">set to true if existing entry should be replaced</param>
        /// <returns></returns>
        public PanelClassConfig Add(string appId, string appName, string appDescription, bool replaceIfExists = false)
        {
            PanelClassConfig panelClassConfig = Find(appId);
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
            PanelClassConfig existingEntry = Find(panelClassConfig.AppId);
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
        /// <returns>PanelClassConfig entry, null if not found</returns>
        public PanelClassConfig Find(string appId)
        {
            return PanelClassConfigs.FirstOrDefault(panelClassConfig => string.Compare(panelClassConfig.AppId, appId, true) == 0);
        }

        public override bool ResetToDefault()
        {
            PanelClassConfig tmp = LoadDefaults<PanelClassConfig>();
            var res = Save(tmp, FileName);
            Load(FileName);

            return res;
        }

        /// <summary>
        /// Saves settings.  No Op.  Doesn't save.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            return Save(this, FileName);
        }
    }
}