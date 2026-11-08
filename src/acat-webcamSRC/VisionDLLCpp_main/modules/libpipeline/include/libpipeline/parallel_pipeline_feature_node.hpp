///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_feature_node.hpp" company="Intel Corporation">
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


#ifndef __PARALLEL_PIPELINE_FEATURE_NODE_HPP__
#define __PARALLEL_PIPELINE_FEATURE_NODE_HPP__

#include <map>
#include <queue>
#include "libpipeline/pipeline_feature_node.hpp"
#include "libpipeline/parallel_pipeline_node.hpp"
#include "libivcp/feature/ifeature.hpp"
#include "libivcp/video_source/video_playlist.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file parallel_pipeline_feature_node.hpp
 * \brief Declaration of pipeline feature filter class
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Parallel feature filter wrapper node implementation
 */
class IVCP_EXPORT ParallelPipelineFeatureNode: public PipelineFeatureNode, ParallelPipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] feature Feature filter to create wrapper node for
     * \param[in] period Frame period during which feature node should process new data
     * \param[in] settings Pipeline settings
     */
    ParallelPipelineFeatureNode(SPtr<IFeature> feature, size_t period, IPipeline::Settings settings = IPipeline::Settings());
    virtual ~ParallelPipelineFeatureNode() {};
    
    virtual void run();

    virtual void transmitMessage(cv::Mat message, std::string caller);

    virtual void init();

    virtual void finalize();

    virtual void setPriority(IPipeline::ProcessingPriority prior);

protected:
    /*!
     * \brief Thread function for worker thread
     * \param[in] context Pointer to ParallelPipelineFeatureNode object in main thread
     */
    static void pipelineFeatureNodeWorker(ParallelPipelineFeatureNode* context);

    /*!
     * \brief Shared object for transfer calculated feature value from worker to main thread
     */
    cv::Mat shared_feature;

    /*!
     * \brief Feature calculated on previous step
     */
    cv::Mat prev_feature;

private:
};

};

/** @}*/
#endif 
