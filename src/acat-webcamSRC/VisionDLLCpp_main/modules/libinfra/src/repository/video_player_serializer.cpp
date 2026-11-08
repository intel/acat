///////////////////////////////////////////////////////////////////////////
// <copyright file="video_player_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/video_player_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
VideoPlayerXMLSerializer::VideoPlayerXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

VideoPlayerXMLSerializer::~VideoPlayerXMLSerializer()
{
}

SPtr<Filter> VideoPlayerXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<VideoPlayer> videoPlayer = filter;
    if (videoPlayer == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to VideoPlayer for serialization");
        }
        videoPlayer = Factory::createVideoPlayer();
        filter = videoPlayer;
    }

    FilterXMLSerializer::load(fs, fn, videoPlayer);

    auto filterMap = loadMap(fs, fn);
    string playerId = filterMap["playlist_id"].str;
    if ("" != playerId)
    {
        videoPlayer->setPlaylist(rep->loadVideoPlaylist(playerId));
    }

    return videoPlayer;
}

void VideoPlayerXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<VideoPlayer> player = filter;
    if (player == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to VideoPlayer for serialization");
    }

    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);

    SPtr<VideoPlaylist> playlist = player->getPlaylist();

    if (NULL != playlist)
    {
        rep->save(playlist);
    }
}

map<string, IXMLSerializer::SerializeData> VideoPlayerXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    string playlistId = "";
    if (!fn["playlist_id"].empty())
    {
        fn["playlist_id"] >> playlistId;    
        filterMap["playlist_id"] = playlistId;
    }

    return filterMap;
}

void VideoPlayerXMLSerializer::saveMap(map<string, IXMLSerializer::SerializeData> &filterData, FileStorage &fs)
{
    FilterXMLSerializer::saveMap(filterData, fs);
}

map<string, IXMLSerializer::SerializeData> VideoPlayerXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    SPtr<VideoPlayer> videoPlayer = filter;
    if (videoPlayer == NULL)
    {
        return filterMap;
    }

    SPtr<VideoPlaylist> playlist = videoPlayer->getPlaylist();
    if (NULL != playlist)
    {
        filterMap["playlist_id"] = playlist->getId();
    }
    
    return filterMap;
}

}