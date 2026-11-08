///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_classifier_node.cpp" company="Intel Corporation">
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


#include "libpipeline/parallel_pipeline_classifier_node.hpp"
#include <algorithm>

using namespace std;
using namespace cv;

namespace ivcp
{
ParallelPipelineClassifierNode::ParallelPipelineClassifierNode(SPtr<IClassifier> _classifier, PipelineMode _mode, size_t period):
    PipelineClassifierNode(_classifier, _mode), ParallelPipelineNode(period)
{ }

void ParallelPipelineClassifierNode::pipelineClassifierNodeWorker(ParallelPipelineClassifierNode* context)
{
    while (true)
    {
        Mat feature;
        {
            unique_lock<mutex> lk(context->m);
            context->cv.wait(lk, [&] { return context->isConsumerReady || context->isConsumerFinalized;});
            feature = context->feature.clone();
            context->isConsumerReady = false;
        }

        if (context->isConsumerFinalized)
            break;

        ClassType type;
        vector<float> prob;

        try
        {
            context->reportStart();
            type = context->classifier->clasify(feature);
            prob = context->classifier->getDistribution(feature);
            context->reportStop();
        }
        catch (Exception &e)
        {
            (void) e;
            context->reportStop();
            lock_guard<mutex> lk(context->m);
            context->isWorkerReady = true;
            context->workerException = current_exception();
            context->cv.notify_one();
            return;
        }
 
        {
            lock_guard<mutex> lk(context->m);
            context->sharedClassType = type;
            context->sharedProb = prob;
            context->isWorkerReady = true;
        }
        context->cv.notify_one();
    }
}
        
void ParallelPipelineClassifierNode::run()
{
    ClassType classifiedType = NOT_CLASSIFIED;
    vector<float> prob;
    resetReady();
    if (prevResultLifeTime == 0)
    {
        {
            unique_lock<mutex> lk(m);
            if (!isWorkerReady)
            {
                cv.wait(lk, [&] { return isWorkerReady; } );
            }

            if (workerException != nullptr)
            {
                rethrow_exception(workerException);
            }

            classifiedType = sharedClassType;
            prob = sharedProb;
            isWorkerReady = false;
        }

        prevClassType = classifiedType;
        prevProb = prob;
    }
    else
    {
        classifiedType = prevClassType;
        prob = prevProb;
    }
    prevResultLifeTime--;

    if ((TEST_PIPELINE == mode) && performanceCounter)
    {
        performanceCounter->addClassifyResult(activePipeline->getCurrentFrame(), classifiedType);
    }

    if(callbacks.size())
    {
        Mat res(1, 1 + (int)prob.size(), CV_32FC1);
        res.at<float>(0, 0) = (float)classifiedType;
        for (unsigned int i = 0; i < prob.size(); i++)
        {
            res.at<float>(0, i+1) = prob[i];
        }

        for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
        {
            (*callback->first)( res, callback->second);
        }
    }

    if (csv_output.is_open())
    {
        csv_output << cv::format(feature, cv::Formatter::FMT_CSV) << csv_delimeter << ' ' << classifiedType << endl;
    }
}

void ParallelPipelineClassifierNode::transmitMessage(Mat message, string caller)
{
    lock_guard<mutex> lk(m);
    PipelineClassifierNode::transmitMessage(message, caller);
    if (isReady() && (prevResultLifeTime < 0))
    {
        prevResultLifeTime = (int) framePeriod - 1;
        isConsumerReady = true;
        cv.notify_one();
    }
}

void ParallelPipelineClassifierNode::init()
{
    worker = new thread(pipelineClassifierNodeWorker, this);
    prevResultLifeTime = -1;
    prevClassType = 0;
    prevProb.resize(classifier->getClassCount(), 0.0);
}

void ParallelPipelineClassifierNode::setPriority(IPipeline::ProcessingPriority prior)
{
    setWorkerPriority(prior);
}

void ParallelPipelineClassifierNode::finalize()
{
    {
        lock_guard<mutex> lk(m);
        isConsumerFinalized = true;
    }
    cv.notify_one();
    worker->join();
}
    
}
