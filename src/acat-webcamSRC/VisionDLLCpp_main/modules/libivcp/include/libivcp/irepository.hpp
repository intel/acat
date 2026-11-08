///////////////////////////////////////////////////////////////////////////
// <copyright file="irepository.hpp" company="Intel Corporation">
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


#ifndef __REPOSITORY_HPP__
#define __REPOSITORY_HPP__

#include <string>
#include "libivcp/ipipeline.hpp"
#include "libivcp/session.hpp"
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/data_source/iclassifier_result_data_source.hpp"
#include "libivcp/video_source/ivideo_source.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"


/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file irepository.hpp
 * \brief Declaration of IRepository interface 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Repository is responsible for Filter persistence
 */
class IVCP_EXPORT IRepository
{
public:
    /*!
     * \brief Destructor
     */ 
    virtual ~IRepository() {};

    /*!
     * \brief Load Filter with given id
     * \param[in] id Filter id
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found filter with given id
     * \return Filter loaded from the Repository storage
     */
    virtual SPtr<Filter> load(std::string id) = 0;

    /*!
     * \brief Load Session with given id
     * \param[in] id Id of the Session to load. In case id is empty first Session will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found session with given id
     * \return Session loaded from the Repository storage
     */
    virtual SPtr<Session> loadSession(std::string id = "") = 0;

    /*!
     * \brief Load Pipeline with given id
     * \param[in] id Id of the Pipeline to load. In case id is empty first Pipeline will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found pipeline with given id
     * \return Pipeline loaded from the Repository storage
     */
    virtual SPtr<IPipeline> loadPipeline(std::string id = "") = 0;

    /*!
     * \brief Load ParallelPipeline with given id
     * \param[in] id Id of the ParallelPipeline to load. In case id is empty first ParallelPipeline will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found parallel pipeline with given id
     * \return ParallelPipeline loaded from the Repository storage
     */
    virtual SPtr<IPipeline> loadParallelPipeline(std::string id = "") = 0;

    /*!
     * \brief Load Classifier with given id
     * \param[in] id Id of the Classifier to load. In case id is empty first Classifier will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found classifier with given id
     * \return Classifier loaded from the Repository storage
     */
    virtual SPtr<IClassifier> loadClassifier(std::string id = "") = 0;

    /*!
     * \brief Load GTRegionDataSource with given id
     * \param[in] id Id of the GTRegionDataSource to load. In case id is empty first GTRegionDataSource will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found region data source with given id
     * \return GTRegionDataSource loaded from the Repository storage
     */
    virtual SPtr<IRegionDataSource> loadRegionDataSource(std::string id = "") = 0;

    /*!
     * \brief Load ClassifierResultDataSource with given id
     * \param[in] id Id of the ClassifierResultDataSource to load. In case id is empty first ClassifierResultDataSource will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found classifier result datasource with given id
     * \return ClassifierResultDataSource loaded from the Repository storage
     */
    virtual SPtr<IClassifierResultDataSource> loadClassifierResultDataSource(std::string id = "") = 0;

    /*!
     * \brief Load data source (ClassifierResultDataSource or RegionDataSource) with given id
     * \param[in] id Id of the data source to load. In case id is empty first data source will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found datasource with given id
     * \return Data source loaded from the Repository storage
     */
    virtual SPtr<IFilter> loadDataSource(std::string id = "") = 0;

    /*!
     * \brief Load Feature with given id
     * \param[in] id Id of the Feature to load. In case id is empty first Feature will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found feature with given id
     * \return Feature loaded from the Repository storage
     */
    virtual SPtr<IFeature> loadFeature(std::string id = "") = 0;

    /*!
     * \brief Load Performance with given id
     * \param[in] id Id of the Performance filter to load. In case id is empty first Performance filter will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found performance filter with given id
     * \return Performance loaded from the Repository storage
     */
    virtual SPtr<IPerformance> loadPerformance(std::string id = "") = 0;

    /*!
     * \brief Load RegionCreator with given id
     * \param[in] id Id of the RegionCreator to load. In case id is empty first RegionCreator will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found region creator with given id
     * \return RegionCreator loaded from the Repository storage
     */
    virtual SPtr<IRegionCreator> loadRegionCreator(std::string id = "") = 0;

    /*!
     * \brief Load VideoSource with given id
     * \param[in] id Id of the VideoSource to load. In case id is empty first VideoSource will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found videosource with given id
     * \return VideoSource loaded from the Repository storage
     */
    virtual SPtr<IVideoSource> loadVideoSource(std::string id = "") = 0;

    /*!
     * \brief Load Video Player with given id
     * \param[in] id Id of the Video Player to load. In case id is empty first Video Player will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found video player with given id
     * \return Video Player loaded from the Repository storage
     */
    virtual SPtr<IVideoPlayer> loadVideoPlayer(std::string id = "") = 0;

    /*!
     * \brief Load Video Playlist with given id
     * \param[in] id Id of the Video Playlist to load. In case id is empty first Video Playlist will be loaded
     * \throw IOException FileStorage internal error
     * \throw NoDataException Not found video playlist with given id
     * \return Video Playlist loaded from the Repository storage
     */
    virtual SPtr<VideoPlaylist> loadVideoPlaylist(std::string id = "") = 0;

    /*!
     * \brief Save filter to the Repository storage
     * \throw IOException FileStorage internal error
     * \throw NotImplementedException Not implemented serialiazer for filter with given id
     * \param[in] filter Filter to save
     */
    virtual void save(SPtr<Filter> filter) = 0;

};
}

/** @}*/
#endif
