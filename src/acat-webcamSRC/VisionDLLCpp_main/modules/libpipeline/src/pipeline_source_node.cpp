///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_source_node.cpp" company="Intel Corporation">
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


#include "libpipeline/pipeline_source_node.hpp"
#include <algorithm>

using namespace std;
using namespace cv;

namespace ivcp
{
PipelineSourceNode::PipelineSourceNode(SPtr<IVideoPlayer> _source):
    PipelineNode(_source),source(_source)
{
    if ( source->haveNextFrame() )
    {
        setReady();
    }
    else 
    {
        resetReady();
    }
    type = SOURCE_NODE;
}

void PipelineSourceNode::addSourceLink(string from)
{
    (void)from;
    IVCP_LOG_ERROR("PipelineSourceNode::addSourceLink method is not supposed to be called");
    return;
}

void PipelineSourceNode::removeSourceLink(string from)
{
    (void)from;
    IVCP_LOG_ERROR("PipelineSourceNode::removeSourceLink method is not supposed to be called");
    return;
}
        
void PipelineSourceNode::run()
{
    Mat frame;
    do
    {
        reportStart();
        frame = source->getNextFrame();
        reportStop();
        if (!frame.empty())
        {
            if (!source->getAutoExposureStatus())
            {
                for_each(destinations.begin(), destinations.end(), [&](SPtr<IPipelineNode > to)
                {
                    to->transmitMessage(frame, getId());
                });
            } 

            for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
            {
                (*callback->first)( frame, callback->second );
            }
        }
    } while (source->haveNextFrame() && source->getAutoExposureStatus());

    activePipeline->setCurrentFrame(source->getPlaylist()->getCurrentFrame());

    if ( !frame.empty() && source->haveNextFrame() ) 
    {
        setReady();
    }
    else 
    {
        resetReady();
    }
}

void PipelineSourceNode::transmitMessage(Mat message, string caller)
{
    (void) message;
    (void) caller;

    return;
}
    
void PipelineSourceNode::init()
{
    if (NULL == source)
    {
        IVCP_EXCEPTION(IncorrectParamException, "No Video Player is set in the Pipeline");
    }

    VideoPlaylistRange firstRange = source->getPlaylist()->getRangeByIndex(0);
    source->seek(VideoPlaylistFrame(firstRange.sourceIndex, firstRange.sourceRange.firstFrame));

    if ( source->haveNextFrame() )
    {
        setReady();
    }
    else 
    {
        resetReady();
    }
}

};