///////////////////////////////////////////////////////////////////////////
// <copyright file="ixml_serializer.hpp" company="Intel Corporation">
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

#ifndef __IXML_SERIALIZER_HPP__
#define __IXML_SERIALIZER_HPP__

#include <string>
#include <map>
#include "libivcp/filter.hpp"
#include "libivcp/wptr.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/exceptions/cast_exception.hpp"
#include "libivcp/exceptions/no_data_exception.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/classifier/svm_model.hpp"
#include "libivcp/classifier/lr_model.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file ixml_serializer.hpp
 * \brief Declaration of basic filter serialization interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*! 
 * \brief Basic interface of all serialization classes
 * \details The serializer classes are used exclusively by the Repository.
 * If this serializer is desired for loading an object of type A derived from B it is recommended to derive
 * serializer of A from the serializer of B.
 * Please read the detailed instructions on creating a serializer for your own Filter in the User guide
 */
class IVCP_EXPORT IXMLSerializer
{
public:
    /*! 
     * \brief This class can store different types which could be used by IVCP filter
     * \details This is a service classes used by loadMap and saveMap function. All datatypes
     * used by the IVCP filters shall be described by one of SerializeType fields
     */
    struct SerializeData
    {
    public:
        /*!
         * \brief Types that can be serialized
         */
        enum SerializeType
        {
            INT, 
            FLOAT, 
            DOUBLE, 
            MAT, 
            STRING, 
            ML_SVM, 
            STRING_VEC,
            SVM_PARAMS,
            ML_LR,
            LR_PARAMS
        };

        /*!
         * \brief Constructor
         */
        SerializeData(SerializeType _type = INT,
            int _ival = 0, 
            float _fval = 0.0f, 
            double _dval = 0.0, 
            std::string _str = "", 
            cv::Mat _mat = cv::Mat(), 
            SPtr<SVMModel> _svm = 0, 
            std::vector<std::string> _strVec = std::vector<std::string>(),
            SVMParameters _svmParams = SVMParameters(),
            SPtr<LRModel> _lr = 0,            
			LRParameters _lrParams = LRParameters()
)
        {
			init(_type, _ival, _fval, _dval, _str, _mat, _svm, _strVec, _svmParams, _lr, _lrParams);
        };

        /*!
         * \brief Constructor
         * \param[in] _ival Integer value that should be serialized
         */
        SerializeData(int _ival)
        {
            init(INT, _ival);
        };

        /*!
         * \brief Constructor
         * \param[in] _fval Float value that should be serialized
         */
        SerializeData(float _fval)
        {
            init(FLOAT, 0, _fval);
        };

        /*!
         * \brief Constructor
         * \param[in] _dval Double value that should be serialized
         */
        SerializeData(double _dval)
        {
            init(DOUBLE, 0, 0.0f, _dval);
        };

        /*!
         * \brief Constructor
         * \param[in] _str String that should be serialized
         */
        SerializeData(std::string _str)
        {
            init(STRING, 0, 0.0f, 0.0, _str);
        };

        /*!
         * \brief Constructor
         * \param[in] _mat Matrix that should be serialized
         */
        SerializeData(cv::Mat _mat)
        {
            init(MAT, 0, 0.0f, 0.0, "", _mat);
        };

        /*!
         * \brief Constructor
         * \param[in] _svm SVM classifier model that should be serialized
         */
        SerializeData(SPtr<SVMModel> _svm)
        {
            init(ML_SVM, 0, 0.0f, 0.0, "", cv::Mat(), _svm);
        };
        
        /*!
         * \brief Constructor
         * \param[in] _lr LR classifier model that should be serialized
         */
        SerializeData(SPtr<LRModel> _lr)
        {
            init(ML_LR, 0, 0.0f, 0.0, "", cv::Mat(), 0, std::vector<std::string>(), SVMParameters(), _lr);
        };

        /*!
         * \brief Constructor
         * \param[in] _strVec String vector that should be serialized
         */
        SerializeData(const std::vector<std::string> &_strVec)
        {
            init(STRING_VEC, 0, 0.0f, 0.0, "", cv::Mat(), 0, _strVec);
        }

        /*!
         * \brief Constructor
         * \param[in] _svmParams SVM parameters that should be serialized
         */
        SerializeData(const SVMParameters &_svmParams)
        {
            init(SVM_PARAMS, 0, 0.0f, 0.0, "", cv::Mat(), 0, std::vector<std::string>(), _svmParams);
        }

        /*!
         * \brief Constructor
         * \param[in] _lrParams LR parameters that should be serialized
         */
        SerializeData(const LRParameters &_lrParams)
        {
            init(LR_PARAMS, 0, 0.0f, 0.0, "", cv::Mat(), 0, std::vector<std::string>(), SVMParameters(), 0, _lrParams);
        }

        /*!
         * \brief Integer value
         */
        int ival;

        /*!
         * \brief Float value
         */
        float fval;

        /*!
         * \brief Double value
         */
        double dval;

        /*!
         * \brief String
         */
        std::string str;

        /*!
         * \brief Matrix
         */
        cv::Mat mat;

        /*!
         * \brief SVM classifer model
         */
        SPtr<SVMModel> svm;

        /*!
         * \brief SVM classifer model
         */
        SPtr<LRModel> lr;
        
        /*!
         * \brief SVM parameters
         */
        SVMParameters svmParams;

        /*!
         * \brief LR parameters
         */
        LRParameters lrParams;

        /*!
         * \brief String vector
         */
        std::vector<std::string> strVec;

        /*!
         * \brief Type of data that should be serialized
         */
        SerializeType type;

    protected:
        /*!
         * \brief Initialization of structure fields
         */
        void init(SerializeType _type = INT,
            int _ival = 0, 
            float _fval = 0.0f, 
            double _dval = 0.0, 
            std::string _str = "", 
            cv::Mat _mat = cv::Mat(), 
            SPtr<SVMModel> _svm = 0, 
            std::vector<std::string> _strVec = std::vector<std::string>(),
            SVMParameters _svmParams = SVMParameters(),
            SPtr<LRModel> _lr = 0,
			LRParameters _lrParams = LRParameters()
)
        {
             type = _type;
             ival = _ival;
             fval = _fval;
             dval = _dval;
             str = _str;
             mat = _mat;
             svm =_svm;
             strVec = _strVec;
             svmParams = _svmParams;
             lr =_lr;
             lrParams = _lrParams;
        }
    };
    
    /*!
     * \brief The structure that can be used to describe an arbitrary IVCP Filter
     */
    typedef std::map<std::string, SerializeData> FilterState;

    /*! 
     * \brief Destructor
     */
    virtual ~IXMLSerializer() {};

    /*! 
     * \brief Load filter from XML file
     * \param[in] fs OpenCV FileStorage containing the Filter to load
     * \param[in] fn OpenCV FileNode containing the Filter to load
     * \param[in] filter Optional parameter with partially loaded Filter
     * If filter parameter is NULL the load function will create a new instance of Filter.
     * If filter is not NULL the load function casts the filter to appropriate class and updates its content
     * \details Recommended steps in implementing the function:
     * - if filter is NULL, create a new Filter object with using the Factory
     * - call for the load method of the parent class with the newly created filter so that parent data is loaded
     * - load data specific for A from the FileStorage
     * - if the filter contains references to other IVCP filters load them from the Repository by id
     * \return Filter loaded from the storage 
     */
    virtual SPtr<Filter> load(cv::FileStorage &fs, cv::FileNode fn, SPtr<Filter> filter = NULL) = 0;

    /*! 
     * \brief Save filter to XML file
     * \param[in] filter Filter to save
     * \param[in] fs OpenCV FileStorage where the filter shall be saved
     * \details Recommended steps in implementing the function:
     * - create a description of the Filter with using the createMap function
     * - save the map with using the saveMap function
     * - if the Filter contains references to other IVCP Filters save them with using Repository::save method
     */
    virtual void save(SPtr<Filter> filter, cv::FileStorage &fs) = 0;

    /*! 
     * \brief Load an auxiliary structure that fully describes state of a Filter
     * \details loadMap shall load all fields describing the Filter so that later the saveMap function can generate 
     * the same XML description of the Filter
     * \param[in] fs OpenCV FileStorage containing the Filter to load
     * \param[in] fn OpenCV FileNode containing the Filter to load
     * \details Recommended steps in implementing the function:
     * - call for loadMap method of the parent serializer
     * - add Filter-specific fields to the description
     * \return Description of the Filter
     */
    virtual FilterState loadMap(cv::FileStorage &fs, cv::FileNode fn) = 0;

    /*! 
     * \brief Saves the XML description of a Filter based on filterData auxiliary structure loaded by the loadMap function
     * \details saveMap shall generate the same XML description of a Filter from filterData as 
     * the save function would generate based on the original filter
     * \param[in] filterData an auxiliary structure created by the loadMap function that fully describes the Filter
     * \param[in] fs OpenCV FileStorage where the filter shall be saved
     * \details Recommended steps in implementing the function:
     * - if your Filter description contains only the following types: SerializeData::INT, SerializeData::FLOAT, 
     * SerializeData::DOUBLE, SerializeData::MAT, SerializeData::STRING, SerializeData::ML_SVM, SerializeData::ML_LR
     * there is no need to implement saveMap function - those types are saved by default in XMLSerializer class
     * - for the rest types you shall add code to save them to the FileStorage
     * - you can create a new Filter state with the except of the saved parameters and call for parent saveMap function
     * to save the standard types
     */
    virtual void saveMap(FilterState &filterData, cv::FileStorage &fs) = 0;

protected:    
    /*! 
     * \brief Create an structure that describes the Filter identical to the structure generated by loadMap function
     * \param[in] filter IVCP Filter
     * \details Recommended steps in implementing the function:
     * - call for createMap method of the parent serializer
     * - add Filter-specific fields to the description
     * \return The outpt structure described the original filter. Format of the output structure is identical 
     * to the format of the structure used by the loadMap and saveMap functions
     */
    virtual FilterState createMap(SPtr<Filter> filter) = 0;

};
}

/** @}*/
#endif
