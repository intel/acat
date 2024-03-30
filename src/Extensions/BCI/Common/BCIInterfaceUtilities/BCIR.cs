
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIR.cs
//
// Access language dependent strings through this class for BCI
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    public class BCIR
    {
        /// <summary>
        /// Root name of the resource in the assembly
        /// </summary>
        private const String BaseName = "BCIInterfaceUtilities.ResourcesUtils.BCIInterfaceResource";
        /// <summary>
        /// Has the class been initialized?
        /// </summary>
        private static bool _initDone = false;

        /// <summary>
        /// .NET resourcemanager object
        /// </summary>
        private static ResourceManager _resourceManager;

        /// <summary>
        /// Returns specific string from the language resource file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            string text = string.Empty;
            try
            {
                if (!_initDone)
                {
                    InitResourceManagerBCI();
                }
                text = _resourceManager.GetString(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine("BCI | BCIR | GetString| Error accessing resources: " + ex.Message);
            }
            return text;
        }

        /// <summary>
        /// Returns the culture-specific string from the language resource file
        /// identfiied by the 'name' string identifier.  If
        /// resource is not found, returns 'name'
        /// <param name="name">string identifier</param>
        /// <param name="ci">culture</param>
        /// <returns>string from resource</returns>
        public static string GetString(string key, CultureInfo ci)
        {
            string text = string.Empty;
            try
            {
                if (!_initDone)
                {
                    InitResourceManagerBCI();
                }

                text = _resourceManager.GetString(key, ci);
            }
            catch (Exception ex)
            {
                Console.WriteLine("BCI | BCIR | GetString CultureInfo | Error accessing resources: " + ex.Message);
            }
            return text;
        }

        /// <summary>
        /// Initializes the class.  Creates the resource manager object
        /// </summary>
        public static void InitResourceManagerBCI()
        {
            if (!_initDone)
            {
                _resourceManager = new ResourceManager(BaseName, Assembly.GetExecutingAssembly());
                Console.WriteLine("BCI | InitResourceManagerBCI | String Resources for BCI loaded");
                _initDone = true;
            }
        }


    }
}
