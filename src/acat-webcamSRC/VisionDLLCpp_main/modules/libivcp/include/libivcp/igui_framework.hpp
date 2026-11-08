///////////////////////////////////////////////////////////////////////////
// <copyright file="igui_framework.hpp" company="Intel Corporation">
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


#ifndef __IGUI_FRAMEWORK_HPP__
#define __IGUI_FRAMEWORK_HPP__

#include <string>
#include "libivcp/ipipeline.hpp"

/*! 
 * \defgroup libivcp IVCP Filters Library 
 * \brief IVCP filters library
 * @{
 */

/*! 
 * \file igui_framework.hpp
 * \brief Declaration of GUI framework interface
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief GUI framework interface
* \details Interface for a pipeline visualizer. It allows to display processed playlist frame by frame
* and show detected regions and classification results during playback.
*/
class IVCP_EXPORT IGUIFramework
{
public:

    /*!
     * \brief Region settings
     */
    struct GUIRegionSettings
    {
        /*!
         * \brief Constructor
         */
        GUIRegionSettings(float regionWidthExpansionMultiplier = 1.1f, float regionHeightExpansionMultiplier = 1.2f, int _regionOffsetX = 0, int _regionOffsetY = 0):
            regionOffsetX(_regionOffsetX), regionOffsetY(_regionOffsetY), regionExpansionHScale(regionHeightExpansionMultiplier), regionExpansionWScale(regionWidthExpansionMultiplier)
        {
        }

        /*!
         * \brief Region offset along X axis
         */
        int regionOffsetX; 

        /*!
         * \brief Region offset along Y axis
         */        
        int regionOffsetY;

        /*!
         * \brief Height scale of the region
         */
        float regionExpansionHScale;
        
        /*!
         * \brief Width scale of the region
         */
        float regionExpansionWScale;

        /*!
         * \brief Eqaul comparison operator
         * \param[in] b Structure to compare with
         * \return True is 2 structures are eqaul
         */
        bool operator==(const GUIRegionSettings b) const
        {
            return (regionOffsetX == b.regionOffsetX) && (regionOffsetY == b.regionOffsetY) && 
                (fabs(regionExpansionHScale - b.regionExpansionHScale) < 0.001f) && 
                (fabs(regionExpansionWScale - b.regionExpansionWScale) < 0.001f);
        }
    };

    /*!
    * \brief GUI settings
    */
    struct Settings
    {
        /*!
         * \brief Constructor
         */
        Settings(int _sensitivity = 0, bool _autoExposure=true, int _exposure = 40, float _stabThreshold = 0.0, unsigned int _stabFrames = 10):
            stabThreshold(_stabThreshold), stabFrames(_stabFrames), sensitivity(_sensitivity), exposure(_exposure), autoExposure(_autoExposure) 
        {} //Redundant class prefix is a workaround for cppcheck v1.63.1 error message

         /*!
         * \brief Face movement threshold used to stabilize resulting frames in case there is no motion detected
         * \details This setting is related to  Dlib or RealSense handler and is used only once on first creation of handler
         */
        float stabThreshold;

        /*!
         * \brief Number of frames that are used to check for face motion
         * \details This setting is related to Dlib or RealSense handler and is used only once on first creation of handler
         */
        unsigned int stabFrames;

         /*!
         * \brief Sensitivity level number
         * \details Could take values between 0-10
         */    
        int sensitivity;

         /*!
         * \brief Manual exposure setting in milliseconds (when auto exposure is switched off)
         * \details Could take values between 10-90
         */        
        double exposure;

        /*!
         * \brief This setting enables or disables auto exposure
         */
        bool autoExposure;

        /*!
         * \brief Gestures settings 
         * \details Every gesture could be enabled or disabled
         */  
        std::map <std::string, bool> gestures;

        /*!
         * \brief Region settings 
         * \details Every gesture could be enabled or disabled
         */
        std::map <std::string, GUIRegionSettings> regions;
    };

    /*!
     * \brief Destructor
     */
    virtual ~IGUIFramework() {};

    /*!
     * \brief Set visualization window name
     * \details Frame processing result is displayed in separate window called in accordance with configuration
     * \param[in] name Visualization window name
     */
    virtual void setWindowName(std::string name) = 0;

    /*!
     * \brief Get current visualization window name
     * \details Frame processing result is displayed in separate window called in accordance with configuration
     * \return Visualization window name
     */
    virtual std::string getWindowName() = 0;

    /*!
     * \brief Check whether there are some pipeline processing results stored in memory that could be stored to a videofile
     * \details Checks that there are some frames processed prior to creation of output videofile
     * \return true if there are some unsaved frames
     */
    virtual bool haveVideoOutput() = 0;

    /*!
     * \brief Store pipeline processing result to videofile
     * \details Displayed frame processing result will be stored as a videofile
     * \param[in] filename Output video file name
     * \param[in] fourcc Output video file codec. By default it is 0, that means avi format
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \param[in] type Output video file type
     * \param[in] convertTo8bit Whether need to save video in 8bit format
     * \details For RAWFILE_OUTPUT type fourcc and convertTo8bit parameters are ignored
     * \throw IOException Unable to open the video file
     */
    virtual void generateVideoOutput(std::string filename, int fourcc = 0, cv::Size frameSize = cv::Size(0, 0), int fps = 30, VideoOutputType type = VIDEOFILE_OUTPUT, bool convertTo8bit = false) = 0;

    /*!
     * \brief Enable retaining of pipeline processing result in videofile
     * \details Displayed frame processing result will be stored as a videofile
     * \param[in] filename Output video file name
     * \param[in] fourcc Output video file codec
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \param[in] framesThreshold Frame number limit per output file (if non zero)
     * \param[in] type Output video file type
     * \param[in] convertTo8bit Whether need to save video in 8bit format
     * \details For RAWFILE_OUTPUT type fourcc and convertTo8bit parameters are ignored
     * \throw IOException Unable to open the video file
     */
    virtual void enableVideoOutput(std::string filename, int fourcc, cv::Size frameSize, int fps = 30, int framesThreshold = 0, VideoOutputType type = VIDEOFILE_OUTPUT, bool convertTo8bit = false) = 0;

    /*!
     * \brief Disable retaining of pipeline processing result in videofile
     */
    virtual void disableVideoOutput() = 0;

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
     * \brief Save current and default GUI settings to the file
     * \param[in] filename File to which sittings should be saved
     * \throws IOException Unable to save settigs to given file
     */
    virtual void saveSettings(const std::string& filename) const = 0;

    /*!
     * \brief Load GUI current and default settings from the file
     * \param[in] filename File from which sittings should be loaded
     * \throws IOException Unable to read settigs from given file
     */
    virtual void loadSettings(const std::string& filename) = 0;

    /*!
     * \brief Set GUI default settings
     * \param[in] settings New default settings.
     */
    virtual void setDefaultSettings(IGUIFramework::Settings settings) = 0; //Redundant class prefix for Settings is workaround for correct work of doxygen

    /*!
     * \brief Set GUI settings
     * \param[in] settings New settings
     */
    virtual void setSettings(IGUIFramework::Settings settings) = 0;

    /*!
     * \brief Return pipeline to default settings    
     */
    virtual void replaceSettingsWithDefaults() = 0;

    /*!
     * \brief Get default GUI settings
     * \return Default GUI settings
     */
    virtual Settings getDefaultSettings() const = 0;

    /*!
     * \brief Get current GUI settings
     * \return GUI settings
     */
    virtual Settings getSettings() const = 0;

    /*!
     * \brief Getter for the current sensitivity
     * \return Value of the current sensitivity
     */
    virtual int getCurrentSensitivity() const = 0;

    /*!
     * \brief Getter for the current gesture settings
     * \return Current gesture settings (name of the gesture and the flag showing if gesture is enabled)
     */
    virtual std::map<std::string, bool> getGesturesSettings() const = 0;

    /*!
     * \brief Setter for current sensitivity
     * \param[in] sens Sensitivity to set
     */
    virtual void setSensitivitySettings(int sens) = 0;

    /*!
     * \brief Setter for current gesture settings
     * \param[in] gestures New gesture settings (name of the gesture and the flag showing if gesture is enabled)
     */
    virtual void setGesturesSettings(const std::map<std::string, bool>& gestures) = 0;

    /*!
     * \brief To start or not to start capturing to video output file from the first frame
     */
    virtual void setVideoCapture(bool isToCapture) = 0;

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    /*!
     * \brief Listen socket for commands to open/close GUIFramework window
     * \param[in] port Port to listen
     * \details Send "action=SHOWGUI" command to show GUI
     * \throws IOException Unable to open socket
     */
    virtual void listenSocket(int port = 27015) = 0;
#ifdef ASTER_SUPPORT
	virtual bool sendGesture(const char *buffer) = 0;
#endif
#endif

	virtual std::string getCurOutputFilename() = 0;
	virtual int getOutputFramesCount() = 0;
	virtual void setWorkerThreadStatusToExiting() = 0;

	virtual void sendCommandToGUI(const char *buffer, double value) = 0;
	virtual void showVideo() = 0;
	virtual void hideVideo() = 0;
	virtual bool isVideoVisible() = 0;

};
};

/** @}*/
#endif
