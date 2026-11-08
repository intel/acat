///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_video_source.hpp" company="Intel Corporation">
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


#ifndef __VIDEO_SOURCE_REALSENSE_HPP__
#define __VIDEO_SOURCE_REALSENSE_HPP__

#ifdef USE_REALSENSE

#include "libivcp/video_source/video_source.hpp"
#include "libivcp/exceptions/no_camera_exception.hpp"

#include "pxcsensemanager.h"
#include "pxcfacemodule.h"
#include "pxcfacedata.h"
#include "pxcfaceconfiguration.h"

/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file realsense_video_source.hpp
 * \brief RealSense video source
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief RealSense camera video source
 * \details This video source returns frames from RealSense camera in BGR format
 */
class IVCP_EXPORT RealSenseVideoSource: public VideoSource
{
public:
    /*!
     * \brief Default constructor
     */
    RealSenseVideoSource();

    virtual ~RealSenseVideoSource();

    virtual cv::Mat getNextFrame();

    virtual void seek(int frame);

    virtual VideoSourceType getType();

    virtual void open();

    virtual void release();

    /*!
     * \brief Open RealSense camera
     * \param[in] index Id of the used video capturing device
     * \throw NoCameraException Unable to open RealSense camera with given index
     * \throw IncorrectParamException RealSense does not support given settings
     */
    void openCamera(int index = 0);

    /*!
     * \brief Check that RealSense camera is opened
     * \return True if RealSense camera is opened
     */
    bool isOpen();

    /*!
     * \brief Set index of the used RealSense camera
     * \param[in] cameraIndex Index of the used RealSense camera
     */
    void setCameraIndex(int cameraIndex);

    /*!
     * \brief Get id of the used video capturing device
     * \return Index of the used RealSense camera
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
     * \throw IncorrectParamException RealSense camera does not support given resolution
     */
    virtual void setOutputResolution(int width, int height);

    /*!
     * \brief Set 3d tracking option
     * \param[in] is3dTracking Whether to use 3d tracking
     */
    void set3dTracking(bool is3dTracking = true);

    /*!
     * \brief Get 3d tracking option
     * \return Whether 3d tracking is used
     */
    bool get3dTracking();

    /*!
     * \brief Find all available devices
     * \return Vector with didx and name pairs of all devices
     */
    static std::vector<std::pair<int, std::string>> listDevices();

protected:
    /*!
     * \brief Id of the used video capturing device
     */
    int cameraIndex;

    /*!
     * \brief SenseManager main instance
     */
    PXCSenseManager* psm;

    /*!
     * \brief CaptureManager, needed to locate an I/O device
     */
    PXCCaptureManager* captureMgr;

    /*!
     * \brief RealSense return code
     */
    pxcStatus status;

    /*!
     * \brief Instanse of face tracking and detection
     */
    PXCFaceModule* faceAnalyzer;

    /*!
     * \brief Instance of the face tracking module output data
     */
    PXCFaceData* fdata;

    /*!
     * \brief RealSense face configuration
     */
    PXCFaceConfiguration* config;

    /*!
     * \brief Whether SenseManager main instance is initialized
     */
    bool isInitialized;

    /*!
     * \brief Get from RealSense instances current frame in opencv Mat format
     */
    cv::Mat getFrameMat();

    /*!
     * \brief Get from RealSense instances current detected face rectangle and landmarks
     */
    void getAllFaceData();

    /*!
     * \brief Reset RealSense face tracking function
     * \details Perform reset action if needed only
     */
    void resetAction();

    /*!
     * \brief Whether to use 3d tracking
     */
    bool is3dTracking;

private:

};
};

/** @}*/
#endif
#endif
