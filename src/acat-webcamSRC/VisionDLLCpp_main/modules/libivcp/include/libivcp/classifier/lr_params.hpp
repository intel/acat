///////////////////////////////////////////////////////////////////////////
// <copyright file="lr_params.hpp" company="Intel Corporation">
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

#ifndef __LR_PARAMS_HPP__
#define __LR_PARAMS_HPP__

#include "libivcp/types.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file lr_params.hpp
 * \brief The parameters of an Logistic Regression model
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
 /*!
 * \brief The parameters of an Logistic Regression model
 */
class IVCP_EXPORT LRParameters
{
public:
    /*!
     * \brief Types of a Logistic Regression model
     * \details Possible values:
     * 1) L2R_LR - L2-regularized logistic regression (primal)
     * This value is used by default.
     * 2) L1R_LR - L1-regularized logistic regression
     * 3) L2R_LR_DUAL - L2-regularized logistic regression (dual)
     */
    LRType solver_type;

    /* these are for training only */
    /*!
     * \brief Termination criteria of the iterative LR training procedure
     * \details Must be positive.
     */
    double eps;
    /*!
     * \brief The cost of constraints violation.
     */
    double C;
    /*!
     * \brief The number of elements in the arrays weight_label and weight
     */
    int nr_weight;
    /*!
     * \brief Array of size nr_weight of class labels for weights. User is responsible for allocation/deallocation
     * of memory segment.
     */
    int *weight_label;
    /*!
     * \brief Optional class weights, assigned to particular classes
     * \details Array of size nr_weight is used to change the penalty  for some classes
     * (If the weight for a class is not changed, it is set to 1).
     * This is useful for training classifier using unbalanced
     * input data or with asymmetric misclassification cost.
     * User is responsible for allocation/deallocation of memory segment.
     */
    double* weight;

    /*!
     * \brief Constructor
     * \param[in] solver_type Type of Logistic Regression model
     * \param[in] eps Termination criteria of the iterative LR training procedure
     * \param[in] C The cost of constraints violation
     * \param[in] nr_weight The number of elements in the array weightLabel and weight
     * \param[in] weightLabel Class labels for weights
     * \param[in] weight Class weights
     */
    LRParameters(LRType solver_type = L2R_LR, double eps = 0.00001, double C = 1, int nr_weight = 0, int* weightLabel = NULL, double* weight = NULL);
    /*!
     * \brief Destructor
     */
    ~LRParameters() 
    {
    }
};
};

/** @}*/
#endif
