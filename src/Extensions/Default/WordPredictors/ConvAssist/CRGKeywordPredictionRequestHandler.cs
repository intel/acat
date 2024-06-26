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
    internal class CRGKeywordPredictionRequestHandler
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
        private List<string> _prevKeywordPredictionResult = new List<string>();

        /// <summary>
        /// Word predictor object
        /// </summary>
        private readonly ConvAssistWordPredictor WordPredictor;

        public CRGKeywordPredictionRequestHandler(ConvAssistWordPredictor wordPredictor)
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
        public WordPredictionResponse ProcessPredictionRequest2(WordPredictionRequest request)
        {
            List<String> keywords = new List<string>
            {
                "&CRGKEYWORDS",
                "AAAA",
                "BBBB",
                "CCCC",
                "DDDD"
            };

            return new WordPredictionResponse(request, keywords, true);
        }


        public WordPredictionResponse ProcessPredictionRequest(WordPredictionRequest request)
        {
            Log.Debug("Predict for: " + request.Context);
            List<string> result;
            if (request.PredictionType != PredictionTypes.Keywords)
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

                        string keywords = String.Empty;

                        keywords = WordPredictor.SendMessageConvAssistCRGKeywordPrediction(context, request.WordPredictionMode);

                        Log.Debug("ConvAssist keywords response: " + keywords);

                        try
                        {
                            result = processKeywordPredictions(WordPredictor, keywords);//, currentWord);
                        }
                        catch (Exception)
                        {
                            result = new List<string>();
                        }
                        _prevKeywordPredictionResult = result;
                    }
                    catch (Exception)
                    {
                        result = new List<string>();
                        _prevKeywordPredictionResult = result;
                    }

                    response = new WordPredictionResponse(request, result, true);
                }
                else
                {
                    response = new WordPredictionResponse(request, _prevKeywordPredictionResult, true);
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
        /// <param name="jsonResponse">Result from ConvAssist</param>
        /// <param name="currentWord">Word if the cursors is in the middle of a word</param>
        /// <returns>List of predictions with a keyword to tell apart wach type</returns>
        private List<string> processKeywordPredictions(ConvAssistWordPredictor wordPredictor, string jsonResponse)//, string currentWord)
        {
            StringBuilder resultKeywords = new StringBuilder();
            WordAndCharacterPredictionResponse response = new WordAndCharacterPredictionResponse();
            var retVal = new List<string>();
            response = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(jsonResponse);
            List<string> predictResponses = new List<string>();

            if (response != null)
            {
                response.PredictedKeywords = response.PredictedKeywords.Replace("[", String.Empty).Replace("]", String.Empty);
                predictResponses = response.PredictedKeywords.Split(',').ToList();
            }

            var pref = (wordPredictor as ISupportsPreferences).GetPreferences();
            if ((pref as Settings).UseDefaultEncoding)
            {
                for (int ii = 0; ii < predictResponses.Count(); ii++)
                {
                    predictResponses[ii] = ConvAssistUtils.UTF8EncodingToDefault(predictResponses[ii].Trim());
                    predictResponses[ii] = ConvAssistUtils.RemoveEnclosingQuotes(predictResponses[ii]);
                }
            }

            retVal.Add("&CRGKEYWORDS");

            retVal.AddRange(predictResponses);

            return retVal;
        }
    }
}