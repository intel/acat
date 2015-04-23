////////////////////////////////////////////////////////////////////////////
// <copyright file="TTSManager.cs" company="Intel Corporation">
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
    /// Manages text to speech (TTS) engines.  There are a variety
    /// of methods to convert TTS.  Some are hardware solutions (SpeechPlus)
    /// and some are software (SAPI).  ACAT supports a
    /// variety of TTS engine to be used through the ITTSEngine interface.
    /// The specifics of doing the conversion is up to the developer of the
    /// engine.
    /// The TTS manager holds a list of engines that it finds and also
    /// tracks the currently active TTS engine
    /// This class is a singleton.
    /// </summary>
    public class TTSManager : IDisposable
    {
        /// <summary>
        /// The root directory where all the TTS engines are located.
        /// </summary>
        static public String TTSRootDir = "TTSEngines";

        /// <summary>
        /// Upper bound for the volume
        /// </summary>
        private const int _maxNormalizedVolume = 9;

        /// <summary>
        /// Lower bound for the volume level.  Since different engines support
        /// different ranges for volume, ACAT normalizes them and
        /// uses an integer between 0 and 9 for the volume level
        /// </summary>
        private const int _minNormalizedVolume = 0;

        /// <summary>
        /// Singleton instance of the manager
        /// </summary>
        private static readonly TTSManager _instance = new TTSManager();

        /// <summary>
        /// A null TTS engine.  This is a barebones class and doesn't
        /// actually do any TTS conversions.
        /// </summary>
        private readonly ITTSEngine _nullEngine;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// List of TTS engines
        /// </summary>
        private TTSEngines _ttsEngines;

        /// <summary>
        /// Initializes the singleton instance of the class
        /// </summary>
        private TTSManager()
        {
            _nullEngine = new Core.TTSEngines.NullEngine();
            ActiveEngine = _nullEngine;
            ActiveEngine.Init();
        }

        /// <summary>
        /// Indicates that the active TTS engine changed
        /// </summary>
        /// <param name="oldEngine">previous engine</param>
        /// <param name="newEngine">changed to this</param>
        public delegate void EngineChanged(ITTSEngine oldEngine, ITTSEngine newEngine);

        /// <summary>
        /// Raised when the engine changes
        /// </summary>
        public event EngineChanged EvtEngineChanged;

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static TTSManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the currently active TTS engine
        /// </summary>
        public ITTSEngine ActiveEngine { get; private set; }

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

        /// <summary>
        /// Returns the current normalized volume
        /// </summary>
        /// <returns>volume level</returns>
        public TTSValue GetNormalizedVolume()
        {
            ITTSValue<int> volume = ActiveEngine.GetVolume();

            if (volume.RangeMax == 0)
            {
                return new TTSValue(_minNormalizedVolume, _maxNormalizedVolume, 0);
            }

            float fraction = (float)volume.Value / volume.RangeMax;
            float currentValue = fraction * 9.0f;
            int v = (int)Math.Round(currentValue, 0);
            if (v == 0 && volume.Value > 0)
            {
                v = 1;
            }

            return new TTSValue(_minNormalizedVolume, _maxNormalizedVolume, v);
        }

        /// <summary>
        /// XML file and creates a list of TTSEngines from it.  The
        /// extension dirs parameter contains the root directory under
        /// which to search for TTSEngine DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Base, Hawking
        /// These are relative to the application execution directory or
        /// to the directory where ACAT has been installed.
        /// It recusrively walks the directories and looks for TTS Engine
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            if (_ttsEngines == null)
            {
                _ttsEngines = new TTSEngines();
                retVal = _ttsEngines.Load(extensionDirs);
            }

            return retVal;
        }

        /// <summary>
        /// Sets the Active TTS engine identified by the specified
        /// GUID or name
        /// </summary>
        /// <param name="idOrName">name or ID of the engine</param>
        /// <returns>true on success</returns>
        public bool SetActiveEngine(String idOrName)
        {
            Guid guid;

            if (!Guid.TryParse(idOrName, out guid))
            {
                guid = _ttsEngines.GetByName(idOrName);
            }

            return (guid != Guid.Empty) && SetActiveEngine(guid);
        }

        /// <summary>
        /// Sets the Active TTS engine identified by the GUID id
        /// </summary>
        /// <param name="id">GUID of the engine</param>
        /// <returns>true on success</returns>
        public bool SetActiveEngine(Guid id)
        {
            bool retVal;
            var type = _ttsEngines[id];

            if (type != null)
            {
                var oldEngine = ActiveEngine;

                retVal = createAndSetActiveEngine(type);

                notifyEngineChanged(oldEngine, ActiveEngine);

                if (oldEngine != null)
                {
                    oldEngine.Dispose();
                }
            }
            else
            {
                retVal = false;
            }

            Log.Debug("retVal=" + retVal);
            return retVal;
        }

        /// <summary>
        /// Sets the normalized volume.   Since different engines support
        /// different ranges for volume, ACAT normalizes them and
        /// uses an integer between 0 and 9 for the volume level
        /// </summary>
        /// <param name="normalizedVolume">normalized volume level</param>
        public void SetNormalizedVolume(int normalizedVolume)
        {
            ITTSValue<int> v = ActiveEngine.GetVolume();
            if (v.RangeMax == 0)
            {
                return;
            }

            float fraction = (float)normalizedVolume / 9.0f;
            float currentValue = fraction * v.RangeMax;
            int volSetting = (int)Math.Round(currentValue, 0);

            ActiveEngine.SetVolume(volSetting);
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
                    if (ActiveEngine != null)
                    {
                        ActiveEngine.Dispose();
                    }

                    if (_ttsEngines != null)
                    {
                        _ttsEngines.Dispose();
                    }

                    if (_nullEngine != null)
                    {
                        _nullEngine.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Creates a TTS engine oft he specified Type, initializes it
        /// and sets it as the active engine.  If it could not create it,
        /// it sets the null engine as the active engine
        /// </summary>
        /// <param name="engineType">The .NET type of the engine</param>
        /// <returns>true on success</returns>
        private bool createAndSetActiveEngine(Type engineType)
        {
            bool retVal;

            try
            {
                var engine = (ITTSEngine)Activator.CreateInstance(engineType);
                retVal = engine.Init();
                ActiveEngine = (retVal) ? engine : null;
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to instantiate TTS engine of type " + engineType.ToString() + ", assembly: "
                    + engineType.Assembly.FullName + ". Exception: " + ex.ToString());
                retVal = false;
            }

            if (!retVal)
            {
                ActiveEngine = _nullEngine;
            }

            Log.Debug("retVal=" + retVal);

            return retVal;
        }

        /// <summary>
        /// Raises an event indicating that the active engine has changed
        /// </summary>
        /// <param name="oldEngine">previous engine</param>
        /// <param name="newEngine">the newly activated engine</param>
        private void notifyEngineChanged(ITTSEngine oldEngine, ITTSEngine newEngine)
        {
            if (EvtEngineChanged != null)
            {
                EvtEngineChanged(oldEngine, newEngine);
            }
        }
    }
}