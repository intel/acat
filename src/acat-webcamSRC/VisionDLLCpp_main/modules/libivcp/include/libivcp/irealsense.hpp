///////////////////////////////////////////////////////////////////////////
// <copyright file="irealsense.hpp" company="Intel Corporation">
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


#ifndef __IREALSENSE_HPP__
#define __IREALSENSE_HPP__

#include "libivcp/types.hpp"
#include "libivcp/exceptions/incorrect_param_exception.hpp"
#include "libivcp/exceptions/io_exception.hpp"
#include "libivcp/video_source/video_playlist_frame.hpp"
/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

/*!
 * \file irealsense.hpp
 * \brief Declaration of basic RealSense handler interface
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Basic RealSense handler interface
*/
class IVCP_EXPORT IRealSense
{
public:

    /*!
     * \brief RealSense region creator configuration
     */
    struct Settings
    {
    public:
        /*!
         * \brief Constructor
         */
        Settings(float _stabThreshold = 0.0, unsigned int _stabFrames = 10, bool _glassesFixEnabled = false,  bool _useTemplateRegions = false, float _glassesFixStrength = 0.0f,
            bool _useAutoGlassesSearch = false, bool _cutBackground = false):
            stabThreshold(_stabThreshold), stabFrames(_stabFrames), glassesFixEnabled(_glassesFixEnabled), useTemplateRegions(_useTemplateRegions), 
            useAutoGlassesSearch(_useAutoGlassesSearch), cutBackground(_cutBackground)
        {
            glassesFixStrength = _glassesFixStrength;
            if (glassesFixStrength < 0.f)
            {
                glassesFixStrength = 0.f;
            }
            else if (glassesFixStrength > 1.f)
            {
                glassesFixStrength = 1.f;
            }
        }

        /*!
         * \brief Face movement threshold used to stabilize resulting frames in case there is no motion detected
         * \details This setting is related to RealSense handler and is used only once on first creation of RealSense handler
         */
        float stabThreshold;

        /*!
         * \brief Number of frames that are used to check for face motion
         * \details This setting is related to RealSense handler and is used only once on first creation of RealSense handler
         */
        unsigned int stabFrames;

        /*!
         * \brief Enable glasses detection
         */
        bool glassesFixEnabled;

        /*!
         * \brief Enable RealSense improvement using a-priory information about face and the subset of canonical landmarks
         */
        bool useTemplateRegions;

        /*!
        * \brief glassesFixStrength value used to tune position of eyebrow
        */
        float glassesFixStrength;

        /*!
         * \brief Whether use auto glasses search workaround
         */
        bool useAutoGlassesSearch;

        /*!
         * \brief Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
         */
        bool cutBackground;
    };

    /*!
     * \brief Get actual RealSense region detector configuration
     * \return RealSense region creator configuration
     */
    virtual IRealSense::Settings getHandlerSettings() = 0;

    /*!
     * \brief Set RealSense region detector configuration
     * \param[in] settings RealSense region creator configuration
     */
    virtual void setHandlerSettings(IRealSense::Settings settings) = 0;

    /*!
     * \brief Destructor
     */
    virtual ~IRealSense() {};

    /*!
     * \brief Get rectangle of giventype on given image using RealSense face detector
     * \param[in] image Video frame where we search for a region
     * \param[in] currentFrame Current frame index in playlist
     * \param[in] regionToSearch Type of face region to search
     * \param[in] normalize Perform face normalization
     * \return Rectangle with given region on the image
     * \throw IncorrectParamException RealSense is not found
     * \throw IOException RealSense internal error
     */
    virtual cv::Rect getRegion(cv::Mat image, VideoPlaylistFrame currentFrame, RegionType regionToSearch, bool normalize = false) = 0;

    /*!
     * \brief Get transformation matrix used to normalize last detected face image
     * \return Used transformation matrix
     */
    virtual cv::Mat getTransform() = 0;

    /*!
     * \brief Whether face is found
     */
    virtual bool isFaceFound() = 0;

    /*!
     * \brief Whether regions can be shown in GUI (all searchings are ended)
     */
    virtual bool isRegionsReady() = 0;

    /*!
     * \brief Set faceRect and landmarks paramaters from RealSense tracking module
     */
    virtual void setFaceTrackedData(cv::Rect faceRect, std::vector<cv::Point> landmarks) = 0;

    /*!
     * \brief Reset RealSense tracking
     */
    virtual void resetTracking() = 0;

    /*!
     * \brief Whether reset action for RealSence face tracking is need to be done
     */
    virtual bool isReseted() = 0;

    /*!
     * \brief Reset action for RealSence face tracking is done
     */
    virtual void resetDone() = 0;

protected:

private:

};
};

/** @}*/
#endif
