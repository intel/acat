///////////////////////////////////////////////////////////////////////////
// <copyright file="types.hpp" company="Intel Corporation">
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


#ifndef __TYPES_HPP__
#define __TYPES_HPP__

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file types.hpp
 * \brief Declaration of simple datatypes
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

#ifndef IVCP_EXPORT
/*!
 * \brief Dll export
 */
#ifdef IVCP_API_EXPORTS
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#define IVCP_EXPORT __declspec(dllexport)
#else
#define IVCP_EXPORT
#endif
#else
#define IVCP_EXPORT
#endif
#endif

#ifdef _MSC_VER
#pragma warning( disable : 4251 )
#endif

#include "opencv2/opencv.hpp"
#include "opencv2/geometry.hpp"
#include <deque>

namespace ivcp
{
/*! 
 * \brief Type of regions known by the region creators
 */
enum RegionType
{
    REGION_MOUTH, /**< Mouth */
    REGION_NOSE,  /**< Nose */
    REGION_LEYE,  /**< Left eye */
    REGION_REYE,  /**< Right eye */
    REGION_LEYEBROW,  /**< Left eye */
    REGION_REYEBROW,  /**< Right eye */
    REGION_FACE,  /**< Face */
    REGION_LCHEEK,  /**< Left cheek */
    REGION_RCHEEK  /**< Right cheek */
};

/*! 
 * \brief Type of a Video source
 */
enum VideoSourceType
{
    VIDEOFILE,
    CAMERA,
    NEW_VIDEO_SOURCE, 
    RAWFILE,
    RS_CAMERA
};

/*! 
 * \brief Type of a Region creator
 */
enum RegionCreatorType
{
    BBOX,
    GROUNDTRUTH,
    NEW_REGION_CREATOR,
    DLIB_REGION,
    RS_REGION
};

/*! 
 * \brief Type of a Feature
 */
enum FeatureType
{
    NORMALFLOW,
    STLBP,
    CONCATENATOR,
    AGGREGATOR,
    VLBP,
    CLASSIFIER_FEATURE,
    SUMMATOR,
    NEW_FEATURE,
    NORMALFLOWBLURRED,
    MOTIONESTIMATIONSTDDEV
};

/*!
 * \brief Type of classifier output
 */
enum ClassifierOutputType
{
    PROBABILISTIC,
    CATEGORICAL
};

/*! 
 * \brief Type of a Classifier
 */
enum ClassifierType
{
    SVM,
    NEW_CLASSIFIER = 2,
    LOGISTIC_REGRESSION
};

/*! 
 * \brief Type of a Performance filter
 */
enum PerformanceType
{
    TEXT_REPORT,
    NEW_PERFORMANCE
};

/*! 
 * \brief Type of a Data Source - this enumeration is obsolete and used only for backward compatability 
 * with the previously created repositories
 * \details This type is obsolete. Use ClassifierResultType and GTRegionsType instead
 */
enum DataSourceTypeObsolete
{
    CLASSIFIER_RESULT_OBSOLETE,
    GROUNDTRUTH_REGIONS_OBSOLETE,
};

/*!
 * \brief Type of classifier results source
 */
enum ClassifierResultType
{
    XML_CLASSIFIER_RESULTS,
    NEW_CLASSIFIER_RESULTS
};

/*!
 * \brief Type of groundtruth regions source
 */
enum GTRegionsType
{
    XML_GROUNDTRUTH_REGIONS,
    NEW_GROUNDTRUTH_REGIONS
};

/*! 
 * \brief Root type of a Filter
 */
enum FilterType
{
    CLASSIFIER_FILTER, 
    DATA_SOURCE_OBSOLETE, 
    FEATURE_FILTER, 
    PERFORMANCE_FILTER, 
    REGION_CREATOR_FILTER, 
    VIDEO_SOURCE_FILTER, 
    VIDEO_PLAYER_FILTER, 
    SESSION_FILTER, 
    PIPELINE_FILTER,
    VIDEO_PLAYLIST_FILTER,
    CLASSIFIER_RESULTS_FILTER,
    GROUNDTRUTH_REGIONS_FILTER,
    PARALLEL_PIPELINE_FILTER
};

/*!
 * \brief Types of a SVM formulation
 * \details C_SVC - C-Support Vector Classification
 * NU_SVC - nu-Support Vector Classification
 * ONE_CLASS - Distribution Estimation (One-class SVM)
 * EPSILON_SVR - epsilon-Support Vector Regression
 * NU_SVR - nu-Support Vector Regression
 */
enum SVMType
{ 
    C_SVC, 
    NU_SVC, 
    ONE_CLASS, 
    EPSILON_SVR, 
    NU_SVR 
};    

/*!
 * \brief Types of a Logistic Regression formulation
 * \details 
 */
enum LRType
{
    L2R_LR,     //L2-regularized logistic regression (primal)
    L1R_LR,     //L1-regularized logistic regression
    L2R_LR_DUAL //L2-regularized logistic regression (dual)
};

/*!
 * \brief Types of a SVM kernel
 * \details LINEAR:	u' * v
 * POLY: (gamma * u' * v + coef0) ^ degree
 * RBF:	exp(-gamma * |u - v|^2)
 * SIGMOID:	tanh(gamma * u' * v + coef0)
 */
enum SVMKernelType
{ 
    LINEAR, 
    POLY, 
    RBF, 
    SIGMOID
};

/*! 
 * \brief Verbosity level of a Logger
 */
enum LogLevel
{
    LOG_INFO,
    LOG_WARNING,
    LOG_ERROR,
	LOG_NONE
};

/*! 
 * \brief Type of normal flow blurred feature
 */
enum NFBFType
{
    NFBF_ENERGY,
    NFBF_DX_DY,
    NFBF_ENERGY_DX_DY
};

/*!
 * \brief Type of transformation for normalization
 */
enum TypeOfTransform
{
    PERSPECTIVE = 0,
    ROTATION = 1
};

/*!
 * \brief Type of a Video output
 */
enum VideoOutputType
{
    VIDEOFILE_OUTPUT,
    RAWFILE_OUTPUT
};

/*!
 * \brief Sequence of regions used by the dynamic features
 */
typedef std::deque<cv::Mat > RegionSequence;

/*! 
 * \brief Classification type (ie which type classify function should return)
 */
typedef int ClassType;

#if (defined USE_DLIB || defined USE_REALSENSE)
/*!
* \brief Stabilization markers structure
*/
typedef struct StabMarker
{
    cv::Point2d noseLeft;
    cv::Point2d noseRight;
    cv::Point2d noseTop;
    cv::Point2d noseBottom;
} StabMarker;
#endif

/*!
 * \brief This means that frame is not classified
 */
#define NOT_CLASSIFIED -1
};

/** @}*/
#endif

