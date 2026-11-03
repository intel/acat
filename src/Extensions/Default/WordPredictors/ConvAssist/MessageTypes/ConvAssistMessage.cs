////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistMessage.cs
//
/// Class for the format of the type of message send to ConvAssist
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.WordPredictorManagement.Interfaces;
using System;

namespace ACAT.Extensions.WordPredictors.ConvAssist.MessageTypes
{
    [Serializable]
    internal class ConvAssistMessage
    {
        public string Data { get; set; } = string.Empty;
        public WordPredictorMessageTypes MessageType { get; set; } = WordPredictorMessageTypes.None;
        public WordPredictionModes PredictionType { get; set; } = WordPredictionModes.None;

        // Parameterless constructor for deserialization - explicitly initialize all properties
        public ConvAssistMessage()
        {
            Data = string.Empty;
            MessageType = WordPredictorMessageTypes.None;
            PredictionType = WordPredictionModes.None;
        }

        // this is the JSON representation of the data
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="PredictionMode"></param>
        /// <param name="message"></param>
        public ConvAssistMessage(WordPredictorMessageTypes msgType, WordPredictionModes PredictionMode, string message)
        {
            MessageType = msgType;
            PredictionType = PredictionMode;
            Data = message ?? string.Empty;
        }
    }
}