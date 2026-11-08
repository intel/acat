///////////////////////////////////////////////////////////////////////////
// <copyright file="normal_flow_feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_NORMAL_FLOW_HPP__
#define __FEATURE_NORMAL_FLOW_HPP__

#include "libivcp/feature/feature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file normal_flow_feature.hpp
 * \brief Normal Flow Feature 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Normal Flow Feature 
 */
class IVCP_EXPORT NormalFlowFeature:public Feature
{
public:
    /*!
     * \brief Default constructor
     * \param[in] _threshold Threshold value
     */
    NormalFlowFeature(float _threshold = 0.f);

    /*!
     * \brief Destructor
     */
    virtual ~NormalFlowFeature();

    /*!
     * \brief Get Normal Flow feature
     * \return Normal flow feature of size 1x1 and of type 32 bit floating point 
     */
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);

    /*!
     * \brief Get feature type (NORMALFLOW)
     * \return Normal flow feature (NORMALFLOW)
     */
    virtual FeatureType getType();

    /*!
     * \brief Get threshold value for NormalFlowFeature
     * \return Threshold value
     */
    float getThreshold();

protected:
    /*!
     * \brief Threshold value
     */
    float threshold;
};
};

/** @}*/
#endif 
