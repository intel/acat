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
using System.Diagnostics.CodeAnalysis;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Custom attribute for String fields/properties
    /// </summary>
    public class StringDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="desc">Description of the field/property</param>
        /// <param name="defaultvalue">Default value</param>
        public StringDescriptorAttribute(String desc, String defaultvalue = "")
        {
            Description = desc;
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public String DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; private set; }
    }
}