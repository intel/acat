﻿using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.SpellCheckManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Lib.Core.DialogSenseManagement
{
    /// <summary>
    /// Maintains a list of discovered SpellCheckers in an internal cache.
    /// The mapping is between the GUID for the SpellChecker and the
    /// SpellChecker object.
    /// </summary>
    public class DialogSenseAgents : IDisposable
    {
        /// <summary>
        /// Name of the config file where ID's of preferred spellcheckers are stored
        /// </summary>
        private const String PreferredConfigFile = "PreferredDialogSenseAgents.xml";

        /// <summary>
        /// Null spellchecker. Doesn't do anything :-)
        /// </summary>
        private static IDialogSenseAgent _nullDialogSenseAgent = null;

        /// <summary>
        /// The object that holds the preferred spellcheckers
        /// </summary>
        private readonly PreferredDialogSenseAgents _preferredDialogSenseAgents;

        /// <summary>
        /// Table mapping the GUID and culture to the spellchecker Type
        /// </summary>
        private readonly Dictionary<Guid, Tuple<String, Type>> _dialogSenseAgentsTypeCache;

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
        /// Initializes an instance of the class
        /// </summary>
        public DialogSenseAgents()
        {
            _dialogSenseAgentsTypeCache = new Dictionary<Guid, Tuple<String, Type>>();

            PreferredDialogSenseAgents.FilePath = UserManager.GetFullPath(PreferredConfigFile);
            _preferredDialogSenseAgents = PreferredDialogSenseAgents.Load();
        }

        /// <summary>
        /// Gets the null spellchecker object
        /// </summary>
        public static IDialogSenseAgent NullDialogSenseAgent
        {
            get
            {
                if (_nullDialogSenseAgent == null)
                {
                    _nullDialogSenseAgent = new NullDialogSenseAgent();
                    _nullDialogSenseAgent.Init(CultureInfo.DefaultThreadCurrentUICulture);
                }

                return _nullDialogSenseAgent;
            }
        }

        /// <summary>
        /// Gets a collection of spellchecker types from the cache
        /// </summary>
        public ICollection<Type> Collection
        {
            get
            {
                return _dialogSenseAgentsTypeCache.Values.Select(value => value.Item2).ToList();
            }
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
        /// Returns the list of spellcheckers for the specified language
        /// </summary>
        /// <param name="language">language (culture)</param>
        /// <returns>list of spellcheckers</returns>
        public ICollection<Type> Get(String language)
        {
            var list = _dialogSenseAgentsTypeCache.Values;

            return (String.IsNullOrEmpty(language) || language.Length == 0) ?
                (from tuple in list where String.IsNullOrEmpty(tuple.Item1) select tuple.Item2).ToList() :
                (from tuple in list where String.Compare(tuple.Item1, language, true) == 0 select tuple.Item2).ToList();
        }

        /// <summary>
        /// Returns the ID of the SpellChecker that supports the
        /// specified culture info.  If no culture-specific spellchecker
        /// is found, Guid.Empty is returned
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>ID of the spellchecker</returns>
        public Guid GetDefaultByCulture(CultureInfo ci)
        {
            Tuple<String, Type> foundTuple = null;

            // first look for culture-specific word predictors
            foreach (var tuple in _dialogSenseAgentsTypeCache.Values)
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
                    Log.Debug("Found DialogSenseAgent for culture " + (ci != null ? ci.Name : "Neutral") + "[" + descriptor.Name + "]");
                    return descriptor.Id;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Returns the ID of the preferred spellcheckr for
        /// the specifed culture, guid.Empty if none found
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>the guid</returns>
        public Guid GetPreferredByCulture(CultureInfo ci)
        {
            return _preferredDialogSenseAgents.GetByCulture(ci);
        }

        /// <summary>
        /// Returns the preferred spellchecker or the default spellchecker
        /// for the specified culture.  Returns guid.empty
        /// if none found for the culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>id of the spellchecker</returns>
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
        /// Recursively look into each of the extension directories looking
        /// for SpellChecker DLL's. If found, cache the GUID and an instance of the
        /// object.
        /// </summary>
        /// <param spellCheckerName="extensionDirs">list of extension directories</param>
        /// <param spellCheckerName="recursive">should it look deep?</param>
        /// <returns></returns>
        public bool Load(IEnumerable<String> extensionDirs, bool recursive = true)
        {
            foreach (var dir in extensionDirs)
            {
                var extensionDir = dir + "\\" + DialogSenseManager.DialogSenseManagersRootName;
                loadDialogSenseAgentTypesIntoCache(extensionDir, null, recursive);
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
                    extensionRoot = Path.Combine(extensionRoot, DialogSenseManager.DialogSenseManagersRootName);

                    loadDialogSenseAgentTypesIntoCache(extensionRoot, language, recursive);
                }
            }
            return true;
        }

        /// <summary>
        /// Looks up the cache for the specified GUID and returns the Type
        /// of the object
        /// </summary>
        /// <param spellCheckerName="guid">Guid to look for</param>
        /// <returns>.NET Type </returns>
        public Type Lookup(Guid guid)
        {
            if (Equals(guid, NullDialogSenseAgent.Descriptor.Id))
            {
                return typeof(NullDialogSenseAgent);
            }

            foreach (Type type in Collection)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                if (descriptor != null && Equals(guid, descriptor.Id))
                {
                    Log.Debug("Found dialogSenseAgent of type " + type);
                    return type;
                }
            }

            Log.Debug("Could not find DialogSenseAgent for id " + guid);
            return null;
        }

        /// <summary>
        /// Sets the preferred spellchecker for the
        /// specified language
        /// </summary>
        /// <param name="language">language (culture)</param>
        /// <param name="guid">id of the spellchecker</param>
        /// <returns>true on success</returns>
        public bool SetPreferred(String language, Guid guid)
        {
            try
            {
                bool retVal = _preferredDialogSenseAgents.SetAsDefault(language, guid);
                if (retVal)
                {
                    retVal = _preferredDialogSenseAgents.Save();
                }

                return retVal;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the SpellChecker identified by the GUID and Type to
        /// the cache
        /// </summary>
        /// <param name="guid">guid of the wp to add</param>
        /// <param name="language">language (culture of the wp)</param>
        /// <param name="type">the class Type of the wp</param>
        internal void Add(Guid guid, String language, Type type)
        {
            if (_dialogSenseAgentsTypeCache.ContainsKey(guid))
            {
                Log.Debug("SpellChecker " + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding SpellChecker " + type.FullName + ", guid " + guid + " to cache");
            _dialogSenseAgentsTypeCache.Add(guid, new Tuple<String, Type>(language, type));
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
                    _dialogSenseAgentsTypeCache.Clear();

                    if (_nullDialogSenseAgent != null)
                    {
                        _nullDialogSenseAgent.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Recursively walks through the directory and discovers the spellchecker
        /// DLL's in there and loads their Types into the cache
        /// </summary>
        /// <param name="dir">dir to descend into</param>
        /// <param name="culture">culture (optional) of the word predictor</param>
        /// <param name="resursive">true if deep-descend</param>
        private void loadDialogSenseAgentTypesIntoCache(String dir, String culture, bool resursive = true)
        {
            DirectoryWalker walker = new DirectoryWalker(dir, "*.dll");
            _dirWalkCurrentCulture = culture;
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Call back function for when it finds a DLL.  Load the dll and
        /// look for spell checker types in there. If found, add them to the cache
        /// </summary>
        /// <param name="dllName">Full path to the dll</param>
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
                            Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n Status Error: ERSC",
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
                    var dialogSenseAssembly = Assembly.LoadFile(dllName);
                    foreach (var type in dialogSenseAssembly.GetTypes())
                    {
                        if (typeof(IDialogSenseAgent).IsAssignableFrom(type))
                        {
                            var attr = DescriptorAttribute.GetDescriptor(type);
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
                Log.Debug("Could get types from assembly " + dllName + ". Exception : " + ex);
            }
        }
    }
}
