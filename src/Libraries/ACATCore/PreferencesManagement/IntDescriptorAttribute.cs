////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IntDescriptorAttribute.cs
//
// Custom attribute for int/Int32 fields/properties
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Custom attribute for int/Int32 fields/properties
    /// </summary>
    public class IntDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        /// <param name="desc">Descrption of the field/property</param>
        /// <param name="minvalue">lower bound</param>
        /// <param name="maxvalue">upper bound</param>
        /// <param name="defaultvalue">default value</param>
        public IntDescriptorAttribute(String desc, int minvalue = int.MinValue, int maxvalue = int.MaxValue, int defaultvalue = 0)
        {
            Description = desc;
            MinValue = minvalue;
            MaxValue = maxvalue;
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public int DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// Gets or sets the upper bound
        /// </summary>
        public int MaxValue { get; private set; }

        /// <summary>
        /// Gets or sets the lower bound
        /// </summary>
        public int MinValue { get; private set; }
    }
}