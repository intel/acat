////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WordAndCharacterPredictionResponse.cs
//
// Classes for responses from ConvAssist
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    [Serializable]
    internal class WordAndCharacterPredictionResponse
    {
        /// <summary>
        /// Type of message to send
        /// </summary>
        public ConvAssistMessageTypes MessageType;

        /// <summary>
        /// Prediction request type
        /// </summary>
        public ConvAssistPredictionTypes PredictionType;

        /// <summary>
        /// Prediction result (Words)
        /// </summary>
        public String PredictedWords;

        public String PredictedKeywords;

        /// <summary>
        /// Prediction result (Letters)
        /// </summary>
        public String NextCharacters;

        /// <summary>
        /// Prediction result (Letters from sentences)
        /// </summary>
        public String NextCharactersSentence;

        /// <summary>
        /// Prediction result (Sentences)
        /// </summary>
        public String PredictedSentence;

        public String PredictedCRGSentence;

                
        public WordAndCharacterPredictionResponse(ConvAssistMessageTypes msgType, 
                                                    ConvAssistPredictionTypes predType, 
                                                    String words, 
                                                    String characters, 
                                                    String charactersSentence, 
                                                    String sentences,
                                                    String keywords = null,
                                                    String crgResponses = null)
        {
            MessageType = msgType;
            PredictionType = predType;
            PredictedWords = words;
            NextCharacters = characters;
            NextCharactersSentence = charactersSentence;
            PredictedSentence = sentences;
            PredictedKeywords = keywords;
            PredictedCRGSentence = crgResponses;
        }

        public WordAndCharacterPredictionResponse()
        {
            MessageType = 0;
            PredictionType = 0;
            PredictedWords = string.Empty;
            NextCharacters = string.Empty;
            NextCharactersSentence = string.Empty;
            PredictedSentence = string.Empty;
        }

        public enum ConvAssistMessageTypes
        {
            None,
            SetParam,
            NextWordPredictionRequest,
        }

        public enum ConvAssistPredictionTypes
        {
            None,
            Normal,
            ShorthandMode,
            SentenceMode,
            CannedPhrasesMode,
        }
    }
}