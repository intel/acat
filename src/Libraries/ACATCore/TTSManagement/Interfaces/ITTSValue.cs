////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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