////////////////////////////////////////////////////////////////////////////
// <copyright file="UserManager.cs" company="Intel Corporation">
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
    /// Manages users.  The purpose for having "users" is to support apps
    /// that require multi-user support.  Assets such as word prediction models,
    /// abbreviations etc are user specific and the application can store
    /// them in the user directory
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Name of the default user
        /// </summary>
        public const String DefaultUserName = "Default";

        /// <summary>
        /// The name of the current user
        /// </summary>
        private static String _currentUserName = DefaultUserName;

#if SUPPORT_ASSETS
        private static String _sourceDir = "Install";

        private static Tuples<Assets, String, String> _assetList = new Tuples<Assets, string, string>
        {
            {Assets.Abbreviations, "AbbreviationsEmpty.xml", "Abbreviations.xml"},
            {Assets.Spellings, "SpellCheck.xml", "*"},
            {Assets.WordPrediction, "WordPrediction", "*"}
        };
#endif

        /// <summary>
        /// Initializes the user manager
        /// </summary>
        static UserManager()
        {
            _currentUserName = DefaultUserName;
        }

        /// <summary>
        /// Gets or set the current user
        /// </summary>
        public static String CurrentUser
        {
            get
            {
                return _currentUserName;
            }

            set
            {
                _currentUserName = value;
            }
        }

        /// <summary>
        /// Gets the root path to the "Users" directory
        /// </summary>
        public static String UsersDirBasePath
        {
            get
            {
                return FileUtils.GetUsersDir();
            }
        }

        /// <summary>
        /// Returns the root path to the directory of the current user
        /// </summary>
        public static String CurrentUserDir
        {
            get
            {
                return Path.Combine(FileUtils.GetUsersDir(), _currentUserName);
            }
        }

        /// <summary>
        /// Returns the user dir for the specified user name
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <returns>user directory</returns>
        public static String GetUserDir(String userName)
        {
            return Path.Combine(FileUtils.GetUsersDir(), userName);
        }

        /// <summary>
        /// Gets the full path relative to the user dir of the specified
        /// relative path / filename
        /// </summary>
        /// <param name="path">relative path</param>
        /// <returns>full path</returns>
        public static String GetFullPath(String path)
        {
            return Path.Combine(CurrentUserDir, path);
        }

        /// <summary>
        /// Checks if the current user's directory exists
        /// </summary>
        /// <returns>true on success</returns>
        public static bool CurrentUserExists()
        {
            return UserExists(_currentUserName);
        }

        /// <summary>
        /// Checks if the specified user name exists (checks if the
        /// directory exists)
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <returns>true on success</returns>
        public static bool UserExists(String userName)
        {
            return Directory.Exists(GetUserDir(userName));
        }

        /// <summary>
        /// Checks if the default user exists
        /// </summary>
        /// <returns>true if it does</returns>
        public static bool DefaultUserExists()
        {
            return UserExists(DefaultUserName);
        }

        /// <summary>
        /// Creates default user directory
        /// </summary>
        /// <returns>true on success</returns>
        public static bool CreateDefaultUser()
        {
            return CreateUser(DefaultUserName);
        }

        /// <summary>
        /// Creates user specified by the username
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <returns>true on success</returns>
        public static bool CreateUser(String userName)
        {
            bool retVal = true;

            if (UserExists(userName))
            {
                return false;
            }

            try
            {
                var dir = GetUserDir(userName);
                if (!UserExists(userName))
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
        /// Deletes the specified user. (not supported yet)
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <returns>true on success</returns>
        public static bool DeleteUser(String userName)
        {
            return false;
        }
    }
}