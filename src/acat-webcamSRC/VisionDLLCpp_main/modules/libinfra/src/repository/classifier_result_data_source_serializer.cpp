////////////////////////////////////////////////////////////////////////////
// <copyright file="classifier_result_data_source_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/classifier_result_data_source_serializer.hpp"
#include "libinfra/data_source/classifier_result_data_source.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
ClassifierResultDataSourceXMLSerializer::ClassifierResultDataSourceXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

ClassifierResultDataSourceXMLSerializer::~ClassifierResultDataSourceXMLSerializer()
{
}

SPtr<Filter> ClassifierResultDataSourceXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<ClassifierResultDataSource> classifierResults = filter;
    if (classifierResults == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to ClassifierResultDataSource");
        }
        filter = classifierResults = Factory::createClassifierResultDataSource();
    }

    FilterXMLSerializer::load(fs, fn, filter);

    classifierResults->load(fs, fn);
    return classifierResults;
}

void ClassifierResultDataSourceXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<ClassifierResultDataSource> classifierResults = filter;
    if (classifierResults == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to ClassifierResultDataSource for serialization");
    }

    classifierResults->save(fs);

    FilterXMLSerializer::save(filter, fs);
}

map<string, IXMLSerializer::SerializeData> ClassifierResultDataSourceXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    map<string, IXMLSerializer::SerializeData> filterMap = FilterXMLSerializer::loadMap(fs, fn);
    filterMap["sourceranges"] = IXMLSerializer::SerializeData(ClassifierResultDataSource::loadSerializeMat(fs, fn));
    return filterMap;
}

void ClassifierResultDataSourceXMLSerializer::saveMap(std::map<std::string, SerializeData> &filterData, cv::FileStorage &fs)
{ 
    if (filterData.find("sourceranges") != filterData.end())
    {
        ClassifierResultDataSource::saveSerializeMat(filterData["sourceranges"].mat, fs);
    }

    map<string, SerializeData> classResData = filterData;
    classResData.erase("sourceranges");

    FilterXMLSerializer::saveMap(classResData, fs);
}

}