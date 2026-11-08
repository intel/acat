///////////////////////////////////////////////////////////////////////////
// <copyright file="idlib.hpp" company="Intel Corporation">
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


#ifndef __IDLIB_HPP__
#define __IDLIB_HPP__

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
 * \file idlib.hpp
 * \brief Declaration of basic DLIB handler interface
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief Basic DLIB handler interface
*/
class IVCP_EXPORT IDlib
{
public:

    /*!
     * \brief Dlib region creator configuration
     */
    struct Settings
    {
    public:
        /*!
         * \brief Constructor
         */
        Settings(float _stabThreshold = 0.0, unsigned int _stabFrames = 10, int _normFrames = -1, int _noFaceFrames = -1, bool _noFaceClockwiseRotation = false,
            bool _glassesFixEnabled = false,  bool _useTemplateRegions = false, float _glassesFixStrength = 0.0f, bool _useAutoGlassesSearch = false, bool _cutBackground = false,
            double _resizeDownscale = 1.0):
            stabThreshold(_stabThreshold), stabFrames(_stabFrames), normFrames(_normFrames), noFaceFrames(_noFaceFrames), noFaceClockwiseRotation(_noFaceClockwiseRotation),
                glassesFixEnabled(_glassesFixEnabled), useTemplateRegions(_useTemplateRegions), useAutoGlassesSearch(_useAutoGlassesSearch), cutBackground(_cutBackground),
                resizeDownscale(_resizeDownscale)
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
         * \details This setting is related to Dlib handler and is used only once on first creation of Dlib handler
         */
        float stabThreshold;

        /*!
         * \brief Number of frames that are used to check for face motion
         * \details This setting is related to Dlib handler and is used only once on first creation of Dlib handler
         */
        unsigned int stabFrames;

        /*!
         * \brief Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames)
         * \details In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed.
         * Max value is assumed to be 4095, min value is assumed to be 0
         */
        int normFrames;

        /*!
         * \brief Frequency of applying the "no face" Dlib workaround
         * \details In case noFaceWorkaroundFrames is -1 no "no face workaround" is applied - try to rotate the image until face is detected
         */
        int noFaceFrames;

        /*!
         * \brief If true we rotate the image clockwise, otherwise we rotate counter-clockwise
         */
        bool noFaceClockwiseRotation;

        /*!
         * \brief Enable glasses detection
         */
        bool glassesFixEnabled;

        /*!
         * \brief Enable Dlib improvement using a-priory information about face and the subset of canonical landmarks
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

        /*!
         * \brief Downscale for every frame image to decrease face detection time
         */
        double resizeDownscale;
    };

    /*!
     * \brief Get actual Dlib region detector configuration
     * \return Dlib region creator configuration
     */
    virtual IDlib::Settings getHandlerSettings() = 0;

    /*!
     * \brief Set Dlib region detector configuration
     * \param[in] settings Dlib region creator configuration
     */
    virtual void setHandlerSettings(IDlib::Settings settings) = 0;

    /*!
     * \brief Destructor
     */
    virtual ~IDlib() {};

    /*!
     * \brief Get rectangle of giventype on given image using Dlib face detector
     * \param[in] image Video frame where we search for a region
     * \param[in] currentFrame Current frame index in playlist
     * \param[in] regionToSearch Type of face region to search
     * \param[in] normalize Perform face normalization
     * \return Rectangle with given region on the image
     * \throw IncorrectParamException Dlib is not found
     * \throw IOException Dlib internal error
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
     * \brief Get frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames)
     * \details In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed
     * \return Frequency of searching for min and max 16 bit values
     */
    virtual int getNormFrames() = 0;

    /*!
     * \brief Set frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames)
     * \param[in] normFrames Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames).
     * In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed
     */
    virtual void setNormFrames(int normFrames = -1) = 0;

    /*!
     * \brief Get frequency of applying the "no face" Dlib workaround
     * \details In case noFaceFrames is -1 no "no face workaround" is applied. Otherwise pipeline processing is stopped
     * each noFaceFrames frames and we try to rotate the image until face is detected
     * \return Frequency of applying the "no face" Dlib workaround
     */
    virtual int getNoFaceFrames() = 0;

    /*!
     * \brief Set frequency of applying the "no face" Dlib workaround
     * \param[in] numFrames Frequency of applying the "no face" Dlib workaround
     * \details In case noFaceFrames is -1 no "no face workaround" is applied. Otherwise pipeline processing is stopped
     * each noFaceFrames frames and we try to rotate the image until face is detected
     */
    virtual void setNoFaceFrames(int numFrames = -1) = 0;

    /*!
     * \brief Reset Dlib tracking
     */
    virtual void resetTracking() = 0;

    /*!
     * \brief Rotation for face workaround
     * \param[in] image An image for rotation
     */
    virtual void rotateImageWA(cv::Mat image) = 0;

    /*!
     * \brief Whether face is searching
     */
    virtual bool isFaceSearching() = 0;

    /*!
     * \brief Whether regions can be shown in GUI (all searchings are ended)
     */
    virtual bool isRegionsReady() = 0;

	//SS 05/14/2023
	virtual bool getIsFaceDataUpdated() = 0;

protected:

private:

};
};

/** @}*/
#endif
