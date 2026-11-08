////////////////////////////////////////////////////////////////////////////
// <copyright file="bbox_region_creator_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/bbox_region_creator_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
BBoxRegionCreatorXMLSerializer::BBoxRegionCreatorXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

BBoxRegionCreatorXMLSerializer::~BBoxRegionCreatorXMLSerializer()
{
}

SPtr<Filter> BBoxRegionCreatorXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    auto filterMap = loadMap(fs, fn);

    int x = filterMap["x"].ival;
    int y = filterMap["y"].ival;
    int width = filterMap["width"].ival;
    int height = filterMap["height"].ival;

    SPtr<BBoxRegionCreator> regionCreator = filter;
    if (regionCreator == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to BBoxRegionCreator");
        }
        regionCreator = Factory::createBBoxRegionCreator(Rect(x, y, width, height));
        filter = regionCreator;
    }
    else
    {
        regionCreator->setBBox(x, y, width, height);
    }

    FilterXMLSerializer::load(fs, fn, regionCreator);

    return regionCreator;
}

map<string, IXMLSerializer::SerializeData> BBoxRegionCreatorXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    int x = 0, y = 0, width = 0, height = 0;
    if (!fn["x"].empty())
    {
        fn["x"] >> x;
    }
    if (!fn["y"].empty())
    {
        fn["y"] >> y;
    }
    if (!fn["width"].empty())
    {
        fn["width"] >> width;
    }
    if (!fn["height"].empty())
    {
        fn["height"] >> height;
    }

    filterMap["x"] = x;
    filterMap["y"] = y;
    filterMap["width"] = width;
    filterMap["height"] = height;

    return filterMap;
}

map<string, IXMLSerializer::SerializeData> BBoxRegionCreatorXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<BBoxRegionCreator> regionCreator = filter;
    if (regionCreator == NULL)
    {
        IVCP_LOG(LOG_WARNING, "regionCreator is null");
        return filterMap;
    }

    Rect rect = regionCreator->getBBox();
    filterMap["x"] = rect.x;
    filterMap["y"] = rect.y;
    filterMap["width"] = rect.width;
    filterMap["height"] = rect.height;

    return filterMap;
}

}
