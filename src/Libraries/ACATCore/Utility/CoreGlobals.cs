////////////////////////////////////////////////////////////////////////////
// <copyright file="CoreGlobals.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Stores global variables of the ACAT Core library
    /// </summary>
    public class CoreGlobals
    {
        /// <summary>
        /// Default factory settings for the preferences
        /// </summary>
        public static Preferences AppDefaultPreferences { get; set; }

        /// <summary>
        /// The global preferences that spans applications (eg username,
        /// profilename)
        /// </summary>
        public static GlobalPreferences AppGlobalPreferences { get; set; }

        /// <summary>
        /// Application sepecific preferences
        /// </summary>
        public static Preferences AppPreferences { get; set; }
    }
}