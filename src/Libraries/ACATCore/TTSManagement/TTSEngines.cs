////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Maintains a list of TTS engines that were discovered.  The
    /// table it maintains is a mapping bewtween the unique GUID for
    /// each engine and the .NET Type for the class which can be used
    /// to create an instance of the engine.  Discovery is done by
    /// recursively walking the TTSEngines folder and looking for
    /// DLL's that contain TTSEngines
    /// </summary>
    public class TTSEngines : IDisposable
    {
        /// <summary>
        /// Name of the config file where Id's of preferred TTSEngines are stored
        /// </summary>
        private const String PreferredConfigFile = "PreferredTTSEngines.xml";

        /// <summary>
        /// Null TTSEngine. Doesn't do anything :-)
        /// </summary>
        private static ITTSEngine _nullTTSEngine = null;

        /// <summary>
        /// Table mapping the GUID and culture to the TTSEngine type
        /// </summary>
        private readonly Dictionary<Guid, Tuple<String, Type>> _ttsEnginesTypeCache;

        /// <summary>
        /// Top level language-specific folder (eg en, fr etc)
        /// </summary>
        private String _dirWalkCurrentCulture;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;
        /// <summary>
        /// If one of the dll found has an error with the certificate
        /// </summary>
        private static volatile bool _DLLError = false;
        /// <summary>
        /// The object that holds the preferred TTS Engines
        /// </summary>
        private PreferredTTSEngines _preferredTTSEngines;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public TTSEngines()
        {
            _ttsEnginesTypeCache = new Dictionary<Guid, Tuple<String, Type>>();

            PreferredTTSEngines.FilePath = UserManager.GetFullPath(PreferredConfigFile);
            _preferredTTSEngines = PreferredTTSEngines.Load();
        }

        /// <summary>
        /// Gets the null TTS Engine object
        /// </summary>
        public static ITTSEngine NullTTSEngine
        {
            get
            {
                if (_nullTTSEngine == null)
                {
                    _nullTTSEngine = new NullTTSEngine();
                    _nullTTSEngine.Init(CultureInfo.DefaultThreadCurrentUICulture);
                }

                return _nullTTSEngine;
            }
        }

        /// <summary>
        /// Returns a collection of the TTSEngine class types discovered
        /// </summary>
        public ICollection<Type> Collection
        {
            get
            {
                return _ttsEnginesTypeCache.Values.Select(value => value.Item2).ToList();
            }
        }

        /// <summary>
        /// Gets the TTSEngine class Type correspoding to the guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Type this[Guid guid]
        {
            get
            {
                foreach (var type in Collection)
                {
                    IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                    if (descriptor != null && Guid.Equals(guid, descriptor.Id))
                    {
                        Log.Debug("Found TTS engine of type " + type);
                        return type;
                    }
                }

                Log.Debug("Could not find TTS engine for id " + guid.ToString());
                return null;
            }
        }

        /// <summary>
        /// Disposes the TTSEngines object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the list of TTS Engines for the specified language
        /// </summary>
        /// <param name="language">language (culture)</param>
        /// <returns>list of TTS Engines</returns>
        public ICollection<Type> Get(String language)
        {
            var list = _ttsEnginesTypeCache.Values;

            //return (from tuple in list where String.Compare(tuple.Item1, language, true) == 0 select tuple.Item2).ToList();

            return (String.IsNullOrEmpty(language) || language.Length == 0) ?
               (from tuple in list where String.IsNullOrEmpty(tuple.Item1) select tuple.Item2).ToList() :
               (from tuple in list where String.Compare(tuple.Item1, language, true) == 0 select tuple.Item2).ToList();
        }

        /// <summary>
        /// Returns the ID of the TTS Engine that supports the
        /// specified culture info.  If no culture-specific
        /// TTS Engine is found, Guid.Empty is returned
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>ID of the TTS Engine</returns>
        public Guid GetDefaultByCulture(CultureInfo ci)
        {
            Tuple<String, Type> foundTuple = null;

            // first look for culture-specific TTS Engines
            foreach (var tuple in _ttsEnginesTypeCache.Values)
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
                    Log.Debug("Found TTS Engine for culture " + (ci != null ? ci.Name : "Neutral") + "[" + descriptor.Name + "]");
                    return descriptor.Id;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Returns the ID of the preferred TTS Engine for
        /// the specifed culture, guid.Empty if none found
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>the guid</returns>
        public Guid GetPreferredByCulture(CultureInfo ci)
        {
            return _preferredTTSEngines.GetByCulture(ci);
        }

        /// <summary>
        /// Returns the preferred TTS Engine or the default TTS Engine
        /// for the specified culture.  Returns guid.empty
        /// if none found for the culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>id of the TTS Engine</returns>
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
        /// for TTS Engine DLL's. If found, cache the GUID and Type.
        /// </summary>
        /// <param name="extensionDirs">Folders to search under</param>
        /// <param name="recursive">recursively descend and search?</param>
        /// <returns>true</returns>
        public bool Load(IEnumerable<String> extensionDirs, bool recursive = true)
        {
            foreach (string dir in extensionDirs)
            {
                var extensionDir = dir + "\\" + TTSManager.TTSRootDir;
                loadTTSEngineTypesIntoCache(extensionDir, null, recursive);
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
                    extensionRoot = Path.Combine(extensionRoot, TTSManager.TTSRootDir);

                    loadTTSEngineTypesIntoCache(extensionRoot, language, recursive); 
                    if (_DLLError)
                        return false;
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
            if (Equals(guid, NullTTSEngine.Descriptor.Id))
            {
                return typeof(NullTTSEngine);
            }

            foreach (Type type in Collection)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                if (descriptor != null && Equals(guid, descriptor.Id))
                {
                    Log.Debug("Found TTS Engine of type " + type);
                    return type;
                }
            }

            Log.Debug("Could not find TTS Engine for id " + guid);
            return null;
        }

        /// <summary>
        /// Sets the preferred TTS Engine for the
        /// specified language
        /// </summary>
        /// <param name="language">language (culture)</param>
        /// <param name="guid">id of the TTS Engine</param>
        /// <returns>true on success</returns>
        public bool SetPreferred(String language, Guid guid)
        {
            bool retVal = _preferredTTSEngines.SetAsDefault(language, guid);
            if (retVal)
            {
                retVal = _preferredTTSEngines.Save();
            }

            return retVal;
        }

        /// <summary>
        /// Adds the TTSEngine Type to the cache indexed by the guid
        /// </summary>
        /// <param name="guid">GUID of the engine</param>
        /// <param name="type">.NET class Type of the engine</param>
        internal void Add(Guid guid, String language, Type type)
        {
            if (_ttsEnginesTypeCache.ContainsKey(guid))
            {
                Log.Debug("TTS Engine" + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding TTS Engine " + type.FullName + ", guid " + guid + " to cache");
            _ttsEnginesTypeCache.Add(guid, new Tuple<String, Type>(language, type));
        }

        /// <summary>
        /// Disposer. Releases resources and cleanup.
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
                    if (_nullTTSEngine != null)
                    {
                        _nullTTSEngine.Dispose();
                    }

                    // dispose all managed resources.
                    _ttsEnginesTypeCache.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Recursively walks through the directory and discovers the TTS
        /// Engine DLL's in there and loads their Types into the cache
        /// </summary>
        /// <param name="dir">dir to descend into</param>
        /// <param name="culture">culture (optional) of the TTS Engine</param>
        /// <param name="resursive">true if deep-descend</param>
        private void loadTTSEngineTypesIntoCache(String dir, String culture, bool resursive = true)
        {
            DirectoryWalker walker = new DirectoryWalker(dir, "*.dll");
            _dirWalkCurrentCulture = culture;
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Call back function for when it finds a DLL.  Load the dll and
        /// look for TTS Engine types in there. If found, add them to the cache
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
                            Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n Status Error: ERTTS",
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
                    Assembly ttsEngineAssembly = Assembly.LoadFile(dllName);
                    foreach (Type type in ttsEngineAssembly.GetTypes())
                    {
                        if (typeof(ITTSEngine).IsAssignableFrom(type))
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