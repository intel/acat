////////////////////////////////////////////////////////////////////////////
// <copyright file="PresageWordPredictorBase.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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

using ACAT.Extensions.Default.WordPredictors.PresageBase.PresageService;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ACAT.Extensions.Default.WordPredictors.PresageBase
{
    /// <summary>
    /// Base class for all word prediction extensions that use
    /// the Presage word predictor for next word prediction.
    /// Presage is an intelligent predictive
    /// text engine created by Matteo Vescovi.
    /// http://presage.sourceforge.net/
    /// </summary>
    public class PresageWordPredictorBase : IWordPredictor, ISupportsPreferences, IExtension
    {
        /// <summary>
        /// Name of the presage config file
        /// </summary>
        public const String PresageConfigFileName = "presage.xml";

        /// <summary>
        /// Name of the presage database file for ngram prediction
        /// </summary>
        protected String DatabaseFileName = "database.db";

        /// <summary>
        /// Name of the file where learnt phrases are stored
        /// </summary>
        protected String LearningDBFileName = "learn.db";

        /// <summary>
        /// Name of the presage process which runs as a systray app
        /// </summary>
        protected const String PresageProcessName = "presage_wcf_service_system_tray";

        /// <summary>
        /// Root folder for presage files and extensions
        /// </summary>
        protected const String PresageRoot = "WordPredictors\\Presage";

        /// <summary>
        /// Name of the presage service executable
        /// </summary>
        protected const String PresageWCFTrayAppName = "presage_wcf_service_system_tray.exe";

        /// <summary>
        /// Current culture (language) for word prediction
        /// </summary>
        protected CultureInfo cultureInfo;

        /// <summary>
        /// The Presage object
        /// </summary>
        protected PresageClient presage;

        /// <summary>
        /// Used to invoke methods/properties in the agent
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Buffer to hold byte representation of string for encoding conversion
        /// </summary>
        private byte[] _byteBuffer = new byte[10240];

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        protected PresageWordPredictorBase()
        {
            _invoker = new ExtensionInvoker(this);
        }

        /// <summary>
        /// Returns the ACAT descriptor for this class
        /// </summary>
        public virtual IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Get or sets the value to indicate whether punctuations
        /// should be filtered.
        /// This property is not used.
        /// </summary>
        public virtual bool FilterPunctuationsEnable { get; set; }

        /// <summary>
        /// Gets or sets the NGram for word prediction.
        /// Not used
        /// </summary>
        public virtual int NGram
        {
            get { return -1; }

            set { }
        }

        /// <summary>
        /// Gets or sets the number of words in the suggested
        /// prediction list.
        /// </summary>
        public virtual int PredictionWordCount
        {
            get
            {
                int result = -1;

                try
                {
                    var configValue = presage.get_config("Presage.Selector.SUGGESTIONS");
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
                    presage.set_config("Presage.Selector.SUGGESTIONS", value.ToString());
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Gets value that indicates whether this word
        /// predictor supports learning.  Returns true.
        /// The Learn() function can be called to add
        /// sentences to the user's word prediction
        /// model for better word prediction.
        /// </summary>
        public virtual bool SupportsLearning
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public virtual bool SupportsPreferencesDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the word prediction settings object
        /// </summary>
        protected PresageSettingsBase presageSettings { get; set; }

        /// <summary>
        /// Disposer for this class
        /// </summary>
        public virtual void Dispose()
        {
            try
            {
                //killPresage();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Override this to return the default preferences object for the word predictor
        /// </summary>
        /// <returns>default preferences object</returns>
        public virtual IPreferences GetDefaultPreferences()
        {
            return null;
        }

        /// <summary>
        /// Returns the invoker object
        /// </summary>
        /// <returns>invoker object</returns>
        public virtual ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Override this to return the preferences object for the word predictor
        /// </summary>
        /// <returns>preferences object</returns>
        public virtual IPreferences GetPreferences()
        {
            return null;
        }

        /// <summary>
        /// Performs initialization.  Must be called first.
        /// Sets word prediction parameters and initializes Presage.
        /// CultureInfo parameter defines the language.  If word
        /// prediction database is not found for the specified language
        /// English is used as the default.
        /// </summary>
        /// <param name="ci">language for word prediction</param>
        /// <returns>true on success</returns>
        public bool Init(CultureInfo ci)
        {
            bool retVal = false;

            try
            {
                Attributions.Add("PRESAGE",
                                "Predictive text functionality is powered by Presage, the " +
                                "intelligent predictive text engine created by Matteo Vescovi. " +
                                "(http://presage.sourceforge.net/)");

                upgradeFromPreviousVersion(ci);

                deleteConfigFile();

                checkAndRunPresage();

                Log.Debug("Calling initPresage()");

                retVal = initPresage(ci);

                Log.Debug("Returned from initPresage() " + retVal);

                if (!retVal)
                {
                    int numRetries = 2;

                    for (int ii = 0; ii < numRetries; ii++)
                    {
                        Log.Debug("initPresage() returned false.  Will retry");

                        deleteConfigFile();

                        killPresage();

                        checkAndRunPresage();

                        Log.Debug("Calling initPresage() once more");

                        retVal = initPresage(ci);
                        if (retVal)
                        {
                            Log.Debug("initPresage() succeeded!");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error initializing Presage. " + ex);
            }

            return retVal;
        }

        /// <summary>
        /// If learning is enabled, learns the text by adding 
        /// to the user's personal database
        /// </summary>
        /// <param name="text">text to add</param>
        /// <returns>true on success</returns>
        public bool Learn(String text)
        {
            bool result = true;

            if (Common.AppPreferences.EnableWordPredictionDynamicModel)
            {
                result = learn(text);
            }

            return result;
        }

        /// <summary>
        /// Adds the specified text to the user's personal
        /// word prediction model to learn the user's writing
        /// style.  This makes word prediciton more relevant.
        /// </summary>
        /// <param name="text">Text to add</param>
        /// <returns>true on success</returns>
        protected virtual bool learn(String text)
        {
            bool result = false;

            try
            {
                presage.learn(text);
                result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
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
        public virtual int LoadContext(String text)
        {
            // not supported

            return -1;
        }

        /// <summary>
        /// Updates extensions with factory default settings.
        /// </summary>
        /// <returns></returns>
        public virtual bool LoadDefaultSettings()
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
        public virtual bool LoadSettings(String settingsFilePath)
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
            bool success = true;

            var wordList = predict(prevWords, currentWord, ref success);
            if (!success)
            {
                Log.Debug("Prediction error.  Will initialize again");

                var retVal = Init(cultureInfo);
                if (retVal)
                {
                    wordList = predict(prevWords, currentWord, ref success);
                }
            }

            return wordList;
        }

        /// <summary>
        /// Saves the word predictor settings to a file that is maintained
        /// by the word predictor.
        /// </summary>
        /// <param name="settingsFilePath">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        public virtual bool SaveSettings(String settingsFilePath)
        {
            updateSettings(Common.AppPreferences);
            return true;
        }

        /// <summary>
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool ShowPreferencesDialog()
        {
            return true;
        }

        /// <summary>
        /// Unloads previously loaded context (see LoadContext).
        /// Not used.
        /// </summary>
        /// <param name="contextHandle">the handle</param>
        public virtual void UnloadContext(int contextHandle)
        {
            // not supported
        }

        /// <summary>
        /// Checks if Presage is already running, if not launches it.
        /// </summary>
        protected void checkAndRunPresage()
        {
            Log.Debug("Checking if presage is running");
            if (!isPresageRunning())
            {
                Log.Debug("It is not running");

                runPresage();
            }
        }

        /// <summary>
        /// Converts default encoding to UTF8
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string defaultEncodingToUTF8(string input)
        {
            int length = Encoding.Default.GetBytes(input, 0, input.Length, _byteBuffer, 0);

            return Encoding.UTF8.GetString(_byteBuffer, 0, length);
        }

        /// <summary>
        /// Gets path to the database file relative to
        /// the application run folder
        /// </summary>
        /// <param name="prefix">language prefix for the folder</param>
        /// <returns>full path to the file</returns>
        protected String getAppRelativeDBPath(String language, String fileName)
        {
            String fullPath = Path.Combine(FileUtils.ACATPath, language);
            fullPath = Path.Combine(fullPath, PresageRoot);
            return Path.Combine(fullPath, fileName);
        }

        /// <summary>
        /// Returns the Presage database file name (full path)
        /// for the specified language.  First checks for the
        /// existence of the file in the user folder and then in the
        /// application language folder
        /// </summary>
        /// <returns>full path to the file</returns>
        protected String getdbFilePath(String fileName)
        {
            String fullPath = getAppRelativeDBPath(Thread.CurrentThread.CurrentUICulture.Name, fileName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            fullPath = getAppRelativeDBPath(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, fileName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            return String.Empty;
        }

        protected virtual String getDefaultPresageConfigFilePath()
        {
            var presageDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.presage";
            return presageDir + "\\presage.xml";
        }

        /// <summary>
        /// Returns the Presage database file name (full path)
        /// for the specified language that will store the words
        /// dynamically learned. THe parameter is either
        /// the culture name or the two-letter ISO name
        /// </summary>
        /// <param name="language">language</param>
        /// <returns>full path to the file</returns
        protected virtual String getLearningDBFilePath(String language, String fileName)
        {
            var fullPath = UserManager.GetFullPath(language);

            fullPath = Path.Combine(fullPath, PresageRoot);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return Path.Combine(fullPath, fileName);
        }

        /// <summary>
        /// Returns the install dir of the Presage word predictor
        /// </summary>
        /// <returns>dir, empty string if not installed</returns>
        protected string getPresageInstallDir()
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

        protected String getUserRelativePath(String language, String fileName, bool createDirIfDoesNotExist = false)
        {
            var fullPath = UserManager.GetFullPath(language);

            if (!Directory.Exists(fullPath) && createDirIfDoesNotExist)
            {
                Directory.CreateDirectory(fullPath);
            }

            return Path.Combine(fullPath, fileName);
        }

        /// <summary>
        /// Sets presage config parameters to point to the
        /// database file for the specified culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>true on success</returns>
        protected virtual bool initDatabase(CultureInfo ci)
        {
            bool retVal = true;

            try
            {
                // check if presage database file for the specified language exists.
                var dbFileName = getDatabaseFilePath(ci, DatabaseFileName);

                if (String.IsNullOrEmpty(dbFileName))
                {
                    return false;
                }

                cultureInfo = ci;

                presage.set_config("Presage.Predictors.DefaultSmoothedNgramPredictor.DBFILENAME", dbFileName);

                presage.set_config("Presage.PredictorRegistry.PREDICTORS",
                        "DefaultSmoothedNgramPredictor UserSmoothedNgramPredictor DefaultRecencyPredictor");

                var learningdbFileName = getLearningDBFilePath(ci.TwoLetterISOLanguageName, LearningDBFileName);
                presage.set_config("Presage.Predictors.UserSmoothedNgramPredictor.DBFILENAME", learningdbFileName);

                presage.set_config("Presage.Selector.REPEAT_SUGGESTIONS", "yes");
                presage.set_config("Presage.ContextTracker.ONLINE_LEARNING", "no");
            }
            catch (Exception ex)
            {
                Log.Debug("Error initializing Presage. " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Checks if Presage wcf tray app is running
        /// </summary>
        /// <returns>true if it is</returns>
        protected bool isPresageRunning()
        {
            var pname = Process.GetProcessesByName(PresageProcessName);
            return pname.Length != 0;
        }

        /// <summary>
        /// Kills the presage process
        /// </summary>
        protected void killPresage()
        {
            try
            {
                var processes = Process.GetProcessesByName(PresageProcessName);
                if (processes.Length > 0)
                {
                    Log.Debug("Killing presage");
                    processes[0].Kill();
                }
                else
                {
                    Log.Debug("Presage is not running");
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Checks to see how much of the prefix matches with the
        /// specified word. The preference setting
        /// WordPredictionFilterMatchPrefixLengthAdjust controls
        /// how many chars to match.  If a match if found, returns true
        /// </summary>
        /// <param name="prefix">partially entered word</param>
        /// <param name="word">word to match with</param>
        /// <returns>true if a match was found</returns>
        protected virtual bool matchPrefix(String prefix, String word)
        {
            if (!Common.AppPreferences.WordPredictionFilterMatchPrefix)
            {
                return true;
            }

            prefix = prefix.Trim();
            if (String.IsNullOrEmpty(prefix))
            {
                return true;
            }

            int numCharsToMatch = prefix.Length - Common.AppPreferences.WordPredictionFilterMatchPrefixLengthAdjust;
            if (numCharsToMatch <= 0)
                numCharsToMatch = prefix.Length;

            if (numCharsToMatch > 0)
            {
                if (word.Length > numCharsToMatch)
                    word = word.Substring(0, numCharsToMatch);
                if (prefix.Length > numCharsToMatch)
                    prefix = prefix.Substring(0, numCharsToMatch);
            }

            return (word.Length > prefix.Length) ?
                word.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) :
                prefix.StartsWith(word, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount property
        /// </summary>
        /// <param name="prevWords">Previous words in the sentence</param>
        /// <param name="currentWord">current word (may be partially spelt out</param>
        /// <param name="success">true if the function was successsful</param>
        /// <returns>A list of predicted words</returns>
        protected virtual IEnumerable<String> predict(String prevWords, String currentWord, ref bool success)
        {
            Log.Debug("Predict for: " + prevWords + " " + currentWord);

            var retVal = new List<string>();

            success = true;

            try
            {
                prevWords = UTF8EncodingToDefault(prevWords);
                currentWord = UTF8EncodingToDefault(currentWord);

                prevWords = prevWords.Replace('\'', '´');
                currentWord = currentWord.Replace('\'', '´');

                string[] prediction = presage.predict(prevWords, currentWord);

                for (int ii = 0; ii < prediction.Length; ii++)
                {
                    prediction[ii] = defaultEncodingToUTF8(prediction[ii]);
                }

                var predictionList = prediction.ToList();

                for (int count = 0, ii = 0; count < PredictionWordCount && ii < predictionList.Count(); ii++)
                {
                    if (matchPrefix(currentWord, predictionList[ii]))
                    {
                        //LogDebug(String.Format("Prediction["+ ii + "] = " + predictions[ii].Term));
                        retVal.Add(predictionList[ii]);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Log.Debug("Presage Predict Exception " + ex);
                retVal = new List<string>();
            }

            return retVal;
        }

        /// <summary>
        /// Runs the Presage WCF tray app
        /// </summary>
        protected void runPresage()
        {
            var installDir = getPresageInstallDir();
            Log.Debug("presage installdir is " + installDir);

            if (String.IsNullOrEmpty(installDir))
            {
                return;
            }

            var fullPath = Path.Combine(installDir + "\\bin", PresageWCFTrayAppName);
            Log.Debug(fullPath);
            try
            {
                Log.Debug("Starting preage " + fullPath);

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
        protected virtual void updateSettings(ACATPreferences prefs)
        {
            PredictionWordCount = prefs.WordPredictionCount;
            FilterPunctuationsEnable = prefs.WordPredictionFilterPunctuations;
            NGram = prefs.WordPredictionNGram;
        }

        protected virtual void upgradeFromPreviousVersion(CultureInfo ci)
        {
        }

        /// <summary>
        /// Converts UTF8 encoded string to the default encoding
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>converted string</returns>
        protected string UTF8EncodingToDefault(string input)
        {
            int length = Encoding.UTF8.GetBytes(input, 0, input.Length, _byteBuffer, 0);

            return Encoding.Default.GetString(_byteBuffer, 0, length);
        }

        /// <summary>
        /// Delete the presage config file
        /// </summary>
        private void deleteConfigFile()
        {
            try
            {
                var file = getDefaultPresageConfigFilePath();
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the path to the database file for the specified
        /// culture. Returns empty string if no database file was
        /// found for the specified culture
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>database file path</returns>
        protected virtual String getDatabaseFilePath(CultureInfo ci, string databaseFileName)
        {
            var fullPath = getAppRelativeDBPath(ci.Name, databaseFileName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            fullPath = getAppRelativeDBPath(ci.TwoLetterISOLanguageName, databaseFileName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            fullPath = String.Empty;

            return fullPath;
        }

        /// <summary>
        /// Initializes the Presage word prediction engine.
        /// </summary>
        /// <param name="ci">language for word prediction</param>
        /// <returns>true on success</returns>
        private bool initPresage(CultureInfo ci)
        {
            bool retVal = true;

            try
            {
                presage = new PresageClient();

                if (ci == null)
                {
                    ci = Thread.CurrentThread.CurrentUICulture;
                }

                cultureInfo = ci;

                Log.Debug("Calling initdatabase");
                retVal = initDatabase(ci);
                Log.Debug("Returned from initdb " + retVal);

                if (retVal)
                {
                    presage.save_config();
                }

                // this is a workaround for a bug in presage.  If you
                // switch languages, it seems to use the database from the
                // previous language
                deleteConfigFile();
            }
            catch (Exception ex)
            {
                Log.Debug("Presage init error " + ex);
                retVal = false;
            }

            Log.Debug("Returning from initpresage " + retVal);

            return retVal;
        }
    }
}