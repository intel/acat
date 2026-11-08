///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_source_node.cpp" company="Intel Corporation">
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


#include "libpipeline/parallel_pipeline_source_node.hpp"
#include <algorithm>

using namespace std;
using namespace cv;

namespace ivcp
{
ParallelPipelineSourceNode::ParallelPipelineSourceNode(SPtr<IVideoPlayer> _source, size_t period):
    PipelineSourceNode(_source), ParallelPipelineNode(period)
{
    frameCount = 0;
}

void ParallelPipelineSourceNode::pipelineSourceNodeWorker(ParallelPipelineSourceNode* context)
{
    while (true)
    {
        if (!context->isConsumerReady)
        {
            unique_lock<mutex> lk(context->m);
            context->cv.wait(lk, [&] { return context->isConsumerReady || context->isConsumerFinalized;});
        }
        
        if (context->isConsumerFinalized)
            return;

        context->reportStart();
        // clone image is needed because videosource used a shared memory
        Mat frame = context->source->getNextFrame().clone();
        context->reportStop();

        {
            lock_guard<mutex> lk(context->m);
            context->isConsumerReady = false;
            context->shared_frame = frame;
            context->isWorkerReady = true;
        }
        context->cv.notify_one();
    }
}
        
void ParallelPipelineSourceNode::run()
{
    Mat frame;

    {
        unique_lock<mutex> lk(m);
        if (!isWorkerReady)
        {
            cv.wait(lk, [&] { return isWorkerReady; } );
        }
        frame = shared_frame.clone();
        isWorkerReady = false;
        activePipeline->setCurrentFrame(source->getPlaylist()->getCurrentFrame());
            
        if ( !frame.empty() && source->haveNextFrame() ) 
        {
            setReady();
            isConsumerReady = true;
        }
        else 
        {
            resetReady();
        }
    }
    cv.notify_one();

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

    frameCount++;
}
    
void ParallelPipelineSourceNode::init()
{
    worker = new thread(pipelineSourceNodeWorker, this);
    PipelineSourceNode::init();
    {
        lock_guard<mutex> lk(m);
        isConsumerReady = true;
    }
    cv.notify_one();
}

void ParallelPipelineSourceNode::setPriority(IPipeline::ProcessingPriority prior)
{
    setWorkerPriority(prior);
}

void ParallelPipelineSourceNode::finalize()
{
    {
        lock_guard<mutex> lk(m);
        isConsumerFinalized = true;
    }
    cv.notify_one();
    worker->join();
}

};
