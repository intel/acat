////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Stores global variables of the ACAT Core library
    /// </summary>
    public class CoreGlobals
    {
        public static String AppId = "ACAT";

        public static string LogFileSuffix = "_" + Process.GetCurrentProcess().StartTime.ToString("s").Replace(':', '_');

        public static Stopwatch Stopwatch1 = new();

        public static Stopwatch Stopwatch2 = new();

        public static Stopwatch Stopwatch3 = new();

        public static Stopwatch Stopwatch4 = new();

        public static Stopwatch Stopwatch5 = new();

        public static String ACATUserGuideFileName = "ACAT User Guide.pdf";

        public static string MacroACATUserGuide = "$ACAT_USER_GUIDE";

        public static string MacroAssetsVideosDir = "$ASSETS_VIDEOS_DIR";

        public static string MacroAssetsImagesDir = "$ASSETS_IMAGES_DIR";

        /// <summary>
        /// Default factory settings for the preferences
        /// </summary>
        public static SystemPreferences AppDefaultPreferences { get; set; }

        /// <summary>
        /// The global preferences that spans applications (eg username,
        /// profilename)
        /// </summary>
        public static GlobalPreferences AppGlobalPreferences { get; set; }

        /// <summary>
        /// Application sepecific preferences
        /// </summary>
        public static SystemPreferences AppPreferences { get; set; }
    }
}