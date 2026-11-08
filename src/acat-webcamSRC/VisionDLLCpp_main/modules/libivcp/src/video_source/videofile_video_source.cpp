///////////////////////////////////////////////////////////////////////////
// <copyright file="videofile_video_source.cpp" company="Intel Corporation">
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


#include <sstream>
#include "libivcp/video_source/videofile_video_source.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{

VideofileVideoSource::VideofileVideoSource(std::string filename, bool _multipleVideoFiles, bool is12Bit):
    videoFilename(filename), videoFileIndex(0)
{
    set12BitSource(is12Bit);
    setMultipleFiles(_multipleVideoFiles);
}

VideofileVideoSource::~VideofileVideoSource() 
{
    release();
}

std::string VideofileVideoSource::getNextFileName() const
{
    string tempFilename = videoFilename;
    size_t dotPosition = tempFilename.find_last_of('.');
    if (dotPosition != string::npos)
    {
        tempFilename.insert(dotPosition,std::to_string((unsigned long long)videoFileIndex));
    }
    else
    {
        tempFilename.append(std::to_string((unsigned long long)videoFileIndex));
    }
    return tempFilename;
}

Mat VideofileVideoSource::getNextFrame()
{
    if (!isOpen())
    {
        this->openVideo();
    }

    Mat imageOriginal;
    cap >> imageOriginal;

    if (imageOriginal.empty() && multipleVideoFiles)
    {
        videoFileIndex++;
        string tempFilename = getNextFileName();

        cap.release();
        cap.open(tempFilename);
        if (!cap.isOpened())
        {
            return Mat();
        }
        cap >> imageOriginal;
    }

    if ((output_resolution.width != 0) || (output_resolution.height != 0))
    {
        Mat imageResized(output_resolution.height, output_resolution.width, imageOriginal.type());
        resize(imageOriginal, imageResized, imageResized.size());
        imageOriginal = imageResized;
        imageConverted.create(imageOriginal.size(), imageOriginal.type());
    }

    try
    {
        convertColorFormat(imageOriginal, imageConverted);
    }
    catch (IncorrectParamException)
    {
        return Mat();
    }

    return imageConverted;
}

void VideofileVideoSource::openVideo(string filename)
{
    release();
    if (!filename.empty())
    {
        setFilename(filename);
    }
    cap.open(videoFilename);

    if (!cap.isOpened())
    {
        stringstream ss; 
        ss << "Unable to open video file: " << videoFilename;
        IVCP_EXCEPTION( IOException, ss.str() );
    }
}

void VideofileVideoSource::seek(int frame)
{
    if (!isOpen())
    {
        this->openVideo();
    }

    if (multipleVideoFiles)
    {
        string tempFileName = videoFilename;
        videoFileIndex = 0;

        cap.release();
        cap.open(tempFileName);
        while ( (frame > cap.get(cv::CAP_PROP_FRAME_COUNT) - 1) && (cap.isOpened()))
        {
            frame -= (int)cap.get(cv::CAP_PROP_FRAME_COUNT);

            videoFileIndex++;
            tempFileName = getNextFileName();

            cap.release();
            cap.open(tempFileName);
        }
        if (!cap.isOpened())
        {
            // if frame value exceedes total number of frames among all availiable files
            videoFileIndex--;
            tempFileName = getNextFileName();
            cap.open(tempFileName);
        }
    }

    cap.set(cv::CAP_PROP_POS_FRAMES, frame);
}

int VideofileVideoSource::getFrameCount() 
{
    if (!isOpen())
    {
        this->openVideo();
    }

    if (multipleVideoFiles)
    {
        cv::VideoCapture tempCap;
        size_t tempInputFilesCount = videoFileIndex,
            frameCount = 0;
        string tempFileName = videoFilename;

        tempCap.open(tempFileName);
        videoFileIndex = 0;
        while (tempCap.isOpened())
        {
            frameCount += (size_t)cap.get(cv::CAP_PROP_FRAME_COUNT);
            videoFileIndex++;
            tempFileName = getNextFileName();

            tempCap.release();
            tempCap.open(tempFileName);
        }

        videoFileIndex = tempInputFilesCount;
        return (int)frameCount;
    }

    if (cap.isOpened())
    {
        return (int)cap.get(cv::CAP_PROP_FRAME_COUNT);
    }
    else 
    {
        return 0;
    }
}

Size VideofileVideoSource::getOutputResolution()
{
    Size resolution;
    if ((output_resolution.width > 0) && output_resolution.height > 0)
    {
        resolution = VideoSource::getOutputResolution();
    }
    else
    {
        if (!isOpen())
        {
            openVideo();
        }

        if (cap.isOpened()) 
        {
            resolution.width = (int) cap.get(cv::CAP_PROP_FRAME_WIDTH);
            resolution.height = (int) cap.get(cv::CAP_PROP_FRAME_HEIGHT);
        }
    }
    return resolution;
}

VideoSourceType VideofileVideoSource::getType()
{
    return VIDEOFILE;
}

string VideofileVideoSource::getFilename()
{
    return videoFilename;
}

void VideofileVideoSource::setFilename(string filename)
{
    videoFilename = filename;
    release();
}

void VideofileVideoSource::setMultipleFiles(bool _multipleVideoFiles)
{
    multipleVideoFiles = _multipleVideoFiles;
}

bool VideofileVideoSource::hasMultipleVideoFiles()
{
    return multipleVideoFiles;
}

bool VideofileVideoSource::is12BitSource()
{
    return data12Bit;
}

void VideofileVideoSource::set12BitSource(bool is12Bit)
{
    data12Bit = is12Bit;
}

bool VideofileVideoSource::isOpen()
{
    return cap.isOpened();
}

void VideofileVideoSource::open()
{
    openVideo();
}

void VideofileVideoSource::release()
{
    if (cap.isOpened())
    {
        cap.release();
    }
}

void VideofileVideoSource::convertColorFormat(const Mat &src, Mat &dst)
{
    if (is12BitSource() && (!src.empty()) && (src.type() == CV_8UC3))
    {
        if ((dst.type() != CV_16UC1) && (dst.size() != src.size()))
        {
            dst.create(src.size(), CV_16UC1);
        }

        for (int r = 0; r < src.rows; r++)
        {
            for (int c = 0; c < src.cols; c++)
            {
                unsigned short b0 = src.at<cv::Vec3b>(r, c)[0];
                unsigned short b1 = ((unsigned short)src.at<cv::Vec3b>(r, c)[1] & 15) << 8;

                dst.at<unsigned short>(r, c) = b0 | b1;
            }
        }
    }
    else
    {
        VideoSource::convertColorFormat(src, dst);
    }
}

}
