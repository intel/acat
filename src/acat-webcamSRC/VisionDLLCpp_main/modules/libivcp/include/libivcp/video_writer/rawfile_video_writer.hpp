///////////////////////////////////////////////////////////////////////////
// <copyright file="rawfile_video_writer.hpp" company="Intel Corporation">
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



#ifndef __VIDEO_WRITER_RAWFILE_HPP__
#define __VIDEO_WRITER_RAWFILE_HPP__

#include <string>
#include <fstream>
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/video_writer/ivideo_writer.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file rawfile_video_writer.hpp
 * \brief Rawfile video writer
 * \copyright (C) 2014, Intel Corporation, all rights reserved
 */

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

/*!
 * \brief Raw file type
 */
enum RawType { GRAY16 = 1, GRAY8 = 2 };

/*!
 * \brief Rawfile video writer
 * \details This video writer outputs frames to raw file in GRAY format
 */
class IVCP_EXPORT RawfileVideoWriter : public IVideoWriter
{
public:
    /*!
     * \brief Default constructor
     * \param[in] filename Path to videofile
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \param[in] type Output video file raw type
     */
    RawfileVideoWriter(std::string filename = "", cv::Size frameSize = cv::Size(1280, 1024), int fps = 30, RawType type = GRAY16);

    /*!
     * \brief Destructor
     */
    virtual ~RawfileVideoWriter();

    virtual bool open(std::string filename = "", int fourcc = 0, cv::Size frameSize = cv::Size(1280, 1024), int fps = 30);
    virtual void release();
    virtual bool isOpened() const;
    virtual void write(cv::Mat image);

protected:
    /*!
     * \brief Write raw file header
     * \param[in] fileHeader raw file header
     */
    void writeFileHeader(FileHeader fileHeader);

    /*!
     * \brief Write raw frame header
     * \param[in] frameHeader raw frame header
     */
    void writeFrameHeader(FrameHeader frameHeader);

    /*!
     * \brief Write video frame
     * \param[in] image Output video frame
     */
    void writeFrame(cv::Mat image);

    /*!
     * \brief std handler for writing to raw output file
     */
    std::ofstream stream;

    /*!
     * \brief Header of the raw output file
     */
    FileHeader rawHeader;

    /*!
     * \brief Type of the raw output file
     */
    RawType type;
};
};

/** @}*/
#endif
