///////////////////////////////////////////////////////////////////////////
// <copyright file="iperformance.hpp" company="Intel Corporation">
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


#ifndef __IPERFORMANCE_HPP__
#define __IPERFORMANCE_HPP__

#include <string>
#include "libivcp/filter.hpp"
#include "libivcp/sptr.hpp"
#include "libivcp/video_source/video_playlist_frame.hpp"
#include "libivcp/data_source/iclassifier_result_data_source.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file iperformance.hpp
 * \brief Declaration of performance and quality report generating interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic interface for generating of performance and quality reports
 * \sa Performance
 * \sa TextPerformance
 */
class IVCP_EXPORT IPerformance: public Filter
{
public:
    /*!
     * \brief Start named timer
     * \details startTimer() and stopTimer() functions are used for performance reports.
     * Filters shall call for these functions with unick names which would be later used in timing reports
     * \param[in] timerName Name of timer
     */
    virtual void startTimer(std::string timerName) = 0;

    /*!
     * \brief Stop named timer
     * \param[in] timerName Name of timer
     */
    virtual void stopTimer(std::string timerName) = 0;

    /*!
     * \brief Save classification report for quality report
     * \details Specific implementation of IPerformance shall have reference to a Data Source reader of ground truth classification data
     * \param[in] frame Frame index in Video Playlist
     * \param[in] classType Classification result of tested pipeline
     * \throw IncorrectParamException No Data Source is set
     */
    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType classType) = 0;

    /*!
     * \brief Set a Data Source reader of ground truth classification data
     * \param[in] markup Pointer to classifier result data source reader
     */
    virtual void setMarkup(SPtr<IClassifierResultDataSource> markup) = 0;

    /*!
     * \brief Check if this Performance Filter has a Classifier Result Data Source set
     * \return true if the Data Source is set, false otherwise
     */
    virtual bool hasMarkup() = 0;

    /*!
     * \brief Save quality report to a file
     * \param[in] filename Report filename
     */
    virtual void generateQualityReport(std::string filename) = 0;

    /*!
     * \brief Save performance report to a file
     * \param[in] filename Report filename
     */
    virtual void generatePerformanceReport(std::string filename) = 0;

    /*!
     * \brief Get type of performance
     * \return Performance type
     */
    virtual PerformanceType getType() = 0;

    /*!
     * \brief Clean Performance filter statistics
     */
    virtual void clear() = 0;

    /*!
    * \brief Get average time of filter work
    * \param[in] filterId Identifier of filter
    * \return Average time of work in ms
    */
    virtual double getAverageTime(std::string filterId) = 0;

     /*!
    * \brief Get max time of filter work
    * \param[in] filterId Identifier of filter
    * \return Max time of work in ms
    */
    virtual double getMaxTime(std::string filterId) = 0;

     /*!
    * \brief Get total time of filter work
    * \param[in] filterId Identifier of filter
    * \return Total time of work in ms
    */
    virtual double getTotalTime(std::string filterId) = 0;

    /*!
     * \brief Get total precision of classification
     * \details Count of correct classification results / Total count of classification results
     * \return Precision of classification
     */
    virtual float getTotalPrecision() = 0;

    /*!
    * \brief Get precision of classification for the given class
    * \details Count of correct classification results with the given class / Total count of classification results with the given class
    * \param[in] cl Class
    * \return Precision of classification for the given class
    */
    virtual float getPrecisionByClass(ClassType cl) = 0;

    /*!
    * \brief Get recall for the given class
    * \details Count of correct classification results with the given class / Total count of elements with the given class in markup
    * \param[in] cl Class
    * \return Recall for the given class
    */
    virtual float getRecallByClass(ClassType cl) = 0;

     /*!
     * \brief Get confusion map
     * \details The first parameter is classes pair, second parameter is 
     * Count of elements of the first class that were classified as the second class / Total count of elements of the first class
     * \return Confusion map 
     */
    virtual std::map< std::pair<ClassType, ClassType>, float> getConfusionMap() = 0;

protected:

private:

};
};

/** @}*/
#endif
