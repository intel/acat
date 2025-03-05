////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.UserManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Useful resource-related functions
    /// </summary>
    public class ResourceUtils
    {
        /// <summary>
        /// Name of the language resources dll
        /// </summary>
        public const String ACATResourcesDLLName = "ACATResources.resources.dll";

        /// <summary>
        /// Name of the language-specific settings file
        /// </summary>
        private const string LanguageSettingsFileName = "LanguageSettings.xml";

        /// <summary>
        /// Language-specific settings
        /// </summary>
        private static LanguageSettings _languageSettings;

        /// <summary>
        /// Enumerates languages installed in the application directory of
        /// the running assembly.
        /// </summary>
        ///
        /// <returns>List of cultureinfo of installed languages</returns>
        public static IEnumerable<CultureInfo> EnumerateInstalledLanguages(bool excludeCurrent = false)
        {
            var dirs = Directory.EnumerateDirectories(SmartPath.ApplicationPath);

            var list = new List<CultureInfo>();

            var currentResourcesDir = FileUtils.GetResourcesDir();
            var lastIndex = currentResourcesDir.LastIndexOf("\\");
            currentResourcesDir = currentResourcesDir.Substring(lastIndex + 1);

            foreach (var dir in dirs)
            {
                try
                {
                    lastIndex = dir.LastIndexOf("\\");

                    var resourcesDir = dir.Substring(lastIndex + 1);
                    var cultureInfo = new CultureInfo(resourcesDir);

                    if (excludeCurrent)
                    {
                        if (String.Compare(currentResourcesDir, resourcesDir, true) == 0)
                        {
                            continue;
                        }
                    }

                    var resourcesFileName = Path.Combine(dir, ACATResourcesDLLName);
                    if (File.Exists(resourcesFileName))
                    {
                        list.Add(cultureInfo);
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }

            return list;
        }

        /// <summary>
        /// Returns a list of complete path to the language folders discovered
        /// under the ACAT application folder
        /// </summary>
        /// <returns>List of language folders</returns>
        public static IEnumerable<String> GetInstalledLanugageDirectories()
        {
            var dirs = Directory.EnumerateDirectories(SmartPath.ApplicationPath);

            var list = new List<String>();

            foreach (var dir in dirs)
            {
                try
                {
                    var lastIndex = dir.LastIndexOf("\\");
                    var resourcesDir = dir.Substring(lastIndex + 1);
                    var cultureInfo = new CultureInfo(resourcesDir);

                    // the above statement will throw an exception if
                    // the folder is not a language name or a two-letter
                    // iso name
                    list.Add(dir);
                }
                catch (Exception ex)
                {
                    Log.Debug("Language detect: Skipping folder " + dir + ". " + ex.Message);
                }
            }

            return list;
        }

        /// <summary>
        /// Copies language (culture) specific files for the user
        /// </summary>
        public static void InstallLanguageForUser(String baseDir = null)
        {
            var defaultUserBaseDir = UserManager.BaseUserInstallDir + "\\" + UserManager.DefaultUserName;
            var installBaseDir = (String.IsNullOrEmpty(baseDir)) ? defaultUserBaseDir : baseDir;
            var srcDir = Path.Combine(SmartPath.ApplicationPath, installBaseDir,
                CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName);
            String language;

            if (Directory.Exists(srcDir))
            {
                language = CultureInfo.DefaultThreadCurrentUICulture.Name;
            }
            else
            {
                srcDir = Path.Combine(SmartPath.ApplicationPath, installBaseDir,
                    CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName);

                language = CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName;
            }

            if (!Directory.Exists(srcDir))
            {
                return;
            }

            var targetDir = Path.Combine(UserManager.CurrentUserDir, language);

            Log.Debug("Copy directory " + srcDir + "=> " + targetDir);

            FileUtils.CopyDir(srcDir, targetDir);
        }

        /// <summary>
        /// Returns true if the specified culture is the current one
        /// </summary>
        /// <param name="cultureName">name of the culture</param>
        /// <returns>true if it is </returns>
        public static bool IsCurrentCulture(String cultureName)
        {
            return (String.Compare(cultureName, CultureInfo.DefaultThreadCurrentUICulture.Name, true) == 0 ||
                    String.Compare(cultureName, CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, true) == 0);
        }

        /// <summary>
        /// Checks if the ACAT resources for specified culture is installed
        /// </summary>
        /// <param name="ci">CultureInfo</param>
        /// <returns>true if it is</returns>
        public static bool IsInstalledCulture(CultureInfo ci)
        {
            var installedCultures = EnumerateInstalledLanguages();
            foreach (var c in installedCultures)
            {
                if (String.Compare(ci.Name, c.Name, true) == 0 ||
                    String.Compare(c.TwoLetterISOLanguageName, ci.TwoLetterISOLanguageName, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the language settings for the current culture
        /// </summary>
        /// <returns>language settings object</returns>
        public static LanguageSettings LanguageSettings()
        {
            if (_languageSettings == null)
            {
                var languageSettingsFile = Path.Combine(FileUtils.GetResourcesDir(), LanguageSettingsFileName);
                if (!File.Exists(languageSettingsFile))
                {
                    languageSettingsFile = Path.Combine(FileUtils.GetDefaultResourcesDir(), LanguageSettingsFileName);
                }

                _languageSettings = File.Exists(languageSettingsFile)
                                        ? Utility.LanguageSettings.Load<LanguageSettings>(languageSettingsFile)
                                        : new LanguageSettings();
            }

            return _languageSettings;
        }

        /// <summary>
        /// Set culture to the language specified
        /// </summary>
        /// <param name="language">language to set</param>
        public static void SetCulture(String language = "en")
        {
            try
            {
                language = language.Trim();

                // Check to make sure we have assets avaialable for the requested culture.
                // If not, fallback to english.

                CultureInfo culture = CultureInfo.CreateSpecificCulture(language);

                var resourcesDir = FileUtils.GetResourcesDir(culture.TwoLetterISOLanguageName);
                var resourceDll = Path.Combine(resourcesDir, ACATResourcesDLLName);

                //TODO: Code Smell
                if (!Directory.Exists(resourcesDir) || !File.Exists(resourceDll))
                {
                    Log.Warn(language + " resources not found.  Will use English as the default");
                    culture = CultureInfo.CreateSpecificCulture("en");
                }

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            catch (Exception ex)
            {
                Log.Error("Error setting culture to " + language + ", " + ex + ", will use English as the default");
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en");
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en");
            }
        }
    }
}