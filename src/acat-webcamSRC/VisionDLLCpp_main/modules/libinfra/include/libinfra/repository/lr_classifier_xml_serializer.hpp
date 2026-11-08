///////////////////////////////////////////////////////////////////////////
// <copyright file="lr_classifier_xml_serializer.hpp" company="Intel Corporation">
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

#ifndef __LR_CLASSIFIER_SERIALIZER_HPP__
#define __LR_CLASSIFIER_SERIALIZER_HPP__

#include "libinfra/repository/filter_xml_serializer.hpp"
#include "libivcp/classifier/lr_params.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file lr_classifier_xml_serializer.hpp
 * \brief Implementation of LRClassifier serialization 
 * \copyright (C) 2013, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief LRClassifier serialization 
 */
class IVCP_EXPORT LRClassifierXMLSerializer: public FilterXMLSerializer
{
public:
    /*!
     * \brief Constructor
     * \param[in] _rep Repository for to save and load objects
     * \throw IncorrectParamException Repository is NULL pointer
     */
    LRClassifierXMLSerializer(SPtr<IRepository> _rep);

    virtual ~LRClassifierXMLSerializer();

    virtual SPtr<Filter> load(cv::FileStorage &fs, cv::FileNode fn, SPtr<Filter> filter = NULL);
    virtual void save(SPtr<Filter> filter, cv::FileStorage &fs); 
    virtual FilterState loadMap(cv::FileStorage &fs, cv::FileNode fn);    

    /*!
     * \brief Save LR parameters as FileNode in FileStorage
     * \param[in] fs FileStorage where parameters shall be saved
     * \param[in] name Name of node with the parameters
     * \param[in] params LR parameters to save
     * \throw IncorrectParamException Repository is NULL pointer
     */
    static void saveLRParams(cv::FileStorage &fs, const std::string name, const LRParameters &params);    

protected:
    virtual FilterState createMap(SPtr<Filter> filter); 
    /*!
     * \brief Load LR parameters from a FileNode
     * \param[in] fs FileStorage where was saved parameters
     * \param[in] fn FileNode where was saved parameters
     * \return LR parameters
     */
    LRParameters loadLRParams(cv::FileStorage &fs, cv::FileNode fn);

private:

};
}
/** @}*/
#endif
