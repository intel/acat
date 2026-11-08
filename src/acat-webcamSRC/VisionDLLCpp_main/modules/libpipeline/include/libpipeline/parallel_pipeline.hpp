///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline.hpp" company="Intel Corporation">
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


#ifndef __PARALLEL_PIPELINE_HPP__
#define __PARALLEL_PIPELINE_HPP__

#include <string>
#include <chrono> 
#include "libivcp/video_source/video_playlist.hpp"
#include "libpipeline/pipeline.hpp"
#include "libpipeline/pipeline_node.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline.hpp
 * \brief Declaration of parallel pipeline class
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Parallel pipeline implementation
 * \details The Pipeline component is responsible for connecting all the rest components together and 
 * for managing the message traffic between the components.
*/
class IVCP_EXPORT ParallelPipeline: public Pipeline
{
public:
    /*!
     * \brief Constructor
     * \details Operational mode should be defined
     * \param[in] mode Pipeline operational mode
     * \param[in] settings Pipeline settings
     * \throw IncorrectParamException Couldn't create parallel pipeline in train mode
     */
    ParallelPipeline(PipelineMode mode, Settings settings = Settings());

    /*!
     * \brief Constructor
     * \details Operational mode should be defined
     * \param[in] session Session to search for filters
     * \param[in] mode Pipeline operational mode
     * \param[in] settings Pipeline settings
     * \throw IncorrectParamException Couldn't create parallel pipeline in train mode
     */
    ParallelPipeline(SPtr<Session > session, PipelineMode mode, Settings settings = Settings());

    /*!
     * \brief Destructor
     */
    virtual ~ParallelPipeline() {};

    virtual void run();

    virtual FilterType getRootType() const;

    /*!
     * \brief Set execution priority for the filter
     * \param[in] filterName Filter name, for which priority should be changed
     * \param[in] priority Priority to set (ivcp::IPipeline::LOW, ivcp::IPipeline::NORMAL, ivcp::IPipeline::HIGH)
     */
    virtual void setFilterPriority(const std::string &filterName, ProcessingPriority priority);

protected:
    /*!
     * \brief Create a node for provided filter
     * \param[in] filter Filter to create wrapper node for
     * \returns Pointer to created node or empty pointer if node could not be created
     * \throw IncorrectParamException No playlist is set or error in creating Pipeline Nodes
     */
    SPtr<IPipelineNode> addNode(SPtr<IFilter> filter);

    /*!
     * \brief Finalize pipeline and all internal nodes
     */
    void finalize();

    /*!
     * \brief Filter thread priority
     */
    std::map< std::string, IPipeline::ProcessingPriority > priorities;

private:

    /*!
     * \brief Based on measured filter performance update internal fps recommendations for the filters
     */
    void updateRecomendations();

    /*
     * \brief FPS of filters quick access map 
     */
    std::map<std::string,  float> filtersFPS; 

    /* 
     * \brief Measured time (in ms) of filters runtime
     */
    std::map<std::string, int> filtersWaitTime;

    /*
     * \brief Statistics of recommended fps for the latest frames
     * \details Map: filter name -> fps -> number of reccomendations
     */
    std::map<std::string, std::map<float, int> > fpsAdvices; 

    /*
     * \brief A counter for frames when pipeline was not able to run at required fps
     */
    int pipelineDelayCount;

    /*
     * \brief Last time we showed the warning message that the pipeline is not able to run at  required fps
     */
    std::chrono::high_resolution_clock::time_point lastFPSWarning;

};
};

/** @}*/
#endif

