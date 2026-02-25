////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;
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

            // Use PreferencesRepository instead of direct XmlUtils calls
            var repo = new PreferencesRepository<GlobalPreferences>(_logger);
            GlobalPreferences retVal = repo.Load(prefFile);

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

            // Save to ensure file exists with current settings
            if (!repo.Save(retVal, prefFile))
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
            // Use PreferencesRepository instead of direct XmlUtils calls
            var repo = new PreferencesRepository<GlobalPreferences>(_logger);
            var retVal = repo.Save(prefs, preferencesFile);

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
        /// Asynchronously reads global preferences from <paramref name="prefFile"/>.
        /// Creates a default file when the file is absent (when
        /// <paramref name="loadDefaultsOnFail"/> is <c>true</c>).
        /// </summary>
        /// <param name="prefFile">Full path to the preferences file.</param>
        /// <param name="loadDefaultsOnFail">When <c>true</c>, return defaults if the file is absent.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The loaded preferences, or <c>null</c> on failure.</returns>
        public static async Task<GlobalPreferences> LoadAsync(string prefFile, bool loadDefaultsOnFail = true, CancellationToken cancellationToken = default)
        {
            saveFactoryDefaultSettings();

            var repo = new PreferencesRepository<GlobalPreferences>(_logger);
            GlobalPreferences retVal = await repo.LoadAsync(prefFile, cancellationToken).ConfigureAwait(false);

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

            if (!await repo.SaveAsync(retVal, prefFile, cancellationToken).ConfigureAwait(false))
            {
                _logger?.LogError("Unable to save global preferences!");
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Asynchronously reads global preferences from <see cref="PreferencesFilePath"/>.
        /// </summary>
        /// <param name="loadDefaultsOnFail">When <c>true</c>, return defaults if the file is absent.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The loaded preferences, or <c>null</c> on failure.</returns>
        public static Task<GlobalPreferences> LoadAsync(bool loadDefaultsOnFail = true, CancellationToken cancellationToken = default)
        {
            return !String.IsNullOrEmpty(PreferencesFilePath)
                ? LoadAsync(PreferencesFilePath, loadDefaultsOnFail, cancellationToken)
                : Task.FromResult(LoadDefaultSettings());
        }

        /// <summary>
        /// Asynchronously saves global preferences to the specified file.
        /// </summary>
        /// <param name="prefs">Preferences to save.</param>
        /// <param name="preferencesFile">Full path to the file.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> on success; <c>false</c> otherwise.</returns>
        public static async Task<bool> SaveAsync(GlobalPreferences prefs, string preferencesFile, CancellationToken cancellationToken = default)
        {
            var repo = new PreferencesRepository<GlobalPreferences>(_logger);
            bool retVal = await repo.SaveAsync(prefs, preferencesFile, cancellationToken).ConfigureAwait(false);

            if (!retVal)
            {
                _logger?.LogError("Error saving preferences! file={PreferencesFile}", preferencesFile);
            }

            return retVal;
        }

        /// <summary>
        /// Asynchronously saves these settings to <see cref="PreferencesFilePath"/>.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> on success; <c>false</c> otherwise.</returns>
        public Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return !String.IsNullOrEmpty(PreferencesFilePath)
                ? SaveAsync(this, PreferencesFilePath, cancellationToken)
                : Task.FromResult(false);
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