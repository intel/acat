///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_factory.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_FACTORY_HPP__
#define __PIPELINE_FACTORY_HPP__

#include <string>
#include "libivcp/ipipeline_factory.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_factory.hpp
 * \brief Declaration of Pipeline factory
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Pipeline factory implementation
 * \details Pipeline factory creates pipeline instance
 */
class IVCP_EXPORT PipelineFactory: public IPipelineFactory
{
public:
    virtual SPtr<IPipeline> getPipeline(PipelineMode mode, std::string id = "", IPipeline::Settings settings = IPipeline::Settings());

    virtual SPtr<IPipeline> getParallelPipeline(PipelineMode mode, std::string id = "", IPipeline::Settings settings = IPipeline::Settings());

    virtual SPtr<IGUIFramework> getGUIFramework(SPtr<IPipeline> pipeline, float framerate = 30.303f);

protected:

private:
};
}
/** @}*/
#endif

