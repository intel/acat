///////////////////////////////////////////////////////////////////////////
// <copyright file="feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_HPP__
#define __FEATURE_HPP__

#include <deque>
#include "libivcp/feature/ifeature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file feature.hpp
 * \brief Basic feature operations
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Implementation of common functions for all features
 * \sa IFeature NormalFlowFeature NormalFlowBlurredFeature STLBPFeature VLBPFeature ConcatenatorFeature AggregatorFeature
 */
class IVCP_EXPORT Feature: public IFeature
{
public:
    /*!
    * \brief Constructor
    * \param[in] nRegions Number of regions necessary to compute the feature
    * \param[in] nSequences Number of region sequences
    */
    Feature(int nRegions, int nSequences = 1);

    virtual ~Feature() {};

    virtual unsigned int getNumberOfRegions();
    virtual unsigned int getNumberOfSequences();
    virtual unsigned int getFeatureLength();
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs) = 0;

    virtual FilterType getRootType() const;    

protected:
    /*!
     * \brief Number of regions in a sequence the Feature needs (for example the Normal flow feature has 2 regions)
     */
    unsigned int numberOfRegions;

    /*!
     * \brief Number of region sequences the Feature needs (this is reserved for future multi seq features)
     */
    unsigned int numberOfSequences;

    /*!
     * \brief Length of feature output vector
     * \details 0 if length is undefined
     */
    unsigned int featureLength;

private:

};
};

/** @}*/
#endif 
