////////////////////////////////////////////////////////////////////////////
// <copyright file="Actuators.cs" company="Intel Corporation">
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
using System.IO;
using System.Reflection;
using System.Xml;
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
#endregion

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Contains a collection of actuators in the system.  Creates
    /// the actuator objects by parsing the Actuator config file,
    /// looks into the Actuators extension directory, loads the DLL's
    /// and caches the Type of the actuator objects
    /// </summary>
    public class Actuators : IDisposable
    {
        /// <summary>
        /// A map of the guid and the type of the actuator.  The Type will
        /// be used to create an instance of the actuator
        /// </summary>
        private readonly Dictionary<Guid, Type> _actuatorsTypeCache;

        /// <summary>
        /// A list of actuators
        /// </summary>
        private readonly List<IActuator> _actuators;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Initializes the Actuator object
        /// </summary>
        public Actuators()
        {
            _actuatorsTypeCache = new Dictionary<Guid, Type>();
            _actuators = new List<IActuator>();
        }

        /// <summary>
        /// Gets the list of actuators
        /// </summary>
        public IEnumerable<IActuator> Collection
        {
            get
            {
                return _actuators;
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
        /// Parse the "Actuators" root element in the XML file and 
        /// create a list of actuators. Also loads the attributes for 
        /// each actuator
        /// </summary>
        /// <param name="configFile">Name of the XML config file</param>
        /// <returns></returns>
        public bool Load(IEnumerable<String> extensionDirs, String configFile, bool recursive = true)
        {
            addKeyboardActuatorToCache();

            foreach (string dir in extensionDirs)
            {
                String extensionDir = dir + "\\" + ActuatorManager.ActuatorsRootDir;
                loadActuatorTypesIntoCache(extensionDir, recursive);
            }

            var doc = new XmlDocument();

            if (!File.Exists(configFile))
            {
                return false;
            }

            doc.Load(configFile);

            var actuatorNodes = doc.SelectNodes("/ACAT/Actuators/Actuator");

            if (actuatorNodes == null)
            {
                return false;
            }

            // enumerate all the actuator nodes, create an object and add it
            // to the collection only if the enabled attribute is set
            foreach (XmlNode node in actuatorNodes)
            {
                try
                {
                    bool enabled = XmlUtils.GetXMLAttrBool(node, "enabled", false);
                    String id = XmlUtils.GetXMLAttrString(node, "id");
                    String name = XmlUtils.GetXMLAttrString(node, "name");
                    if (enabled && !String.IsNullOrEmpty(id))
                    {
                        Guid guid;
                        if (Guid.TryParse(id, out guid))
                        {
                            if (_actuatorsTypeCache.ContainsKey(guid))
                            {
                                var type = _actuatorsTypeCache[guid];
                                if (type != null)
                                {
                                    // allow the actuator to load its info from the XML file
                                    var assembly = Assembly.LoadFrom(type.Assembly.Location);
                                    var actuator = (IActuator) assembly.CreateInstance(type.FullName);
                                    if (actuator != null)
                                    {
                                        actuator.Load(node);
                                        actuator.Name = name;
                                        _actuators.Add(actuator);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }

            return true;
        }

        /// <summary>
        /// Returns an actutator object of the specified type
        /// </summary>
        /// <param name="actuatorType">Type of the object</param>
        /// <returns>The object </returns>
        public IActuator Find(Type actuatorType)
        {
            foreach (var actuator in _actuators)
            {
                if (actuatorType.FullName == actuator.GetType().FullName)
                {
                    Log.Debug("Found actuator of type " + actuatorType.Name);
                    return actuator;
                }
            }

            Log.Debug("Could not find actuator of type " + actuatorType.Name);
            return null;
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
                    foreach (var actuator in _actuators)
                    {
                        actuator.Dispose();
                    }

                    _actuatorsTypeCache.Clear();
                    _actuators.Clear();
                }

                // Release unmanaged resources. 
            }

            _disposed = true;
        }

        /// <summary>
        /// Adds the keyboard actuator to the list of actuators.  This
        /// actuator is bundled with the SDK.
        /// </summary>
        private void addKeyboardActuatorToCache()
        {
            var attr = DescriptorAttribute.GetDescriptor(typeof(InputActuators.KeyboardActuator));
            if (attr != null)
            {
                addActuatorToCache(attr.Id, typeof(InputActuators.KeyboardActuator));
            }
        }

        /// <summary>
        /// Recursively descends into the directory and loads all the 
        /// actuator types in each of the actuator DLLs
        /// </summary>
        /// <param name="dir">Directory to descend into/param>
        /// <param name="resursive">Descend recursively</param>
        private void loadActuatorTypesIntoCache(String dir, bool resursive = true)
        {
            var walker = new DirectoryWalker(dir, "*.dll");
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Callback function for the directory walker. called whenever
        /// it finds a DLL
        /// </summary>
        /// <param name="dllName"></param>
        private void onFileFound(String dllName)
        {
            try
            {
                var inputActuatorsAssembly = Assembly.LoadFrom(dllName);
                foreach (Type type in inputActuatorsAssembly.GetTypes())
                {
                    if (typeof(ActuatorBase).IsAssignableFrom(type))
                    {
                        var attr = DescriptorAttribute.GetDescriptor(type);
                        if (attr != null && attr.Id != Guid.Empty)
                        {
                            addActuatorToCache(attr.Id, type);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Could not get types from assembly " + dllName + ". Exception : " + ex.ToString());
            }
        }

        /// <summary>
        /// Adds the actuator with the GUID and type to the cache
        /// </summary>
        /// <param name="guid">GUID of the actuator</param>
        /// <param name="type">Type of the actuato</param>
        private void addActuatorToCache(Guid guid, Type type)
        {
            if (_actuatorsTypeCache.ContainsKey(guid))
            {
                Log.Debug("Actuator " + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding Actuator " + type.FullName + ", guid " + guid + " to cache");
            _actuatorsTypeCache.Add(guid, type);
        }
    }
}
