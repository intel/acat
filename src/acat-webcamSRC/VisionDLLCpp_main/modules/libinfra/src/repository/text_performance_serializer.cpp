///////////////////////////////////////////////////////////////////////////
// <copyright file="text_performance_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/text_performance_serializer.hpp"
#include "libivcp/factory.hpp"
#include "libivcp/performance/textfile_performance.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
TextPerformanceXMLSerializer::TextPerformanceXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{

}

TextPerformanceXMLSerializer::~TextPerformanceXMLSerializer()
{
}

SPtr<Filter> TextPerformanceXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<TextPerformance> performance = filter;
    if (performance == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to TextPerformance for serialization");
        }
        performance = Factory::createPerformance();
        filter = performance;
    }

    auto perfMap = loadMap(fs, fn);
    string markupId = perfMap["class_markup_id"].str;
    if (markupId != "")
    {
        SPtr<IClassifierResultDataSource> markup = rep->loadClassifierResultDataSource(markupId);
        if (markup != NULL)
        {
            performance->setMarkup(markup);
        }
    }

    Mat keyMat = perfMap["class_types"].mat;
    vector<string> names = perfMap["class_names"].strVec;
    for (int i = 0; i < keyMat.rows; i++)
    {
        performance->setClassName(keyMat.at<int>(i, 0), names[i]);
    }

    FilterXMLSerializer::load(fs, fn, performance);
    return performance;
}

map<string, IXMLSerializer::SerializeData> TextPerformanceXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    string markupId = "";
    if (!fn["class_markup_id"].empty())
    {
        fn["class_markup_id"] >> markupId;
    }
    filterMap["class_markup_id"] = markupId;

    vector<string> values;
    loadStrVec(values, "class_names", "name", fs, fn);

    Mat keyMat;
    if (!fn["class_types"].empty())
    {
        fn["class_types"] >> keyMat;
    }
    filterMap["class_types"] = keyMat;
    filterMap["class_names"] = values;

    return filterMap;
}

void TextPerformanceXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<TextPerformance> textPerformance = filter;
    if (textPerformance == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to TextPerformance for serialization");
    }

    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);

    auto markup = textPerformance->getClassMarkup();
    if (markup != NULL)
    {
        rep->save(markup);
    }
}

void TextPerformanceXMLSerializer::saveMap(map<string, IXMLSerializer::SerializeData> &filterData, FileStorage &fs)
{
    saveStrVec(filterData["class_names"].strVec, "class_names", "name", fs);

    map<string, SerializeData> filterDataCopy = filterData;
    filterDataCopy.erase("class_names");

    FilterXMLSerializer::saveMap(filterDataCopy, fs);
}

map<string, IXMLSerializer::SerializeData> TextPerformanceXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<TextPerformance> perf = filter;
    if (perf == NULL)
    {
        return filterMap;
    }

    string classMarkupId = (NULL != perf->getClassMarkup())?
        (perf->getClassMarkup()->getId()):("");
    
    map<ClassType, string> &nameMap = perf->getClassNamesMap();

    Mat classTypes((int)nameMap.size(), 1, CV_32SC1);
    vector<string> classNames(nameMap.size());
    int counter = 0;
    for (auto it = nameMap.begin(); it != nameMap.end(); it++, counter++)
    {
        classTypes.at<int>(counter, 0) = it->first;
        classNames[counter] = it->second;
    }

    filterMap["class_markup_id"] = classMarkupId;
    if (!classNames.empty())
    {
        filterMap["class_types"] = classTypes;
        filterMap["class_names"] = classNames;
    }

    return filterMap;
}

}
