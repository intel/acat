///////////////////////////////////////////////////////////////////////////
// <copyright file="dlib_region_creator.cpp" company="Intel Corporation">
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

#include "libivcp/region_creator/dlib_region_creator.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
DlibRegionCreator::DlibRegionCreator(SPtr<IDlib> dlibHandler, DlibRegionCreator::Settings settings)
{
    setDlibHandler(dlibHandler);
    setSearchRegionSettings(settings);
}

void DlibRegionCreator::setDlibHandler(SPtr<IDlib> dlibHandler)
{
    usedDlibHandler = dlibHandler;
}

SPtr<IDlib> DlibRegionCreator::getDlibHandler()
{
    return usedDlibHandler;
}

void DlibRegionCreator::setSearchRegionSettings(DlibRegionCreator::Settings _settings)
{
    lock_guard<recursive_mutex> lk(settingsMutex);
    settings = _settings;
}

DlibRegionCreator::Settings DlibRegionCreator::getSearchRegionSettings()
{
    lock_guard<recursive_mutex> lk(settingsMutex);
    return settings;
}

DlibRegionCreator::~DlibRegionCreator()
{
}

IRegionCreator::RegionDescription DlibRegionCreator::processRegion(Mat image, VideoPlaylistFrame currentFrame)
{
    lock_guard<recursive_mutex> lk(settingsMutex);
    if(usedDlibHandler == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Dlib was not initialised. Dlib handler is NULL" );
    }

    try
    {
        Rect boundAPI = usedDlibHandler->getRegion(image, currentFrame, settings.typeToSearch, settings.normalize);
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
            return IRegionCreator::RegionDescription(Rect(0,0,0,0), usedDlibHandler->getTransform());
        }

        //Apply region offset
		//printf("offsetY=%d\n", settings.regionOffsetY);
        bound.x += settings.regionOffsetX;
        bound.y += settings.regionOffsetY;

        if (settings.normalize)
        {
            return IRegionCreator::RegionDescription(bound, usedDlibHandler->getTransform());
        }
        return IRegionCreator::RegionDescription(bound, Mat());
    }
    catch (IOException &exception)
    {
        IVCP_LOG_INFO(exception.message);
        IVCP_EXCEPTION(IncorrectParamException, "Provided Dlib handler is invalid" );
    }
}

IRegionCreator::RegionDescription DlibRegionCreator::getRegion(cv::Mat image)
{
    if(activePlaylist == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Dlib region creator does not have a playlist" );
    }

    return processRegion(image, getPlaylist()->getCurrentFrame());
}

IRegionCreator::RegionDescription DlibRegionCreator::getRegion(cv::Mat image, ivcp::VideoPlaylistFrame frame)
{
    return processRegion(image, frame);
}

RegionCreatorType DlibRegionCreator::getType()
{
    return DLIB_REGION;
}

}
