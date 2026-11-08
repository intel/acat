///////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_creator_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/gt_region_creator_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
GTRegionCreatorXMLSerializer::GTRegionCreatorXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

GTRegionCreatorXMLSerializer::~GTRegionCreatorXMLSerializer()
{
}

SPtr<Filter> GTRegionCreatorXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<GTRegionCreator> regionCreator = filter;
    SPtr<IRegionDataSource> source = NULL;
    int regionClass = 0;

    if (regionCreator == NULL)
    {
        if (filter != NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast filter to GTRegionCreator");
        }
        regionCreator = Factory::createGTRegionCreator(source, regionClass);
        filter = regionCreator;
    }

    FilterXMLSerializer::load(fs, fn, filter);
    
    if (!fn["region_class"].empty())
    {
        fn["region_class"] >> regionClass;
    }

    if (!fn["region_data_source_id"].empty())
    {
        string sourceId = "";
        fn["region_data_source_id"] >> sourceId;
        source = rep->loadRegionDataSource(sourceId);
    }
    regionCreator->setParams(source, regionClass);

    SPtr<const VideoPlaylist> playlist = NULL;
    if (!fn["playlist_id"].empty())
    {
        string playlistId = "";
        fn["playlist_id"] >> playlistId;
        playlist = rep->loadVideoPlaylist(playlistId);
    }
    regionCreator->setPlaylist(playlist);

    return filter;
}

void GTRegionCreatorXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    SPtr<GTRegionCreator> regionCreator = filter;
    if (regionCreator == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to GTRegionCreator");
    }

    auto creatorMap = createMap(filter);
    saveMap(creatorMap, fs); 

    auto regions = regionCreator->getRegionDataSource();
    if (NULL != regions)
    {
        rep->save(regions);
    }

    SPtr<const VideoPlaylist> playlistConst = regionCreator->getPlaylist();
    SPtr<VideoPlaylist> playlist = *(reinterpret_cast<SPtr<VideoPlaylist> *>(&playlistConst));
    if (NULL != playlist)
    {
        rep->save(playlist);
    }
}

map<string, IXMLSerializer::SerializeData> GTRegionCreatorXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto creatorMap = FilterXMLSerializer::loadMap(fs, fn);

    if (!fn["region_class"].empty())
    {
        int regionClass = 0;
        fn["region_class"] >> regionClass;
        creatorMap["region_class"] = regionClass;
    }

    if (!fn["region_data_source_id"].empty())
    {
        string sourceId = "";
        fn["region_data_source_id"] >> sourceId;
        creatorMap["region_data_source_id"] = sourceId;
    }

    if (!fn["playlist_id"].empty())
    {
        string playlistId = "";
        fn["playlist_id"] >> playlistId;
        creatorMap["playlist_id"] = playlistId;
    }

    return creatorMap;
}

map<string, IXMLSerializer::SerializeData> GTRegionCreatorXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto creatorMap = FilterXMLSerializer::createMap(filter);
    
    SPtr<GTRegionCreator> regionCreator = filter;
    if (regionCreator == NULL)
    {
        return creatorMap;
    }

    SerializeData regClass(regionCreator->getRegionClass());
    creatorMap["region_class"] = regClass;

    auto regions = regionCreator->getRegionDataSource();
    if (NULL != regions)
    {
        creatorMap["region_data_source_id"] = regions->getId();
    }

    auto playlist = regionCreator->getPlaylist();
    if (NULL != playlist)
    {
        creatorMap["playlist_id"] = playlist->getId();
    }

    return creatorMap;
}

}
