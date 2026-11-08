///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_source_node.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_SOURCE_NODE_HPP__
#define __PIPELINE_SOURCE_NODE_HPP__

#include "libpipeline/pipeline_node.hpp"
#include "libivcp/video_source/ivideo_player.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_source_node.hpp
 * \brief Declaration of pipeline source filter class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Source filter wrapper node implementation
 */
class IVCP_EXPORT PipelineSourceNode: public PipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] _source Source filter to create wrapper node for
     */
    PipelineSourceNode(SPtr<IVideoPlayer> _source);
    virtual ~PipelineSourceNode() {};

    virtual void addSourceLink(std::string from);
    virtual void removeSourceLink(std::string from);
    
    virtual void run();
    virtual void transmitMessage(cv::Mat message, std::string caller);

    /*!
     * \brief Seek to the first frame in the playlist
     */
    virtual void init();

protected:
    /*!
     * \brief Pointer to the wrapped source filter(casted filter pointer)
     */
    SPtr<IVideoPlayer> source;

private:

};

};

/** @}*/
#endif 
