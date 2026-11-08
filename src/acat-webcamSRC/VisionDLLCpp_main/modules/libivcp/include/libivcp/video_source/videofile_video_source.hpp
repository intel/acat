///////////////////////////////////////////////////////////////////////////
// <copyright file="videofile_video_source.hpp" company="Intel Corporation">
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


#ifndef __VIDEO_SOURCE_VIDEOFILE_HPP__
#define __VIDEO_SOURCE_VIDEOFILE_HPP__

#include <string>
#include "libivcp/video_source/video_source.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file videofile_video_source.hpp
 * \brief Videofile video source 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Videofile video source 
 * \details This video source returns frames from videofile in BGR format
 */
class IVCP_EXPORT VideofileVideoSource: public VideoSource
{
public:
    /*!
     * \brief Default constructor
     * \param[in] filename Path to videofile
     * \param[in] _multipleVideoFiles Whether or not to use multiple video files as a single file
     * \param[in] is12Bit Whether or not we convert 24 bit RGB values to 16 bit b/w
     */
    VideofileVideoSource(std::string filename = "", bool _multipleVideoFiles = false, bool is12Bit = false);

    virtual ~VideofileVideoSource();

    /*!
     * \brief Get next frame from source
     * \throw IOException Unable to open videofile if several files are used
     */
    virtual cv::Mat getNextFrame();

    virtual void seek(int frame);

    virtual VideoSourceType getType();

    virtual void open();

    virtual void release();

    /*!
     * \brief Open videofile
     * \param[in] filename Path to videofile
     * \throw IOException Unable to open videofile
     */
    void openVideo(std::string filename = "");

    /*!
     * \brief Check that videofile source is opened
     * \return True if video source is opened
     */
    bool isOpen();

    /*!
     * \brief Get number of frames in videofile
     * \return Number of frames
     */
    int getFrameCount();

    /*!
     * \brief Set path to videofile
     * \param[in] filename Path to videofile
     */
    void setFilename(std::string filename);

    /*!
     * \brief Get path to used videofile
     * \return Path to videofile
     */
    std::string getFilename();

    /*!
     * \brief Set flag to use multiple video files as a single file
     * \param[in] _multipleVideoFiles Value to set
     */
    void setMultipleFiles(bool _multipleVideoFiles);

    /*!
     * \brief Setter for the data12Bit flag
     * \param[in] is12Bit Whether or not we convert 24 bit RGB values to 16 bit b/w
     */
    void set12BitSource(bool is12Bit = false);

    /*!
     * \brief Getter for the data12Bit flag
     * \return Whether or not we convert 24 bit RGB values to 16 bit b/w
     */
    bool is12BitSource();

    /*!
     * \brief Getter for the multipleVideoFiles flag
     * \return Whether we want to use multiple video files or just one as a single file
     */
    bool hasMultipleVideoFiles();

    /*!
     * \brief Get output resolution of the video file
     * \return Output resolution
     */
    virtual cv::Size getOutputResolution();

protected:

    virtual void convertColorFormat(const cv::Mat &src, cv::Mat &dst);

    /*!
     * \brief Path to used videofile
     */
    std::string videoFilename;

    /*!
     * \brief OpenCV VideoCapture used to obtain video frames
     */
    cv::VideoCapture cap;

    /*!
     * \brief True for color video files which were constucted from 12 bit b/w source 
     * \details In case the flag is set output of the Video Source is 16 bit b/w frames. The 16 bit data 
     * contains only 12 bits of valid information. The rest is zeros. The 16 bite elements were created from 2 first 
     * bytes of the original color video source
     */
    bool data12Bit;

    /*!
     * \brief Temp storage of images converted to 12 bit format
     * \details In case we save 12 bit data from camera to an avi file it is stored as color video stream with only
     * first 12 meaningful bits. When we read it back we have to convert the data to the original 12 bit format
     */
    cv::Mat imageConverted;

    /*!
     * \brief This flag indicates whether videosource is one-file-source or several-files-source.
     * \details In case multiple video files were created by previous gui framework operation
     * and user wants to use them as videofile videosource, additional parameter
     * "multipleVideoFiles" with true value should be passed to createVideofileVideoSource()
     */
    bool multipleVideoFiles;

    /*!
     * \brief This field stores current file index
     * \details In case multiple video files were created by previous gui framework operation
     * and user wants to use them as videofile videosource, this parameter stores current file
     * index
     */
    size_t videoFileIndex;

private:

    std::string getNextFileName() const;
};
};

/** @}*/
#endif
