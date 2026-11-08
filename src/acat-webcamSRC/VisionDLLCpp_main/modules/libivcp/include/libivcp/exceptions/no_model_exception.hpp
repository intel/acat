///////////////////////////////////////////////////////////////////////////
// <copyright file="no_model_exception.hpp" company="Intel Corporation">
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


#ifndef __NO_MODEL_EXCEPTION_HPP__
#define __NO_MODEL_EXCEPTION_HPP__

#include "libivcp/exceptions/exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file no_model_exception.hpp
 * \brief Exception thrown by a classifier in case it was asked to predic a class id by has no trained model
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Exception thrown by a classifier in case it was asked to predic a class id by has no trained model
 */
class IVCP_EXPORT NoModelException: public Exception
{
public:
    /*!
     * \brief Default constructor
     */
    NoModelException() {};

    /*!
     * \brief Constructor
     *\ param[in] msg Message describing what caused the exception
     *\ param[in] func Function where the exception was thrown
     *\ param[in] file File where the exception was thrown
     *\ param[in] line Line where the exception was thrown
     */
    NoModelException(const std::string& msg, const std::string& func, const std::string& file, int line):
        Exception(msg, func, file, line) {};

    /*!
     * \brief Destructor
     */
    virtual ~NoModelException() {};

};
}
/** @}*/
#endif
