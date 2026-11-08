///////////////////////////////////////////////////////////////////////////
// <copyright file="video_playlist_frame.cpp" company="Intel Corporation">
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



#include "libivcp/video_source/video_playlist_frame.hpp"

namespace ivcp
{
VideoPlaylistFrame::VideoPlaylistFrame(int _sourceIndex, int _frameIndex):
    sourceIndex(_sourceIndex), frameIndex(_frameIndex)
{
}

bool VideoPlaylistFrame::operator== (const VideoPlaylistFrame &frame)
{
    return (sourceIndex == frame.sourceIndex) && (frameIndex == frame.frameIndex);
}

bool VideoPlaylistFrame::operator!= (const VideoPlaylistFrame &frame)
{
    return !(this->operator == (frame));
}

VideoPlaylistFrame& VideoPlaylistFrame::operator++()
{
    frameIndex++;
    return *this;
}
}
