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
    /// Represents a single configuration that is a collection
    /// of panels
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
    public class PanelClassConfigMap
    {
        /// <summary>
        /// Is this the default?
        /// </summary>
        public bool Default;

        /// <summary>
        /// User friendly description
        /// </summary>
        public string Description;

        public string DisplayNameLong;

        public string DisplayNameShort;

        /// <summary>
        /// A name for this configuration
        /// </summary>
        public string Name;

        /// <summary>
        /// A list of panels that are in this configuration
        /// </summary>
        public List<PanelClassConfigMapEntry> PanelClassConfigMapEntries = new();

        public string ScreenshotFileName;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PanelClassConfigMap()
        {
            Name = string.Empty;
            Description = string.Empty;
            Default = false;
            ScreenshotFileName = string.Empty;
            DisplayNameShort = string.Empty;
            DisplayNameLong = string.Empty;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="name">name of the configuration</param>
        /// <param name="description">description</param>
        /// <param name="isDefault">is this the default?</param>
        public PanelClassConfigMap(string name, string description, string displayNameShort = "", string displayNameLong = "", bool isDefault = false)
        {
            Name = name;
            Description = description;
            Default = isDefault;
            DisplayNameShort = displayNameShort;
            DisplayNameLong = displayNameLong;
        }

        /// <summary>
        /// Adds a new entry with the specified panelClass and configId.
        /// </summary>
        /// <param name="panelClass">panelClass of the new entry</param>
        /// <param name="configId">the config ID </param>
        /// <param name="replaceIfExists">set to true if existing entry should be replaced</param>
        /// <returns></returns>
        public PanelClassConfigMapEntry Add(string panelClass, Guid configId, bool replaceIfExists = false)
        {
            var panelClassConfigMapEntry = Find(panelClass);

            if (panelClassConfigMapEntry == null)
            {
                panelClassConfigMapEntry = new PanelClassConfigMapEntry(panelClass, configId);
                PanelClassConfigMapEntries.Add(panelClassConfigMapEntry);
            }
            else if (replaceIfExists)
            {
                panelClassConfigMapEntry.ConfigId = configId;
            }

            return panelClassConfigMapEntry;
        }

        /// <summary>
        /// Adds the specified entry to the collection.
        /// </summary>
        /// <param name="mapEntry">entry to add</param>
        /// <param name="replaceIfExists">set to true if existing entry should be replaced</param>
        public void Add(PanelClassConfigMapEntry mapEntry, bool replaceIfExists)
        {
            var existingEntry = Find(mapEntry.PanelClass);
            if (existingEntry == null)
            {
                PanelClassConfigMapEntries.Add(mapEntry);
            }
            else if (replaceIfExists)
            {
                PanelClassConfigMapEntries.Remove(existingEntry);
                PanelClassConfigMapEntries.Add(mapEntry);
            }
        }

        /// <summary>
        /// Finds the entry for the specified class. Returns null
        /// if not found
        /// </summary>
        /// <param name="panelClass">panelclass to look for</param>
        /// <returns>entry if found</returns>
        public PanelClassConfigMapEntry Find(string panelClass)
        {
            return PanelClassConfigMapEntries.FirstOrDefault(panelClassConfigMapEntry => string.Compare(panelClassConfigMapEntry.PanelClass, panelClass, true) == 0);
        }
    }
}