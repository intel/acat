///////////////////////////////////////////////////////////////////////////
// <copyright file="iregion_creator.hpp" company="Intel Corporation">
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


#ifndef __IREGION_CREATOR_HPP__
#define __IREGION_CREATOR_HPP__

#include "libivcp/filter.hpp"
#include "libivcp/sptr.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/video_source/video_playlist.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file iregion_creator.hpp
 * \brief Interface of region creating objects
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 * \details Declaration of the region creating interface
 */

namespace ivcp
{
/*! 
 * \brief Basic interface for all region creators
 * \sa RegionCreator
 * \sa BBoxRegionCreator
 * \sa GTRegionCreator
 */
class IVCP_EXPORT IRegionCreator: public Filter
{
public:

    /*! 
     * \brief Description of given region location on the image
     * \details Given region location is described as rectangle on transformed image
     * yielded during normalization of given object image representation
     * \sa getRegion
     */
    struct RegionDescription
    {
    public:
        /*! 
         * \brief Rect describing given region location on the transformed image
         */
        cv::Rect regionFrame;

        /*! 
         * \brief 3x3 Homographyry transformation matrix used for image transformation during normalization of object representation
         */
        cv::Mat regionRotation;

        /*!
         * \brief Default constructor
         */
           RegionDescription(cv::Rect _regionFrame = cv::Rect(), cv::Mat _regionRotation = cv::Mat()):
            regionFrame(_regionFrame), regionRotation(_regionRotation)
            {
                if(_regionRotation.type() != CV_64FC1 && _regionRotation.rows != 0 && _regionRotation.cols != 0)
                IVCP_EXCEPTION(IncorrectParamException, "Frame rotation matrix should be of type CV_64FC1");
            };
        /*!
         * \brief Creates correct copy of given RegionDescription
         * \return Copy of given RegionDescription
         */
        RegionDescription clone()
        {
            RegionDescription copy;
            copy.regionFrame = regionFrame;
            regionRotation.copyTo(copy.regionRotation);
            return copy;
        };
	};

    /*!
     * \brief It is used for backward compatability
     */
    typedef RegionDescription regionDescription;

    /*!
     * \brief Default destructor
     */
    virtual ~IRegionCreator() {};

    /*!
     * \brief Get rectangle of region on given image
     * \param[in] image Video frame where we search for a region
     * \return Description of given region location on the image
     * \throw IncorrectParamException Unable to create rectangle of region on given image
     */
    virtual RegionDescription getRegion(cv::Mat image) = 0;

    /*!
     * \brief Get rectangle of region on given image and VideoPlaylistFrame
     * \param[in] image Video frame where we search for a region
     * \param[in] frame Video frame description
     * \return Description of given region location on the image
     * \throw IncorrectParamException Unable to create rectangle of region on given image
     */
    virtual RegionDescription getRegion(cv::Mat image, VideoPlaylistFrame frame) = 0;

    /*!
     * \brief Set active playlist
     * \param[in] videoPlaylist Video playlist used to check for currently processed frame
     * \details Currently processed frame could be used by some filters to distinguish between frames and provide data relative for currently active
     * in case the filter use some external information to process it
     */
    virtual void setPlaylist(SPtr<const VideoPlaylist> videoPlaylist) = 0;

    /*!
     * \brief Get active playlist
     * \return Video playlist used to check for currently processed frame
     * \details Currently processed frame could be used by some filters to distinguish between frames and provide data relative for currently active
     * in case the filter use some external information to process it
     */
    virtual SPtr<const VideoPlaylist> getPlaylist() = 0;

    /*!
     * \brief Get type of region creator
     * \return Region creator type
     */
    virtual RegionCreatorType getType() = 0;

protected:

private:

};
};

/** @}*/
#endif 
