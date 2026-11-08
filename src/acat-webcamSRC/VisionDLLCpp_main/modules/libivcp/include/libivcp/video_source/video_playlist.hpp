///////////////////////////////////////////////////////////////////////////
// <copyright file="video_playlist.hpp" company="Intel Corporation">
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



#ifndef __VIDEO_PLAYLIST_HPP__
#define __VIDEO_PLAYLIST_HPP__

#include "libivcp/sptr.hpp"
#include "libivcp/video_source/ivideo_source.hpp"
#include "libivcp/video_source/video_playlist_range.hpp"
#include "libivcp/video_source/video_playlist_frame.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file video_playlist.hpp
 * \brief Video playlist
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 * \details Declaration of Video playlist object
 */

namespace ivcp
{  
/*! 
 * \brief Video playlist
 * \details Video playlist is used for creating a list of video sources and 
 * (possibly) ranges of frames inside of the videos
 * \sa VideoPlayer
 */
class IVCP_EXPORT VideoPlaylist: public Filter
{
public:
    /*!
     * \brief Default VideoPlaylist class constructor
     */
    VideoPlaylist();

    /*!
     * \brief Default VideoPlaylist class destructor
     */
    virtual ~VideoPlaylist();

    /*! 
     * \brief Add new subset of frames from given video source to the playlist
     * \details The frames are added to the end of the playlist
     * \param[in] videoSource Video source object 
     * \param[in] videoRange Range of frames in the video source
     */
    void addSourceRange(SPtr<IVideoSource > videoSource, const FrameRange &videoRange);

    /*! 
     * \brief Delete subset of frames from given video source to the playlist
     * \details The frames are removed from the playlist
     * \param[in] rangeIndex Index of range of frames in the video source
     */
    void delSourceRange(int rangeIndex);

    /*!
     * \brief Delete all sources 
     */
    void clear();

    /*! 
     * \brief Check whether next frame is available
     * \return True if there is the next fame in the playlist
     */
    bool haveNextFrame() const;

    /*! 
     * \brief Get index of the next frame
     * \details Returns index of the next frame in the playlist and advance to it
     * \return Index of the next frame in the playlist, VIDEO_PLAYLIST_FRAME_EOF in case there is no more frames in Video Playlist
     */
    VideoPlaylistFrame getNextFrame();

    /*! 
     * \brief Get index of the previous frame
     * \details Returns index of the previous frame in the playlist and advance to it
     * \return Index of the previous frame in the playlist
     */
    VideoPlaylistFrame getPreviousFrame();

    /*! 
     * \brief Get index of the current frame
     * \return Index of the current frame in the playlist (ie index of the current video source + index of the current frame in the current video source)
     */
    VideoPlaylistFrame getCurrentFrame() const;

    /*!
     * \brief Seek to specific position in the playlist
     * \param[in] frame This frame will be the next returned by getNextFrameIndex()
     */
    void seek(const VideoPlaylistFrame &frame);

    /*!
     * \brief Seek to specific position in the playlist
     * \param[in] rangeIndex Range index in the playlist 
     * \param[in] frameIndex Index of frame from selected range that will be next returned by getNextFrame()
     */
    void seek(int rangeIndex, int frameIndex);

    /*!
     * \brief Seek to the first frame of the current range so that getNextFrame will return first frame of the next range
     * \return The last frame of the current range or VIDEO_PLAYLIST_FRAME_EOF in case there is no frames in the list
     */
    VideoPlaylistFrame seekToNextRange();

    /*!
     * \brief Get video source from playlist
     * \param[in] sourceIndex Video source index in the playlist
     * \return Video source or 0 if sourceIndex is incorrect
     */
    SPtr<IVideoSource> getSourceByIndex(int sourceIndex) const;

    /*!
     * \brief Get range of frames from playlist
     * \param[in] rangeIndex Range index in the playlist 
     * \return Video playlist range or 0 if rangeIndex is incorrect
     */
    const VideoPlaylistRange getRangeByIndex(int rangeIndex) const;

    /*!
     * \brief Get index of the video source
     * \param[in] videoSource IVideoSource pointer
     * \return Index of video source in the playlist
     */
    int getIndexBySource(SPtr<IVideoSource> videoSource) const;

    /*!
     * \brief Get number of video sources in the playlist
     * \return Number of video sources
     */
    int getNumberOfSources() const;

    /*!
     * \brief Get number of ranges in the playlist
     * \return Number of ranges
     */
    int getNumberOfRanges() const;
    
    /*!
     * \brief Return type of a filter (ie Classifier, Feature etc)
     * \return Filter type
     */
    virtual FilterType getRootType() const;

protected:
    /*!
     * \brief Video sources for this playlist
     */
    std::deque<SPtr<IVideoSource> > videoSources;

    /*!
     * \brief Set of the frame ranges
     */
    std::deque<VideoPlaylistRange > videoFrameRanges;

    /*!
     * \brief Index of current frame range
     */
    int currentRangeIndex; 

    /*!
    * \brief Index of current frame
    */
    int currentFrameIndex;

private:

};
};

/** @}*/
#endif
