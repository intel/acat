////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// LongDescriptorAttribute.cs
//
// Custom attribute for long fields/properties
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Custom attribute for long fields/properties
    /// </summary>
    public class LongDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        /// <param name="desc">Descrption of the field/property</param>
        /// <param name="minvalue">lower bound</param>
        /// <param name="maxvalue">upper bound</param>
        /// <param name="defaultvalue">default value</param>
        public LongDescriptorAttribute(String desc, long minvalue = long.MinValue, long maxvalue = long.MaxValue, long defaultvalue = 0)
        {
            Description = desc;
            MinValue = minvalue;
            MaxValue = maxvalue;
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public long DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// Gets or sets the upper bound
        /// </summary>
        public long MaxValue { get; private set; }

        /// <summary>
        /// Gets or sets the lower bound
        /// </summary>
        public long MinValue { get; private set; }
    }
}