///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_classifier_node.hpp" company="Intel Corporation">
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


#ifndef __PARALLEL_PIPELINE_CLASSIFIER_NODE_HPP__
#define __PARALLEL_PIPELINE_CLASSIFIER_NODE_HPP__

#include "libivcp/classifier/iclassifier.hpp"
#include "libivcp/data_source/iclassifier_result_data_source.hpp"
#include "libivcp/ipipeline.hpp"
#include "libivcp/video_source/video_playlist.hpp"
#include "libpipeline/pipeline_classifier_node.hpp"
#include "libpipeline/parallel_pipeline_node.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file parallel_pipeline_classifier_node.hpp
 * \brief Declaration of parallel pipeline classifier filter class
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Parallel classifier filter wrapper node implementation
 */
class IVCP_EXPORT ParallelPipelineClassifierNode: public PipelineClassifierNode, ParallelPipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] classifier Classifier filter to create wrapper node for
     * \param[in] mode Pipepline operational mode
     * \param[in] period Frame period during which classification node should process new data
     */
    ParallelPipelineClassifierNode(SPtr<IClassifier> classifier, PipelineMode mode, size_t period);

    /*!
     * \brief Destructor
     */
    virtual ~ParallelPipelineClassifierNode() {};
    
    virtual void run();

    virtual void transmitMessage(cv::Mat message, std::string caller);

    virtual void init();

    virtual void setPriority(IPipeline::ProcessingPriority prior);

    virtual void finalize();

protected:
    /*!
     * \brief Thread function for worker thread
     * \param[in] context Pointer to ParallelPipelineClassifierNode object in main thread
     */
    static void pipelineClassifierNodeWorker(ParallelPipelineClassifierNode* context);

    /*!
     * \brief Shared object for transfer classification result from worker to main thread
     */
    ClassType sharedClassType;

    /*!
     * \brief Shared object for transfer probabiliry vector from worker to main thread
     */
    std::vector<float> sharedProb;
    
    /*!
     * \brief Class type calculated on previous step
     */
    ClassType prevClassType;

    /*!
     * \brief Probability vector calculated on previous step
     */
    std::vector<float> prevProb;

private:

};

};

/** @}*/
#endif 
