////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferencesBase.cs" company="Intel Corporation">
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
using System.Text;

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
    /// Contains system-wide preference settings.  Settings are serialized
    /// into a file for saving and loaded from the file.
    /// Be careful about renaming variables in this as the variable names
    /// are used as the setting names in the saved XML file.
    /// </summary>
    [Serializable]
    public abstract class PreferencesBase
    {
        public delegate void PreferencesChangedDelegate();

        public event PreferencesChangedDelegate EvtPreferencesChanged;

        /// <summary>
        /// Read preferences from the specified file.  If the file
        /// doesn't exist, it creates a default file with factory
        /// defaults.
        /// </summary>
        /// <param name="preferencesFile">Name of the preferences file</param>
        /// <param name="loadDefaultsOnFail">true: If the file doesn't exist, use defaults, false: return null</param>
        /// <returns>Preferences read or null</returns>
        public static T Load<T>(String preferencesFile, bool loadDefaultsOnFail = true) where T : new()
        {
            T preferences = default(T);

            if (String.IsNullOrEmpty(preferencesFile))
            {
                return preferences;
            }

            preferences = XmlUtils.XmlFileLoad<T>(preferencesFile);

            if (preferences == null)
            {
                Log.Debug("Could not load preferences from " + preferencesFile + ". Creating a new one");
                if (loadDefaultsOnFail == true)
                {
                    preferences = new T();
                }
            }

            if (preferences != null)
            {
                if (!XmlUtils.XmlFileSave<T>(preferences, preferencesFile))
                {
                    Log.Error("Unable to save default preferences!");
                    preferences = default(T);
                }
            }
            return preferences;
        }

        public static T LoadDefaults<T>() where T : new()
        {
            return new T();
        }

        /// <summary>
        /// Saves preferences to the specificed file
        /// </summary>
        /// <param name="prefs">Preferences</param>
        /// <param name="preferencesFile">full path to the file</param>
        /// <returns></returns>
        public static bool Save<T>(T prefs, String preferencesFile)
        {
            bool retVal;

            // save current settings into current file and preset file

            Preferences x = prefs as Preferences;

            retVal = XmlUtils.XmlFileSave<T>(prefs, preferencesFile);

            if (retVal == false)
            {
                Log.Error("Error saving preferences! file=" + preferencesFile);
            }

            return retVal;
        }

        public static void SaveDefaults<T>(String fileName) where T : new()
        {
            T prefs = new T();
            Save(prefs, fileName);
        }

        /// <summary>
        /// Notify subscribers that the settings have changed
        /// </summary>
        public void NotifyPreferencesChanged()
        {
            if (EvtPreferencesChanged != null)
            {
                EvtPreferencesChanged();
            }
        }

        public abstract bool Save();

        /// <summary>
        /// Returns a string representation of the settings
        /// </summary>
        public virtual String toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Preferences: ");
            sb.Append(XmlUtils.XmlSerializeToString<PreferencesBase>(this));
            return sb.ToString();
        }
    }
}