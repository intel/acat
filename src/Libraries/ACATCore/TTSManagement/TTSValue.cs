////////////////////////////////////////////////////////////////////////////
// <copyright file="TTSValue.cs" company="Intel Corporation">
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
    /// Represents an integer value that has a range
    /// </summary>
    public class TTSValue : ITTSValue<int>
    {
        /// <summary>
        /// Initializes a new instance of the TTSValue class
        /// </summary>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="value">current value</param>
        public TTSValue(int min, int max, int value)
        {
            RangeMin = min;
            RangeMax = max;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the TTSValue class
        /// </summary>
        public TTSValue()
        {
            RangeMin = 0;
            RangeMax = 0;
            Value = 0;
        }

        /// <summary>
        /// Gets or sets the maximum for the range
        /// </summary>
        public int RangeMax { get; set; }

        /// <summary>
        /// Gets or sets the minimum for the range
        /// </summary>
        public int RangeMin { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Indicates whether the value is within the range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsValid(int value)
        {
            return value >= RangeMin && value <= RangeMax;
        }
    }
}