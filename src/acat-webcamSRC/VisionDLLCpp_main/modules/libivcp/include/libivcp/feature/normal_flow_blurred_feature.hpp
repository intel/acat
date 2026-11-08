///////////////////////////////////////////////////////////////////////////
// <copyright file="normal_flow_blurred_feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_NORMAL_FLOW_BLURRED_HPP__
#define __FEATURE_NORMAL_FLOW_BLURRED_HPP__

#include "libivcp/feature/feature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file normal_flow_blurred_feature.hpp
 * \brief Normal Flow Blurred Feature 
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Normal Flow Blurred Feature 
 */
class IVCP_EXPORT NormalFlowBlurredFeature:public Feature
{
public:
    /*!
     * \brief Default constructor
     * \param[in] threshold Threshold value
     * \param[in] outputType Type of normal flow blurred feature output
     * \param[in] robustFlag If true, solve system using robust estimator
     */
    NormalFlowBlurredFeature(float threshold = 0.f, NFBFType outputType = NFBF_ENERGY, bool robustFlag = false);

    /*!
     * \brief Destructor
     */
    virtual ~NormalFlowBlurredFeature();

    /*!
     * \brief Get Normal Flow Blurred feature
     * \param[in] regSeqs Sequence of regions to use for dynamic features
     * \return Normal flow blurred feature of size 1xn and of type 32 bit floating point
     * \details if "outputType" variable is NFBF_ENERGY: n=1, return energy value
     * NFBF_DX_DY: n=2, return dx and dy values
     * NFBF_ENERGY_DX_DY: n=3, return energy, dx and dy values
     * \throw IncorrectParamException Unable to get a feature
     */
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);

    /*!
     * \brief Returns lenght of output Normal Flow Blurred feature vector
     * \return Length of output Normal Flow Blurred feature vector
     * \throw IncorrectParamException Feature length is undefined
     */
    virtual unsigned int getFeatureLength();
    
    /*!
     * \brief Get feature type (NORMALFLOWBLURRED)
     * \return Normal flow feature (NORMALFLOWBLURRED)
     */
    virtual FeatureType getType();

    /*!
     * \brief Get threshold value for NormalFlowBlurredFeature
     * \return Threshold value
     */
    float getThreshold();

protected:
    /*!
     * \brief Threshold value
     */
    float threshold;

    /*!
     * \brief If true, solve system using robust estimator
     */
    bool robustFlag;

    /*!
     * \brief Type of normal flow blurred feature output
     */
    NFBFType outputType;
};
};

/** @}*/
#endif 
