///////////////////////////////////////////////////////////////////////////
// <copyright file="video_player.hpp" company="Intel Corporation">
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



#ifndef __VIDEO_PLAYER_HPP__
#define __VIDEO_PLAYER_HPP__

#include "libivcp/video_source/ivideo_player.hpp"
#include "libivcp/video_source/video_playlist.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file video_player.hpp
 * \brief Video player
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Video player
 * \details Video player retrieves frames in Video playlist order
 * \sa IVideoPlayer
 * \sa VideoPlaylist
 */
class IVCP_EXPORT VideoPlayer: public IVideoPlayer
{
public:
    /*!
     * \brief Default constructor
     */
    VideoPlayer();

    virtual ~VideoPlayer() {};

    virtual void setPlaylist(SPtr<VideoPlaylist > videoPlaylist);

    virtual SPtr<VideoPlaylist > getPlaylist();

    virtual cv::Mat getNextFrame();

    virtual cv::Mat getPreviousFrame();

    virtual void seek(const VideoPlaylistFrame &frame);

    virtual void stop();

    virtual bool haveNextFrame();

    virtual FilterType getRootType() const;

    virtual bool getAutoExposureStatus();

protected:
    /*! 
     * \brief Video player uses this playlist to choose the next frame to load
     */
    SPtr<VideoPlaylist> playlist;

    /*!
     * \brief Current frame in the playlist
     */
    VideoPlaylistFrame currentFrame;

private:
    /*!
     * \brief Whether playback is stoped
     */
    bool stopped;
};
};

/** @}*/
#endif
