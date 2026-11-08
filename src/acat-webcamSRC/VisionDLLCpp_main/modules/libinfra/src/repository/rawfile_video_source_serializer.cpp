///////////////////////////////////////////////////////////////////////////
// <copyright file="raw_video_source_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/rawfile_video_source_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
RawfileVideoSourceSerializer::RawfileVideoSourceSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)  
{
}

RawfileVideoSourceSerializer::~RawfileVideoSourceSerializer()
{
}

SPtr<Filter> RawfileVideoSourceSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    string path = "";
    int output_width = 0;
    int output_height = 0;
    if (!fn["file"].empty())
    {
        fn["file"] >> path;
        fn["output_width"] >> output_width;
        fn["output_height"] >> output_height;
    }

    SPtr<RawfileVideoSource> videoSource = filter;
    if (videoSource == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to RawfileVideoSource for serialization");
        }        
        videoSource = Factory::createRawfileVideoSource(path); 
        filter = videoSource;
    }
    else
    {
        videoSource->setFilename(path);
    }

    videoSource->setOutputResolution(output_width, output_height);

    FilterXMLSerializer::load(fs, fn, videoSource);
    return videoSource;
}

map<string, IXMLSerializer::SerializeData> RawfileVideoSourceSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    string filename = "";
    int output_width = 0;
    int output_height = 0;

    if (!fn["file"].empty())
    {
        fn["file"] >> filename;
        fn["output_width"] >> output_width;
        fn["output_height"] >> output_height;
    }
    filterMap["file"] = filename;
    filterMap["output_width"] = output_width;
    filterMap["output_height"] = output_height;

    return filterMap;
}

map<string, IXMLSerializer::SerializeData> RawfileVideoSourceSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<RawfileVideoSource> videoSource = filter;
    if (videoSource != NULL)
    {
        filterMap["file"] = videoSource->getFilename();
        filterMap["output_width"] = videoSource->getOutputResolution().width;
        filterMap["output_height"] = videoSource->getOutputResolution().height;
    }

    return filterMap;
}

}
