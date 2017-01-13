////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelClassConfigMap.cs" company="Intel Corporation">
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
        public String Description;

        /// <summary>
        /// A name for this configuration
        /// </summary>
        public String Name;

        /// <summary>
        /// A list of panels that are in this configuration
        /// </summary>
        public List<PanelClassConfigMapEntry> PanelClassConfigMapEntries = new List<PanelClassConfigMapEntry>();

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PanelClassConfigMap()
        {
            Name = String.Empty;
            Description = String.Empty;
            Default = false;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="name">name of the configuration</param>
        /// <param name="description">description</param>
        /// <param name="isDefault">is this the default?</param>
        public PanelClassConfigMap(String name, String description, bool isDefault = false)
        {
            Name = name;
            Description = description;
            Default = isDefault;
        }

        /// <summary>
        /// Adds a new entry with the specified panelClass and configId.
        /// </summary>
        /// <param name="panelClass">panelClass of the new entry</param>
        /// <param name="configId">the config ID </param>
        /// <param name="replaceIfExists">set to true if existing entry should be replaced</param>
        /// <returns></returns>
        public PanelClassConfigMapEntry Add(String panelClass, Guid configId, bool replaceIfExists = false)
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
        public PanelClassConfigMapEntry Find(String panelClass)
        {
            return PanelClassConfigMapEntries.FirstOrDefault(panelClassConfigMapEntry => String.Compare(panelClassConfigMapEntry.PanelClass, panelClass, true) == 0);
        }
    }
}