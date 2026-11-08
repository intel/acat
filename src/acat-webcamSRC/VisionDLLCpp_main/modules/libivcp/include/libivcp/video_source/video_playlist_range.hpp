///////////////////////////////////////////////////////////////////////////
// <copyright file="video_playlist_range.hpp" company="Intel Corporation">
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



#ifndef __VIDEO_PLAYLIST_RANGE_HPP__
#define __VIDEO_PLAYLIST_RANGE_HPP__

#include <utility>
#include <stdint.h>
#include <limits>

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

namespace ivcp
{
/*!
 * \brief It is used when we need add all frames in video range
 */
const int END_FRAME = std::numeric_limits<int32_t>::max();

/*! 
 * \brief Frame range
 * \details Contains start frame index in range and end frame index in range
 * \sa VideoPlaylistRange
 */
struct IVCP_EXPORT FrameRange
{
    /*!
     * \brief Frame range constructor
     * \param[in] _firstFrame The first frame index in range
     * \param[in] _endFrame Index of range end. This index is not included in the frame range
     */
    FrameRange(int _firstFrame = 0, int _endFrame = END_FRAME):
        firstFrame(_firstFrame), endFrame(_endFrame)
    {}

    /*!
     * \brief The first frame index in range
     */
    int firstFrame;

    /*!
     * \brief Index of range end (a frame after the last frame)
     */
    int endFrame;
};

/*! 
 * \brief Video playlist range
 * \details Contains index of video source and frame range in this video source
 * \sa FrameRange
 * \sa VideoPlaylistFrame
 */
struct IVCP_EXPORT VideoPlaylistRange
{
    /*!
     * \brief Video playlist range constructor
     * \param[in] _sourceIndex Index of video source
     * \param[in] _sourceRange Frame range in given video source
     */
    VideoPlaylistRange(int _sourceIndex = -1, FrameRange _sourceRange = FrameRange(0, 0)):
        sourceIndex(_sourceIndex), sourceRange(_sourceRange)
    {}

    /*!
     * \brief Index of video source
     */
    int sourceIndex;

    /*!
     * \brief Frame range in the video source
     */
    FrameRange sourceRange;
};

};

/** @}*/
#endif
