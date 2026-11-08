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

#ifndef __DLIB_REGION_CREATOR_HPP__
#define __DLIB_REGION_CREATOR_HPP__

#include <mutex>
#include "libivcp/region_creator/region_creator.hpp"
#include "libivcp/sptr.hpp"
#include "libivcp/idlib.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

namespace ivcp
{
/*!
 * \brief Dlib region creator implementation
 * \details Dlib region creator search could search frame for face and/or separate face regions(eyes, eyebrows, mouth, nose) using Dlib face detector.
 */
class IVCP_EXPORT DlibRegionCreator: public RegionCreator
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
        Settings(RegionType regionToSearch = REGION_FACE, float regionWidthExpansionMultiplier = 1.1f, float regionHeightExpansionMultiplier = 1.2f, bool _normalize = false,
                float _stabThreshold = 0.0, unsigned int _stabFrames = 10, int _regionOffsetX = 0, int _regionOffsetY = 0, int _normFrames = -1,
                int _noFaceFrames = -1, bool _noFaceClockwiseRotation = false, bool _glassesFixEnabled = false, bool _useTemplateRegions = false,
                float _glassesFixStrength = 0.0f, bool _useAutoGlassesSearch = false, bool _cutBackground = false, double _resizeDownscale = 1.0):
            typeToSearch(regionToSearch), regionExpansionWScale(regionWidthExpansionMultiplier), regionExpansionHScale(regionHeightExpansionMultiplier), normalize(_normalize),
            stabThreshold(_stabThreshold), stabFrames(_stabFrames), regionOffsetX(_regionOffsetX), regionOffsetY(_regionOffsetY), normFrames(_normFrames),
            noFaceFrames(_noFaceFrames), noFaceClockwiseRotation(_noFaceClockwiseRotation), glassesFixEnabled(_glassesFixEnabled), useTemplateRegions(_useTemplateRegions), 
            useAutoGlassesSearch(_useAutoGlassesSearch), cutBackground(_cutBackground), resizeDownscale(_resizeDownscale)
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
         * \brief Region type to search
         * \details Could be face, left/right eye, left/right eyebrow, mouth and nose.
         */
        RegionType typeToSearch;

        /*!
         * \brief Region width scaling factor
         * \details Detected region could be scaled in order to include more or less background to the region.
         */
        float regionExpansionWScale;

        /*!
         * \brief Region height scaling factor
         * \details Detected region could be scaled in order to include more or less background to the region.
         */
        float regionExpansionHScale;

        /*!
         * \brief Enable face normalization
         * \details Face image could be normalized prior to evaluation of region location.
         */
        bool normalize;

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
         * \brief Region offset along X axis
         */
        int regionOffsetX;

        /*!
         * \brief Region offset along Y axis
         */
        int regionOffsetY;

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
     * \brief Dlib region creator constructor
     * \details Initial region detector configuration and Dlib handler to use could be provided
     * \param[in] dlibHandler Dlib detector library handler
     * \param[in] settings Dlib region creator configuration
     */
    DlibRegionCreator(SPtr<IDlib> dlibHandler = NULL, DlibRegionCreator::Settings settings = DlibRegionCreator::Settings());

    /*!
     * \brief Set Dlib library handler to use
     * \details Dlib handler is responsible for interconnection with Dlib library and application of Dlib detector to a particular frame sequence
     * \param[in] dlibHandler Dlib detector library handler
     */
    void setDlibHandler(SPtr<IDlib> dlibHandler);

    /*!
     * \brief Get used Dlib library handler
     * \details Dlib handler is responsible for interconnection with Dlib library and application of Dlib detector to a particular frame sequence
     * \return Pointer to used Dlib detector library handler
     */
    SPtr<IDlib> getDlibHandler();

    /*!
     * \brief Set Dlib region detector configuration
     * \details This method is thread-safe
     * \param[in] settings Dlib region creator configuration
     */
    void setSearchRegionSettings(DlibRegionCreator::Settings settings = DlibRegionCreator::Settings());

    /*!
     * \brief Get actual Dlib region detector configuration
     * \details This method is thread-safe
     * \return Dlib region creator configuration
     */
    DlibRegionCreator::Settings getSearchRegionSettings();

    virtual ~DlibRegionCreator();

    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image);
    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image, VideoPlaylistFrame frame);
    virtual RegionCreatorType getType();
protected:
    /*!
     * \brief Used Dlib library handler
     * \details Dlib handler is responsible for interconnection with Dlib library and application of Dlib detector to a particular frame sequence
     */
    SPtr<IDlib> usedDlibHandler;

    /*!
     * \brief Dlib region creator settings
     */
    Settings settings;

    /*!
     * \brief Process new region using Dlib
     * \param[in] srcImage Video frame where we search for a region
     * \param[in] currentFrame Video frame description
     * \return Description of given region location on the image
     * \throw IncorrectParamException Unable to create rectangle of region on given image
     */
    IRegionCreator::RegionDescription processRegion(cv::Mat srcImage, VideoPlaylistFrame currentFrame);

private:

    /*!
     * \brief Mutex for sinchronization access to the settings structure
     */
    std::recursive_mutex settingsMutex;

};
};

/** @}*/
#endif
