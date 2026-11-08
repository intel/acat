///////////////////////////////////////////////////////////////////////////
// <copyright file="idata_source_factory.hpp" company="Intel Corporation">
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


#ifndef __IDATASOURCE_FACTORY_HPP__
#define __IDATASOURCE_FACTORY_HPP__

#include <string>
#include "libivcp/sptr.hpp"
#include "libivcp/data_source/iclassifier_result_data_source.hpp"
#include "libivcp/data_source/iregion_data_source.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file idata_source_factory.hpp
 * \brief Interface for data source factory
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Interface for data source factory
 * \details DataSourceFactory class is implemented in libinfra library. 
 * IDataSource is a domain interface used in IVCP library. In order to get Data source on IVCP layer 
 * Factory class uses the IDataSourceFactory interface
 * \sa DataSourceFactory
 * \sa DataSource
 */
class IVCP_EXPORT IDataSourceFactory
{
public:
    /*!
    * \brief Destructor
    */ 
    virtual ~IDataSourceFactory() {};

    /*!
     * \brief Create ClassifierResultDataSource
     * \param[in] id Identifier of data source
     * \return Classifier results data source
     */
    virtual SPtr<IClassifierResultDataSource> getClassifierResultDataSource(std::string id = "") = 0;

    /*!
     * \brief Create RegionDataSource
     * \param[in] id Identifier of data source
     * \return Region data source
     */
    virtual SPtr<IRegionDataSource> getRegionDataSource(std::string id = "") = 0;

};
}
/** @}*/
#endif
