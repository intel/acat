////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using Microsoft.Extensions.Logging;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Contains global settings for ACAT. These are
    /// separate from the User specific settings.  The settings
    /// file is stored in the same directory as the application.
    /// </summary>
    [Serializable]
    public class GlobalPreferences
    {
        private static ILogger<GlobalPreferences> _logger => LogManager.GetLogger<GlobalPreferences>();
        public static String DefaultPreferencesFilePath = String.Empty;

        public static String LogFileName = String.Empty;

        public static String PreferencesFilePath = String.Empty;

        public String DefaultLogLevel = "Verbose";

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
        /// <returns>SystemPreferences read or null</returns>
        public static GlobalPreferences Load(String prefFile, bool loadDefaultsOnFail = true)
        {
            saveFactoryDefaultSettings();

            GlobalPreferences retVal = XmlUtils.XmlFileLoad<GlobalPreferences>(prefFile);

            if (retVal == null)
            {
                _logger?.LogError("Could not load global preferences from {PrefFile}. Creating a new one.", prefFile);
                if (loadDefaultsOnFail)
                {
                    retVal = new GlobalPreferences();
                }
                else
                {
                    return retVal;
                }
            }

            if (!XmlUtils.XmlFileSave(retVal, prefFile))
            {
                _logger?.LogError("Unable to save global preferences!");
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
            var retVal = XmlUtils.XmlFileSave(prefs, preferencesFile);

            if (retVal == false)
            {
                _logger?.LogError("Error saving preferences! file={PreferencesFile}", preferencesFile);
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