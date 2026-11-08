///////////////////////////////////////////////////////////////////////////
// <copyright file="session.cpp" company="Intel Corporation">
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




#include "libivcp/session.hpp"
#include "libivcp/factory.hpp"
#include "libivcp/performance/textfile_performance.hpp"

using namespace std;
using namespace cv;

namespace ivcp
{
Session::Session():
    videoPlayer(Factory::createVideoPlayer("player"))
{
}

Session::~Session()
{
}

SPtr<IVideoPlayer> Session::getVideoPlayer()
{
    return videoPlayer;
}

void Session::setVideoPlayer(SPtr<VideoPlayer > player)
{
    if (NULL != player)
    {
        checkIdIsUnick(player);
        videoPlayer = player;
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "player is a NULL pointer");
    }
}

SPtr<VideoPlaylist> Session::getVideoPlaylist()
{
    return videoPlayer->getPlaylist();
}

void Session::setVideoPlaylist(SPtr<VideoPlaylist> playlist)
{
    checkIdIsUnick(playlist);
    videoPlayer->setPlaylist(playlist);
}

void Session::addFeature(SPtr<IFeature> feature)
{
    checkIdIsUnick(feature);
    if (find(features.begin(), features.end(), feature) == features.end())
    {
        features.push_back(feature);
    }
}

void Session::delFeature(SPtr<IFeature> feature)
{
    for (vector<SPtr<IFeature> >::iterator iter = features.begin(); iter != features.end(); ++iter)
    {
        if (*iter == feature)
        {
            features.erase(iter);
            return;
        }
    }
}

SPtr<IFeature> Session::getFeature(string featureId)
{
    for (vector<SPtr<IFeature> >::iterator iter = features.begin(); iter != features.end(); ++iter)
    {
        if ((*iter)->getId() == featureId)
        {
            return *iter;
        }
    }
    IVCP_EXCEPTION(IncorrectParamException, "There is no feature with provided ID");
}

SPtr<IFeature > Session::getFeature(int featureIndex)
{
    if (featureIndex < (int)features.size())
    {
        return features[featureIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no feature with provided index");
    }
}

int Session::getNumberOfFeatures() const
{
    return (int)features.size();
}

void Session::addRegionCreator(SPtr<IRegionCreator> regionCreator)
{
    checkIdIsUnick(regionCreator);
    if (find(regions.begin(), regions.end(), regionCreator) == regions.end())
    {
        regions.push_back(regionCreator);
    }
}

void Session::delRegionCreator(SPtr<IRegionCreator> regionCreator)
{
    for (vector<SPtr<IRegionCreator> >::iterator iter = regions.begin(); iter != regions.end(); ++iter)
    {
        if (*iter == regionCreator)
        {
            regions.erase(iter);
            return;
        }
    }
}

SPtr<IRegionCreator > Session::getRegionCreator(string regionId)
{
    for (vector<SPtr<IRegionCreator> >::iterator iter = regions.begin(); iter != regions.end(); ++iter)
    {
        if ((*iter)->getId() == regionId)
        {
            return *iter;
        }
    }
    IVCP_EXCEPTION(IncorrectParamException, "There is no region creator with provided ID");
}

SPtr<IRegionCreator> Session::getRegionCreator(int regionCreatorIndex)
{
    if (regionCreatorIndex < (int)regions.size())
    {
        return regions[regionCreatorIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no region creator with provided index");
    }
}

int Session::getNumberOfRegionCreators() const
{
    return (int)regions.size();
}

void Session::addClassifier(SPtr<IClassifier > classifier)
{
    checkIdIsUnick(classifier);
    if (find(classifiers.begin(), classifiers.end(), classifier) == classifiers.end())
    {
        classifiers.push_back(classifier);
    }
}

void Session::delClassifier(SPtr<IClassifier > classifier)
{
    for (vector<SPtr<IClassifier > >::iterator iter = classifiers.begin(); iter != classifiers.end(); ++iter)
    {
        if(*iter == classifier)
        {
            classifiers.erase(iter);
            return;
        }
    }
}

SPtr<IClassifier > Session::getClassifier(string classifierId)
{
    for(vector<SPtr<IClassifier > >::iterator iter = classifiers.begin(); iter != classifiers.end(); ++iter)
    {
        if( (*iter)->getId() == classifierId )
        {
            return *iter;
        }
    }
    IVCP_EXCEPTION(IncorrectParamException, "There is no classifier with provided ID");
}

SPtr<IClassifier> Session::getClassifier(int classifierIndex)
{
    if (classifierIndex < (int)classifiers.size())
    {
        return classifiers[classifierIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no classifier with provided index");
    }
}

int Session::getNumberOfClassifiers() const
{
    return (int)classifiers.size();
}

void Session::addPerformanceFilter(SPtr<IPerformance > performance)
{
    checkIdIsUnick(performance);
    if (find(performances.begin(), performances.end(), performance) == performances.end())
    {
        performances.push_back(performance);
    }
}

void Session::delPerformanceFilter(SPtr<IPerformance > performance)
{
    for (vector<SPtr<IPerformance> >::iterator iter = performances.begin(); iter != performances.end(); ++iter)
    {
        if (*iter == performance)
        {
            performances.erase(iter);
            return;
        }
    }
}

SPtr<IPerformance > Session::getPerformance(string performanceId)
{
    for (vector<SPtr<IPerformance > >::iterator iter = performances.begin(); iter != performances.end(); ++iter)
    {
        if ((*iter)->getId() == performanceId)
        {
            return *iter;
        }
    }
    IVCP_EXCEPTION(IncorrectParamException, "There is no performance with provided ID");
}

SPtr<IPerformance > Session::getPerformance(int performanceIndex)
{
    if (performanceIndex < (int)performances.size())
    {
        return performances[performanceIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no performance with provided index");
    }
}

int Session::getNumberOfPerformanceFilters() const
{
    return (int)performances.size();
}

void Session::addVideoSource(SPtr<IVideoSource > source)
{
    checkIdIsUnick(source);
    if (find(sources.begin(), sources.end(), source) == sources.end())
    {
        sources.push_back(source);
    }
}

void Session::delVideoSource(SPtr<IVideoSource > source)
{
    for (vector<SPtr<IVideoSource > >::iterator iter = sources.begin(); iter != sources.end(); ++iter)
    {
        if (*iter == source)
        {
            sources.erase(iter);
            return;
        }
    }
}

SPtr<IVideoSource > Session::getVideoSource(string sourceId)
{
    for (vector<SPtr<IVideoSource > >::iterator iter = sources.begin(); iter != sources.end(); ++iter)
    {
        if ((*iter)->getId() == sourceId)
        {
            return *iter;
        }
    }
    IVCP_EXCEPTION(IncorrectParamException, "There is no source with provided ID");
}

SPtr<IVideoSource > Session::getVideoSource(int sourceIndex)
{
    if (sourceIndex < (int)sources.size())
    {
        return sources[sourceIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no source with provided index");
    }
}

int Session::getNumberOfVideoSources() const
{
    return (int)sources.size();
}

void Session::addDataSource(SPtr<IFilter > dataSource)
{
    if (dataSource->getRootType() == CLASSIFIER_RESULTS_FILTER || dataSource->getRootType() == GROUNDTRUTH_REGIONS_FILTER)
    {
        checkIdIsUnick(dataSource);
        if (find(dataSources.begin(), dataSources.end(), dataSource) == dataSources.end())
        {
            dataSources.push_back(dataSource);
        }
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "Incorrect root type of data source. Data source should be IClassifierDataSource or IRegionDataSource");
    }
}

void Session::delDataSource(SPtr<IFilter > dataSource)
{
    for (vector<SPtr<IFilter > >::iterator iter = dataSources.begin(); iter != dataSources.end(); ++iter)
    {
        if (*iter == dataSource)
        {
            dataSources.erase(iter);
            return;
        }
    }
}

SPtr<IFilter > Session::getDataSource(string dataSourceId)
{
    for (vector<SPtr<IFilter > >::iterator iter = dataSources.begin(); iter != dataSources.end(); ++iter)
    {
        if ((*iter)->getId() == dataSourceId)
        {
            return *iter;
        }
    }
    IVCP_EXCEPTION(IncorrectParamException, "There is no data source with provided ID");
}

SPtr<IFilter > Session::getDataSource(int dataSourceIndex)
{
    if (dataSourceIndex < (int)dataSources.size())
    {
        return dataSources[dataSourceIndex];
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "There is no data source with provided index");
    }
}

int Session::getNumberOfDataSources() const
{
    return (int)dataSources.size();
}

void Session::addFilter(SPtr<Filter> filter)
{
    SPtr<Classifier> classifier = NULL;
    SPtr<IClassifierResultDataSource> classRes = NULL;
    SPtr<IRegionDataSource> gtRegs = NULL;
    SPtr<Feature> feature = NULL;
    SPtr<Performance> performance = NULL;
    SPtr<RegionCreator> regionCreator = NULL;
    SPtr<VideoSource> videoSource = NULL;
    SPtr<VideoPlayer> selectedVideoPlayer = NULL;
    SPtr<VideoPlaylist> videoPlaylist = NULL;

    checkIdIsUnick(filter);

    switch (filter->getRootType())
    {
    case CLASSIFIER_FILTER:
        classifier = filter;
        if (NULL == classifier)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Classifier");
        }
        addClassifier(classifier);
        break;
    case CLASSIFIER_RESULTS_FILTER:
        classRes = filter;
        if (NULL == classRes)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to IClassifierResultDataSource");
        }
        addDataSource(classRes);
        break;
    case GROUNDTRUTH_REGIONS_FILTER:
        gtRegs = filter;
        if (NULL == gtRegs)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to IRegionDataSource");
        }
        addDataSource(gtRegs);
        break;
    case FEATURE_FILTER: 
        feature = filter;
        if (NULL == feature)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Feature");
        }
        addFeature(feature);
        break;
    case PERFORMANCE_FILTER: 
        performance = filter;
        if (NULL == performance)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Performance");
        }
        addPerformanceFilter(performance);
        break;
    case REGION_CREATOR_FILTER: 
        regionCreator = filter;
        if (NULL == regionCreator)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Region Creator");
        }
        addRegionCreator(regionCreator);
        break;
    case VIDEO_SOURCE_FILTER: 
        videoSource = filter;
        if (NULL == videoSource)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Video Source");
        }
        addVideoSource(filter);
        break;
    case VIDEO_PLAYER_FILTER: 
        selectedVideoPlayer = filter;
        if (NULL == selectedVideoPlayer)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Video Player");
        }
        setVideoPlayer(selectedVideoPlayer);
        break;
    case VIDEO_PLAYLIST_FILTER:
        videoPlaylist = filter;
        if (NULL == videoPlaylist)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Video Playlist");
        }
        setVideoPlaylist(videoPlaylist);
        break;
    case SESSION_FILTER: 
    case PIPELINE_FILTER:
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unable to add Filter of this type to Session");
        break;
    }
}

void Session::delFilter(SPtr<Filter> filter)
{
    SPtr<Classifier> classifier = NULL;
    SPtr<IClassifierResultDataSource> classRes = NULL;
    SPtr<IRegionDataSource> gtRegs = NULL;
    SPtr<Feature> feature = NULL;
    SPtr<Performance> performance = NULL;
    SPtr<RegionCreator> regionCreator = NULL;
    SPtr<VideoSource> videoSource = NULL;
    SPtr<VideoPlaylist> videoPlaylist = NULL;

    switch (filter->getRootType())
    {
    case CLASSIFIER_FILTER:
        classifier = filter;
        if (NULL == classifier)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Classifier");
        }
        delClassifier(classifier);
        break;
    case CLASSIFIER_RESULTS_FILTER:
        classRes = filter;
        if (NULL == classRes)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to IClassifierResultDataSource");
        }
        delDataSource(classRes);
        break;
    case GROUNDTRUTH_REGIONS_FILTER:
        gtRegs = filter;
        if (NULL == gtRegs)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to IRegionDataSource");
        }
        delDataSource(gtRegs);
        break;
    case FEATURE_FILTER: 
        feature = filter;
        if (NULL == feature)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Feature");
        }
        delFeature(feature);
        break;
    case PERFORMANCE_FILTER: 
        performance = filter;
        if (NULL == performance)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Performance");
        }
        delPerformanceFilter(performance);
        break;
    case REGION_CREATOR_FILTER: 
        regionCreator = filter;
        if (NULL == regionCreator)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Region Creator");
        }
        delRegionCreator(regionCreator);
        break;
    case VIDEO_SOURCE_FILTER: 
        videoSource = filter;
        if (NULL == videoSource)
        {
            IVCP_EXCEPTION(CastException, "Unable to cast Filter to Video Source");
        }
        delVideoSource(filter);
        break;
    case VIDEO_PLAYER_FILTER: 
    case VIDEO_PLAYLIST_FILTER:
    case SESSION_FILTER: 
    case PIPELINE_FILTER:
    default:
        IVCP_EXCEPTION(IncorrectParamException, "Unable to delete Filter of this type from Session");
        break;
    }
}

SPtr<IFilter> Session::getFilter(std::string filterId)
{
    if (videoPlayer->getId() == filterId)
    {
        return videoPlayer;
    }

    if (videoPlayer->getPlaylist()->getId() == filterId)
    {
        return videoPlayer->getPlaylist();
    }

    for (vector<SPtr<IVideoSource> >::iterator iter = sources.begin(); iter != sources.end(); ++iter)
    {
        if ( (*iter)->getId() == filterId )
        {
            return *iter;
        }
    }

    for (vector<SPtr<IRegionCreator> >::iterator iter = regions.begin(); iter != regions.end(); ++iter)
    {
        if ( (*iter)->getId() == filterId )
        {
            return *iter;
        }
    }
    for (vector<SPtr<IFeature> >::iterator iter = features.begin(); iter != features.end(); ++iter)
    {
        if ( (*iter)->getId() == filterId )
        {
            return *iter;
        }
    }
    for (vector<SPtr<IClassifier> >::iterator iter = classifiers.begin(); iter != classifiers.end(); ++iter)
    {
        if ( (*iter)->getId() == filterId )
        {
            return *iter;
        }
    }
    for (vector<SPtr<IPerformance> >::iterator iter = performances.begin(); iter != performances.end(); ++iter)
    {
        if ( (*iter)->getId() == filterId )
        {
            return *iter;
        }
    }
    for (vector<SPtr<IFilter> >::iterator iter = dataSources.begin(); iter != dataSources.end(); ++iter)
    {
        if ( (*iter)->getId() == filterId )
        {
            return *iter;
        }
    }

    return NULL;
}

FilterType Session::getRootType() const
{
    return SESSION_FILTER;
}

void Session::checkIdIsUnick(SPtr<Filter> newFilter)
{
    if (this->getId() == newFilter->getId())
    {
        stringstream ss;
        ss << "Double id " << newFilter->getId() << " is found in Session";
        IVCP_EXCEPTION(IncorrectParamException, ss.str());
    }

    SPtr<IFilter> filter = getFilter(newFilter->getId());
    if (filter != NULL)
    {
        if ((filter->getRootType() == newFilter->getRootType()) &&
            ((filter->getRootType() == VIDEO_PLAYER_FILTER) || (filter->getRootType() == VIDEO_PLAYLIST_FILTER)))
        {
            //that's ok 
            return;
        }

        stringstream ss;
        ss << "Double id " << newFilter->getId() << " is found in Session";
        IVCP_EXCEPTION(IncorrectParamException, ss.str());
    }
}

}
