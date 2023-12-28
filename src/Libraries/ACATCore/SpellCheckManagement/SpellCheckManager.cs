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

namespace ACAT.Lib.Core.SpellCheckManagement
{
    /// <summary>
    /// Manages SpellChecker engines.  The engines are essentially DLLs
    /// located in the SpellChecker folder in one of the extension directories.
    /// All SpellCheck engines derive from the ISpellChecker interface.  This class
    /// looks for these DLL's and maintains a list of available SpellCheckers.  The
    /// app can also set the active SpellChecker.
    ///
    /// This is a singleton instance class.
    /// </summary>
    public class SpellCheckManager : IDisposable
    {
        /// <summary>
        /// Name of the folder under which the Word predictor DLLs are located
        /// </summary>
        public static String SpellCheckersRootName = "SpellCheckers";

        /// <summary>
        /// Word prediction manager instance
        /// </summary>
        private static readonly SpellCheckManager _instance = new SpellCheckManager();

        /// <summary>
        /// The active word predictor
        /// </summary>
        private ISpellChecker _activeSpellChecker;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Contains a list of all word predictors discovered
        /// </summary>
        private SpellCheckers _spellCheckers;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        private SpellCheckManager()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            _activeSpellChecker = SpellCheckers.NullSpellChecker;

            Context.EvtCultureChanged += Context_EvtCultureChanged;
        }

        /// <summary>
        /// Gets the instance of the spell check manager
        /// </summary>
        public static SpellCheckManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the currently active spell checker
        /// </summary>
        public ISpellChecker ActiveSpellChecker
        {
            get { return _activeSpellChecker; }
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Type> GetExtensions()
        {
            return _spellCheckers.Collection;
        }

        /// <summary>
        /// Initialize the SpellCheck manager
        /// </summary>
        /// <param name="extensionDirs">list of directories</param>
        /// <returns></returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            return true;
        }

        /// <summary>
        /// Indicates to the active word predictor that it needs to
        /// load its default settings
        /// </summary>
        public void LoadDefaultSettings()
        {
            loadDefaultSettings(_activeSpellChecker);
        }

        /// <summary>
        /// Initializes the SpellCheck manager by looking for
        /// SpellCheck extension dlls.
        /// The extension dirs parameter contains the root directory under
        /// which to search for SpellCheck DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Default, SomeDir
        /// These are relative to the application execution directory or
        /// to the directory where the ACAT framework has been installed.
        /// It recusrively walks the directories and looks for SpellCheck
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">list of directories</param>
        /// <returns>true on success</returns>
        public bool LoadExtensions(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            if (_spellCheckers == null)
            {
                _spellCheckers = new SpellCheckers();

                retVal = _spellCheckers.Load(extensionDirs);
            }

            return retVal;
        }

        /// <summary>
        /// Indicates to the active spell checker that it needs to save
        /// its settings
        /// </summary>
        public void SaveSettings()
        {
            saveSettings(_activeSpellChecker);
        }

        /// <summary>
        /// Sets the active spellchecker for the specified culture.  If
        /// culture is null, the default culture is used
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>true on success</returns>
        public bool SetActiveSpellChecker(CultureInfo ci = null)
        {
            bool retVal = true;
            Guid guid = Guid.Empty;
            Guid cultureNeutralGuid = Guid.Empty;

            if (ci == null)
            {
                ci = CultureInfo.DefaultThreadCurrentUICulture;
            }

            guid = _spellCheckers.GetPreferredOrDefaultByCulture(ci);
            cultureNeutralGuid = _spellCheckers.GetPreferredOrDefaultByCulture(null);

            if (!Equals(guid, Guid.Empty))  // found something for the specific culture
            {
                var type = _spellCheckers.Lookup(guid);

                if (_activeSpellChecker != null)
                {
                    _activeSpellChecker.Dispose();
                    _activeSpellChecker = null;
                }

                retVal = createAndSetActiveSpellChecker(type, ci);

                if (!retVal)
                {
                    _activeSpellChecker = SpellCheckers.NullSpellChecker;
                    retVal = true; // TODO::
                }
            }
            else
            {
                if (!Equals(cultureNeutralGuid, Guid.Empty))
                {
                    var type = _spellCheckers.Lookup(cultureNeutralGuid);
                    retVal = createAndSetActiveSpellChecker(type, ci);
                }
                else
                {
                    retVal = false;
                }

                if (!retVal)
                {
                    _activeSpellChecker = SpellCheckers.NullSpellChecker;
                    retVal = true; // TODO::
                }
            }

            return retVal;
        }

        /// <summary>
        /// Displays the preferences dialog for spellcheckers.
        /// First displays the dialog that lets the user select the
        /// culture (language) and then displays all the spell checkers
        /// discovered for that culture. The user can select the
        /// preferred spellchecker, change settings etc.
        /// </summary>
        public void ShowPreferences()
        {
            if (!ResourceUtils.IsInstalledCulture(CultureInfo.DefaultThreadCurrentUICulture))
            {
                return;
            }

            var ci = CultureInfo.DefaultThreadCurrentUICulture;

            var spellCheckTypeList = new List<Type>();

            // add all the spellcheckers for the selected language
            spellCheckTypeList.AddRange(_spellCheckers.Get(ci.Name).ToList());

            if (String.Compare(ci.Name, ci.TwoLetterISOLanguageName, true) != 0)
            {
                spellCheckTypeList.AddRange(_spellCheckers.Get(ci.TwoLetterISOLanguageName).ToList());
            }

            spellCheckTypeList.AddRange(_spellCheckers.Get(null).ToList());
            spellCheckTypeList.Add(typeof(NullSpellChecker));

            //Now create a list of all the spellchecker objects
            List<object> objList = spellCheckTypeList.Select(type => Activator.CreateInstance(type)).ToList();

            var categories = objList.Select(spellChecker => new PreferencesCategory(spellChecker)).ToList();

            var preferredGuid = _spellCheckers.GetPreferredOrDefaultByCulture(ci);
            if (Equals(preferredGuid, Guid.Empty))
            {
                preferredGuid = _spellCheckers.GetPreferredOrDefaultByCulture(null);
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

            // display the form for the user to select default spellchecker,
            // change settings etc
            var form1 = new PreferencesCategorySelectForm
            {
                PreferencesCategories = categories,
                EnableColumnHeaderText = "Default",
                CategoryColumnHeaderText = "SpellChecker",
                Title = "Spell Checkers - " + ci.DisplayName,
                AllowMultiEnable = false
            };

            if (form1.ShowDialog() == DialogResult.OK)
            {
                foreach (var category in form1.PreferencesCategories)
                {
                    if (category.Enable && category.PreferenceObj is IExtension)
                    {
                        _spellCheckers.SetPreferred(ci.Name, ((IExtension)category.PreferenceObj).Descriptor.Id);
                    }
                }
            }

            form1.Dispose();
        }

        /// <summary>
        /// Switch language to the specified one.
        /// </summary>
        /// <param name="ci">culture to switch to</param>
        /// <returns>true on success</returns>
        public bool SwitchLanguage(CultureInfo ci)
        {
            if (_activeSpellChecker != null)
            {
                _activeSpellChecker.Dispose();
                _activeSpellChecker = null;
            }

            return SetActiveSpellChecker(ci);
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
                    if (_activeSpellChecker != null)
                    {
                        _activeSpellChecker.Dispose();
                    }

                    if (_spellCheckers != null)
                    {
                        _spellCheckers.Dispose();
                    }

                    Context.EvtCultureChanged -= Context_EvtCultureChanged;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Culture changed. Reinitialize spell checker
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Context_EvtCultureChanged(object sender, CultureChangedEventArg arg)
        {
            SwitchLanguage(arg.Culture);
        }

        /// <summary>
        /// Creates the spellchecker for the specified culture.  If it fails,
        /// it set the null spell checker as the active one.
        /// </summary>
        /// <param name="type">Type of the spellchecker class</param>
        /// <param name="ci">Culture</param>
        /// <returns>true on success</returns>
        private bool createAndSetActiveSpellChecker(Type type, CultureInfo ci)
        {
            bool retVal;

            try
            {
                var spellChecker = (ISpellChecker)Activator.CreateInstance(type);
                retVal = spellChecker.Init(ci);
                if (retVal)
                {
                    saveSettings(spellChecker);
                    _activeSpellChecker = spellChecker;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load spellchecker " + type + ", assembly: " + type.Assembly.FullName + ". Exception: " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// If there are 3rd party DLL's, they may not reside in the app folder.  Redirect
        /// them to the folder where the word predictor DLL resides.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="args">event args</param>
        /// <returns>The assembly</returns>
        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return FileUtils.AssemblyResolve(Assembly.GetExecutingAssembly(), args);
        }

        /// <summary>
        /// Loads default settings for the specified word predictor
        /// </summary>
        /// <param name="spellChecker">spell checker object</param>
        private void loadDefaultSettings(ISpellChecker spellChecker)
        {
            spellChecker.LoadDefaultSettings();
        }

        /// <summary>
        /// Saves settings for the specified wordpredictor
        /// </summary>
        /// <param name="spellChecker">Spell checker object</param>
        private void saveSettings(ISpellChecker spellChecker)
        {
            spellChecker.SaveSettings("dummy");
        }
    }
}