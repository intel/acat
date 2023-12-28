////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents a mapping between a panel class and its
    /// panel configid.  The config id points to a combination
    /// of the scanner form and the panel xml config file (the one
    /// that contains animations)
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
    public class PanelClassConfigMapEntry
    {
        /// <summary>
        /// The config id for this panel class
        /// </summary>
        public Guid ConfigId;

        /// <summary>
        /// The panel class (eg Alphabet, Punctuations etc)
        /// </summary>
        public String PanelClass;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PanelClassConfigMapEntry()
        {
            PanelClass = String.Empty;
            ConfigId = Guid.Empty;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="panelClass">Panel class</param>
        /// <param name="configId">the config id it maps to</param>
        public PanelClassConfigMapEntry(String panelClass, Guid configId)
        {
            PanelClass = panelClass;
            ConfigId = configId;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="panelClass">panel class</param>
        /// <param name="configId">String rep of the config id guid</param>
        public PanelClassConfigMapEntry(String panelClass, String configId)
        {
            Guid guid;

            if (Guid.TryParse(configId, out guid))
            {
                PanelClass = panelClass;
                ConfigId = guid;
            }
        }
    }
}