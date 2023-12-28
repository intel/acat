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
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Core.WordPredictionManagement
{
    /// <summary>
    /// Manages word prediction engines.  The engines are  DLLs
    /// located in the WordPredictors folder in one of the extension directories.
    /// All engines derive from the IWordPredictor interface.  This class looks
    /// for these DLL's and maintains a list of available word predictors.  The
    /// app sets the preferred word predcitor to use.
    /// This is a singleton instance class
    /// </summary>
    public class WordPredictionManager : IDisposable
    {
        /// <summary>
        /// Name of the folder under which the Word predictor DLLs are located
        /// </summary>
        public static String WordPredictorsRootName = "WordPredictors";

        /// <summary>
        /// Word prediction manager instance
        /// </summary>
        private static readonly WordPredictionManager _instance = new WordPredictionManager();

        /// <summary>
        /// Current User's current profile directory
        /// </summary>
        private readonly String _profileRootDir;

        /// <summary>
        /// Word predictor root directory relative to the user's home directory
        /// </summary>
        private readonly String _userWordPredictorRootDir;

        /// <summary>
        /// The active word predictor
        /// </summary>
        private IWordPredictor _activeWordPredictor;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Contains a list of all word predictors discovered
        /// </summary>
        private WordPredictors _wordPredictors;

        /// <summary>
        /// Initializes and instance of the WordPredictionManager class.
        /// </summary>
        private WordPredictionManager()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            Context.EvtCultureChanged += Context_EvtCultureChanged;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            _activeWordPredictor = WordPredictors.NullWordPredictor;

            _userWordPredictorRootDir = UserManagement.UserManager.GetFullPath(WordPredictorsRootName);
            _profileRootDir = UserManagement.ProfileManager.GetFullPath(WordPredictorsRootName);
        }

        /// <summary>
        /// Gets the instance of the Word prediction manager
        /// </summary>
        public static WordPredictionManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the currently active word predictor
        /// </summary>
        public IWordPredictor ActiveWordPredictor
        {
            get { return _activeWordPredictor; }
        }

        /// <summary>
        /// Gets the collection of discovered word predictors
        /// </summary>
        public ICollection<Type> WordPredictorExtensions
        {
            get { return _wordPredictors.Collection; }
        }

        /// <summary>
        /// Gets the word predictor root directory relative the user's
        /// current profile
        /// </summary>
        public String WordPredictorRootDirRelativeToProfile
        {
            get { return _profileRootDir; }
        }

        /// <summary>
        /// Gets the word predictor root directory relative to the user
        /// home directory
        /// </summary>
        public String WordPredictorRootDirRelativeToUser
        {
            get { return _userWordPredictorRootDir; }
        }

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
        /// Returns form that displays preferences selection form for word predictors and allows configuration.
        /// User can enable/disable word predictors and also configure settings for each word predictor.
        /// </summary>
        public Form GetPreferencesSelectionForm(IntPtr parentControlHandle)
        {
            if (!ResourceUtils.IsInstalledCulture(CultureInfo.DefaultThreadCurrentUICulture))
            {
                return null;
            }

            var ci = CultureInfo.DefaultThreadCurrentUICulture;

            List<Type> wpTypeList = new List<Type>();

            // Add all the word predictors for the selected language
            wpTypeList.AddRange(_wordPredictors.Get(ci.Name).ToList());

            if (String.Compare(ci.Name, ci.TwoLetterISOLanguageName, true) != 0)
            {
                wpTypeList.AddRange(_wordPredictors.Get(ci.TwoLetterISOLanguageName).ToList());
            }

            // Get names of word predictor types added thus far
            List<String> wpTypeNameList = wpTypeList.Select(type => type.Name).ToList();

            // Get culture neutral word predictor types and only add if type not already added for specific language
            foreach (Type wpNeutralCultureType in _wordPredictors.Get(null).ToList())
            {
                if (!wpTypeNameList.Contains(wpNeutralCultureType.Name))
                {
                    wpTypeList.Add(wpNeutralCultureType);
                }
            }

            // Add NullWordPredictor
            wpTypeList.Add(typeof(NullWordPredictor));

            // Now create a list of all the word predictor objects
            List<object> objList = wpTypeList.Select(type => Activator.CreateInstance(type)).ToList();

            var categories = objList.Select(wordPredictor => new PreferencesCategory(wordPredictor)).ToList();

            var preferredGuid = _wordPredictors.GetPreferredOrDefaultByCulture(ci);
            if (Equals(preferredGuid, Guid.Empty))
            {
                preferredGuid = _wordPredictors.GetPreferredOrDefaultByCulture(null);
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

            /// Create and return the form for the user to select default word predictor, change settings etc.
            var form = new PreferencesCategorySelectForm
            {
                PreferencesCategories = categories,
                Title = "Word Predictors - " + ci.DisplayName,
                EnableColumnHeaderText = "Default",
                CategoryColumnHeaderText = "Word Predictor",
                AllowMultiEnable = false,
                ParentControlHandle = parentControlHandle
            };

            return form;
        }

        /// <summary>
        /// Initialize the Word Predictor manager
        /// </summary>
        /// <param name="extensionDirs"></param>
        /// <returns></returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            return true;
        }

        public bool PostInit()
        {
            if (_activeWordPredictor != null)
            {
                return _activeWordPredictor.PostInit();
            }

            return true;
        }

        /// <summary>
        /// Indicates to the active word predictor that it needs to
        /// load its default settings
        /// </summary>
        public void LoadDefaultSettings()
        {
            loadDefaultSettings(_activeWordPredictor);
        }

        /// <summary>
        /// Loads all the word prediction extensions.
        /// The extension dirs parameter contains the root directory under
        /// which to search for Word Predictor DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Base, Hawking
        /// These are relative to the application execution directory or
        /// to the directory where the ACAT framework has been installed.
        /// It recusrively walks the directories and looks for Word Predictor
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">root directory</param>
        /// <returns>true on success</returns>
        public bool LoadExtensions(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            if (_wordPredictors == null)
            {
                _wordPredictors = new WordPredictors();

                retVal = _wordPredictors.Load(extensionDirs);
            }

            return retVal;
        }

        /// <summary>
        /// Saves preferences in word predictor settings
        /// </summary>
        public void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories)
        {
            var ci = CultureInfo.DefaultThreadCurrentUICulture;

            foreach (var category in preferencesCategories)
            {
                if (category.Enable && category.PreferenceObj is IExtension)
                {
                    _wordPredictors.SetPreferred(ci.Name, ((IExtension)category.PreferenceObj).Descriptor.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Indicates to the active word predictor that it needs to save
        /// its settings
        /// </summary>
        public void SaveSettings()
        {
            saveSettings(_activeWordPredictor);
        }

        /// <summary>
        /// Sets the active word predictor for the specified culture. If
        /// culture is null, the default culture is used
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>true on success</returns>
        public bool SetActiveWordPredictor(CultureInfo ci = null)
        {
            if (ci == null)
            {
                ci = CultureInfo.DefaultThreadCurrentUICulture;
            }

            Guid guid = _wordPredictors.GetPreferredOrDefaultByCulture(ci);
            Guid cultureNeutralGuid = _wordPredictors.GetPreferredOrDefaultByCulture(null);
            bool retVal;
            if (!Equals(guid, Guid.Empty))  // found something for the specific culture
            {
                var type = _wordPredictors.Lookup(guid);

                if (_activeWordPredictor != null)
                {
                    _activeWordPredictor.Dispose();
                    _activeWordPredictor = null;
                }

                retVal = createAndSetActiveWordPredictor(type, ci);

                if (!retVal)
                {
                    _activeWordPredictor = WordPredictors.NullWordPredictor;
                }
            }
            else
            {
                if (!Equals(cultureNeutralGuid, Guid.Empty))
                {
                    var type = _wordPredictors.Lookup(cultureNeutralGuid);
                    retVal = createAndSetActiveWordPredictor(type, ci);

                    if (!retVal)
                    {
                        ci = new CultureInfo("en-US");
                        guid = _wordPredictors.GetDefaultByCulture(ci);
                        if (!Equals(guid, Guid.Empty))
                        {
                            type = _wordPredictors.Lookup(guid);
                            retVal = createAndSetActiveWordPredictor(type, ci);
                        }
                    }
                }
                else
                {
                    retVal = false;
                }

                if (!retVal)
                {
                    _activeWordPredictor = WordPredictors.NullWordPredictor;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Switch language to the specified one.
        /// </summary>
        /// <param name="ci">culture to switch to</param>
        /// <returns>true on success</returns>
        public bool SwitchLanguage(CultureInfo ci)
        {
            if (_activeWordPredictor != null)
            {
                _activeWordPredictor.Dispose();
                _activeWordPredictor = null;
            }

            return SetActiveWordPredictor(ci);
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

                Context.EvtCultureChanged -= Context_EvtCultureChanged;

                if (disposing)
                {
                    // dispose all managed resources.
                    if (_activeWordPredictor != null)
                    {
                        _activeWordPredictor.Dispose();
                    }

                    if (_wordPredictors != null)
                    {
                        _wordPredictors.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Event handler for when the culture changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void Context_EvtCultureChanged(object sender, CultureChangedEventArg arg)
        {
            SwitchLanguage(arg.Culture);
        }

        /// <summary>
        /// Creates the word predictor using reflection on the
        /// specified type and makes it the active one.  If it fails,
        /// it set the null word predictor as the active one.
        /// </summary>
        /// <param name="type">Type of the word predictor class</param>
        /// <param name="ci">Culture</param>
        /// <returns>true</returns>
        private bool createAndSetActiveWordPredictor(Type type, CultureInfo ci)
        {
            bool retVal;

            try
            {
                var wordPredictor = (IWordPredictor)Activator.CreateInstance(type);
                retVal = wordPredictor.Init(ci);
                if (retVal)
                {
                    _activeWordPredictor = wordPredictor;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load WordPredictor " + type + ", assembly: " + type.Assembly.FullName + ". Exception: " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// If there are 3rd party DLL's, they may not reside in the app folder.  Redirect
        /// them to the folder where the word predictor DLL resides.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return FileUtils.AssemblyResolve(Assembly.GetExecutingAssembly(), args);
        }

        /// <summary>
        /// Loads default settings for the specified word predictor
        /// </summary>
        /// <param name="wordPredictor"></param>
        private void loadDefaultSettings(IWordPredictor wordPredictor)
        {
            wordPredictor.LoadDefaultSettings();
        }

        /// <summary>
        /// Saves settings for the specified wordpredictor
        /// </summary>
        /// <param name="wordPredictor"></param>
        private void saveSettings(IWordPredictor wordPredictor)
        {
            wordPredictor.SaveSettings("dummy");
        }
    }
}