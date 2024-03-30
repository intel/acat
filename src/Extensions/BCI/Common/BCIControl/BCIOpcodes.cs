////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// OpCodes.cs
//
// OpCodes for communication between ACAT and the actuator
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public enum OpCodes
    {
        None = 0,
        CalibrationEndRepetition = 100,
        CalibrationEnd = 101,
        CalibrationResult = 102,
        TypingEndRepetition = 103,
        TypingEndRepetitionResult = 104,
        LanguageModelProbabilities = 105,
        ToggleCalibrationWindow = 106,
        CalibrationWindowPreShow = 107,
        CalibrationWindowShow = 108,
        CalibrationWindowClose = 109,
        Init = 110,
        Pause = 111,
        HighlightOnOff = 112,
        RequestParameters = 113,
        SendParameters = 114,
        StartSession = 115,
        StartSessionResult = 116,
        CalibrationEndRepetitionResult = 117,
        RequestCalibrationStatus = 118,
        SendCalibrationStatus = 119,
        RequestMapOptions = 120,
        SendMapOptions = 121,
        SendUpdatedMappings = 122,
        CalibrationEyesClosedRequestParameters = 200,
        CalibrationEyesClosedSendParameters = 201,
        CalibrationEyesClosedSaveParameters = 202,
        CalibrationEyesClosedIterationEnd = 203,
        CalibrationEyesClosedEnd = 204,
        TriggerTestRequestParameters = 300,
        TriggerTestSendParameters = 301,
        TriggerTestSaveParameters = 302,
        TriggerTestStart = 303,
        TriggerTestStartReady = 304,
        TriggerTestStop = 305,
        TriggerTestResult = 306,
    }
}