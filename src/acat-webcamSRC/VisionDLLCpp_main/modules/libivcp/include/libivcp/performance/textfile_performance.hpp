///////////////////////////////////////////////////////////////////////////
// <copyright file="textfile_performance.hpp" company="Intel Corporation">
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


#ifndef __TEXTFILE_PERFORMANCE_HPP__
#define __TEXTFILE_PERFORMANCE_HPP__

#include <string>
#include "libivcp/performance/performance.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file textfile_performance.hpp
 * \brief Declaration of class for performance and quality report generating and saving them in a text file
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Class for generating of performance and quality reports and saving them in a text file
 * \sa Performance
 */
class IVCP_EXPORT TextPerformance: public Performance
{
public:
    virtual void generateQualityReport(std::string filename);
    virtual void generatePerformanceReport(std::string filename);
    virtual PerformanceType getType();

protected:

private:

};
};

/** @}*/
#endif
