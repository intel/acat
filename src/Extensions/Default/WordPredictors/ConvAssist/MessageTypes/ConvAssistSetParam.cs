////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistSetParam.cs
//
/// Class for the format of the type of parameter send to ConvAssist
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.WordPredictors.ConvAssist.MessageTypes
{
    [Serializable]
    internal class ConvAssistSetParam
    {
        public ConvAssistParameterType Parameter { get; set; }
        public string Value { get; set; }

        // Parameterless constructor for deserialization
        public ConvAssistSetParam()
        { }

        public ConvAssistSetParam(ConvAssistParameterType param, string value)
        {
            Parameter = param;
            Value = value;
        }

        public ConvAssistSetParam(ConvAssistParameterType param, int value)
        {
            Parameter = param;
            Value = value.ToString();
        }

        public ConvAssistSetParam(ConvAssistParameterType param, float value)
        {
            Parameter = param;
            Value = value.ToString();
        }

        public float GetFloatValue()
        {
            return float.TryParse(Value, out float result) ? result : 0.0f;
        }

        public int GetIntValue()
        {
            return int.TryParse(Value, out int result) ? result : 0;
        }

        public enum ConvAssistParameterType
        {
            None,
            Path,
            Suggestions,
            TestGeneralSentencePrediction,
            RetrieveACC,
            PathStatic,
            PathPersonilized,
            PathLog,
            EnableLog,
        }
    }
}