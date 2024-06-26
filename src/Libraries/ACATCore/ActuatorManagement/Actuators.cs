////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Actuators.cs
//
// Contains a collection of actuators in the system.  Creates
// the actuator objects by loading the Actuator settings file,
// looks into the Actuators extension directory, loads the DLL's
// and caches the Type of the actuator objects
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Contains a collection of actuators in the system.  Creates
    /// the actuator objects by loading the Actuator settings file,
    /// looks into the Actuators extension directory, loads the DLL's
    /// and caches the Type of the actuator objects
    /// </summary>
    public class Actuators : IDisposable
    {
        /// <summary>
        /// A list of actuators
        /// </summary>
        private readonly List<IActuator> _actuators;

        /// <summary>
        /// A list of actuators
        /// </summary>
        private readonly List<ActuatorEx> _actuatorsEx;

        /// <summary>
        /// A map of the guid and the type of the actuator.  The Type will
        /// be used to create an instance of the actuator
        /// </summary>
        private readonly Dictionary<Guid, Type> _actuatorsTypeCache;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed = false;
        /// <summary>
        /// If one of the dll found has an error with the certificate
        /// </summary>
        private static volatile bool _DLLError = false;
        /// <summary>
        /// Initializes the Actuator object
        /// </summary>
        public Actuators()
        {
            _actuatorsTypeCache = new Dictionary<Guid, Type>();
            _actuatorsEx = new List<ActuatorEx>();
            _actuators = new List<IActuator>();
        }

        /// <summary>
        /// Gets the list of actuators
        /// </summary>
        public IEnumerable<IActuator> ActuatorList
        {
            get { return _actuators; }
        }

        /// <summary>
        /// Gets the object that contains the settings for the
        /// actuators and their switches
        /// </summary>
        public ActuatorConfig Config { get; private set; }

        /// <summary>
        /// Gets the list of actuators
        /// </summary>
        internal IEnumerable<ActuatorEx> Collection
        {
            get
            {
                return _actuatorsEx;
            }
        }

        /// <summary>
        /// Adds the specified switch to the specified actuator.
        /// Notifies the actuator about the switch.  Also adds the
        /// switch to the settings file and saves the settings file
        /// </summary>
        /// <param name="actuator">atuator to add to</param>
        /// <param name="switchSetting">switch to add</param>
        /// <returns>true on success</returns>
        public bool AddSwitch(IActuator actuator, SwitchSetting switchSetting)
        {
            var actuatorSetting = Config.Find(actuator.Descriptor.Id);
            if (actuatorSetting == null)
            {
                return false;
            }

            var sw = actuatorSetting.Find(switchSetting.Name);
            if (sw != null)
            {
                return true;
            }

            bool retVal = actuator.Load(new List<SwitchSetting>() { switchSetting });

            if (retVal)
            {
                actuatorSetting.SwitchSettings.Add(switchSetting);
                Config.Save();
            }

            return retVal;
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
        /// Finds actuator by name
        /// </summary>
        /// <param name="name">name of the actuator</param>
        /// <returns>actuator object</returns>
        public IActuator Find(String name)
        {
            foreach (var actuatorEx in _actuatorsEx)
            {
                if (String.Compare(name, actuatorEx.SourceActuator.Name, true) == 0)
                {
                    return actuatorEx.SourceActuator;
                }
            }

            Log.Debug("Could not find actuator by name " + name);
            return null;
        }

        /// <summary>
        /// Returns an actutator object of the specified type
        /// </summary>
        /// <param name="actuatorType">Type of the object</param>
        /// <returns>The object </returns>
        public IActuator Find(Type actuatorType)
        {
            foreach (var actuatorEx in _actuatorsEx)
            {
                if (actuatorType.FullName == actuatorEx.SourceActuator.GetType().FullName)
                {
                    Log.Debug("Found actuator of type " + actuatorType.Name);
                    return actuatorEx.SourceActuator;
                }
            }

            Log.Debug("Could not find actuator of type " + actuatorType.Name);
            return null;
        }

        /// <summary>
        /// Loads actuator settigns from the settings file.
        /// Walks through the extensions dirs, looks for actuators in there
        /// and caches the Types of the actuators.
        /// Configures the actuators with the settings from the settings file.
        /// Also if any acutators were discovered that are not in the settings file,
        /// adds them to the settings file and saves it.
        /// </summary>
        /// <param name="extensionDirs">directories to walk through</param>
        /// <param name="configFile">name of the actuators settings file</param>
        /// <param name="loadAll">whether to load even the disabled actuators</param>
        /// <returns>true on success</returns>
        public bool Load(IEnumerable<String> extensionDirs, String configFile, bool loadAll = false)
        {
            addKeyboardActuatorToCache();
            addSwitchInterfaceActuatorTocache();

            foreach (string dir in extensionDirs)
            {
                String extensionDir = dir + "\\" + ActuatorManager.ActuatorsRootDir;
                loadActuatorTypesIntoCache(extensionDir);
            }
            if(_DLLError)
                return false;   
            if (!File.Exists(configFile))
            {
                return false;
            }

            ActuatorConfig.ActuatorSettingsFileName = configFile;
            Config = ActuatorConfig.Load();

            // walk through the settings file create and configure
            // actuators
            foreach (var actuatorSetting in Config.ActuatorSettings)
            {
                try
                {
                    bool enabled = (loadAll) || actuatorSetting.Enabled;
                    if (enabled && (actuatorSetting.Id != Guid.Empty))
                    {
                        if (!_actuatorsTypeCache.ContainsKey(actuatorSetting.Id))
                        {
                            continue;
                        }

                        var type = _actuatorsTypeCache[actuatorSetting.Id];
                        if (type != null)
                        {
                            var assembly = Assembly.LoadFrom(type.Assembly.Location);
                            var actuator = (IActuator)assembly.CreateInstance(type.FullName);
                            if (actuator != null)
                            {
                                actuator.OnRegisterSwitches();
                                actuator.Load(actuatorSetting.SwitchSettings);
                                actuator.Enabled = actuatorSetting.Enabled;
                                var actuatorEx = new ActuatorEx(actuator);
                                _actuatorsEx.Add(actuatorEx);
                                _actuators.Add(actuator);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }

            // now go through all the actuators that are not in the
            // settings file and add them to the settings file

            bool isDirty = false;
            foreach (var actuatorType in _actuatorsTypeCache.Values)
            {
                var attr = DescriptorAttribute.GetDescriptor(actuatorType);
                if (attr != null && attr.Id != Guid.Empty)
                {
                    var actuatorSetting = Config.Find(attr.Id);
                    if (actuatorSetting != null) continue;

                    try
                    {
                        var assembly = Assembly.LoadFrom(actuatorType.Assembly.Location);
                        var actuator = (IActuator)assembly.CreateInstance(actuatorType.FullName);
                        if (actuator != null)
                        {
                            var actuatorEx = new ActuatorEx(actuator);
                            _actuatorsEx.Add(actuatorEx);
                            _actuators.Add(actuator);

                            actuatorSetting = new ActuatorSetting(attr.Name, attr.Id);
                            Config.ActuatorSettings.Add(actuatorSetting);

                            actuator.OnRegisterSwitches();
                            actuator.Load(actuatorSetting.SwitchSettings);

                            isDirty = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }

            if (isDirty)
            {
                Config.Save();
            }

            return true;
        }

        /// <summary>
        /// Handler for when the application exits
        /// </summary>
        public void OnAppQuit()
        {
            foreach (var actuator in _actuators)
            {
                actuator.OnQuitApplication();
            }
        }

        /// <summary>
        /// Remove the specified switch from the actuator. Also
        /// remove it from the settings file and save the file
        /// </summary>
        /// <param name="actuator">Actuator</param>
        /// <param name="switchName">switch to remove</param>
        /// <returns>true on success</returns>
        public bool RemoveSwitch(IActuator actuator, String switchName)
        {
            var actuatorSetting = Config.Find(actuator.Descriptor.Id);
            if (actuatorSetting == null)
            {
                return false;
            }

            var sw = actuatorSetting.Find(switchName);
            if (sw == null)
            {
                return false;
            }

            bool retVal = actuator.RemoveSwitch(switchName);

            actuatorSetting.SwitchSettings.Remove(sw);
            Config.Save();

            return retVal;
        }

        /// <summary>
        /// Saves actuator settings to file.  Set the enabled
        /// attribute before saving
        /// </summary>
        public void SaveActuatorSettings()
        {
            var actuatorConfig = ActuatorConfig.Load();

            foreach (var actuatorSetting in actuatorConfig.ActuatorSettings)
            {
                var actuator = Find(actuatorSetting.Name);
                if (actuator != null)
                {
                    actuatorSetting.Enabled = actuator.Enabled;
                }
            }

            actuatorConfig.Save();
        }

        /// <summary>
        /// Finds the acuatorex object that contains the
        /// actuator
        /// </summary>
        /// <param name="actuator">source actuator</param>
        /// <returns>object, null if not found</returns>
        internal ActuatorEx find(IActuator actuator)
        {
            return _actuatorsEx.FirstOrDefault(ac => ac.SourceActuator == actuator);
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
                    foreach (var actuator in _actuatorsEx)
                    {
                        actuator.SourceActuator.Dispose();
                    }

                    _actuatorsTypeCache.Clear();
                    _actuatorsEx.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
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

        private void addSwitchInterfaceActuatorTocache()
        {
            var attr = DescriptorAttribute.GetDescriptor(typeof(InputActuators.SwitchInterfaceActuator));
            if (attr != null)
            {
                addActuatorToCache(attr.Id, typeof(InputActuators.SwitchInterfaceActuator));
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
                            Prompt = $"The following DLL is not digitally signed \nDLL: {dllName}.\nReason for failure: {ex.Message} \n Status Error: ERA",
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
                
            }
            catch// (Exception ex)
            {
                //Log.Debug("Could not get types from assembly " + dllName + ". Exception : " + ex.ToString());
            }
        }
    }
}