///////////////////////////////////////////////////////////////////////////
// <copyright file="rawfile_video_source.cpp" company="Intel Corporation">
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
#include "libivcp/video_source/rawfile_video_source.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{

static const int fileMagic = 0x51515169;
static const int frameMagic = 0x21212177;

/*!
 * \brief Raw file information stored in file header 
 */
struct FileHeader
{
    /*!
     * \brief Number for handling reading/writing file
     */
    int fileMagic;
    
    /*!
     * \brief File size
     */
    int size;

    /*!
     * \brief Frame rate for raw file
     */
    int framerate;

    /*!
     * \brief Frames width
     */
    int width;

    /*!
     * \brief Frames height
     */
    int height;

    /*!
     * \brief Type of raw file (8-bit or 12-bit)
     */
    int imageType;

    /*!
     * \brief System time ticks
     */
    long systemTimeTicks;
};

/*!
 * \brief Frame information stored in frame header 
 */
struct FrameHeader
{
    /*!
     * \brief Number for handling reading/writing frame
     */
    int frameMagic;

    /*!
     * \brief Size of frame
     */
    int size;

    /*!
     * \brief Time offset
     */
    long timeOffset;
};


RawfileVideoSource::RawfileVideoSource(std::string filename):
    videoFilename(filename), begin(0), step(0), frameCount(0), framerate(0),
    systemTimeTicks(0), timeOffset(0), resolution(0, 0), type(GRAY16)
{
}

RawfileVideoSource::~RawfileVideoSource()
{
    release();
}

Mat RawfileVideoSource::getNextFrame()
{
    if (!isOpen())
    {
        this->openVideo();
    }

    if (getFrameIndex() >= getFrameCount())
    {
        return Mat();
    }

    return getNextFrameNoChecks();
}

Mat RawfileVideoSource::getNextFrameNoChecks()
{
    FrameHeader frameHeader;
    memset(&frameHeader, 0, sizeof(frameHeader));
    stream.read((char*) &frameHeader, sizeof(frameHeader));
    if (!stream.good())
    {
        stringstream ss;
        ss << "Unable to read frame from video file: " << videoFilename;
        IVCP_EXCEPTION( IOException, ss.str() );
    }
    if (frameHeader.frameMagic != frameMagic)
    {
        stringstream ss;
        ss << "Error while reading video file: " << videoFilename<<endl;
        ss << "frameHeader.frameMagic "<<frameHeader.frameMagic;
        ss << " does not equal to "<<frameMagic;
        IVCP_EXCEPTION( IOException, ss.str() );
    }
    if (frameHeader.size < (int)sizeof(frameHeader))
    {
        stringstream ss;
        ss << "Error while reading video file: " << videoFilename<<endl;
        ss << "frameHeader.size "<<frameHeader.size;
        ss << " is lesser than "<<sizeof(frameHeader);
        IVCP_EXCEPTION( IOException, ss.str() );
    }
    
    if (frameHeader.size > (int)sizeof(frameHeader))
    {
        stream.seekg(frameHeader.size-sizeof(frameHeader), ios::cur);
    }

    timeOffset = frameHeader.timeOffset;

    Mat imageOriginal(resolution, (type == GRAY16) ? CV_16UC1 : CV_8UC1);

    int rowBytes = resolution.width*((int)imageOriginal.elemSize());
    for (int y = 0; y < resolution.height; y++)
    {
        stream.read((char*) imageOriginal.ptr(y), rowBytes); // todo padding?
    }

    if (!stream.good())
    {
        stringstream ss;
        ss << "Unable to read frame from video file: " << videoFilename;
        IVCP_EXCEPTION( IOException, ss.str() );
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

void RawfileVideoSource::seek(int frame)
{
    if (!isOpen())
    {
        this->openVideo();
    }
    
    if (frame < 0)
    {
        frame = 0;
    }
    else if (frame >= frameCount)
    {
        frame = frameCount - 1;
    }

    stream.seekg(begin+frame*step);
}

int RawfileVideoSource::getFrameCount()
{
    if (!isOpen())
    {
        this->openVideo();
    }
    
    return frameCount;
}

void RawfileVideoSource::openVideo(string filename)
{
    release();
    if (!filename.empty())
    {
        setFilename(filename);
    }

    stream.open(videoFilename, ios::in | ios::binary );
    if (!stream.is_open() || !stream.good())
    {
        release();
        stringstream ss;
        ss << "Unable to open raw file: " << videoFilename;
        IVCP_EXCEPTION( IOException, ss.str() );
    }
    stream.seekg(0, ios::end);

    std::streampos end = stream.tellg();
    stream.seekg(0, ios::beg);

    FileHeader fileHeader;
    memset(&fileHeader, 0, sizeof(fileHeader));

    stream.read((char*) &fileHeader, sizeof(fileHeader));

    if (fileHeader.fileMagic != fileMagic ||
        fileHeader.size < (int)sizeof(fileHeader) ||
        fileHeader.width < 1 ||
        fileHeader.height < 1 ||
        (!stream.good()))
    {
        release();
        stringstream ss;
        ss << "Unable to open raw file: " << videoFilename;
        IVCP_EXCEPTION( IOException, ss.str() );
    }
    if (fileHeader.size > (int)sizeof(fileHeader))
    {
        stream.seekg(fileHeader.size-sizeof(fileHeader),ios::cur);
    }

    framerate = fileHeader.framerate;
    systemTimeTicks = fileHeader.systemTimeTicks;
    resolution = Size(fileHeader.width, fileHeader.height);
    type = (Type) fileHeader.imageType;

    begin = stream.tellg();
    getNextFrameNoChecks(); // read first frame to find out full frame size
    step = stream.tellg()-begin;
    stream.seekg(begin, ios::beg); // go back to the first frame

    if (step > 0) frameCount = (int) ((end-begin)/step);
}

int RawfileVideoSource::getFramerate()
{
    if (!isOpen())
    {
        this->openVideo();
    }

    return framerate;
}

int RawfileVideoSource::getFrameIndex()
{
    if (!stream.good())
    {
        return 0;
    }

    return (int)((stream.tellg() - begin) / step);
}

long RawfileVideoSource::getTimeOffset()
{
    if (!isOpen())
    {
        this->openVideo();
    }

    return timeOffset;
}

long RawfileVideoSource::getSystemTimeTicks() 
{
    if (!isOpen())
    {
        this->openVideo();
    }

    return systemTimeTicks;
}
    
Size RawfileVideoSource::getOutputResolution()
{
    Size r;
    if ((output_resolution.width > 0) && output_resolution.height > 0)
    {
        r = VideoSource::getOutputResolution();
    }
    else
    {
        if (!isOpen())
        {
            openVideo();
        }
        r = resolution;
    }
    return r;
}

VideoSourceType RawfileVideoSource::getType()
{
    return RAWFILE;
}

string RawfileVideoSource::getFilename() const
{
    return videoFilename;
}

void RawfileVideoSource::setFilename(string filename)
{
    videoFilename = filename;
    release();
}

bool RawfileVideoSource::is12BitSource() const
{
    return (type == GRAY16);
}

bool RawfileVideoSource::isOpen() const
{
    return (stream.is_open());
}

void RawfileVideoSource::open()
{
    openVideo();
}

void RawfileVideoSource::release()
{
    if (isOpen())
    {
        stream.close();
        type = GRAY16;
        framerate = 0;
        resolution = Size(0, 0);
        frameCount = 0;
        begin = std::streampos(0);
        step = std::streamoff(0);
    }
}

}
