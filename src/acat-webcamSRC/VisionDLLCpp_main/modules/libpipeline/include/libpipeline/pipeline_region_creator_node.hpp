///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_region_creator_node.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_REGION_CREATOR_NODE_HPP__
#define __PIPELINE_REGION_CREATOR_NODE_HPP__

#include "libpipeline/pipeline_node.hpp"
#include "libivcp/region_creator/iregion_creator.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_region_creator_node.hpp
 * \brief Declaration of pipeline region creator filter class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Region creator filter wrapper node implementation
 */
class IVCP_EXPORT PipelineRegionCreatorNode: public PipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] region Region creator filter to create wrapper node for
     */
    PipelineRegionCreatorNode(SPtr<IRegionCreator> region);
    virtual ~PipelineRegionCreatorNode() {};

    virtual void addSourceLink(std::string from);
    virtual void removeSourceLink(std::string from);
    
    virtual void run();
    virtual void transmitMessage(cv::Mat message, std::string caller);

protected:
    /*!
     * \brief Pointer to the wrapped region creator(casted filter pointer)
     */
    SPtr<IRegionCreator> region;

    /*!
     * \brief Last received frame
     */
    cv::Mat sourcedata;

#if defined USE_DLIB
    /*!
     * \brief Index of region node
     */
    int regionNodeNumber;

    /*!
     * \brief Counter of region nodes
     * \details This variable is used for counting nodes and giving index to node
     */
    static int countRegionNodes;
#endif

private:

};

};

/** @}*/
#endif 
