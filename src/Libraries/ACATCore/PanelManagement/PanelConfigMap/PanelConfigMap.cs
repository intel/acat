////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelConfigMap.cs" company="Intel Corporation">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;
using ACAT.Lib.Core.AgentManagement;
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// PanelConfigMap is an xml file that contains a mapping between the
    /// name of the class for the scanner and the name of the xml file that
    /// contains animation and other info for the scanner.  By default, the
    /// name of the class is used as the name of the xml file, but using
    /// PanelConfigMap, the user can use it to map to any filename.  This
    /// gives us a way to try out different animations for the same scanner.
    /// </summary>
    public class PanelConfigMap
    {
        /// <summary>
        /// Name of the config file that has the mapping.  This is loaded from
        /// the user directory
        /// </summary>
        private const String PanelConfigMapFileName = "panelconfigmap.xml";

        /// <summary>
        /// Maps a name of a scanner to the panelconfigmapentry
        /// </summary>
        private static readonly Dictionary<String, List<PanelConfigMapEntry>> _mapTable = new Dictionary<String, List<PanelConfigMapEntry>>();

        /// <summary>
        /// Maps the name of a config file to the complete path of the file
        /// </summary>
        private static readonly Dictionary<String, String> _configFileLocationMap = new Dictionary<String, String>();

        /// <summary>
        /// Caches the class Type of forms
        /// </summary>
        private static readonly Hashtable _formsCache = new Hashtable();

        /// <summary>
        /// Stores preferred config file names
        /// </summary>
        private static readonly PreferredPanelConfig _preferredPanelConfig = new PreferredPanelConfig();

        /// <summary>
        /// Initializes a static instance of hte class
        /// </summary>
        static PanelConfigMap()
        {
            _preferredPanelConfig.Load();
        }

        public static String[] PreferredPanelConfigNames
        {
            get
            {
                return _preferredPanelConfig.PreferredPanelConfigNames;
            }

            set
            {
                _preferredPanelConfig.PreferredPanelConfigNames = value;
            }
        }

        /// <summary>
        /// Walks the directories specified in extensionDir,
        /// looks for DLL's, loads all the types and looks for
        /// Types that are derived from IPanel (which is all the
        /// scanners) and caches them
        /// </summary>
        /// <param name="extensionDirs">Directories to look</param>
        /// <returns>true on success</returns>
        public static bool Load(IEnumerable<String> extensionDirs)
        {
            foreach (string dir in extensionDirs)
            {
                String extensionDir = dir + "\\" + AgentManager.AppAgentsRootDir;
                loadPanelConfigMap(extensionDir);

                extensionDir = dir + "\\" + AgentManager.FunctionalAgentsRootDir;
                loadPanelConfigMap(extensionDir);
            }

            foreach (string dir in extensionDirs)
            {
                String extensionDir = dir + "\\" + PanelManager.UiRootDir;
                Log.Debug("LoadPanelConfigMap for " + extensionDir);
                loadPanelConfigMap(extensionDir);
            }

            return true;
        }

        /// <summary>
        /// Loads class Types from the specified assembly
        /// </summary>
        /// <param name="assembly">Assembly to load from</param>
        /// <returns>true on success</returns>
        public static bool Load(Assembly assembly)
        {
            bool retVal = loadTypesFromAssembly(assembly);
            return retVal;
        }

        /// <summary>
        /// Returns the config map for the specified scanner.  Looks up
        /// the preferred config map first to see if there is a config
        /// entry that has been specifically configured for the scanner.
        /// If not, it looks up the map talbe
        /// </summary>
        /// <param name="panel">Name of the scanner</param>
        /// <returns>Panel config map object</returns>
        public static PanelConfigMapEntry GetPanelConfigMapEntry(String panel)
        {
            PanelConfigMapEntry retVal = null;
            List<PanelConfigMapEntry> list = null;

            _mapTable.TryGetValue(panel, out list);

            if (list == null || list.Count <= 0)
            {
                return null;
            }

            var configName = _preferredPanelConfig.GetConfigName(panel);
            if (!String.IsNullOrEmpty(configName))
            {
                retVal = findMapEntryInList(configName, list);
            }

            if (retVal == null)
            {
                retVal = findMapEntryInMapTable(configName);
            }

            if (retVal == null)
            {
                retVal = list[0];
            }

            return retVal;
        }

        /// <summary>
        /// Returns the name of the animation config file for the specified
        /// scanner
        /// </summary>
        /// <param name="panelClass">scanner name/class</param>
        /// <returns>the animation config file name</returns>
        public static String GetConfigFileForPanel(String panelClass)
        {
            var retVal = String.Empty;
            var mapEntry = GetPanelConfigMapEntry(panelClass);
            if (mapEntry != null)
            {
                retVal = mapEntry.ConfigFileName;
            }

            return retVal;
        }

        /// <summary>
        /// Returns the config file for the specified Type that
        /// represents a scanner
        /// </summary>
        /// <param name="type">Type of the scanner</param>
        /// <returns>animation config file name</returns>
        public static String GetConfigFileForScreen(Type type)
        {
            String retVal = String.Empty;
            String xmlFileName;

            Log.Debug("Type: " + type.FullName);
            Guid guid = GetFormId(type);
            if (guid != Guid.Empty)
            {
                Log.Debug(type + " has a guid " + guid);
                xmlFileName = findConfigFileByGuid(guid);
            }
            else
            {
                Log.Debug(type + " does not have a guid. Checking type instead");
                xmlFileName = findConfigFileByType(type);
            }

            if (String.IsNullOrEmpty(xmlFileName))
            {
                Log.Debug("Did not find config file for " + type + " in maptable");
                xmlFileName = (type.Name + ".xml").ToLower();
                Log.Debug("Checking for configFile " + xmlFileName + " in _configFileMap");
                if (_configFileLocationMap.ContainsKey(xmlFileName))
                {
                    retVal = _configFileLocationMap[xmlFileName];
                    Log.Debug("Found " + xmlFileName + " location. It is " + retVal);
                }
                else
                {
                    Log.Debug("configFileMap does not have xmlfile " + xmlFileName);
                }
            }
            else
            {
                retVal = xmlFileName;
            }

            Log.Debug("For type " + type.Name + " config file is [" + retVal + "]");
            return retVal;
        }

        /// <summary>
        /// Checks if two scanners are the same
        /// </summary>
        /// <param name="panel1">first scanner</param>
        /// <param name="panel2">scanner to compare</param>
        /// <returns>true if they are</returns>
        public static bool AreEqual(String panel1, String panel2)
        {
            return String.Compare(panel1, panel2, true) == 0;
        }

        /// <summary>
        /// Cleans up the map tables of entries that are orphans. These
        /// are forms that don't have a corresponding animatinon file
        /// </summary>
        internal static void CleanupOrphans()
        {
            Log.Debug("Cleaning up panelConfigMap entries...");

            var removeList = new List<PanelConfigMapEntry>();
            foreach (var list in _mapTable.Values)
            {
                foreach (var mapEntry in list)
                {
                    Log.Debug("Looking up " + mapEntry.ToString());
                    if (_formsCache.ContainsKey(mapEntry.FormId))
                    {
                        mapEntry.FormType = (Type)_formsCache[mapEntry.FormId];
                    }

                    /*else if (_formsCache.ContainsKey(mapEntry.PanelName) && mapEntry.FormId.Equals(Guid.Empty))
                    {
                        mapEntry.FormType = (Type)_formsCache[mapEntry.PanelName];
                    }
                    */
                    Log.IsNull("mapEntry.FormType ", mapEntry.FormType);

                    if (mapEntry.FormType != null && _configFileLocationMap.ContainsKey(mapEntry.ConfigFileName))
                    {
                        Log.Debug("Yes. _configFileLocationMap has configfile " + mapEntry.ConfigFileName);
                        mapEntry.ConfigFileName = _configFileLocationMap[mapEntry.ConfigFileName];
                    }
                    else
                    {
                        Log.Debug("No. _configFileLocationMap does not have configfile " + mapEntry.ConfigFileName);
                        removeList.Add(mapEntry);
                    }
                }
            }

            foreach (var panelConfigMapEntry in removeList)
            {
                removeMapEntry(panelConfigMapEntry);
            }
        }

        /// <summary>
        /// Gets the ACAT descriptor guid for the specifed
        /// Type
        /// </summary>
        /// <param name="type">Scanner class Type</param>
        /// <returns>The descirptor guid</returns>
        internal static Guid GetFormId(Type type)
        {
            var descAttribute = DescriptorAttribute.GetDescriptor(type);
            Guid retVal = Guid.Empty;
            if (descAttribute != null)
            {
                retVal = descAttribute.Id;
            }

            return retVal;
        }

        /// <summary>
        /// Adds the specified Type to the cache keyed by
        /// the Guid.
        /// </summary>
        /// <param name="guid">Guid for the scanner</param>
        /// <param name="type">Scanner class Type</param>
        internal static void AddFormToCache(Guid guid, Type type)
        {
            if (_formsCache.ContainsKey(guid))
            {
                Log.Debug("Screen " + type.FullName + ", guid " + guid.ToString() + " is already added");
                return;
            }

            Log.Debug("Adding screen " + type.FullName + ", guid " + guid.ToString() + " to cache");
            _formsCache.Add(guid, type);

            var mapEntry = new PanelConfigMapEntry(type.Name, type.Name, (type.Name + ".xml").ToLower(), guid, type);
            Log.Debug("mapEntry.ConfigFileName: " + mapEntry.ConfigFileName);
            addToMapTable(mapEntry);

            updateFormTypeReferences(guid, type);
        }

        /// <summary>
        /// Adds the specified mapEntry object to the map table. Also
        /// looks up the map table if it already has the formID specified
        /// in the mapEntry and updates its config file name with the
        /// one in mapEntry
        /// </summary>
        /// <param name="mapEntry">map entry to add</param>
        private static void addToMapTable(PanelConfigMapEntry mapEntry)
        {
            if (!_mapTable.ContainsKey(mapEntry.PanelClass))
            {
                _mapTable.Add(mapEntry.PanelClass, new List<PanelConfigMapEntry>());
            }

            List<PanelConfigMapEntry> list = _mapTable[mapEntry.PanelClass];
#if abc
            foreach (var panelConfigMapEntry in list)
            {
                if (panelConfigMapEntry.FormId == mapEntry.FormId)
                {
                    panelConfigMapEntry.ConfigFileName = mapEntry.ConfigFileName;
                    return;
                }
            }
#endif
            Log.Debug("Adding " + mapEntry);
            list.Add(mapEntry);
        }

        /// <summary>
        /// Removes the specified map entry from the map table
        /// </summary>
        /// <param name="entryToRemove">entry to remove</param>
        private static void removeMapEntry(PanelConfigMapEntry entryToRemove)
        {
            foreach (List<PanelConfigMapEntry> list in _mapTable.Values)
            {
                foreach (PanelConfigMapEntry mapEntry in list)
                {
                    if (mapEntry == entryToRemove)
                    {
                        list.Remove(mapEntry);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Looks up the map table for the specified animation
        /// config file and returns the map entry
        /// </summary>
        /// <param name="configName">name of the animation file</param>
        /// <returns>the map entry</returns>
        private static PanelConfigMapEntry findMapEntryInMapTable(String configName)
        {
            PanelConfigMapEntry retVal = null;
            foreach (var list in _mapTable.Values)
            {
                retVal = findMapEntryInList(configName, list);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Finds the specified config file name in the list and returns
        /// the map entry object that has the file name
        /// </summary>
        /// <param name="configName">name of the animation file</param>
        /// <param name="list">list to look in</param>
        /// <returns></returns>
        private static PanelConfigMapEntry findMapEntryInList(String configName, IEnumerable<PanelConfigMapEntry> list)
        {
            foreach (var mapEntry in list)
            {
                if (String.Compare(configName, mapEntry.ConfigName, true) == 0)
                {
                    return mapEntry;
                }
            }

            return null;
        }

        /// <summary>
        /// Looks up the map table for the speified Scanner
        /// Type and returns its animation config file name
        /// </summary>
        /// <param name="type">scanner Type</param>
        /// <returns>the animation config file name</returns>
        private static String findConfigFileByType(Type type)
        {
            String retVal = String.Empty;
            foreach (List<PanelConfigMapEntry> list in _mapTable.Values)
            {
                foreach (PanelConfigMapEntry mapEntry in list)
                {
                    if (mapEntry.ConfigName == type.Name)
                    {
                        retVal = mapEntry.ConfigFileName;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Looks up the map table for the specified
        /// scanner guid and returns the animation config file
        /// </summary>
        /// <param name="guid">the scanner guid</param>
        /// <returns>animation config file name</returns>
        private static String findConfigFileByGuid(Guid guid)
        {
            String retVal = String.Empty;
            foreach (List<PanelConfigMapEntry> list in _mapTable.Values)
            {
                foreach (PanelConfigMapEntry mapEntry in list)
                {
                    if (Guid.Equals(guid, mapEntry.FormId))
                    {
                        retVal = mapEntry.ConfigFileName;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Walks the specified directory (rescursively)
        /// to look for files
        /// </summary>
        /// <param name="dir">Directory to walk</param>
        /// <param name="resursive">Recursively search?</param>
        private static void loadPanelConfigMap(String dir, bool resursive = true)
        {
            var walker = new DirectoryWalker(dir, "*.*");
            Log.Debug("Walking dir " + dir);
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Callback function for the directory walker that's invoked
        /// when a file is found.  Checks the file is a dll or an
        /// xml file and handles them appropriately
        /// </summary>
        /// <param name="file">name of the file found</param>
        private static void onFileFound(String file)
        {
            String filePath = file.ToLower();
            String fileName = Path.GetFileName(filePath);
            if (String.Compare(fileName, PanelConfigMapFileName, true) == 0)
            {
                onScreenConfigFileFound(filePath);
            }
            else
            {
                String extension = Path.GetExtension(filePath);
                if (String.Compare(extension, ".dll", true) == 0)
                {
                    onDllFound(filePath);
                }
                else if (String.Compare(extension, ".xml", true) == 0)
                {
                    onXmlFileFound(filePath);
                }
            }
        }

        /// <summary>
        /// Found the panel config file. This is the xml file that contains
        /// a mapping of the name and guid of the scanner and the name of
        /// the animation file for the scanner,
        /// Parses the config file and populates the map table with info
        /// from the file.
        /// </summary>
        /// <param name="configFileName">full path to the config file</param>
        private static void onScreenConfigFileFound(String configFileName)
        {
            try
            {
                var doc = new XmlDocument();

                doc.Load(configFileName);

                var configNodes = doc.SelectNodes("/ACAT/ConfigMapEntries/ConfigMapEntry");

                // load each scheme from the config file
                foreach (XmlNode node in configNodes)
                {
                    var mapEntry = new PanelConfigMapEntry();
                    if (mapEntry.Load(node))
                    {
                        addToMapTable(mapEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Found an XML file. Store the complete path to the file
        /// to the location map table
        /// </summary>
        /// <param name="xmlFileName">name of the xml file</param>
        private static void onXmlFileFound(String xmlFileName)
        {
            String fileName = Path.GetFileName(xmlFileName).ToLower();
            if (!_configFileLocationMap.ContainsKey(fileName))
            {
                Log.Debug("Adding xmlfile " + fileName + ", fullPath: " + xmlFileName);
                _configFileLocationMap.Add(fileName.ToLower(), xmlFileName);
            }
        }

        /// <summary>
        /// Found a DLL.  Load the class Types of all the relevant classes
        /// from the DLL
        /// </summary>
        /// <param name="dllName">name of the dll</param>
        private static void onDllFound(String dllName)
        {
            try
            {
                Log.Debug("Found dll " + dllName);
                Assembly screenAssembly = Assembly.LoadFile(dllName);
                loadTypesFromAssembly(screenAssembly);
            }
            catch (Exception ex)
            {
                Log.Debug("Could get types from assembly " + dllName + ". Exception : " + ex);
                if (ex is ReflectionTypeLoadException)
                {
                    var typeLoadException = (ReflectionTypeLoadException)ex;
                    var exceptions = typeLoadException.LoaderExceptions;
                    foreach (var e in exceptions)
                    {
                        Log.Debug("Loader exception: " + e);
                    }
                }
            }
        }

        /// <summary>
        /// Loads relevant types from the assembly and caches them
        /// </summary>
        /// <param name="assembly">name of the assembly</param>
        /// <returns>true on success</returns>
        private static bool loadTypesFromAssembly(Assembly assembly)
        {
            bool retVal = true;

            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    addTypeToCache(type);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Adds the specified type to the cache
        /// </summary>
        /// <param name="type">scanner class Type</param>
        private static void addTypeToCache(Type type)
        {
            if (typeof(IPanel).IsAssignableFrom(type))
            {
                var guid = GetFormId(type);
                if (guid != Guid.Empty)
                {
                    AddFormToCache(guid, type);
                }
            }
        }

        /// <summary>
        /// Looks up the maptable, find entries that have the
        /// specified guid and updates the class Type to the
        /// specified type
        /// </summary>
        /// <param name="guid">Scanner guid</param>
        /// <param name="type">Scanner Type</param>
        private static void updateFormTypeReferences(Guid guid, Type type)
        {
            foreach (var list in _mapTable.Values)
            {
                foreach (var panelConfigMapEntry in list)
                {
                    if (panelConfigMapEntry.FormType == null && panelConfigMapEntry.FormId.Equals(guid))
                    {
                        panelConfigMapEntry.FormType = type;
                    }
                }
            }
        }
    }
}