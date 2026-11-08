///////////////////////////////////////////////////////////////////////////
// <copyright file="motion_estimation_feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_MOTION_ESTIMATION_HPP__
#define __FEATURE_MOTION_ESTIMATION_HPP__

#include "libivcp/feature/feature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file motion_estimation_feature.hpp
 * \brief Motion Estimation Feature
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Motion Estimation Feature
 */
class IVCP_EXPORT MotionEstimationFeature: public Feature
{
public:
    /*!
     * \brief Default constructor
     * \param[in] nSequences Number of region sequences
     */
    MotionEstimationFeature(int nSequences);

    /*!
     * \brief Destructor
     */
    virtual ~MotionEstimationFeature();

    /*!
     * \brief Get Motion Estimation feature
     * \param[in] regSeqs Sequence of regions to use for dynamic features
     * \return Motion estimation feature matrix of size equals input region sequence's size and 32 bit floating point type
     * \throw IncorrectParamException Unable to get a feature
     */
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);
protected:
    /*!
     * \brief Estimate motion for MotionEstimationFeature
     * \param[in] regions vector of regions for specified region sequence
     * \return 32 bit floating point number means motion amount in specified metric
     */
    virtual float estimate(const std::vector<cv::Mat> &regions) = 0;

    /*!
     * \brief Compute intersection of regions in region sequences
     * \param[in] regSeqs vector of region sequences
     * \param[out] intersectionOfRegionSequences vector of Mat, the result of regions' intersection
     */
    void getIntersectionOfRegionSequences(const std::vector<RegionSequence> &regSeqs,
        std::vector<std::vector<cv::Mat> > &intersectionOfRegionSequences);
};
}

/** @}*/
#endif
