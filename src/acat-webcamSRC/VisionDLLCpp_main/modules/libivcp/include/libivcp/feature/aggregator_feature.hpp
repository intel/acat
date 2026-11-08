///////////////////////////////////////////////////////////////////////////
// <copyright file="aggregator_feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_AGGREGATOR_HPP__
#define __FEATURE_AGGREGATOR_HPP__

#include <list>
#include "libivcp/sptr.hpp"
#include "libivcp/feature/feature.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file aggregator_feature.hpp
 * \brief Aggregator Feature 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Aggregator feature implementation
* \details Aggregator feature provide result as a concatenation of subfeature results yielded on the same frame sequence.
* These subfeatures are always evaluated in the same order as they were added to the subfeature list.
*/
class IVCP_EXPORT AggregatorFeature: public Feature
{
public:
    /*!
    * \brief Constructor
    * \details Quantity of regions necessary to compute the feature could be provided
    * \param[in] nRegions Number of regions
    * \param[in] nSequences Number of sequences
    */
    AggregatorFeature(int nRegions = 2, int nSequences = 1);

    /*!
    * \brief Destructor
    */
    virtual ~AggregatorFeature();
    
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);
    virtual FeatureType getType();
    
    /*!
     * \brief Update quantity of regions necessary to compute the feature
     * \details Number of regions to use is determined by settings
     * \param[in] newNumberOfRegions Number of regions
     */
    void setNumberOfRegions(unsigned int newNumberOfRegions);
    
    /*!
     * \brief Add the feature to the list of evaluated features
     * \details Result provided by aggregator feature will consist of concatenated results provided by features from the list evaluated on the same input sequence
     * received by aggregator feature
     * \param[in] feature Feature to add
     */
    void addFeature(SPtr<IFeature > feature);
    /*!
     * \brief Remove the feature from the list of evaluated features
     * \details Result provided by aggregator feature will consist of concatenated results provided by features from the list evaluated on the same input sequence
     * received by aggregator feature
     * \param[in] feature Feature to remove
     */
    void delFeature(SPtr<IFeature > feature);

    /*!
     * \brief Get the list of evaluated features
     * \details Result provided by aggregator feature will consist of concatenated results provided by features from the list evaluated on the same input sequence
     * received by aggregator feature
     * \return List of features to evaluate
     */
    std::list<SPtr<IFeature> > &getFeatureList();

protected:
    /*!
     * \brief The list of evaluated features
     * \details Result provided by aggregator feature will consist of concatenated results provided by features from this list
     */
    std::list<SPtr<IFeature> > features;

private:

};
};

/** @}*/
#endif 
