///////////////////////////////////////////////////////////////////////////
// <copyright file="session.hpp" company="Intel Corporation">
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


#ifndef __SESSION_HPP__
#define __SESSION_HPP__

#include <vector>
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/region_creator/iregion_creator.hpp"
#include "libivcp/feature/ifeature.hpp"
#include "libivcp/classifier/iclassifier.hpp"
#include "libivcp/performance/iperformance.hpp"
#include "libivcp/video_source/video_player.hpp"
#include "libivcp/exceptions/cast_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file session.hpp
 * \brief Declaration of Session class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Session class is a Factory of all filters
 */
class IVCP_EXPORT Session: public Filter
{
public:
    /*!
     * \brief Default constructor
     */
    Session();

    /*!
     * \brief Default destructor
     */
    virtual ~Session();

    /*!
     * \brief Set Video Player
     * \param[in] player Video Player 
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void setVideoPlayer(SPtr<VideoPlayer > player);

    /*!
     * \brief Get pointer to Video Player interface
     */
    SPtr<IVideoPlayer > getVideoPlayer();

    /*!
     * \brief Get pointer to Video Playlist interface
     * \details There is only 1 copy of Video Playlist per session allowed
     */
    SPtr<VideoPlaylist > getVideoPlaylist();

    /*!
     * \brief Assign a Video Playlist to the current Video Player
     * \param[in] playlist Video Playlist pointer
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void setVideoPlaylist(SPtr<VideoPlaylist > playlist);

    /*!
     * \brief Add Feature filter to the Session
     * \param[in] feature Feature filter interface
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void addFeature(SPtr<IFeature > feature);

    /*!
     * \brief Delete Feature filter from the Session
     * \param[in] feature Feature filter interface
     */
    void delFeature(SPtr<IFeature > feature);

    /*!
     * \brief Get Feature interface by string id
     * \param[in] featureId string Feature id
     */
    SPtr<IFeature > getFeature(std::string featureId);

    /*!
     * \brief Get Feature interface by index (from 0 to getNumberOfFeatures()-1)
     * \param[in] featureIndex Feature index in the Session
     */
    SPtr<IFeature > getFeature(int featureIndex);

    /*!
     * \brief Get number of Features in the Session
     */
    int getNumberOfFeatures() const;

    /*!
     * \brief Add Region Creator filter to the Session
     * \param[in] regionCreator Region Creator filter interface
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void addRegionCreator(SPtr<IRegionCreator > regionCreator);

    /*!
     * \brief Delete Region Creator filter from the Session
     * \param[in] regionCreator Region Creator filter interface
     */
    void delRegionCreator(SPtr<IRegionCreator > regionCreator);

    /*!
     * \brief Get Region Creator interface by string id
     * \param[in] regionId string Region Creator id
     */
    SPtr<IRegionCreator > getRegionCreator(std::string regionId);

    /*!
     * \brief Get Region Creator interface by index (from 0 to getNumberOfRegionCreators()-1)
     * \param[in] regionCreatorIndex Region Creator index in the Session
     */
    SPtr<IRegionCreator > getRegionCreator(int regionCreatorIndex);

    /*!
     * \brief Get number of Region Creator filters in the Session
     */
    int getNumberOfRegionCreators() const;

    /*!
     * \brief Add Classifier filter to the Session
     * \param[in] classifier Classifier filter interface
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void addClassifier(SPtr<IClassifier > classifier);

    /*!
     * \brief Delete Classifier filter from the Session
     * \param[in] classifier Classifier filter interface
     */
    void delClassifier(SPtr<IClassifier > classifier);

    /*!
     * \brief Get Classifier interface by string id
     * \param[in] classifierId string Classifier id
     */
    SPtr<IClassifier > getClassifier(std::string classifierId);

    /*!
     * \brief Get Classifier interface by index (from 0 to getNumberOfClassifiers()-1)
     * \param[in] classifierIndex Classifier index in the Session
     */
    SPtr<IClassifier > getClassifier(int classifierIndex);

    /*!
     * \brief Get number of Classifier filters in the Session
     */
    int getNumberOfClassifiers() const;

    /*!
     * \brief Add Performance filter to the Session
     * \param[in] performance Performance filter interface
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void addPerformanceFilter(SPtr<IPerformance > performance);

    /*!
     * \brief Delete Performance filter from the Session
     * \param[in] performance Performance filter interface
     */
    void delPerformanceFilter(SPtr<IPerformance > performance);

    /*!
     * \brief Get Performance interface by string id
     * \param[in] performanceId string Performance id
     */
    SPtr<IPerformance > getPerformance(std::string performanceId);

    /*!
     * \brief Get Performance interface by index (from 0 to getNumberOfPerformanceFilters()-1)
     * \param[in] performanceIndex Performance index in the Session
     */
    SPtr<IPerformance > getPerformance(int performanceIndex);

    /*!
     * \brief Get number of Performance filters in the Session
     */
    int getNumberOfPerformanceFilters() const;

    /*!
     * \brief Add VideoSource filter to the Session
     * \param[in] performance Performance filter interface
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     */
    void addVideoSource(SPtr<IVideoSource > performance);

    /*!
     * \brief Delete VideoSource filter from the Session
     * \param[in] performance Performance filter interface
     */
    void delVideoSource(SPtr<IVideoSource > performance);

    /*!
     * \brief Get VideoSource interface by string id
     * \param[in] performanceId string Performance id
     */
    SPtr<IVideoSource > getVideoSource(std::string performanceId);

    /*!
     * \brief Get VideoSource interface by index (from 0 to getNumberOfVideoSources()-1)
     * \param[in] performanceIndex Performance index in the Session
     */
    SPtr<IVideoSource > getVideoSource(int performanceIndex);

    /*!
     * \brief Get number of VideoSource filters in the Session
     */
    int getNumberOfVideoSources() const;

    /*!
     * \brief Add data source to the Session	
     * \param[in] dataSource Data source (classifier results or groundtruth regions)
     * \throw IncorrectParamException A Filter with such id is already present in the Session
     * or dataSource is not IClassifierResultDataSource and not IRegionDataSource
     */
    void addDataSource(SPtr<IFilter > dataSource);

    /*!
     * \brief Delete data source from the Session	
     * \param[in] dataSource Data source (classifier results or groundtruth regions)
     */
    void delDataSource(SPtr<IFilter > dataSource);

    /*!
     * \brief Get data source by string id
     * \param[in] dataSourceId String data source id
     */
    SPtr<IFilter > getDataSource(std::string dataSourceId);

    /*!
     * \brief Get data source by index (from 0 to getNumberOfDataSources()-1)
     * \param[in] dataSourceIndex Data source index in the Session
     */
    SPtr<IFilter > getDataSource(int dataSourceIndex);

    /*!
     * \brief Get number of data sources in the Session
     */
    int getNumberOfDataSources() const;
    
    /*!
     * \brief Add Filter to the Session
     * \param[in] filter Filter to add
     * \throw IncorrectParamException Unable to add a Filter of specific type (for example of type Pipeline) to Session
     * \throw CastException Unable to cast Filter to known type
     */
    void addFilter(SPtr<Filter> filter);

    /*!
     * \brief Delete Filter from the Session
     * \param[in] filter Filter to delete
     * \throw IncorrectParamException Unable to delete a Filter of specific type from Session (for example we can not delete a Video Player)
     * \throw CastException Unable to cast Filter to known type
     */
    void delFilter(SPtr<Filter> filter);
    
    /*!
     * \brief Get Filter interface by string id
     * \param[in] filterId string Filter id
     */
    SPtr<IFilter> getFilter(std::string filterId);

    /*!
     * \brief Return type of a filter (ie Classifier, Feature etc)
     * \return Filter type
     */
    virtual FilterType getRootType() const;    

protected:
    /*!
     * \brief Check no such in in the Session and generate IncorrectParameException otherwise
     * \param[in] filter New filter which id shall not be presented in the Session
     * \throw IncorrectParamException There is such id in the Session
     */
    void checkIdIsUnick(SPtr<Filter> filter);

    /*!
     * \brief Video player
     */
    SPtr<VideoPlayer> videoPlayer;

    /*!
     * \brief Set of video sources for video player
     */
    std::vector<SPtr<IVideoSource > > sources;

    /*!
     * \brief Set of region creators
     */
    std::vector<SPtr<IRegionCreator > > regions;

    /*!
     * \brief Set of features
     */
    std::vector<SPtr<IFeature > > features;

    /*!
     * \brief Set of classifiers
     */
    std::vector<SPtr<IClassifier > > classifiers;

    /*!
     * \brief Set of Performance Filters (Performance and Quality report generators)
     */
    std::vector<SPtr<IPerformance > > performances;

    /*!
     * \brief Data sources (classifier results and groundtruth regions)
     */
    std::vector<SPtr<IFilter > > dataSources;

private:

};
};

/** @}*/
#endif 
