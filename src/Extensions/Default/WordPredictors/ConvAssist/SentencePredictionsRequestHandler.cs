////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SentencePredictionsRequestHandler.cs
//
// Processes sentences predictions requests
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extensions.WordPredictors.ConvAssist.MessageTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACAT.Extensions.WordPredictors.ConvAssist
{
    internal class SentencePredictionsRequestHandler
    {
        private string _prevCurrentWord = null;
        private WordPredictionModes _prevMode = WordPredictionModes.None;
        private string _prevPrevWords = null;
        private List<string> _prevSentencePredictionResults = new();
        private readonly ConvAssistWordPredictor _wordPredictor;
        private readonly ILogger<SentencePredictionsRequestHandler> _logger;

        public SentencePredictionsRequestHandler(ConvAssistWordPredictor wordPredictor, ILogger<SentencePredictionsRequestHandler> logger)
        {
            _wordPredictor = wordPredictor;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount propertys
        /// </summary>
        /// <param name="prevWords">Previous words in the sentence</param>
        /// <param name="currentWord">current word (may be partially spelt out</param>
        /// <param name="success">true if the function was successsful</param>
        /// <returns>A list of predicted words</returns>
        public WordPredictionResponse ProcessPredictionRequest(WordPredictionRequest request)
        {
            StringBuilder preceedingWords = new();

            _logger?.LogInformation(">>> ProcessPredictionRequest called - Type: {Type}, PrevWords: '{PrevWords}', CurrentWord: '{CurrentWord}', Mode: {Mode}", 
                request.PredictionType, request.PrevWords ?? "", request.CurrentWord ?? "", request.WordPredictionMode);

            if (request.PredictionType != PredictionTypes.Sentences)
            {
                _logger?.LogWarning("Request type is not Sentences, returning empty response");
                return new WordPredictionResponse(request, new List<String>(), false);
            }

            WordPredictionResponse response;
            try
            {
                _logger?.LogDebug("_prevMode: {PrevMode}, currentMode: {CurrentMode}", _prevMode, _wordPredictor.GetMode());

                bool modeChanged = _prevMode != _wordPredictor.GetMode();
                bool prevWordsChanged = _prevPrevWords == null || String.Compare(_prevPrevWords, request.PrevWords) != 0;
                bool currentWordChanged = _prevCurrentWord == null || String.Compare(_prevCurrentWord, request.CurrentWord) != 0;

                _logger?.LogDebug("Change detection - ModeChanged: {ModeChanged}, PrevWordsChanged: {PrevWordsChanged}, CurrentWordChanged: {CurrentWordChanged}",
                    modeChanged, prevWordsChanged, currentWordChanged);

                if (modeChanged || prevWordsChanged || currentWordChanged)
                {
                    _prevMode = _wordPredictor.GetMode();
                    _prevPrevWords = request.PrevWords;
                    _prevCurrentWord = request.CurrentWord;

                    IPreferences pref = (_wordPredictor as ISupportsPreferences).GetPreferences();

                    String prevWords = request.PrevWords;
                    String currentWord = request.CurrentWord;

                    //if ((pref as Settings).UseDefaultEncoding)
                    //{
                    //    prevWords = ConvAssistUtils.UTF8EncodingToDefault(prevWords);
                    //    currentWord = ConvAssistUtils.UTF8EncodingToDefault(currentWord);
                    //}

                    preceedingWords.Clear();
                    preceedingWords.Append(prevWords);
                    preceedingWords.Append(currentWord);

                    _logger?.LogInformation("Building prediction request - PreceedingWords: '{PreceedingWords}', CurrentWordEmpty: {IsEmpty}", 
                        preceedingWords.ToString(), String.IsNullOrEmpty(currentWord.Trim()));

                    List<string> result;
                    try
                    {
                        string predictedWords = String.Empty;
                        string predictedSentences = string.Empty;

                        if (String.IsNullOrEmpty(currentWord.Trim()))// && (prevWords.Length + currentWord.Length > 1))
                        {
                            _logger?.LogInformation("CurrentWord is empty, requesting predictions in mode: {Mode}", request.WordPredictionMode);

                            if (request.WordPredictionMode == WordPredictionModes.Sentence)
                            {
                                _logger?.LogInformation("Calling SendMessageConvAssistSentencePrediction with text: '{Text}'", preceedingWords.ToString());
                                predictedSentences = _wordPredictor.SendMessageConvAssistSentencePrediction(preceedingWords.ToString(),
                                                                                    request.WordPredictionMode);
                                _logger?.LogInformation("ConvAssist sentences response received - Length: {Length}, Content: {Content}", 
                                    predictedSentences?.Length ?? 0, predictedSentences);
                            }
                            else
                            {
                                _logger?.LogInformation("Calling SendMessageConvAssistWordPrediction with text: '{Text}'", preceedingWords.ToString());
                                predictedWords = _wordPredictor.SendMessageConvAssistWordPrediction(preceedingWords.ToString(),
                                                                                request.WordPredictionMode);
                                predictedSentences = predictedWords;
                                _logger?.LogInformation("ConvAssist words response: {PredictedWords}", predictedWords);
                            }

                            try
                            {
                                _logger?.LogDebug("Processing sentence predictions, predictedSentences: '{Sentences}'", predictedSentences);
                                result = ProcessSentencesPredictions(predictedSentences, currentWord);
                                _logger?.LogInformation("ProcessSentencesPredictions returned {Count} results: [{Results}]", 
                                    result.Count, String.Join(", ", result));
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogError(ex, "Error in ProcessSentencesPredictions");
                                result = new List<string>();
                            }

                            _prevSentencePredictionResults = result;
                        }
                        else
                        {
                            _logger?.LogInformation("CurrentWord is NOT empty ('{CurrentWord}'), returning empty result", currentWord);
                            result = new List<string>();
                            _prevSentencePredictionResults = result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Exception in sentence prediction processing");
                        result = new List<string>();
                        _prevSentencePredictionResults = result;
                    }

                    var s = String.Join(", ", result);
                    _logger?.LogInformation("Creating response with {Count} predictions: [{Results}]", result.Count, s);
                    response = new WordPredictionResponse(request, result, true);
                }
                else
                {
                    _logger?.LogInformation("Nothing changed. returning previous {Count} results", _prevSentencePredictionResults.Count);
                    response = new WordPredictionResponse(request, _prevSentencePredictionResults, true);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ConvAssist Predict Exception");

                _prevSentencePredictionResults = new List<string>();
                response = new WordPredictionResponse(request, new List<String>(), false);
            }
            finally
            {
            }

            _logger?.LogInformation("<<< ProcessPredictionRequest returning response with {Count} predictions", 
                response.Results != null ? System.Linq.Enumerable.Count(response.Results) : 0);
            return response;
        }

        /// <summary>
        /// Splits the types of predictions
        /// </summary>
        /// <param name="predictions">Result from ConvAssist</param>
        /// <param name="currentWord">Word if the cursors is in the middle of a word</param>
        /// <returns>List of predictions with a keyword to tell apart wach type</returns>
        private List<string> ProcessSentencesPredictions(string predictions, string currentWord)
        {
            StringBuilder resultFullPredictionWords = new();
            WordAndCharacterPredictionResponse answer = new();
            var retVal = new List<string>();

            // Check for empty response before deserializing
            if (string.IsNullOrWhiteSpace(predictions))
            {
                return retVal; // Return empty list
            }

            try
            {
                answer = JsonSerializer.Deserialize<WordAndCharacterPredictionResponse>(predictions);
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Log and return empty list if deserialization fails
                return retVal;
            }

            List<string> predictSenetnces = new();
            List<string> predictLettersSentence = new();
            int i = 0;

            if (answer != null)
            {
                predictSenetnces = answer.PredictedSentence.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                predictLettersSentence = answer.NextCharactersSentence.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
            }
            string[] sentenceChPred;
            string[] sentencePred;

            // Keyword to split between predictions
            retVal.Add("&SENTENCES");
            List<KeyValuePair<string, double>> SentenceList = new();
            SentenceList = ConvAssistUtils.ToList(predictSenetnces);
            sentencePred = new string[SentenceList.Count];
            foreach (KeyValuePair<string, double> element in SentenceList)
            {
                sentencePred[i] = ConvAssistUtils.CleanText(element.Key, true, false); //ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharactersSentences(element.Key), true);
                i += 1;
            }
            for (int count = 0, ii = 0; count < 5 && ii < SentenceList.Count(); ii++)
            {
                if (sentencePred[ii].Length > 0)
                    retVal.Add(sentencePred[ii]);
                count++;
            }

            // Keyword to split between predictions, under consideration not currently used since already had letters predictions
            retVal.Add("&SENTENCESLETTERS");
            try
            {
                //Create Dictionary of each to set the number value as a Double
                List<KeyValuePair<string, double>> SentenceChList = new();
                SentenceChList = ConvAssistUtils.ToList(predictLettersSentence);
                sentenceChPred = new string[SentenceChList.Count];

                foreach ((KeyValuePair<string, double> item, int index) in SentenceChList.Select((item, index) => (item, index)))
                {
                    sentenceChPred[index] = ConvAssistUtils.CleanText(item.Key, false, false); //ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharacters(item.Key));
                }
            }
            catch (Exception sentencesLetters)
            {
                _logger?.LogError(sentencesLetters, "ConvAssist Predict sentencesLetters");
            }
            return retVal;
        }
    }
}