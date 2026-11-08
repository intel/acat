///////////////////////////////////////////////////////////////////////////
// <copyright file="pipeline_types.hpp" company="Intel Corporation">
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


#ifndef __PIPELINE_TYPES_HPP__
#define __PIPELINE_TYPES_HPP__

#ifndef IVCP_EXPORT
#ifdef IVCP_API_EXPORTS
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#define IVCP_EXPORT __declspec(dllexport)
#else
#define IVCP_EXPORT
#endif
#else
#define IVCP_EXPORT
#endif
#endif

#ifdef _MSC_VER
#pragma warning( disable : 4251 )
#endif

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file pipeline_types.hpp
 * \brief Declaration of pipeline datatypes
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Pipeline filter types (ie pipeline filter levels)
 */
enum NodeType
{
    UNDEFINED_NODE,
    SOURCE_NODE,
    REGION_NODE,
    FEATURE_NODE,
    CLASSIFICATOR_NODE,
    PERFORMANCE_NODE,
    DATASOURCE_NODE
};

};

/** @}*/
#endif

