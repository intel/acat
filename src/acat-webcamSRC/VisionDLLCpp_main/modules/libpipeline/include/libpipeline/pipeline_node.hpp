///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_node.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_NODE_HPP__
#define __PIPELINE_NODE_HPP__

#include <set>
#include <condition_variable>
#include "libivcp/ipipeline.hpp"
#include "libpipeline/ipipeline_node.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_node.hpp
 * \brief Declaration of pipeline filter class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief pipeline node implementation
 * \details The Pipeline node component is responsible for receiving information from previous levels of pipeline
 * processing of this information via incorporated filter object and sending results to the next level of pipeline.
 */
class IVCP_EXPORT PipelineNode: public IPipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] filter Filter to create wrapper node for
     */
    PipelineNode(SPtr<IFilter> filter);
    virtual ~PipelineNode();

    virtual std::string getId() const;
    virtual NodeType getType();
    virtual bool isReady();
    virtual std::list<SPtr<IPipelineNode> > getDestinations();

    virtual void addDestinationLink(SPtr<IPipelineNode> to);
    virtual void removeDestinationLink(SPtr<IPipelineNode> to);
    virtual void addSourceLink(std::string from);
    virtual void removeSourceLink(std::string from);
    virtual void setPerformance(SPtr<IPerformance> performance);
    virtual void setPipeline(IPipeline* pipeline);
    
    //Default implementation of init and finalize are empty function
    virtual void init() {};
    virtual void finalize() {};
    
    virtual void addCallback(PipelineCallback func, void* context);
    virtual void setPriority(IPipeline::ProcessingPriority prior);
protected:
    /*!
     * \brief Node readiness flag
     * \details true in case enough information is provided for evaluation, false otherwise
     */
    bool ready;

    /*!
     * \brief Type of the wrapped pipeline filter(pipeline level)
     */
    NodeType type;

    /*!
     * \brief Pointer to the wrapped pipeline filter
     */
    SPtr<IFilter> filter;

    /*!
     * \brief Pointers to successive pipeline nodes(in pipeline link flow graph)
     */
    std::list<SPtr<IPipelineNode> > destinations;

    /*!
     * \brief Names of preceding pipeline nodes(in pipeline link flow graph)
     */
    std::list<std::string> sources;

    /*!
     * \brief Pointer to used performance counter filter
     */
    SPtr<IPerformance> performanceCounter;

    /*!
     * \brief Pointer to active pipeline
     */
    IPipeline* activePipeline;

    /*!
     * \brief Pointer to a set of pairs (callback function, context pointer)
     */
    std::set<std::pair<PipelineCallback, void *> > callbacks;

    /*!
     * \brief Set node readiness flag
     */
    void setReady();

    /*!
     * \brief Reset node readiness flag
     */
    void resetReady();

    /*!
     * \brief Report filter processing start time to configured performance counter
     */
    void reportStart();

    /*!
     * \brief Report filter processing stop time to configured performance counter
     */
    void reportStop();
private:

};

};

/** @}*/
#endif

