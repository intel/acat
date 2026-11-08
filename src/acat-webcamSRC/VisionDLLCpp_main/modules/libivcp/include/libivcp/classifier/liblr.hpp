///////////////////////////////////////////////////////////////////////////
// <copyright file="liblr.hpp" company="Intel Corporation">
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

  The code has been derived from liblinear library
  R.-E. Fan, K.-W. Chang, C.-J. Hsieh, X.-R. Wang, and C.-J. Lin.
  LIBLINEAR: A Library for Large Linear Classification, Journal of
  Machine Learning Research 9(2008), 1871-1874. Software available at
  http://www.csie.ntu.edu.tw/~cjlin/liblinear

  Here is the orignal copyright:
------------------------------------------------------------------------------------------
  
  Copyright (c) 2007-2013 The LIBLINEAR Project.
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


#ifndef __LIBLR_HPP__
#define __LIBLR_HPP__

#include <vector>
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/classifier/lr_params.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file liblr.hpp
 * \brief liblr library: types and functions
 * \details http://www.csie.ntu.edu.tw/~cjlin/liblinear
 */
namespace liblr
{
/*!
 * \brief Structure for storing non-zero features conponents (as an element of sparse array)
 */
struct feature_node
{
    /*!
     * \brief index of non-zero element in array
     */
    int index;
    /*!
     * \brief non-zero value
     */
    double value;
};

/*!
 * \brief Features and responses for training model
 */
class IVCP_EXPORT LRTrainData
{
public:
    /*!
     * \brief Train dataset size
     */
    size_t l;
    /*!
     * \brief feature size
     */
    size_t n;
    /*!
     * \brief train responses
     */
    double *y;
    /*!
     * \brief train features (as sparse arrays)
     */
    struct feature_node **x;
    /*!
     * \brief bias feature (if > 0 then additional term is added to feature vector)
     */
    double bias;

    /*!
     * \brief Constructor
     * \details Sets the number of features and allocates memory for responses and features
     */
    LRTrainData();
    /*!
     * \brief Destructor
     */
    ~LRTrainData();

    /*!
     * \brief Releases the allocated memory (features and responses)
     */
    void release();

    /*!
     * \brief Set data for LR model training
     * \param[in] _features The features for training
     * \param[in] _responses The corresponding classes
     * \param[in] bias Whether to use bias term in model or not
     * \throw NoDataException The features or the responses are missing
     * \throw IncorrectParamException Number of features and number of responses mismatch
     */
    void set(const std::list<cv::Mat>& _features, const std::vector<double>& _responses, double bias = 1);

    /*!
     * \brief Get size of training data
     * \return Size of training dataset
     */
    size_t size() const { return l; }

private:
};

/*!
 * \brief Logistic Regression model
*/
class IVCP_EXPORT LibLRModel
{
public:
    /*!
     * \brief The parameters of the model
     */
    ivcp::LRParameters param;
    /*!
     * \brief Number of classes
     */
    int nr_class;		/* number of classes */
    /*!
     * \brief feature size
     */
    int nr_feature;
    /*!
     * \brief feature weights for each class (calculated while training)
     */
    double *w;
    /*!
     * \brief Labels of each class
     */
    int *label;		/* label of each class */
    /*!
     * \brief bias feature (if > 0 then additional dimansion is added to feature vector)
     */
    double bias;
    /*!
     * \brief Default constructor
     */
    LibLRModel();
    /*!
     * \brief Destructor
     */
    ~LibLRModel();
};

/*!
 * \brief Train LR model
 * \param[in] prob Data for training
 * \param[in] param The parameters of classifier model
 * \return LR model
 */
IVCP_EXPORT LibLRModel* lrTrain(const LRTrainData* prob, const ivcp::LRParameters* param);

/*!
* \brief Classification on a test vector x given a model
* \param[in] model LR model
* \param[in] x Test vector
* \param[in] bias Bias vector
* \return The predicted class for x is returned.
*/
IVCP_EXPORT double lrPredict(const LibLRModel* model, const std::vector<double>& x, const double bias);
/*!
* \brief Classification on a test vector x given a model with probability information
* \param[in] model LR model
* \param[in] x Test vector
* \param[in] bias Bias vector
* \param[out] prob_estimates Probability estimated for each class
* \return The class with the highest probability
*/
IVCP_EXPORT double lrPredictProbability(const LibLRModel* model, const std::vector<double>& x, const double bias, double* prob_estimates);

/*!
 * \brief Check parameters correctness
 * \param[in] param Parameter to check
 * \return Error message if parameter is wrong
 */
IVCP_EXPORT const char *lrCheckParameter(const ivcp::LRParameters *param);
};
/** @}*/
#endif
