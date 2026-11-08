///////////////////////////////////////////////////////////////////////////
// <copyright file="lr_classifier.hpp" company="Intel Corporation">
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

#ifndef __CLASSIFIER_LR_HPP__
#define __CLASSIFIER_LR_HPP__

#include "libivcp/classifier/classifier.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/classifier/lr_model.hpp"
#include "libivcp/classifier/lr_params.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file lr_classifier.hpp
 * \brief Logistic Regression classifier
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 * \details Declaration of Logistic Regression classifier
 */
namespace ivcp
{
 /*!
 * \brief Logistic regression classifier
 * \sa svm_classifier.hpp
 */
class IVCP_EXPORT LRClassifier: public Classifier
{
public:
    /*!
     * \brief Default LRClassifier class constructor
     * \param[in] classifierParams LR classifier configuration parameters
     */
    LRClassifier(LRParameters classifierParams = LRParameters());

    /*!
     * \brief Destructor
     */
    virtual ~LRClassifier();

    /*!
     * \brief Classify given feature matrix in accordance with the classification model
     * \param[in] feature Feature matrix calculated by one of feature classes
     * \return predicted classification response for given feature matrix
     * \throw NoModelException The model is not yet trained
     * \throw MismatchFeatureSizeException Mismatch size of feature matrix
     */
    virtual ClassType clasify(cv::Mat feature);

    /*!
     * \brief Train model with using features added in addTrainFeature function
     * \throw NoDataException The features or the responses are missing
     * \throw IncorrectParamException LR parameters are invalid
     */
    virtual void trainModel();

    /*!
     * \brief Get type of classifier
     * \return LOGISTIC_REGRESSION
     */
    virtual ClassifierType getType();

    /*!
     * \brief Check whether model has been trained or not
     * \return true if model has been trained
     */
    virtual bool isTrained();

    /*!
     * \brief Get liblinear underlying model
     * \return liblinear model
     * \throw NoModelException The model is not yet trained
     */
    SPtr<LRModel> getModel();

    /*!
     * \brief Set liblinear underlying model
     */
    void setModel(const SPtr<LRModel>& model);

    /*!
     * \brief Set LR classifier configuration parameters
     * \param[in] classifierParams LR classifier configuration parameters
     */
    void setClassifierParams(LRParameters classifierParams);
    
    /*!
     * \brief Get LR classifier configuration parameters
     * \return LR classifier configuration parameters
     */
    LRParameters getClassifierParams();

    /*!
     * \brief Get probability for each class that the given feature corresponds to
     * \param[in] feature One-row feature matrix
     * \return Vector of probabities for each class
     * \throw NoModelException The model is not yet trained
     * \throw MismatchFeatureSizeException Mismatch size of feature matrix
     */
    virtual std::vector<float> getDistribution(cv::Mat feature);

    /*!
     * \brief Get the number of variables
     * \return The number of variables
     * \throw NoModelException The model is not yet trained
     */
    virtual int getVariablesCount();

    /*!
     * \brief Get the number of classes
     * \return The number of classes
     * \throw NoModelException The model is not yet trained
     */
    virtual int getClassCount();

#ifndef SS_2023 //SS
	virtual void setClassifierParameters(const char *paramName, double paramValue);
#endif

private:
    SPtr<LRModel> lr;
    LRParameters classifierParams;
};
};

/** @}*/
#endif 
