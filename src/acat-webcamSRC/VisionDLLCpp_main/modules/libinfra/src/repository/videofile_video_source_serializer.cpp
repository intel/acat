///////////////////////////////////////////////////////////////////////////
// <copyright file="videofile_video_source_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/videofile_video_source_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
VideofileVideoSourceSerializer::VideofileVideoSourceSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)  
{
}

VideofileVideoSourceSerializer::~VideofileVideoSourceSerializer()
{
}

SPtr<Filter> VideofileVideoSourceSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    string path = "";
    int output_width = 0;
    int output_height = 0;
    bool multipleVideoFiles = false;
    bool is12BitSource = false;
    if (!fn["file"].empty())
    {
        fn["file"] >> path;
        fn["output_width"] >> output_width;
        fn["output_height"] >> output_height;
        fn["has_multiple_video_files"] >> multipleVideoFiles;
        fn["is_12Bit_source"] >> is12BitSource;
    }

    SPtr<VideofileVideoSource> videoSource = filter;
    if (videoSource == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to VideofileVideoSource for serialization");
        }        
        videoSource = Factory::createVideofileVideoSource(path); 
        filter = videoSource;
    }
    else
    {
        videoSource->setFilename(path);
    }

    videoSource->setOutputResolution(output_width, output_height);
    videoSource->setMultipleFiles(multipleVideoFiles);
    videoSource->set12BitSource(is12BitSource);

    FilterXMLSerializer::load(fs, fn, videoSource);
    return videoSource;
}

map<string, IXMLSerializer::SerializeData> VideofileVideoSourceSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    string filename = "";
    int output_width = 0;
    int output_height = 0;
    bool multipleVideoFiles = false;
    bool is12BitSource = false;

    if (!fn["file"].empty())
    {
        fn["file"] >> filename;
        fn["output_width"] >> output_width;
        fn["output_height"] >> output_height;
        fn["has_multiple_video_files"] >> multipleVideoFiles;
        fn["is_12Bit_source"] >> is12BitSource;
    }
    filterMap["file"] = filename;
    filterMap["output_width"] = output_width;
    filterMap["output_height"] = output_height;
    filterMap["has_multiple_video_files"] = multipleVideoFiles;
    filterMap["is_12Bit_source"] = is12BitSource;

    return filterMap;
}

map<string, IXMLSerializer::SerializeData> VideofileVideoSourceSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<VideofileVideoSource> videoSource = filter;
    if (videoSource != NULL)
    {
        filterMap["file"] = videoSource->getFilename();
        filterMap["output_width"] = videoSource->getOutputResolution().width;
        filterMap["output_height"] = videoSource->getOutputResolution().height;
        filterMap["has_multiple_video_files"] = videoSource->hasMultipleVideoFiles();
        filterMap["is_12Bit_source"] = videoSource->is12BitSource();
    }

    return filterMap;
}

}
