////////////////////////////////////////////////////////////////////////////
// <copyright file="classifer_result_data_source.hpp" company="Intel Corporation">
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

#ifndef __XML_CLASSIFIER_RESULTS_HPP__
#define __XML_CLASSIFIER_RESULTS_HPP__

#include <list>
#include <map>
#include "libivcp/data_source/iclassifier_result_data_source.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*! 
 * \defgroup libinfra IVCP Infrastructure Library 
 * \brief IVCP infrastructure library
 * @{
 */

/*! 
 * \file classifier_result_data_source.hpp
 * \brief Declaration of class to read and save classification results (ie ground truth) in XML format
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Class to read and save classification results in XML format
 * \details Classification results for the set of frames, saving and reading them
 */
class IVCP_EXPORT ClassifierResultDataSource: public IClassifierResultDataSource
{
public:
    /*!
     * \brief The structure that can be used to describe a frame range
     * \details Contains the bounds of frame range
     */
    struct FrameRange
    {
        /*!
         * \brief FrameRange constructor
         * \param[in] _first Left bound of frame range
         * \param[in] _last Right bound of frame range
         */
        FrameRange(int _first = 0, int _last = 0):
            firstFrame(_first), lastFrame(_last)
        {};

        /*!
         * \brief Left bound of frame range
         */
        int firstFrame;
        /*!
         * \brief Right bound of frame range
         */
        int lastFrame;
    };

    /*!
     * \brief The structure that can be used to describe a marked frame range
     * \details It is pair of frame range and corresponding class
     */
    struct MarkedRange
    {
        /*!
         * \brief MarkedRange constructor
         * \param[in] _range Frame range
         * \param[in] _classId Corresponding class
         */
        MarkedRange(FrameRange _range = FrameRange(), ClassType _classId = 0):
            range(_range), classId(_classId) 
        {};
        
        /*!
         * \brief Frame range
         */
        FrameRange range;
        /*!
         * \brief Corresponding class
         */
        ClassType classId;
    };

    /*!
     * \brief The structure that can be used to describe a list of marked frame ranges
     */
    typedef std::list <MarkedRange> ListOfMarkedFrameRanges;

    virtual ClassType getClassifyResult(VideoPlaylistFrame frame);
    virtual bool isClassified(VideoPlaylistFrame frame);

    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType mark);
    virtual void removeClassifyResult(VideoPlaylistFrame frame);

    /*!
     * \brief Save classification results to XML file
     * \param[in] fs OpenCV FileStorage where classifier results shall be saved
     * \throw IOException Unable to save classifier results to this file storage
     */
    virtual void save(cv::FileStorage& fs);

    /*!
     * \brief Load classification results from XML file
     * \param[in] fs OpenCV FileStorage containing the ClassifierResultDataSource to load
     * \param[in] fn OpenCV FileNode containing the ClassifierResultDataSource to load
     * \throw IOException Unable to load classifier results from this file storage
     */
    virtual void load(cv::FileStorage &fs, cv::FileNode fn);

    /*!
    * \brief Save content of the serialization matrix to the File storage
    * \details Serialization matrix is used by XMLSerializer classes
    * \param[out] data Serialization matrix
    * \param[in, out] fs File Storage where content of the serialization matrix is saved
    * \throw IOException Unable to save serialization matrix to the file storage
    */
    static void saveSerializeMat(const cv::Mat& data, cv::FileStorage&fs);

    /*!
    * \brief Load serialization matrix from File storage
    * \details Serialization matrix is used by XMLSerializer classes 
    * \param[in] fn FileNode that contains the serialization parameters
    * \param[in] fs File Storage that contains the serialization parameters
    * \return Serialization matrix
    * \throw IOException Unable to load serialization matrix from the file storage
    */
    static cv::Mat loadSerializeMat(cv::FileStorage &fs, cv::FileNode fn);

    virtual ClassifierResultType getType();

protected:
    //Markup list should be ordered in accordance with frameRange locations in a frame sequence

    /*!
     * \brief Markup for the video frame ranges
     * \details Map contains the pairs of video source index and list of marked frame ranges
     */
    std::map <int, ListOfMarkedFrameRanges > frameMarkup;

    /*!
     * \brief For each source index ->  last requested frame (this speeds up searching in case of request to sequential frames)
     */
    std::map <int, ListOfMarkedFrameRanges::iterator > lastCheck;

private:

};
};

/** @}*/
#endif
