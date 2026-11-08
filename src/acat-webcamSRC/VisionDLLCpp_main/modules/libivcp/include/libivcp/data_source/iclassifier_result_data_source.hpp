///////////////////////////////////////////////////////////////////////////
// <copyright file="iclassifier_result_data_source.hpp" company="Intel Corporation">
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


#ifndef __IDATA_SOURCE_CLASSIFIER_RESULT_HPP__
#define __IDATA_SOURCE_CLASSIFIER_RESULT_HPP__

#include "libivcp/filter.hpp"
#include "libivcp/video_source/video_playlist_frame.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file iclassifier_result_data_source.hpp
 * \brief Declaration of interface to read and save classification results (ie ground truth)
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic interface to read and save classification results (ie ground truth)
 */
class IVCP_EXPORT IClassifierResultDataSource: public Filter
{
public:
    /*!
     * \brief Get ground truth classification results for given frame index in video playlist
     * \param[in] frame Frame index in video playlist
     * \return Ground truth classification result
     */
    virtual ClassType getClassifyResult(VideoPlaylistFrame frame) = 0;

    /*!
     * \brief Verify that ground truth classification results for given frame index in video playlist is available
     * \param[in] frame Frame index in video playlist 
     * \return true if Ground truth classification result is availability
     */
    virtual bool isClassified(VideoPlaylistFrame frame) = 0;

    /*!
     * \brief Set ground truth classification results for given frame index in video playlist
     * \param[in] frame Frame index in video playlist
     * \param[in] mark Ground truth classification result for the frame
     */
    virtual void addClassifyResult(VideoPlaylistFrame frame, ClassType mark) = 0;

    /*!
     * \brief Clear ground truth classification result for given frame index in video playlist
     * \param[in] frame Frame index in video playlist
     */
    virtual void removeClassifyResult(VideoPlaylistFrame frame) = 0;

    /*!
     * \brief Get type of classifier results
     * \return ClassifierResult type
     */
    virtual ClassifierResultType getType() = 0;

    /*!
     * \brief Return type of a filter
     * \return CLASSIFIER_RESULTS_FILTER
     */
    virtual FilterType getRootType() const;

protected:

private:

};
};

/** @}*/
#endif
