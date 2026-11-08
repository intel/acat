///////////////////////////////////////////////////////////////////////////
// <copyright file="libsvm.hpp" company="Intel Corporation">
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

/****************************************************************************************\
                                COPYRIGHT NOTICE
                                ----------------

  The code has been derived from libsvm library (version 3.16)
  (http://www.csie.ntu.edu.tw/~cjlin/libsvm).

  Here is the orignal copyright:
------------------------------------------------------------------------------------------
    Copyright (c) 2000-2013 Chih-Chung Chang and Chih-Jen Lin
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions
    are met:

    1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.

    3. Neither name of copyright holders nor the names of its contributors
    may be used to endorse or promote products derived from this software
    without specific prior written permission.


    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
\****************************************************************************************/

#ifndef __LIBSVM_HPP__
#define __LIBSVM_HPP__

#include <list>
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/classifier/svm_params.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file libsvm.hpp
 * \brief libSVM library: types and functions
 * \details http://www.csie.ntu.edu.tw/~cjlin/libsvm 
 */

namespace libsvm
{
/*!
 * \brief Class to store features and support vectors
 */
class IVCP_EXPORT SVMVector
{
public:
    /*!
     * \brief Data array
     */
    double* data;

    /*!
     * \brief Constructor
     * \param[in] _size Size of vector
     */
    SVMVector(unsigned int _size = 0);

    /*!
     * \brief Releases the memory allocated for the vector
     */
    void release();

    /*!
     * \brief Change size of vector 
     * \details This function releases the memory allocated for the vector 
     * and allocates new memory of the given size
     * \param[in] _size New size of the vector
     */
    void resize(unsigned int _size);

    /*!
     * \brief Swap content of two SVMVectors
     * \param[in] v SVMVector with which to swap content
     */
    void swap(SVMVector &v);

    /*!
     * \brief Copy data from the other SVMVector
     * \param[in] v SVMVector from which need copy data
     */
    void setData(const SVMVector &v);

    /*!
     * \brief Get size of the SVMVector
     * \return Size of the data array
     */
    int size() const;

private:
    /*!
     * \brief Size of data array
     */
    unsigned int varCount;
};

/*!
 * \brief Features and responses for training model
 */
class IVCP_EXPORT SVMTrainData
{
public:
    /*!
     * \brief Responses
     * \details An array containing target values. (integers in classification, 
     * real numbers in regression)
     */
    double* responses;
    /*!
     * \brief Features for training
     */
    SVMVector* features;

    /*!
     * \brief Constructor
     * \details Sets the number of features and allocates memory for responses and features
     * \param[in] numOfFeatures The number of the features for training
     */
    SVMTrainData(int numOfFeatures);

    /*!
     * \brief Destructor
     */
    ~SVMTrainData();

    /*!
     * \brief Releases the allocated memory (features and responses)
     */
    void release();

    /*!
     * \brief Set data for SVM model training
     * \param[in] _features The features for training
     * \param[in] _responses The corresponding classes
     * \throw NoDataException The features or the responses are missing
     * \throw IncorrectParamException Number of features and number of responses mismatch
     */
    void set(const std::list<cv::Mat>& _features, const std::vector<double>& _responses);

    /*!
     * \brief Get size of training data
     * \return Number of features
     */
    unsigned int size() const { return numberOfFeatures; }

private:
    /*!
     * \brief The size of training data
     */
    unsigned int numberOfFeatures;
};

/*!
 * \brief SVM model
*/
class IVCP_EXPORT LibSVMModel
{
public:
    /*!
     * \brief The parameters of the model
     */
    ivcp::SVMParameters params;
    /*!
     * \brief Number of classes, = 2 in regression/one class svm
     */
    int class_count;
    /*!
     * \brief Total number of support vectors
     */
    int sv_count;        
    /*!
     * \brief Support vectors
     */
    SVMVector* SV;
    /*!
     * \brief Coefficients for SVs in decision functions (sv_coef[class_count-1][sv_count]) 
     */
    double** sv_coef;
    /*!
     * \brief Constants in decision functions (rho[class_count*(class_count-1)/2])
     */
    double* rho;
    /*!
     * \brief Pairwise probability information (with probB)
     */
    double* probA;    
    /*!
     * \brief Pairwise probability information (with probA)
     */
    double* probB;
    /*!
     * \brief Values in [1, ..., num_training_data] to indicate SVs in the training set
     */
    int* sv_indices;    

    /*!
     * \brief Labels of each class
     */
    int* labels;
    /*!
     * \brief Number of support vectors for each class
     * \details nSV[0] + nSV[1] + ... + nSV[class_count-1] = sv_count
     */
    int* numSVForClasses;    
    /*!
     * \brief Default constructor
     */
    LibSVMModel();
    /*!
     * \brief Destructor
     */
    ~LibSVMModel();
};

/*!
 * \brief Train SVM model
 * \param[in] prob Data for training
 * \param[in] param The parameters of classifier model
 * \return SVM model
 */
IVCP_EXPORT LibSVMModel* svmTrain(const SVMTrainData* prob, const ivcp::SVMParameters* param);
/*!
 * \brief Classification or regression on a test vector x given a model
 * \param[in] model SVM model
 * \param[in] x Test vector
 * \return For a classification model, the predicted class for x is returned. 
 * For a regression model, the function value of x calculated using the model is returned. 
 * For an one-class model, +1 or -1 is returned.
 */
IVCP_EXPORT double svmPredict(const LibSVMModel* model, const SVMVector &x);
/*!
 * \brief Classification or regression on a test vector x given a model with probability information
 * \details For regression/one-class SVM, the array prob_estimates is unchanged and 
 * the returned value is the same as that of svmPredict
 * \param[in] model SVM model
 * \param[in] x Test vector
 * \param[out] prob_estimates Probability estimates for each class
 * \return The class with the highest probability
 */
IVCP_EXPORT double svmPredictProbability(const LibSVMModel* model, const SVMVector &x, double* prob_estimates);
};

/** @}*/
#endif
