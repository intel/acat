////////////////////////////////////////////////////////////////////////////
// <copyright file="R.cs" company="Intel Corporation">
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
    public class R
    {
        public const String ResourcesDllName = "ACATResources.resources.dll";

        /// <summary>
        /// Root name of the resource in the assembly
        /// </summary>
        private const String BaseName = "ACATResources.ACATResources";

        /// <summary>
        /// Has the class been initialized?
        /// </summary>
        private static bool _initDone = false;

        /// <summary>
        /// .NET resourcemanager object
        /// </summary>
        private static ResourceManager _resourceManager;

        public static ResourceManager ResourceManager
        {
            get { return _resourceManager; }
        }

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
        public static string GetString(string name)
        {
            try
            {
                if (!_initDone)
                {
                    Init();
                }

                return _resourceManager.GetString(name);
            }
            catch
            {
                return name;
            }
        }

        /// <summary>
        /// Returns the culture-specific string from the language resource file
        /// identfiied by the 'name' string identifier.  If
        /// resource is not found, returns 'name'
        /// <param name="name">string identifier</param>
        /// <param name="ci">culture</param>
        /// <returns>string from resource</returns>
        public static string GetString(string name, CultureInfo ci)
        {
            try
            {
                if (!_initDone)
                {
                    Init();
                }

                return _resourceManager.GetString(name, ci);
            }
            catch
            {
                return name;
            }
        }

        /// <summary>
        /// Returns the string from the installed OS UI cultureIf
        /// resource is not found, returns 'name'
        /// </summary>
        /// <param name="name">string identifier</param>
        /// <returns>string from resource</returns>
        public static string GetString2(string name)
        {
            try
            {
                if (!_initDone)
                {
                    Init();
                }

                return GetString(name, CultureInfo.InstalledUICulture);
            }
            catch
            {
                return name;
            }
        }

        /// <summary>
        /// Initializes the class.  Creates the resource manager object
        /// </summary>
        public static void Init()
        {
            if (!_initDone)
            {
                _resourceManager = new ResourceManager(BaseName, Assembly.GetExecutingAssembly());
                _initDone = true;
            }
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