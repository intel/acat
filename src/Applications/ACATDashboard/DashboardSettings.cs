////////////////////////////////////////////////////////////////////////////
// <copyright file="DashboardSettings.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Applications.ACATDashboard
{
    /// <summary>
    /// Holds the settings for the ACAT Dashboard.  This
    /// includes a list of applications that the user would
    /// like to launch
    /// </summary>
    [Serializable]
    public class DashboardSettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// List of applications including path, cmd line etc
        /// </summary>
        public AppInfo[] Applications;

        /// <summary>
        /// Hide form when minimized;
        /// </summary>
        public bool HideWhenMinimzied;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DashboardSettings()
        {
            HideWhenMinimzied = true;
            Applications = new[] { new AppInfo() };
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns>settings object</returns>
        public static DashboardSettings Load()
        {
            return PreferencesBase.Load<DashboardSettings>(PreferencesFilePath);
        }

        /// <summary>
        /// Save settings.  No op for now
        /// </summary>
        /// <returns>true always</returns>
        public override bool Save()
        {
            return PreferencesBase.Save(this, PreferencesFilePath);
        }
    }
}