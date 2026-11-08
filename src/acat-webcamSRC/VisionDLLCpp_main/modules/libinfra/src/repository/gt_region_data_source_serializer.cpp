///////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_data_source_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/gt_region_data_source_serializer.hpp"
#include "libinfra/data_source/gt_region_data_source.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
GTRegionDataSourceXMLSerializer::GTRegionDataSourceXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

GTRegionDataSourceXMLSerializer::~GTRegionDataSourceXMLSerializer()
{
}

SPtr<Filter> GTRegionDataSourceXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<GTRegionDataSource> gtRegions = filter;
    if (gtRegions == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to GTRegionDataSource");
        }
        gtRegions = Factory::createRegionDataSource();
        filter = gtRegions;
    }
    else
    {
        gtRegions = filter;
    }

    FilterXMLSerializer::load(fs, fn, filter);

    gtRegions->load(fs, fn);
    return gtRegions;
}

void GTRegionDataSourceXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<GTRegionDataSource> gtRegions = filter;
    if (gtRegions == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to GTRegionDataSource for serialization");
    }

    gtRegions->save(fs);

    FilterXMLSerializer::save(filter, fs);
}

map<string, IXMLSerializer::SerializeData> GTRegionDataSourceXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    map<string, IXMLSerializer::SerializeData> filterMap = FilterXMLSerializer::loadMap(fs, fn);
    Mat frames, transformations;
    GTRegionDataSource::loadSerializeMat(fs, fn, frames, transformations);
    filterMap["frames"] = IXMLSerializer::SerializeData(frames);
    filterMap["transformations"] = IXMLSerializer::SerializeData(transformations);
    return filterMap;
}

void GTRegionDataSourceXMLSerializer::saveMap(std::map<std::string, SerializeData> &filterData, cv::FileStorage &fs)
{ 
    if (filterData.find("frames") != filterData.end() && filterData.find("transformations") != filterData.end())
    {
        GTRegionDataSource::saveSerializeMat(filterData["frames"].mat, filterData["transformations"].mat, fs);
    }

    map<string, SerializeData> gtData = filterData;
    gtData.erase("frames");
    gtData.erase("transformations");

    FilterXMLSerializer::saveMap(gtData, fs);
}


}
