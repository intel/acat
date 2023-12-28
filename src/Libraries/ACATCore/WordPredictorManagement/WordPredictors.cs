////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictorManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ACAT.Lib.Core.WordPredictionManagement
{
    /// <summary>
    /// Maintains a list of discovered word predictors in an internal cache.
    /// The mapping is between the GUID for the word predictor and the .NET
    /// Type of the word predictor.  The Type will be used to instantiate the
    /// word predictor using Reflection
    /// </summary>
    public class WordPredictors : IDisposable
    {
        /// <summary>
        /// Name of the config file where Id's of preferred word predictors are stored
        /// </summary>
        private const String PreferredConfigFile = "PreferredWordPredictors.xml";

        /// <summary>
        /// Null word predictor. Doesn't do anything :-)
        /// </summary>
        private static IWordPredictor _nullWordPredictor = null;

        /// <summary>
        /// Table mapping the GUID and culture to the word predictor Type
        /// </summary>
        private readonly Dictionary<Guid, Tuple<String, Type>> _wordPredictorsTypeCache;

        /// <summary>
        /// Top level language-specific folder (eg en, fr etc)
        /// </summary>
        private String _dirWalkCurrentCulture;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;
        /// <summary>
        /// If one of the dll found has an error with the certificate
        /// </summary>
        private static volatile bool _DLLError = false;
        /// <summary>
        /// The object that holds the preferred word predictors
        /// </summary>
        private readonly PreferredWordPredictors _preferredWordPredictors;

        /// <summary>
        /// Initializes an instance of the WordPredictors class
        /// </summary>
        public WordPredictors()
        {
            _wordPredictorsTypeCache = new Dictionary<Guid, Tuple<String, Type>>();

            PreferredWordPredictors.FilePath = UserManager.GetFullPath(PreferredConfigFile);
            _preferredWordPredictors = PreferredWordPredictors.Load();
        }

        /// <summary>
        /// Gets the null wordpredictor object
        /// </summary>
        public static IWordPredictor NullWordPredictor
        {
            get
            {
                if (_nullWordPredictor == null)
                {
                    _nullWordPredictor = new NullWordPredictor();
                    _nullWordPredictor.Init(CultureInfo.DefaultThreadCurrentUICulture);
                }

                return _nullWordPredictor;
            }
        }

        /// <summary>
        /// Gets a collection of word predictor types from the cache
        /// </summary>
        public ICollection<Type> Collection
        {
            get
            {
                return _wordPredictorsTypeCache.Values.Select(value => value.Item2).ToList();
            }
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
        /// Returns the list of word predictors for the specified language
        /// </summary>
        /// <param name="language">language (culture)</param>
        /// <returns>list of word predictors</returns>
        public ICollection<Type> Get(String language)
        {
            var list = _wordPredictorsTypeCache.Values;

            //return (from tuple in list where String.Compare(tuple.Item1, language, true) == 0 select tuple.Item2).ToList();

            return (String.IsNullOrEmpty(language) || language.Length == 0) ?
               (from tuple in list where String.IsNullOrEmpty(tuple.Item1) select tuple.Item2).ToList() :
               (from tuple in list where String.Compare(tuple.Item1, language, true) == 0 select tuple.Item2).ToList();
        }

        /// <summary>
        /// Returns the ID of the WordPredictor that supports the
        /// specified culture info.  If no culture-specific word
        /// predictor is found, Guid.Empty is returned
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>ID of the word predictor</returns>
        public Guid GetDefaultByCulture(CultureInfo ci)
        {
            Tuple<String, Type> foundTuple = null;

            // first look for culture-specific word predictors
            foreach (var tuple in _wordPredictorsTypeCache.Values)
            {
                if (ci == null)
                {
                    if (String.IsNullOrEmpty(tuple.Item1))
                    {
                        foundTuple = tuple;
                        break;
                    }
                }
                else if (!String.IsNullOrEmpty(tuple.Item1) &&
                    (String.Compare(tuple.Item1, ci.Name, true) == 0) ||
                    String.Compare(tuple.Item1, ci.TwoLetterISOLanguageName, true) == 0)
                {
                    foundTuple = tuple;
                    break;
                }
            }

            if (foundTuple != null)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(foundTuple.Item2);
                if (descriptor != null)
                {
                    Log.Debug("Found word predictor for culture " + (ci != null ? ci.Name : "Neutral") + "[" + descriptor.Name + "]");
                    return descriptor.Id;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Returns the ID of the preferred word predictor for
        /// the specifed culture, guid.Empty if none found
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>the guid</returns>
        public Guid GetPreferredByCulture(CultureInfo ci)
        {
            return _preferredWordPredictors.GetByCulture(ci);
        }

        /// <summary>
        /// Returns the preferred word predictor or the default word
        /// predictor for the specified culture.  Returns guid.empty
        /// if none found for the culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>id of the word predictor</returns>
        public Guid GetPreferredOrDefaultByCulture(CultureInfo ci)
        {
            var guid = GetPreferredByCulture(ci);

            if (Equals(guid, Guid.Empty))
            {
                guid = GetDefaultByCulture(ci);
            }

            return guid;
        }

        /// <summary>
        /// Recursively looks into each of the extension directories looking
        /// for WordPredictor DLL's. If found, cache the GUID and Type.
        /// </summary>
        /// <param name="extensionDirs">Folders to search under</param>
        /// <param name="recursive">recursively descend and search?</param>
        /// <returns>true</returns>
        public bool Load(IEnumerable<String> extensionDirs, bool recursive = true)
        {
            foreach (string dir in extensionDirs)
            {
                var extensionDir = dir + "\\" + WordPredictionManager.WordPredictorsRootName;
                loadWordPredictorsTypesIntoCache(extensionDir, null, recursive);
            }
            if (_DLLError)
                return false;

            var languageDirs = ResourceUtils.GetInstalledLanugageDirectories();
            foreach (string dir in languageDirs)
            {
                var extensionDir = dir + "\\" + FileUtils.ExtensionsDir;
                var extensionsRoots = CoreGlobals.AppPreferences.Extensions.Split(',');
                var lastIndex = dir.LastIndexOf("\\");
                var language = dir.Substring(lastIndex + 1);

                foreach (string root in extensionsRoots)
                {
                    var extensionRoot = Path.Combine(extensionDir, root);
                    extensionRoot = Path.Combine(extensionRoot, WordPredictionManager.WordPredictorsRootName);

                    loadWordPredictorsTypesIntoCache(extensionRoot, language, recursive);
                }
            }

            return true;
        }

        /// <summary>
        /// Looks up the cache for the specified GUID and returns the Type
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Type Lookup(Guid guid)
        {
            if (Equals(guid, NullWordPredictor.Descriptor.Id))
            {
                return typeof(NullWordPredictor);
            }

            foreach (Type type in Collection)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                if (descriptor != null && Equals(guid, descriptor.Id))
                {
                    Log.Debug("Found word predictor of type " + type);
                    return type;
                }
            }

            Log.Debug("Could not find word predictor for id " + guid.ToString());
            return null;
        }

        /// <summary>
        /// Sets the preferred word predictor for the
        /// specified language
        /// </summary>
        /// <param name="language">language (culture)</param>
        /// <param name="guid">id of the word predictor</param>
        /// <returns>true on success</returns>
        public bool SetPreferred(String language, Guid guid)
        {
            bool retVal = _preferredWordPredictors.SetAsDefault(language, guid);
            if (retVal)
            {
                retVal = _preferredWordPredictors.Save();
            }

            return retVal;
        }

        /// <summary>
        /// Adds the word predictor identified by the GUID and Type to
        /// the cache
        /// </summary>
        /// <param name="guid">guid of the wp to add</param>
        /// <param name="language">language (culture of the wp)</param>
        /// <param name="type">the class Type of the wp</param>
        internal void Add(Guid guid, String language, Type type)
        {
            if (_wordPredictorsTypeCache.ContainsKey(guid))
            {
                Log.Debug("Wordpredictor " + type.FullName + ", guid " + guid.ToString() + " is already added");
                return;
            }

            Log.Debug("Adding Wordpredictor " + type.FullName + ", guid " + guid.ToString() + " to cache");
            _wordPredictorsTypeCache.Add(guid, new Tuple<String, Type>(language, type));
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

                if (_nullWordPredictor != null)
                {
                    _nullWordPredictor.Dispose();
                }

                if (disposing)
                {
                    _wordPredictorsTypeCache.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Recursively walks through the directory and discovers the word
        /// predictor DLL's in there and loads their Types into the cache
        /// </summary>
        /// <param name="dir">dir to descend into</param>
        /// <param name="culture">culture (optional) of the word predictor</param>
        /// <param name="resursive">true if deep-descend</param>
        private void loadWordPredictorsTypesIntoCache(String dir, String culture, bool resursive = true)
        {
            DirectoryWalker walker = new DirectoryWalker(dir, "*.dll");
            _dirWalkCurrentCulture = culture;
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Call back function for when it finds a DLL.  Load the dll and
        /// look for word predictor types in there. If found, add them to the cache
        /// </summary>
        /// <param name="dllName">name of the dll found</param>
        private void onFileFound(String dllName)
        {
            try
            {

                var retVal = VerifyDigitalSignature.ValidateCertificate(dllName);
                if (retVal && !_DLLError)
                {
                    try
                    {
                        VerifyDigitalSignature.Verify(dllName);
                    }
                    catch (Exception ex)
                    {
                        ConfirmBoxSingleOption confirmBoxSingleOption = new ConfirmBoxSingleOption
                        {
                            Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n WordPredictors",
                            DecisionPrompt = "ok",
                            LabelFont = 10
                        };
                        confirmBoxSingleOption.BringToFront();
                        confirmBoxSingleOption.TopMost = true;
                        confirmBoxSingleOption.ShowDialog();
                        confirmBoxSingleOption.Dispose();
                        _DLLError = true;
                    }
                }
                if (!_DLLError)
                {
                    Assembly wordPredictorAssembly = Assembly.LoadFile(dllName);
                    foreach (Type type in wordPredictorAssembly.GetTypes())
                    {
                        if (typeof(IWordPredictor).IsAssignableFrom(type))
                        {
                            DescriptorAttribute attr = DescriptorAttribute.GetDescriptor(type);
                            if (attr != null && attr.Id != Guid.Empty)
                            {
                                Add(attr.Id, _dirWalkCurrentCulture, type);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug("Could get types from assembly " + dllName + ". Exception : " + ex.ToString());
            }
        }
    }
}