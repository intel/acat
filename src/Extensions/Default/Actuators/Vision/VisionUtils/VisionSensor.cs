////////////////////////////////////////////////////////////////////////////
// <copyright file="VisionSensor.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace ACAT.Extensions.Default.Actuators.VisionActuator.VisionUtils
{
    /// <summary>
    /// Interop functions to call ACAT vision.
    /// Contains entry points into the ACAT Vision DLL
    /// </summary>
    public class VisionSensor
    {
        private const string DllFilePath = "acat_gestures_dll.dll";

        public delegate void VisionEventCallback(string text);

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
        public static extern void SetVisionEventHandler(VisionEventCallback func);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void showVideoWindow();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        public static extern void visionCommand(String command);
    }
}