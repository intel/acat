///////////////////////////////////////////////////////////////////////////
// <copyright file="video_source.cpp" company="Intel Corporation">
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



#include "libivcp/video_source/video_source.hpp"

using namespace cv;

namespace ivcp
{
FilterType VideoSource::getRootType() const
{
    return VIDEO_SOURCE_FILTER;
}

void VideoSource::convertColorFormat(const cv::Mat &src, cv::Mat &dst)
{
    if ((src.type() != CV_8UC3) && (src.type() != CV_8UC1) && (src.type() != CV_16UC1) && (!src.empty()))
    {
        IVCP_EXCEPTION(IncorrectParamException, "Source image is not in one of acceptable formats (RBG, 8_UC1, 16_UC1)");
    }

    dst = src;
}

bool VideoSource::getAutoExposureStatus()
{
    return false;
}
Size VideoSource::getOutputResolution()
{
    return output_resolution;
}

void VideoSource::setOutputResolution(int width, int height)
{
    output_resolution = Size(width, height);
}

}
