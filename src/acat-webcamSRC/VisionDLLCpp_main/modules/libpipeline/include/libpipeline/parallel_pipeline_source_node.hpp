///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_source_node.hpp" company="Intel Corporation">
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


#ifndef __PARALLEL_PIPELINE_SOURCE_NODE_HPP__
#define __PARALLEL_PIPELINE_SOURCE_NODE_HPP__

#include "libpipeline/pipeline_source_node.hpp"
#include "libpipeline/parallel_pipeline_node.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_source_node.hpp
 * \brief Declaration of parallel pipeline source filter class
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Parallel source filter wrapper node implementation
 */
class IVCP_EXPORT ParallelPipelineSourceNode: public PipelineSourceNode, ParallelPipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] _source Source filter to create wrapper node for
     * \param[in] period Frame period during which source node should process new data
     */
    ParallelPipelineSourceNode(SPtr<IVideoPlayer> _source, size_t period);
    virtual ~ParallelPipelineSourceNode() {};
    
    virtual void run();

    /*!
     * \brief Seek to the first frame in the playlist
     */
    virtual void init();

    virtual void setPriority(IPipeline::ProcessingPriority prior);

    virtual void finalize();

protected:
    /*!
     * \brief Thread function for worker thread
     * \param[in] context Pointer to ParallelPipelineSourceNode object in main thread
     */
    static void pipelineSourceNodeWorker(ParallelPipelineSourceNode* context);

    /*!
     * \brief Shared object for transfer new frame from worker to main thread
     */
    cv::Mat shared_frame;

    /*!
     * \brief Frames counter
     */
    size_t frameCount;

private:

};

};

/** @}*/
#endif 
