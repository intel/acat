////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement.Interfaces;
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

namespace ACAT.Core.UserControlManagement
{
    /// <summary>
    /// UserControlConfigMap is an xml file that contains a mapping between the
    /// user control and the name of the xml file that
    /// contains animation and other info for the user control.
    /// </summary>
    public class UserControlConfigMap
    {
        private static readonly ILogger<UserControlConfigMap> _logger = LogManager.GetLogger<UserControlConfigMap>();

        private const String DefaultKey = "panelconfigs";

        /// <summary>
        /// Name of the config file that has the mapping.  This is loaded from
        /// the user directory
        /// </summary>
        private const String UserControlConfigMapFileName = "usercontrolconfigmap.xml";

        /// <summary>
        /// Maps the name of a config file to the complete path of the file
        /// </summary>
        private static Dictionary<String, Dictionary<String, String>> _configFileLocationMap;

        private static Dictionary<String, List<Guid>> _ConfigIdMapTable;

        private static Dictionary<String, String> _loadConfigFileLocationMap;

        //private static String _loadCulture;

        private static List<Guid> _loadUserControlConfigMapTable;

        private static Dictionary<Guid, UserControlConfigMapEntry> _masterUserControlConfigMapTable;

        /// <summary>
        /// If one of the dll found has an error with the certificate
        /// </summary>
        private static volatile bool _DLLError = false;

        /// <summary>
        /// Caches the class Type of user controls
        /// </summary>
        private static Hashtable _userControlsCache;

        /// <summary>
        /// Adds the specified Type to the cache keyed by the Guid.
        /// </summary>
        /// <param name="guid">Guid for the usercontrol</param>
        /// <param name="type">Usercontrol class Type</param>
        public static void AddUserControlToCache(Guid guid, Type type)
        {
            if (_userControlsCache.ContainsKey(guid))
            {
                _logger?.LogDebug("Form Type {TypeName} with guid {Guid} is already added", type.FullName, guid);
                return;
            }

            _logger?.LogDebug("Adding form {TypeName} with guid {Guid} to cache", type.FullName, guid);
            _userControlsCache.Add(guid, type);

            updateUserControlTypeReferences(guid, type);
        }

        /// <summary>
        /// Checks if two usercontrols are the same
        /// </summary>
        /// <param name="panel1">first usercontrol</param>
        /// <param name="panel2">usercontrol to compare</param>
        /// <returns>true if they are</returns>
        public static bool AreEqual(String name1, String name2)
        {
            return String.Compare(name1, name2, true) == 0;
        }

        /// <summary>
        /// Returns the name of the animation config file for the specified
        /// usercontrol.  The GetPanelConfigMapEntry function first checks
        /// the culture folder (if non-English is the current culture)
        /// It it doesn't find it thre, it looks up
        /// the English culture folder
        /// </summary>
        /// <param name="panelClass">usercontrol name/class</param>
        /// <returns>the animation config file name</returns>
        public static String GetConfigFileForUserControl(String name)
        {
            var retVal = String.Empty;
            var mapEntry = GetUserControlConfigMapEntry(name);
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
        public static Guid GetConfigIdForConfigName(String configName)
        {
            foreach (var configMapEntry in _masterUserControlConfigMapTable.Values)
            {
                if (String.Compare(configName, configMapEntry.ConfigName, true) == 0)
                {
                    return configMapEntry.ConfigId;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Returns the config map for the specified usercontrol. Looks at the
        /// current culture and if not found , looks at English which is the
        /// default
        /// </summary>
        /// <param name="panel">Name of the usercontrol</param>
        /// <returns>Panel config map object</returns>
        public static UserControlConfigMapEntry GetUserControlConfigMapEntry(Guid guid)
        {
            return getConfigMapEntry(guid);
        }

        /// <summary>
        /// Returns the config map for the specified usercontrol. Looks at the
        /// current culture and if not found , looks at English which is the
        /// default
        /// </summary>
        /// <param name="panel">Name of the usercontrol</param>
        /// <returns>Panel config map object</returns>
        public static UserControlConfigMapEntry GetUserControlConfigMapEntry(String name)
        {
            return getConfigMapEntry(name);
        }

        /// <summary>
        /// Returns the config map entry for the specified config id
        /// </summary>
        /// <param name="configId">config id</param>
        /// <returns>the panel config map entry object</returns>
        public static UserControlConfigMapEntry GetUserControlconfigMapEntryForConfigId(Guid configId)
        {
            return _masterUserControlConfigMapTable.Values.FirstOrDefault(configMapEntry => Equals(configMapEntry.ConfigId, configId));
        }

        /// <summary>
        /// Gets the ACAT descriptor guid for the specifed Type
        /// </summary>
        /// <param name="type">usercontrol class Type</param>
        /// <returns>The descirptor guid</returns>
        public static Guid GetUserControlId(Type type)
        {
            var descAttribute = ClassDescriptorAttribute.GetDescriptor(type);
            Guid retVal = Guid.Empty;
            if (descAttribute != null)
            {
                retVal = descAttribute.Id;
            }

            return retVal;
        }


        /// <summary>
        /// Walks the directories specified in extensionDir,
        /// looks for DLL's, loads all the types and looks for
        /// Types that are derived from IUserControl (which is all the
        /// usercontrols) and caches them
        /// </summary>
        /// <param name="extensionDirs">Directories to look</param>
        /// <returns>true on success</returns>
        public static bool Load(IEnumerable<String> extensionDirs)
        {
            _masterUserControlConfigMapTable = new Dictionary<Guid, UserControlConfigMapEntry>();

            _ConfigIdMapTable = new Dictionary<string, List<Guid>>();

            _configFileLocationMap = new Dictionary<string, Dictionary<string, string>>();

            _userControlsCache = new Hashtable();

            _loadUserControlConfigMapTable = new List<Guid>();

            _loadConfigFileLocationMap = new Dictionary<string, string>();

            // first walk the extension directories
            foreach (string dir in extensionDirs)
            {
                load(dir, "ACAT*.dll");
                if (_DLLError)
                    return false;
            }

            // load the usercontrolconfigmap.xml file
            var usercontrolConfigMapFile = Path.Combine(FileUtils.GetPanelConfigDir(), UserControlConfigMapFileName);
            LoadUserControlConfigMap(usercontrolConfigMapFile);

            var configsDir = FileUtils.GetPanelConfigDir();
            _logger?.LogDebug("Loading resources from {ConfigsDir}", configsDir);

            // load the panels from the default culture (which is English)
            var resourcesDir = Path.Combine(FileUtils.GetPanelConfigDir(), "common");
            _logger?.LogDebug("Default resources directory: {ResourcesDir}", resourcesDir);
            load(resourcesDir, "*.xml");

            // Also pick up any overrides for the current culture
            resourcesDir = Path.Combine(FileUtils.GetPanelConfigDir(), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            _logger?.LogDebug("Culture-specific resources directory: {ResourcesDir}", resourcesDir);
            load(resourcesDir, "*.xml");

            _ConfigIdMapTable.Add(DefaultKey, _loadUserControlConfigMapTable);
            _configFileLocationMap.Add(DefaultKey, _loadConfigFileLocationMap);

            return true;
        }

        public static void Reset()
        {
            if (_ConfigIdMapTable != null)
            {
                _ConfigIdMapTable.Clear();
                _ConfigIdMapTable = null;
            }

            if (_masterUserControlConfigMapTable != null)
            {
                _masterUserControlConfigMapTable.Clear();
                _masterUserControlConfigMapTable = null;
            }

            if (_configFileLocationMap != null)
            {
                _configFileLocationMap.Clear();
                _configFileLocationMap = null;
            }

            if (_userControlsCache != null)
            {
                _userControlsCache.Clear();
                _userControlsCache = null;
            }
        }

        /// <summary>
        /// Cleans up the map tables of entries that are orphans. These
        /// are forms that don't have a corresponding animatinon file
        /// </summary>
        internal static void CleanupOrphans()
        {
            _logger?.LogDebug("Cleaning up userControlConfigMap entries");

            var removeList = new List<UserControlConfigMapEntry>();
            foreach (var mapEntry in _masterUserControlConfigMapTable.Values)
            {
                _logger?.LogDebug("Looking up entry: {Entry}", mapEntry.ToString());
                if (_userControlsCache.ContainsKey(mapEntry.UserControlId))
                {
                    mapEntry.UserControlType = (Type)_userControlsCache[mapEntry.UserControlId];
                }

                _logger?.LogTrace("UserControlType is null: {IsNull}", mapEntry.UserControlType == null);

                var configFilePath = getConfigFilePathFromLocationMap(mapEntry.ConfigFileName);

                if (mapEntry.UserControlType != null && !String.IsNullOrEmpty(configFilePath))
                {
                    _logger?.LogDebug("Found config file {ConfigFileName} in location map", mapEntry.ConfigFileName);
                    mapEntry.ConfigFileName = configFilePath;
                }
                else
                {
                    _logger?.LogDebug("Config file {ConfigFileName} not found in location map - marking for removal", mapEntry.ConfigFileName);
                    removeList.Add(mapEntry);
                }
            }

            foreach (var panelConfigMapEntry in removeList)
            {
                removeMapEntry(panelConfigMapEntry);
            }
        }

        /// <summary>
        /// Adds the specified mapEntry object to the map table. Also
        /// looks up the map table if it already has the usercontrol ID specified
        /// in the mapEntry and updates its config file name with the
        /// one in mapEntry
        /// </summary>
        /// <param name="mapTable">Table to add to</param>
        /// <param name="mapEntry">map entry to add</param>
        private static void addToMapTable(List<Guid> configIdTable, UserControlConfigMapEntry mapEntry)
        {
            if (!configIdTable.Contains(mapEntry.ConfigId))
            {
                configIdTable.Add(mapEntry.ConfigId);
            }

            if (!_masterUserControlConfigMapTable.ContainsKey(mapEntry.ConfigId))
            {
                _masterUserControlConfigMapTable.Add(mapEntry.ConfigId, mapEntry);
            }
        }

        /// <summary>
        /// Adds the specified type to the cache
        /// </summary>
        /// <param name="type">scanner class Type</param>
        private static void addUserControlTypeToCache(Type type)
        {
            if (typeof(IUserControl).IsAssignableFrom(type))
            {
                var guid = GetUserControlId(type);
                if (guid != Guid.Empty)
                {
                    AddUserControlToCache(guid, type);
                }
            }
        }

        /// <summary>
        /// Returns the fullpath to the config file for the specified culture
        /// </summary>
        /// <param name="language">culture</param>
        /// <param name="configFile">config file</param>
        /// <returns>full path, empty if not found</returns>
        private static String getConfigFilePathFromLocationMap(String configFile)
        {
            if (_configFileLocationMap.ContainsKey(DefaultKey))
            {
                var map = _configFileLocationMap[DefaultKey];

                if (map.ContainsKey(configFile))
                {
                    return map[configFile];
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns the panelconfigmapentry for the specified usercontrol class
        /// for the specified language
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="panelClass">panel class</param>
        /// <returns>object, null if not found</returns>
        private static UserControlConfigMapEntry getConfigMapEntry(String name)
        {
            if (!_ConfigIdMapTable.ContainsKey(DefaultKey))
            {
                return null;
            }

            List<Guid> configIds = _ConfigIdMapTable[DefaultKey];

            foreach (var configId in configIds)
            {
                if (_masterUserControlConfigMapTable.ContainsKey(configId))
                {
                    var userControlConfigMapEntry = _masterUserControlConfigMapTable[configId];
                    if (String.Compare(userControlConfigMapEntry.Name, name, true) == 0)
                    {
                        return userControlConfigMapEntry;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the panelconfigmapentry for the specified usercontrol class
        /// for the specified language
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="panelClass">panel class</param>
        /// <returns>object, null if not found</returns>
        private static UserControlConfigMapEntry getConfigMapEntry( Guid guid)
        {
            if (!_ConfigIdMapTable.ContainsKey(DefaultKey))
            {
                return null;
            }

            List<Guid> configIds = _ConfigIdMapTable[DefaultKey];

            foreach (var configId in configIds)
            {
                if (_masterUserControlConfigMapTable.ContainsKey(configId))
                {
                    var userControlConfigMapEntry = _masterUserControlConfigMapTable[configId];
                    if (userControlConfigMapEntry.UserControlId == guid)
                    {
                        return userControlConfigMapEntry;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Walks the specified directory (rescursively)
        /// to look for files
        /// </summary>
        /// <param name="dir">Directory to walk</param>
        /// <param name="recursive">Recursively search?</param>
        private static void load(String dir, string wildcard)
        {
            if (Directory.Exists(dir) && !_DLLError)
            {
                var walker = new DirectoryWalker(dir, wildcard);
                _logger?.LogDebug("Walking directory {Directory}", dir);
                walker.Walk(new OnFileFoundDelegate(onFileFound));
            }
        }


        /// <summary>
        /// Found a DLL.  Load the class Types of all the relevant classes
        /// from the DLL
        /// </summary>
        /// <param name="dllName">name of the dll</param>
        private static void onDllFound(String dllName)
        {
            TypeLoader<IUserControl> typeLoader = new();

            try
            {
                _logger?.LogDebug("Found dll {DllName}", dllName);

                typeLoader.LoadFromAssembly(dllName, false);

                foreach (var type in typeLoader.LoadedTypes.Values)
                {
                    _logger?.LogDebug("Found type {TypeName}", type.FullName);
                    addUserControlTypeToCache(type);
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
        /// Callback function for the directory walker that's invoked
        /// when a file is found.  Checks the file is a dll or an
        /// xml file and handles them appropriately
        /// </summary>
        /// <param name="file">name of the file found</param>
        private static void onFileFound(String file)
        {
            String filePath = file.ToLower();
            String fileName = Path.GetFileName(filePath);

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

        /// <summary>
        /// Found the panel config file. This is the xml file that contains
        /// a mapping of the name and guid of the usercontrol and the name of
        /// the animation file for the usercontrol,
        /// Parses the config file and populates the map table with info
        /// from the file.
        /// </summary>
        /// <param name="configFileName">full path to the config file</param>
        private static void LoadUserControlConfigMap(String configFileName)
        {
            try
            {
                var doc = new XmlDocument();

                doc.Load(configFileName);

                var configNodes = doc.SelectNodes("/ACAT/UserControlConfigMapEntries/UserControlConfigMapEntry");

                // load each scheme from the config file
                foreach (XmlNode node in configNodes)
                {
                    var mapEntry = new UserControlConfigMapEntry();
                    if (mapEntry.Load(node))
                    {
                        addToMapTable(_loadUserControlConfigMapTable, mapEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading user control config map");
            }
        }

        /// <summary>
        /// Found an XML file. Store the complete path to the file
        /// to the location map table
        /// </summary>
        /// <param name="xmlFileName">name of the xml file</param>
        private static void onXmlFileFound(String xmlFileName)
        {
            string fileName = Path.GetFileName(xmlFileName).ToLower();

            if (_loadConfigFileLocationMap.ContainsKey(fileName))
            {
                _logger?.LogDebug("Updating xml file {FileName}, full path: {FullPath}", fileName, xmlFileName);
                _loadConfigFileLocationMap[fileName] = xmlFileName;
            }
            else
            {
                _logger?.LogDebug("Adding xml file {FileName}, full path: {FullPath}", fileName, xmlFileName);
                _loadConfigFileLocationMap.Add(fileName, xmlFileName);
            }
        }

        /// <summary>
        /// Removes the specified map entry from the map table
        /// </summary>
        /// <param name="entryToRemove">entry to remove</param>
        private static void removeMapEntry(UserControlConfigMapEntry entryToRemove)
        {
            if (_masterUserControlConfigMapTable.ContainsKey(entryToRemove.ConfigId))
            {
                _masterUserControlConfigMapTable.Remove(entryToRemove.ConfigId);
            }
        }

        /// <summary>
        /// Looks up the maptable, find entries that have the
        /// specified guid and updates the class Type to the
        /// specified type
        /// </summary>
        /// <param name="guid">Scanner guid</param>
        /// <param name="type">Scanner Type</param>
        private static void updateUserControlTypeReferences(Guid guid, Type type)
        {
            foreach (var configMapEntry in _masterUserControlConfigMapTable.Values)
            {
                if (configMapEntry.UserControlType == null && configMapEntry.UserControlId.Equals(guid))
                {
                    configMapEntry.UserControlType = type;
                }
            }
        }
    }
}