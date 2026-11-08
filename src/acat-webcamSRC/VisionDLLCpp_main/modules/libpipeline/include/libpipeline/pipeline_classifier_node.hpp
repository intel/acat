///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_classifier_node.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_CLASSIFIER_NODE_HPP__
#define __PIPELINE_CLASSIFIER_NODE_HPP__

#include "libivcp/classifier/iclassifier.hpp"
#include "libivcp/data_source/iclassifier_result_data_source.hpp"
#include "libivcp/ipipeline.hpp"
#include "libivcp/video_source/video_playlist.hpp"
#include "libpipeline/pipeline_node.hpp"
#include <fstream>

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_classifier_node.hpp
 * \brief Declaration of pipeline classifier filter class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Classifier filter wrapper node implementation
 */
class IVCP_EXPORT PipelineClassifierNode: public PipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] classifier Classifier filter to create wrapper node for
     * \param[in] mode Pipepline operational mode
     */
    PipelineClassifierNode(SPtr<IClassifier> classifier, PipelineMode mode);
    virtual ~PipelineClassifierNode();

    virtual void addDestinationLink(SPtr<IPipelineNode> to);
    virtual void removeDestinationLink(SPtr<IPipelineNode> to);
    virtual void addSourceLink(std::string from);
    virtual void removeSourceLink(std::string from);
    
    virtual void run();
    virtual void finalize();
    virtual void transmitMessage(cv::Mat message, std::string caller);

    /*!
     * \brief Set a-priori frame mark source
     * \details frame mark source is used in training and testing mode to provide reference frame marks
     * \param[in] markup Pointer to a-priori frame mark source
     */
    virtual void setMarkup(SPtr<IClassifierResultDataSource> markup);
    /*!
     * \brief Set name of output file for tracking features
     * \param[in] filename Output filename
     */
    virtual void setOutputFilename(std::string filename);

    /*!
     * \brief Pointer to the wrapped classifier(casted filter pointer)
     */
    SPtr<IClassifier> classifier;
    /*!
     * \brief Check if classifier is ready to classify feature
     */
    virtual bool isClassifierReady(cv::Mat feature);

    /*!
     * \brief Last received feature matrix
     */
    cv::Mat feature;

    /*!
     * \brief Cached pipeline operational mode
     */
    PipelineMode mode;

    /*!
     * \brief Pointer to a-priori frame mark source(used in training and testing mode)
     */
    SPtr<IClassifierResultDataSource> classMarkup;

    /*!
     * \brief Stream for feature csv output 
     */
    std::fstream csv_output;

    /*!
     * \brief Default csv delimeter
     */
    static const char csv_delimeter = ',';

};

};

/** @}*/
#endif 
