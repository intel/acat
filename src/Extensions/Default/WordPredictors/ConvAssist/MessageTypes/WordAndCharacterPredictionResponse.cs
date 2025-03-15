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

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    [Serializable]
    internal class WordAndCharacterPredictionResponse
    {
        /// <summary>
        /// Type of message to send
        /// </summary>
        public ConvAssistMessageTypes MessageType { get; set; }

        /// <summary>
        /// Prediction result (Letters)
        /// </summary>
        public String NextCharacters { get; set; }

        /// <summary>
        /// Prediction result (Letters from sentences)
        /// </summary>
        public String NextCharactersSentence { get; set; }

        /// <summary>
        /// Prediction result (Sentences)
        /// </summary>
        public String PredictedSentence { get; set; }

        /// <summary>
        /// Prediction result (Words)
        /// </summary>
        public String PredictedWords { get; set; }

        /// <summary>
        /// Prediction request type
        /// </summary>
        public ConvAssistPredictionTypes PredictionType { get; set; }

        // Parameterless constructor for deserialization
        public WordAndCharacterPredictionResponse() { }

        public WordAndCharacterPredictionResponse(ConvAssistMessageTypes msgType, 
                                                    ConvAssistPredictionTypes predType, 
                                                    String words, 
                                                    String characters, 
                                                    String charactersSentence, 
                                                    String sentences)
        {
            MessageType = msgType;
            PredictionType = predType;
            PredictedWords = words;
            NextCharacters = characters;
            NextCharactersSentence = charactersSentence;
            PredictedSentence = sentences;
        }

        //public WordAndCharacterPredictionResponse()
        //{
        //    MessageType = 0;
        //    PredictionType = 0;
        //    PredictedWords = string.Empty;
        //    NextCharacters = string.Empty;
        //    NextCharactersSentence = string.Empty;
        //    PredictedSentence = string.Empty;
        //}

        public enum ConvAssistMessageTypes
        {
            NotReady,
            SetParam,
            NextWordPredictionRequest,
            ReadyForPredictions = 11,
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