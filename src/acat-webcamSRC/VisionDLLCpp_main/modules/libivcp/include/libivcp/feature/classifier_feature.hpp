///////////////////////////////////////////////////////////////////////////
// <copyright file="classifier_feature.hpp" company="Intel Corporation">
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


#ifndef __CLASSIFIER_FEATURE_HPP__
#define __CLASSIFIER_FEATURE_HPP__

#include "libivcp/feature/feature.hpp"
#include "libivcp/classifier/iclassifier.hpp"
#include "libivcp/sptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file classifier_feature.hpp
 * \brief Classifier Feature 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Classifier feature implementation
* \details ClassifierFeature is responsible for constructing a new feature 
* based on output from another feature and a classifier.  ClassifierFeature calculates 
* the underlying feature on the images and use output of the underlying classifier as new feature output. 
*/
class IVCP_EXPORT ClassifierFeature: public Feature
{
public:
    /*!
    * \brief Constructor
    * \details There are two possible variants: 
    * 1) numberOfSeq is equal to number of sequences of internal feature and length of internal feature is
    * equal to classifier variables count
    * 2) Number of sequences of internal feature is 1?, numberOfSeq * length of internal feature is
    * equal to classifier variables count
    * \param[in] numberOfSeq Number of region sequences
    * \param[in] type Type of classifier output (probabilistic or categorical)
    */
    ClassifierFeature(int numberOfSeq = 1, ClassifierOutputType type = PROBABILISTIC);

    /*!
    * \brief Destructor
    */
    virtual ~ClassifierFeature();
    
    virtual cv::Mat getFeature(const std::vector<RegionSequence> &regSeqs);
    virtual FeatureType getType();

    /*!
     * \brief Set output type (probabilistic or categorical)
     * \param[in] type Output type
     * \throw IncorrectParamException Unknown output type
     */
    void setOutputType(ClassifierOutputType type);
    /*!
     * \brief Get output type
     * \return Output type (probabilistic or categorical)
     */
    ClassifierOutputType getOutputType();
    
    /*!
     * \brief Set the internal feature
     * \param[in] newFeature Feature to set
     * \throw IncorrectParamException Pointer to new feature is null or
     * internal feature length, number of sequences of internal feature 
     * and classifier feature, classifier variables count are mismatch
     */
    void setFeature(SPtr<IFeature > newFeature);

    /*!
     * \brief Get the internal subfeature
     * \return Smart pointer to internal feature
     */
    SPtr<IFeature> getInternalFeature();

    /*!
     * \brief Set the internal classifier
     * \details The classifier shall be trained separately by using standard train mode of the Pipeline
     * \param[in] newClassifier Classifier to set
     * \throw IncorrectParamException Pointer to new classifier is null, new classifier is not trained or 
     * the number of variables in the classifier model, internal feature length and number of sequences are mismatch
     */
    void setClassifier(SPtr<IClassifier> newClassifier);

    /*!
     * \brief Get the internal classifier
     * \return Smart pointer to internal classifier
     */
    SPtr<IClassifier> getInternalClassifier();

protected:
    /*!
     * \brief Internal feature
     */
    SPtr<IFeature > feature;

    /*!
     * \brief Internal classifier
     */
    SPtr<IClassifier > classifier;

    /*!
     * \brief Type of classifier output
     */
    ClassifierOutputType outputType;

private:

};
};

/** @}*/
#endif 
