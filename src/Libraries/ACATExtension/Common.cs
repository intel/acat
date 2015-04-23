////////////////////////////////////////////////////////////////////////////
// <copyright file="Common.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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
        /// Event handler for when the screen manager initializes. Add any
        /// Scanners in this library to the screen manager cache. This is because
        /// the screen manager only looks at the extension folders for scanners and
        /// this DLL does not reside int he extension folder.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private static void AppPanelManager_EvtStartupAddForms(object sender, EventArgs e)
        {
            Context.AppPanelManager.AddFormToCache(typeof(ContextualMenuMinimal));
            Context.AppPanelManager.AddFormToCache(typeof(ContextualMenu));
        }
    }
}