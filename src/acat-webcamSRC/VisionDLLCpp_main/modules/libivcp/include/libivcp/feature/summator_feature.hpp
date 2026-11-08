///////////////////////////////////////////////////////////////////////////
// <copyright file="summator_feature.hpp" company="Intel Corporation">
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


#ifndef __FEATURE_SUMMATOR_HPP__
#define __FEATURE_SUMMATOR_HPP__

#include "libivcp/feature/feature.hpp"
#include "libivcp/sptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

namespace ivcp
{
/*!
* \brief Summator feature implementation
* \details Summator feature provide result as a sum of results sequence provided by subfeature.
* Subfeature is evaluated on subsets of original frame sequence provided by data windowing.
*/
class IVCP_EXPORT SummatorFeature: public Feature
{
public:
    /*!
     * \brief SummatorFeature settings
     */
    struct Settings
    {
    public:
       /*!
        * \brief SummatorFeature settings constructor
        * \param[in] _nRegions Quantity of regions necessary to compute the feature
        * \param[in] _countMode Result evaluation mode
        * \param[in] _threshold Count mode threshold value
        * \param[in] _normalize Result normalization flag.
        */
        Settings(int _nRegions = 2, bool _countMode = false, float _threshold = 0.4f, bool _normalize = true)
        {
            nRegions = _nRegions;
            countMode = _countMode;
            threshold = _threshold;
            normalize = _normalize;
        }

        /*!
         * \brief Number of regions
         */
        int nRegions;

        /*!
         * \brief Result normalization flag
         */
        bool normalize;

        /*!
         * \brief Result evaluation mode
         * \details If true quantity of above the threshold values in subfeature results will be calculated instead of the sum
         * Example:
         * In case we have the Sumator feature evaluated on 3 frames with subfeature evaluated on 2 frames
         * subfeature is evaluated 2 times and produce 2 feature vectors
         * - vector1 = (0.1, 0.2, 0.3, 0.4)
         * - vector2 = (0.12, 0.2, 0.5, 0.18)
         * If the SummatorFeature is configured with threshold = 0.2
         * it produce the following results
         * - result in case countMode is false, normalise is true = (0.11, 0.2, 0.4, 0.29)
         * - result in case countMode is true, normalise is true = (0, 0, 1, 0.5)
         * - result in case countMode is false, normalise is false = (0.22, 0.4, 0.8, 0.58)
         * - result in case countMode is true, normalise is false = (0, 0, 2, 1)
         */
        bool countMode;

        /*!
         * \brief Count mode threshold value
         */
        float threshold;
    };

    /*!
    * \brief Constructor
    * \param[in] settings Summator feature configuration
    */
    SummatorFeature(SummatorFeature::Settings settings = SummatorFeature::Settings());

    /*!
    * \brief Destructor
    */
    virtual ~SummatorFeature();
    
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);
    virtual FeatureType getType();

    /*!
     * \brief Update number of regions processed by the Summator
     * \details Number of regions must be greater or equal to number of regions used by the underlying feature
     * \param[in] nRegions Number of processed regions
     * \throw IncorrectParamException New number of regions is less than number of regions used by the underlying feature
     */
    void setNumberOfRegions(int nRegions);
    
    /*!
     * \brief Set the feature to evaluate
     * \details Result provided by summator feature will consist of summed up results provided by preconfigured subfeature
     * on different subsets of entire frame sequence
     * \param[in] newFeature Feature to set
     */
    void setFeature(SPtr<IFeature > newFeature);

    /*!
     * \brief Get the evaluated subfeature
     * \details Result provided by concatenator feature will consist of summed up results provided by preconfigured subfeature
     * on different subsets of entire frame sequence
     * \return Smart pointer to evaluated feature
     */
    SPtr<IFeature> getInternalFeature();

    /*!
     * \brief Get the summator feature settings
     * \return Summator feature configuration structure
     */
    SummatorFeature::Settings getSettings();

protected:
    /*!
     * \brief Evaluated feature
     */
    SPtr<IFeature > feature;
    /*!
     * \brief Result normalization flag
     */
    bool normalize;
    /*!
     * \brief Result evaluation mode
     * \details If true quantity of above the threshold values in subfeature results will be calculated instead of the sum
     */
    bool countMode;
    /*!
     * \brief Count mode threshold value
     */
    float threshold;
private:
    unsigned int numberOfEvaluatedSubsequences;

};
};

/** @}*/
#endif 
