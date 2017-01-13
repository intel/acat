////////////////////////////////////////////////////////////////////////////
// <copyright file="Common.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Reflection;
using ACAT.Lib.Core.CommandManagement;
using ACAT.Lib.Core.UserManagement;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Contains system wide globals and also initializes the extension
    /// library
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// System-wide ACAT preference settings
        /// </summary>
        public static ACATPreferences AppPreferences { get; set; }

        /// <summary>
        /// Performs initialization
        /// </summary>
        public static void Init()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Log.Debug("Assembly name: " + assembly.FullName);
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
            Context.AppPanelManager.AddFormToCache(typeof(HorizontalStripScanner2));
        }

        
    }
}