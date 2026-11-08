///////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_data_source_serializer.hpp" company="Intel Corporation">
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

#ifndef __GT_REGION_DATA_SOURCE_XML_SERIALIZER_HPP__
#define __GT_REGION_DATA_SOURCE_XML_SERIALIZER_HPP__

#include "libinfra/repository/filter_xml_serializer.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file gt_region_data_source_serializer.hpp
 * \brief Implementation of GTRegionDataSource serialization
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief GTRegionDataSource serialization 
 */
class IVCP_EXPORT GTRegionDataSourceXMLSerializer: public FilterXMLSerializer
{
public:
    /*!
     * \brief XMLSerializer constructor
     * \param[in] _rep Repository for to save and load objects
     * \throw IncorrectParamException Repository is NULL pointer
     */
    GTRegionDataSourceXMLSerializer(SPtr<IRepository> _rep);

    virtual ~GTRegionDataSourceXMLSerializer();

    virtual SPtr<Filter> load(cv::FileStorage &fs, cv::FileNode fn, SPtr<Filter> filter = NULL);
    virtual void save(SPtr<Filter> filter, cv::FileStorage &fs); 

    virtual FilterState loadMap(cv::FileStorage &fs, cv::FileNode fn);
    virtual void saveMap(FilterState &filterData, cv::FileStorage &fs);

protected:

private:

};
}

/** @}*/
#endif
