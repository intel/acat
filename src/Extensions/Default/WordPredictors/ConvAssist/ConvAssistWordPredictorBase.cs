////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistWordPredictorBase.cs
//
// Base class for all word prediction extensions that use
// the ConvAsasist word predictor for next word prediction.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    /// <summary>
    /// Base class for all word prediction extensions that use
    /// the ConvAssist word predictor for next word prediction.
    /// </summary>
    public abstract class ConvAssistWordPredictorBase : IWordPredictor, ISupportsPreferences, IExtension
    {
        protected WordPredictionModes _wordPredictionMode = WordPredictionModes.Sentence;

        /// <summary>
        /// Current culture (language) for word prediction
        /// </summary>
        protected CultureInfo cultureInfo;

        /// <summary>
        /// Used to invoke methods/properties in the agent
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        protected ConvAssistWordPredictorBase()
        {
            _invoker = new ExtensionInvoker(this);
        }

        public event ModeChangedDelegate EvtModeChanged;

        public event NextLetterProbabilitiesDelegate EvtNotifyNextLetterProbabilities;

        public event NextWordProbabilitiesDelegate EvtNotifyNextWordProbabilities;

        public event WordPredictionAsyncResponseDelegate EvtWordPredictionAsyncResponse;

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
        public virtual int PredictionLetterCount
        {
            get
            {
                return 10;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the number of words in the suggested
        /// prediction list.
        /// </summary>
        public virtual int PredictionWordCount
        {
            get
            {
                return 10;
            }

            set
            {
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

        public abstract bool SupportsPredictSync { get; }

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
        protected Settings convAssistSettings { get; set; }

        /// <summary>
        /// Disposer for this class
        /// </summary>
        public virtual void Dispose()
        {
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
        /// Gets the current mode for the predictor
        /// </summary>
        /// <returns></returns>
        public WordPredictionModes GetMode()
        {
            return _wordPredictionMode;
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
        /// Sets word prediction parameters and initializes ConvAssist.
        /// CultureInfo parameter defines the language.  If word
        /// prediction database is not found for the specified language
        /// English is used as the default.
        /// </summary>
        /// <param name="ci">language for word prediction</param>
        /// <returns>true on success</returns>
        public virtual bool Init(CultureInfo ci)
        {
            return true;
        }

        /// <summary>
        /// Perform post init operations.
        /// </summary>
        /// <returns></returns>
        public virtual bool PostInit()
        {
            return true;
        }

        /// <summary>
        /// If learning is enabled, learns the text by adding
        /// to the user's personal database
        /// </summary>
        /// <param name="text">text to add</param>
        /// <returns>true on success</returns>
        public bool Learn(String text, WordPredictorMessageTypes requestType)
        {
            bool result = true;

            if (Common.AppPreferences.WordPredictionEnableLearn)
            {
                result = learn(text, requestType);
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
        /// Notify subscribers of next letter probabilities
        /// </summary>
        /// <param name="letterList">List of letters and their probabilities</param>
        /// <param name="wordsChanged">Has the word list changed?</param>
        public void NotifyNextLetterProbabilities(List<KeyValuePair<string, double>> letterList, bool wordsChanged)
        {
            EvtNotifyNextLetterProbabilities?.Invoke(letterList, wordsChanged);
        }

        /// <summary>
        /// Notify subscribers of next word probabilities
        /// </summary>
        /// <param name="wordList">List of words and their probabilities</param>
        /// <param name="wordsChanged">Has the word list changed?</param>
        public void NotifyNextWordProbabilities(List<KeyValuePair<string, double>> wordList, bool wordsChanged)
        {
            EvtNotifyNextWordProbabilities?.Invoke(wordList, wordsChanged);
        }

        public abstract WordPredictionResponse Predict(WordPredictionRequest req);

        public abstract bool PredictAsync(WordPredictionRequest req);

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
        /// Set the mode in which the predictor will work
        /// </summary>
        /// <param name="wordPredictionMode"></param>
        public void SetMode(WordPredictionModes wordPredictionMode)
        {
            var oldMode = GetMode();
            if (wordPredictionMode != oldMode)
            {
                _wordPredictionMode = wordPredictionMode;
                EvtModeChanged?.Invoke(wordPredictionMode);
            }
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
        /// Adds the specified text to the user's personal
        /// word prediction model to learn the user's writing
        /// style.  This makes word prediciton more relevant.
        /// </summary>
        /// <param name="text">Text to add</param>
        /// <returns>true on success</returns>
        protected virtual bool learn(String text, WordPredictorMessageTypes requestType)
        {
            return false;
        }

        protected void notifyPredictionResults(WordPredictionResponse response)
        {
            EvtWordPredictionAsyncResponse?.Invoke(response);
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
        }
    }
}