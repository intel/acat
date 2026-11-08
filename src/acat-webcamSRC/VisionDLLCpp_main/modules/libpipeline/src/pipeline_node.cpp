///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_node.cpp" company="Intel Corporation">
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


#include "libpipeline/pipeline_node.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
PipelineNode::PipelineNode(SPtr<IFilter> filter):
    filter(filter), performanceCounter(NULL)
{
}

PipelineNode::~PipelineNode()
{
}

string PipelineNode::getId() const
{
    return filter->getId();
}

NodeType PipelineNode::getType()
{
    return type;
}

bool PipelineNode::isReady()
{
    return ready;
}

list<SPtr<IPipelineNode> > PipelineNode::getDestinations()
{
    return destinations;
}

void PipelineNode::addDestinationLink(SPtr<IPipelineNode> to)
{
    destinations.push_back(to);
}
    
void PipelineNode::removeDestinationLink(SPtr<IPipelineNode> to)
{
    auto current = destinations.begin();
    for ( ; current != destinations.end(); current++ )
    {
        if ( to == *current )
        {
            destinations.erase(current);
            return;
        }
    }
}
    
void PipelineNode::addSourceLink(string from)
{
    sources.push_back(from);
}

void PipelineNode::removeSourceLink(string from)
{
    auto current = sources.begin();
    for( ; current != sources.end(); current++ )
    {
        if ( from == *current )
        {
            sources.erase(current);
            return;
        }
    }
}

void PipelineNode::setPerformance(SPtr<IPerformance> performance)
{
    performanceCounter = performance;
}

void PipelineNode::setPipeline(IPipeline* pipeline)
{
    activePipeline = pipeline;
}

void PipelineNode::setReady()
{
    ready = true;
}

void PipelineNode::resetReady()
{
    ready = false;
}

void PipelineNode::reportStart()
{
    if (performanceCounter)
    {
        performanceCounter->startTimer( getId() );
    }
}

void PipelineNode::reportStop()
{
    if (performanceCounter)
    {
        performanceCounter->stopTimer( getId() );
    }
}

void PipelineNode::addCallback(PipelineCallback func, void* context)
{
    pair<PipelineCallback, void *> newCallback(func, context);
    callbacks.insert(newCallback);
}

void PipelineNode::setPriority(IPipeline::ProcessingPriority prior)
{
    (void) prior;
    IVCP_EXCEPTION(NotImplementedException, "Method could be used only for parallel pipeline nodes.");
}
}
