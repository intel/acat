////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// PanelConfigMap is an xml file that contains a mapping between the
    /// name of the class for the scanner and the name of the xml file that
    /// contains animation and other info for the scanner.  This
    /// allows for mapping different animation files to the same scanner (form).
    /// For instance, a QWERTY layout alphabet scanner in English can have a different
    /// layout of letters for anothe language like French.
    /// </summary>
    public class PanelConfigMap
    {

        private static volatile bool _DLLError = false;
        /// <summary>
        /// Name of the panel class config file. This file contains a
        /// list of panel configurations to use
        /// </summary>
        public static String PanelClassConfigFileName = "panelclassconfig.xml";

        private const String DefaultCulture = "en";

        /// <summary>
        /// Name of the config file that has the mapping.  This is loaded from
        /// the user directory
        /// </summary>
        private const String PanelConfigMapFileName = "panelconfigmap.xml";

        /// <summary>
        /// Maps the name of a config file to the complete path of the file
        /// </summary>
        private static Dictionary<String, Dictionary<String, String>> _configFileLocationMap;

        private static Dictionary<String, AppPanelClassConfig> _cultureAppPanelClassConfig;
        private static Dictionary<String, List<Guid>> _cultureConfigIdMapTable;

        private static Dictionary<String, PanelClassConfig> _culturePanelClassConfigMapTable;
        private static AppPanelClassConfig _currentAppPanelClassConfig = null;

        /// <summary>
        /// Caches the class Type of forms
        /// </summary>
        private static Hashtable _formsCache;

        private static Dictionary<String, String> _loadConfigFileLocationMap;
        private static String _loadCulture;
        private static List<Guid> _loadPanelConfigMapTable;
        private static Dictionary<Guid, PanelConfigMapEntry> _masterPanelConfigMapTable;

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
        /// Returns the name of the animation config file for the specified
        /// scanner.  The GetPanelConfigMapEntry function first checks
        /// the culture folder (if non-English is the current culture)
        /// It it doesn't find it thre, it looks up
        /// the English culture folder
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
        /// Returns the config id for the specified config name.  Returns
        /// empty guid if not found.
        /// </summary>
        /// <param name="configName">the config name</param>
        /// <returns>config id</returns>
        public static Guid GetConfigIdForConfigName(String configName)
        {
            foreach (var panelConfigMapEntry in _masterPanelConfigMapTable.Values)
            {
                if (String.Compare(configName, panelConfigMapEntry.ConfigName, true) == 0)
                {
                    return panelConfigMapEntry.ConfigId;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Returns the panelclass config filename for the current culture
        /// </summary>
        /// <returns>the full file name</returns>
        public static String GetCurrentCulturePanelClassConfigFile()
        {
            var fileName = Path.Combine(UserManager.CurrentUserDir, CultureInfo.DefaultThreadCurrentUICulture.Name, PanelClassConfigFileName);
            if (!File.Exists(fileName))
            {
                fileName = Path.Combine(UserManager.CurrentUserDir, CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, PanelClassConfigFileName);
            }

            return fileName;
        }

        /// <summary>
        /// Returns the panelclass config file name for the default English language
        /// </summary>
        /// <returns>the full filename</returns>
        public static String GetDefaultPanelClassConfigFileName()
        {
            return Path.Combine(UserManager.CurrentUserDir, DefaultCulture, PanelClassConfigFileName);
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
            PanelClassConfig panelClassConfig = null;

            if (_culturePanelClassConfigMapTable.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture.Name, out panelClassConfig))
            {
                _currentAppPanelClassConfig = _cultureAppPanelClassConfig[CultureInfo.DefaultThreadCurrentUICulture.Name];
            }
            else if (_culturePanelClassConfigMapTable.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, out panelClassConfig))
            {
                _currentAppPanelClassConfig = _cultureAppPanelClassConfig[CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName];
            }
            else if (_culturePanelClassConfigMapTable.TryGetValue(DefaultCulture, out panelClassConfig))
            {
                _currentAppPanelClassConfig = _cultureAppPanelClassConfig[DefaultCulture];
            }

            return panelClassConfig?.GetDefaultClassConfigMap();
        }

        public static PanelClassConfig GetPanelClassConfigForApp()
        {
            PanelClassConfig panelClassConfig = null;

            if (_culturePanelClassConfigMapTable.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture.Name, out panelClassConfig))
            {
                _currentAppPanelClassConfig = _cultureAppPanelClassConfig[CultureInfo.DefaultThreadCurrentUICulture.Name];
            }
            else if (_culturePanelClassConfigMapTable.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, out panelClassConfig))
            {
                _currentAppPanelClassConfig = _cultureAppPanelClassConfig[CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName];
            }
            else if (_culturePanelClassConfigMapTable.TryGetValue(DefaultCulture, out panelClassConfig))
            {
                _currentAppPanelClassConfig = _cultureAppPanelClassConfig[DefaultCulture];
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
        public static PanelConfigMapEntry GetPanelConfigMapEntry(String panel)
        {
            PanelConfigMapEntry retVal = getMapEntryFromPanelClassConfigMap(panel);

            if (retVal == null)
            {
                retVal = getCultureConfigMapEntry(CultureInfo.DefaultThreadCurrentUICulture.Name, panel);
                if (retVal == null)
                {
                    retVal = getCultureConfigMapEntry(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, panel);
                }

                if (retVal == null)
                {
                    retVal = getCultureConfigMapEntry(DefaultCulture, panel);
                }
            }

            return retVal;
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
        public static bool Load(IEnumerable<String> extensionDirs)
        {
            _culturePanelClassConfigMapTable = new Dictionary<string, PanelClassConfig>();

            _masterPanelConfigMapTable = new Dictionary<Guid, PanelConfigMapEntry>();

            LoadPanelClassConfig();

            _cultureConfigIdMapTable = new Dictionary<string, List<Guid>>();

            _configFileLocationMap = new Dictionary<string, Dictionary<string, string>>();

            _cultureAppPanelClassConfig = new Dictionary<string, AppPanelClassConfig>();

            _formsCache = new Hashtable();

            _loadCulture = DefaultCulture;

            _loadPanelConfigMapTable = new List<Guid>();

            _loadConfigFileLocationMap = new Dictionary<string, string>();

            // first walk the extension directories
            foreach (string dir in extensionDirs)
            {
                String extensionDir = dir + "\\" + AgentManager.AppAgentsRootDir;
                load(extensionDir);
                if(_DLLError)
                    return false;

                extensionDir = dir + "\\" + AgentManager.FunctionalAgentsRootDir;
                load(extensionDir);
                if (_DLLError)
                    return false;

                extensionDir = dir + "\\" + PanelManager.UiRootDir;
                load(extensionDir);
                if (_DLLError)
                    return false;
            }

            // load the panels from the default culture (which is English)
            var resourcesDir = FileUtils.GetDefaultResourcesDir();
            Log.Debug("DefaultResourcesDir: " + resourcesDir);
            load(resourcesDir);

            _cultureConfigIdMapTable.Add(_loadCulture, _loadPanelConfigMapTable);

            _configFileLocationMap.Add(_loadCulture, _loadConfigFileLocationMap);

            if (!_cultureConfigIdMapTable.ContainsKey(CultureInfo.DefaultThreadCurrentUICulture.Name))
            {
                resourcesDir = Path.Combine(FileUtils.ACATPath, CultureInfo.DefaultThreadCurrentUICulture.Name);
                if (Directory.Exists(resourcesDir))
                {
                    _loadCulture = CultureInfo.DefaultThreadCurrentUICulture.Name;

                    _loadPanelConfigMapTable = new List<Guid>();

                    _loadConfigFileLocationMap = new Dictionary<string, string>();

                    Log.Debug("ResourcesDir: " + resourcesDir);
                    load(resourcesDir);

                    _cultureConfigIdMapTable.Add(_loadCulture, _loadPanelConfigMapTable);

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

                    _loadPanelConfigMapTable = new List<Guid>();

                    _loadConfigFileLocationMap = new Dictionary<string, string>();

                    Log.Debug("ResourcesDir: " + resourcesDir);
                    load(resourcesDir);

                    _cultureConfigIdMapTable.Add(_loadCulture, _loadPanelConfigMapTable);

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
            if (_formsCache == null)
            {
                _formsCache = new Hashtable();
            }

            return loadTypesFromAssembly(assembly);
        }

        public static void LoadPanelClassConfig()
        {
            loadPanelClassConfig(CultureInfo.DefaultThreadCurrentUICulture.Name);
            loadPanelClassConfig(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName);
            loadPanelClassConfig(DefaultCulture);
        }

        public static void Reset()
        {
            if (_cultureConfigIdMapTable != null)
            {
                _cultureConfigIdMapTable.Clear();
                _cultureConfigIdMapTable = null;
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

            if (_culturePanelClassConfigMapTable != null)
            {
                _culturePanelClassConfigMapTable.Clear();
                _culturePanelClassConfigMapTable = null;
            }

            if (_cultureAppPanelClassConfig != null)
            {
                _cultureAppPanelClassConfig.Clear();
                _cultureAppPanelClassConfig = null;
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
            return (_currentAppPanelClassConfig != null) && _currentAppPanelClassConfig.Save();
        }

        /// <summary>
        /// Sets the specified config name as the default config name
        /// for the app.
        /// </summary>
        /// <param name="configName">name of the config</param>
        /// <returns>true on success</returns>
        public static bool SetDefaultPanelConfig(String configName)
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(configName))
            {
                return false;
            }

            PanelClassConfig panelClassConfig = null;

            if (_culturePanelClassConfigMapTable.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture.Name, out panelClassConfig) ||
                _culturePanelClassConfigMapTable.TryGetValue(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, out panelClassConfig) ||
                _culturePanelClassConfigMapTable.TryGetValue(DefaultCulture, out panelClassConfig))
            {
                retVal = panelClassConfig.SetDefaultClassConfigMap(configName);
            }

            return retVal;
        }

        /// <summary>
        /// Add a new entry to the PanelClassConfig and save the file
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="language"></param>
        /// <param name="panelClassConfigMap"></param>
        /// <returns></returns>
        public static bool AddPanelClassConfigMap(String appId, String language, PanelClassConfigMap panelClassConfigMap)
        {
            var panelClassConfigFilePath = Path.Combine(UserManager.CurrentUserDir, language, PanelClassConfigFileName);

            if (File.Exists(panelClassConfigFilePath))
            {
                var appPanelClassConfig = AppPanelClassConfig.Load(panelClassConfigFilePath);

                var panelClassConfig = appPanelClassConfig.Find(CoreGlobals.AppId);

                if (panelClassConfig != null)
                {
                    var result = panelClassConfig.PanelClassConfigMaps.Find(mapEntry => String.Compare(mapEntry.Name, panelClassConfigMap.Name, true) == 0);

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
        /// Adds the specified Type to the cache keyed by the Guid.
        /// </summary>
        /// <param name="guid">Guid for the scanner</param>
        /// <param name="type">Scanner class Type</param>
        internal static void AddFormToCache(Guid guid, Type type)
        {
            if (_formsCache.ContainsKey(guid))
            {
                Log.Debug("Form Type " + type.FullName + ", guid " + guid + " is already added");
                return;
            }

            Log.Debug("Adding form " + type.FullName + ", guid " + guid + " to cache");
            _formsCache.Add(guid, type);

            updateFormTypeReferences(guid, type);
        }

        /// <summary>
        /// Cleans up the map tables of entries that are orphans. These
        /// are forms that don't have a corresponding animatinon file
        /// </summary>
        internal static void CleanupOrphans()
        {
            Log.Debug("Cleaning up panelConfigMap entries...");

            var removeList = new List<PanelConfigMapEntry>();
            foreach (var mapEntry in _masterPanelConfigMapTable.Values)
            {
#if DEBUG
                if (mapEntry.ConfigId.Equals(new Guid("C753E412-0A2C-40A2-B47C-954C620573ED")))
                {
                    Log.Debug("Breakpoint");
                }
#endif

                Log.Debug("Looking up " + mapEntry.ToString());
                if (_formsCache.ContainsKey(mapEntry.FormId))
                {
                    mapEntry.FormType = (Type)_formsCache[mapEntry.FormId];
                }

                Log.IsNull("mapEntry.FormType ", mapEntry.FormType);

                var configFilePath = getConfigFilePathFromLocationMap(CultureInfo.DefaultThreadCurrentUICulture.Name, mapEntry.ConfigFileName);
                if (String.IsNullOrEmpty(configFilePath))
                {
                    configFilePath = getConfigFilePathFromLocationMap(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, mapEntry.ConfigFileName);
                }

                if (String.IsNullOrEmpty(configFilePath))
                {
                    configFilePath = getConfigFilePathFromLocationMap(DefaultCulture, mapEntry.ConfigFileName);
                }

                if (mapEntry.FormType != null && !String.IsNullOrEmpty(configFilePath))
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
        /// Gets the ACAT descriptor guid for the specifed Type
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
        /// Adds the specified mapEntry object to the map table. Also
        /// looks up the map table if it already has the formID specified
        /// in the mapEntry and updates its config file name with the
        /// one in mapEntry
        /// </summary>
        /// <param name="mapTable">Table to add to</param>
        /// <param name="mapEntry">map entry to add</param>
        private static void addToMapTable(List<Guid> configIdTable, PanelConfigMapEntry mapEntry)
        {
#if DEBUG
            if (mapEntry.ConfigId.Equals(new Guid("C753E412-0A2C-40A2-B47C-954C620573ED")))
            {
                Log.Debug("Breakpoint");
            }
#endif

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
                var guid = GetFormId(type);

#if DEBUG
                if (guid.Equals(new Guid("61E8A29A-5076-4047-A9F5-89E7E4903407")))
                {
                    Log.Debug("Breakpoint");
                }
#endif
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
        private static PanelClassConfigMapEntry getClassConfigMapEntryForCulture(String language, String panelClass)
        {
            if (_culturePanelClassConfigMapTable.ContainsKey(language))
            {
                var panelClassConfig = _culturePanelClassConfigMapTable[language];

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
        /// Returns the panelconfigmapentry for the specified panel class
        /// for the specified language
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="panelClass">panel class</param>
        /// <returns>object, null if not found</returns>
        private static PanelConfigMapEntry getCultureConfigMapEntry(String language, String panelClass)
        {
            if (!_cultureConfigIdMapTable.ContainsKey(language))
            {
                return null;
            }

            List<Guid> configIds = _cultureConfigIdMapTable[language];

            foreach (var configId in configIds)
            {
                if (_masterPanelConfigMapTable.ContainsKey(configId))
                {
                    var panelConfigMapEntry = _masterPanelConfigMapTable[configId];
                    if (String.Compare(panelConfigMapEntry.PanelClass, panelClass, true) == 0)
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
        private static PanelConfigMapEntry getMapEntryFromPanelClassConfigMap(String panelClass)
        {
            var panelClassConfigMapEntry = (getClassConfigMapEntryForCulture(CultureInfo.DefaultThreadCurrentUICulture.Name, panelClass) ??
                                            getClassConfigMapEntryForCulture(CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, panelClass)) ??
                                           getClassConfigMapEntryForCulture(DefaultCulture, panelClass);

            if (panelClassConfigMapEntry == null)
            {
                return null;
            }

            var configId = panelClassConfigMapEntry.ConfigId;

            return (_masterPanelConfigMapTable.ContainsKey(configId))
                            ? _masterPanelConfigMapTable[configId]
                            : null;
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
        /// For this application, load the panel configurations to use from
        /// the panelclassconfig.xml file
        /// </summary>
        /// <param name="language"></param>
        private static void loadPanelClassConfig(String language)
        {
            if (_cultureAppPanelClassConfig == null)
            {
                _cultureAppPanelClassConfig = new Dictionary<string, AppPanelClassConfig>();
            }

            if (_culturePanelClassConfigMapTable == null)
            {
                _culturePanelClassConfigMapTable = new Dictionary<string, PanelClassConfig>();
            }

            var panelClassConfigFilePath = Path.Combine(UserManager.CurrentUserDir, language, PanelClassConfigFileName);

            if (File.Exists(panelClassConfigFilePath) && !_culturePanelClassConfigMapTable.ContainsKey(language))
            {
                var appPanelClassConfig = AppPanelClassConfig.Load(panelClassConfigFilePath);
                _cultureAppPanelClassConfig[language] = appPanelClassConfig;

                var panelClassConfig = appPanelClassConfig.Find(CoreGlobals.AppId);

                if (panelClassConfig != null && panelClassConfig.PanelClassConfigMaps.Count > 0)
                {
                    _culturePanelClassConfigMapTable[language] = panelClassConfig;
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
                            Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n Status Error: ERPCM",
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
                    loadTypesFromAssembly(Assembly.LoadFile(dllName));
                
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

            if (String.Compare(fileName, PanelConfigMapFileName, true) == 0)
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
        /// a mapping of the name and guid of the scanner and the name of
        /// the animation file for the scanner,
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

                var configNodes = doc.SelectNodes("/ACAT/ConfigMapEntries/ConfigMapEntry");

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
            foreach (var panelConfigMapEntry in _masterPanelConfigMapTable.Values)
            {
                if (panelConfigMapEntry.FormType == null && panelConfigMapEntry.FormId.Equals(guid))
                {
                    panelConfigMapEntry.FormType = type;
                }
            }
        }
    }
}