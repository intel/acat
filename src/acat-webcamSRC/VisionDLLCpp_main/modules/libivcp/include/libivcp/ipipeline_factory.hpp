///////////////////////////////////////////////////////////////////////////
// <copyright file="ipipeline_factory.hpp" company="Intel Corporation">
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


#include "libivcp/ipipeline.hpp"
#include "libivcp/igui_framework.hpp"

#ifndef __IPIPELINE_FACTORY_HPP__
#define __IPIPELINE_FACTORY_HPP__

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file ipipeline_factory.hpp
 * \brief Declaration of Pipeline factory interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Pipeline factory interface
 * \details Pipeline factory creates pipeline instance
 */
class IVCP_EXPORT IPipelineFactory
{
public:
    /*!
     * \brief Pipeline factory destructor
     */
    virtual ~IPipelineFactory() {};

    /*!
     * \brief Create new instance of Pipeline
     * \param[in] mode Pipeline execution mode
     * \param[in] id Identifier of pipeline
     * \param[in] settings Pipeline settings
     * \return Pipeline object
     * \throw IncorrectParamException Pipeline settings are not correct
     */
    virtual SPtr<IPipeline> getPipeline(PipelineMode mode, std::string id = "", IPipeline::Settings settings = IPipeline::Settings()) = 0;

    /*!
     * \brief Create new instance of parallel Pipeline
     * \param[in] mode Pipeline execution mode (Note: you can't create pipeline in train mode)
     * \param[in] id Identifier of pipeline
     * \param[in] settings Pipeline settings
     * \return ParallelPipeline object
     * \throw IncorrectParamException Pipeline settings are not correct
     */
    virtual SPtr<IPipeline> getParallelPipeline(PipelineMode mode, std::string id = "", IPipeline::Settings settings = IPipeline::Settings()) = 0;

    /*!
     * \brief Create new instance of GUIFramework
     * \param[in] pipeline Pipeline that will be visualised
     * \param[in] framerate Quantity of pipeline processed frames to visualize per second.(Please note that real framerate could be lower in case the pipeline couldn't process frames fast enough)
     * \return GUIFramework object
     * \throw IncorrectParamException Pipeline is incorrect
     */
    virtual SPtr<IGUIFramework> getGUIFramework(SPtr<IPipeline> pipeline, float framerate = 30.303f) = 0;

protected:

private:
};
}
/** @}*/
#endif

