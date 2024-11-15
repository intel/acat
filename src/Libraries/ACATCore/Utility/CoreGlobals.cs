////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Stores global variables of the ACAT Core library
    /// </summary>
    public class CoreGlobals
    {
        public static String AppId = "ACAT";

        public static string LogFileSuffix = "_" + Process.GetCurrentProcess().StartTime.ToString("s").Replace(':', '_');

        public static Stopwatch Stopwatch1 = new Stopwatch();

        public static Stopwatch Stopwatch2 = new Stopwatch();

        public static Stopwatch Stopwatch3 = new Stopwatch();

        public static Stopwatch Stopwatch4 = new Stopwatch();

        public static Stopwatch Stopwatch5 = new Stopwatch();

        public static String ACATUserGuideFileName = "ACAT User Guide.pdf";

        public static string MacroACATUserGuide = "$ACAT_USER_GUIDE";

        public static string MacroAssetsVideosDir = "$ASSETS_VIDEOS_DIR";

        public static string MacroAssetsImagesDir = "$ASSETS_IMAGES_DIR";

        public static bool LnRMode = false;

        public static bool SwitchLnR = false;

        /// <summary>
        /// Default factory settings for the preferences
        /// </summary>
        public static Preferences AppDefaultPreferences { get; set; }

        /// <summary>
        /// The global preferences that spans applications (eg username,
        /// profilename)
        /// </summary>
        public static GlobalPreferences AppGlobalPreferences { get; set; }

        /// <summary>
        /// Application sepecific preferences
        /// </summary>
        public static Preferences AppPreferences { get; set; }

        /// <summary>
        /// Delegate for the fatal error event
        /// </summary>
        /// <param name="reason"></param>
        public delegate void FatalErrorDelegate(String reason);

        /// <summary>
        /// Triggered when there is a fatal error and the system must exit
        /// </summary>
        public static event FatalErrorDelegate EvtFatalError;

        /// <summary>
        /// Call this to trigger a fatal error which will cause ACAT
        /// to exit immediately.  Use this only for unrecoverable error
        /// </summary>
        /// <param name="reason"></param>
        public static void OnFatalError(String reason)
        {
            EvtFatalError?.Invoke(reason);
        }
    }
}