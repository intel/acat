///////////////////////////////////////////////////////////////////////////
// <copyright file="video_player.cpp" company="Intel Corporation">
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


#include "libivcp/video_source/video_player.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{

VideoPlayer::VideoPlayer():
    playlist(Factory::createVideoPlaylist()), currentFrame(VIDEO_PLAYLIST_FRAME_EOF), stopped(false)
{
}

void VideoPlayer::setPlaylist(SPtr<VideoPlaylist> videoPlaylist)
{
    playlist = videoPlaylist;
    stopped = false;
}

SPtr<VideoPlaylist> VideoPlayer::getPlaylist()
{
    return playlist;
}

Mat VideoPlayer::getNextFrame()
{
    Mat image;
    VideoPlaylistFrame frame = VIDEO_PLAYLIST_FRAME_EOF;

    if(stopped)
    {
        return image;
    }

    do
    {
        frame = playlist->getNextFrame();

        if ((frame.sourceIndex != currentFrame.sourceIndex) && 
            (currentFrame.sourceIndex >= 0))
        {
            SPtr<IVideoSource> videoSource = playlist->getSourceByIndex(currentFrame.sourceIndex);
            if (videoSource != NULL)
            {
                videoSource->release();
            }
        }

        if (frame != VIDEO_PLAYLIST_FRAME_EOF)
        {
            SPtr<IVideoSource> videoSource = playlist->getSourceByIndex(frame.sourceIndex);
            if (videoSource != NULL)
            {
                if (frame == (++currentFrame))
                {
                    image = videoSource->getNextFrame();
                }
                else
                {
                    videoSource->seek(frame.frameIndex);
                    image = videoSource->getNextFrame();
                    currentFrame = frame;
                }
            }

            if (image.empty())
            {
                playlist->seekToNextRange();
            }
        }
    } while ((frame != VIDEO_PLAYLIST_FRAME_EOF) && (image.empty()));

    return image;
}

Mat VideoPlayer::getPreviousFrame()
{
    if(stopped)
    {
        return Mat();
    }

    VideoPlaylistFrame current = playlist->getCurrentFrame();
    if (current.sourceIndex == VIDEO_PLAYLIST_FRAME_END && current.frameIndex == VIDEO_PLAYLIST_FRAME_END)
    {
        return Mat();
    }
    playlist->getPreviousFrame();
    playlist->getPreviousFrame();
    return getNextFrame();
}

void VideoPlayer::seek(const VideoPlaylistFrame &frame)
{
    playlist->seek(frame);
    stopped = false;
}

bool VideoPlayer::haveNextFrame()
{
    return ( !stopped && playlist->haveNextFrame() );
}

void VideoPlayer::stop()
{
    stopped = true;
}

FilterType VideoPlayer::getRootType() const
{
    return VIDEO_PLAYER_FILTER;
}

bool VideoPlayer::getAutoExposureStatus()
{
    if (stopped)
    {
        return false;
    }

    VideoPlaylistFrame frame = playlist->getCurrentFrame();
    if (frame != VIDEO_PLAYLIST_FRAME_EOF)
    {
        SPtr<IVideoSource> videoSource = playlist->getSourceByIndex(frame.sourceIndex);
        if (videoSource != NULL)
        {
            return videoSource->getAutoExposureStatus();
        }
    }
    return false;
}

};
