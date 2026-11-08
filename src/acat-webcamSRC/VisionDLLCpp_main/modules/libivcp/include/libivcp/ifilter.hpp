///////////////////////////////////////////////////////////////////////////
// <copyright file="ifilter.hpp" company="Intel Corporation">
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


#ifndef __IFILTER_HPP__
#define __IFILTER_HPP__

#include <string>
#include "libivcp/types.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file ifilter.hpp
 * \brief Declaration of basic filter interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Basic filter interface
 * \details IFilter is the basic interface for all entities of the library. IFilter provides basic methods to set/get id
 * \sa Filter
 */
class IVCP_EXPORT IFilter
{
public:
    /*!
    * \brief Destructor
    */
    virtual ~IFilter() {};

    /*!
    * \brief Set id for the filter
    * \param[in] id Identifier of the filter
    */
    virtual void setId(std::string id) = 0;

    /*!
    * \brief Get id of the filter
    * \return Identifier of the filter
    */
    virtual std::string getId() const = 0;

    /*!
     * \brief Return type of a filter (ie Classifier, Feature etc)
     * \return Filter type
     */
    virtual FilterType getRootType() const = 0;

    /*!
     * \brief Return performance rate of a filter in frames per second
     * \return Performance rate or zero if it's not set
     */    
    virtual float getPerformanceRate() const = 0;

    /*!
     * \brief Set performance rate for the filter
     * \param[in] rate New performance rate
     */  
    virtual void setPerformanceRate(float rate = 0) = 0;

protected:

private:

};
};

/** @}*/
#endif
