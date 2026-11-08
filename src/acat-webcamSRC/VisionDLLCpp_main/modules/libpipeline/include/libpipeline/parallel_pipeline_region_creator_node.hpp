///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_region_creator_node.hpp" company="Intel Corporation">
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


#ifndef __PARALLEL_PIPELINE_REGION_CREATOR_NODE_HPP__
#define __PARALLEL_PIPELINE_REGION_CREATOR_NODE_HPP__

#include "libpipeline/pipeline_region_creator_node.hpp"
#include "libivcp/region_creator/iregion_creator.hpp"
#include "libpipeline/parallel_pipeline_node.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file parallel_pipeline_region_creator_node.hpp
 * \brief Declaration of parallel pipeline region creator filter class
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Parallel region creator filter wrapper node implementation
 */
    class IVCP_EXPORT ParallelPipelineRegionCreatorNode: public PipelineRegionCreatorNode, ParallelPipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] region Region creator filter to create wrapper node for
     * \param[in] period Frame period during which region creator node should process new data
     */
    ParallelPipelineRegionCreatorNode(SPtr<IRegionCreator> region, size_t period);
    virtual ~ParallelPipelineRegionCreatorNode() {};
    
    virtual void run();

    virtual void transmitMessage(cv::Mat message, std::string caller);

    virtual void init();

    virtual void setPriority(IPipeline::ProcessingPriority prior);

    virtual void finalize();

    /*!
     * \brief Get type of region
     */
    RegionCreatorType getRegionType() {return region->getType(); };
protected:
    /*!
     * \brief Thread function for worker thread
     * \param[in] context Pointer to ParallelPipelineRegionCreatorNode object in main thread
     */
    static void pipelineRegionCreatorNodeWorker(ParallelPipelineRegionCreatorNode* context);

    /*!
     * \brief Shared object for transfer calculated region description from worker to main thread
     */
    IRegionCreator::RegionDescription shared_region;

    /*!
     * \brief Region description calculated on previous step
     */
    IRegionCreator::RegionDescription prev_region;

    /*!
     * \brief Current processed frame
     */
    VideoPlaylistFrame currentFrame;
#if defined USE_DLIB
    /*!
     * \brief Index of current parallel region node
     */
    int parallelRegionNodeNumber;

    /*!
     * \brief Counter of parallel region nodes
     * \details This variable is used for counting nodes and giving index to node
     */
    static int countParallelRegionNodes;
#endif
    
private:

};

};

/** @}*/
#endif 
