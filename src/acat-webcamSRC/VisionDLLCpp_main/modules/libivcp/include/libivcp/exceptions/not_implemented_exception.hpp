// Copyright (C) 2012, Intel Corporation, all rights reserved.

#ifndef __NOT_IMPLEMENTED_EXCEPTION_HPP__
#define __NOT_IMPLEMENTED_EXCEPTION_HPP__

#include "libivcp/exceptions/exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file no_model_exception.hpp
 * \brief Exception thrown by a method if it is not implemented yet (for example serializer for a newly added filter is not implemented)
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Exception thrown by a method if it is not implemented yet (for example serializer for a newly added filter is not implemented)
 */
class IVCP_EXPORT NotImplementedException: public Exception
{
public:
    /*!
     * \brief Default constructor
     */
    NotImplementedException() {};

    /*!
     * \brief Constructor
     *\ param[in] msg Message describing what caused the exception
     *\ param[in] func Function where the exception was thrown
     *\ param[in] file File where the exception was thrown
     *\ param[in] line Line where the exception was thrown
     */
    NotImplementedException(const std::string& msg, const std::string& func, const std::string& file, int line):
        Exception(msg, func, file, line) {};

    /*!
     * \brief Destructor
     */
    virtual ~NotImplementedException() {};

};
}
/** @}*/
#endif
