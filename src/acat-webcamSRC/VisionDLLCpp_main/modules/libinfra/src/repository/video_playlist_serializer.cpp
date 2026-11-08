///////////////////////////////////////////////////////////////////////////
// <copyright file="video_playlist_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/video_playlist_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
VideoPlaylistXMLSerializer::VideoPlaylistXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

VideoPlaylistXMLSerializer::~VideoPlaylistXMLSerializer()
{
}

SPtr<Filter> VideoPlaylistXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<VideoPlaylist> videoPlaylist = filter;
    if (videoPlaylist == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to VideoPlaylist for serialization");
        }
        videoPlaylist = Factory::createVideoPlaylist();
        filter = videoPlaylist;
    }

    FilterXMLSerializer::load(fs, fn, videoPlaylist);

    auto filterMap = loadMap(fs, fn);
    Mat videoRanges = filterMap["video_ranges"].mat;
    vector<string> videoIds = filterMap["video_ids"].strVec;

    vector<SPtr<IVideoSource> > videoSources(videoIds.size());
    for (unsigned int i = 0; i < videoIds.size(); i++)
    {
        SPtr<IVideoSource> source = rep->loadVideoSource(videoIds[i]);
        videoSources[i] = source;
    }

    for (int r = 0; r < videoRanges.rows; r++)
    {
        int sourceIndex = videoRanges.at<int>(r, 0);
        FrameRange range(videoRanges.at<int>(r,1), videoRanges.at<int>(r,2));

        videoPlaylist->addSourceRange(videoSources[sourceIndex], range);
    }

    return videoPlaylist;
}

void VideoPlaylistXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<VideoPlaylist> playlist = filter;
    if (playlist == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to VideoPlaylist for serialization");
    }

    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);

    int numberOfSources = playlist->getNumberOfSources();

    for (int s = 0; s < numberOfSources; s++)
    {
        rep->save(playlist->getSourceByIndex(s));
    }
}

map<string, IXMLSerializer::SerializeData> VideoPlaylistXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    size_t numberOfRanges = fn["video_ranges"].size();
    Mat videoRanges;
    if (numberOfRanges > 0)
    {
        videoRanges = Mat((int)numberOfRanges, 3, CV_32SC1);

        int counter = 0;
        for (FileNodeIterator it = fn["video_ranges"].begin(); 
            it != fn["video_ranges"].end(); it++, counter++)
        {
            int sourceIndex = 0;
            int firstFrame = 0;
            int lastFrame = 0;

            if ((*it)["source_index"].empty() || (*it)["first_frame"].empty() || (*it)["last_frame"].empty())
            {
                IVCP_EXCEPTION(IOException, "Unable to read playlist. Data is possibly broken");
            }
            
            (*it)["source_index"] >> sourceIndex;
            (*it)["first_frame"] >> firstFrame;
            (*it)["last_frame"] >> lastFrame;

            videoRanges.at<int>(counter, 0) = sourceIndex;
            videoRanges.at<int>(counter, 1) = firstFrame;
            videoRanges.at<int>(counter, 2) = lastFrame;
        }
    }

    vector<string> videoIds;
    for (FileNodeIterator it = fn["video_ids"].begin(); 
        it != fn["video_ids"].end(); it++)
    {
        string id;
        if (!(*it)["id"].empty())
        {
            (*it)["id"] >> id;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read video sources. Data is possibly broken");
        }

        videoIds.push_back(id);
    }

    filterMap["video_ids"] = videoIds;
    filterMap["video_ranges"] = videoRanges;

    return filterMap;   
}

void VideoPlaylistXMLSerializer::saveMap(map<string, IXMLSerializer::SerializeData> &filterData, FileStorage &fs)
{
    if (filterData.find("video_ids") != filterData.end())
    {
        vector<string> videoIds = filterData["video_ids"].strVec;

        if (!videoIds.empty())
        {
            fs << "video_ids" << "[";
            for (unsigned int i = 0; i < videoIds.size(); i++)
            {
                fs << "{";
                fs << "id" << videoIds[i];
                fs << "}";
            }
            fs << "]";
        }

    }

    if (filterData.find("video_ranges") != filterData.end())
    {
        Mat videoRanges = filterData["video_ranges"].mat;

        if (videoRanges.rows > 0)
        {
            fs << "video_ranges" << "[";
            for (int i = 0; i < videoRanges.rows; i++)
            {
                fs << "{";
                fs << "source_index" << videoRanges.at<int>(i, 0);
                fs << "first_frame" << videoRanges.at<int>(i, 1);
                fs << "last_frame" << videoRanges.at<int>(i, 2);
                fs << "}";
            }
            fs << "]"; 
        }
    }

    map<string, SerializeData> featureData = filterData;
    featureData.erase("video_ids");
    featureData.erase("video_ranges");

    FilterXMLSerializer::saveMap(featureData, fs);
}

map<string, IXMLSerializer::SerializeData> VideoPlaylistXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);

    //Map consists of 2 elements:
    //VideoSources ids
    //VideoRanges: <vs index>, <r_start>, <r_stop>
    SPtr<VideoPlaylist> playlist = filter;

    int numberOfSources = playlist->getNumberOfSources();
    int numberOfRanges = playlist->getNumberOfRanges();
 
    Mat videoRanges(numberOfRanges, 3, CV_32SC1);
    vector<string> videoIds(numberOfSources);

    for (int i = 0; i < numberOfSources; i++)
    {
        auto source = playlist->getSourceByIndex(i);
        videoIds[i] = source->getId();
    }

    for (int r = 0; r < numberOfRanges; r++)
    {
        VideoPlaylistRange range = playlist->getRangeByIndex(r);

        videoRanges.at<int>(r, 0) = range.sourceIndex;
        videoRanges.at<int>(r, 1) = range.sourceRange.firstFrame;
        videoRanges.at<int>(r, 2) = range.sourceRange.endFrame;
    }

    filterMap["video_ranges"] = videoRanges;
    filterMap["video_ids"] = videoIds;

    return filterMap;
}

}
