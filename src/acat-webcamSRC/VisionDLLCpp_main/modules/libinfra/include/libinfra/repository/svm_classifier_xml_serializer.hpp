///////////////////////////////////////////////////////////////////////////
// <copyright file="svm_classifier_xml_serializer.hpp" company="Intel Corporation">
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

#ifndef __SVM_CLASSIFIER_SERIALIZER_HPP__
#define __SVM_CLASSIFIER_SERIALIZER_HPP__

#include "libinfra/repository/filter_xml_serializer.hpp"
#include "libivcp/classifier/svm_params.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file svm_classifier_xml_serializer.hpp
 * \brief Implementation of SVMClassifier serialization 
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief SVMClassifier serialization 
 */
class IVCP_EXPORT SVMClassifierXMLSerializer: public FilterXMLSerializer
{
public:
    /*!
     * \brief Constructor
     * \param[in] _rep Repository for to save and load objects
     * \throw IncorrectParamException Repository is NULL pointer
     */
    SVMClassifierXMLSerializer(SPtr<IRepository> _rep);

    virtual ~SVMClassifierXMLSerializer();

    virtual SPtr<Filter> load(cv::FileStorage &fs, cv::FileNode fn, SPtr<Filter> filter = NULL);
    virtual void save(SPtr<Filter> filter, cv::FileStorage &fs); 
    virtual FilterState loadMap(cv::FileStorage &fs, cv::FileNode fn);    

    /*!
     * \brief Save SVM parameters as FileNode in FileStorage
     * \param[in] fs FileStorage where parameters shall be saved
     * \param[in] name Name of node with the parameters
     * \param[in] params SVM parameters to save
     */
    static void saveSVMParams(cv::FileStorage &fs, const std::string name, const SVMParameters &params);    

protected:
    virtual FilterState createMap(SPtr<Filter> filter); 
    /*!
     * \brief Load SVM parameters from a FileNode
     * \param[in] fs FileStorage where was saved parameters
     * \param[in] fn FileNode where was saved parameters
     * \return SVM parameters
     */
    SVMParameters loadSVMParams(cv::FileStorage &fs, cv::FileNode fn);

private:

};
}
/** @}*/
#endif
