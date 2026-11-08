///////////////////////////////////////////////////////////////////////////
// <copyright file="xml_serializer.hpp" company="Intel Corporation">
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

#ifndef __XML_SERIALIZER_HPP__
#define __XML_SERIALIZER_HPP__

#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/irepository.hpp"
#include "libinfra/repository/ixml_serializer.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file xml_serializer.hpp
 * \brief Implementation of basic filter serialization functions
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Implementation of common functions of IXMLSerializer interface
 */
class IVCP_EXPORT XMLSerializer: public IXMLSerializer
{
public:
    /*!
     * \brief XMLSerializer constructor
     * \param[in] _rep Repository for to save and load objects
     * \throw IncorrectParamException Repository is NULL pointer
     */
    XMLSerializer(SPtr<IRepository> _rep);

    /*!
     * \brief XMLSerializer destructor
     */
    virtual ~XMLSerializer();

    virtual void saveMap(FilterState &filterData, cv::FileStorage &fs);

protected:
    /*!
     * \brief Repository for to save and load objects
     */
    WPtr<IRepository> rep;

    /*!
     * \brief Save string vector
     * \param[in] strVec String vector
     * \param[in] groupName Name of group
     * \param[in] valueName Name of every vector element
     * \param[in] fs OpenCV FileStorage where the vector shall be saved
     */
    virtual void saveStrVec(const std::vector<std::string > &strVec, std::string groupName, std::string valueName, cv::FileStorage &fs);

    /*!
     * \brief Load string vector
     * \param[in, out] strVec String vector
     * \param[in] groupName Name of group
     * \param[in] valueName Name of every vector element
     * \param[in] fs OpenCV FileStorage containing the vector to load
     * \param[in] fn OpenCV FileNode containing the vector to load
     */
    virtual void loadStrVec(std::vector<std::string> &strVec,  std::string groupName, std::string valueName, cv::FileStorage &fs, cv::FileNode fn);

private:

};
}
/** @}*/
#endif
