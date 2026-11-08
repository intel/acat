///////////////////////////////////////////////////////////////////////////
// <copyright file="gt_region_creator.hpp" company="Intel Corporation">
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


#ifndef __REGION_CREATOR_GT_HPP__
#define __REGION_CREATOR_GT_HPP__

#include "libivcp/region_creator/region_creator.hpp"
#include "libivcp/video_source/video_playlist.hpp"
#include "libivcp/data_source/iregion_data_source.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file gt_region_creator.hpp
 * \brief Ground truth region creator
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
 /*! 
 * \brief Ground truth region creator
 * \details This region creator gets rectangles of regions used video playlist 
 * and region data source contained ground truth regions for given playlis
 * \sa BBoxRegionCreator
 */

class IVCP_EXPORT GTRegionCreator: public RegionCreator
{
public:
    /*!
     * \brief Default constructor
     */
    GTRegionCreator();

    /*!
     * \brief Constructor of ground truth region creator
     * \param[in] source Ground truth regions for given playlist
     * \param[in] regClass Class of regions for this region creator
     */
    GTRegionCreator(SPtr<IRegionDataSource> source, ClassType regClass);

    /*!
     * \brief Set necessary params
     * \param[in] source Ground truth regions for given playlist
     * \param[in] regClass Class of created region
     */
    virtual void setParams(SPtr<IRegionDataSource> source, ClassType regClass);

    /*!
     * \brief Default ground truth region destructor
     */
    virtual ~GTRegionCreator() {};

    /*!
     * \brief Get bounding rectangle of region on current video frame in playlist
     * \param[in] image Video frame where we search for a region
     * \return Rectangle with given region on the frame
     */
    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image);

    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image, VideoPlaylistFrame frame);

    /*!
     * \brief Get type of region creator
     * \return GROUNDTRUTH
     */
    virtual RegionCreatorType getType();

    /*!
     * \brief Get data source contained ground truth regions for active playlist
     * \return Region data source
     */
    virtual SPtr<IRegionDataSource> getRegionDataSource();

    /*!
     * \brief Get class of created region
     * \return Class of created region
     */
    virtual ClassType getRegionClass();

protected:
    /*!
     * \brief Class of created region
     */
    ClassType regionClass;

    /*!
     * \brief Data source contained ground truth regions
     */
    SPtr<IRegionDataSource> regions;

private:

};
}

/** @}*/
#endif