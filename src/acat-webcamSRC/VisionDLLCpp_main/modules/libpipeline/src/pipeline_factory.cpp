///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_factory.cpp" company="Intel Corporation">
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


#include "libpipeline/pipeline_factory.hpp"
#include "libpipeline/pipeline.hpp"
#include "libpipeline/parallel_pipeline.hpp"
#include "libpipeline/gui_framework.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
SPtr<IPipeline> PipelineFactory::getPipeline(PipelineMode mode, string id, IPipeline::Settings settings)
{
    if (settings.distanceBetweenFrames < 1 || settings.distanceBetweenStartsFrameSeq < 1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Pipeline settings are not correct. Parameters should be integer values more than 0");
    }

    SPtr<IPipeline> pipeline = new Pipeline(mode, settings);
    if (id.empty())
    {
        id = "pipeline";
    }

    pipeline->setId(id);

    return pipeline;
}

SPtr<IPipeline> PipelineFactory::getParallelPipeline(PipelineMode mode, string id, IPipeline::Settings settings)
{
    if (settings.distanceBetweenFrames < 1 || settings.distanceBetweenStartsFrameSeq < 1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Pipeline settings are not correct. Parameters should be integer values more than 0");
    }

    SPtr<IPipeline> pipeline = new ParallelPipeline(mode, settings);
    if (id.empty())
    {
        id = "parallel_pipeline";
    }

    pipeline->setId(id);

    return pipeline;
}

SPtr<IGUIFramework> PipelineFactory::getGUIFramework(SPtr<IPipeline> pipeline, float framerate)
{
    if (pipeline == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Pipeline is null pointer");
    }

    if (pipeline->getSession() == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Session is not defined for current pipeline");
    }

    SPtr<IGUIFramework> gui = new GUIFramework(pipeline, framerate);
    return gui;
}
}

