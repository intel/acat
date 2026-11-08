///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_video_source_serializer.cpp" company="Intel Corporation">
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

#ifdef USE_REALSENSE

#include "libinfra/repository/realsense_video_source_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
RealSenseVideoSourceXMLSerializer::RealSenseVideoSourceXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

RealSenseVideoSourceXMLSerializer::~RealSenseVideoSourceXMLSerializer()
{
}

SPtr<Filter> RealSenseVideoSourceXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    int cameraIndex = 0;
    int output_width = 0;
    int output_height = 0;
    bool is3dTracking = true;
    if (!fn["camera_index"].empty())
    {
        fn["camera_index"] >> cameraIndex;
        fn["video_width"] >> output_width;
        fn["video_height"] >> output_height;
        fn["type_tracking"] >> is3dTracking;
    }

    SPtr<RealSenseVideoSource> cameraSource = filter;
    if (cameraSource == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to RealSenseVideoSource");
        }
        cameraSource = Factory::createRealSenseVideoSource(cameraIndex);
        filter = cameraSource;
    }
    else
    {
        cameraSource->setCameraIndex(cameraIndex);
    }

    cameraSource->setOutputResolution(output_width, output_height);
    cameraSource->set3dTracking(is3dTracking);

    FilterXMLSerializer::load(fs, fn, cameraSource);
    return cameraSource;
}

map<string, IXMLSerializer::SerializeData> RealSenseVideoSourceXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    int cameraIndex = 0;
    int cameraVideoWidth = 0;
    int cameraVideoHeight = 0;
    bool is3dTracking = true;
    if (!fn["camera_index"].empty())
    {
        fn["camera_index"] >> cameraIndex;
        fn["video_width"] >> cameraVideoWidth;
        fn["video_height"] >> cameraVideoHeight;
        fn["type_tracking"] >> is3dTracking;
    }
    filterMap["camera_index"] = cameraIndex;
    filterMap["video_width"] = cameraVideoWidth;
    filterMap["video_height"] = cameraVideoHeight;
    filterMap["type_tracking"] = is3dTracking;
    return filterMap;
}

map<string, IXMLSerializer::SerializeData> RealSenseVideoSourceXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<RealSenseVideoSource> cameraSource = filter;
    if (cameraSource != NULL)
    {
        filterMap["camera_index"] = cameraSource->getCameraIndex();
        filterMap["video_width"] = cameraSource->getOutputResolution().width;
        filterMap["video_height"] = cameraSource->getOutputResolution().height;
        filterMap["type_tracking"] = cameraSource->get3dTracking();
    }
    else
    {
        IVCP_LOG(LOG_ERROR, "RealSense video source is not set");
    }

    return filterMap;
}

}

#endif
