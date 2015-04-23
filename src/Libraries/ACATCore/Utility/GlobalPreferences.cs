////////////////////////////////////////////////////////////////////////////
// <copyright file="GlobalPreferences.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics.CodeAnalysis;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Contains global settings for ACAT. These are
    /// separate from the User specific settings
    /// </summary>
    [Serializable]
    public class GlobalPreferences
    {
        /// <summary>
        /// Factory default settings file
        /// </summary>
        [NonSerialized]
        public static String DefaultsFileName = "DefaultSettings.xml";

        /// <summary>
        /// Current global settings file
        /// </summary>
        [NonSerialized]
        public static String FileName = "Settings.xml";

        /// <summary>
        /// Path where this file is stored
        /// </summary>
        [NonSerialized]
        public static String SettingsDefaultPath = FileUtils.GetPreferencesFileFullPath(DefaultsFileName);

        /// <summary>
        /// Default profile for the user
        /// </summary>
        public String CurrentProfile = "Default";

        /// <summary>
        /// Default user name
        /// </summary>
        public String CurrentUser = "Default";

        /// <summary>
        /// Read preferences from the specified file.  If the file
        /// doesn't exist, it creates a default file with factory
        /// defaults.
        /// </summary>
        /// <param name="prefFile">Name of the preferences file</param>
        /// <param name="loadDefaultsOnFail">true: If the file doesn't exist, use defaults, false: return null</param>
        /// <returns>Preferences read or null</returns>
        public static GlobalPreferences Load(String prefFile, bool loadDefaultsOnFail = true)
        {
            GlobalPreferences retVal;

            saveFactoryDefaultSettings();

            retVal = XmlUtils.XmlFileLoad<GlobalPreferences>(prefFile);

            if (retVal == null)
            {
                Log.Debug("Could not load global preferences from " + prefFile + ". Creating a new one");
                if (loadDefaultsOnFail)
                {
                    retVal = new GlobalPreferences();
                }
                else
                {
                    return retVal;
                }
            }

            if (!XmlUtils.XmlFileSave<GlobalPreferences>(retVal, prefFile))
            {
                Log.Error("Unable to save global preferences!");
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Saves preferenes to the specified file
        /// </summary>
        /// <param name="prefs">preferences to save</param>
        /// <param name="preferencesFile">full path to the file</param>
        /// <returns>true on success</returns>
        public static bool Save(GlobalPreferences prefs, String preferencesFile)
        {
            // save current settings into current file and preset file
            var retVal = XmlUtils.XmlFileSave<GlobalPreferences>(prefs, preferencesFile);

            if (retVal == false)
            {
                Log.Error("Error saving preferences! file=" + preferencesFile);
            }

            return retVal;
        }

        /// <summary>
        /// Save factory default settings
        /// </summary>
        private static void saveFactoryDefaultSettings()
        {
            Save(new GlobalPreferences(), SettingsDefaultPath);
        }
    }
}