////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Manages text to speech (TTS) engines.  There are a variety
    /// of methods to convert TTS.  Some are hardware solutions where
    /// the speech conversion is done by external hardware.
    /// and some are software (such as the Microsof Speech Engine).
    /// ACAT supports a variety of TTS engine to be used through the
    /// ITTSEngine interface. The specifics of doing the conversion is up
    /// to the developer of the
    /// engine.
    /// The TTS manager holds a list of engines that it finds and also
    /// tracks the currently active TTS engine
    ///
    /// This class is a singleton.
    /// </summary>
    public class TTSManager : IDisposable
    {
        /// <summary>
        /// The root directory where all the TTS engines are located.
        /// </summary>
        public static String TTSRootDir = "TTSEngines";

        /// <summary>
        /// Upper bound for the volume
        /// </summary>
        private const int _maxNormalizedVolume = 9;

        /// <summary>
        /// Lower bound for the volume level.  Since different engines support
        /// different ranges for volume, ACAT normalizes them and
        /// uses an integer between 0 and 9 for the volume level
        /// </summary>
        private const int _minNormalizedVolume = 0;

        /// <summary>
        /// Singleton instance of the manager
        /// </summary>
        private static readonly TTSManager _instance = new TTSManager();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// List of TTS engines
        /// </summary>
        private TTSEngines _ttsEngines;

        /// <summary>
        /// Initializes the singleton instance of the class
        /// </summary>
        private TTSManager()
        {
            ActiveEngine = TTSEngines.NullTTSEngine;
            Context.EvtCultureChanged += Context_EvtCultureChanged;
        }

        /// <summary>
        /// Indicates that the active TTS engine changed
        /// </summary>
        /// <param name="oldEngine">previous engine</param>
        /// <param name="newEngine">changed to this</param>
        public delegate void EngineChanged(ITTSEngine oldEngine, ITTSEngine newEngine);

        /// <summary>
        /// Raised when the engine changes
        /// </summary>
        public event EngineChanged EvtEngineChanged;

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static TTSManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the currently active TTS engine
        /// </summary>
        public ITTSEngine ActiveEngine { get; private set; }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the collection of discovered TTS Engine Types
        /// </summary>
        public ICollection<Type> GetExtensions()
        {
            return _ttsEngines.Collection;
        }

        /// <summary>
        /// Returns the current normalized volume
        /// </summary>
        /// <returns>volume level</returns>
        public TTSValue GetNormalizedVolume()
        {
            ITTSValue<int> volume = ActiveEngine.GetVolume();

            if (volume.RangeMax == 0)
            {
                return new TTSValue(_minNormalizedVolume, _maxNormalizedVolume, 0);
            }

            float fraction = (float)volume.Value / volume.RangeMax;
            float currentValue = fraction * 9.0f;
            int v = (int)Math.Round(currentValue, 0);
            if (v == 0 && volume.Value > 0)
            {
                v = 1;
            }

            return new TTSValue(_minNormalizedVolume, _maxNormalizedVolume, v);
        }

        /// <summary>
        /// Returns form that displays preferences selection form for text-to-speech engines and allows configuration.
        /// User can enable/disable text-to-speech engines and also configure settings for each text-to-speech engine.
        /// </summary>
        public Form GetPreferencesSelectionForm(IntPtr parentControlHandle)
        {
            if (!ResourceUtils.IsInstalledCulture(CultureInfo.DefaultThreadCurrentUICulture))
            {
                return null;
            }

            var ci = CultureInfo.DefaultThreadCurrentUICulture;

            List<Type> ttsEngineTypeList = new List<Type>();

            // Add all the text-to-speech engines for the selected language
            ttsEngineTypeList.AddRange(_ttsEngines.Get(ci.Name).ToList());

            if (String.Compare(ci.Name, ci.TwoLetterISOLanguageName, true) != 0)
            {
                ttsEngineTypeList.AddRange(_ttsEngines.Get(ci.TwoLetterISOLanguageName).ToList());
            }

            // Get names of text-to-speech engines added thus far
            List<String> ttsEngineTypeNameList = ttsEngineTypeList.Select(type => type.Name).ToList();

            // Get culture neutral text-to-speech engines and only add if engine not already added for specific language
            foreach (Type ttsEngineNeutralCultureType in _ttsEngines.Get(null).ToList())
            {
                if (!ttsEngineTypeNameList.Contains(ttsEngineNeutralCultureType.Name))
                {
                    ttsEngineTypeList.Add(ttsEngineNeutralCultureType);
                }
            }

            // Add NullTTSEngine
            ttsEngineTypeList.Add(typeof(NullTTSEngine));

            // Now create a list of all the text-to-speech engine objects
            List<object> objList = ttsEngineTypeList.Select(type => Activator.CreateInstance(type)).ToList();

            var categories = objList.Select(ttsEngine => new PreferencesCategory(ttsEngine)).ToList();

            var preferredGuid = _ttsEngines.GetPreferredOrDefaultByCulture(ci);
            if (Equals(preferredGuid, Guid.Empty))
            {
                preferredGuid = _ttsEngines.GetPreferredOrDefaultByCulture(null);
            }

            foreach (var category in categories)
            {
                category.Enable = false;
            }

            foreach (var category in categories)
            {
                var iExtension = category.PreferenceObj as IExtension;
                category.Enable = (iExtension != null && iExtension.Descriptor.Id == preferredGuid);
                if (category.Enable)
                {
                    break;
                }
            }

            // Create and return the form for the user to select default text-to-speech engine, change settings etc.
            var form = new PreferencesCategorySelectForm
            {
                PreferencesCategories = categories,
                EnableColumnHeaderText = "Default",
                CategoryColumnHeaderText = "TTS Engine",
                Title = "Text-to-speech - " + ci.DisplayName,
                AllowMultiEnable = false,
                ParentControlHandle = parentControlHandle
            };

            return form;
        }

        /// <summary>
        /// Initializes the TTS manager
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            return true;
        }

        /// <summary>
        /// The extension dirs parameter contains the root directory under
        /// which to search for TTSEngine DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Default, MyCustomACATDir
        /// These are relative to the application execution directory or
        /// to the directory where ACAT has been installed.
        /// It recusrively walks the directories and looks for TTS Engine
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        public bool LoadExtensions(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            if (_ttsEngines == null)
            {
                _ttsEngines = new TTSEngines();

                retVal = _ttsEngines.Load(extensionDirs);
            }

            return retVal;
        }

        /// <summary>
        /// Saves preferences in text-to-speech settings
        /// </summary>
        public void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories)
        {
            var ci = CultureInfo.DefaultThreadCurrentUICulture;

            foreach (var category in preferencesCategories)
            {
                if (category.Enable && category.PreferenceObj is IExtension)
                {
                    _ttsEngines.SetPreferred(ci.Name, ((IExtension)category.PreferenceObj).Descriptor.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the active spellchecker for the specified culture.  If
        /// culture is null, the default culture is used
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>true on success</returns>
        public bool SetActiveEngine(CultureInfo ci = null)
        {
            bool retVal = true;
            Guid guid = Guid.Empty;
            Guid cultureNeutralGuid = Guid.Empty;

            if (ci == null)
            {
                ci = CultureInfo.DefaultThreadCurrentUICulture;
            }

            guid = _ttsEngines.GetPreferredOrDefaultByCulture(ci);
            cultureNeutralGuid = _ttsEngines.GetPreferredOrDefaultByCulture(null);

            if (!Equals(guid, Guid.Empty))  // found something for the specific culture
            {
                var type = _ttsEngines.Lookup(guid);

                if (ActiveEngine != null)
                {
                    ActiveEngine.Dispose();
                    ActiveEngine = null;
                }

                retVal = createAndSetActiveEngine(type, ci);

                if (!retVal)
                {
                    ActiveEngine = TTSEngines.NullTTSEngine;
                    retVal = true; // TODO::
                }
            }
            else
            {
                if (!Equals(cultureNeutralGuid, Guid.Empty))
                {
                    var type = _ttsEngines.Lookup(cultureNeutralGuid);
                    retVal = createAndSetActiveEngine(type, ci);
                }
                else
                {
                    retVal = false;
                }

                if (!retVal)
                {
                    ActiveEngine = TTSEngines.NullTTSEngine;
                    retVal = true; // TODO::
                }
            }

            return retVal;
        }

        /// <summary>
        /// Sets the normalized volume.   Since different engines support
        /// different ranges for volume, ACAT normalizes them and
        /// uses an integer between 0 and 9 for the volume level
        /// </summary>
        /// <param name="normalizedVolume">normalized volume level</param>
        public void SetNormalizedVolume(int normalizedVolume)
        {
            ITTSValue<int> v = ActiveEngine.GetVolume();
            if (v.RangeMax == 0)
            {
                return;
            }

            float fraction = (float)normalizedVolume / 9.0f;
            float currentValue = fraction * v.RangeMax;
            int volSetting = (int)Math.Round(currentValue, 0);

            ActiveEngine.SetVolume(volSetting);
        }

        /// <summary>
        /// Switch language to the specified one.
        /// </summary>
        /// <param name="ci">culture to switch to</param>
        /// <returns>true on success</returns>
        public bool SwitchLanguage(CultureInfo ci)
        {
            if (ActiveEngine != null)
            {
                ActiveEngine.Dispose();
                ActiveEngine = null;
            }

            bool ret = SetActiveEngine(ci);

            return ret;
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                    if (ActiveEngine != null)
                    {
                        ActiveEngine.Dispose();
                    }

                    if (_ttsEngines != null)
                    {
                        _ttsEngines.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Culture changed. Reinitialize TTS Engine
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Context_EvtCultureChanged(object sender, CultureChangedEventArg arg)
        {
            SwitchLanguage(arg.Culture);
        }

        /// <summary>
        /// Creates the TTS Engine for the specified culture.  If it fails,
        /// it set the null TTS Engine as the active one.
        /// </summary>
        /// <param name="type">Type of the TTS Engine</param>
        /// <param name="ci">Culture</param>
        /// <returns>true on success</returns>
        private bool createAndSetActiveEngine(Type type, CultureInfo ci)
        {
            bool retVal;

            try
            {
                var ttsEngine = (ITTSEngine)Activator.CreateInstance(type);
                retVal = ttsEngine.Init(ci);
                if (retVal)
                {
                    saveSettings(ttsEngine);
                    ActiveEngine = ttsEngine;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load TTS Engine" + type + ", assembly: " + type.Assembly.FullName + ". Exception: " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Raises an event indicating that the active engine has changed
        /// </summary>
        /// <param name="oldEngine">previous engine</param>
        /// <param name="newEngine">the newly activated engine</param>
        private void notifyEngineChanged(ITTSEngine oldEngine, ITTSEngine newEngine)
        {
            if (EvtEngineChanged != null)
            {
                EvtEngineChanged(oldEngine, newEngine);
            }
        }

        /// <summary>
        /// Saves settings for the specified tts engine
        /// </summary>
        /// <param name="spellChecker">TTS Engine object</param>
        private void saveSettings(ITTSEngine ttsEngine)
        {
            ttsEngine.Save();
        }
    }
}