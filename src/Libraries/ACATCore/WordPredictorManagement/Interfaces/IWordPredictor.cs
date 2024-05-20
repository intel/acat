////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Lib.Core.WordPredictionManagement
{
    /// <summary>
    /// Word prediction mode changed
    /// </summary>
    /// <param name="newMode">The new Mode</param>
    public delegate void ModeChangedDelegate(WordPredictionModes newMode);

    /// <summary>
    /// Contains the probabilities for the next letters
    /// </summary>
    /// <param name="letterList">List of letters and probs</param>
    /// <param name="lettersChanged">has the list changed?</param>
    public delegate void NextLetterProbabilitiesDelegate(List<KeyValuePair<string, double>> letterList, bool lettersChanged);

    /// <summary>
    /// Contains the probabilities for the next word
    /// </summary>
    /// <param name="wordList">list of words with probabilities</param>
    /// <param name="wordsChanged">has the word list changed?</param>
    public delegate void NextWordProbabilitiesDelegate(List<KeyValuePair<string, double>> wordList, bool wordsChanged);

    /// <summary>
    /// Handles async responses from ConvAssist
    /// </summary>
    /// <param name="response">Response object</param>
    public delegate void WordPredictionAsyncResponseDelegate(WordPredictionResponse response);

    /// <summary>
    /// Modes in which the predictor can work
    /// </summary>
    public enum WordPredictionModes
    {
        None,
        Sentence,
        Shorthand,
        CannedPhrases
    }

    public enum WordPredictorMessageTypes
    {
        None,
        SetParam,
        NextWordPredictionRequest,
        NextWordPredictionResponse,
        NextSentencePredictionRequest,
        NextSentencePredictionResponse,
        LearnWords,
        LearnCanned,
        LearnShorthand,
        LearnSentence,
        ForceQuitApp,
        CRGKeywordPredictionRequest,
        CRGSentencePredictionRequest
    }

    /// <summary>
    /// Interface to Word prediction engines.  A word predictor predicts
    /// the next word using the previous n-words as context.  Some word predictors
    /// also support learning where the engine tunes its internal model for
    /// more accurate predictions based on the user.
    /// All WordPredictors should implement this interface.
    /// </summary>
    public interface IWordPredictor : IDisposable
    {
        /// <summary>
        /// Event to indicate mode has changed
        /// </summary>
        event ModeChangedDelegate EvtModeChanged;

        /// <summary>
        /// Event to notify next letter probabilities
        /// </summary>
        event NextLetterProbabilitiesDelegate EvtNotifyNextLetterProbabilities;

        /// <summary>
        /// Event to notify next word probabilities
        /// </summary>
        event NextWordProbabilitiesDelegate EvtNotifyNextWordProbabilities;

        /// <summary>
        /// Event to notify response received from ConvAssist
        /// </summary>
        event WordPredictionAsyncResponseDelegate EvtWordPredictionAsyncResponse;

        /// <summary>
        /// Returns a descriptor which contains a user readable name, a
        /// short textual description and a unique GUID.
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Whether puncutations should be a part of the prediction. For
        /// instance, some word predictors may return a question mark as a
        /// prediction if it detects the end of a sentence.
        /// </summary>
        bool FilterPunctuationsEnable { get; set; }

        /// <summary>
        /// How many previous words to use for prediction.
        /// </summary>
        int NGram { get; set; }

        /// <summary>
        /// Sets the number of predicted words to return.
        /// </summary>
        int PredictionWordCount { get; set; }

        /// <summary>
        /// Returns a flag indicating whether the word predictor supports
        /// dynamic learning.
        /// </summary>
        bool SupportsLearning { get; }

        bool SupportsPredictSync { get; }

        /// <summary>
        /// Gets the current mode for the predictor
        /// </summary>
        /// <returns></returns>
        WordPredictionModes GetMode();

        /// <summary>
        /// Initialize the word predictor.  Language is optional,
        /// if not specified, use the current culture
        /// </summary>
        /// <param name="ci">Language for the model</param>
        /// <returns>true on success, false on failure</returns>
        bool Init(CultureInfo ci);

        /// <summary>
        /// Execute post init operations.
        /// </summary>
        /// <returns>true on success</returns>
        bool PostInit();

        /// <summary>
        /// Add the text to the word prediction models for better
        /// prediction.  The text would typically represent something
        /// the user typed.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool Learn(String text, WordPredictorMessageTypes RequestType);

        /// <summary>
        /// Call this on a context switch to a document.  Creates
        /// a temporary model
        /// </summary>
        /// <param name="text">Text from the document</param>
        /// <returns>Handle.  Pass this to unload</returns>
        int LoadContext(String text);

        /// <summary>
        /// Reset to factory default settings
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool LoadDefaultSettings();

        /// <summary>
        /// Load settings from a file maintained by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool LoadSettings(String configFileDirectory);

        WordPredictionResponse Predict(WordPredictionRequest req);

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount property
        /// </summary>
        /// <param name="prevNWords">Previous words in the sentence</param>
        /// <param name="lastWord">current word (may be partially spelt out</param>
        /// <returns>A list of predicted words</returns>
        bool PredictAsync(WordPredictionRequest req);

        /// <summary>
        /// Save the word predictor settings to a file that is maintained
        /// by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool SaveSettings(String configFileDirectory);

        /// <summary>
        /// Set the mode in which the predictor will work
        /// </summary>
        /// <param name="wordPredictionMode"></param>
        void SetMode(WordPredictionModes wordPredictionMode);

        /// <summary>
        /// Unload context
        /// </summary>
        /// <param name="contextHandle">Handle returned by load</param>
        void UnloadContext(int contextHandle);
    }
}