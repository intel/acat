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