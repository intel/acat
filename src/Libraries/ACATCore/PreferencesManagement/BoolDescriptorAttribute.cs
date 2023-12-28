////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BoolDescriptorAttribute.cs
//
// Custom attribute for boolean fields/properties
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Custom attribute for boolean fields/properties
    /// </summary>
    public class BoolDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="desc">Description of the field/property</param>
        /// <param name="defaultvalue">Default value</param>
        public BoolDescriptorAttribute(String desc, bool defaultvalue = false)
        {
            Description = desc;
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public bool DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; private set; }
    }
}