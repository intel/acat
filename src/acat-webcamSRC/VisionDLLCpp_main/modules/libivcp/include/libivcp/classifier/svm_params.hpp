///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_params.hpp" company="Intel Corporation">
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

#ifndef __SVM_PARAMS_HPP__
#define __SVM_PARAMS_HPP__

#include "libivcp/types.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file svm_params.hpp
 * \brief The parameters of an SVM model
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
 /*!
 * \brief The parameters of an SVM model
 */
class IVCP_EXPORT SVMParameters
{
public:
    /*!
     * \brief Types of a SVM
     * \details Possible values:
     * 1) C_SVC - C-Support Vector Classification. n-class classification (n >= 2), allows imperfect separation of classes with penalty multiplier C for outliers.
     * This value is used by default.
     * 2) NU_SVC - nu-Support Vector Classification. n-class classification with possible imperfect separation. 
     * Parameter nu (in the range 0..1, the larger the value, the smoother the decision boundary) is used instead of C.
     * 3) ONE_CLASS - Distribution Estimation (One-class SVM). All the training data are from the same class, 
     * SVM builds a boundary that separates the class from the rest of the feature space.
     * 4) EPSILON_SVR - epsilon-Support Vector Regression. The distance between feature vectors from the training set and the fitting hyper-plane must be less than p. 
     * For outliers the penalty multiplier C is used.
     * 5) NU_SVR - nu-Support Vector Regression. nu is used instead of p.
     */
    SVMType svmType;

    /*!
     * \brief Types of a SVM kernel
     * \details Possible values:
     * 1) LINEAR - Linear kernel, u' * v. This kernel is used by default.
     * 2) POLY - Polynomial kernel: (gamma * u' * v + coef0) ^ degree
     * 3) RBF - Radial basis function (RBF):    exp(-gamma * |u - v|^2)
     * 4) SIGMOID - Sigmoid kernel:	tanh(gamma * u' * v + coef0)
     */
    SVMKernelType kernelType;

    /*!
     * \brief Parameter degree of a polynomial kernel function (POLY)
     * \details Must be positive or zero. Default value is zero.
     */
    int degree;

    /*!
     * \brief Parameter gamma of the kernels POLY, RBF, SIGMOID
     * \details Must be positive or zero. Default value is 1.
     */
    double gamma;

    /*!
     * \brief Parameter coef0 of the kernels POLY, SIGMOID
     * \details Default value is zero
     */
    double coef0;

    /*!
     * \brief The size of the kernel cache, specified in megabytes
     * \details Must be positive. Default value is 50 Mb
     */
    double cacheSize;

    /*!
     * \brief Termination criteria of the iterative SVM training procedure
     * \details Must be positive. Default value is 0.00001
     */
    double eps;

    /*!
     * \brief The cost of constraints violation
     * \details This parameter is used if a kernel type is C_SVC, EPSILON_SVR or NU_SVR. 
     * Must be positive. Default value is 1
     */
    double C;    

    /*!
     * \brief Parameter nu of a SVM optimization problem
     * \details This parameter is used if a kernel type is NU_SVC, ONE_CLASS or NU_SVR. 
     * The larger the value, the smoother the decision boundary.
     * Must be in the range (0, 1] not including zero. Also if svmType is NU_SVC,
     * the following condition should be performed for each class pair:
     * nu * (n1 + n2) / 2 <= min(n1, n2), where n1 - number of features for one class, 
     * n2 - number of features for another class.
     * Default value is 0. You must set this parameter if used kernel is NU_SVC, ONE_CLASS or NU_SVR. 
     */
    double nu;

    /*!
     * \brief Parameter p of a SVM optimization problem
     * \details This parameter is used if a kernel type is EPSILON_SVR.
     * The distance between feature vectors from the training set and the fitting hyper-plane must be less than p.
     * Must be positive or zero. Default value is zero.
     */
    double p;

    /*!
     * \brief The number of elements in the array weightLabels and weights
     * \details This parameter is used only for C_SVC, in other cases weightCount = 0.
     * Default value is zero
     */
    int weightCount;

    /*!
     * \brief Class labels for weights, used only for C_SVC
     */
    std::vector<int> weightLabels;

    /*!
     * \brief Optional weights for the C_SVC, assigned to particular classes
     * \details They are multiplied by C so the parameter C of class weightLabels[i] becomes weights[i]*C. 
     * Thus these weights affect the misclassification penalty for different classes. 
     * The larger weight, the larger penalty on misclassification of data from the corresponding class.
     */
    std::vector<double> weights;    
    
    /*!
     * \brief Need use the shrinking heuristics
     */
    bool useShrinking;

    /*!
     * \brief Need estimate probabilities
     * \details This parameter is needed for libsvm. It is always true.
     * If you set it in false, before the training SVM it will be set in true
     */
    bool estimateProbability;

    /*!
     * \brief Default constructor
     * \param[in] svmType Type of SVM
     * \param[in] kernelType Type of a SVM kernel
     * \param[in] degree Parameter degree of a kernel function (POLY)
     * \param[in] gamma Parameter gamma of a kernel function (POLY / RBF / SIGMOID)
     * \param[in] coef0 Parameter coef0 of a kernel function (POLY / SIGMOID)
     * \param[in] epsilon Termination criteria of the iterative SVM training procedure
     * \param[in] C The cost of constraints violation (C_SVC / EPSILON_SVR / NU_SVR)
     * \param[in] nu Parameter nu of a SVM optimization problem (NU_SVC / ONE_CLASS / NU_SVR)
     * \param[in] p Parameter p of a SVM optimization problem (EPSILON_SVR)
     * \param[in] weightCount The number of elements in the array weightLabels and weights
     * \param[in] weightLabels Class labels for weights, used for C_SVC
     * \param[in] weights Class weights, used for C_SVC
     * \param[in] useShrinking Need use the shrinking heuristics
     * \param[in] cache The size of the kernel cache, specified in megabytes
     */
    SVMParameters(SVMType svmType = C_SVC, SVMKernelType kernelType = RBF, int degree = 0, double gamma = 1, 
        double coef0 = 0, double epsilon = 0.00001, double C = 1, double nu = 0, double p = 0, 
        int weightCount = 0, std::vector<int> weightLabels = std::vector<int>(), std::vector<double> weights = std::vector<double>(),
        bool useShrinking = false, double cache = 50);
    /*!
     * \brief Destructor
     */
    ~SVMParameters() 
    {
    }
};
};

/** @}*/
#endif