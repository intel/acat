///////////////////////////////////////////////////////////////////////////
// <copyright file="concatenator_feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_CONCATENATOR_HPP__
#define __FEATURE_CONCATENATOR_HPP__

#include "libivcp/feature/feature.hpp"
#include "libivcp/sptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file concatenator_feature.hpp
 * \brief Concatenator Feature 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Concatenator feature implementation
* \details Concatenator feature provide result as a concatenation of results sequence provided by subfeature.
* Subfeature is evaluated on subsets of original frame sequence provided by data windowing.
*/
class IVCP_EXPORT ConcatenatorFeature: public Feature
{
public:
    /*!
    * \brief Constructor
    * \details Quantity of regions necessary to compute the feature
    * \param[in] nRegions Number of regions
    */
    ConcatenatorFeature(int nRegions = 2);

    /*!
    * \brief Destructor
    */
    virtual ~ConcatenatorFeature();
    
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);
    virtual FeatureType getType();

    /*!
     * \brief Update number of regions concatenated by the Concatenator
     * \details Number of regions must be greater or equal to number of regions used by the underlying feature
     * \param[in] nRegions Number of concatenated regions
     * \throw IncorrectParamException New number of regions is less than number of regions used by the underlying feature
     */
    void setNumberOfRegions(int nRegions);
    
    /*!
     * \brief Set the feature to evaluate
     * \details Result provided by concatenator feature will consist of concatenated results provided by preconfigured subfeature
     * on different subsets of entire frame sequence
     * \param[in] newFeature Feature to set
     */
    void setFeature(SPtr<IFeature > newFeature);

    /*!
     * \brief Get the evaluated subfeature
     * \details Result provided by concatenator feature will consist of concatenated results provided by preconfigured subfeature
     * on different subsets of entire frame sequence
     * \return Smart pointer to evaluated feature
     */
    SPtr<IFeature> getInternalFeature();

protected:
    /*!
     * \brief Evaluated feature
     */
    SPtr<IFeature > feature;

private:
    unsigned int numberOfEvaluatedSubsequences;

};
};

/** @}*/
#endif 
