///////////////////////////////////////////////////////////////////////////
// <copyright file="parallel_pipeline_serializer.hpp" company="Intel Corporation">
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

#ifndef __PARALLEL_PIPELINE_SERIALIZER_HPP__
#define __PARALLEL_PIPELINE_SERIALIZER_HPP__

#include "libinfra/repository/pipeline_serializer.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file parallel_pipeline_serializer.hpp
 * \brief Implementation of ParallelPipeline serialization 
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief ParallelPipeline serialization 
 */
class IVCP_EXPORT ParallelPipelineXMLSerializer: public PipelineXMLSerializer
{
public:
    /*!
     * \brief Constructor
     * \param[in] _rep Repository for to save and load objects
     * \throw IncorrectParamException Repository is NULL pointer
     */
    ParallelPipelineXMLSerializer(SPtr<IRepository> _rep);

    virtual ~ParallelPipelineXMLSerializer();

    virtual SPtr<Filter> load(cv::FileStorage &fs, cv::FileNode fn, SPtr<Filter> filter = NULL);

};
}

/** @}*/
#endif
