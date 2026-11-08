///////////////////////////////////////////////////////////////////////////
// <copyright file="avi_video_writer.hpp" company="Intel Corporation">
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


#ifndef __AVI_VIDEO_WRITER_HPP__
#define __AVI_VIDEO_WRITER_HPP__

#include <string>
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/video_writer/ivideo_writer.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file avi_video_writer.hpp
 * \brief AVI video writer
 * \copyright (C) 2014, Intel Corporation, all rights reserved
 */

namespace ivcp
{

/*!
 * \brief Avi video writer
 * \details This video writer outputs frames to avi file in BGR format
 */
class IVCP_EXPORT AvifileVideoWriter : public IVideoWriter
{
public:
    /*!
     * \brief Default constructor
     */
    AvifileVideoWriter();

    /*!
     * \brief Destructor
     */
    virtual ~AvifileVideoWriter();

    virtual bool open(std::string filename = "", int fourcc = 0, cv::Size frameSize = cv::Size(1280, 1024), int fps = 30);
    virtual void release();
    virtual bool isOpened() const;
    virtual void write(cv::Mat image);
protected:
    /*!
     * \brief Video writer handler
     */
    cv::VideoWriter output;
};
};

/** @}*/
#endif
