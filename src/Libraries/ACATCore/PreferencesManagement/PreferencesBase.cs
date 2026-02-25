////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesBase.cs
//
// Use this as the base class for any derived class that
// needs to be serialzied or deserialzed to/from an XML
// file.  Contains useful helper functions to instantiate
// a class by deserializing from the xml file.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.DataAccess;
using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace ACAT.Core.PreferencesManagement
{
    /// <summary>
    /// Use this as the base class for any derived class that
    /// needs to be serialzied or deserialzed to/from an XML
    /// file.  Contains useful helper functions to instantiate
    /// a class by deserializing from the xml file.
    /// </summary>
    [Serializable]
    public abstract class PreferencesBase : ObservableValidator, IPreferences, IDisposable
    {
        private static readonly ILogger<PreferencesBase> _logger = LogManager.GetLogger<PreferencesBase>();

        [XmlIgnore]
        public bool IsDirty { get; private set; } = false;

        public PreferencesBase()
        {
            PropertyChanged += PreferencesBase_PropertyChanged;

        }

        private void PreferencesBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsDirty))
            {
                IsDirty = true;
                NotifyPreferencesChanged();
            }
        }

        [NonSerialized, XmlIgnore]
        public static Assembly ApplicationAssembly;

        /// <summary>
        /// Returns a string representation of the settings
        /// </summary>
        public String toString()
        {
            StringBuilder sb = new();
            sb.Append("Preferences: ");
            sb.Append(XmlUtils.XmlSerializeToString(this));
            return sb.ToString();
        }

        /// <summary>
        /// For the event that notifies that preferences changed
        /// </summary>
        public delegate void PreferencesChangedDelegate();

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            ValidateProperty(GetType().GetProperty(e.PropertyName)?.GetValue(this), e.PropertyName);
        }

        /// <summary>
        /// Event that is raised when any of the preferences change.
        /// </summary>
        public event PreferencesChangedDelegate EvtPreferencesChanged;

        /// <summary>
        /// Read preferences from the specified file.  If the file
        /// doesn't exist, it creates a default file with factory
        /// defaults.
        /// </summary>
        /// <param name="preferencesFile">Name of the preferences file</param>
        /// <param name="loadDefaultsOnFail">true: If the file doesn't exist, use defaults, false: return null</param>
        /// <param name="saveAfterLoad">true: Save preferences after loading to ensure file exists</param>
        /// <returns>Preferences read or null</returns>
        public static T Load<T>(String preferencesFile, bool loadDefaultsOnFail = true, bool saveAfterLoad = true) where T : class, new()
        {
            T preferences = default;

            if (String.IsNullOrEmpty(preferencesFile))
            {
                return preferences;
            }

            // Use PreferencesRepository instead of direct XmlUtils calls
            var repo = new PreferencesRepository<T>(_logger);
            preferences = repo.Load(preferencesFile);

            if (preferences == null)
            {
                _logger.LogWarning("Could not load preferences from {PreferencesFile} - creating a new one", preferencesFile);
                if (loadDefaultsOnFail == true)
                {
                    preferences = new T();
                }
            }

            if (preferences != null && saveAfterLoad)
            {
                if (!repo.Save(preferences, preferencesFile))
                {
                    _logger.LogError("Unable to save default preferences");
                    preferences = default;
                }
            }
            return preferences;
        }

        /// <summary>
        /// Creates a new instance of the class (which has the
        /// default settings)
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>created object</returns>
        public static T LoadDefaults<T>() where T : new()
        {
            return new T();
        }

        /// <summary>
        /// Attempts to reload preferences from the specified file. If loading or saving
        /// fails the method returns null and the caller retains the existing instance
        /// (rollback). On success the newly loaded instance is returned.
        /// </summary>
        /// <typeparam name="T">Preferences type.</typeparam>
        /// <param name="preferencesFile">Full path to the preferences file.</param>
        /// <returns>The reloaded preferences, or null if reload failed (caller should keep existing instance).</returns>
        public static T Reload<T>(String preferencesFile) where T : class, new()
        {
            if (String.IsNullOrEmpty(preferencesFile))
            {
                _logger.LogError("Reload failed: preferences file path is null or empty");
                return null;
            }

            var repo = new PreferencesRepository<T>(_logger);
            T reloaded = repo.Load(preferencesFile);

            if (reloaded == null)
            {
                _logger.LogError("Reload failed: could not load preferences from {PreferencesFile}", preferencesFile);
                return null;
            }

            _logger.LogInformation("Preferences reloaded successfully from {PreferencesFile}", preferencesFile);
            return reloaded;
        }

        /// <summary>
        /// Saves preferences to the specificed file
        /// </summary>
        /// <param name="prefs">Preferences</param>
        /// <param name="preferencesFile">full path to the file</param>
        /// <returns>true on success</returns>
        public static bool Save<T>(T prefs, String preferencesFile) where T : class, new()
        {
            // Use PreferencesRepository instead of direct XmlUtils calls
            var repo = new PreferencesRepository<T>(_logger);
            var retVal = repo.Save(prefs, preferencesFile);

            if (retVal == false)
            {
                _logger.LogError("Error saving preferences to file {PreferencesFile}", preferencesFile);
            }

            return retVal;
        }

        /// <summary>
        /// Asynchronously reads preferences from the specified file.
        /// If the file doesn't exist a default instance is returned (when
        /// <paramref name="loadDefaultsOnFail"/> is <c>true</c>).
        /// </summary>
        /// <typeparam name="T">Preferences type.</typeparam>
        /// <param name="preferencesFile">Full path to the preferences file.</param>
        /// <param name="loadDefaultsOnFail">When <c>true</c>, return defaults if the file is absent or unreadable.</param>
        /// <param name="saveAfterLoad">When <c>true</c>, save preferences after loading to ensure the file exists.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The loaded preferences, or <c>null</c> on failure.</returns>
        public static async Task<T> LoadAsync<T>(string preferencesFile, bool loadDefaultsOnFail = true, bool saveAfterLoad = true, CancellationToken cancellationToken = default) where T : class, new()
        {
            T preferences = default;

            if (string.IsNullOrEmpty(preferencesFile))
            {
                return preferences;
            }

            var repo = new PreferencesRepository<T>(_logger);
            preferences = await repo.LoadAsync(preferencesFile, cancellationToken).ConfigureAwait(false);

            if (preferences == null)
            {
                _logger.LogWarning("Could not load preferences from {PreferencesFile} - creating a new one", preferencesFile);
                if (loadDefaultsOnFail)
                {
                    preferences = new T();
                }
            }

            if (preferences != null && saveAfterLoad)
            {
                if (!await repo.SaveAsync(preferences, preferencesFile, cancellationToken).ConfigureAwait(false))
                {
                    _logger.LogError("Unable to save default preferences");
                    preferences = default;
                }
            }

            return preferences;
        }

        /// <summary>
        /// Asynchronously attempts to reload preferences from the specified file.
        /// Returns <c>null</c> on failure so the caller can retain the existing instance.
        /// </summary>
        /// <typeparam name="T">Preferences type.</typeparam>
        /// <param name="preferencesFile">Full path to the preferences file.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The reloaded preferences, or <c>null</c> if reload failed.</returns>
        public static async Task<T> ReloadAsync<T>(string preferencesFile, CancellationToken cancellationToken = default) where T : class, new()
        {
            if (string.IsNullOrEmpty(preferencesFile))
            {
                _logger.LogError("ReloadAsync failed: preferences file path is null or empty");
                return null;
            }

            var repo = new PreferencesRepository<T>(_logger);
            T reloaded = await repo.LoadAsync(preferencesFile, cancellationToken).ConfigureAwait(false);

            if (reloaded == null)
            {
                _logger.LogError("ReloadAsync failed: could not load preferences from {PreferencesFile}", preferencesFile);
                return null;
            }

            _logger.LogInformation("Preferences reloaded successfully from {PreferencesFile}", preferencesFile);
            return reloaded;
        }

        /// <summary>
        /// Asynchronously saves preferences to the specified file.
        /// </summary>
        /// <typeparam name="T">Preferences type.</typeparam>
        /// <param name="prefs">Preferences object to save.</param>
        /// <param name="preferencesFile">Full path to the file.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> on success; <c>false</c> otherwise.</returns>
        public static async Task<bool> SaveAsync<T>(T prefs, string preferencesFile, CancellationToken cancellationToken = default) where T : class, new()
        {
            var repo = new PreferencesRepository<T>(_logger);
            bool retVal = await repo.SaveAsync(prefs, preferencesFile, cancellationToken).ConfigureAwait(false);

            if (!retVal)
            {
                _logger.LogError("Error saving preferences to file {PreferencesFile}", preferencesFile);
            }

            return retVal;
        }

        /// <summary>
        /// Saves default values of the preferences
        /// </summary>
        /// <typeparam name="T">Preferences object</typeparam>
        /// <param name="fileName">name of the file to save to</param>
        public static void SaveDefaults<T>(String fileName) where T : class, new()
        {
            T prefs = new();
            Save(prefs, fileName);
        }

        /// <summary>
        /// Notify subscribers that the settings have changed
        /// </summary>
        public void NotifyPreferencesChanged()
        {
            EvtPreferencesChanged?.Invoke();
        }

        /// <summary>
        /// Saves preferences
        /// </summary>
        /// <returns></returns>
        public abstract bool Save();

        public abstract bool ResetToDefault();

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed) return;

            PropertyChanged -= PreferencesBase_PropertyChanged;
            _disposed = true;

        }
    }
}