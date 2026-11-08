///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_node.cpp" company="Intel Corporation">
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

#include "libpipeline/parallel_pipeline_node.hpp"
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#include <windows.h>
#else
#include <pthread.h>
#endif

using namespace std;

namespace ivcp
{

void ParallelPipelineNode::setWorkerPriority(IPipeline::ProcessingPriority prior)
{
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    switch (prior)
    {
    case IPipeline::HIGH:
        SetThreadPriority(worker->native_handle(), THREAD_PRIORITY_ABOVE_NORMAL);
        break;
    case IPipeline::NORMAL:
        SetThreadPriority(worker->native_handle(), THREAD_PRIORITY_NORMAL);
        break;
    case IPipeline::LOW:
        SetThreadPriority(worker->native_handle(), THREAD_PRIORITY_BELOW_NORMAL);
        break;
    default:
        break;
    }
#else
    pthread_attr_t thAttr;
    int policy = 0;
    int max_prio_for_policy = 0, min_prio_for_policy = 0;
    pthread_attr_init(&thAttr);
    pthread_attr_getschedpolicy(&thAttr, &policy);
    max_prio_for_policy = sched_get_priority_max(policy);
    min_prio_for_policy= sched_get_priority_min(policy);
    switch (prior)
    {
    case IPipeline::HIGH:
        pthread_setschedprio(worker->native_handle(), max_prio_for_policy);
        break;
    case IPipeline::LOW:
        pthread_setschedprio(worker->native_handle(), min_prio_for_policy);
        break;
    default:
        break;
    }
    pthread_attr_destroy(&thAttr);
#endif
}

};
