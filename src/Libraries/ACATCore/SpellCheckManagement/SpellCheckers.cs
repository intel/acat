////////////////////////////////////////////////////////////////////////////
// <copyright file="SpellCheckers.cs" company="Intel Corporation">
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
    /// Maintains a list of discovered word predictors in an internal cache.
    /// The mapping is between the GUID for the word predictor and the .NET
    /// Type of the word predictor.  The Type will be used to instantiate the
    /// word predictor using Reflection
    /// </summary>
    public class SpellCheckers : IDisposable
    {
        /// <summary>
        /// Table mapping the GUID to the word predictor Type
        /// </summary>
        private readonly Dictionary<Guid, Type> _spellCheckersTypeCache;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes an instance of the WordPredictors class
        /// </summary>
        public SpellCheckers()
        {
            _spellCheckersTypeCache = new Dictionary<Guid, Type>();
        }

        /// <summary>
        /// Gets a collection of word predictor types from the cache
        /// </summary>
        public ICollection<Type> Collection
        {
            get { return _spellCheckersTypeCache.Values; }
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
        /// Returns the ID of the spellchecker by looking
        /// up the spellCheckerName of the spellchecker
        /// </summary>
        /// <param spellCheckerName="spellCheckerName">name to check</param>
        /// <returns>the spellchecker id, Empty if not found</returns>
        public Guid GetByName(String spellCheckerName)
        {
            foreach (var type in _spellCheckersTypeCache.Values)
            {
                var descriptor = DescriptorAttribute.GetDescriptor(type);
                if (descriptor != null && String.Compare(descriptor.Name, spellCheckerName, true) == 0)
                {
                    Log.Debug("Found spellchecker [" + spellCheckerName + "]");
                    return descriptor.Id;
                }
            }

            Log.Debug("Could not find spellchecker for [" + spellCheckerName + "]");
            return Guid.Empty;
        }

        /// <summary>
        /// Recursively look into each of the extension directories looking
        /// for WordPredictor DLL's. If found, cache the GUID and Type.
        /// </summary>
        /// <param spellCheckerName="extensionDirs">list of extension directories</param>
        /// <param spellCheckerName="recursive">should it look deep?</param>
        /// <returns></returns>
        public bool Load(IEnumerable<String> extensionDirs, bool recursive = true)
        {
            foreach (var dir in extensionDirs)
            {
                var extensionDir = dir + "\\" + SpellCheckManager.SpellCheckersRootName;
                loadSpellCheckerTypesIntoCache(extensionDir);
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
            foreach (Type type in _spellCheckersTypeCache.Values)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                if (descriptor != null && Guid.Equals(guid, descriptor.Id))
                {
                    Log.Debug("Found spellchecker of type " + type);
                    return type;
                }
            }

            Log.Debug("Could not find spellchecker for id " + guid.ToString());
            return null;
        }

        /// <summary>
        /// Adds the spell checker identified by the GUID and Type to
        /// the cache.
        /// <param name="guid">guid of the spell checker</param>
        /// <param name="type">.NET Type of the class</param>
        internal void add(Guid guid, Type type)
        {
            if (_spellCheckersTypeCache.ContainsKey(guid))
            {
                Log.Debug("Wordpredictor " + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding Wordpredictor " + type.FullName + ", guid " + guid + " to cache");
            _spellCheckersTypeCache.Add(guid, type);
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
                    _spellCheckersTypeCache.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Recursively walks through the directory and discovers the word
        /// predictor DLL's in there and loads their Types into the cache
        /// </summary>
        /// <param name="dir">root directory</param>
        private void loadSpellCheckerTypesIntoCache(String dir)
        {
            var walker = new DirectoryWalker(dir, "*.dll");
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
                var wordPredictorAssembly = Assembly.LoadFile(dllName);
                foreach (var type in wordPredictorAssembly.GetTypes())
                {
                    if (typeof(ISpellChecker).IsAssignableFrom(type))
                    {
                        var attr = DescriptorAttribute.GetDescriptor(type);
                        if (attr != null && attr.Id != Guid.Empty)
                        {
                            add(attr.Id, type);
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