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

namespace ACAT.Core.PreferencesManagement.Attributes
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
        public StringDescriptorAttribute(string desc, string defaultvalue = "") : base(desc)
        {
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public string DefaultValue { get; set; }
    }
}