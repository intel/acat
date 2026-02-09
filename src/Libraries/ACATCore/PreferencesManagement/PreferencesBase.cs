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

using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
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
        private static readonly ILogger<PreferencesBase> _logger = LoggingConfiguration.CreateLogger<PreferencesBase>();

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
        /// <returns>Preferences read or null</returns>
        public static T Load<T>(String preferencesFile, bool loadDefaultsOnFail = true, bool saveAfterLoad = true) where T : new()
        {
            T preferences = default;

            if (String.IsNullOrEmpty(preferencesFile))
            {
                return preferences;
            }

            preferences = XmlUtils.XmlFileLoad<T>(preferencesFile);

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
                if (!XmlUtils.XmlFileSave(preferences, preferencesFile))
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
        /// Saves preferences to the specificed file
        /// </summary>
        /// <param name="prefs">Preferences</param>
        /// <param name="preferencesFile">full path to the file</param>
        /// <returns></returns>
        public static bool Save<T>(T prefs, String preferencesFile)
        {
            // save current settings into current file and preset file
            var retVal = XmlUtils.XmlFileSave(prefs, preferencesFile);

            if (retVal == false)
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
        public static void SaveDefaults<T>(String fileName) where T : new()
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