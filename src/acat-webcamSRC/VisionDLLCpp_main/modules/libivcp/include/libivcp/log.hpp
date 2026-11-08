///////////////////////////////////////////////////////////////////////////
// <copyright file="log.hpp" company="Intel Corporation">
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


#ifndef __LOG_HPP__
#define __LOG_HPP__

#include <string>
#include "libivcp/types.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file log.hpp
 * \brief Declaration of Log class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Log is responsible for logging error, warning and info messages
 * \details Log builds message with using given parameters and writes them. 
 * You can choose a file or the console to write messages.
 */
class IVCP_EXPORT Log
{
public:
     /*!
     * \brief Builds message and writes it to the file or to the console
     * \param[in] level Logging level. There is three possible values of this parameter: 
     * LOG_INFO, LOG_WARNING, LOG_ERROR
     * \param[in] msg A format string that follows the same specifications as format in printf
     * \param[in] func Function where method was called
     * \param[in] file File where method was called
     * \param[in] line Line where method was called
     * \param[in] ... Parameters for the format string msg
     */
    static void log(LogLevel level, const std::string &msg, const std::string &func, const std::string &file, int line, ...);

    /*!
     * \brief Redirects log messages to the file
     * \param[in] file Path to the log file
     */
    static void logToFile(const std::string &file); 

    /*!
     * \brief Redirects log messages to the console
     */
    static void logToConsole();
    /*!
     * \brief Sets verbosity level
     * \details All messages with level less than verbosity level will be ignored
     * \param[in] level Verbosity level
     */
    static void setVerbosityLevel(LogLevel level);

protected:

private:
    static void writeMessageToFile(const std::string &msg);
    static void writeMessageToConsole(const std::string &msg);

    static std::string logFile;
    static bool writeToFile;
    static LogLevel verbosityLevel;
};

/*!
 * \brief Log message with given level
 */
#ifdef __GNUC__
#define IVCP_LOG(level, msg, ...) Log::log(level, msg, __func__, __FILE__, __LINE__, ##__VA_ARGS__)
#else
#define IVCP_LOG(level, msg, ...) Log::log(level, msg, "", __FILE__, __LINE__, ##__VA_ARGS__)
#endif

/*!
 * \brief Log info message
 */
#define IVCP_LOG_INFO(msg, ...) IVCP_LOG(LOG_INFO, msg, ##__VA_ARGS__)

/*!
 * \brief Log warning message
 */
#define IVCP_LOG_WARNING(msg, ...) IVCP_LOG(LOG_WARNING, msg, ##__VA_ARGS__)

/*!
 * \brief Log error message
 */
#define IVCP_LOG_ERROR(msg, ...) IVCP_LOG(LOG_ERROR, msg, ##__VA_ARGS__)

/*!
 * \brief Redirects log messages to the file
 */
#define IVCP_LOG_TO_FILE(file) Log::logToFile(file)

/*!
 * \brief Redirects log messages to the console
 */
#define IVCP_LOG_TO_CONSOLE() Log::logToConsole()

/*!
 * \brief Sets verbosity level, all messages with level less than verbosity level will be ignored
 */
#define IVCP_LOG_VERBOSITY_LEVEL(level) Log::setVerbosityLevel(level)

};
/** @}*/
#endif
