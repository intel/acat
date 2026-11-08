///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_region_creator.hpp" company="Intel Corporation">
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



#ifndef __REALSENSE_REGION_CREATOR_HPP__
#define __REALSENSE_REGION_CREATOR_HPP__

#include <mutex>
#include "libivcp/region_creator/region_creator.hpp"
#include "libivcp/sptr.hpp"
#include "libivcp/irealsense.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*!
 * \defgroup libivcp IVCP Filters Library
 * \brief IVCP filters library
 * @{
 */

namespace ivcp
{
/*!
 * \brief RealSense region creator implementation
 * \details RealSense region creator search could search frame for face and/or separate face regions(eyes, eyebrows, mouth, nose) using RealSense face detector.
 */
class IVCP_EXPORT RealSenseRegionCreator: public RegionCreator
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
        Settings(RegionType regionToSearch = REGION_FACE, float regionWidthExpansionMultiplier = 1.1f, float regionHeightExpansionMultiplier = 1.2f, bool _normalize = false,
                float _stabThreshold = 0.0, unsigned int _stabFrames = 10, int _regionOffsetX = 0, int _regionOffsetY = 0, bool _glassesFixEnabled = false,
                bool _useTemplateRegions = false, float _glassesFixStrength = 0.0f, bool _useAutoGlassesSearch = false, bool _cutBackground = false):
            typeToSearch(regionToSearch), regionExpansionWScale(regionWidthExpansionMultiplier), regionExpansionHScale(regionHeightExpansionMultiplier), normalize(_normalize),
            stabThreshold(_stabThreshold), stabFrames(_stabFrames), regionOffsetX(_regionOffsetX), regionOffsetY(_regionOffsetY), glassesFixEnabled(_glassesFixEnabled),
            useTemplateRegions(_useTemplateRegions), useAutoGlassesSearch(_useAutoGlassesSearch), cutBackground(_cutBackground)
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
         * \details This setting is related to RealSense handler and is used only once on first creation of RealSense handler
         */
        float stabThreshold;

        /*!
         * \brief Number of frames that are used to check for face motion
         * \details This setting is related to RealSense handler and is used only once on first creation of RealSense handler
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
     * \brief RealSense region creator constructor
     * \details Initial region detector configuration and RealSense handler to use could be provided
     * \param[in] rsHandler RealSense detector library handler
     * \param[in] settings RealSense region creator configuration
     */
    RealSenseRegionCreator(SPtr<IRealSense> rsHandler = NULL, RealSenseRegionCreator::Settings settings = RealSenseRegionCreator::Settings());

    /*!
     * \brief Set RealSense library handler to use
     * \details RealSense handler is responsible for interconnection with RealSense library and application of RealSense detector to a particular frame sequence
     * \param[in] rsHandler RealSense detector library handler
     */
    void setRealSenseHandler(SPtr<IRealSense> rsHandler);

    /*!
     * \brief Get used RealSense library handler
     * \details RealSense handler is responsible for interconnection with RealSense library and application of RealSense detector to a particular frame sequence
     * \return Pointer to used RealSense detector library handler
     */
    SPtr<IRealSense> getRealSenseHandler();

    /*!
     * \brief Set RealSense region detector configuration
     * \details This method is thread-safe
     * \param[in] settings RealSense region creator configuration
     */
    void setSearchRegionSettings(RealSenseRegionCreator::Settings settings = RealSenseRegionCreator::Settings());

    /*!
     * \brief Get actual RealSense region detector configuration
     * \details This method is thread-safe
     * \return RealSense region creator configuration
     */
    RealSenseRegionCreator::Settings getSearchRegionSettings();

    virtual ~RealSenseRegionCreator();

    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image);
    virtual IRegionCreator::RegionDescription getRegion(cv::Mat image, VideoPlaylistFrame frame);
    virtual RegionCreatorType getType();
protected:
    /*!
     * \brief Used RealSense library handler
     * \details RealSense handler is responsible for interconnection with RealSense library and application of RealSense detector to a particular frame sequence
     */
    SPtr<IRealSense> usedRsHandler;

    /*!
     * \brief RealSense region creator settings
     */
    Settings settings;

    /*!
     * \brief Process new region using RealSense
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
