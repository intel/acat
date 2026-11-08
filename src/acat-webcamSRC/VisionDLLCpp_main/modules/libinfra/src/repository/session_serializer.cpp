///////////////////////////////////////////////////////////////////////////
// <copyright file="session_serializer.cpp" company="Intel Corporation">
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

#include "libinfra/repository/session_serializer.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
SessionXMLSerializer::SessionXMLSerializer(SPtr<IRepository> _rep):
    FilterXMLSerializer(_rep)
{
}

SessionXMLSerializer::~SessionXMLSerializer()
{
}

SPtr<Filter> SessionXMLSerializer::load(FileStorage &fs, FileNode fn, SPtr<Filter> filter)
{
    SPtr<Session> session = filter;
    if (session == NULL)
    {
        if (NULL != filter)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Unable to cast Filter to IPipeline for serialization");
        }
        session = Factory::createSession();
        filter = session;
    }

    FilterXMLSerializer::load(fs, fn, session);

    auto filterMap = loadMap(fs, fn);

    vector<string> sourceIds = filterMap["source_ids"].strVec;
    vector<string> playerIds = filterMap["player_ids"].strVec;
    vector<string> regionCreatorIds = filterMap["region_creator_ids"].strVec;
    vector<string> featureIds = filterMap["feature_ids"].strVec;
    vector<string> classifierIds = filterMap["classifier_ids"].strVec;
    vector<string> performanceIds = filterMap["performance_ids"].strVec;
    vector<string> dataSourceIds = filterMap["data_source_ids"].strVec;

    //Add filters
    unsigned int i = 0;
    for (i = 0; i < sourceIds.size(); i++)
    {
        session->addVideoSource(rep->loadVideoSource(sourceIds[i]));
    }

    for (i = 0; i < playerIds.size(); i++)
    {
        SPtr<VideoPlayer> player = rep->loadVideoPlayer(playerIds[i]);
        session->setVideoPlayer(player);
    }

    for (i = 0; i < regionCreatorIds.size(); i++)
    {
        session->addRegionCreator(rep->loadRegionCreator(regionCreatorIds[i]));
    }

    for (i = 0; i < featureIds.size(); i++)
    {
        session->addFeature(rep->loadFeature(featureIds[i]));
    }

    for (i = 0; i < classifierIds.size(); i++)
    {
        session->addClassifier(rep->loadClassifier(classifierIds[i]));
    }

    for (i = 0; i < performanceIds.size(); i++)
    {
        session->addPerformanceFilter(rep->loadPerformance(performanceIds[i]));
    }

    for (i = 0; i < dataSourceIds.size(); i++)
    {
        session->addDataSource(rep->loadDataSource(dataSourceIds[i]));
    }

    return session;
}

void SessionXMLSerializer::save(SPtr<Filter> filter, FileStorage &fs)
{
    auto filterMap = createMap(filter);
    saveMap(filterMap, fs);

    SPtr<Session> session = filter;
    if  (session == NULL)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast Filter to Session for serialization");
    }

    vector<string> filterIds = filterMap["source_ids"].strVec;
    for (auto it = filterIds.begin(); it != filterIds.end(); it++)
    {
        rep->save(session->getVideoSource(*it));
    }

    rep->save(session->getVideoPlayer());

    filterIds = filterMap["region_creator_ids"].strVec;
    for (auto it = filterIds.begin(); it != filterIds.end(); it++)
    {
        rep->save(session->getRegionCreator(*it));
    }

    filterIds = filterMap["feature_ids"].strVec;
    for (auto it = filterIds.begin(); it != filterIds.end(); it++)
    {
        rep->save(session->getFeature(*it));
    }

    filterIds = filterMap["classifier_ids"].strVec;
    for (auto it = filterIds.begin(); it != filterIds.end(); it++)
    {
        rep->save(session->getClassifier(*it));
    }

    filterIds = filterMap["performance_ids"].strVec;
    for (auto it = filterIds.begin(); it != filterIds.end(); it++)
    {
        rep->save(session->getPerformance(*it));
    }

    filterIds = filterMap["data_source_ids"].strVec;
    for (auto it = filterIds.begin(); it != filterIds.end(); it++)
    {
        rep->save(session->getDataSource(*it));
    }
}

map<string, IXMLSerializer::SerializeData> SessionXMLSerializer::loadMap(FileStorage &fs, FileNode fn)
{
    auto filterMap = FilterXMLSerializer::loadMap(fs, fn);

    vector<string> data;
    loadStrVec(data, "source_ids", "id", fs, fn);
    filterMap["source_ids"] = data;
    loadStrVec(data, "player_ids", "id", fs, fn);
    filterMap["player_ids"] = data;
    loadStrVec(data, "region_creator_ids", "id", fs, fn);
    filterMap["region_creator_ids"] = data;
    loadStrVec(data, "feature_ids", "id", fs, fn);
    filterMap["feature_ids"] = data;
    loadStrVec(data, "classifier_ids", "id", fs, fn);
    filterMap["classifier_ids"] = data;
    loadStrVec(data, "performance_ids", "id", fs, fn);
    filterMap["performance_ids"] = data;
    loadStrVec(data, "data_source_ids", "id", fs, fn);
    filterMap["data_source_ids"] = data;

    return filterMap;
}

void SessionXMLSerializer::saveMap(map<string, SerializeData> &filterData, FileStorage &fs)
{
    saveStrVec(filterData["source_ids"].strVec, "source_ids", "id", fs);
    saveStrVec(filterData["player_ids"].strVec, "player_ids", "id", fs);
    saveStrVec(filterData["region_creator_ids"].strVec, "region_creator_ids", "id", fs);
    saveStrVec(filterData["feature_ids"].strVec, "feature_ids", "id", fs);
    saveStrVec(filterData["classifier_ids"].strVec, "classifier_ids", "id", fs);
    saveStrVec(filterData["performance_ids"].strVec, "performance_ids", "id", fs);
    saveStrVec(filterData["data_source_ids"].strVec, "data_source_ids", "id", fs);

    map<string, SerializeData> filterDataCopy = filterData;
    filterDataCopy.erase("source_ids");
    filterDataCopy.erase("player_ids");
    filterDataCopy.erase("region_creator_ids");
    filterDataCopy.erase("feature_ids");
    filterDataCopy.erase("classifier_ids");
    filterDataCopy.erase("performance_ids");
    filterDataCopy.erase("data_source_ids");

    FilterXMLSerializer::saveMap(filterDataCopy, fs);
}

map<string, IXMLSerializer::SerializeData> SessionXMLSerializer::createMap(SPtr<Filter> filter)
{
    auto filterMap = FilterXMLSerializer::createMap(filter);
    
    SPtr<Session> session = filter;
    if (session == NULL)
    {
        return filterMap;
    }
 
    int numberOfSources = session->getNumberOfVideoSources();
    int numberOfRegions = session->getNumberOfRegionCreators();
    int numberOfFeatures = session->getNumberOfFeatures();
    int numberOfClassifiers = session->getNumberOfClassifiers();
    int numberOfPerformanceFilters = session->getNumberOfPerformanceFilters();
    int numberOfDataSources = session->getNumberOfDataSources();
    int numberOfPlayers = 1;

    vector<string> sourceIds(numberOfSources), 
        playerIds(numberOfPlayers), 
        regionCreatorIds(numberOfRegions), 
        featureIds(numberOfFeatures), 
        classifierIds(numberOfClassifiers), 
        performanceIds(numberOfPerformanceFilters), 
        datasourceIds(numberOfDataSources); 

    int f = 0;
    for (f = 0; f < numberOfSources; f++)
    {
        sourceIds[f] = session->getVideoSource(f)->getId();
    }

    for (f = 0; f < numberOfRegions; f++)
    {
        regionCreatorIds[f] = session->getRegionCreator(f)->getId();
    }

    for (f = 0; f < numberOfFeatures; f++)
    {
        featureIds[f] = session->getFeature(f)->getId();
    }

    for (f = 0; f < numberOfClassifiers; f++)
    {
        classifierIds[f] = session->getClassifier(f)->getId();
    }

    for (f = 0; f < numberOfPerformanceFilters; f++)
    {
        performanceIds[f] = session->getPerformance(f)->getId();
    }

    for (f = 0; f < numberOfDataSources; f++)
    {
        datasourceIds[f] = session->getDataSource(f)->getId();
    }

    for (f = 0; f < numberOfPlayers; f++)
    {
        playerIds[f] = session->getVideoPlayer()->getId();
    }

    filterMap["source_ids"] = sourceIds;
    filterMap["player_ids"] = playerIds;
    filterMap["region_creator_ids"] = regionCreatorIds;
    filterMap["feature_ids"] = featureIds;
    filterMap["classifier_ids"] = classifierIds;
    filterMap["performance_ids"] = performanceIds;
    filterMap["data_source_ids"] = datasourceIds;

    return filterMap;
}

}
