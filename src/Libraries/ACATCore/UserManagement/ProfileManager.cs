////////////////////////////////////////////////////////////////////////////
// <copyright file="ProfileManager.cs" company="Intel Corporation">
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
using System.IO;
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

namespace ACAT.Lib.Core.UserManagement
{
    /// <summary>
    /// ACAT supports multiple profiles for each user. Applications
    /// settings can be stored in the profile directory. This could include aspects
    /// such as color schemes, word prediciton models etc.  Each user profile has
    /// its own root folder under the user's home folder.  This class manages
    /// profiles for a user
    /// </summary>
    public class ProfileManager
    {
        /// <summary>
        /// Name of the default profile
        /// </summary>
        public const String DefaultProfileName = "Default";

        /// <summary>
        /// Name of the root directory of the profiles foler
        /// </summary>
        private const String ProfilesDir = "Profiles";

        /// <summary>
        /// Tracks the currently active profile
        /// </summary>
        private static String _currentProfileName = DefaultProfileName;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        static ProfileManager()
        {
            _currentProfileName = DefaultProfileName;
        }

        /// <summary>
        /// Returns the current profile name
        /// </summary>
        public static String CurrentProfile
        {
            get { return _currentProfileName; }

            set { _currentProfileName = value; }
        }

        /// <summary>
        /// Returns the path to the current profile directory
        /// </summary>
        public static String CurrentProfileDir
        {
            get
            {
                return Path.Combine(ProfileDirBasePath, _currentProfileName);
            }
        }

        /// <summary>
        /// Returns the full path to the user's profiles root directory
        /// </summary>
        public static String ProfileDirBasePath
        {
            get
            {
                return Path.Combine(UserManager.CurrentUserDir, ProfilesDir);
            }
        }

        /// <summary>
        /// Creates the default profile for the user
        /// </summary>
        /// <returns>true on success</returns>
        public static bool CreateDefaultProfile()
        {
            return CreateProfile(DefaultProfileName);
        }

        /// <summary>
        /// Creates the specified profile for the user
        /// </summary>
        /// <param name="profileName">name of the profile</param>
        /// <returns>true on success</returns>
        public static bool CreateProfile(String profileName)
        {
            bool retVal = true;

            if (ProfileExists(profileName))
            {
                return false;
            }

            try
            {
                var dir = GetProfileDir(profileName);
                if (!ProfileExists(profileName))
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Checks if the default profile exists
        /// </summary>
        /// <returns></returns>
        public static bool DefaultProfileExists()
        {
            return ProfileExists(DefaultProfileName);
        }

        /// <summary>
        /// Gets the fullpath of the spcified path relative to the
        /// profile directory.  Use this to get full paths to files
        /// that will be store with the user's profile (e.g. settings)
        /// </summary>
        /// <param name="path">relative path</param>
        /// <returns>full path</returns>
        public static String GetFullPath(String path)
        {
            return Path.Combine(CurrentProfileDir, path);
        }

        /// <summary>
        /// Returns the profile directory for the specified profile
        /// </summary>
        /// <param name="profile">the profile</param>
        /// <returns>directory</returns>
        public static String GetProfileDir(String profile)
        {
            return Path.Combine(ProfileDirBasePath, profile);
        }

        /// <summary>
        /// Checks if the specified profile already exists
        /// </summary>
        /// <param name="profileName">name of the profile</param>
        /// <returns>true if it exists</returns>
        public static bool ProfileExists(String profileName)
        {
            return Directory.Exists(GetProfileDir(profileName));
        }
    }
}