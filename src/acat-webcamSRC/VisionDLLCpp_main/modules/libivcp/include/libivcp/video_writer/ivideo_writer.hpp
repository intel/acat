///////////////////////////////////////////////////////////////////////////
// <copyright file="ivideo_writer.hpp" company="Intel Corporation">
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


#ifndef __IVIDEO_WRITER_HPP__
#define __IVIDEO_WRITER_HPP__

#include "libivcp/exceptions/io_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file ivideo_writer.hpp
 * \brief Interface for video writer 
 * \copyright (C) 2014, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Interface for video writer
 * \sa RawfileVideoWriter
 * \sa AvifileVideoWriter
 */
class IVCP_EXPORT IVideoWriter
{
public:
    /*!
     * \brief Destructor
     */
    virtual ~IVideoWriter() {};

    /*!
     * \brief Initialize or reinitialize video writer
     * \param[in] filename Output video file name
     * \param[in] fourcc Output video file codec
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \throw IOException Unable to open the video file
     * \return True if video writer has been successfully initialized
     */
    virtual bool open(std::string filename = "", int fourcc = 0, cv::Size frameSize = cv::Size(1280, 1024), int fps = 30) = 0;

    /*!
     * \brief Release video writer
     */
    virtual void release() = 0;

    /*!
     * \brief Check that video writer is opened
     * \return True if video writer has been successfully initialized
     */
    virtual bool isOpened() const = 0;
    
    /*!
     * \brief Write the next video frame
     * \param[in] image Output video frame
     * \details image must have the same size as has been specified when opening the video writer
     */
    virtual void write(cv::Mat image) = 0;

};
};

/** @}*/
#endif