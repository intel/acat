///////////////////////////////////////////////////////////////////////////
// <copyright file="avi_video_writer.cpp" company="Intel Corporation">
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

#include "libivcp/video_writer/avi_video_writer.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
    AvifileVideoWriter::AvifileVideoWriter(){}

    AvifileVideoWriter::~AvifileVideoWriter()
    {
        output.release();
    }

    bool AvifileVideoWriter::open(std::string filename, int fourcc, Size frameSize, int fps)
    {
        return output.open(filename, fourcc, fps, frameSize);
    }

    void AvifileVideoWriter::release()
    {
        output.release();
    }

    bool AvifileVideoWriter::isOpened() const
    {
        return output.isOpened();
    }
    
    void AvifileVideoWriter::write(cv::Mat image)
    {
        output.write(image);
    }
}