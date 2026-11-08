///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_region_creator.cpp" company="Intel Corporation">
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


#include "libivcp/region_creator/realsense_region_creator.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
RealSenseRegionCreator::RealSenseRegionCreator(SPtr<IRealSense> rsHandler, RealSenseRegionCreator::Settings settings)
{
    setRealSenseHandler(rsHandler);
    setSearchRegionSettings(settings);
}

void RealSenseRegionCreator::setRealSenseHandler(SPtr<IRealSense> rsHandler)
{
    usedRsHandler = rsHandler;
}

SPtr<IRealSense> RealSenseRegionCreator::getRealSenseHandler()
{
    return usedRsHandler;
}

void RealSenseRegionCreator::setSearchRegionSettings(RealSenseRegionCreator::Settings _settings)
{
    lock_guard<recursive_mutex> lk(settingsMutex);
    settings = _settings;
}

RealSenseRegionCreator::Settings RealSenseRegionCreator::getSearchRegionSettings()
{
    lock_guard<recursive_mutex> lk(settingsMutex);
    return settings;
}

RealSenseRegionCreator::~RealSenseRegionCreator()
{
}

IRegionCreator::RegionDescription RealSenseRegionCreator::processRegion(Mat image, VideoPlaylistFrame currentFrame)
{
    lock_guard<recursive_mutex> lk(settingsMutex);
    if(usedRsHandler == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "RealSense was not initialised. RealSense handler is NULL" );
    }

    try
    {
        Rect boundAPI = usedRsHandler->getRegion(image, currentFrame, settings.typeToSearch, settings.normalize);
        boundAPI.x = boundAPI.x - (int)(boundAPI.width * (settings.regionExpansionWScale - 1) / 2);
        boundAPI.y = boundAPI.y - (int)(boundAPI.height * (settings.regionExpansionHScale - 1) / 2);
        boundAPI.width = (int)(boundAPI.width * settings.regionExpansionWScale);
        boundAPI.height = (int)(boundAPI.height * settings.regionExpansionHScale);

        Rect bound;
        bound.x = max(boundAPI.x, 0);
        bound.y = max(boundAPI.y, 0);
        bound.width = max( min(boundAPI.width - (bound.x - boundAPI.x), image.cols - bound.x), 0);
        bound.height = max( min(boundAPI.height - (bound.y - boundAPI.y), image.rows - bound.y), 0);
        if(bound.width <= 0 || bound.height <= 0)
        {
            return IRegionCreator::RegionDescription(Rect(0,0,0,0), usedRsHandler->getTransform());
        }

        //Apply region offset
        bound.x += settings.regionOffsetX;
        bound.y += settings.regionOffsetY;

        if (settings.normalize)
        {
            return IRegionCreator::RegionDescription(bound, usedRsHandler->getTransform());
        }
        return IRegionCreator::RegionDescription(bound, Mat());
    }
    catch (IOException &exception)
    {
        IVCP_LOG_INFO(exception.message);
        IVCP_EXCEPTION(IncorrectParamException, "Provided RealSense handler is invalid" );
    }
}

IRegionCreator::RegionDescription RealSenseRegionCreator::getRegion(cv::Mat image)
{
    if(activePlaylist == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "RealSense region creator does not have a playlist" );
    }

    return processRegion(image, getPlaylist()->getCurrentFrame());
}

IRegionCreator::RegionDescription RealSenseRegionCreator::getRegion(cv::Mat image, ivcp::VideoPlaylistFrame frame)
{
    return processRegion(image, frame);
}

RegionCreatorType RealSenseRegionCreator::getType()
{
    return RS_REGION;
}

}
