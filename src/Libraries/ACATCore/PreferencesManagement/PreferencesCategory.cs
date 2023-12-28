////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesCategory.cs
//
// Represents a ACAT category for which preferences need
// to be configured.  
//
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