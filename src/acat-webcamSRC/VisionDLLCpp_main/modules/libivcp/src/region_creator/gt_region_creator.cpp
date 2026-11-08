///////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_creator.cpp" company="Intel Corporation">
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

#include "libivcp/region_creator/gt_region_creator.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
GTRegionCreator::GTRegionCreator()
{
    setParams(NULL, 0);
}

GTRegionCreator::GTRegionCreator(SPtr<IRegionDataSource> source, ClassType regClass)
{
    setParams(source, regClass);
}

void GTRegionCreator::setParams(SPtr<IRegionDataSource> source, ClassType regClass) 
{ 
    regionClass = regClass;
    regions = source;
}

IRegionCreator::RegionDescription GTRegionCreator::getRegion(cv::Mat image)
{
    (void)image;

    if(activePlaylist == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Ground truth region creator does not have a playlist" );
    }

    if (regions == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Ground truth region creator does not have a data source assigned" );
    }

    VideoPlaylistFrame currentFrame = activePlaylist->getCurrentFrame();
    return IRegionCreator::RegionDescription( regions->getRegion(currentFrame, regionClass), regions->getTransformation(currentFrame) );
}

IRegionCreator::RegionDescription GTRegionCreator::getRegion(cv::Mat image, ivcp::VideoPlaylistFrame frame)
{
    (void)image;

    if (regions == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Ground truth region creator does not have a data source assigned" );
    }

    return IRegionCreator::RegionDescription( regions->getRegion(frame, regionClass), regions->getTransformation(frame) );
}

RegionCreatorType GTRegionCreator::getType()
{
    return GROUNDTRUTH;
}

SPtr<IRegionDataSource> GTRegionCreator::getRegionDataSource()
{
    return regions;
}

ClassType GTRegionCreator::getRegionClass()
{
    return regionClass;
}

}
