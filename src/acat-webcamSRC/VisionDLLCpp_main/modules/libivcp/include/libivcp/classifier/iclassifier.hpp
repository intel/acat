///////////////////////////////////////////////////////////////////////////
// <copyright file="iclassifier.hpp" company="Intel Corporation">
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

#ifndef __ICLASSIFIER_HPP__
#define __ICLASSIFIER_HPP__

#include "libivcp/filter.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/exceptions/mismatch_feature_size_exception.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/no_model_exception.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/exceptions/mismatch_feature_size_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file iclassifier.hpp
 * \brief Declaration of classifier basic interface
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic interface of all classifiers
 * \sa svm_classifier.hpp lr_classifier.hpp
 */
class IVCP_EXPORT IClassifier: public Filter
{
public:
    /*!
    * \brief Destructor
    */
    virtual ~IClassifier() {};

    /*!
     * \brief Classify given feature in accordance with the classification model
     * \param[in] feature One-row feature matrix calculated by one of feature classes
     * \return Predicted classification response for given feature matrix
     * \throw NoModelException The classifier is not yet trained
     * \throw MismatchFeatureSizeException Mismatch size of feature matrix 
     * (there is more than one row or the number of columns is not equal to the number 
     * of variables in the classifier model)
     */
    virtual ClassType clasify(cv::Mat feature) = 0;

    /*!
     * \brief Get probability for each class that the given feature corresponds to this class
     * \param[in] feature Feature matrix
     * \return Vector of probabities for each class
     * \throw NoModelException The classifier is not yet trained
     * \throw MismatchFeatureSizeException Mismatch size of feature matrix 
     * (there is more than one row or the number of columns is not equal to the number 
     * of variables in the classifier model)
     */
    virtual std::vector<float> getDistribution(cv::Mat feature) = 0;

    /*!
     * \brief Accumulate new feature matrix in its response 
     * \details The accumulated features will be used for model training in saveModel
     * \param[in] feature Matrix calculated by feature filter for Region Sequence
     * \param[in] response Desired response on given feature matrix
     */
    virtual void addTrainFeature(cv::Mat feature, ClassType response) = 0;
    
    /*!
     * \brief Train model with using features added in addTrainFeature function
     */
    virtual void trainModel() = 0;

    /*!
     * \brief Get type of classifier
     * \return Classifier type
     */
    virtual ClassifierType getType() = 0;

    /*!
     * \brief Check that classifier is trained
     * \return True if classifier is trained
     */
    virtual bool isTrained() = 0;

    /*!
     * \brief Get size of feature which can be used for training or classification
     * \return The number of variables or 0 if classifier is not trained and does not accumulate features
     */
    virtual int getVariablesCount() = 0;

    /*!
     * \brief Get the number of classes 
     * \return The number of classes in the model
     */
    virtual int getClassCount() = 0;

    /*!
     * \brief Check if classifier is ready to perform classification
     * \return Is classifier ready to perform classification
     */
    virtual bool isReady() const = 0;

#ifndef SS_2023 //SS
	virtual void setClassifierParameters(const char *paramName, double paramValue) = 0;
#endif

protected:

private:

};
};

/** @}*/
#endif
