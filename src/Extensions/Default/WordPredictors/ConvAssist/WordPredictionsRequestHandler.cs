////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WordPredictionsRequestHandler.cs
//
// Handles requests for next word prediction
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    internal class WordPredictionsRequestHandler
    {
        /// <summary>
        /// Previous mode
        /// </summary>
        private WordPredictionModes _prevMode = WordPredictionModes.None;

        /// <summary>
        /// Previous words saved
        /// </summary>
        private string _prevContext = null;

        /// <summary>
        /// Previous predictions results
        /// </summary>
        private List<string> PrevWordPredictionResults = new List<string>();

        /// <summary>
        /// Word predictor object
        /// </summary>
        private readonly ConvAssistWordPredictor WordPredictor;

        public WordPredictionsRequestHandler(ConvAssistWordPredictor wordPredictor)
        {
            WordPredictor = wordPredictor;
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
            List<string> result;

            Log.Debug("Predict for: " + request.Context);

            if (request.PredictionType != PredictionTypes.Words)
            {
                return new WordPredictionResponse(request, new List<String>(), false);
            }

            WordPredictionResponse response;
            try
            {
                if (_prevMode != WordPredictor.GetMode() ||
                    _prevContext == null ||
                    String.Compare(_prevContext, request.Context) != 0)
                {
                    _prevMode = WordPredictor.GetMode();
                    _prevContext = request.Context;

                    var pref = (WordPredictor as ISupportsPreferences).GetPreferences();

                    String context = request.Context;
                    if ((pref as Settings).UseDefaultEncoding)
                    {
                        context = ConvAssistUtils.UTF8EncodingToDefault(context);
                    }

                    try
                    {
                        Log.Debug("ConvAssist Mode: " + request.WordPredictionMode);

                        string predictedWords = String.Empty;

                        predictedWords = WordPredictor.SendMessageConvAssistWordPrediction(context, request.WordPredictionMode, request.CRG);

                        Log.Debug("ConvAssist Words response: " + predictedWords);

                        try
                        {
                            result = ProcessWordPredictions(WordPredictor, predictedWords);//, currentWord);
                        }
                        catch (Exception)
                        {
                            result = new List<string>();
                        }
                        PrevWordPredictionResults = result;
                    }
                    catch (Exception)
                    {
                        result = new List<string>();
                        PrevWordPredictionResults = result;
                    }

                    response = new WordPredictionResponse(request, result, true);
                }
                else
                {
                    response = new WordPredictionResponse(request, PrevWordPredictionResults, true);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("ConvAssist Predict Exception " + ex);
                response = new WordPredictionResponse(request, new List<String>(), false);
            }
            finally
            {
            }

            return response;
        }

        /// <summary>
        /// Splits the types of predictions
        /// </summary>
        /// <param name="predictions">Result from ConvAssist</param>
        /// <param name="currentWord">Word if the cursors is in the middle of a word</param>
        /// <returns>List of predictions with a keyword to tell apart wach type</returns>
        private List<string> ProcessWordPredictions(ConvAssistWordPredictor wordPredictor, string predictions)//, string currentWord)
        {
            StringBuilder resultFullPredictionWords = new StringBuilder();
            WordAndCharacterPredictionResponse answer = new WordAndCharacterPredictionResponse();
            var retVal = new List<string>();
            answer = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(predictions);
            List<string> predictWords = new List<string>();
            List<string> predictLetters = new List<string>();
            int i = 0;

            if (answer != null)
            {
                predictWords = answer.PredictedWords.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
                predictLetters = answer.NextCharacters.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
            }

            string[] wordsPred;
            string[] letterPred;

            try
            {
                // Keyword to split between predictions
                retVal.Add("&WORDS");

                //Create Dictionary of each to set the number value as a Double
                List<KeyValuePair<string, double>> WordsList = new List<KeyValuePair<string, double>>();
                WordsList = ConvAssistUtils.ToList(predictWords);
                wordPredictor.NotifyNextWordProbabilities(WordsList, true);

                //Adding all elements into one single string
                wordsPred = new string[WordsList.Count];

                foreach (var element in WordsList)
                {
                    wordsPred[i] = ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharacters(element.Key));
                    i += 1;
                }
                for (int ii = 0; ii < wordsPred.Length; ii++)
                {
                    var pref = (wordPredictor as ISupportsPreferences).GetPreferences();

                    if ((pref as Settings).UseDefaultEncoding)
                    {
                        wordsPred[ii] = ConvAssistUtils.DefaultEncodingToUTF8(wordsPred[ii]);
                    }

                    if (!String.IsNullOrEmpty((pref as Settings).FilterChars))
                    {
                        wordsPred[ii] = ConvAssistUtils.FilterChars((pref as Settings).FilterChars, wordsPred[ii]);
                    }
                }
                foreach (var element in wordsPred)
                {
                    resultFullPredictionWords.Append(ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharacters(element)) + ",");
                }
                var tempList = resultFullPredictionWords.ToString();
                if (tempList.Length > 0)
                {
                    tempList = tempList.Substring(0, tempList.Length - 1);
                }
                var listWords = tempList.Split(',');

                retVal.AddRange(resultFullPredictionWords.ToString().Split(','));

                /*
                for (int count = 0, ii = 0; count < wordPredictor.PredictionWordCount && ii < listWords.Count(); ii++)
                {
                    if (ConvAssistUtils.MatchPrefix(currentWord, listWords[ii]) && listWords[ii].Length > 0)
                    {
                        retVal.Add(listWords[ii]);
                        count++;
                    }
                }*/
            }
            catch (Exception words)
            {
                Log.Debug("ConvAssist Predict Words " + words);
            }
            // Keyword to split between predictions
            retVal.Add("&LETTERS");
            try
            {
                //Create Dictionary of each to set the probability number value as a Double
                List<KeyValuePair<string, double>> LetterList = new List<KeyValuePair<string, double>>();
                LetterList = ConvAssistUtils.ToList(predictLetters);
                wordPredictor.NotifyNextLetterProbabilities(LetterList, true);
                i = 0;
                letterPred = new string[LetterList.Count];
                foreach (var element in LetterList)
                {
                    letterPred[i] = ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharacters(element.Key));
                    i += 1;
                }
                for (int count = 0, ii = 0; count < wordPredictor.PredictionLetterCount && ii < LetterList.Count(); ii++)
                {
                    if (LetterList[ii].Key.Length > 0)
                    {
                        retVal.Add(LetterList[ii].Key);
                    }
                    count++;
                }
            }
            catch (Exception letters)
            {
                Log.Debug("ConvAssist Predict letters " + letters);
            }
            return retVal;
        }
    }
}