///////////////////////////////////////////////////////////////////////////
// <copyright file="ivideo_player.hpp" company="Intel Corporation">
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


#ifndef __IVIDEO_PLAYER_HPP__
#define __IVIDEO_PLAYER_HPP__

#include "libivcp/video_source/video_playlist.hpp"
#include "libivcp/sptr.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file ivideo_player.hpp
 * \brief Interface for Video player class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Interface for Video player class
 * \details Video player retrieves frames in Video playlist order
 * \sa VideoPlayer
 * \sa VideoPlaylist
 */
class IVCP_EXPORT IVideoPlayer: public Filter
{
public:
    /*!
    * \brief Destructor
    */
    virtual ~IVideoPlayer() {};

    /*! 
     * \brief Set video playlist 
     * \details Video player uses this playlist to choose the next frame to load
     * \param[in] videoPlaylist Pointer to video playlist
     */
    virtual void setPlaylist(SPtr<VideoPlaylist> videoPlaylist) = 0;

    /*! 
     * \brief Get video playlist 
     * \details Video player uses this playlist to choose the next frame to load
     * \return Pointer to Video playlist used in this player
     */
    virtual SPtr<VideoPlaylist> getPlaylist() = 0;

    /*! 
     * \brief Get image of the current frame
     * \return Image of the current frame in the order of the playlist
     */
    virtual cv::Mat getNextFrame() = 0;

    /*! 
     * \brief Get image of the previous frame and move current playlist frame pointer to this frame
     * \return Image of the previous frame of the playlist
     */
    virtual cv::Mat getPreviousFrame() = 0;

    /*!
     * \brief Seek to specific position in the playlist
     * \param[in] frame This frame will be the next returned by getNextFrame()
     */
    virtual void seek(const VideoPlaylistFrame &frame) = 0;

    /*!
     * \brief Stop the playback regardles of the playlist position
     * \details After that haveNextFrame function will return false even if the playlist have more frames to process
     * Playback is resumed back to normal state when setPlaylist or seek functions are called
     */
    virtual void stop() = 0;

    /*!
     * \brief Check whether playlist have one more frame to display
     */
    virtual bool haveNextFrame() = 0;

    /*!
     * \brief Check that auto exposure is working on current video source
     * \return True if auto exposure is working, false otherwise 
     */
    virtual bool getAutoExposureStatus() = 0;

protected:

private:

};
};

/** @}*/
#endif
