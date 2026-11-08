///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_classifier.hpp" company="Intel Corporation">
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

#ifndef __CLASSIFIER_SVM_HPP__
#define __CLASSIFIER_SVM_HPP__

#include "libivcp/classifier/classifier.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/classifier/svm_model.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file svm_classifier.hpp
 * \brief SVM classifier
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 * \details Declaration of SVM classifier
 */

namespace ivcp
{
 /*!
 * \brief SVM classifier
 * \sa lr_classifier.hpp
 */
class IVCP_EXPORT SVMClassifier: public Classifier
{
public:
    /*!
     * \brief Default SVMClassifier class constructor
     * \param[in] classifierParams SVM classifier configuration parameters
     */
    SVMClassifier(SVMParameters classifierParams = SVMParameters());

    /*!
     * \brief Destructor
     */
    virtual ~SVMClassifier();

    /*!
     * \brief Classify given feature matrix in accordance with the classification model
     * \param[in] feature Feature matrix calculated by one of feature classes
     * \return predicted classification response for given feature matrix
     */
    virtual ClassType clasify(cv::Mat feature);

    /*!
     * \brief Train model with using features added in addTrainFeature function
     */
    virtual void trainModel();

    /*!
     * \brief Get type of classifier
     * \return SVM
     */
    virtual ClassifierType getType();

    virtual bool isTrained();
   
    /*!
     * \brief Get OpenCV underlying model 
     * \return OpenCV model
     */
    SPtr<SVMModel> getModel();

    /*!
     * \brief Set SVM classifier configuration parameters
     * \param[in] classifierParams SVM classifier configuration parameters
     */
    void setClassifierParams(SVMParameters classifierParams);
    /*!
     * \brief Get SVM classifier configuration parameters
     * \return SVM classifier configuration parameters
     */
    SVMParameters getClassifierParams();

    virtual std::vector<float> getDistribution(cv::Mat feature);

    virtual int getVariablesCount();

    /*!
     * \brief Get the number of classes 
     * \return The number of classes in the model or 0 if classifier is not yet trained
     */
    virtual int getClassCount();

#ifndef SS_2023 //SS
	virtual void setClassifierParameters(const char *paramName, double paramValue);
#endif

protected:

    /*!
     * \brief Pointer to SVM model
     */
    SPtr<SVMModel> svm;

    /*!
     * \brief Parameters of SVM classifier
     */
    SVMParameters classifierParams;
};
};

/** @}*/
#endif 
