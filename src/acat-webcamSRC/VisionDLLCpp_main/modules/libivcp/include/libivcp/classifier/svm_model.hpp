///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_model.hpp" company="Intel Corporation">
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

#ifndef __SVM_MODEL_HPP__
#define __SVM_MODEL_HPP__

#include "libivcp/classifier/libsvm.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/exceptions/mismatch_feature_size_exception.hpp"
#include "libivcp/exceptions/no_model_exception.hpp"
#include "libivcp/sptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file svm_model.hpp
 * \brief SVM model
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 * \details Declaration of SVM model
 */

namespace ivcp
{
/*!
 * \brief SVM model
 * \details SVMModel uses libsvm for training model and classification
 */
class IVCP_EXPORT SVMModel
{
public:
    /*!
     * \brief Default constructor
     */
    SVMModel();

    /*!
     * \brief Destructor
     */
    ~SVMModel();
    /*!
     * \brief Clean model
     */
    void clear();

    /*!
     * \brief Get SVM parameters
     * \return Pointer to copy of SVM parameters
     */
    SPtr<const SVMParameters> getParams();
    /*!
     * \brief Get number of support vectors
     * \return Number of support vectors
     */
    int getSupportVectorsCount();
    /*!
     * \brief Get support vector with index i
     * \param[in] i Index of the support vector
     * \return Support vector with index i or empty vector if i is incorrect
     */
    std::vector<double> getSupportVector(int i);
    /*!
     * \brief Get the number of variables
     * \return The number of variables
     */
    int getVarCount();
    /*!
     * \brief Get the number of classes
     * \return The number of classes or 0 if model is not yet trained
     */
    int getClassCount();
    /*!
     * \brief Get the sorted vector of classes labels
     * \return Labels of classes
     */
    std::vector<int> getClassLabels();
    /*!
     * \brief Set the LIBSVM model
     * \param[in] model LIBSVM model
     */
    void setLibSvmModel(libsvm::LibSVMModel* model);
    /*!
     * \brief Train the model
     * \param[in] features List of feature matrixes
     * \param[in] responses Th responses for feature matrixes
     * \param[in] params SVM training parameters
     * \throw NoDataException The features or the responses are missing
     * \throw IncorrectParamException SVM parameters are invalid
     */
    void train(std::list<cv::Mat> features, std::vector<double> responses, SVMParameters params = SVMParameters());
    /*!
     * \brief Predicts the response for input feature
     * \param[in] feature One-row feature matrix
     * \return Prediction result
     * \throw NoModelException The model is not yet trained
     * \throw MismatchFeatureSizeException Mismatch size of feature matrix
     */
    double predict(const cv::Mat &feature);
    /*!
     * \brief Get probability for each class that the given feature corresponds to this class
     * \param[in] feature One-row feature matrix
     * \return Vector of probabities for each class
     * \throw NoModelException The model is not yet trained
     * \throw MismatchFeatureSizeException Mismatch size of feature matrix
     */
    std::vector<float> getProbabilities(cv::Mat feature);
    /*!
     * \brief Save model as FileNode in FileStorage
     * \param[in] fs FileStorage where model shall be saved
     * \param[in] name Name of node with the model
     * \throw NoModelException Model is not yet trained
     */
    void write(cv::FileStorage &fs, const std::string &name = "");
    /*!
     * \brief Save model to a file
     * \param[in] filename Path to a file
     * \param[in] name Name of node with the model
     * \throw NoModelException Model is not yet trained
     */
    void save(const std::string &filename, const std::string &name = "");
    /*!
     * \brief Load model from a FileNode
     * \param[in] fs FileStorage where was saved model
     * \param[in] fn FileNode where was saved model
     * \throw IOException Unable to load the model
     */
    void read(cv::FileStorage &fs, cv::FileNode fn); 
    /*!
     * \brief Load model from a file
     * \param[in] filename Path to a file
     * \param[in] name Name of node with the model
     * \throw IOException Unable to load the model
     */
    void load(const std::string &filename, const std::string &name = "");    
   
protected:

private:
    libsvm::LibSVMModel* model;

    bool checkProbabilityModel(const libsvm::LibSVMModel *model);
    void checkParams(libsvm::SVMTrainData &problem, SVMParameters &params);
};
};

/** @}*/
#endif