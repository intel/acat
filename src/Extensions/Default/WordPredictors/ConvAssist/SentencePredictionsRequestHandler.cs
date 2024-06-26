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
    internal class SentencePredictionsRequestHandler
    {
        private string _prevContext = null;
        private WordPredictionModes _prevMode = WordPredictionModes.None;
        private List<string> _prevSentencePredictionResults = new List<string>();
        private readonly ConvAssistWordPredictor _wordPredictor;

        public SentencePredictionsRequestHandler(ConvAssistWordPredictor wordPredictor)
        {
            _wordPredictor = wordPredictor;
        }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount propertys
        /// </summary>
        /// <returns>Resopnse containing a list of predicted sentences</returns>
        public WordPredictionResponse ProcessPredictionRequest(WordPredictionRequest request)
        {
            List<string> result;

            if (request.PredictionType != PredictionTypes.Sentences)
            {
                return new WordPredictionResponse(request, new List<String>(), false);
            }

            WordPredictionResponse response;
            try
            {
                Log.Debug("_prevMode: " + _prevMode + ", currentMode: " + _wordPredictor.GetMode());
                if (_prevMode != _wordPredictor.GetMode() ||
                    _prevContext == null ||
                    String.Compare(_prevContext, request.Context) != 0)
                {
                    _prevMode = _wordPredictor.GetMode();
                    _prevContext = request.Context;
                    var pref = (_wordPredictor as ISupportsPreferences).GetPreferences();

                    String context = request.Context;

                    if ((pref as Settings).UseDefaultEncoding)
                    {
                        context = ConvAssistUtils.UTF8EncodingToDefault(context);
                    }

                    try
                    {

                        string predictedSentences = string.Empty;

                        if (request.WordPredictionMode == WordPredictionModes.Sentence)
                        {
                            predictedSentences = _wordPredictor.SendMessageConvAssistSentencePrediction(context,
                                                                                request.WordPredictionMode, request.CRG);
                            Log.Debug("ConvAssist sentences response: " + predictedSentences);
                        }
                        else
                        {
                            predictedSentences = _wordPredictor.SendMessageConvAssistWordPrediction(context,
                                                                            request.WordPredictionMode);
                        }

                        try
                        {
                            result = ProcessSentencesPredictions(predictedSentences);
                        }
                        catch (Exception)
                        {
                            result = new List<string>();
                        }

                        _prevSentencePredictionResults = result;
                    }
                    catch (Exception)
                    {
                        result = new List<string>();
                        _prevSentencePredictionResults = result;
                    }

                    response = new WordPredictionResponse(request, result, true);
                }
                else
                {
                    Log.Debug("Nothing changed. returning previous");
                    response = new WordPredictionResponse(request, _prevSentencePredictionResults, true);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Pressaio Predict Exception " + ex);

                _prevSentencePredictionResults = new List<string>();
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
        private List<string> ProcessSentencesPredictions(string predictions)
        {
            StringBuilder resultFullPredictionWords = new StringBuilder();
            WordAndCharacterPredictionResponse answer = new WordAndCharacterPredictionResponse();
            var retVal = new List<string>();
            answer = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(predictions);
            List<string> predictSenetnces = new List<string>();
            List<string> predictLettersSentence = new List<string>();
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
            List<KeyValuePair<string, double>> SentenceList = new List<KeyValuePair<string, double>>();
            SentenceList = ConvAssistUtils.ToList(predictSenetnces);
            sentencePred = new string[SentenceList.Count];
            foreach (var element in SentenceList)
            {
                sentencePred[i] = ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharactersSentences(element.Key), true);
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
                List<KeyValuePair<string, double>> SentenceChList = new List<KeyValuePair<string, double>>();
                SentenceChList = ConvAssistUtils.ToList(predictLettersSentence);
                sentenceChPred = new string[SentenceChList.Count];
                foreach (var element in SentenceChList)
                {
                    sentenceChPred[i] = ConvAssistUtils.RemoveApostrophes(ConvAssistUtils.RemoveSpecialCharacters(element.Key));
                    i += 1;
                }
            }
            catch (Exception sentencesLetters)
            {
                Log.Debug("Pressaio Predict sentencesLetters " + sentencesLetters);
            }
            return retVal;
        }
    }
}