////////////////////////////////////////////////////////////////////////////
// <copyright file="FileBrowserSettings.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Xml.Serialization;

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
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Folders from which to select files.  Semi-colon delimited
        /// folders can be specified.
        /// </summary>
        [StringDescriptor("Folders from which to open files in the File Browser.  Enter semi-colon delimited full path to the folders. Use @MyDocuments to reperent your Documents folder. " +
            "For e.g. @MyDocuments\\Files;C:\\Docs;")]
        public String FavoriteFolders = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// Semi-colon delimited list of files with these extensions to exclude.
        /// </summary>
        [StringDescriptor("Exclude these extensions when displaying files in the File Browser. Enter semi-colon delimited extensions.  For e.g.  *.jpg;*.pdf;*.png;*.xls")]
        public String ExcludeFileExtensions = String.Empty;

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
        /// Save settings to the preferences file (PreferencesFilePath)
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Save()
        {
            return Save(this, PreferencesFilePath);
        }
    }
}