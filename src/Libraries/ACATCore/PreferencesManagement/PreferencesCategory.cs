////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferencesCategory.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Represents a ACAT category for which preferences need
    /// to be configured.  
    /// </summary>
    public class PreferencesCategory
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferencesCategory()
        {
            PreferenceObj = null;
            AllowEnable = true;
            Enable = true;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="obj">The object that needs to be configured</param>
        /// <param name="allowEnable">Allow enabling/disabling this?</param>
        /// <param name="enable">Is this enabled?</param>
        public PreferencesCategory(Object obj, bool allowEnable = true, bool enable = true)
        {
            PreferenceObj = obj;
            AllowEnable = allowEnable;
            Enable = enable;
        }

        /// <summary>
        /// Gets or sets whether the object in the category can
        /// be enabled or not. If false, the checkbox is grayed out
        /// </summary>
        public bool AllowEnable { get; set; }

        /// <summary>
        /// Is this enabled?
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// The object which needs to be configured.
        /// Example:  An application agent object,
        /// 
        /// </summary>
        public Object PreferenceObj { get; set; }
    }
}