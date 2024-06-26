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

using ACAT.Lib.Core.WordPredictionManagement;
using System;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    [Serializable]
    internal class ConvAssistMessage
    {
        public String Data;
        public WordPredictorMessageTypes MessageType;
        public WordPredictionModes PredictionType;
        public readonly bool CRG;
        public readonly String Keyword;

        // this is the JSON representation of the data
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="PredictionMode"></param>
        /// <param name="message"></param>
        public ConvAssistMessage(WordPredictorMessageTypes msgType, WordPredictionModes PredictionMode, String message, bool crg = false, String keyword = null)
        {
            MessageType = msgType;
            PredictionType = PredictionMode;
            Data = message;
            CRG = crg;
            Keyword = keyword;  
        }
    }
}