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

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    [Serializable]
    internal class ConvAssistSetParam
    {
        public ConvAssistParameterType Parameter;

        public String Value;

        public ConvAssistSetParam(ConvAssistParameterType param, String value)
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

        public float GetFloatValue()
        {
            return float.Parse(Value);
        }

        public int GetIntValue()
        {
            return Int32.Parse(Value);
        }
    }
}