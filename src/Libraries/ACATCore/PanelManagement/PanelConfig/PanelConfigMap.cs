// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0

using ACAT.Core.AgentManagement;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.Utility.TypeLoader;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ACAT.Core.PanelManagement.PanelConfig
{
    /// <summary>
    /// PanelConfigMap is an xml file that contains a mapping between the
    /// name of the class for the scanner and the name of the xml file that
    /// contains animation and other info for the scanner.  This
    /// allows for mapping different animation files to the same scanner (form).
    /// For instance, a QWERTY layout alphabet scanner in English can have a different
    /// layout of letters for another language like French.
    /// </summary>
    public class PanelConfigMap
    {
        private static readonly ILogger<PanelConfigMap> _logger = LogManager.GetLogger<PanelConfigMap>();

        /// <summary>
        /// Name of the panel class config file. This file contains a
        /// list of panel configurations to use
        /// </summary>
        public static string PanelClassConfigFileName = "panelclassconfig.xml";

        private const string DefaultKey = "panelconfigs";
        /// <summary>
        /// Name of the config file that has the mapping.  This is loaded from
        /// the user directory
        /// </summary>
        private const string PanelConfigMapFileName = "panelconfigmap.xml";

        /// <summary>
        /// Maps the name of a config file to the complete path of the file
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> _configFileLocationMap;

        private static Dictionary<string, AppPanelClassConfig> _AppPanelClassConfig;
        private static Dictionary<string, List<Guid>> _ConfigIdMapTable;
        private static Dictionary<string, PanelClassConfig> _PanelClassConfigMapTable;
        private static AppPanelClassConfig _currentAppPanelClassConfig = null;
        private static volatile bool _DLLError = false;
        /// <summary>
        /// Caches the class Type of forms
        /// </summary>
        private static Hashtable _formsCache;

        private static Dictionary<string, string> _loadConfigFileLocationMap;

        private static List<Guid> _loadPanelConfigMapTable;
        private static Dictionary<Guid, PanelConfigMapEntry> _masterPanelConfigMapTable;

        /// <summary>
        /// Add a new entry to the PanelClassConfig and save the file
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="language"></param>
        /// <param name="panelClassConfigMap"></param>
        /// <returns></returns>
        public static bool AddPanelClassConfigMap(string appId, PanelClassConfigMap panelClassConfigMap)
        {
            var panelClassConfigFilePath = GetOrCreateUserPanelClassConfigFile();

            if (File.Exists(panelClassConfigFilePath))
            {
                var appPanelClassConfig = AppPanelClassConfig.Load(panelClassConfigFilePath);

                PanelClassConfig panelClassConfig = appPanelClassConfig.Find(appId);

                if (panelClassConfig != null)
                {
                    PanelClassConfigMap result = panelClassConfig.PanelClassConfigMaps.Find(mapEntry => string.Compare(mapEntry.Name, panelClassConfigMap.Name, true) == 0);

                    if (result != null)
                    {
                        return false;
                    }

                    panelClassConfig.PanelClassConfigMaps.Add(panelClassConfigMap);
                    appPanelClassConfig.Save();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if two scanners are the same
        /// </summary>
        /// <param name="panel1">first scanner</param>
        /// <param name="panel2">scanner to compare</param>
        /// <returns>true if they are</returns>
        public static bool AreEqual(string panel1, string panel2)
        {
            return string.Compare(panel1, panel2, true) == 0;
        }

        /// <summary>
        /// Returns the name of the animation config file for the specified
        /// scanner.  The GetPanelConfigMapEntry function first checks
        /// the culture folder (if non-English is the current culture)
        /// It it doesn't find it thre, it looks up
        /// the English culture folder
        /// </summary>
        /// <param name="panelClass">scanner name/class</param>
        /// <returns>the animation config file name</returns>
        public static string GetConfigFileForPanel(string panelClass)
        {
            var retVal = string.Empty;
            PanelConfigMapEntry mapEntry = GetPanelConfigMapEntry(panelClass);
            if (mapEntry != null)
            {
                retVal = mapEntry.ConfigFileName;
            }

            return retVal;
        }

        /// <summary>
        /// Returns the config id for the specified config name.  Returns
        /// empty guid if not found.
        /// </summary>
        /// <param name="configName">the config name</param>
        /// <returns>config id</returns>
        public static Guid GetConfigIdForConfigName(string configName)
        {
            foreach (PanelConfigMapEntry panelConfigMapEntry in _masterPanelConfigMapTable.Values)
            {
                if (string.Compare(configName, panelConfigMapEntry.ConfigName, true) == 0)
                {
                    return panelConfigMapEntry.ConfigId;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Returns the default panelclasssconfig map for the app.
        /// First checks the culture folder (for non-English) and
        /// if it doesn't find it ther, looks up the default English
        /// culture folder
        /// </summary>
        /// <returns>default panelclassconfigmap, null if not found</returns>
        ///
        public static PanelClassConfigMap GetDefaultPanelClassConfigMap()
        {
            if (_PanelClassConfigMapTable.TryGetValue(DefaultKey, out PanelClassConfig panelClassConfig))
            {
                _currentAppPanelClassConfig = _AppPanelClassConfig[DefaultKey];
            }

            return panelClassConfig?.GetDefaultClassConfigMap();
        }

        public static PanelClassConfig GetPanelClassConfigForApp()
        {
            if (_PanelClassConfigMapTable.TryGetValue(DefaultKey, out PanelClassConfig panelClassConfig))
            {
                _currentAppPanelClassConfig = _AppPanelClassConfig[DefaultKey];
            }

            return panelClassConfig;
        }

        /// <summary>
        /// Returns the config map for the specified scanner. Looks at the
        /// current culture and if not found , looks at English which is the
        /// default
        /// </summary>
        /// <param name="panel">Name of the scanner</param>
        /// <returns>Panel config map object</returns>
        public static PanelConfigMapEntry GetPanelConfigMapEntry(string panel)
        {
            PanelConfigMapEntry retVal = getMapEntryFromPanelClassConfigMap(panel);

            if (retVal == null)
            {
                retVal = getConfigMapEntry(panel);
            }

            return retVal;
        }

        public static PanelConfigMapEntry GetPanelConfigMapEntryForConfig(string configName)
        {
            return _masterPanelConfigMapTable.Values.FirstOrDefault(PanelConfigMapEntry => Equals(PanelConfigMapEntry.ConfigName, configName));
        }

        /// <summary>
        /// Returns the config map entry for the specified config id
        /// </summary>
        /// <param name="configId">config id</param>
        /// <returns>the panel config map entry object</returns>
        public static PanelConfigMapEntry GetPanelConfigMapEntryForConfigId(Guid configId)
        {
            return _masterPanelConfigMapTable.Values.FirstOrDefault(panelConfigMapEntry => Equals(panelConfigMapEntry.ConfigId, configId));
        }
        /// <summary>
        /// Walks the directories specified in extensionDir,
        /// looks for DLL's, loads all the types and looks for
        /// Types that are derived from IPanel (which is all the
        /// scanners) and caches them
        /// </summary>
        /// <param name="extensionDirs">Directories to look</param>
        /// <returns>true on success</returns>
        public static bool Load(IEnumerable<string> extensionDirs)
        {
            _PanelClassConfigMapTable = new Dictionary<string, PanelClassConfig>();

            _masterPanelConfigMapTable = new Dictionary<Guid, PanelConfigMapEntry>();

            LoadPanelClassConfig();

            _ConfigIdMapTable = new Dictionary<string, List<Guid>>();
            _configFileLocationMap = new Dictionary<string, Dictionary<string, string>>();
            _AppPanelClassConfig = new Dictionary<string, AppPanelClassConfig>();
            _formsCache = new Hashtable();

            _loadPanelConfigMapTable = new List<Guid>();
            _loadConfigFileLocationMap = new Dictionary<string, string>();

            // Load Agents from the Agent Manager Instance
            IEnumerable<object> agents = AgentManager.Instance.GetExtensions();
            foreach (IApplicationAgent agent in agents.Cast<IApplicationAgent>())
            {
                _logger?.LogTrace("Loading agent {Agent}", agent);
                addTypeToCache(agent.GetType());
            }

            // first walk the extension directories
            foreach (string dir in extensionDirs)
            {
                string extensionDir = dir + "\\" + PanelManager.UiRootDir;
                LoadTypesFromExtensions(extensionDir, onDllFound, "ACAT*.dll");
                if (_DLLError)
                    return false;
            }

            var configsDir = FileUtils.GetPanelConfigDir();
            _logger?.LogDebug("Loading resources from {ConfigsDir}", configsDir);

            if (Directory.Exists(Path.Combine(configsDir, "common")))
            {
                _loadPanelConfigMapTable ??= new List<Guid>();
                _loadConfigFileLocationMap ??= new Dictionary<string, string>();

                LoadResourcesFromDir(configsDir);
            }
            if (Directory.Exists(Path.Combine(configsDir, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)))
            {
                _loadPanelConfigMapTable ??= new List<Guid>();
                _loadConfigFileLocationMap ??= new Dictionary<string, string>();

                LoadResourcesFromDir(configsDir);
            }

            _ConfigIdMapTable.Add(DefaultKey, _loadPanelConfigMapTable);
            _configFileLocationMap.Add(DefaultKey, _loadConfigFileLocationMap);
            return true;
        }

        /// <summary>
        /// Loads class Types from the specified assembly
        /// </summary>
        /// <param name="assembly">Assembly to load from</param>
        /// <returns>true on success</returns>
        public static bool Load(Assembly assembly)
        {
            _formsCache ??= new Hashtable();

            return loadTypesFromAssembly(assembly);
        }

        //public static void LoadPanelClassConfig()
        //{
        //    loadPanelClassConfig();
        //}

        public static void Reset()
        {
            if (_ConfigIdMapTable != null)
            {
                _ConfigIdMapTable.Clear();
                _ConfigIdMapTable = null;
            }

            if (_masterPanelConfigMapTable != null)
            {
                _masterPanelConfigMapTable.Clear();
                _masterPanelConfigMapTable = null;
            }

            if (_configFileLocationMap != null)
            {
                _configFileLocationMap.Clear();
                _configFileLocationMap = null;
            }

            if (_PanelClassConfigMapTable != null)
            {
                _PanelClassConfigMapTable.Clear();
                _PanelClassConfigMapTable = null;
            }

            if (_AppPanelClassConfig != null)
            {
                _AppPanelClassConfig.Clear();
                _AppPanelClassConfig = null;
            }

            if (_loadConfigFileLocationMap != null)
            {
                _loadConfigFileLocationMap.Clear();
                _loadConfigFileLocationMap = null;
            }

            if (_loadPanelConfigMapTable != null)
            {
                _loadPanelConfigMapTable.Clear();
                _loadPanelConfigMapTable = null;
            }

            if (_formsCache != null)
            {
                _formsCache.Clear();
                _formsCache = null;
            }
        }

        public static bool SavePanelClassConfig()
        {
            return _currentAppPanelClassConfig != null && _currentAppPanelClassConfig.Save();
        }

        /// <summary>
        /// Sets the specified config name as the default config name
        /// for the app.
        /// </summary>
        /// <param name="configName">name of the config</param>
        /// <returns>true on success</returns>
        public static bool SetDefaultPanelConfig(string configName)
        {
            bool retVal = true;

            if (string.IsNullOrEmpty(configName))
            {
                return false;
            }

            if ( _PanelClassConfigMapTable.TryGetValue(DefaultKey, out PanelClassConfig panelClassConfig))
            {
                retVal = panelClassConfig.SetDefaultClassConfigMap(configName);
            }

            return retVal;
        }
        /// <summary>
        /// Adds the specified Type to the cache keyed by the Guid.
        /// </summary>
        /// <param name="guid">Guid for the scanner</param>
        /// <param name="type">Scanner class Type</param>
        internal static void AddFormToCache(Guid guid, Type type)
        {
            if (_formsCache.ContainsKey(guid))
            {
                _logger?.LogTrace("Form Type {TypeName} with guid {Guid} is already added", type.FullName, guid);
                return;
            }

            _logger?.LogTrace("Adding form {TypeName} with guid {Guid} to cache", type.FullName, guid);
            _formsCache.Add(guid, type);

            updateFormTypeReferences(guid, type);
        }

        /// <summary>
        /// Cleans up the map tables of entries that are orphans. These
        /// are forms that don't have a corresponding animation file
        /// </summary>
        internal static void CleanupOrphans()
        {
            _logger?.LogDebug("Cleaning up panelConfigMap entries");

            var removeList = new List<PanelConfigMapEntry>();
            foreach (PanelConfigMapEntry mapEntry in _masterPanelConfigMapTable.Values)
            {
                _logger?.LogTrace("Looking up entry: {Entry}", mapEntry.ToString());
                if (_formsCache.ContainsKey(mapEntry.FormId))
                {
                    mapEntry.FormType = (Type)_formsCache[mapEntry.FormId];
                }

                _logger?.LogTrace("FormType is null: {IsNull}", mapEntry.FormType == null);

                var configFilePath = getConfigFilePathFromLocationMap(mapEntry.ConfigFileName);

                if (mapEntry.FormType != null && !string.IsNullOrEmpty(configFilePath))
                {
                    _logger?.LogTrace("Found config file {ConfigFileName} in location map", mapEntry.ConfigFileName);
                    mapEntry.ConfigFileName = configFilePath;
                }
                else
                {
                    _logger?.LogTrace("Config file {ConfigFileName} not found in location map - marking for removal", mapEntry.ConfigFileName);
                    removeList.Add(mapEntry);
                }
            }

            foreach (PanelConfigMapEntry panelConfigMapEntry in removeList)
            {
                removeMapEntry(panelConfigMapEntry);
            }
        }

        /// <summary>
        /// Gets the ACAT descriptor guid for the specifed Type
        /// </summary>
        /// <param name="type">Scanner class Type</param>
        /// <returns>The descirptor guid</returns>
        internal static Guid GetFormId(Type type)
        {
            Guid retVal = Guid.Empty;

            var descAttribute = ClassDescriptorAttribute.GetDescriptor(type);
            if (descAttribute != null)
            {
                retVal = descAttribute.Id;
            }
            else
            {
                if (type.GUID != null && type.GUID != Guid.Empty)
                {
                    retVal = type.GUID;
                }
                else
                {
                    _logger?.LogError("Type {TypeName} does not have a valid ACAT descriptor guid", type.FullName);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Adds the specified mapEntry object to the map table. Also
        /// looks up the map table if it already has the formID specified
        /// in the mapEntry and updates its config file name with the
        /// one in mapEntry
        /// </summary>
        /// <param name="mapTable">Table to add to</param>
        /// <param name="mapEntry">map entry to add</param>
        private static void addToMapTable(List<Guid> configIdTable, PanelConfigMapEntry mapEntry)
        {
            if (!configIdTable.Contains(mapEntry.ConfigId))
            {
                configIdTable.Add(mapEntry.ConfigId);
            }

            if (!_masterPanelConfigMapTable.ContainsKey(mapEntry.ConfigId))
            {
                _masterPanelConfigMapTable.Add(mapEntry.ConfigId, mapEntry);
            }
        }

        /// <summary>
        /// Adds the specified type to the cache
        /// </summary>
        /// <param name="type">scanner class Type</param>
        private static void addTypeToCache(Type type)
        {
            if (typeof(IPanel).IsAssignableFrom(type))
            {
                Guid guid = GetFormId(type);

                if (guid != Guid.Empty)
                {
                    AddFormToCache(guid, type);
                }
            }
        }

        /// <summary>
        /// Returns the panelClassConfigMapEntry object for the specified panel
        /// and for the specifed culture
        /// </summary>
        /// <param name="language">the culture</param>
        /// <param name="panelClass">panel class</param>
        /// <returns>object, null if not found</returns>
        private static PanelClassConfigMapEntry getClassConfigMapEntry(string panelClass)
        {
            if (_PanelClassConfigMapTable.ContainsKey(DefaultKey))
            {
                PanelClassConfig panelClassConfig = _PanelClassConfigMapTable[DefaultKey];

                return panelClassConfig.GetDefaultClassConfigMapEntry(panelClass);
            }

            return null;
        }

        /// <summary>
        /// Returns the fullpath to the config file for the specified culture
        /// </summary>
        /// <param name="language">culture</param>
        /// <param name="configFile">config file</param>
        /// <returns>full path, empty if not found</returns>
        private static string getConfigFilePathFromLocationMap(string configFile)
        {
            if (_configFileLocationMap.ContainsKey(DefaultKey))
            {
                Dictionary<string, string> map = _configFileLocationMap[DefaultKey];

                if (map.ContainsKey(configFile))
                {
                    return map[configFile];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the panelconfigmapentry for the specified panel class
        /// for the specified language
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="panelClass">panel class</param>
        /// <returns>object, null if not found</returns>
        private static PanelConfigMapEntry getConfigMapEntry(string panelClass)
        {
            if (!_ConfigIdMapTable.ContainsKey(DefaultKey))
            {
                return null;
            }

            List<Guid> configIds = _ConfigIdMapTable[DefaultKey];

            foreach (Guid configId in configIds)
            {
                if (_masterPanelConfigMapTable.ContainsKey(configId))
                {
                    PanelConfigMapEntry panelConfigMapEntry = _masterPanelConfigMapTable[configId];
                    if (string.Compare(panelConfigMapEntry.PanelClass, panelClass, true) == 0)
                    {
                        return panelConfigMapEntry;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the PanelConfigMapEntry for the specified panel. Looks at
        /// the current culture first and then the default (English) culture
        /// </summary>
        /// <param name="panelClass">panel class</param>
        /// <returns>the object, null if not found</returns>
        private static PanelConfigMapEntry getMapEntryFromPanelClassConfigMap(string panelClass)
        {
            PanelClassConfigMapEntry entry = getClassConfigMapEntry(panelClass);

            if (entry == null)
            {
                return null;
            }

            Guid configId = entry.ConfigId;

            return _masterPanelConfigMapTable.ContainsKey(configId)
                            ? _masterPanelConfigMapTable[configId]
                            : null;
        }

        private static string GetOrCreateUserPanelClassConfigFile()
        {
            var userPath = Path.Combine(UserManager.CurrentUserDir, PanelClassConfigFileName);
            var commonDir = FileUtils.GetPanelConfigDir();
            var commonPath = Path.Combine(commonDir, PanelClassConfigFileName);

            // Ensure user directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(userPath)!);

            // Copy from common dir on first run
            if (!File.Exists(userPath) && File.Exists(commonPath))
            {
                File.Copy(commonPath, userPath, overwrite: false);
            }

            return userPath;
        }


        /// <summary>
        /// For this application, load the panel configurations to use from
        /// the panelclassconfig.xml file
        /// </summary>
        /// <param name="language"></param>
        public static void LoadPanelClassConfig()
        {
            _AppPanelClassConfig ??= new Dictionary<string, AppPanelClassConfig>();

            _PanelClassConfigMapTable ??= new Dictionary<string, PanelClassConfig>();

            var panelClassConfigFilePath = GetOrCreateUserPanelClassConfigFile();

            if (File.Exists(panelClassConfigFilePath) && !_PanelClassConfigMapTable.ContainsKey(DefaultKey))
            {
                var appPanelClassConfig = AppPanelClassConfig.Load(panelClassConfigFilePath);
                _AppPanelClassConfig[DefaultKey] = appPanelClassConfig;

                PanelClassConfig panelClassConfig = appPanelClassConfig.Find(CoreGlobals.AppId);

                if (panelClassConfig != null && panelClassConfig.PanelClassConfigMaps.Count > 0)
                {
                    _PanelClassConfigMapTable[DefaultKey] = panelClassConfig;
                }
            }
        }

        private static void LoadResourcesFromDir(string dirName)
        {
            DirectoryWalker walker = new(dirName, "*.xml");
            walker.Walk(onXmlFileFound);
        }

        /// <summary>
        /// Loads relevant types from the assembly and caches them
        /// </summary>
        /// <param name="assembly">name of the assembly</param>
        /// <returns>true on success</returns>
        private static bool loadTypesFromAssembly(Assembly assembly)
        {
            bool retVal = true;

            if (assembly == null)
            {
                return false;
            }

            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    addTypeToCache(type);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading types from assembly");
                retVal = false;
            }

            return retVal;
        }

        private static void LoadTypesFromExtensions(string dir, OnFileFoundDelegate founddelegate, string wildcard)
        {
            var walker = new DirectoryWalker(dir, wildcard);
            walker.Walk(founddelegate);
        }
        /// <summary>
        /// Found a DLL.  Load the class Types of all the relevant classes
        /// from the DLL
        /// </summary>
        /// <param name="dllName">name of the dll</param>
        private static void onDllFound(string dllName)
        {
            // Skip resource assemblies (satellite assemblies for localization)
            // These are automatically loaded by .NET and should not be loaded directly
            string fileName = Path.GetFileName(dllName);
            if (fileName.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            TypeLoader<IPanel> typeLoader = new();

            try
            {
                _logger?.LogTrace("Found dll {DllName}", dllName);

                typeLoader.LoadFromAssembly(dllName, false);

                foreach (Type type in typeLoader.LoadedTypes.Values)
                {
                    _logger?.LogTrace("Found type {TypeName}", type.FullName);
                    addTypeToCache(type);
                }
            }

            catch (BadImageFormatException ex)
            {
                _logger?.LogError(ex, "Error loading dll {DllName}", dllName);
                //_DLLError = true;
            }

            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading dll {DllName}", dllName);
                _DLLError = true;
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
        private static void onPanelConfigMapFileFound(string configFileName)
        {
            try
            {
                var doc = new XmlDocument();

                doc.Load(configFileName);

                XmlNodeList configNodes = doc.SelectNodes("/ACAT/ConfigMapEntries/ConfigMapEntry");

                // load each scheme from the config file
                foreach (XmlNode node in configNodes)
                {
                    var mapEntry = new PanelConfigMapEntry();
                    if (mapEntry.Load(node))
                    {
                        addToMapTable(_loadPanelConfigMapTable, mapEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading panel config map");
            }
        }
        /// <summary>
        /// Found an XML file. Store the complete path to the file
        /// to the location map table
        /// </summary>
        /// <param name="xmlFileName">name of the xml file</param>
        private static void onXmlFileFound(string xmlFileName)
        {
            string filePath = xmlFileName.ToLower();
            string fileName = Path.GetFileName(filePath);

            if (string.Equals(fileName, PanelConfigMapFileName, StringComparison.OrdinalIgnoreCase))
            {
                onPanelConfigMapFileFound(filePath);
            }

            if (_loadConfigFileLocationMap.ContainsKey(fileName))
            {
                _logger?.LogTrace("Updating xml file {FileName}, full path: {FullPath}", fileName, xmlFileName);
                _loadConfigFileLocationMap[fileName] = xmlFileName;
            }
            else
            {
                _logger?.LogTrace("Adding xml file {FileName}, full path: {FullPath}", fileName, xmlFileName);
                _loadConfigFileLocationMap.Add(fileName, xmlFileName);
            }
        }

        /// <summary>
        /// Removes the specified map entry from the map table
        /// </summary>
        /// <param name="entryToRemove">entry to remove</param>
        private static void removeMapEntry(PanelConfigMapEntry entryToRemove)
        {
            if (_masterPanelConfigMapTable.ContainsKey(entryToRemove.ConfigId))
            {
                _masterPanelConfigMapTable.Remove(entryToRemove.ConfigId);
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
            foreach (PanelConfigMapEntry panelConfigMapEntry in _masterPanelConfigMapTable.Values)
            {
                if (panelConfigMapEntry.FormType == null && panelConfigMapEntry.FormId.Equals(guid))
                {
                    panelConfigMapEntry.FormType = type;
                }
            }
        }
    }
}