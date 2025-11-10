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

namespace ACATResources
{
    /// <summary>
    /// Access language dependent strings through this class
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// .NET resource manager object
        /// </summary>
        private static readonly ResourceManager _resourceManager =
            new(BaseName, Assembly.GetExecutingAssembly());

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

        private static readonly string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        public static List<CultureInfo> GetAvailableResourceCultures(Assembly mainAssembly)
        {
            var cultures = new List<CultureInfo>();
            string baseDirectory = Path.GetDirectoryName(mainAssembly.Location);

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures))
            {
                string cultureName = culture.Name;
                if (string.IsNullOrEmpty(cultureName)) continue;

                string resourcePath = Path.Combine(baseDirectory, cultureName, _assemblyName + ".resources.dll");
                if (File.Exists(resourcePath))
                {
                    cultures.Add(culture);
                }
            }

            return cultures;
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