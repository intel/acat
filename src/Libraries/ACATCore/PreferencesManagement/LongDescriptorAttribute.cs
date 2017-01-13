////////////////////////////////////////////////////////////////////////////
// <copyright file="LongDescriptorAttribute.cs" company="Intel Corporation">
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