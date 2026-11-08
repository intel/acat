///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_serializer.cpp" company="Intel Corporation">
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

#include "libivcp/exceptions/io_exception.hpp"
#include "libinfra/repository/parallel_pipeline_serializer.hpp"
#include "libivcp/factory.hpp"
#include "libpipeline/pipeline.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
ParallelPipelineXMLSerializer::ParallelPipelineXMLSerializer(SPtr<IRepository> _rep):
    PipelineXMLSerializer(_rep)
{
}

ParallelPipelineXMLSerializer::~ParallelPipelineXMLSerializer()
{
}

SPtr<Filter> ParallelPipelineXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto filterMap = loadMap(fs, fn);

    vector<string> linksFrom = filterMap["links_from"].strVec;
    vector<string> linksTo = filterMap["links_to"].strVec;
    int mode = filterMap["mode"].ival;
    string sessionId = filterMap["session_id"].str;

    SPtr<IPipeline> pipeline = filter;
    if (pipeline == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to IPipeline for serialization");
        }
        pipeline = Factory::createParallelPipeline((PipelineMode)mode);
        filter = pipeline;
    }

    if(pipeline->getMode() != mode)
    {
        IVCP_EXCEPTION(IOException, "Could not change pipeline mode");
    }

    FilterXMLSerializer::load(fs, fn, pipeline);

    pipeline->setSession(rep->loadSession(sessionId));
    //Add links
    if (linksFrom.size() == linksTo.size())
    {
        for (unsigned int i = 0; i < linksFrom.size(); i++)
        {
            pipeline->addLink(linksFrom[i], linksTo[i]);
        }
    }
    else
    {
        stringstream msg;
        msg << "Links sizes are incorrect: size of linksFrom is " << linksFrom.size() << ", size of linksTo is " << linksTo.size();
        IVCP_EXCEPTION(IncorrectParamException, msg.str());
    }

    return pipeline;
}

}
