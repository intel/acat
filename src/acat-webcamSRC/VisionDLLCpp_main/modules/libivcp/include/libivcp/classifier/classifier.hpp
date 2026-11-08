///////////////////////////////////////////////////////////////////////////
// <copyright file="classifier.hpp" company="Intel Corporation">
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

#ifndef __CLASSIFIER_HPP__
#define __CLASSIFIER_HPP__

#include <list>
#include "libivcp/classifier/iclassifier.hpp"
#include "libivcp/exceptions/mismatch_feature_size_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file classifier.hpp
 * \brief Declaration of Classifier class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
 /*!
 * \brief Basic class of all classifiers
 * \sa svm_classifier.hpp lr_classifier.hpp
 */
class IVCP_EXPORT Classifier: public IClassifier
{
public:
    /*!
     * \brief Default constructor
     */
     Classifier();

    virtual ~Classifier();

    /*!
     * \brief Accumulate new feature matrix in its response 
     * \details The accumulated features will be used for model training in trainModel
     * \param[in] feature Matrix calculated by feature filter for Region Sequence
     * \param[in] response Desired response on given feature matrix
     */
    virtual void addTrainFeature(cv::Mat feature, ClassType response);

    /*!
     * \brief Return type of a filter (ie Classifier, Feature etc)
     * \return Filter type
     */
    virtual FilterType getRootType() const;

    virtual bool isReady() const;

#ifndef SS_2023 //SS
	virtual void setClassifierParameters(const char *paramName, double paramValue) = 0;
#endif

protected:
    /*!
     * \brief List of accumulated feature matrixes in order of addition
     * \details The accumulated features will be used for model training in trainModel
     */
    std::list<cv::Mat > accumulatedFeatures;

    /*!
     * \brief List of desired responses for accumulated feature matrixes in order of addition
     * \details The accumulated features will be used for model training in trainModel
     */
    std::list<ClassType > accumulatedResponses;

    /*!
     * \brief The number of variables
     * \details Size of feature which can be used for training or classification 
     * or 0 if classifier is not trained and does not accumulate features
     */
    int varCount;

private:

};
}

/** @}*/
#endif
