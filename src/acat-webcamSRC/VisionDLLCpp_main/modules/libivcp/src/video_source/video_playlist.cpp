///////////////////////////////////////////////////////////////////////////
// <copyright file="video_playlist.cpp" company="Intel Corporation">
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



#include <algorithm>
#include "libivcp/video_source/video_playlist.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"

using namespace std;

namespace ivcp
{
VideoPlaylist::VideoPlaylist()
{
    clear();
}

VideoPlaylist::~VideoPlaylist()
{
	videoSources.clear();
}

void VideoPlaylist::addSourceRange(SPtr<IVideoSource> videoSource, const FrameRange &videoRange)
{
    VideoPlaylistRange plRange;
    plRange.sourceRange.firstFrame = max(0, videoRange.firstFrame);
    if(plRange.sourceRange.firstFrame >= videoRange.endFrame)
    {
        IVCP_LOG_WARNING("The range is empty and couldn't be added to the playlist");
        return;
    }
    plRange.sourceRange.endFrame = videoRange.endFrame;

    int sourceIndex = getIndexBySource(videoSource);

    if (sourceIndex < 0)
    {
        videoSources.push_back(videoSource);
        sourceIndex = (int)videoSources.size() - 1;
    }   

    plRange.sourceIndex = sourceIndex;

    videoFrameRanges.push_back(plRange);
}

void VideoPlaylist::clear()
{
    videoSources.clear();
    videoFrameRanges.clear();
    currentFrameIndex = -1;
    currentRangeIndex = 0; 
}

void VideoPlaylist::delSourceRange(int rangeIndex)
{
    if (rangeIndex < (int)videoFrameRanges.size() && rangeIndex >= 0)
    {
        videoFrameRanges.erase(videoFrameRanges.begin() + rangeIndex);
    }
}

bool VideoPlaylist::haveNextFrame() const
{
    int rangeIndex = currentRangeIndex;
    int frameIndex = currentFrameIndex;

    if ( ( rangeIndex < (int)videoFrameRanges.size() ) )
    {
        frameIndex++;
        while(frameIndex >= videoFrameRanges[rangeIndex].sourceRange.endFrame)//while instead of if to skip empty ranges
        {
            rangeIndex++;
            if ( rangeIndex >= (int)videoFrameRanges.size() )
            {
                return false;
            }
            frameIndex = videoFrameRanges[rangeIndex].sourceRange.firstFrame;
        }
        return true;
    }
    else
    {
        return false;
    }
}

VideoPlaylistFrame VideoPlaylist::getNextFrame()
{
    if ( ( currentRangeIndex < (int)videoFrameRanges.size() ) )
    {
        if (currentFrameIndex < 0)
        {
            currentFrameIndex = videoFrameRanges[0].sourceRange.firstFrame;
        }
        else
        {
            currentFrameIndex++;
        }

        //while instead of if to skip empty ranges
        while(currentFrameIndex >= videoFrameRanges[currentRangeIndex].sourceRange.endFrame)
        {
            currentRangeIndex++;
            if ( currentRangeIndex >= (int)videoFrameRanges.size() )
            {
                return VIDEO_PLAYLIST_FRAME_EOF;
            }
            currentFrameIndex = videoFrameRanges[currentRangeIndex].sourceRange.firstFrame;
        }
        return VideoPlaylistFrame(videoFrameRanges[currentRangeIndex].sourceIndex, 
                                     currentFrameIndex);
    }
    else
    {
        return VIDEO_PLAYLIST_FRAME_EOF;
    }
    
}

VideoPlaylistFrame VideoPlaylist::getPreviousFrame()
{
    if ( currentRangeIndex >= (int)videoFrameRanges.size() )
    {
        currentRangeIndex = (int)videoFrameRanges.size() - 1;
        if ( currentRangeIndex < 0 )
        {
            currentRangeIndex = 0;
            currentFrameIndex = -1;
            return VideoPlaylistFrame(videoFrameRanges[currentRangeIndex].sourceIndex, 
                                     currentFrameIndex);
        }
        currentFrameIndex = videoFrameRanges[currentRangeIndex].sourceRange.endFrame;
    }

    currentFrameIndex--;

    //while instead of if to skip empty ranges
    while(currentFrameIndex < videoFrameRanges[currentRangeIndex].sourceRange.firstFrame)
    {
        currentRangeIndex--;
        if ( currentRangeIndex < 0 )
        {
            currentRangeIndex = 0;
            currentFrameIndex = -1;
            return VideoPlaylistFrame(videoFrameRanges[currentRangeIndex].sourceIndex, 
                                     currentFrameIndex);
        }
        currentFrameIndex = videoFrameRanges[currentRangeIndex].sourceRange.endFrame - 1;
    }
    return VideoPlaylistFrame(videoFrameRanges[currentRangeIndex].sourceIndex, 
                                    currentFrameIndex);
}

VideoPlaylistFrame VideoPlaylist::getCurrentFrame() const
{
    if ( ( currentRangeIndex < (int)videoFrameRanges.size() ) &&
         ( currentFrameIndex >= videoFrameRanges[currentRangeIndex].sourceRange.firstFrame - 1 ) &&
         ( currentFrameIndex < videoFrameRanges[currentRangeIndex].sourceRange.endFrame ) )
    {
        VideoPlaylistFrame frame(videoFrameRanges[currentRangeIndex].sourceIndex, 
                                 currentFrameIndex);
        return frame;
    }
    else
    {
        return VIDEO_PLAYLIST_FRAME_EOF;
    }
}


SPtr<IVideoSource> VideoPlaylist::getSourceByIndex(int sourceIndex) const
{
    SPtr<IVideoSource> videoSource = 0;
    
    if (sourceIndex < (int)videoSources.size() && sourceIndex >= 0)
    {
        videoSource = videoSources[sourceIndex];
    }

    return videoSource;
}

const VideoPlaylistRange VideoPlaylist::getRangeByIndex(int rangeIndex) const
{   
    if (rangeIndex < (int)videoFrameRanges.size() && rangeIndex >= 0)
    {
        return videoFrameRanges[rangeIndex];
    }

    return VideoPlaylistRange();
}

int VideoPlaylist::getIndexBySource(SPtr<IVideoSource> videoSource) const
{
    std::deque <SPtr <IVideoSource> >::const_iterator it = 
        find(videoSources.begin(), videoSources.end(), videoSource);

    if (it != videoSources.end())
    {
        return (int)(it - videoSources.begin());
    }
    else
    {
        return -1;
    }
}

int VideoPlaylist::getNumberOfSources() const
{
    return (int)videoSources.size();
}

int VideoPlaylist::getNumberOfRanges() const
{
    return (int)videoFrameRanges.size();
}

void VideoPlaylist::seek(const VideoPlaylistFrame &frame)
{
    for (unsigned int i = 0; i < videoFrameRanges.size(); i++)
    {
        if ( (frame.sourceIndex == videoFrameRanges[i].sourceIndex) && 
             (frame.frameIndex >= videoFrameRanges[i].sourceRange.firstFrame) && 
             (frame.frameIndex < videoFrameRanges[i].sourceRange.endFrame) )
        {
            currentFrameIndex = frame.frameIndex - 1;
            currentRangeIndex = i;
            if (currentFrameIndex < videoFrameRanges[i].sourceRange.firstFrame)
            {
                if (currentRangeIndex > 0)
                {
                    currentRangeIndex--;
                    currentFrameIndex = videoFrameRanges[currentRangeIndex].sourceRange.endFrame - 1;         
                } 
                else 
                {
                    currentFrameIndex = -1;
                }
            }
            return;
        }
    };

    IVCP_EXCEPTION(IncorrectParamException, "No frame found");
}

void VideoPlaylist::seek(int rangeIndex, int frameIndex)
{
    if ( ( rangeIndex < (int)videoFrameRanges.size() ) && ( rangeIndex >= 0 ) &&
         ( frameIndex >= videoFrameRanges[rangeIndex].sourceRange.firstFrame ) &&
         ( frameIndex < videoFrameRanges[rangeIndex].sourceRange.endFrame ) )
    {
        currentRangeIndex = rangeIndex;
        currentFrameIndex = frameIndex - 1;
    } 
    else 
    {
        IVCP_EXCEPTION(IncorrectParamException, "No frame found");    
    }
}

VideoPlaylistFrame VideoPlaylist::seekToNextRange()
{
    if ((videoFrameRanges.empty()) || 
        (currentRangeIndex >= (int)videoFrameRanges.size()) || 
        (currentRangeIndex < 0))
    {
        return VIDEO_PLAYLIST_FRAME_EOF;
    }

    currentFrameIndex = videoFrameRanges[currentRangeIndex].sourceRange.endFrame - 1;

    return VideoPlaylistFrame(videoFrameRanges[currentRangeIndex].sourceIndex, currentFrameIndex);
}

FilterType VideoPlaylist::getRootType() const
{
    return VIDEO_PLAYLIST_FILTER;
}

}
