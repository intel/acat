///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline.cpp" company="Intel Corporation">
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
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/region_creator/gt_region_creator.hpp"
#include "libpipeline/pipeline.hpp"
#include "libpipeline/pipeline_source_node.hpp"
#include "libpipeline/pipeline_region_creator_node.hpp"
#include "libpipeline/pipeline_feature_node.hpp"
#include "libpipeline/pipeline_classifier_node.hpp"

using namespace cv;
using namespace std;

namespace ivcp
{
Pipeline::Pipeline(PipelineMode mode, Settings settings)
{
    currentMode = mode;
    playlist = NULL;
    currentSession = NULL;
    pipelineSettings = settings;
}

Pipeline::Pipeline(SPtr<Session> session, PipelineMode mode, Settings settings)
{
    currentMode = mode;
    playlist = NULL;
    currentSession = NULL;
    pipelineSettings = settings;
    setSession(session);
}

PipelineMode Pipeline::getMode()
{
    return currentMode;
}

void Pipeline::setSession(SPtr<Session >session)
{
    currentSession = session;
}

SPtr<Session > Pipeline::getSession()
{
    return currentSession;
}

bool Pipeline::areConnectable(FilterType sourceType, FilterType destinationType)
{
    switch( sourceType )
    {
    case VIDEO_PLAYER_FILTER:
        return ( REGION_CREATOR_FILTER == destinationType || destinationType == PERFORMANCE_FILTER );
    case REGION_CREATOR_FILTER:
        return ( FEATURE_FILTER == destinationType || destinationType == PERFORMANCE_FILTER );
    case FEATURE_FILTER:
        return ( CLASSIFIER_FILTER == destinationType || destinationType == PERFORMANCE_FILTER );
    case CLASSIFIER_FILTER:
        return ( PERFORMANCE_FILTER == destinationType );
        break;
    case CLASSIFIER_RESULTS_FILTER:
        return (destinationType == CLASSIFIER_FILTER || destinationType == PERFORMANCE_FILTER);
        break;
    case PERFORMANCE_FILTER:
        return false;
        break;
    case VIDEO_SOURCE_FILTER:
    case SESSION_FILTER:
    default:
        return false;
        break;
    }
}

void Pipeline::connectAll()
{
    if (currentSession == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Session is not defined for current pipeline");
    }

    if (currentSession->getNumberOfPerformanceFilters() > 1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Could not perform automatic linkage. There are several performance filters");
    }

    if (currentSession->getNumberOfClassifiers() > 1)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Could not perform automatic linkage. There are several classifier filters");
    }

    if (currentSession->getNumberOfFeatures() > 1)
    {
        if (getMode() != TRAIN_PIPELINE)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Could not perform automatic linkage. There are several feature filters");
        }
        else
        {
            IVCP_LOG_WARNING("There are several feature filters defined. All feature filters will be connected to classifier");
        }
    }

    if (getNumberOfLinks() > 0)
    {
        IVCP_LOG_INFO("Pipeline contains previously created links. New links will be added to already existing.");
    }

    if (currentSession->getNumberOfPerformanceFilters() > 0)
    {
        addLink(currentSession->getVideoPlayer()->getId(), currentSession->getPerformance(0)->getId());
    }

    if (currentSession->getNumberOfRegionCreators() > 1 && currentSession->getNumberOfFeatures() > 0)
    {
        IVCP_LOG_WARNING("There are several region creators defined. All region creators will be connected to features");
    }

    if (currentSession->getNumberOfFeatures() <= 0)
    {
        IVCP_LOG_WARNING("Automatic linkage issue. No Features in the Session");
    }

    if (currentSession->getNumberOfRegionCreators() <= 0)
    {
        IVCP_LOG_WARNING("Automatic linkage issue. No Region Creators in the Session");
    }

    if (currentSession->getNumberOfClassifiers() <= 0)
    {
        IVCP_LOG_WARNING("Automatic linkage issue. No Classifiers in the Session");
    }

    if (((getMode() == TEST_PIPELINE) || (getMode() == TRAIN_PIPELINE)) && 
        (currentSession->getNumberOfDataSources() <= 0))
    {
        IVCP_EXCEPTION(IncorrectParamException, "Could not perform automatic linkage. No Data Sources in the Session");
    }

    for (int i = 0; i < currentSession->getNumberOfRegionCreators(); i++)
    {
        addLink(currentSession->getVideoPlayer()->getId(), currentSession->getRegionCreator(i)->getId());
        if (currentSession->getNumberOfPerformanceFilters() > 0)
        {
            addLink(currentSession->getRegionCreator(i)->getId(), currentSession->getPerformance(0)->getId());
        }
        for(int j = 0; j < currentSession->getNumberOfFeatures(); j++)
        {
            addLink(currentSession->getRegionCreator(i)->getId(), currentSession->getFeature(j)->getId());
        }
    }

    for (int i = 0; i < currentSession->getNumberOfFeatures(); i++)
    {
        if (currentSession->getNumberOfPerformanceFilters() > 0)
        {
            addLink(currentSession->getFeature(i)->getId(), currentSession->getPerformance(0)->getId());
        }
        if (currentSession->getNumberOfClassifiers() > 0)
        {
            addLink(currentSession->getFeature(i)->getId(), currentSession->getClassifier(0)->getId());
        }
    }

    if (currentSession->getNumberOfPerformanceFilters() > 0 && currentSession->getNumberOfClassifiers() > 0)
    {
        addLink(currentSession->getClassifier(0)->getId(), currentSession->getPerformance(0)->getId());
    }

    SPtr<IClassifierResultDataSource> dataSource;
    for (int i = 0; i < currentSession->getNumberOfDataSources(); i++)
    {
        SPtr<IFilter > temp = currentSession->getDataSource(i);
        if (temp->getRootType() == CLASSIFIER_RESULTS_FILTER)
        {
            if (dataSource.isEmpty())
            {
                dataSource = temp;
            }
            else
            {
                IVCP_EXCEPTION(IncorrectParamException, "Could not perform automatic linkage. There are several classifier data sources");
            }
        }
    }

    if (!dataSource.isEmpty())
    {
        if (getMode() == TRAIN_PIPELINE)
        {
            if (currentSession->getNumberOfClassifiers() > 0)
            {
                addLink(dataSource->getId(), currentSession->getClassifier(0)->getId());
            }
        }
        if (getMode() == TEST_PIPELINE)
        {
            if ((currentSession->getNumberOfPerformanceFilters() > 0) &&
                (!currentSession->getPerformance(0)->hasMarkup()))
            {
                addLink(dataSource->getId(), currentSession->getPerformance(0)->getId());
            }
        }
    }
}

void Pipeline::run()
{
    bool Acted;

    createNodes();

    for (auto iter = nodes.begin(); iter != nodes.end(); ++iter)
    {
        (*iter)->init();
    }

    do
    {
        Acted = false;
        for (auto iter = nodes.begin(); iter != nodes.end(); ++iter)
        {
            if((*iter)->isReady())
            {
                Acted = true;
                (*iter)->run();
            }
        }
    }
    while(Acted);

    for (auto iter = nodes.begin(); iter != nodes.end(); ++iter)
    {
        (*iter)->finalize();
    }

    clearNodes();
}

Pipeline::~Pipeline()
{
	//cout << "SSdeb In ~Pipeline" << endl;
}

SPtr<IPipelineNode> Pipeline::getNode(string filterId)
{
    for (auto current = nodes.begin(); current != nodes.end(); current++ ) 
    {
        if( filterId == (*current)->getId() )
        {
            return *current;
        }
    }

    return NULL;
}

void Pipeline::addNodeCallback(string filterId, PipelineCallback func, void* context )
{
    pair<PipelineCallback, void*> funcContext(func, context);
    nodeCallbacks[filterId].insert(funcContext);
}

void Pipeline::removeNodeCallback(string filterId, PipelineCallback func, void* context )
{
    pair<PipelineCallback, void*> funcContext(func, context);
    auto it = nodeCallbacks[filterId].find(funcContext);
    if(it != nodeCallbacks[filterId].end())
    {
        nodeCallbacks[filterId].erase(it);
    }
}

void Pipeline::removeNodeCallbacks(string filterId)
{
    nodeCallbacks.erase(filterId);
}

SPtr<IPipelineNode> Pipeline::addNode(SPtr<IFilter> filter)
{
    string newFilterId = filter->getId();
    SPtr<IPipelineNode> pipelineNode = getNode(newFilterId);
    if(pipelineNode != NULL)
    {
        return pipelineNode;
    }

    switch(filter->getRootType())
    {
    case VIDEO_PLAYER_FILTER:
        if (playlist)
        {
            IVCP_EXCEPTION(IncorrectParamException, "Source is already defined");
        }
        pipelineNode = new PipelineSourceNode( SPtr<IVideoPlayer>( filter ) );
        playlist = SPtr<IVideoPlayer>( filter )->getPlaylist();
        break;
    case REGION_CREATOR_FILTER:
        pipelineNode = new PipelineRegionCreatorNode( SPtr<IRegionCreator>(filter) );
        break;
    case FEATURE_FILTER:
        pipelineNode = new PipelineFeatureNode( SPtr<IFeature>(filter), pipelineSettings );
        break;
    case CLASSIFIER_FILTER:
        pipelineNode = new PipelineClassifierNode( SPtr<IClassifier>(filter), this->currentMode );
        {
            auto outputFile = classifierToOutputFile.find(filter->getId());
            if (outputFile != classifierToOutputFile.end())
            {
                SPtr<PipelineClassifierNode>(pipelineNode)->setOutputFilename(outputFile->second);
            }
        }
        break;
    case CLASSIFIER_RESULTS_FILTER:
    case GROUNDTRUTH_REGIONS_FILTER:
    case PERFORMANCE_FILTER:
    case VIDEO_SOURCE_FILTER:
    case SESSION_FILTER:
        IVCP_EXCEPTION(IncorrectParamException, "Could not create node for such type of filter");
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Undefined filter type");
    }
    pipelineNode->setPipeline(this);

    std::map<NodeType, int> filterPos; 
    filterPos[SOURCE_NODE] = 0;
    filterPos[REGION_NODE] = 1;
    filterPos[FEATURE_NODE] = 2;
    filterPos[CLASSIFICATOR_NODE] = 3;

    //Find insert position (for Video Player this is a position of the first Region Creator, for a Region Creator this is a position of a Feature Node etc)
    auto insertPos = find_if(nodes.begin(), nodes.end(), [&filterPos, &pipelineNode](SPtr<IPipelineNode> node) -> bool 
    {
        if (filterPos[node->getType()] > filterPos[pipelineNode->getType()])
        {
            return true;
        }
        return false;
    });

    nodes.insert(insertPos, pipelineNode);
    if(nodeCallbacks.count(newFilterId))
    {
        for (auto callback = nodeCallbacks[newFilterId].begin(); callback != nodeCallbacks[newFilterId].end(); callback++)
        {
            pipelineNode->addCallback(callback->first, callback->second);
        }
    }
    return pipelineNode;
}

void Pipeline::createNodes()
{
    if(currentSession == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Session is undefined. Could not search for filters");
    }

    for(auto iter = links.begin(); iter != links.end(); iter++)
    {
        SPtr<IFilter> fromFilter = currentSession->getFilter(iter->first);
        SPtr<IFilter> toFilter = currentSession->getFilter(iter->second);
        if( fromFilter == NULL || toFilter == NULL)
        {
            IVCP_LOG_WARNING("Could not create link from %s to %s. There is no filter with provided ID", 
                iter->first.c_str(), iter->second.c_str());
            continue;
        }
        if( !areConnectable( fromFilter->getRootType(), toFilter->getRootType() ) )
        {
            IVCP_LOG_WARNING("Could not create link from %s to %s. Filters with provided IDs could not be linked",
                fromFilter->getId().c_str(), toFilter->getId().c_str());
            continue;
        }
        if(fromFilter->getRootType() == CLASSIFIER_RESULTS_FILTER)
        {
            if(toFilter->getRootType() == PERFORMANCE_FILTER)
            {
                SPtr<IClassifierResultDataSource> dataSource = fromFilter;
                SPtr<IPerformance> performance = toFilter;
                performance->setMarkup(dataSource);
                continue;
            }
            if(toFilter->getRootType() == CLASSIFIER_FILTER)
            {
                SPtr<IClassifierResultDataSource> dataSource = fromFilter;
                SPtr<PipelineClassifierNode> toClassifier = addNode(toFilter);
                toClassifier->setMarkup(dataSource);
                continue;
            }
        }
        //fromFilter here could not be data source due to preceding check
        //fromFilter also could not be performance cause such link is not connectable
        SPtr<IPipelineNode> fromNode = addNode(fromFilter);
        if(fromNode != NULL)
        {
            if(toFilter->getRootType() == PERFORMANCE_FILTER)
            {
                SPtr<IPerformance> performance = toFilter;
                fromNode->setPerformance(performance);
                continue;
            }
            //toFilter here could not be performance due to preceding check
            //toFilter also could not be data source cause such link is not connectable
            SPtr<IPipelineNode> toNode = addNode(toFilter);
            fromNode->addDestinationLink(toNode);
            toNode->addSourceLink(fromNode->getId());
        }
    }
}

void Pipeline::clearNodes()
{
    if(currentSession == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Session is undefined. Looks like something strange happens");
    }
    
    for(auto iter = links.begin(); iter != links.end(); iter++)
    {
        SPtr<IFilter> fromFilter = currentSession->getFilter(iter->first);
        SPtr<IFilter> toFilter = currentSession->getFilter(iter->second);
        if( fromFilter == NULL || toFilter == NULL)
        {
            IVCP_LOG_WARNING("Link from %s to %s was not created because there is no filter with provided ID", 
                iter->first.c_str(), iter->second.c_str());
            continue;
        }
        if( !areConnectable( fromFilter->getRootType(), toFilter->getRootType() ) )
        {
            IVCP_LOG_WARNING("Link from %s to %s was not created because filters with provided IDs could not be linked", 
                fromFilter->getId().c_str(), toFilter->getId().c_str());
            continue;
        }
        if(fromFilter->getRootType() == CLASSIFIER_RESULTS_FILTER)
        {
            if(toFilter->getRootType() == PERFORMANCE_FILTER)
            {
                SPtr<IPerformance> performance = toFilter;
                performance->setMarkup(NULL);
                continue;
            }
        }
    }
    nodes.clear();
    playlist = NULL;
}

void Pipeline::addLink(string from, string to)
{
    if(currentSession == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Session is undefined. Could not search for filters");
    }
    else
    {
        SPtr<IFilter> fromFilter = currentSession->getFilter(from);
        SPtr<IFilter> toFilter = currentSession->getFilter(to);
        if( fromFilter == NULL || toFilter == NULL)
        {
            IVCP_EXCEPTION(IncorrectParamException, "There is no filter with provided ID");
        }
        if( !areConnectable( fromFilter->getRootType(), toFilter->getRootType() ) )
        {
            IVCP_EXCEPTION(IncorrectParamException, "Filters with provided IDs could not be linked");
        }
    }

    pair<string, string> link;
    link.first = from;
    link.second = to;
    if( find(links.begin(), links.end(), link) == links.end() )
    {
        links.push_back(link);
    }
}

void Pipeline::removeLink(string from, string to)
{
    pair<string, string> link;
    link.first = from;
    link.second = to;
    vector< pair<string, string> >::iterator iter = find(links.begin(), links.end(), link);
    if(  iter != links.end() )
    {
        links.erase(iter);
    }
}

void Pipeline::removeAllLinks()
{
    links.clear();
}

pair<string,string> Pipeline::getLink(int linkIndex)
{
    if ( linkIndex < (int)links.size() )
    {
        return links[linkIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no link with provided index");
    }
}

int Pipeline::getNumberOfLinks() const
{
    return (int)links.size();
}

FilterType Pipeline::getRootType() const
{
    return PIPELINE_FILTER;
}

IPipeline::Settings Pipeline::getSettings()
{
    return pipelineSettings;
}

vector<VideoPlaylistFrame> Pipeline::getLastProcessedFrames(std::string featureId)
{
    SPtr<PipelineFeatureNode> featureNode = getNode(featureId);
    if (featureNode == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no feature with provided ID in Pipeline");
    }

    return featureNode->getLastProcessedFrames();
}

void Pipeline::setCurrentFrame(VideoPlaylistFrame frame)
{
    currentFrame = frame;
}

VideoPlaylistFrame Pipeline::getCurrentFrame()
{
    return currentFrame;
}

void Pipeline::trackFeatureToFile(std::string id_classifier, std::string filename)
{
    if(currentSession == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Session is undefined. Could not find feature to track");
    }

    SPtr<IFilter> classifier = currentSession->getFilter(id_classifier);
    if (classifier == NULL)
    {
        IVCP_LOG_WARNING("Can't track features for \"%s\". This classifier not found in current session", id_classifier.c_str());
    }
    else
    {
        classifierToOutputFile[id_classifier] = filename;
    }
}

void Pipeline::setFilterPriority(const std::string& filterName, ProcessingPriority priority)
{
    (void) filterName;
    (void) priority;
    IVCP_LOG_WARNING("Can't set priorities in serial pipeline");
}

};
