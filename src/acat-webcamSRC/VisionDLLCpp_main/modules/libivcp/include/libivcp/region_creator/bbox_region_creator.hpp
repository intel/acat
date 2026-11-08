///////////////////////////////////////////////////////////////////////////
// <copyright file="bbox_region_creator.hpp" company="Intel Corporation">
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

#ifndef __BBOX_REGION_CREATOR_HPP__
#define __BBOX_REGION_CREATOR_HPP__

#include "libivcp/region_creator/region_creator.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

namespace ivcp
{
/*!
 * \brief BlackBox region creator implementation
 * \details BBox region creator always provide the same region preconfigured bounding rectangle regardless of the provided frame content.
 * In case preconfigured region is out of the provided frame the resulting bounding rectangle is evaluated as intersection between
 * preconfigured bounding rectangle and frame boundary rectangle.
 */
class IVCP_EXPORT BBoxRegionCreator: public RegionCreator
{
public:
    /*!
     * \brief Region creator constructor
     * \details Region boundary rectangle parameters should be provided to initialise region creator.
     * \param[in] bbox Region boundary rectangle parameters
     */
    BBoxRegionCreator(cv::Rect bbox);
    /*!
     * \brief Region creator constructor
     * \details Region boundary rectangle parameters should be provided to initialise region creator.
     * \param[in] x New region boundary rectangle top left corner x coordinate
     * \param[in] y New region boundary rectangle top left corner y coordinate
     * \param[in] width New region boundary rectangle width
     * \param[in] height New region boundary rectangle height
     */
    BBoxRegionCreator(int x, int y, int width, int height);

    virtual ~BBoxRegionCreator() {};

    /*!
     * \brief Update region boundary rectangle returned by the region creator
     * \details BBox region creator always provide the same region regardless of the frame content. Preconfigured region would be shrinked to fit into frame.
     * \param[in] bbox New region boundary rectangle parameters
     */
    void setBBox(cv::Rect bbox);
    /*!
     * \brief Update region boundary rectangle returned by the region creator
     * \details BBox region creator always provide the same region regardless of the frame content. Preconfigured region would be shrinked to fit into frame.
     * \param[in] x New region boundary rectangle top left corner x coordinate
     * \param[in] y New region boundary rectangle top left corner y coordinate
     * \param[in] width New region boundary rectangle width
     * \param[in] height New region boundary rectangle height
     */
    void setBBox(int x, int y, int width, int height);

    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image);
    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image, VideoPlaylistFrame frame);

    virtual RegionCreatorType getType();

    /*!
     * \brief Get currently used region boundary rectangle configuration
     * \details BBox region creator always provide the same region regardless of the frame content. Preconfigured region would be shrinked to fit into frame.
     * \return Currently used region boundary rectangle parameters
     */
    cv::Rect getBBox();

protected:
    /*!
     * \brief Region boundary rectangle returned by the region creator
     */
    cv::Rect bBox;

private:

};
};

/** @}*/
#endif
