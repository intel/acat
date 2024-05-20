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
    internal class CRGResponsePredictionRequestHandler
    {
        /// <summary>
        /// Previous mode
        /// </summary>
        private WordPredictionModes _prevMode = WordPredictionModes.None;

        /// <summary>
        /// Previous words saved
        /// </summary>
        private string _prevContext = null;

        private string _prevKeyword = null;

        /// <summary>
        /// Previous predictions results
        /// </summary>
        private List<string> _prevCRGResponses = new List<string>();

        /// <summary>
        /// Word predictor object
        /// </summary>
        private ConvAssistWordPredictor WordPredictor;

        public CRGResponsePredictionRequestHandler(ConvAssistWordPredictor wordPredictor)
        {
            WordPredictor = wordPredictor;
        }


        public WordPredictionResponse ProcessPredictionRequest(WordPredictionRequest request)
        {
            List<String> keywords = new List<string>();

            String append = (String.IsNullOrEmpty(request.Keyword)) ? String.Empty : " for keyword [" + request.Keyword + "]";
            keywords.Add("&CRGRESPONSES");
            keywords.Add("This is CRG response 1 " + append);
            keywords.Add("This is CRG response 2" + append);
            keywords.Add("This is CRG response 3" + append);
            keywords.Add("This is CRG response 4" + append);
            keywords.Add("This is CRG response 5" + append);

            return new WordPredictionResponse(request, keywords, true);
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
            Log.Debug("Predict for: " + request.Context);
            string[] prediction = { "" };
            var result = new List<string>();
            WordPredictionResponse response = null;

            if (request.PredictionType != PredictionTypes.CRGResponses)
            {
                return new WordPredictionResponse(request, new List<String>(), false);
            }

            try
            {
                if (_prevMode != WordPredictor.GetMode() ||
                    _prevContext == null ||
                    String.Compare(_prevContext, request.Context) != 0 ||
                    _prevKeyword == null || (request.Keyword != null && String.Compare(_prevKeyword, request.Keyword) != 0))
                {
                    _prevMode = WordPredictor.GetMode();
                    _prevContext = request.Context;
                    _prevKeyword = request.Keyword;

                    var pref = (WordPredictor as ISupportsPreferences).GetPreferences();

                    String context = request.Context;
                    if ((pref as Settings).UseDefaultEncoding)
                    {
                        context = ConvAssistUtils.UTF8EncodingToDefault(context);
                    }

                    try
                    {
                        Log.Debug("ConvAssist Mode: " + request.WordPredictionMode);

                        string jsonResponse = String.Empty;

                        jsonResponse = WordPredictor.SendMessageConvAssistCRGResponsePrediction(context, request.Keyword, request.WordPredictionMode);

                        Log.Debug("ConvAssist crgreponse: " + jsonResponse);

                        try
                        {
                            result = processCRGResponse(WordPredictor, jsonResponse);//, currentWord);
                        }
                        catch (Exception)
                        {
                            result = new List<string>();
                        }
                        _prevCRGResponses = result;
                    }
                    catch (Exception)
                    {
                        result = new List<string>();
                        _prevCRGResponses = result;
                    }

                    response = new WordPredictionResponse(request, result, true);
                }
                else
                {
                    response = new WordPredictionResponse(request, _prevCRGResponses, true);
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
        private List<string> processCRGResponse(ConvAssistWordPredictor wordPredictor, string jsonResponse)//, string currentWord)
        {
            StringBuilder resultCRGResponses = new StringBuilder();
            WordAndCharacterPredictionResponse answer = new WordAndCharacterPredictionResponse();
            var retVal = new List<string>();
            answer = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(jsonResponse);
            List<string> predictResponses = new List<string>();

            

            if (answer != null)
            {
                predictResponses = answer.PredictedKeywords.Split('(', ')').Where((item, index) => index % 2 != 0).ToList();
            }

            var pref = (wordPredictor as ISupportsPreferences).GetPreferences();
            if ((pref as Settings).UseDefaultEncoding)
            {
                for (int ii = 0; ii < predictResponses.Count(); ii++)
                {
                    predictResponses[ii] = ConvAssistUtils.UTF8EncodingToDefault(predictResponses[ii]);
                }
            }

            retVal.Add("&CRGRESPONSES");

            retVal.AddRange(predictResponses);

            return retVal;
        }
    }
}