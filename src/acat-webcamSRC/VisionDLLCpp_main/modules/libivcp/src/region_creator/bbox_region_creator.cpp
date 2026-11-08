///////////////////////////////////////////////////////////////////////////
// <copyright file="bbox_region_creator.cpp" company="Intel Corporation">
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


#include "libivcp/region_creator/bbox_region_creator.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
BBoxRegionCreator::BBoxRegionCreator(cv::Rect bbox)
{
    setBBox(bbox);
}

BBoxRegionCreator::BBoxRegionCreator(int x, int y, int width, int height)
{
    setBBox(x, y, width, height);
}

void BBoxRegionCreator::setBBox(cv::Rect bbox)
{
    bBox = bbox;
}

void BBoxRegionCreator::setBBox(int x, int y, int width, int height)
{
    bBox.x = x;
    bBox.y = y;
    bBox.width = width;
    bBox.height = height;
}

IRegionCreator::RegionDescription BBoxRegionCreator::getRegion(cv::Mat image)
{
    Rect bound;
    bound.x = max( bBox.x, 0 );
    bound.y = max( bBox.y, 0 );
    bound.width = max( min(bBox.width - (bound.x - bBox.x), image.cols - bound.x), 0 );
    bound.height = max( min(bBox.height - (bound.y - bBox.y), image.rows - bound.y), 0 );
    if(bound.width <= 0 || bound.height <= 0)
    {
        return Rect(0,0,0,0);
    }

    return bound;
}

IRegionCreator::RegionDescription BBoxRegionCreator::getRegion(cv::Mat image, ivcp::VideoPlaylistFrame frame)
{
    (void) frame;
    return getRegion(image);
}

RegionCreatorType BBoxRegionCreator::getType()
{
    return BBOX;
}

Rect BBoxRegionCreator::getBBox()
{
    return bBox;
}

}
