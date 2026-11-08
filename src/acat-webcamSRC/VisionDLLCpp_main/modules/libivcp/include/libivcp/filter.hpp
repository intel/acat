///////////////////////////////////////////////////////////////////////////
// <copyright file="filter.hpp" company="Intel Corporation">
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


#ifndef __FILTER_HPP__
#define __FILTER_HPP__

#include "libivcp/ifilter.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file filter.hpp
 * \brief Declaration of basic filter class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Basic filter class
 * \details Filter is the basic class for all entities of the library. Filter provides basic methods to set/get id
 * \sa Filter
 */
class IVCP_EXPORT Filter: public IFilter
{
public:
    /*
     * \brief Default constructor
     */
    Filter();

    virtual ~Filter() {};

    virtual void setId(std::string id);

    virtual std::string getId() const;

    virtual float getPerformanceRate() const;

    virtual void setPerformanceRate(float rate = 0);

protected:
    /*!
     * \brief Identifier of the filter
     */
    std::string filterID;

    /*!
     * \brief Performance rate of filter
     */
    float performanceRate;

private:

};
};

/** @}*/
#endif
