////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace ACAT.ACATResources
{
    /// <summary>
    /// Access language dependent strings through this class
    /// </summary>
    public static class R
    {
        /// <summary>
        /// .NET resourcemanager object
        /// </summary>
        private static readonly ResourceManager _resourceManager =
            new ResourceManager(BaseName, Assembly.GetExecutingAssembly());

        public const String ResourcesDllName = "ACATResources.resources.dll";

        /// <summary>
        /// Root name of the resource in the assembly
        /// </summary>
        private const String BaseName = "ACATResources.ACATResources";


        /// <summary>
        /// Has the class been initialized?
        /// </summary>
        //private static bool _initDone = false;

        //public static ResourceManager ResourceManager
        //{
        //    get { return _resourceManager; }
        //}

        /// <summary>
        /// Returns cultures available.  Looks for folders with the name of the
        /// language, looks for ACATResources.dll under that.
        /// </summary>
        /// <returns>List of cultures </returns>
        public static IEnumerable<CultureInfo> GetAvailableCultures()
        {
            var rootDir = new DirectoryInfo(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
            return from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
                   join d in rootDir.GetDirectories() on c.IetfLanguageTag equals d.Name
                   where d.GetFiles(ResourcesDllName).Any()
                   select c;
        }

        /// <summary>
        /// Returns the string from the language resource file
        /// identfiied by the 'name' string identifier.  If
        /// resource is not found, returns 'name'
        /// </summary>
        /// <param name="name">string identifier</param>
        /// <returns>string from resource</returns>
        public static string GetString(string key, string culture = null)
        {
            CultureInfo cultureInfo = string.IsNullOrEmpty(culture)
                ? CultureInfo.CurrentUICulture
                : new CultureInfo(culture);

            string localizedString = _resourceManager.GetString(key, cultureInfo) ?? $"[Missing: {key}]";
            Console.WriteLine($"R.GetString({key}, {culture}) = {localizedString}");
            return localizedString;
        }

        /// <summary>
        /// Returns true if the current culture is English
        /// </summary>
        /// <returns>true if it is</returns>
        public static bool IsCurrentCultureEnglish()
        {
            return CultureInfo.DefaultThreadCurrentCulture.TwoLetterISOLanguageName == "en";
        }
    }
}