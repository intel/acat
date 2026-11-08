///////////////////////////////////////////////////////////////////////////
// <copyright file="xml_repository.hpp" company="Intel Corporation">
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

#ifndef __XML_REPOSITORY_HPP__
#define __XML_REPOSITORY_HPP__

#include <list>
#include "libivcp/irepository.hpp"
#include "libinfra/repository/xml_serializer.hpp"
#include "libivcp/exceptions/cast_exception.hpp"
#include "libivcp/exceptions/not_implemented_exception.hpp"
#include "libivcp/session.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file xml_repository.hpp
 * \brief XML Repository implementation
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Implementation of IRepository interface for XML storage
 */
class IVCP_EXPORT XMLRepository: public IRepository
{
public:
    /*!
     * \brief Constructor
     * \param[in] filename XML storage filename
     */ 
    XMLRepository(std::string filename = "");

    /*!
     * \brief Destructor
     */
    virtual ~XMLRepository();

    /*!
     * \brief Set new XML storage filename
     * \param[in] filename XML storage filename
     * \throw IOException Unable to set XML storage filename
     */
    virtual void setUrl(std::string filename);

    /*!
     * \brief Get current XML storage filename
     * \return XML storage filename
     * \throw CastException Unable to get current XML storage filename
     */
    virtual std::string getUrl();

    virtual SPtr<Filter> load(std::string id);
    virtual SPtr<IClassifier> loadClassifier(std::string id = "");
    virtual SPtr<IFilter> loadDataSource(std::string id = "");
    virtual SPtr<IRegionDataSource> loadRegionDataSource(std::string id = "");
    virtual SPtr<IClassifierResultDataSource> loadClassifierResultDataSource(std::string id = "");
    virtual SPtr<IFeature> loadFeature(std::string id = "");
    virtual SPtr<IPerformance> loadPerformance(std::string id = "");
    virtual SPtr<IRegionCreator> loadRegionCreator(std::string id = "");
    virtual SPtr<IVideoSource> loadVideoSource(std::string id = "");
    virtual SPtr<IVideoPlayer> loadVideoPlayer(std::string id = "");
    virtual SPtr<Session> loadSession(std::string id = "");
    virtual SPtr<IPipeline> loadPipeline(std::string id = "");
    virtual SPtr<IPipeline> loadParallelPipeline(std::string id = "");
    virtual SPtr<VideoPlaylist> loadVideoPlaylist(std::string id = "");

    virtual void save(SPtr<Filter> filter);

    /*!
     * \brief Register serializer in Repository
     * \param[in] filter An instance of a Filter which could be processed by the serializer
     * \param[in] serializer Serializer that can process given filter
     */
    void registerSerializer(SPtr<Filter> filter, SPtr<IXMLSerializer> serializer);

    /*!
     * \brief Register serializer in Repository
     * \param[in] filterId Unick id of the Filter created by getFilterTypeId
     * \param[in] serializer Serializer that can process given filter
     */
    void registerSerializer(int filterId, SPtr<IXMLSerializer> serializer);

    /*!
     * \brief Generate unick id of a Filter
     * \param[in] filter Filter which id need to be generated
     */
    int getFilterTypeId(SPtr<Filter> filter);

    /*!
     * \brief Generate unick id of a Filter
     * \param[in] rootId Id returned by Filter::getRootType()
     * \param[in] filterId Id returned by Filter::getType()
     */
    int getFilterTypeId(int rootId, int filterId);

protected:
    /*!
     * \brief Get identifier of basic filter class (ie Classifier, DataSource etc)
     * \param[in] type Root type
     * \return Identifier of basic filter class
     */
    int getRootIdByType(int type);

    /*!
     * \brief Get identifier of filter class (ie SVMClassifier, STLBPFeature etc)
     * \param[in] type Filter type
     * \return Identifier of filter class
     */
    int getFilterIdByType(int type);

    /*!
     * \brief Load the first Filter in the Repository with matching fields
     * \details In case root type or filter types is not set the param shall be set as -1, in case id is not set it shall be set as empty string
     * \param[in] rootType identifier of basic filter class (ie Classifier, DataSource etc)
     * \param[in] filterType identifier of filter class (ie SVMClassifier, STLBPFeature etc)
     * \param[in] id Id of filter to load. In case id is empty first filter will be loaded
     * \return Filter loaded from the Repository storage, return NULL in case there is no match found
     * \throw IOException Unable to open the Repository file
     * \throw NoDataException Unable to load data with specific parameters (match is found by error occur during the loading)
     */
    SPtr<Filter> loadFirstMatching(int rootType = -1, int filterType = -1, std::string id = "");

    /*!
     * \brief Load state of all Filters in the Repository
     * \details OpenCV FileStorage does not support updatting. So when we want to update something we need to:
     * 1. Load previous values
     * 2. Update the values
     * 3. Save the updatted values
     * That's why we need loadAllNodeMaps which loads all filters which later could be saved again
     * \return A map from Filter id to its serialized state
     * \throw IOException Input/Output error occured during the loading
     */
    std::map<std::string, IXMLSerializer::FilterState > loadAllNodeMaps(); 

    /*!
     * \brief Number of bits used to shift rood id when we generate a unique Filter type id based on "root id" and "filter id"
     */
    static const unsigned int numberShiftBits;

    /*!
     * \brief A map from unick filter type id (see getFilterTypeId for more details) to its serializer
     */
    std::map<int, SPtr<IXMLSerializer> > fromFilterIdToSerializer;

    /*!
     * \brief Repository filename
     */
    std::string filename;

    /*!
     * \brief This flag is locked to prevent problems with recursive filters saving (ie when filter A while savign its state calls for Repository::save for underlaying filters)
     */
    bool storageLocked;

    /*!
     * \brief Filters queue
     * \details This queue is used in case a Filter during saving calls for Repository::save method. Since it is not save to 
     * save 2 filters simultaneously the newly saved Filters will be added to the queue
     */
    std::list<SPtr<Filter> > objectsSaveQueue;

    /*!
     * \brief A map from filter id to its cache
     * \details In case we call for the same filter twice Repository won't create a new object, it will
     * return a copy of the object created in the first call. We save a weak pointer to the created object.
     * So it will be freed when user don't need it
     */
    std::map<std::string, WPtr<Filter> > fromFilterIdToFilterCache;

private:

};
}
/** @}*/
#endif
