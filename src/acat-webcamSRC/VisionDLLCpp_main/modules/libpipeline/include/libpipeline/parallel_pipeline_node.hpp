///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_node.hpp" company="Intel Corporation">
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


#ifndef __PARALLEL_PIPELINE_NODE_HPP__
#define __PARALLEL_PIPELINE_NODE_HPP__

#include <thread>
#include <mutex>
#include <condition_variable>
#include "libivcp/ipipeline.hpp"
#include "libpipeline/ipipeline_node.hpp"
#include "libpipeline/parallel_pipeline.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file parallel_pipeline_node.hpp
 * \brief Declaration of parallel pipeline filter
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief parallel pipeline node
 * \details The ParallelPipelineNode component contains common implementation 
 * which must have all parallel pipeline nodes
 */
class IVCP_EXPORT ParallelPipelineNode
{
public:
    /*!
     * \brief Constructor
     * \param[in] period Frame period shows how many steps of parallel pipeline 
     * during which pipeline node should process new result
     * \details Usually frame period is the ratio of the performance rate of filter and
     * parallel pipeline performance rate.
     */
    ParallelPipelineNode(size_t period): isWorkerReady(false), isConsumerReady(false), isConsumerFinalized(false), framePeriod(period), workerException(nullptr) {};

    /*!
     * \brief Destructor
     */
    virtual ~ParallelPipelineNode() {};

protected:
    /*!
    * \brief Set priority for the worker of the current node
    * \param[in] prior Priority to set (ivcp::IPipeline::LOW, ivcp::IPipeline::NORMAL, ivcp::IPipeline::HIGH)
    */
    void setWorkerPriority(IPipeline::ProcessingPriority prior);

    /*!
     * \brief Pointer to worker thread 
     */
    SPtr<std::thread> worker;

    /*!
     * \brief Mutex for sinchronization shared data 
     */
    std::mutex m;

    /*!
     * \brief Condition variable for communication between main and worker threads
     */
    std::condition_variable cv;

    /*!
     * \brief Worker readiness flag
     * \details true in case worker thread finished to process data
     */
    bool isWorkerReady;

    /*!
     * \brief Consumer readiness flag
     * \details true in case main thread provided all necessary data for worker thread
     */
    bool isConsumerReady;

    /*!
     * \brief Whether consumer was finalized
     * \details if true worker thread must be finalized too
     */
    bool isConsumerFinalized;

    /*!
     * \brief Lifetime of previous result
     * \details if this value greater than zero pipeline node must return result processed on previous step
     */
    int prevResultLifeTime;

    /*!
     * \brief Period during which pipeline node should process new result
     * \details determined by performance rate of filter and pipeline
     */
    size_t framePeriod;

    /*!
     * \brief Shared pointer to excpetion throwed in worker thread
     * \details if this pointer don't equal to NULL main thread should rethrow this exception
     */
    std::exception_ptr workerException;

private:

};

};

/** @}*/
#endif

