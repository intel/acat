////////////////////////////////////////////////////////////////////////////
// <copyright file="PresageWordPredictor.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ACAT.Extensions.Default.WordPredictors.Presage.PresageService;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using Microsoft.Win32;

namespace ACAT.Extensions.Default.WordPredictors.PresageWCF
{
    /// <summary>
    /// The Word Predictor that uses the Presage word predictor
    /// for next word prediction. Presage is an intelligent predictive 
    /// text engine created by Matteo Vescovi. 
    /// http://presage.sourceforge.net/
    /// </summary>
    [DescriptorAttribute("1495D4A3-29AD-471F-9FD3-46EC92171AF2",
                            "Presage Word Predictor",
                            "Word prediction engine based on Presage")]
    public class PresageWordPredictor : IWordPredictor
    {
        private const String PresageProcessName = "presage_wcf_service_system_tray";

        private const String PresageWCFTrayAppName = "presage_wcf_service_system_tray.exe";

        /// <summary>
        /// Name of the preferences file
        /// </summary>
        private const String SettingsFileName = "PresageWordPredictorSettings.xml";

        /// <summary>
        /// The input text buffer
        /// </summary>
        private string _inputText = "";

        /// <summary>
        /// The Presage object
        /// </summary>
        private PresageClient _presage;

        /// <summary>
        /// Word prediction settings object
        /// </summary>
        private Settings _presageSettings;

        /// <summary>
        /// Returns the ACAT descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Get or sets the value to indicate whether punctuations
        /// should be filtered.
        /// This property is not used.
        /// </summary>
        public bool FilterPunctuationsEnable { get; set; }

        /// <summary>
        /// Gets or sets the NGram for word prediction.
        /// Not used
        /// </summary>
        public int NGram
        {
            get { return -1; }

            set { }
        }

        /// <summary>
        /// Gets or sets the number of words in the suggested
        /// prediction list.
        /// </summary>
        public int PredictionWordCount
        {
            get
            {
                int result = -1;

                try
                {
                    var configValue = _presage.get_config("Presage.Selector.SUGGESTIONS");
                    result = Convert.ToInt32(configValue);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }

                return result;
            }

            set
            {
                try
                {
                    _presage.set_config("Presage.Selector.SUGGESTIONS", value.ToString());
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Gets the settings dialog for this engine.
        /// Not used
        /// </summary>
        public IWordPredictorSettingsDialog SettingsDialog
        {
            get { return null; }
        }

        /// <summary>
        /// Gets value that indicates whether this word
        /// predictor supports learning.  Returns true.
        /// The Learn() function can be called to add
        /// sentences to the user's word prediction
        /// model for better word prediction.
        /// </summary>
        public bool SupportsLearning
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether it supports a settings dialog
        /// Returns false always.
        /// </summary>
        public bool SupportsSettingsDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Disposer for this class
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Performs initialization.  Must be called first.
        /// Sets word prediction parameters and initializes Presage
        /// </summary>
        /// <returns>true on success</returns>
        public bool Init()
        {
            bool retVal = false;

            try
            {
                Attributions.Add("PRESAGE",
                                "Predictive text functionality is powered by Presage, the " +
                                "intelligent predictive text engine created by Matteo Vescovi. " +
                                "(http://presage.sourceforge.net/)");

                Settings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);

                _presageSettings = Settings.Load();

                if (_presageSettings.StartPresageIfNotRunning)
                {
                    checkAndRunPresage();
                }

                retVal = initPresage();
                if (!retVal)
                {
                    // failure mostly means the presage config file has an
                    // incorrect database path.  Delete the config file so
                    // it will regenerate and then we can insert the correct
                    // database path when we init again
                    deletePresageConfigFile();
                    retVal = initPresage();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error initializing Presage. " + ex);
            }

            return retVal;
        }

        /// <summary>
        /// Adds the specified text to the user's personal
        /// word prediction model to learn the user's writing
        /// style.  This makes word prediciton more relevant.
        /// </summary>
        /// <param name="text">Text to add</param>
        /// <returns>true on success</returns>
        public bool Learn(String text)
        {
            bool result = false;

            if (Common.AppPreferences.EnableWordPredictionDynamicModel)
            {
                try
                {
                    _presage.learn(text);
                    result = true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the text into a temporary context.
        /// This could for instance be the text from a notepad
        /// or word document.  This makes word prediction more
        /// contextual to the document being edited.  Call
        /// UnloadContext to unload it.
        ///
        /// This function is not currently supported.
        /// </summary>
        /// <param name="text">Text to add to the context</param>
        /// <returns>a handle</returns>
        public int LoadContext(String text)
        {
            // not supported

            return -1;
        }

        /// <summary>
        /// Returns factory default settings.
        /// </summary>
        /// <returns></returns>
        public bool LoadDefaultSettings()
        {
            updateSettings(new ACATPreferences());
            return true;
        }

        /// <summary>
        /// Loads settings from the specified file and sets
        /// properties from the settings file.
        /// </summary>
        /// <param name="settingsFilePath">path to the settings file</param>
        /// <returns>true on success</returns>
        public bool LoadSettings(String settingsFilePath)
        {
            updateSettings(Common.AppPreferences);
            return true;
        }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount property
        /// </summary>
        /// <param name="prevWords">Previous words in the sentence</param>
        /// <param name="currentWord">current word (may be partially spelt out</param>
        /// <returns>A list of predicted words</returns>
        public IEnumerable<String> Predict(String prevWords, String currentWord)
        {
            // update the internal buffer with prevWords + currentWord
            _inputText = prevWords + " " + currentWord;
            Log.Debug("[" + _inputText + "]");

            List<string> retVal;

            try
            {
                string[] prediction = _presage.predict(prevWords, currentWord);
                retVal = prediction.ToList();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                retVal = new List<string>();
            }
            return retVal;
        }

        /// <summary>
        /// Save the word predictor settings to a file that is maintained
        /// by the word predictor.
        /// </summary>
        /// <param name="settingsFilePath">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        public bool SaveSettings(String settingsFilePath)
        {
            updateSettings(Common.AppPreferences);
            return true;
        }

        /// <summary>
        /// Unloads previously loaded context (see LoadContext).
        /// Not used.
        /// </summary>
        /// <param name="contextHandle">the handle</param>
        public void UnloadContext(int contextHandle)
        {
            // not supported
        }

        private void checkAndRunPresage()
        {
            if (!isPresageRunning())
            {
                runPresage();
            }
        }

        /// <summary>
        /// Deletes the presage.xml file that is in the users folder.
        /// </summary>
        private void deletePresageConfigFile()
        {
            var presageDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.presage";
            var presageFile = presageDir + "\\presage.xml";
            if (File.Exists(presageFile))
            {
                File.Delete(presageFile);
            }
        }

        /// <summary>
        /// Returns the install dir of the Presage word predictor
        /// </summary>
        /// <returns>dir, empty string if not installed</returns>
        private string getPresageInstallDir()
        {
            string result;

            try
            {
                result = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Presage", "", string.Empty).ToString();
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Initializes the Presage word prediction engine.
        /// </summary>
        /// <returns>true on success</returns>
        private bool initPresage()
        {
            bool retVal = true;

            try
            {
                var presageDataDir = UserManager.GetFullPath("WordPredictors\\Presage");

                _presage = new PresageClient();

                var dbFileName = Path.Combine(presageDataDir, _presageSettings.DatabaseFileName);
                _presage.set_config("Presage.Predictors.DefaultSmoothedNgramPredictor.DBFILENAME", dbFileName);

                dbFileName = Path.Combine(presageDataDir, _presageSettings.LearningDatabaseFileName);
                _presage.set_config("Presage.Predictors.UserSmoothedNgramPredictor.DBFILENAME", dbFileName);

                _presage.set_config("Presage.Selector.REPEAT_SUGGESTIONS", "yes");
                _presage.set_config("Presage.ContextTracker.ONLINE_LEARNING", "no");
                _presage.save_config();
            }
            catch (Exception ex)
            {
                Log.Debug("Presage init error " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Checks if Presage wcf tray app is running
        /// </summary>
        /// <returns>true if it is</returns>
        private bool isPresageRunning()
        {
            var pname = Process.GetProcessesByName(PresageProcessName);
            return pname.Length != 0;
        }

        /// <summary>
        /// Runs the Presage WCF tray app
        /// </summary>
        private void runPresage()
        {
            var installDir = getPresageInstallDir();
            if (String.IsNullOrEmpty(installDir))
            {
                return;
            }

            var fullPath = Path.Combine(installDir + "\\bin", PresageWCFTrayAppName);
            Log.Debug(fullPath);
            try
            {
                Process.Start(fullPath);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                Log.Debug("Unable to run Presage. " + ex);
            }
        }

        /// <summary>
        /// Updates class variables with ACAT settings stored
        /// in prefs
        /// </summary>
        /// <param name="prefs">ACAT settings</param>
        private void updateSettings(ACATPreferences prefs)
        {
            PredictionWordCount = prefs.WordPredictionCount;
            FilterPunctuationsEnable = prefs.WordPredictionFilterPunctuations;
            NGram = prefs.WordPredictionNGram;
        }
    }
}