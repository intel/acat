////////////////////////////////////////////////////////////////////////////
// <copyright file="GlobalPreferences.cs" company="Intel Corporation">
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
using System.Xml.Serialization;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Contains global settings for ACAT. These are
    /// separate from the User specific settings.  The settings
    /// file is stored in the same directory as the application.
    /// </summary>
    [Serializable]
    public class GlobalPreferences
    {
        [NonSerialized, XmlIgnore]
        public static String DefaultPreferencesFilePath = String.Empty;

        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath = String.Empty;
        /// <summary>
        /// Default profile for the user
        /// </summary>
        public String CurrentProfile = "Default";

        /// <summary>
        /// Default user name
        /// </summary>
        public String CurrentUser = "DefaultUser";

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
            saveFactoryDefaultSettings();

            var retVal = XmlUtils.XmlFileLoad<GlobalPreferences>(prefFile);

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
        /// Loads the settings from the preferences path
        /// </summary>
        /// <param name="loadDefaultsOnFail">set to true to load default settings on error</param>
        /// <returns></returns>
        public static GlobalPreferences Load(bool loadDefaultsOnFail = true)
        {
            return !String.IsNullOrEmpty(PreferencesFilePath) ?
                    Load(PreferencesFilePath, loadDefaultsOnFail) :
                    LoadDefaultSettings();
        }

        /// <summary>
        /// Loads default factory settings
        /// </summary>
        /// <returns>Factory default settings</returns>
        public static GlobalPreferences LoadDefaultSettings()
        {
            return loadDefaults<GlobalPreferences>();
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
        /// Saves the settings to the preferences file
        /// </summary>
        /// <returns>true on success</returns>
        public bool Save()
        {
            return !String.IsNullOrEmpty(PreferencesFilePath) && Save(this, PreferencesFilePath);
        }

        /// <summary>
        /// Creates a new instance of the class (which has the
        /// default settings)
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>created object</returns>
        private static T loadDefaults<T>() where T : new()
        {
            return new T();
        }

        /// <summary>
        /// Save factory default settings
        /// </summary>
        private static void saveFactoryDefaultSettings()
        {
            Save(new GlobalPreferences(), DefaultPreferencesFilePath);
        }
    }
}