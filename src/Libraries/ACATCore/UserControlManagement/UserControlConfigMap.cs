////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ACAT.Lib.Core.UserControlManagement
{
    /// <summary>
    /// UserControlConfigMap is an xml file that contains a mapping between the
    /// user control and the name of the xml file that
    /// contains animation and other info for the user control. 
    /// </summary>
    public class UserControlConfigMap
    {
        // TODO - Localize Me
        private const String DefaultCulture = "en";

        /// <summary>
        /// Name of the config file that has the mapping.  This is loaded from
        /// the user directory
        /// </summary>
        private const String UserControlConfigMapFileName = "usercontrolconfigmap.xml";

        /// <summary>
        /// Maps the name of a config file to the complete path of the file
        /// </summary>
        private static Dictionary<String, Dictionary<String, String>> _configFileLocationMap;

        private static Dictionary<String, List<Guid>> _cultureConfigIdMapTable;

        private static Dictionary<String, String> _loadConfigFileLocationMap;

        private static String _loadCulture;

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
                Log.Debug("Form Type " + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding form " + type.FullName + ", guid " + guid + " to cache");
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
            var retVal = getCultureConfigMapEntry(CultureInfo.DefaultThreadCurrentUICulture.Name, guid);
            if (retVal == null)
            {
                retVal = getCultureConfigMapEntry(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, guid);
            }

            if (retVal == null)
            {
                retVal = getCultureConfigMapEntry(DefaultCulture, guid);
            }

            return retVal;
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
            var retVal = getCultureConfigMapEntry(CultureInfo.DefaultThreadCurrentUICulture.Name, name);
            if (retVal == null)
            {
                retVal = getCultureConfigMapEntry(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, name);
            }

            if (retVal == null)
            {
                retVal = getCultureConfigMapEntry(DefaultCulture, name);
            }

            return retVal;
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
            var descAttribute = DescriptorAttribute.GetDescriptor(type);
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

            _cultureConfigIdMapTable = new Dictionary<string, List<Guid>>();

            _configFileLocationMap = new Dictionary<string, Dictionary<string, string>>();

            _userControlsCache = new Hashtable();

            _loadCulture = DefaultCulture;

            _loadUserControlConfigMapTable = new List<Guid>();

            _loadConfigFileLocationMap = new Dictionary<string, string>();

            // first walk the extension directories
            foreach (string dir in extensionDirs)
            {
                //String extensionDir = dir + "\\" + AgentManager.AppAgentsRootDir;
                //load(extensionDir);

                //extensionDir = dir + "\\" + AgentManager.FunctionalAgentsRootDir;
                //load(extensionDir);

                String extensionDir = dir + "\\" + PanelManager.UiRootDir;
                load(extensionDir); 
                if (_DLLError)
                    return false;
            }

            // load the panels from the default culture (which is English)
            var resourcesDir = FileUtils.GetDefaultResourcesDir();
            Log.Debug("DefaultResourcesDir: " + resourcesDir);
            load(resourcesDir);

            _cultureConfigIdMapTable.Add(_loadCulture, _loadUserControlConfigMapTable);

            _configFileLocationMap.Add(_loadCulture, _loadConfigFileLocationMap);

            if (!_cultureConfigIdMapTable.ContainsKey(CultureInfo.DefaultThreadCurrentUICulture.Name))
            {
                resourcesDir = Path.Combine(FileUtils.ACATPath, CultureInfo.DefaultThreadCurrentUICulture.Name);
                if (Directory.Exists(resourcesDir))
                {
                    _loadCulture = CultureInfo.DefaultThreadCurrentUICulture.Name;

                    _loadUserControlConfigMapTable = new List<Guid>();

                    _loadConfigFileLocationMap = new Dictionary<string, string>();

                    Log.Debug("ResourcesDir: " + resourcesDir);
                    load(resourcesDir);

                    _cultureConfigIdMapTable.Add(_loadCulture, _loadUserControlConfigMapTable);

                    _configFileLocationMap.Add(_loadCulture, _loadConfigFileLocationMap);
                }
            }

            // load for the current culture
            if (!_cultureConfigIdMapTable.ContainsKey(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName))
            {
                resourcesDir = Path.Combine(FileUtils.ACATPath, CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName);
                if (Directory.Exists(resourcesDir))
                {
                    _loadCulture = CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName;

                    _loadUserControlConfigMapTable = new List<Guid>();

                    _loadConfigFileLocationMap = new Dictionary<string, string>();

                    Log.Debug("ResourcesDir: " + resourcesDir);
                    load(resourcesDir);

                    _cultureConfigIdMapTable.Add(_loadCulture, _loadUserControlConfigMapTable);

                    _configFileLocationMap.Add(_loadCulture, _loadConfigFileLocationMap);
                }
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
            if (_userControlsCache == null)
            {
                _userControlsCache = new Hashtable();
            }

            return loadTypesFromAssembly(assembly);
        }

        public static void Reset()
        {
            if (_cultureConfigIdMapTable != null)
            {
                _cultureConfigIdMapTable.Clear();
                _cultureConfigIdMapTable = null;
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
            Log.Debug("Cleaning up userControlConfigMap entries...");

            var removeList = new List<UserControlConfigMapEntry>();
            foreach (var mapEntry in _masterUserControlConfigMapTable.Values)
            {
                Log.Debug("Looking up " + mapEntry.ToString());
                if (_userControlsCache.ContainsKey(mapEntry.UserControlId))
                {
                    mapEntry.UserControlType = (Type)_userControlsCache[mapEntry.UserControlId];
                }

                Log.IsNull("mapEntry.UsercontrolType", mapEntry.UserControlType);

                var configFilePath = getConfigFilePathFromLocationMap(CultureInfo.DefaultThreadCurrentUICulture.Name, mapEntry.ConfigFileName);
                if (String.IsNullOrEmpty(configFilePath))
                {
                    configFilePath = getConfigFilePathFromLocationMap(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, mapEntry.ConfigFileName);
                }

                if (String.IsNullOrEmpty(configFilePath))
                {
                    configFilePath = getConfigFilePathFromLocationMap(DefaultCulture, mapEntry.ConfigFileName);
                }

                if (mapEntry.UserControlType != null && !String.IsNullOrEmpty(configFilePath))
                {
                    Log.Debug("Yes. _configFileLocationMap has configfile " + mapEntry.ConfigFileName);
                    mapEntry.ConfigFileName = configFilePath;
                }
                else
                {
                    Log.Debug("No. _configFileLocationMap does not have configfile " + mapEntry.ConfigFileName);
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
        private static String getConfigFilePathFromLocationMap(String language, String configFile)
        {
            if (_configFileLocationMap.ContainsKey(language))
            {
                var map = _configFileLocationMap[language];

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
        private static UserControlConfigMapEntry getCultureConfigMapEntry(String language, String name)
        {
            if (!_cultureConfigIdMapTable.ContainsKey(language))
            {
                return null;
            }

            List<Guid> configIds = _cultureConfigIdMapTable[language];

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
        private static UserControlConfigMapEntry getCultureConfigMapEntry(String language, Guid guid)
        {
            if (!_cultureConfigIdMapTable.ContainsKey(language))
            {
                return null;
            }

            List<Guid> configIds = _cultureConfigIdMapTable[language];

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
        /// <param name="resursive">Recursively search?</param>
        private static void load(String dir, bool resursive = true)
        {
            if (Directory.Exists(dir) && !_DLLError)
            {
                var walker = new DirectoryWalker(dir, "*.*");
                Log.Debug("Walking dir " + dir);
                walker.Walk(new OnFileFoundDelegate(onFileFound));
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

            if (assembly == null)
            {
                return false;
            }

            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    addUserControlTypeToCache(type);
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
        /// Found a DLL.  Load the class Types of all the relevant classes
        /// from the DLL
        /// </summary>
        /// <param name="dllName">name of the dll</param>
        private static void onDllFound(String dllName)
        {
            try
            {
                Log.Debug("Found dll " + dllName);

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
                            Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n Status Error: ERUCCM",
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
                    if (dllName.ToLower().Contains("usercontrols.dll"))
                    {
                        Log.Debug("HAHA");
                    }
                    loadTypesFromAssembly(Assembly.LoadFile(dllName));
                }
                
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
        /// Callback function for the directory walker that's invoked
        /// when a file is found.  Checks the file is a dll or an
        /// xml file and handles them appropriately
        /// </summary>
        /// <param name="file">name of the file found</param>
        private static void onFileFound(String file)
        {
            String filePath = file.ToLower();
            String fileName = Path.GetFileName(filePath);

            if (String.Compare(fileName, UserControlConfigMapFileName, true) == 0)
            {
                onPanelConfigMapFileFound(filePath);
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
        /// a mapping of the name and guid of the usercontrol and the name of
        /// the animation file for the usercontrol,
        /// Parses the config file and populates the map table with info
        /// from the file.
        /// </summary>
        /// <param name="configFileName">full path to the config file</param>
        private static void onPanelConfigMapFileFound(String configFileName)
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
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Found an XML file. Store the complete path to the file
        /// to the location map table
        /// </summary>
        /// <param name="xmlFileName">name of theo xml file</param>
        private static void onXmlFileFound(String xmlFileName)
        {
            String fileName = Path.GetFileName(xmlFileName).ToLower();
            if (!_loadConfigFileLocationMap.ContainsKey(fileName))
            {
                Log.Debug("Adding xmlfile " + fileName + ", fullPath: " + xmlFileName);
                _loadConfigFileLocationMap.Add(fileName.ToLower(), xmlFileName);
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