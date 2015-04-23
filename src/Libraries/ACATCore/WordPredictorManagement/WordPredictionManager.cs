////////////////////////////////////////////////////////////////////////////
// <copyright file="WordPredictionManager.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.WordPredictionManagement
{
    /// <summary>
    /// Manages word prediction engines.  The engines are essentially DLLs
    /// located in the WordPredictors folder in one of the extension directories.
    /// All engines derive from the IWordPredictor interface.  This class looks
    /// for these DLL's and maintains a list of available word predictors.  The
    /// app can also set the active word predictor.
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
        /// Null word predictor. Doesn't do anything :-)
        /// </summary>
        private readonly IWordPredictor _nullWordPredictor;

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
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            _nullWordPredictor = new NullWordPredictor();
            _nullWordPredictor.Init();
            _activeWordPredictor = _nullWordPredictor;

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
        /// Gets the collection of discovered word predictors
        /// </summary>
        public ICollection<Type> WordPredictors
        {
            get { return _wordPredictors.Collection; }
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
        /// <param name="extensionDirs"></param>
        /// <returns></returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            if (_wordPredictors == null)
            {
                _wordPredictors = new WordPredictors();

                // add the null word predictor to our list of
                // recognizied word predictors
                IDescriptor descriptor = _nullWordPredictor.Descriptor;
                if (descriptor != null)
                {
                    _wordPredictors.Add(descriptor.Id, typeof(NullWordPredictor));
                }

                // walk through the directory to discover
                retVal = _wordPredictors.Load(extensionDirs);
            }

            return retVal;
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
        /// Indicates to the active word predictor that it needs to save
        /// its settings
        /// </summary>
        public void SaveSettings()
        {
            saveSettings(_activeWordPredictor);
        }

        /// <summary>
        /// Sets the active word predictor represented by id
        /// as a Guid or name.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SetActiveWordPredictor(String idOrName)
        {
            bool retVal = true;
            Guid guid = Guid.Empty;

            if (!Guid.TryParse(idOrName, out guid))
            {
                guid = _wordPredictors.GetByName(idOrName);
            }

            Type type = Guid.Equals(idOrName, Guid.Empty) ?
                            type = typeof(NullWordPredictor) :
                            _wordPredictors.Lookup(guid);

            if (type != null)
            {
                if (_activeWordPredictor != null)
                {
                    _activeWordPredictor.Dispose();
                    _activeWordPredictor = null;
                }

                retVal = createAndSetActiveWordPredictor(type);
            }
            else
            {
                createAndSetActiveWordPredictor(typeof(NullWordPredictor));
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
                    if (_activeWordPredictor != null)
                    {
                        _activeWordPredictor.Dispose();
                    }

                    if (_wordPredictors != null)
                    {
                        _wordPredictors.Dispose();
                    }

                    if (_nullWordPredictor != null)
                    {
                        _nullWordPredictor.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Creates the word predictor using reflection on the
        /// specified type and makes it the active one.  If it fails,
        /// it set the null word predictor as the active one.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool createAndSetActiveWordPredictor(Type type)
        {
            bool retVal = true;

            try
            {
                var wordPredictor = (IWordPredictor)Activator.CreateInstance(type);
                retVal = wordPredictor.Init();
                if (retVal)
                {
                    saveSettings(wordPredictor);
                    _activeWordPredictor = wordPredictor;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to load WordPredictor " + type + ", assembly: " + type.Assembly.FullName + ". Exception: " + ex.ToString());
                retVal = false;
            }

            if (!retVal)
            {
                _activeWordPredictor = _nullWordPredictor;
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