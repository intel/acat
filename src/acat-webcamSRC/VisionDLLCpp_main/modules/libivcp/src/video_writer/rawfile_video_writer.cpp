///////////////////////////////////////////////////////////////////////////
// <copyright file="rawfile_video_writer.cpp" company="Intel Corporation">
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

#include "libivcp/video_writer/rawfile_video_writer.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
    RawfileVideoWriter::RawfileVideoWriter(string filename, Size frameSize, int fps, RawType _type)
    {
        type = _type;
        if (!filename.empty())
        {
            open(filename, 0, frameSize, fps);
        }
    }

    RawfileVideoWriter::~RawfileVideoWriter()
    {
        release();
    }

    bool RawfileVideoWriter::open(string filename, int /*fourcc*/, Size frameSize, int fps)
    {
        release();
        if (filename.empty())
        {
            stringstream ss;
            ss << "File name is empty";
            IVCP_EXCEPTION(IOException, ss.str());
        }

        stream.open(filename, ios::out | ios::binary);
        if (!stream.is_open() || !stream.good())
        {
            release();
            stringstream ss;
            ss << "Unable to open raw file: " << filename;
            IVCP_EXCEPTION(IOException, ss.str());
        }

        rawHeader.fileMagic = fileMagic;
        rawHeader.framerate = fps;
        rawHeader.height = frameSize.height;
        rawHeader.imageType = type;
        rawHeader.size = sizeof(rawHeader);
        rawHeader.systemTimeTicks = 0;
        rawHeader.width = frameSize.width;

        writeFileHeader(rawHeader);
        return true;
    }

    bool RawfileVideoWriter::isOpened() const
    {
        return (stream.is_open());
    }

    void RawfileVideoWriter::release()
    {
        if (isOpened())
        {
            stream.close();
            type = GRAY16;
        }
    }

    void RawfileVideoWriter::write(cv::Mat image)
    {
        if (!image.empty())
        {
            FrameHeader frameHeader;
            frameHeader.frameMagic = frameMagic;
            frameHeader.size = sizeof(frameHeader);
            frameHeader.timeOffset = 0;
            writeFrameHeader(frameHeader);

            writeFrame(image);
        }
    }

    void RawfileVideoWriter::writeFileHeader(FileHeader fileHeader)
    {
        stream.write((char*)&(fileHeader.fileMagic), sizeof(fileHeader.fileMagic));
        stream.write((char*)&(fileHeader.size), sizeof(fileHeader.size));
        stream.write((char*)&(fileHeader.framerate), sizeof(fileHeader.framerate));
        stream.write((char*)&(fileHeader.width), sizeof(fileHeader.width));
        stream.write((char*)&(fileHeader.height), sizeof(fileHeader.height));
        stream.write((char*)&(fileHeader.imageType), sizeof(fileHeader.imageType));
        stream.write((char*)&(fileHeader.systemTimeTicks), sizeof(fileHeader.systemTimeTicks));
    }

    void RawfileVideoWriter::writeFrameHeader(FrameHeader frameHeader)
    {
        stream.write((char*)&(frameHeader.frameMagic), sizeof(frameHeader.frameMagic));
        stream.write((char*)&(frameHeader.size), sizeof(frameHeader.size));
        stream.write((char*)&(frameHeader.timeOffset), sizeof(frameHeader.timeOffset));
    }

    void RawfileVideoWriter::writeFrame(Mat image)
    {
        Size outputSize = Size(rawHeader.width, rawHeader.height);
        int outputType = (type == GRAY16) ? CV_16UC1 : CV_8UC1;
        Mat outputImage(outputSize, outputType);

        if (image.type() == CV_8UC3)
        {
            cvtColor(image, image, cv::COLOR_BGR2GRAY);
        }
        if (image.size() != outputSize)
        {
            Mat resized(outputSize, image.type());
            resize(image, resized, outputSize);
            image = resized.clone();
        }
        if (image.type() != outputType)
        {
            double alpha = 4096.0 / 256.0;
            if (type == GRAY8)
            {
                alpha = 256.0 / 4096.0;
            }
            image.convertTo(outputImage, outputImage.type(), alpha);
        }
        else
        {
            outputImage = image.clone();
        }
        
        int rowBytes = rawHeader.width * ((int)outputImage.elemSize());
        for (int y = 0; y < rawHeader.height; y++)
        {
            stream.write((char*)outputImage.ptr(y), rowBytes);
        }
    }
}