///////////////////////////////////////////////////////////////////////////
// <copyright file="irepository_factory.hpp" company="Intel Corporation">
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


#ifndef __IREPOSITORY_FACTORY_HPP__
#define __IREPOSITORY_FACTORY_HPP__

#include <string>
#include "libivcp/sptr.hpp"
#include "libivcp/wptr.hpp"
#include "libivcp/irepository.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file irepository_factory.hpp
 * \brief Declaration of IRepositoryFactory interface 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Factory for the Repository
 * \details XMLRepository class is implemented in libinfra library. 
 * IRepository is a domain interface used in IVCP library. In order to get Repository on IVCP layer 
 * Factory class uses the IRepositoryFactory interface
 * \sa RepositoryFactory
 * \sa XMLRepository
 */
class IVCP_EXPORT IRepositoryFactory
{
public:
    /*!
     * \brief Destructor
     */ 
    virtual ~IRepositoryFactory() {};

    /*!
     * \brief Get correct implementation of the IRepository interface by repository url
     * \param[in] url URL of the Repository (filepath in case the XMLRepository is used)
     * \return Correct implementation of IRepository interface which can work with repository described by the url
     */ 
    virtual SPtr<IRepository> getRepository(std::string url) = 0;

};
}

/** @}*/
#endif
