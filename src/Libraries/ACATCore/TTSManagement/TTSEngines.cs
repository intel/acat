////////////////////////////////////////////////////////////////////////////
// <copyright file="TTSBookmarkReachedEventArgs.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Maintains a list of TTS engines that were discovered.  The
    /// table it maintains is a mapping bewtween the unique GUID for
    /// each engine and the .NET Type for the class which can be used
    /// to create an instance of the engine
    /// </summary>
    public class TTSEngines : IDisposable
    {
        /// <summary>
        /// Mapping of Guid to the Type
        /// </summary>
        private readonly Dictionary<Guid, Type> _ttsEnginesTypeCache;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public TTSEngines()
        {
            _ttsEnginesTypeCache = new Dictionary<Guid, Type>();
        }

        /// <summary>
        /// Returns a collection of the TTSEngine class types discovered
        /// </summary>
        public ICollection<Type> Collection
        {
            get { return _ttsEnginesTypeCache.Values; }
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
                foreach (var type in _ttsEnginesTypeCache.Values)
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
        /// Finds engine by the specified name
        /// </summary>
        /// <param name="name">name of the engine</param>
        /// <returns>The engine</returns>
        public Guid GetByName(String name)
        {
            foreach (var type in Collection)
            {
                IDescriptor descriptor = DescriptorAttribute.GetDescriptor(type);
                if (String.Compare(descriptor.Name, name, true) == 0)
                {
                    return descriptor.Id;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Recursively descend into the the list of directories and cache the
        /// .NET class Type for each engine.
        /// </summary>
        /// <param name="extensionDirs">directories to explore</param>
        /// <returns>true on success</returns>
        public bool Load(IEnumerable<String> extensionDirs)
        {
            foreach (var dir in extensionDirs)
            {
                var extensionDir = dir + "\\" + TTSManager.TTSRootDir;
                loadEngineTypesIntoCache(extensionDir);
            }

            return true;
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
                    _ttsEnginesTypeCache.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Adds the TTSEngine Type to the cache indexed by the guid
        /// </summary>
        /// <param name="guid">GUID of the engine</param>
        /// <param name="type">.NET class Type of the engine</param>
        private void addEngineTypeToCache(Guid guid, Type type)
        {
            if (_ttsEnginesTypeCache.ContainsKey(guid))
            {
                Log.Debug("Engine " + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding TTS Engine " + type.FullName + ", guid " + guid + " to cache");
            _ttsEnginesTypeCache.Add(guid, type);
        }

        /// <summary>
        /// Recursively descends into the directory looking for DLLS that
        /// are potentially TTSEngines and caches the class Types for each
        /// engine.  The class Typs can be used to instantiate the engines
        /// </summary>
        /// <param name="dir"></param>
        private void loadEngineTypesIntoCache(String dir)
        {
            var walker = new DirectoryWalker(dir, "*.dll");
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Callback function for the directory walker.  When a DLL is found,
        /// looks for all the TTSEngine types in the DLL and caches the GUID and
        /// the class type for the engine
        /// </summary>
        /// <param name="dllName">name of the DLL to load</param>
        private void onFileFound(String dllName)
        {
            try
            {
                var ttsEngineAssembly = Assembly.LoadFile(dllName);
                foreach (var type in ttsEngineAssembly.GetTypes())
                {
                    if (typeof(ITTSEngine).IsAssignableFrom(type))
                    {
                        var attr = DescriptorAttribute.GetDescriptor(type);
                        if (attr != null && attr.Id != Guid.Empty)
                        {
                            addEngineTypeToCache(attr.Id, type);
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