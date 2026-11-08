///////////////////////////////////////////////////////////////////////////
// <copyright file="cast_exception.hpp" company="Intel Corporation">
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


#ifndef __CAST_EXCEPTION_HPP__
#define __CAST_EXCEPTION_HPP__

#include "libivcp/exceptions/exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file cast_exception.hpp
 * \brief Exception of incorrect casting
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Exception of incorrect casting
 */
class IVCP_EXPORT CastException: public Exception
{
public:
    /*!
     * \brief Default constructor
     */
    CastException() {};

    /*!
     * \brief Constructor
     *\ param[in] msg Message describing what caused the exception
     *\ param[in] func Function where the exception was thrown
     *\ param[in] file File where the exception was thrown
     *\ param[in] line Line where the exception was thrown
     */
    CastException(const std::string& msg, const std::string& func, const std::string& file, int line):
        Exception(msg, func, file, line) {};

    /*!
     * \brief Destructor
     */
    virtual ~CastException() {};

};
}

/** @}*/
#endif
