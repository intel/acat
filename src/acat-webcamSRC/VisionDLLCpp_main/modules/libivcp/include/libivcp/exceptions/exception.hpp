///////////////////////////////////////////////////////////////////////////
// <copyright file="exception.hpp" company="Intel Corporation">
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


#ifndef __EXCEPTION_HPP__
#define __EXCEPTION_HPP__

#include <string>
#include "libivcp/log.hpp"

#ifdef _MSC_VER
//Avoid warning that VS does not implement exception specification
#pragma warning( disable : 4290 )
#endif

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file exception.hpp
 * \brief Exception base class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Exception base class
 */
#ifdef _MSC_VER
// std::exception is provided by the MSVC runtime, not exported by this DLL.
#pragma warning(push)
#pragma warning(disable: 4275)
#endif
class IVCP_EXPORT Exception: public std::exception
{
public:
    /*!
     * \brief Default constructor
     */
    Exception();

    /*!
     * \brief Constructor
     *\ param[in] msg Message describing what caused the exception
     *\ param[in] func Function where the exception was thrown
     *\ param[in] file File where the exception was thrown
     *\ param[in] line Line where the exception was thrown
     */
    Exception(const std::string& msg, const std::string& func, const std::string& file, int line);

    /*!
     * \brief Destructor
     */
    virtual ~Exception();

    /*!
     * \brief Return formatted message describing what caused the exception and where it was thrown
     * \return Formatted message describing what caused the exceptionand where it was thrown
     */
    virtual std::string getMessage() const;

    /*!
     * \brief Return non-formatted message string
     * \return Non-formatted message string
     */
    virtual const char* what() const;

    /*!
     * \brief Message describing what caused the exception
     */
    std::string message; 

    /*!
    * \brief Function where the exception was thrown
    */
    std::string functionName; 

    /*!
     * \brief File where the exception was thrown
     */
    std::string fileName; 
    
    /*!
     * \brief Line where the exception was thrown
     */
    int lineNumber; 
};
#ifdef _MSC_VER
#pragma warning(pop)
#endif

/*!
 * \brief Throw exception with given type and given message
 */
#ifdef __GNUC__
#define IVCP_EXCEPTION(exception, msg ) { IVCP_LOG_ERROR(msg); \
                                          throw ( exception(msg, __func__, __FILE__, __LINE__) ); }
#else
#define IVCP_EXCEPTION(exception, msg ) { IVCP_LOG_ERROR(msg); \
                                          throw ( exception(msg, "", __FILE__, __LINE__) ); }
                                          
#endif

};

/** @}*/
#endif
