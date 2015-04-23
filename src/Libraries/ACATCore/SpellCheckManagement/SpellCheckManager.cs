////////////////////////////////////////////////////////////////////////////
// <copyright file="SpellCheckManager.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Core.SpellCheckManagement
{
    /// <summary>
    /// Manages word prediction engines.  The engines are essentially DLLs
    /// located in the WordPredictors folder in one of the extension directories.
    /// All engines derive from the IWordPredictor interface.  This class looks
    /// for these DLL's and maintains a list of available word predictors.  The
    /// app can also set the active word predictor.
    /// This is a singleton instance class
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
        /// Null word predictor. Doesn't do anything :-)
        /// </summary>
        private readonly ISpellChecker _nullSpellChecker;

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

            _nullSpellChecker = new NullSpellChecker();
            _nullSpellChecker.Init();
            _activeSpellChecker = _nullSpellChecker;

            UserManagement.ProfileManager.GetFullPath(SpellCheckersRootName);
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
        /// Gets the collection of discovered spell checkers
        /// </summary>
        public ICollection<Type> SpellCheckers
        {
            get { return _spellCheckers.Collection; }
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

        /// <summary>
        /// Initialize the Word Predictor manager by looking for
        /// Word predictor dlls.
        /// The extension dirs parameter contains the root directory under
        /// which to search for Word Predictor DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Base, Hawking
        /// These are relative to the application execution directory or
        /// to the directory where the ACAT framework has been installed.
        /// It recusrively walks the directories and looks for Word Predictor
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">list of directories</param>
        /// <returns></returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            if (_spellCheckers == null)
            {
                _spellCheckers = new SpellCheckers();

                // add the null word predictor to our list of
                // recognizied word predictors
                var descriptor = _nullSpellChecker.Descriptor;
                if (descriptor != null)
                {
                    _spellCheckers.add(descriptor.Id, typeof(NullSpellChecker));
                }

                // walk through the directory to discover
                retVal = _spellCheckers.Load(extensionDirs);
            }

            return retVal;
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
        /// Indicates to the active word predictor that it needs to save
        /// its settings
        /// </summary>
        public void SaveSettings()
        {
            saveSettings(_activeSpellChecker);
        }

        /// <summary>
        /// Sets the spell checker represented by id
        /// as a Guid or name.
        /// </summary>
        /// <param name="idOrName">GUID or name of the spell checker</param>
        /// <returns>true on success</returns>
        public bool SetActiveSpellChecker(String idOrName)
        {
            bool retVal;
            var guid = Guid.Empty;

            if (!Guid.TryParse(idOrName, out guid))
            {
                guid = _spellCheckers.GetByName(idOrName);
            }

            var type = Guid.Equals(idOrName, Guid.Empty) ?
                            typeof(NullSpellChecker) :
                            _spellCheckers.Lookup(guid);

            if (type != null)
            {
                if (_activeSpellChecker != null)
                {
                    _activeSpellChecker.Dispose();
                    _activeSpellChecker = null;
                }

                retVal = createAndSetActiveSpellChecker(type);
            }
            else
            {
                createAndSetActiveSpellChecker(typeof(NullSpellChecker));
                retVal = false;
            }

            return retVal;
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

                    if (_nullSpellChecker != null)
                    {
                        _nullSpellChecker.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Creates the spell checker using reflection on the
        /// specified type and makes it the active one.  If it fails,
        /// it set the null spell checker as the active one.
        /// </summary>
        /// <param name="type">.NET class type of the spell checker</param>
        /// <returns>true on success</returns>
        private bool createAndSetActiveSpellChecker(Type type)
        {
            bool retVal;

            try
            {
                var spellChecker = (ISpellChecker)Activator.CreateInstance(type);
                retVal = spellChecker.Init();
                if (retVal)
                {
                    saveSettings(spellChecker);
                    _activeSpellChecker = spellChecker;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load spellChecker " + type +
                        ", assembly: " + type.Assembly.FullName +
                        ". Exception: " + ex);
                retVal = false;
            }

            if (!retVal)
            {
                _activeSpellChecker = _nullSpellChecker;
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