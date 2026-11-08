///////////////////////////////////////////////////////////////////////////
// <copyright file="video_source.hpp" company="Intel Corporation">
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


#ifndef __VIDEO_SOURCE_HPP__
#define __VIDEO_SOURCE_HPP__

#include "libivcp/video_source/ivideo_source.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file video_source.hpp
 * \brief Basic class for video source 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic class for video source 
 * \details Video source returns images in BGR format 
 */
class IVCP_EXPORT VideoSource: public IVideoSource
{
public:

    /*!
     * \brief Default constructor
     */
    VideoSource():
    output_resolution(0,0)
    {}

    /*!
     * \brief Get output resolution of video source
     * \return Output resolution
     */
    virtual cv::Size getOutputResolution();

    /*!
     * \brief Set output resolution of video source 
     * \param[in] width Output width
     * \param[in] height Output height
     * \throw IncorrectParamException Parameter is not supported
     */
    virtual void setOutputResolution(int width, int height);

    virtual FilterType getRootType() const;

    virtual bool getAutoExposureStatus();

protected:
    /*!
     * \brief Apply default rules for Video Source color formats
     * \details Currently the following formats are accepted: RBG, 8_UC1, 16_UC1
     * \param[in] src Image matrix to be converted to default color format
     * \param[out] dst Destination (converted image). In case no conversion is needed - the same as the source image
     * \throw IncorrectParamException src Matrix is not in acceptable format (RBG, 8_UC1, 16_UC1)
     */
    virtual void convertColorFormat(const cv::Mat &src, cv::Mat &dst);

    /*!
     * \brief Output video resolution 
     */
    cv::Size output_resolution;

private:

};
};

/** @}*/
#endif
