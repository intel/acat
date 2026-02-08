////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Utils;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace ACAT.Extension
{
    /// <summary>
    /// Contains system wide globals and also initializes the extension
    /// library
    /// </summary>
    public static class Common
    {
        private static readonly ILogger _logger = LoggingConfiguration.CreateLogger("ACAT.Extension.Common");

        /// <summary>
        /// System-wide ACAT preference settings
        /// </summary>
        public static ACATPreferences AppPreferences { get; set; }

        /// <summary>
        /// Performs initialization
        /// </summary>
        public static void Init()
        {
            AppPreferences.DefaultScanTimingsConfigurePanelName = "ScanTimeAdjustScanner";
            AppPreferences.DefaultTryoutPanelName = "DefaultTryoutScanner";
            var assembly = Assembly.GetExecutingAssembly();
            _logger.LogDebug("Assembly name: {AssemblyName}", assembly.FullName);
        }

        /// <summary>
        /// Pre-initialization. Call this before calling the Init() functon
        /// </summary>
        public static void PreInit()
        {
            // Load private fonts
            Fonts.LoadFontsFromDir(FileUtils.GetFontsDir());
            Fonts.LoadFontsFromDir(FileUtils.GetUserFontsDir());

            Context.AppPanelManager.EvtStartupAddForms += AppPanelManager_EvtStartupAddForms;
            Context.AppPanelManager.EvtStartupAddUserControls += AppPanelManager_EvtStartupAddUserControls;
        }

        /// <summary>
        /// Uninitialization
        /// </summary>
        public static void Uninit()
        {
            Fonts.Instance.Dispose();
        }

        /// <summary>
        /// Event handler for when the Panel Manager initializes. Add any
        /// Scanners in this library to the Panel Manager cache. This is because
        /// the Panel Manager only looks at the extension folders for scanners and
        /// this DLL does not reside in the extension folder.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private static void AppPanelManager_EvtStartupAddForms(object sender, EventArgs e)
        {
            Context.AppPanelManager.AddFormToCache(typeof(MenuPanel));
            Context.AppPanelManager.AddFormToCache(typeof(HorizontalStripScanner));
            Context.AppPanelManager.AddFormToCache(typeof(UserControlContainerForm));
            Context.AppPanelManager.AddFormToCache(typeof(ScanTimeAdjustForm));
        }

        private static void AppPanelManager_EvtStartupAddUserControls(object sender, EventArgs e)
        {
            var guid = UserControlConfigMap.GetUserControlId(typeof(UserControlDefaultTryout));
            UserControlConfigMap.AddUserControlToCache(guid, typeof(UserControlDefaultTryout));

            guid = UserControlConfigMap.GetUserControlId(typeof(UserControlLayoutInterface));
            UserControlConfigMap.AddUserControlToCache(guid, typeof(UserControlLayoutInterface));

            guid = UserControlConfigMap.GetUserControlId(typeof(UserControlScreenLock));
            UserControlConfigMap.AddUserControlToCache(guid, typeof(UserControlScreenLock));
        }
    }
}