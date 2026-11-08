///////////////////////////////////////////////////////////////////////////
// <copyright file="ifeature.hpp" company="Intel Corporation">
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


#ifndef __IFEATURE_HPP__
#define __IFEATURE_HPP__

#include <vector>
#include "libivcp/filter.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file ifeature.hpp
 * \brief Declaration of feature filters basic interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic interface of all feature filters 
 * \sa Feature NormalFlowFeature NormalFlowBlurredFeature STLBPFeature VLBPFeature ConcatenatorFeature AggregatorFeature
 */
class IVCP_EXPORT IFeature: public Filter
{
public:
    /*!
    * \brief Destructor
    */
    virtual ~IFeature() 
    {
    }

    /*!
     * \brief Returns quantity of regions necessary to compute the feature
     * \details Number of regions to use is determined by settings
     * \return Number of regions
     */
    virtual unsigned int getNumberOfRegions() = 0;

    /*!
     * \brief Returns quantity of region sequences necessary to compute the feature
     * \details Number of region sequences to use is determined by settings
     * \return Number of region sequences
     */
    virtual unsigned int getNumberOfSequences() = 0;

    /*!
     * \brief Returns lenght of output feature vector
     * \details Output feature fector size is specific for a particular feature and could depend on feature settings
     * \return Length of output feature vector
     * \throw IncorrectParamException Feature length is undefined
     */
    virtual unsigned int getFeatureLength() = 0;

    /*!
     * \brief Calculate features for given region sequence
     * \param[in] regSeqs Sequence of regions to use for dynamic features
     * \return Feature matrix
     * \throw IncorrectParamException Unable to get a feature
     */
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs) = 0;

    /*!
     * \brief Get type of feature
     * \return Feature type
     */
    virtual FeatureType getType() = 0;

protected:

private:

};
};

/** @}*/
#endif
