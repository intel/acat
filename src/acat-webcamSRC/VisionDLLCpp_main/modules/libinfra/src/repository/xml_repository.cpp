///////////////////////////////////////////////////////////////////////////
// <copyright file="xml_repository.cpp" company="Intel Corporation">
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

#include <sstream>
#include "libinfra/repository/xml_repository.hpp"
#include "libivcp/factory.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
XMLRepository::XMLRepository(std::string _filename)
{
    storageLocked = false;
    if (!_filename.empty())
    {
        setUrl(_filename);
    }
}

XMLRepository::~XMLRepository()
{
}

SPtr<Filter> XMLRepository::load(string id)
{
    return this->loadFirstMatching(-1, -1, id);
}

SPtr<IClassifier> XMLRepository::loadClassifier(string id)
{
    return this->loadFirstMatching((int)ivcp::CLASSIFIER_FILTER, -1, id);
}

SPtr<IRegionDataSource> XMLRepository::loadRegionDataSource(string id)
{
    SPtr<IFilter> source = this->loadFirstMatching((int)GROUNDTRUTH_REGIONS_FILTER, -1, id);
    if (source == NULL)
    {
        source = this->loadFirstMatching((int)DATA_SOURCE_OBSOLETE, GROUNDTRUTH_REGIONS_OBSOLETE, id);
    }
    return source;
}

SPtr<IClassifierResultDataSource> XMLRepository::loadClassifierResultDataSource(string id)
{
    SPtr<IFilter> source = this->loadFirstMatching((int)ivcp::CLASSIFIER_RESULTS_FILTER, -1, id);
    if (source == NULL)
    {
        source = this->loadFirstMatching((int)DATA_SOURCE_OBSOLETE, CLASSIFIER_RESULT_OBSOLETE, id);
    }
    return source;
}

SPtr<IFilter> XMLRepository::loadDataSource(string id)
{
    SPtr<IFilter> source = this->loadFirstMatching((int)DATA_SOURCE_OBSOLETE, -1, id);
    if (source == NULL)
    {
        source = this->loadFirstMatching((int)GROUNDTRUTH_REGIONS_FILTER, -1, id);
    }
    if (source == NULL)
    {
        source = this->loadFirstMatching((int)ivcp::CLASSIFIER_RESULTS_FILTER, -1, id);
    }
    return source;
}

SPtr<IFeature> XMLRepository::loadFeature(string id)
{
    return this->loadFirstMatching((int)ivcp::FEATURE_FILTER, -1, id);
}

SPtr<IPerformance> XMLRepository::loadPerformance(string id)
{
    return this->loadFirstMatching((int)ivcp::PERFORMANCE_FILTER, -1, id);
}

SPtr<IRegionCreator> XMLRepository::loadRegionCreator(string id)
{
    return this->loadFirstMatching((int)ivcp::REGION_CREATOR_FILTER, -1, id);
}

SPtr<IVideoSource> XMLRepository::loadVideoSource(string id)
{
    return this->loadFirstMatching((int)ivcp::VIDEO_SOURCE_FILTER, -1, id);
}

SPtr<IVideoPlayer> XMLRepository::loadVideoPlayer(string id)
{
    return this->loadFirstMatching((int)ivcp::VIDEO_PLAYER_FILTER, -1, id);
}

SPtr<VideoPlaylist> XMLRepository::loadVideoPlaylist(string id)
{
    return this->loadFirstMatching((int)ivcp::VIDEO_PLAYLIST_FILTER, -1, id);
}

SPtr<Session> XMLRepository::loadSession(string id)
{
    return this->loadFirstMatching((int)ivcp::SESSION_FILTER, -1, id);
}

SPtr<IPipeline> XMLRepository::loadPipeline(string id)
{
    return this->loadFirstMatching((int)ivcp::PIPELINE_FILTER, -1, id);
}

SPtr<IPipeline> XMLRepository::loadParallelPipeline(string id)
{
    return this->loadFirstMatching((int)ivcp::PARALLEL_PIPELINE_FILTER, -1, id);
}

void XMLRepository::save(SPtr<Filter> filter)
{
    if (!storageLocked)
    {
        storageLocked = true;

        map<string, map<string, IXMLSerializer::SerializeData> > fromIdToContentMap = this->loadAllNodeMaps();

        FileStorage fs(filename, FileStorage::WRITE);
        string filterId = filter->getId();
        int filterTypeId = getFilterTypeId(filter);
        if (fromFilterIdToSerializer.find(filterTypeId) != fromFilterIdToSerializer.end())
        {
            fs << "objects" << "[";
            //Save our object first
            SPtr<IXMLSerializer> serializer = fromFilterIdToSerializer[filterTypeId];
            fs << "{" << "id" << filterId << "type" << filterTypeId << "internals" << "{";
            serializer->save(filter, fs);
            fs << "}" << "}";

            //Then all previously read objects
            for (auto it = fromIdToContentMap.begin(); it != fromIdToContentMap.end(); it++)
            {
                //In case the prev version already has this filter - ignore the previous version as we
                //have the newest one
                if (it->first == filterId)
                {
                    continue;
                }
                if (fromFilterIdToSerializer.find(it->second["type"].ival) != fromFilterIdToSerializer.end())
                {
                    fs << "{" << "id" << it->first << "type" << it->second["type"].ival << "internals" << "{";
                    SPtr<IXMLSerializer> serializerIt = fromFilterIdToSerializer[it->second["type"].ival];
                    serializerIt->saveMap(it->second, fs);                    
                    fs << "}" << "}";
                }
                else
                {
                    stringstream msg;
                    msg << "Serializer for type " << it->second["type"].ival << " is not registered, ignoring";
                    IVCP_LOG_WARNING(msg.str());
                }
            }
            fs << "]";
        }
        else
        {                    
            fs.release();
            stringstream msg;
            msg << "Serializer for filter with id = " << filterId << " is not implemented";
            IVCP_EXCEPTION(NotImplementedException, msg.str());
        }
        fs.release();

        storageLocked = false;
    }
    else
    {
        objectsSaveQueue.push_back(filter);
    }

    if (!storageLocked)
    {
        list<SPtr<Filter> > objQueue = objectsSaveQueue;
        objectsSaveQueue.clear();

        for (list<SPtr<Filter> >::iterator it = objQueue.begin(); 
            it != objQueue.end(); ++it)
        {
            this->save(*it);
        }
    }
}

int XMLRepository::getFilterTypeId(int rootId, int filterId)
{
    return filterId + (rootId << numberShiftBits);
}

int XMLRepository::getFilterTypeId(SPtr<Filter> filter)
{
    int rtype = (int)filter->getRootType();
    int ftype = 0;

    try
    {
        switch (rtype)
        {
            case CLASSIFIER_FILTER:
                ftype = (int)SPtr<IClassifier>(filter)->getType();
                break;
            case CLASSIFIER_RESULTS_FILTER:          
                ftype = (int)SPtr<IClassifierResultDataSource>(filter)->getType();
                break;
            case GROUNDTRUTH_REGIONS_FILTER:          
                ftype = (int)SPtr<IRegionDataSource>(filter)->getType();
                break;
            case FEATURE_FILTER:
                ftype = (int)SPtr<IFeature>(filter)->getType();
                break;
            case PERFORMANCE_FILTER:
                ftype = (int)SPtr<IPerformance>(filter)->getType();
                break;
            case REGION_CREATOR_FILTER:
                ftype = (int)SPtr<IRegionCreator>(filter)->getType();
                break;
            case VIDEO_SOURCE_FILTER:
                ftype = (int)SPtr<IVideoSource>(filter)->getType();
                break;
            case VIDEO_PLAYER_FILTER: 
            case VIDEO_PLAYLIST_FILTER:
            case SESSION_FILTER:
            case PIPELINE_FILTER:
            case PARALLEL_PIPELINE_FILTER:
                ftype = 0;
                break;
            default:
                IVCP_EXCEPTION(CastException, "Unable to cast filter to descenant class");
                break;
        }
    }
    catch (...)
    {
        IVCP_EXCEPTION(CastException, "Unable to cast filter to descenant class");
    }

    return getFilterTypeId(rtype, ftype);
}

void XMLRepository::registerSerializer(SPtr<Filter> filter, SPtr<IXMLSerializer> serializer)
{
    int filterId = getFilterTypeId(filter);
    registerSerializer(filterId, serializer);
}

void XMLRepository::registerSerializer(int filterId, SPtr<IXMLSerializer> serializer)
{
    fromFilterIdToSerializer[filterId] = serializer;
}

void XMLRepository::setUrl(string _filename)
{
    filename = _filename;
}

string XMLRepository::getUrl()
{
    return filename;
}

map<string, IXMLSerializer::FilterState > XMLRepository::loadAllNodeMaps()
{
    map<string, IXMLSerializer::FilterState > allNodeMaps;

    FileStorage fs(filename, FileStorage::READ);
    if (!fs.isOpened())
    {
        //No file - empty map
        return allNodeMaps;
    }

    FileNode objects = fs["objects"];
    FileNodeIterator itObjects = objects.begin();

    for (itObjects = objects.begin(); itObjects != objects.end(); itObjects++)
    {
        string objectId;
        int serializerIndex;
        if (!(*itObjects)["id"].empty())
        {
            (*itObjects)["id"] >> objectId;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read objects from Repository: missing tag id");
        }

        if (!(*itObjects)["type"].empty())
        {
            (*itObjects)["type"] >> serializerIndex;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read objects from Repository: missing tag type");
        }

        if (fromFilterIdToSerializer.find(serializerIndex) != fromFilterIdToSerializer.end())
        {
            allNodeMaps[objectId] = fromFilterIdToSerializer[serializerIndex]->loadMap(fs, (*itObjects)["internals"]);
            allNodeMaps[objectId]["type"] = IXMLSerializer::SerializeData(IXMLSerializer::SerializeData::INT, serializerIndex);
        }
        else
        {
            stringstream msg;
            msg << "Serializer for type " << serializerIndex << " is not registered";
            IVCP_LOG(LOG_WARNING, msg.str());
        }
    }

    return allNodeMaps;
}

int XMLRepository::getRootIdByType(int type)
{
    return (type >> numberShiftBits);
}

int XMLRepository::getFilterIdByType(int type)
{
    int numBits = (sizeof(type) * 8) - numberShiftBits;
    return ((type << numBits) >> numBits);
}

SPtr<Filter> XMLRepository::loadFirstMatching(int rootType, int filterType, string id)
{
    FileStorage fs(filename, FileStorage::READ);
    if (!fs.isOpened())
    {
        stringstream buffer;
        buffer << "Unable to open file " << filename;
        IVCP_EXCEPTION(IOException, buffer.str());
    }

    FileNode objects = fs["objects"];
    FileNodeIterator itObject = objects.begin();

    for (itObject = objects.begin(); itObject != objects.end(); itObject++)
    {
        string nodeId = "";
        int nodeType = -1;
        int rootNodeType = -1;
        int filterNodeType = -1;

        if (!(*itObject)["id"].empty())
        {
            (*itObject)["id"] >> nodeId;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read objects from Repository: missing tag id");
        }

        if (!(*itObject)["type"].empty())
        {
            (*itObject)["type"] >> nodeType;
        }
        else
        {
            IVCP_EXCEPTION(IOException, "Unable to read objects from Repository: missing tag type");
        }
        
        if (nodeType < 0)
        {
            continue;
        }

        rootNodeType = getRootIdByType(nodeType);
        filterNodeType = getFilterIdByType(nodeType);

        if ((rootNodeType != rootType) && (rootType != -1))
        {
            continue;
        }

        if ((filterNodeType != filterType) && (filterType != -1))
        {
            continue;
        }

        if ((nodeId != id) && !id.empty())
        {
            continue;
        }

        //If already have loaded the filter return a pointer from the cache
        if (fromFilterIdToFilterCache.find(nodeId) != fromFilterIdToFilterCache.end())
        {
            SPtr<IFilter> ptr = fromFilterIdToFilterCache[nodeId];
            if (!ptr.isEmpty())
            {
                return ptr;
            }
            else
            {
                //The object is already deallocated
                fromFilterIdToFilterCache.erase(nodeId);
            }
        }

        if (fromFilterIdToSerializer.find(nodeType) != fromFilterIdToSerializer.end())
        {
            //Match found
            SPtr<IXMLSerializer> serializer = fromFilterIdToSerializer[nodeType];
            SPtr<Filter> filter = NULL;
            try
            {
                filter = serializer->load(fs, (*itObject)["internals"]);
            }
            catch (IncorrectParamException)
            {
                stringstream message;
                message << "Unable to load filter with the following parameters: rootType = " << rootType << " filterType = " << filterType << " id = " << id;
                IVCP_EXCEPTION(NoDataException, message.str());
            }
            fromFilterIdToFilterCache[nodeId] = filter;
            return filter;
        }
        else
        {
            stringstream msg;
            msg << "Serializer for type " << nodeType << " is not registered";
            IVCP_LOG(LOG_WARNING, msg.str());
        }
    }

    //Match not found
    return NULL;
}

const unsigned int XMLRepository::numberShiftBits = 16;

}
