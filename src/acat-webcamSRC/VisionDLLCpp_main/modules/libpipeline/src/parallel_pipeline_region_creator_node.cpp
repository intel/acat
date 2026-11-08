///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_region_creator_node.cpp" company="Intel Corporation">
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


#include "libpipeline/parallel_pipeline_region_creator_node.hpp"
#include "libivcp/region_creator/dlib_region_creator.hpp"
#include <algorithm>

using namespace std;
using namespace cv;

namespace ivcp
{
#if defined USE_DLIB
int ParallelPipelineRegionCreatorNode::countParallelRegionNodes = 0;
#endif

ParallelPipelineRegionCreatorNode::ParallelPipelineRegionCreatorNode(SPtr<IRegionCreator> _region, size_t period): 
    PipelineRegionCreatorNode(_region), ParallelPipelineNode(period)
{
#if defined USE_DLIB
    parallelRegionNodeNumber = countParallelRegionNodes++;
#endif
}

void ParallelPipelineRegionCreatorNode::pipelineRegionCreatorNodeWorker(ParallelPipelineRegionCreatorNode* context)
{
    while (true)
    {
        Mat sourcedata;
        VideoPlaylistFrame currentFrame;
        {
            unique_lock<mutex> lk(context->m);
            context->cv.wait(lk, [&] { return context->isConsumerReady || context->isConsumerFinalized;});
            sourcedata = context->sourcedata.clone();
            currentFrame = context->currentFrame;
            context->isConsumerReady = false;
        }
        
        if (context->isConsumerFinalized)
            break;

        context->reportStart();
        IRegionCreator::RegionDescription region = context->region->getRegion(sourcedata, currentFrame);
        context->reportStop();
        {
            lock_guard<mutex> lk(context->m);
            context->isConsumerReady = false;
            context->shared_region = region;
            context->isWorkerReady = true;
        }
        context->cv.notify_one();

#ifdef USE_DLIB
        // workaround for Dlib searching face
        if (context->region->getType() == DLIB_REGION && context->parallelRegionNodeNumber == 0)
        {
            SPtr<DlibRegionCreator> dlibRegion = context->region;
            if (dlibRegion->getDlibHandler()->isFaceSearching())
            {
                context->resetReady();
                {
                    unique_lock<mutex> lk(context->m);
                    dlibRegion->getDlibHandler()->rotateImageWA(context->sourcedata.clone());
                }
                context->setReady();
            }
        }
#endif
    }
}

        
void ParallelPipelineRegionCreatorNode::run()
{
    resetReady();

    IRegionCreator::RegionDescription detectedRegion;

    if (prevResultLifeTime == 0)
    {
        {
            unique_lock<mutex> lk(m);
            if (!isWorkerReady)
            {
                cv.wait(lk, [&] { return isWorkerReady; } );
            }
            detectedRegion = shared_region.clone();
            isWorkerReady = false;
        }
        prev_region = detectedRegion;
    }
    else
    {
        detectedRegion = prev_region;
    }
    prevResultLifeTime--;

    if(detectedRegion.regionFrame == Rect(0, 0, 0, 0))
    {
        return;
    }

    Mat regionImage;
    if(!detectedRegion.regionRotation.empty())
    { 
        Mat transformedImage(detectedRegion.regionFrame.height, detectedRegion.regionFrame.width, CV_8UC3);
        Mat transformation;
        invert(detectedRegion.regionRotation, transformation);
        transformation.at<double>(0, 2) += transformation.at<double>(0, 0) * detectedRegion.regionFrame.x + transformation.at<double>(0, 1) * detectedRegion.regionFrame.y;
        transformation.at<double>(1, 2) += transformation.at<double>(1, 0) * detectedRegion.regionFrame.x + transformation.at<double>(1, 1) * detectedRegion.regionFrame.y;
        transformation.at<double>(2, 2) += transformation.at<double>(2, 0) * detectedRegion.regionFrame.x + transformation.at<double>(2, 1) * detectedRegion.regionFrame.y;
        warpPerspective(sourcedata, transformedImage, transformation, transformedImage.size(), WARP_INVERSE_MAP);

        regionImage = transformedImage;
    }
    else
    {
        regionImage = sourcedata(detectedRegion.regionFrame);
    }

    for_each(destinations.begin(), destinations.end(), [&](SPtr<IPipelineNode > to)
    {
        to->transmitMessage(regionImage, getId());
    });

    if(callbacks.size())
    {
        // rect should be put to matrix in accordance with: first row x, y, width, height
        // first three columns of last three rows transformation matrix
        Mat res;
        if(detectedRegion.regionRotation.empty())
        {
            res = Mat(1, 4, CV_64FC1);
        }
        else
        {
            res = Mat(4, 4, CV_64FC1);
            detectedRegion.regionRotation.convertTo(res.rowRange(1, 4).colRange(0, 3), CV_64FC1);
        }
        res.at<double>(0, 0) = detectedRegion.regionFrame.x;
        res.at<double>(0, 1) = detectedRegion.regionFrame.y;
        res.at<double>(0, 2) = detectedRegion.regionFrame.width;
        res.at<double>(0, 3) = detectedRegion.regionFrame.height;
        for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
        {
            (*callback->first)( res, callback->second);
        }
    }
}

void ParallelPipelineRegionCreatorNode::init()
{
    worker = new thread(pipelineRegionCreatorNodeWorker, this);
    PipelineRegionCreatorNode::init();
    prevResultLifeTime = -1;
}

 void ParallelPipelineRegionCreatorNode::setPriority(IPipeline::ProcessingPriority prior)
{
    setWorkerPriority(prior);
}

void ParallelPipelineRegionCreatorNode::finalize()
{
    {
        lock_guard<mutex> lk(m);
        isConsumerFinalized = true;
    }
    cv.notify_one();
    worker->join();
}

void ParallelPipelineRegionCreatorNode::transmitMessage(Mat message, string caller)
{
    {
        lock_guard<mutex> lk(m);
        PipelineRegionCreatorNode::transmitMessage(message, caller);
        currentFrame = activePipeline->getCurrentFrame();
        if (isReady() && (prevResultLifeTime < 0))
        {
            isConsumerReady = true;
            prevResultLifeTime = (int) framePeriod - 1;
            cv.notify_one();
        }
    }
}

};
