///////////////////////////////////////////////////////////////////////////
// <copyright file="filter_xml_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/filter_xml_serializer.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
FilterXMLSerializer::FilterXMLSerializer(SPtr<IRepository> _rep): 
    XMLSerializer(_rep)
{
}

FilterXMLSerializer::~FilterXMLSerializer()
{
}

SPtr<Filter> FilterXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    (void)fs;

    if (filter == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "filter is NULL pointer");
    }

    string id = "";
    if (!fn["id"].empty())
    {
        float performanceRate = 0;
        fn["id"] >> id;    
        fn["performance_rate"] >> performanceRate;
        filter->setId(id);
        filter->setPerformanceRate(performanceRate);
    }
    else
    {
        IVCP_EXCEPTION(IOException, "Unable to read tag \"id\" or \"performance_rate\"");
    }
    
    return filter;
}

void FilterXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);
}

map<string, IXMLSerializer::SerializeData> FilterXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    (void)fs;

    map<string, IXMLSerializer::SerializeData> filterMap;
    string id = "";
    if (!fn["id"].empty())
    {
        int performanceRate = 0;
        fn["id"] >> id;    
        fn["performance_rate"] >> performanceRate;
        filterMap["id"] = id;
        filterMap["performance_rate"] = performanceRate;
    }
    else
    {
        IVCP_EXCEPTION(IOException, "Unable to read tag \"id\"");
    }

    return filterMap;
}

map<string, IXMLSerializer::SerializeData> FilterXMLSerializer::createMap(SPtr<Filter> filter)
{
    map<string, IXMLSerializer::SerializeData> filterMap;
    string id = filter->getId();
    float performanceRate = filter->getPerformanceRate();
    if ("" != id)
    {
        filterMap["id"] = id;
        filterMap["performance_rate"] = performanceRate;
    }
    else
    {
        IVCP_EXCEPTION(NoDataException, "Filter has no id, unable to save to the Repository");
    }

    return filterMap;
}

}
