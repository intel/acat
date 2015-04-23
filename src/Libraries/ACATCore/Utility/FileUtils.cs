////////////////////////////////////////////////////////////////////////////
// <copyright file="FileUtils.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using ACAT.Lib.Core.UserManagement;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Contains functions to contruct paths relative to
    /// the various root folders in ACAT such as the Assets folder,
    /// the User folder, the Profile folder, the Extensions folder etc.
    /// </summary>
    public class FileUtils
    {
        /// <summary>
        /// Folder under which all ACAT assets such as
        /// images, fonts, etc are stored
        /// </summary>
        private const String AssetsDir = "Assets";

        /// <summary>
        /// Folder under which config xml files for animation are
        /// stored
        /// </summary>
        private const String ConfigDir = "Config";

        /// <summary>
        /// Folder under which ACAT extensions are stored
        /// </summary>
        private const String ExtensionsDir = "Extensions";

        /// <summary>
        /// Folder under which all the font files are stored
        /// </summary>
        private const String FontsDir = "Fonts";

        /// <summary>
        /// Folder in which all bitmaps are stored
        /// </summary>
        private const String ImagesDir = "Images";

        /// <summary>
        /// Folder under which log files are stored
        /// </summary>
        private const String LogsDir = "Logs";

        /// <summary>
        /// Folder under which all the theme files are stored
        /// </summary>
        private const String SkinsDir = "Skins";

        /// <summary>
        /// Folder in which all ACAT sounds files are stored
        /// </summary>
        private const String SoundsDir = "Sounds";

        /// <summary>
        /// Folder under which all the user files are stored
        /// </summary>
        private const String UsersDir = "Users";

        /// <summary>
        /// Used to check that only one instance of the app is running
        /// </summary>
        private static Mutex _appMutex;

        /// <summary>
        /// Directory from which the application is running
        /// </summary>
        public static String ACATPath
        {
            get
            {
                return SmartPath.ApplicationPath;
            }
        }

        /// <summary>
        /// Directory where application prefernces are stored
        /// </summary>
        public static String AppPreferencesDir { get; set; }

        public static Assembly AssemblyResolve(Assembly executingAssembly, ResolveEventArgs args)
        {
            Log.IsNull("Requesting asembly ", args.RequestingAssembly);

            if (args.RequestingAssembly == null || String.IsNullOrEmpty(args.RequestingAssembly.Location))
            {
                return null;
            }

            Log.Debug("RequestingAssembly: [" + args.RequestingAssembly.Location + "], Name:[" + args.Name + "]");

            var requestingAssemblyDir = Path.GetDirectoryName(args.RequestingAssembly.Location);

            Log.Debug("RequestingAssembly directory is " + requestingAssemblyDir);

            var assemblyName = new AssemblyName(args.Name).Name;
            var assemblyPath = requestingAssemblyDir + "\\" + assemblyName + ".dll";

            Log.Debug("Resolved assembly location: " + assemblyPath);

            Assembly retVal = null;
            try
            {
                if (!String.IsNullOrEmpty(assemblyPath))
                {
                    Log.Debug("LoadFrom " + assemblyPath);
                    retVal = Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    Log.Debug("Could not find assembly " + args.RequestingAssembly.Location);
                }
            }
            catch (FileNotFoundException fnf)
            {
                Log.Exception(fnf);

                if (!assemblyPath.ToLower().Contains(".resources"))
                {
                    throw;
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Debug("Could not load assembly.. Exception: " + ex);
            }

            Log.IsNull("retVal: ", retVal);
            return retVal;
        }

        /// <summary>
        /// Checks if the app is already running. The mutex
        /// is used to check this
        /// </summary>
        /// <param name="mutexName">name of the mutex</param>
        /// <returns>true if is</returns>
        public static bool CheckAppExistingInstance(String mutexName)
        {
            bool retVal = true;
            try
            {
                closeAppMutex();
                _appMutex = Mutex.OpenExisting(mutexName);
                closeAppMutex();
            }
            catch
            {
                //the specified mutex doesn't exist, we should create it
                _appMutex = new Mutex(true, mutexName);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Copies the spcified source to target. IF source
        /// is a folder, recursively copies source to target
        /// </summary>
        /// <param name="source">source file or dir</param>
        /// <param name="target">target</param>
        /// <returns>true on success</returns>
        public static bool Copy(String source, String target)
        {
            bool retVal = true;
            try
            {
                var fileAttr = File.GetAttributes(source);
                if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    retVal = CopyDir(source, target, true);
                }
                else
                {
                    File.Copy(source, target, true);
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
        /// Copies the specified source dir to the target dir
        /// </summary>
        /// <param name="srcDir">source dir</param>
        /// <param name="targetDir"target dir></param>
        /// <param name="recursive">go deep recursively?</param>
        /// <returns>true on success</returns>
        public static bool CopyDir(string srcDir, string targetDir, bool recursive)
        {
            bool retVal = true;

            try
            {
                copyDir(srcDir, targetDir, recursive);
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public static void Dispose()
        {
        }

        /// <summary>
        /// Returns fully qualified path to ACAT assets
        /// </summary>
        /// <returns>full path</returns>
        public static String GetAssetsDir()
        {
            return Path.Combine(ACATPath, AssetsDir);
        }

        /// <summary>
        /// Returns the fully qualified name of the XML config file.
        /// </summary>
        /// <param name="xmlFileName">xml config file name</param>
        /// <returns>full path to the file</returns>
        public static String GetConfigFileFullPath(String xmlFileName)
        {
            return Path.Combine(SmartPath.ApplicationPath, ConfigDir, xmlFileName);
        }

        /// <summary>
        /// Gets version of assembly and referenced assemblies
        /// </summary>
        /// <param name="assembly">input assembly</param>
        public static void GetDependentAssemblyVersion(Assembly assembly)
        {
            Log.Debug("Assembly name: " + assembly.GetName() + " Version: " + assembly.GetName().Version.ToString());

            AssemblyName[] referenced = assembly.GetReferencedAssemblies();
            foreach (AssemblyName refAssembly in referenced)
            {
                Log.Debug("Assembly name: " + refAssembly.Name + " Version: " + refAssembly.Version.ToString());
            }
        }

        /// <summary>
        /// Returns fully qualified path to the Extensions root folder
        /// </summary>
        /// <returns>full path</returns>
        public static String GetExtensionDir()
        {
            return Path.Combine(ACATPath, ExtensionsDir);
        }

        /// <summary>
        /// Returns fully qualified path of the specified dir relative to
        /// to the Extensions root folder
        /// </summary>
        /// <param name="dir">relative path to the extensions folder</param>
        /// <returns>fully qualified path</returns>
        public static String GetExtensionDir(String dir)
        {
            return Path.Combine(GetExtensionDir(), dir);
        }

        /// <summary>
        /// Returns the path to the executable that is associated
        /// with the file extension
        /// </summary>
        /// <param name="extension">file extension</param>
        /// <returns>associated exe.  emptyr string if no association found</returns>
        public static string GetFileAssociationForExtension(string extension)
        {
            uint capacity = 0;
            var emptyString = String.Empty;
            const uint errorCodeNoAssociation = 0x80070483;
            const uint verify = 0x40;
            const uint exeName = 2;

            extension = extension.Trim();
            if (String.IsNullOrEmpty(extension))
            {
                return emptyString;
            }

            if (extension[0] != '.')
            {
                extension = "." + extension;
            }

            var ret1 = AssocQueryString(verify, exeName, extension, null, null, ref capacity);

            var sb = new StringBuilder((int)capacity);
            var ret2 = AssocQueryString(verify, exeName, extension, null, sb, ref capacity);

            if (ret1 == errorCodeNoAssociation || ret2 == errorCodeNoAssociation)
            {
                return emptyString;
            }

            try
            {
                var file = Path.GetFileName(sb.ToString());
                if (String.Compare(file, "shell32.dll", true) == 0)
                {
                    return emptyString;
                }
            }
            catch (Exception)
            {
                return emptyString;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the fully qualified path to the global ACAT fonts folder
        /// </summary>
        /// <returns>path</returns>
        public static String GetFontsDir()
        {
            return Path.Combine(GetAssetsDir(), FontsDir);
        }

        /// <summary>
        /// Returns the fully qualified path to the specified image file.
        /// First checks the User's image folder.  If it doesn't find the
        /// file there, returns the path to the global ACAT images folder
        /// </summary>
        /// <param name="filename">input image file name</param>
        /// <returns>full path to the image file</returns>
        public static String GetImagePath(string imageFile)
        {
            String fullPath = Path.Combine(GetUserImageDir(), imageFile);
            return File.Exists(fullPath) ? fullPath : Path.Combine(GetAssetsDir(), ImagesDir, imageFile);
        }

        /// <summary>
        /// Returns fully qualified path to the global ACAT Images folder
        /// </summary>
        /// <returns>path</returns>
        public static String GetImagesDir()
        {
            return Path.Combine(GetAssetsDir(), ImagesDir);
        }

        /// <summary>
        /// Returns path to where ACAT log files will be stored
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetLogsDir()
        {
            return Path.Combine(ACATPath, LogsDir);
        }

        /// <summary>
        /// Returns the fully qualified name of the preferences file. The
        /// location of the file is the same as the EXE
        /// </summary>
        /// <param name="filename">name of the settings file</param>
        /// <returns>fully qualified path</returns>
        public static String GetPreferencesFileFullPath(String prefName)
        {
            return Path.Combine(SmartPath.ApplicationPath, prefName);
        }

        /// <summary>
        /// Returns the global ACAT themes folder
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetSkinsDir()
        {
            return Path.Combine(GetAssetsDir(), SkinsDir);
        }

        /// <summary>
        /// For the specified imaagefile, gets the fully
        /// qualified path to it for the spcified skin
        /// </summary>
        /// <param name="skin">skin name</param>
        /// <param name="imageFile">image file name</param>
        /// <returns>full path to the image file</returns>
        public static String GetSkinsImagePath(string skin, string imageFile)
        {
            var fullPath = Path.Combine(GetUserSkinsDir(), skin, imageFile);
            return File.Exists(fullPath) ? fullPath : Path.Combine(GetSkinsDir(), skin, imageFile);
        }

        /// <summary>
        /// Returns the fully qualified path to the specified sound file.
        /// First checks the User's sounds folder.  If it doesn't find the
        /// file there, returns the path to the global ACAT sounds folder
        /// </summary>
        /// <param name="soundFile">input sound file name</param>
        /// <returns>full path to the sound file</returns>
        public static String GetSoundPath(string soundFile)
        {
            var fullPath = Path.Combine(GetUserSoundsDir(), soundFile);
            return File.Exists(fullPath) ? fullPath : Path.Combine(GetAssetsDir(), soundFile);
        }

        /// <summary>
        /// Returns the global ACAT sounds folder
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetSoundsDir()
        {
            return Path.Combine(GetAssetsDir(), SoundsDir);
        }

        /// <summary>
        /// Returns fully qualified path to the Fonts folder relative
        /// to the current user's User folder
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetUserFontsDir()
        {
            return Path.Combine(UserManager.GetFullPath(AssetsDir), FontsDir);
        }

        /// <summary>
        /// Returns fully qualified path to the Images folder relative
        /// to the current user's User folder
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetUserImageDir()
        {
            return Path.Combine(UserManager.GetFullPath(AssetsDir), ImagesDir);
        }

        /// <summary>
        /// Returns fully qualified path to the Users folders
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetUsersDir()
        {
            return Path.Combine(ACATPath, UsersDir);
        }

        /// <summary>
        /// Returns path to the themes folder relative to the
        /// current user's USER folder
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetUserSkinsDir()
        {
            return Path.Combine(UserManager.GetFullPath(AssetsDir), SkinsDir);
        }

        /// <summary>
        /// Returns the sounds folder relative to the current
        /// user's USER folder
        /// </summary>
        /// <returns>fully qualified path</returns>
        public static String GetUserSoundsDir()
        {
            return Path.Combine(UserManager.GetFullPath(AssetsDir), SoundsDir);
        }

        /// <summary>
        /// Checks if the specified process is running or not
        /// </summary>
        /// <param name="processName">name of the process to check</param>
        /// <returns>true if it is</returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool IsRunning(String processName)
        {
            Process[] pname = Process.GetProcessesByName(processName);
            return pname.Length != 0;
        }

        /// <summary>
        /// Runs the specified executable.
        /// </summary>
        /// <param name="executable">path to the executable</param>
        /// <returns>true on success</returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static bool Run(String executable)
        {
            bool retVal = true;

            Log.Debug(executable);

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = executable;

            //Start the process.
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Log.Error("Error executing " + executable + ". Exception: " + e);
                return false;
            }

            Log.Debug("Returning " + retVal);

            return retVal;
        }

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint AssocQueryString(uint flags, uint str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, [In][Out] ref uint pcchOut);

        /// <summary>
        /// Closes the application mutex
        /// </summary>
        private static void closeAppMutex()
        {
            if (_appMutex != null)
            {
                _appMutex.Close();
                _appMutex.Dispose();
                _appMutex = null;
            }
        }

        /// <summary>
        /// Recursively copies source dir to target
        /// </summary>
        /// <param name="srcDir">source dir</param>
        /// <param name="targetDir">target dir</param>
        /// <param name="recursive">go deep recursively?</param>
        private static void copyDir(string srcDir, string targetDir, bool recursive)
        {
            var dir = new DirectoryInfo(srcDir);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                Log.Debug("No such directory: " + srcDir);
                return;
            }

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                file.CopyTo(Path.Combine(targetDir, file.Name), false);
            }

            if (recursive)
            {
                foreach (var subdir in dirs)
                {
                    CopyDir(subdir.FullName, Path.Combine(targetDir, subdir.Name), recursive);
                }
            }
        }
    }
}