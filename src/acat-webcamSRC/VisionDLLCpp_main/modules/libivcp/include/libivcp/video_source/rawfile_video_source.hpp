///////////////////////////////////////////////////////////////////////////
// <copyright file="rawfile_video_source.hpp" company="Intel Corporation">
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


#ifndef __VIDEO_SOURCE_RAWFILE_HPP__
#define __VIDEO_SOURCE_RAWFILE_HPP__

#include <string>
#include <fstream>
#include "libivcp/video_source/video_source.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file rawfile_video_source.hpp
 * \brief Rawfile video source
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Rawfile video source
 * \details This video source returns frames from raw file in BGR format
 */
class IVCP_EXPORT RawfileVideoSource: public VideoSource
{
public:
    /*!
     * \brief Default constructor
     * \param[in] filename Path to videofile
     */
    RawfileVideoSource(std::string filename = "");

    virtual ~RawfileVideoSource();

    /*!
     * \brief Get next frame from source
     * \throw IOException Unable to open raw file
     */
    virtual cv::Mat getNextFrame();

    virtual void seek(int frame);

    virtual VideoSourceType getType();

    virtual void open();

    virtual void release();

    /*!
     * \brief Open videofile
     * \param[in] filename Path to raw file
     * \throw IOException Unable to open raw file
     */
    void openVideo(std::string filename = "");

    /*!
     * \brief Check that raw file source is opened
     * \return True if video source is opened
     */
    bool isOpen() const;

    /*!
     * \brief Get number of frames in raw file
     * \return Number of frames
     * \throw IOException File is not opened
     */
    int getFrameCount();

    /*!
     * \brief Set path to raw file
     * \param[in] filename Path to raw file
     */
    void setFilename(std::string filename);

    /*!
     * \brief Get path to used raw file
     * \return Path to raw file
     */
    std::string getFilename() const;

    /*!
     * \brief Getter for type field
     * \return Whether or not we have 12-bit file
     */
    bool is12BitSource() const;

    /*!
     * \brief Get output resolution of the raw file
     * \return Output resolution
     */
    virtual cv::Size getOutputResolution();
        
    /*!
     * \brief Get frame rate of the raw file
     * \return frame rate
     * \throw IOException File is not opened
     */
    int getFramerate();

    /*!
     * \brief Get current frame
     * \return frame index
     */
    int getFrameIndex();

    /*!
     * \brief Get time offset of file header
     * \return frame rate
     * \throw IOException File is not opened
     */
    long getTimeOffset();

    /*!
     * \brief Get system time ticks for file
     * \return frame rate
     * \throw IOException File is not opened
     */
    long getSystemTimeTicks();

protected:

    /*!
     * \brief Path to used raw file
     */
    std::string videoFilename;

    /*!
     * \brief std variable responsible which
     * handles reading/writing to file
     */
    std::ifstream stream;

    /*!
     * \brief The initial position of the current character in the input stream
     * (is set while raw file opening process)
     */
    std::streampos begin;

    /*!
     * \brief Variable to represent position offset in a raw file stream.
     */
    std::streamoff step;

    /*!
     * \brief Total frame number in raw file
     */
    int frameCount;

    /*!
     * \brief Frame rate in raw file
     */
    int framerate;

    /*!
     * \brief Frame rate in raw file
     */
    long systemTimeTicks;

    /*!
     * \brief Frame rate in frame
     */
    long timeOffset;

    /*!
     * \brief Frame resolution in raw file (height, width)
     */
    cv::Size resolution;

    /*!
     * \brief Raw file type type
     */
    enum Type { GRAY16 = 1, GRAY8 = 2 }
    /*!
     * \brief Raw file type
     */
    type;

    /*!
     * \brief Temp storage of images converted to 12 bit format
     * \details In case we save 12 bit data from camera to an raw file it is stored as color video stream with only
     * first 12 meaningful bits.
     */
    cv::Mat imageConverted;
    
    /*!
     * \brief Get next frame from source
     * \details Instead of getNextFrame this function does not check if the file is opened
     * \throw IOException Unable to open raw file
     */
    cv::Mat getNextFrameNoChecks();

private:
};
};

/** @}*/
#endif
