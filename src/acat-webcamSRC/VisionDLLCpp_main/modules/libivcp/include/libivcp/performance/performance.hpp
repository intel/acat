///////////////////////////////////////////////////////////////////////////
// <copyright file="performance.hpp" company="Intel Corporation">
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


#ifndef __PERFORMANCE_HPP__
#define __PERFORMANCE_HPP__

#include <string>
#include <set>
#include <mutex>
#include "libivcp/performance/iperformance.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file performance.hpp
 * \brief Declaration of performance and quality report generating basic class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic class for generating of performance and quality reports
 * \sa IPerformance
 * \sa TextPerformance
 */
class IVCP_EXPORT Performance: public IPerformance
{
public:
    /*!
     * \brief Default constructor
     */
    Performance();

    virtual void startTimer(std::string timerName);
    virtual void stopTimer(std::string timerName);

    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType classType);
    virtual void setMarkup(SPtr<IClassifierResultDataSource> markup);
    virtual bool hasMarkup();

    /*!
     * \brief Set class name for given class type
     * \param[in] classType Class type
     * \param[in] typeName Name for given class
     */
    virtual void setClassName(ClassType classType, std::string typeName);

    /*!
     * \brief Get names for all classes
     * \return Set of class type and class name pairs
     */
    std::map<ClassType, std::string> &getClassNamesMap();

    /*!
     * \brief Get a Data Source reader of ground truth classification data
     * \return Pointer to classifier result data source reader
     */
    SPtr<IClassifierResultDataSource> getClassMarkup();

    virtual FilterType getRootType() const;

    virtual void clear();

    virtual double getAverageTime(std::string filterId);
    virtual double getMaxTime(std::string filterId);
    virtual double getTotalTime(std::string filterId);

    virtual float getTotalPrecision();
    virtual float getPrecisionByClass(ClassType cl);
    virtual float getRecallByClass(ClassType cl);
    virtual std::map< std::pair<ClassType, ClassType>, float> getConfusionMap();

protected:
    /*!
     * \brief Set of the active timers contained timer name and time pair
     */
    std::map<std::string, double> activeTimers;

    /*!
     * \brief Statistic for the every timer
     * \details Map contains timer name and vector of start timer and stop timer pairs
     */
    std::map<std::string, std::vector <std::pair <double, double > > > timerStat;

    /*!
     * \brief All processed frames 
     */
    std::vector <VideoPlaylistFrame> frames;

    /*!
     * \brief Vector of estimate classes for all frames
     */
    std::vector<ClassType> frameEstimatedClass;

    /*!
     * \brief Vector of actual classes for all frames
     */
    std::vector<ClassType> frameActualClass;

    /*!
     * \brief Set of all known classes
     */
    std::set<ClassType> knownClasses;

    /*!
     * \brief Set of class type and class name pairs
     */
    std::map<ClassType, std::string> classNames;

    /*!
     * \brief Pointer to classifier result data source reader
     */
    SPtr<IClassifierResultDataSource> classMarkup;

    /*!
     * \brief Get times for filter
     * \param[in] filterId Identifier of filter
     * \return Vector of start timer and stop timer pairs
     */
    std::vector <std::pair <double, double > > getTimerStatForFilter(std::string filterId);

    /*!
     * \brief Mutex for support multithreading
     */
    std::mutex m;

private:

};
};

/** @}*/
#endif
