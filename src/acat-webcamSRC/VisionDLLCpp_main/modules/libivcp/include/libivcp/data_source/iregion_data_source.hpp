///////////////////////////////////////////////////////////////////////////
// <copyright file="iregion_data_source.hpp" company="Intel Corporation">
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


#ifndef __IDATA_SOURCE_REGION_HPP__
#define __IDATA_SOURCE_REGION_HPP__

#include <list>
#include "libivcp/filter.hpp"
#include "libivcp/video_source/video_playlist_frame.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file iregion_data_source.hpp
 * \brief Declaration of interface to read and save ground truth regions
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
 * \brief Basic interface for region data sources
 */
class IVCP_EXPORT IRegionDataSource: public Filter
{
public:
    /*!
    * \brief Get region
    * \param[in] frame Frame that contains region
    * \param[in] regionClass Class of region
    * \return Bounding rectangle of region
    */
    virtual cv::Rect getRegion(VideoPlaylistFrame frame, ClassType regionClass) = 0;

    /*!
    * \brief Get all regions in frame
    * \param[in] frame Frame that contains the regions
    * \return List of all regions in frame
    */
    virtual std::list <std::pair <ClassType, cv::Rect> > getRegionsByFrame(VideoPlaylistFrame frame) = 0;

    /*!
    * \brief Get transformation of frame related to extraction of ground truth regions
    * \param[in] frame Frame that contains regions
    * \return 3x3 homography transformation matrix of 64 bit doubles for the frame
    */
    virtual cv::Mat getTransformation(VideoPlaylistFrame frame) = 0;

    /*!
    * \brief Set transformation of frame related to extraction of ground truth regions
    * \details There should be at least one region related to the frame to store transformation information.
    * If there is no region defined on the frame nothing would be done.
    * \param[in] frame Frame that contains regions
    * \param[in] transformation 3x3 homography transformation matrix of 64 bit doubles for the frame
    */
    virtual void setTransformation(VideoPlaylistFrame frame, cv::Mat transformation) = 0;

    /*!
    * \brief Add new region in region data source
    * \param[in] frame Frame that contains region
    * \param[in] rect Bounding rectangle of region
    * \param[in] regionClass Class of region
    * \param[in] transformation Matrix of transformation that should be applied to image prior to region extraction
    */
    virtual void addRegion(VideoPlaylistFrame frame, cv::Rect rect, ClassType regionClass, cv::Mat transformation = cv::Mat()) = 0;

    /*!
    * \brief Delete region from region data source
    * \param[in] frame Frame that contains region
    * \param[in] regionClass Class of region
    */
    virtual void delRegion(VideoPlaylistFrame frame, ClassType regionClass) = 0;

    /*!
     * \brief Get type of groundtruth regions
     * \return Groundtruth regions type
     */
    virtual GTRegionsType getType() = 0;

    /*!
     * \brief Return type of a filter
     * \return GROUNDTRUTH_REGIONS_FILTER
     */
    virtual FilterType getRootType() const;

protected:

private:

};
};

/** @}*/
#endif
