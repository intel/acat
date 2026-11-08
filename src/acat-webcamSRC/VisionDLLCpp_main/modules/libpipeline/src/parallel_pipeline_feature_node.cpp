///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_feature_node.cpp" company="Intel Corporation">
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


#include <algorithm>
#include "libpipeline/parallel_pipeline_feature_node.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
ParallelPipelineFeatureNode::ParallelPipelineFeatureNode(SPtr<IFeature> _feature, size_t period, IPipeline::Settings settings):
    PipelineFeatureNode(_feature, settings), ParallelPipelineNode(period)
{ }


void ParallelPipelineFeatureNode::pipelineFeatureNodeWorker(ParallelPipelineFeatureNode* context)
{
    SPtr<IFeature> feature;
    feature = context->feature;
    while (true)
    {
        list<string> sources;
        map<string, RegionSequence> sourceToRegionSequence;
        
        // Wait until consumer sends data
        {
            unique_lock<mutex> lk(context->m);
            context->cv.wait(lk, [&] { return context->isConsumerReady || context->isConsumerFinalized;});
            sources = context->sources;
            sourceToRegionSequence = context->sourceToRegionSequence;
            context->isConsumerReady = false;
        }

        if (context->isConsumerFinalized)
            break;

        Mat evaluatedFeatureValue;

        try
        {
            context->reportStart();
            //Check whether feature is capable to handle multiple region sequence
            if (feature->getNumberOfSequences() == 1)
            {
                //Evaluate feature for each input region sequence and concatenate results
                evaluatedFeatureValue.create(1, (int)sources.size() * feature->getFeatureLength(), CV_32FC1);
                int partStart = 0;            
                for (list<string>::iterator iter = sources.begin(); iter != sources.end(); ++iter)
                {
                    vector<RegionSequence> tmp;
                    tmp.resize(1);
                    tmp[0] = sourceToRegionSequence[*iter];
                    bool regNotEmpty = true;
                    for (auto regionIter = tmp[0].begin(); regionIter != tmp[0].end() && regNotEmpty; ++regionIter)
                    {
                        if (regionIter->empty())
                        {
                            regNotEmpty = false;
                        }
                    }
                    if (regNotEmpty)
                    {
                        feature->getFeature( tmp ).convertTo(evaluatedFeatureValue.colRange( partStart, partStart + feature->getFeatureLength() ), CV_32FC1);
                    }
                    else
                    {
                        evaluatedFeatureValue.colRange( partStart, partStart + feature->getFeatureLength() ) = Mat::zeros(1, feature->getFeatureLength(), CV_32FC1);
                    }
                    partStart += feature->getFeatureLength();
                }
            }
            else
            {
                //Evaluate feature once for all input region sequences simultaneously if we have enough sequences
                if (feature->getNumberOfSequences() == sources.size())
                {
                    vector<RegionSequence> tmp;
                    tmp.resize(sources.size());
                    bool regNotEmpty = true;
                    int i = 0;
                    for (list<string>::iterator iter = sources.begin(); iter != sources.end(); ++iter, ++i)
                    {
                        tmp[i] = sourceToRegionSequence[*iter];
                        for (auto regionIter = tmp[i].begin(); regionIter != tmp[i].end() && regNotEmpty; ++regionIter)
                        {
                            if (regionIter->empty())
                            {
                                regNotEmpty = false;
                            }
                        }
                    }

                    //At the moment only frames with all necessary regions detected are used
                    //ToDo Other frame storing strategies are possible
                    if (regNotEmpty)
                    {
                        evaluatedFeatureValue = feature->getFeature(tmp);
                    }
                    else
                    {
                        evaluatedFeatureValue = Mat::zeros(1, feature->getFeatureLength(), CV_32FC1);
                    }
                }
                else
                {
                    IVCP_EXCEPTION(IncorrectParamException, "Number of sequences is incorrect" );
                }
            }
            context->reportStop();
        }
        catch (IncorrectParamException &e)
        {
            (void) e;
            context->reportStop();
            lock_guard<mutex> lk(context->m);
            context->workerException = current_exception();
            context->isWorkerReady = true;
            throw;
        }

        // Send data back to consumer
        {
            lock_guard<mutex> lk(context->m);
            context->isConsumerReady = false;
            context->shared_feature = evaluatedFeatureValue;
            context->isWorkerReady = true;			
        }
        context->cv.notify_one();
    }
}
        
void ParallelPipelineFeatureNode::run()
{
    resetReady();
    Mat evaluatedFeatureValue;

    if (prevResultLifeTime == 0)
    {
        // wait for the worker
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

            evaluatedFeatureValue = shared_feature.clone();
            isWorkerReady = false;
            lastProcessedFrames.clear();
            for (unsigned int i = 0; i < feature->getNumberOfRegions(); i++)
            {
                lastProcessedFrames.push_back(frameNumbersForProcessing.front());
                frameNumbersForProcessing.pop_front();
            }
        }

        prev_feature = evaluatedFeatureValue;
    }
    else
    {
        evaluatedFeatureValue = prev_feature;
    }
    prevResultLifeTime--;

    for_each(destinations.begin(), destinations.end(), [&](SPtr<IPipelineNode> to)
    {
        to->transmitMessage(evaluatedFeatureValue, getId());
    });

    if (callbacks.size())
    {
        for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
        {
            (*callback->first)( evaluatedFeatureValue, callback->second);
        }
    }
}

void ParallelPipelineFeatureNode::transmitMessage(Mat message, string caller)
{
    // send data to worker thread
    {
        lock_guard<mutex> lk(m);
        PipelineFeatureNode::transmitMessage(message, caller);
        if (isReady() && (prevResultLifeTime < 0))
        {
            isConsumerReady = true;
            prevResultLifeTime = (int) framePeriod - 1;
            cv.notify_one();
        }
    }
}

void ParallelPipelineFeatureNode::init()
{
    worker = new thread(pipelineFeatureNodeWorker, this);
    prev_feature = Mat::zeros(1, feature->getFeatureLength() * ((int) sources.size()), CV_32FC1);
    prevResultLifeTime = -1;
}

void ParallelPipelineFeatureNode::setPriority(IPipeline::ProcessingPriority prior)
{
    setWorkerPriority(prior);
}

void ParallelPipelineFeatureNode::finalize()
{
    // notify worker thread when finalized
    {
        lock_guard<mutex> lk(m);
        isConsumerFinalized = true;
    }
    cv.notify_one();
    worker->join();
}
   
};
