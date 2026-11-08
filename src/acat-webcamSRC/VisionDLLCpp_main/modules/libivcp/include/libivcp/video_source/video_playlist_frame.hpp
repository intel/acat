///////////////////////////////////////////////////////////////////////////
// <copyright file="video_playlist_frame.hpp" company="Intel Corporation">
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



#ifndef __VIDEO_PLAYLIST_FRAME_HPP__
#define __VIDEO_PLAYLIST_FRAME_HPP__

#include <utility>
#include "libivcp/types.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file video_playlist_frame.hpp
 * \brief Video frame
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief End frame/source index
 */
#define VIDEO_PLAYLIST_FRAME_END -1

/*!
 * \brief This means that we have reached the end of the playlist
 */
#define VIDEO_PLAYLIST_FRAME_EOF VideoPlaylistFrame(VIDEO_PLAYLIST_FRAME_END, VIDEO_PLAYLIST_FRAME_END)

/*! 
 * \brief Video frame: video source index and frame index pair
 * \sa VideoPlaylistRange
 */
class IVCP_EXPORT VideoPlaylistFrame
{
public:
    /*!
     * \brief Video playlist frame constructor
     * \param[in] _sourceIndex Index of video source
     * \param[in] _frameIndex Index of frame in video source
     */
    VideoPlaylistFrame(int _sourceIndex = 0, int _frameIndex = 0);

    /*!
     * \brief Overload the operator ==
     * \param[in] frame Frame with which comparison
     * \return True if frames are equal
     */
    bool operator== (const VideoPlaylistFrame &frame);

    /*!
     * \brief Overload the operator !=
     * \param[in] frame Frame with which comparison
     * \return True if frames are not equal
     */
    bool operator!= (const VideoPlaylistFrame &frame);

    /*!
     * \brief Overload the operator ++
     */
    VideoPlaylistFrame& operator++();

    /*!
     * \brief Index of video source
     */
    int sourceIndex;

    /*!
     * \brief Index of frame in video source
     */
    int frameIndex;
};

}

/** @}*/
#endif
