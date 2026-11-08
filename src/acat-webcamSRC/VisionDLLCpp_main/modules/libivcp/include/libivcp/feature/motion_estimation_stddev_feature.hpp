///////////////////////////////////////////////////////////////////////////
// <copyright file="motion_estimation_stddev_feature.hpp" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////


#ifndef __FEATURE_MOTION_ESTIMATION_STDDEV_HPP__
#define __FEATURE_MOTION_ESTIMATION_STDDEV_HPP__

#include "libivcp/feature/motion_estimation_feature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file motion_estimation_stddev_feature.hpp
 * \brief Motion Estimation StdDev Feature
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Motion Estimation StdDev Feature
 */
class IVCP_EXPORT MotionEstimationStdDevFeature: public MotionEstimationFeature
{
public:
    /*!
     * \brief Default constructor
     * \param[in] nSequences Number of region sequences
     */
    MotionEstimationStdDevFeature(int nSequences = 1);

    /*!
     * \brief Destructor
     */
    virtual ~MotionEstimationStdDevFeature();

    /*!
     * \brief Get feature type (MOTIONESTIMATIONSTDDEV)
     * \return Motion estimation stddev feature (MOTIONESTIMATIONSTDDEV)
     */
    virtual FeatureType getType();
protected:
    virtual float estimate(const std::vector<cv::Mat> &regions);
};
}

/** @}*/
#endif
