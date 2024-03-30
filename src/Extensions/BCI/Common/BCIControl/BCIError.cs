////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIError.xml
//
// Handles Errors (error codes and error messages) between the BCI actuator and ACAT
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public enum BCIErrorCodes
    {
        Status_Ok = 0,
        SensorError_UnknownException = 100,
        SensorError_Disconnected = 101,
        SensorError_DataNotReceived = 102,
        SensorError_BadSignalQuality = 103, // 3 of more channels have bad quality
        SensorError_StartSessionFailed = 104,
        OpticalSensorError_UnknownException = 200,
        OpticalSensorError_NoPulsesDetected = 201,
        OpticalSensorError_NotEnoughPulsesDetected = 202,
        OpticalSensorError_TooManyPulsesDetected = 203,
        CalibrationError_UnknwonException = 300,
        CalibrationError_OnRepetitionEnd_UnknwownException = 301,
        CalibrationError_OnRepetitionEnd_NoPulsesDetected = 302,
        CalibrationError_OnRepetitionEnd_TooManyPulsesDetected = 303,
        CalibrationError_OnRepetitionEnd_NotEnoughPulsesDetected = 304,
        CalibrationError_OnAnalyzingData_UnknownException = 310,
        CalibrationError_OnAnalyzingData_TrainingClassifiersError = 311,
        CalibrationError_OnAnalysisData_TooManyPulsesDetected = 312,
        CalibrationError_OnAnalysisData_NotEnoughPulsesDetected = 313,
        CalibrationError_LoadingClassifiers = 320,
        TypingError_UnknownException = 400,
        TypingError_OnRepetitionEnd_UnknownException = 401,
        TypingError_OnRepetitionEnd_NoPulsesDetected = 402,
        TypingError_OnRepetionEnd_TooManyPulsesReceived = 403,
        TypingError_OnRepetitionEnd_NotEnoughPulsesReceived = 404,
        TypingError_OnRepetitionEnd_IncompleteDataReceived = 405,
        TypingError_OnRepetitionEnd_DataNotReceived = 406,
        TypingError_OnRepetitionEnd_SensorDisconected = 407,
        TypingError_OnRepetitionEnd_NoProbabilitiesCalculated = 408,
        TypingError_OnRepetitionEnd_ProabilitiesMarkersMissmatch = 409,
        TypingError_ClassifiersNotLoaded = 401,
    }

    public class BCIMessages
    {
        public const string CalibrationError_CalibrationFailed = "Calibration error, please restart ACAT";
        public const string CalibrationError_Incomplete = "Calibration was incomplete, please restart ACAT";
        public const string OpticalSensorError = "Optical sensor error, please restart ACAT";
        public const string SensorError = "Sensor error, please restart ACAT";
        public const string Status_Ok = "";
        public const string TypingError = "Typing error, please restart ACAT";
        public const string ClassifiersNotLoadedError = "Classifiers not loaded, please restart ACAT";
    }

    public class BCIError
    {
        public BCIErrorCodes ErrorCode;
        public String ErrorMessage;

        public BCIError()
        {
            ErrorCode = BCIErrorCodes.Status_Ok;
            ErrorMessage = BCIMessages.Status_Ok;
        }

        public BCIError(BCIErrorCodes errorCode, String errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}