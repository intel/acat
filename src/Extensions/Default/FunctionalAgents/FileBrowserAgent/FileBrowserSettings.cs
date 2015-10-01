////////////////////////////////////////////////////////////////////////////
// <copyright file="FileBrowserSettings.cs" company="Intel Corporation">
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

namespace ACAT.Extensions.Default.FunctionalAgents.FileBrowserAgent
{
    /// <summary>
    /// Holds the settings for the launch app agent.  This
    /// includes a list of applications from which user can
    /// select
    /// </summary>
    [Serializable]
    public class FileBrowserSettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized]
        public static String PreferencesFilePath;

        /// <summary>
        /// Folders from which to select files.  Semi-colon delimited
        /// folders can be specified.
        /// </summary>
        public String FavoriteFolders = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// Date/time format to use in the file listing
        /// </summary>
        public String FileBrowserDateFormat = "MM/dd/yyyy";

        /// <summary>
        /// Semi-colon delimited list of files with these extensions to exclude.
        /// </summary>
        public String FileBrowserExcludeFileExtensions = String.Empty;

        /// <summary>
        /// Set to true to show options when the user selects a file.  The options
        /// include whether to open or delete the file.
        /// </summary>
        public bool FileBrowserShowFileOperationsMenu = true;

        /// <summary>
        /// Load settings
        /// </summary>
        /// <returns>settings object</returns>
        public static FileBrowserSettings Load()
        {
            return PreferencesBase.Load<FileBrowserSettings>(PreferencesFilePath);
        }

        /// <summary>
        /// Returns list of favorite folders.  Parses the
        /// "FavoriteFolders" setting, normalizes it and
        /// returns the list of folders
        /// </summary>
        /// <returns></returns>
        public String[] GetFavoriteFolders()
        {
            if (String.IsNullOrEmpty(FavoriteFolders))
            {
                return new[] { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            }

            return SmartPath.ACATParseAndNormalizePaths(FavoriteFolders);
        }

        /// <summary>
        /// Save settings.  No op for now
        /// </summary>
        /// <returns>true always</returns>
        public override bool Save()
        {
            return true;
        }
    }
}