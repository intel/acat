// <copyright file="UserManager.cs" company="Intel Corporation">
////////////////////////////////////////////////////////////////////////////
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

using ACAT.Lib.Core.Utility;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Lib.Core.UserManagement
{
    /// <summary>
    /// Manages users.  The purpose for having "users" is to support apps
    /// that require multi-user support.  Assets such as word prediction models,
    /// abbreviations etc are user specific and the application can store
    /// them in the user directory.
    /// Gets paths to the various files relative to the User folder.
    /// </summary>
    public class UserManager
    {
        public const String BaseUserInstallDir = "Install\\Users";

        /// <summary>
        /// Name of the default user
        /// </summary>
        public const String DefaultUserName = "DefaultUser";

        /// <summary>
        /// The name of the current user
        /// </summary>
        private static String _currentUserName = DefaultUserName;

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
        /// Creates default user directory
        /// </summary>
        /// <returns>true on success</returns>
        public static bool CreateDefaultUserDir()
        {
            return CreateUserDir(DefaultUserName);
        }

        /// <summary>
        /// Creates the folder specified user
        /// </summary>
        /// <param name="userName">Name of the user</param>
        /// <returns>true on success</returns>
        public static bool CreateUser(String userName, String sourceUserInstallPath = null)
        {
            var srcDir = String.IsNullOrEmpty(sourceUserInstallPath) ?
                                Path.Combine(SmartPath.ApplicationPath, BaseUserInstallDir, UserManager.DefaultUserName) :
                                sourceUserInstallPath;

            var targetDir = Path.Combine(UserManager.CurrentUserDir);

            Log.Debug("Copy directory " + srcDir + "=> " + targetDir);
            return FileUtils.CopyDir(srcDir, targetDir);
        }

        /// <summary>
        /// Creates user specified by the username
        /// </summary>
        /// <param name="userName">name of the user</param>
        /// <returns>true on success</returns>
        public static bool CreateUserDir(String userName)
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
                MessageBox.Show("Error creating dir. ex: " + ex);

                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
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
        /// Checks if the default user exists
        /// </summary>
        /// <returns>true if it does</returns>
        public static bool DefaultUserExists()
        {
            return UserExists(DefaultUserName);
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
        /// Returns the culture specific dir under the user folder
        /// </summary>
        /// <returns>dir</returns>
        public static String GetResourcesDir()
        {
            var dirName = Path.Combine(CurrentUserDir, Thread.CurrentThread.CurrentUICulture.Name);

            return Directory.Exists(dirName) ? dirName : Path.Combine(CurrentUserDir, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
        }

        /// <summary>
        /// Returns the culture specific dir under the user folder
        /// for the specified culture
        /// <param name="ci">Cultureinfo</param>
        /// <returns>dir</returns>
        public static String GetResourcesDir(CultureInfo ci)
        {
            var dirName = Path.Combine(CurrentUserDir, ci.Name);

            return Directory.Exists(dirName) ? dirName : Path.Combine(CurrentUserDir, ci.TwoLetterISOLanguageName);
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
        /// Returns the source install folder for the Default user
        /// </summary>
        /// <returns>path</returns>
        public static String GetUserInstallDir()
        {
            return Path.Combine(SmartPath.ApplicationPath, BaseUserInstallDir, DefaultUserName);
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
    }
}