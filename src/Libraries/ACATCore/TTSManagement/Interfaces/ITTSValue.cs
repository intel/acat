////////////////////////////////////////////////////////////////////////////
// <copyright file="ITTSValue.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Interface that supports a ranged Type with a min, a max
    /// and a value
    /// </summary>
    public interface ITTSValue<T>
    {
        /// <summary>
        /// Gets or sets the max value
        /// </summary>
        T RangeMax { get; set; }

        /// <summary>
        /// Gets or sets the min value
        /// </summary>
        T RangeMin { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        T Value { get; set; }
    }
}