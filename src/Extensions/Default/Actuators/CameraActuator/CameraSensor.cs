////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CameraSensor.cs
//
// Contains Interop functions to interface with the C++ vision sensor DLL
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    public class CameraSensor
    {
        private const string DllFilePath = "acat_gestures_dll.dll";

        public delegate void CameraEventCallback(string text);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void acatVision();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void hideVideoWindow();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void init();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern bool isVideoWindowVisible();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void quit();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void selectCamera(String camera);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetVisionEventHandler(CameraEventCallback func);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void showVideoWindow();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void visionCommand(String command, double value);
    }
}