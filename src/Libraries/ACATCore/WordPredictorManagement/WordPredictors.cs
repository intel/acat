////////////////////////////////////////////////////////////////////////////
// <copyright file="WordPredictors.cs" company="Intel Corporation">
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
    /// Maintains a list of discovered word predictors in an internal cache.
    /// The mapping is between the GUID for the word predictor and the .NET
    /// Type of the word predictor.  The Type will be used to instantiate the
    /// word predictor using Reflection
    /// </summary>
    public class WordPredictors : IDisposable
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Table mapping the GUID to the word predictor Type
        /// </summary>
        private Dictionary<Guid, Type> _wordPredictorsTypeCache;

        /// <summary>
        /// Initializes an instance of the WordPredictors class
        /// </summary>
        public WordPredictors()
        {
            _wordPredictorsTypeCache = new Dictionary<Guid, Type>();
        }

        /// <summary>
        /// Gets a collection of word predictor types from the cache
        /// </summary>
        public ICollection<Type> Collection
        {
            get { return _wordPredictorsTypeCache.Values; }
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

        public Guid GetByName(String name)
        {
            foreach (Type type in _wordPredictorsTypeCache.Values)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                if (descriptor != null && String.Compare(descriptor.Name, name, true) == 0)
                {
                    Log.Debug("Found word predictor [" + name + "]");
                    return descriptor.Id;
                }
            }

            Log.Debug("Could not find word predictor for [" + name + "]");
            return Guid.Empty;
        }

        /// <summary>
        /// Recursively look into each of the extension directories looking
        /// for WordPredictor DLL's. If found, cache the GUID and Type.
        /// </summary>
        /// <param name="extensionDirs"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public bool Load(IEnumerable<String> extensionDirs, bool recursive = true)
        {
            foreach (string dir in extensionDirs)
            {
                var extensionDir = dir + "\\" + WordPredictionManager.WordPredictorsRootName;
                loadWordPredictorsTypesIntoCache(extensionDir, recursive);
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
            foreach (Type type in _wordPredictorsTypeCache.Values)
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
        /// Adds the word predictor identified by the GUID and Type to
        /// the cache
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        internal void Add(Guid guid, Type type)
        {
            if (_wordPredictorsTypeCache.ContainsKey(guid))
            {
                Log.Debug("Wordpredictor " + type.FullName + ", guid " + guid.ToString() + " is already added");
                return;
            }

            Log.Debug("Adding Wordpredictor " + type.FullName + ", guid " + guid.ToString() + " to cache");
            _wordPredictorsTypeCache.Add(guid, type);
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
        /// <param name="dir"></param>
        /// <param name="resursive"></param>
        private void loadWordPredictorsTypesIntoCache(String dir, bool resursive = true)
        {
            DirectoryWalker walker = new DirectoryWalker(dir, "*.dll");
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Call back function for when it finds a DLL.  Load the dll and
        /// look for word predictor types in there. If found, add them to the cache
        /// </summary>
        /// <param name="dllName"></param>
        private void onFileFound(String dllName)
        {
            try
            {
                Assembly wordPredictorAssembly = Assembly.LoadFile(dllName);
                foreach (Type type in wordPredictorAssembly.GetTypes())
                {
                    if (typeof(IWordPredictor).IsAssignableFrom(type))
                    {
                        DescriptorAttribute attr = DescriptorAttribute.GetDescriptor(type);
                        if (attr != null && attr.Id != Guid.Empty)
                        {
                            Add(attr.Id, type);
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