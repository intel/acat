///////////////////////////////////////////////////////////////////////////
// <copyright file="repository_factory.hpp" company="Intel Corporation">
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

#ifndef __REPOSITORY_CREATOR_HPP__
#define __REPOSITORY_CREATOR_HPP__

#include "libivcp/irepository_factory.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file repository_factory.hpp
 * \brief RepositoryFactory creates a Repository object
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Implementation of the Repository factory
 * \details Currently creates XMLRepository for all URLs
 */
class IVCP_EXPORT RepositoryFactory: public IRepositoryFactory
{
public:
    virtual SPtr<IRepository> getRepository(std::string url);

};
}
/** @}*/
#endif
