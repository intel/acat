///////////////////////////////////////////////////////////////////////////
// <copyright file="ivideo_source.hpp" company="Intel Corporation">
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


#ifndef __IVIDEO_SOURCE_HPP__
#define __IVIDEO_SOURCE_HPP__

#include "libivcp/filter.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file ivideo_source.hpp
 * \brief Interface for video source 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Interface for video source 
 * \details Video source returns images in BGR format
 * \sa VideoSource
 * \sa VideofileVideoSource
 * \sa CameraVideoSource
 */
class IVCP_EXPORT IVideoSource: public Filter
{
public:
    /*!
     * \brief Video source destructor
     */
    virtual ~IVideoSource() {};

    /*! 
     * \brief Get image of the current frame
     * \return Next frame. Frame is in CV_BGR format
     */
    virtual cv::Mat getNextFrame() = 0;

    /*!
     * \brief Seek to specific position in the video source
     * \param[in] frame this frame will be the next returned by getNextFrame()
     */
    virtual void seek(int frame) = 0;

    /*!
     * \brief Get type of video source
     * \return VideoSource type
     */
    virtual VideoSourceType getType() = 0;

    /*!
     * \brief Open video source
     * \throw IOException Unable to open video source
     */
    virtual void open() = 0;

    /*!
     * \brief Release underlying video source resources
     */
    virtual void release() = 0;
    /*!
     * \brief Check that auto exposure is working
     * \return True if auto exposure is working, false otherwise
     */
    virtual bool getAutoExposureStatus() = 0;

    /*!
     * \brief Get output resolution of video source
     * \return Output resolution
     */
    virtual cv::Size getOutputResolution() = 0;

    /*!
     * \brief Set output resolution of video source 
     * \param[in] width Output width
     * \param[in] height Output height
     * \throw IncorrectParamException Parameter is not supported
     */
    virtual void setOutputResolution(int width, int height) = 0;

protected:

private:

};
};

/** @}*/
#endif 
