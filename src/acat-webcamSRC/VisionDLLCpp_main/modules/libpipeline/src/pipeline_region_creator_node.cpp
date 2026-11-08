///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_region_creator_node.cpp" company="Intel Corporation">
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


#include "libpipeline/pipeline_region_creator_node.hpp"
#include "libivcp/region_creator/dlib_region_creator.hpp"
#include <algorithm>

using namespace std;
using namespace cv;

namespace ivcp
{
#if defined USE_DLIB
int PipelineRegionCreatorNode::countRegionNodes = 0;
#endif

PipelineRegionCreatorNode::PipelineRegionCreatorNode(SPtr<IRegionCreator> _region): 
    PipelineNode(_region),region(_region)
{
    resetReady();
    type = REGION_NODE;
#if defined USE_DLIB
	//-----------------------------
	//dirty hack for ACAT 2.0 SS 2023/12/20
	/* In ACAT 2.0 we are allowing user to switch cameras. 
	This requires the acatvision DLL to exit and restart with new camera
	However, the DLL is not exiting cleanly. The static variables remain in memory
	and are not reinitialized. This results the regionNodeNUmbers to not start from 0
	and prevents the workarounds for finding face to not work.
	For ACAT 2.0 we are utililing 6 regions. 
	So the hack is to reset the countRegionNodes if we have already reached the count of 6 (5 since it start from 0)
	*/
	if (countRegionNodes == 5) countRegionNodes = 0;
    //-----------------------------
    regionNodeNumber = countRegionNodes++;
#endif
}

void PipelineRegionCreatorNode::addSourceLink(std::string from)
{
    if( sources.empty() ) 
    {
        PipelineNode::addSourceLink(from);
    }
}

void PipelineRegionCreatorNode::removeSourceLink(std::string from)
{
    PipelineNode::removeSourceLink(from);
}
        
void PipelineRegionCreatorNode::run()
{
    resetReady();
    reportStart();
    IRegionCreator::RegionDescription detectedRegion = region->getRegion(sourcedata, activePipeline->getCurrentFrame());

#ifdef USE_DLIB
    // workaround for Dlib searching face
    if (region->getType() == DLIB_REGION && regionNodeNumber == 0)
    {
        SPtr<DlibRegionCreator> dlibRegion = region;
        if (dlibRegion->getDlibHandler()->isFaceSearching())
        {
            dlibRegion->getDlibHandler()->rotateImageWA(sourcedata.clone());
        }
    }
#endif
    reportStop();

    if(detectedRegion.regionFrame == Rect(0, 0, 0, 0))
    {
        return;
    }

    Rect regionFrame = detectedRegion.regionFrame & Rect(0,0,sourcedata.cols,sourcedata.rows);

    Mat regionImage;
    if(!detectedRegion.regionRotation.empty())
    { 
        Mat transformedImage(regionFrame.height, regionFrame.width, CV_8UC3);
        Mat transformation;
        invert(detectedRegion.regionRotation, transformation);
        transformation.at<double>(0, 2) += transformation.at<double>(0, 0) * regionFrame.x + transformation.at<double>(0, 1) * regionFrame.y;
        transformation.at<double>(1, 2) += transformation.at<double>(1, 0) * regionFrame.x + transformation.at<double>(1, 1) * regionFrame.y;
        transformation.at<double>(2, 2) += transformation.at<double>(2, 0) * regionFrame.x + transformation.at<double>(2, 1) * regionFrame.y;
        warpPerspective(sourcedata, transformedImage, transformation, transformedImage.size(), WARP_INVERSE_MAP);

        regionImage = transformedImage;
    }
    else
    {
        regionImage = sourcedata(regionFrame);
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
        res.at<double>(0, 0) = regionFrame.x;
        res.at<double>(0, 1) = regionFrame.y;
        res.at<double>(0, 2) = regionFrame.width;
        res.at<double>(0, 3) = regionFrame.height;
        for (auto callback = callbacks.begin(); callback != callbacks.end(); callback++)
        {
            (*callback->first)( res, callback->second);
        }
    }
}

void PipelineRegionCreatorNode::transmitMessage(Mat message, string caller)
{
    if( caller == *(sources.begin()) )
    {
        setReady();
        sourcedata = message;
    }
}

};