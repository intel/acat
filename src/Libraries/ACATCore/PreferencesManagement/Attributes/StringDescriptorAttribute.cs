////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// StringDescriptorAttribute.cs
//
// Custom attribute for String fields/properties
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.PreferencesManagement
{
    /// <summary>
    /// Custom attribute for String fields/properties
    /// </summary>
    public class StringDescriptorAttribute : DescriptorAttribute
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="desc">Description of the field/property</param>
        /// <param name="defaultvalue">Default value</param>
        public StringDescriptorAttribute(String desc, String defaultvalue = "") : base(desc)
        {
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public String DefaultValue { get; set; }
    }
}