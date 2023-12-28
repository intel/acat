////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FloatDescriptorAttribute.cs
//
// Custom attribute for float fields/properties
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Custom attribute for float fields/properties
    /// </summary>
    public class FloatDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        /// <param name="desc">Descrption of the field/property</param>
        /// <param name="minvalue">lower bound</param>
        /// <param name="maxvalue">upper bound</param>
        /// <param name="defaultvalue">default value</param>
        public FloatDescriptorAttribute(String desc,
                                        float minvalue = float.MinValue,
                                        float maxvalue = float.MaxValue,
                                        float defaultvalue = 0)
        {
            Description = desc;
            MinValue = minvalue;
            MaxValue = maxvalue;
            DefaultValue = defaultvalue;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public float DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// Gets or sets the upper bound
        /// </summary>
        public float MaxValue { get; private set; }

        /// <summary>
        /// Gets or sets the lower bound
        /// </summary>
        public float MinValue { get; private set; }
    }
}