///////////////////////////////////////////////////////////////////////////
// <copyright file="camera_video_source.hpp" company="Intel Corporation">
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


#ifndef __VIDEO_SOURCE_CAMERA_HPP__
#define __VIDEO_SOURCE_CAMERA_HPP__

#include "libivcp/video_source/video_source.hpp"
#include "libivcp/exceptions/no_camera_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file camera_video_source.hpp
 * \brief Camera video source 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Camera video source 
 * \details This video source returns frames from camera in BGR format
 */
class IVCP_EXPORT CameraVideoSource: public VideoSource
{
public:
    /*!
     * \brief Default constructor
     */
    CameraVideoSource();

    virtual ~CameraVideoSource();

    virtual cv::Mat getNextFrame();

    virtual void seek(int frame);

    virtual VideoSourceType getType();

    virtual void open();
    
    virtual void release();
    
    /*!
     * \brief Open camera
     * \param[in] index Id of the used video capturing device (for example cv::CAP_ANY)
     * \throw NoCameraException Unable to open camera with given index
     * \throw IncorrectParamException Camera does not support given settings
     */
    void openCamera(int index = -1);

    /*!
     * \brief Check that camera is opened
     * \return True if camera is opened
     */
    bool isOpen();
    
    /*!
     * \brief Set index of the used camera
     * \param[in] cameraIndex Index of the used camera
     */
    void setCameraIndex(int cameraIndex);

    /*!
     * \brief Get id of the used video capturing device
     * \return Index of the used camera
     */
    int getCameraIndex();

    /*!
     * \brief Get output resolution of video capturing device
     * \return Output resolution width
     */
	virtual cv::Size getOutputResolution();

    /*!
     * \brief Set output resolution of video capturing device 
     * \param[in] width Output width
     * \param[in] height Output height
     * \throw IncorrectParamException camera does not support given resolution
     */
	virtual void setOutputResolution(int width, int height);

    /*!
     * \brief Find all available devices
     * \return Vector with id and name pairs of all devices
     */
    static std::vector<std::pair<int, std::string>> listDevices();
protected:
    /*!
     * \brief Id of the used video capturing device
     */
    int cameraIndex;

    /*!
     * \brief OpenCV VideoCapture used to obtain video frames
     */
    cv::VideoCapture cap;

private:

};
};

/** @}*/
#endif
