////////////////////////////////////////////////////////////////////////////
// <copyright file="realsense.hpp" company="Intel Corporation">
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

#ifndef __REALSENSE_HPP__
#define __REALSENSE_HPP__

#include <deque>
#include <mutex>
#include <thread>
#include <condition_variable>

#include "libivcp/irealsense.hpp"
#include "libivcp/sptr.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*!
 * \defgroup libinfra IVCP Infrastructure Library
 * \brief IVCP infrastructure library
 * @{
 */

/*!
 * \file realsense.hpp
 * \brief Declaration of RealSense face detector handler
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

#ifdef USE_REALSENSE

namespace ivcp
{
/*!
* \brief RealSense face detector handler implementation
*/
class IVCP_EXPORT RealSense: public IRealSense
{
public:

    virtual IRealSense::Settings getHandlerSettings();
    virtual void setHandlerSettings(IRealSense::Settings settings);

    /*!
     * \brief Constructor
     * \param[in] stabThreshold Movement threshold to start changing regions on face movement
     * \param[in] stabFrames Number of previous frames to check for face movement during face stabilization
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true RealSense improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \throw IncorrectParamException RealSense is not found
     * \throw IOException RealSense internal error
     */
    RealSense(float stabThreshold = 0.0f,
        unsigned int stabFrames = 10,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false);

    virtual ~RealSense();
    virtual cv::Rect getRegion(cv::Mat image, VideoPlaylistFrame currentFrame, RegionType regionToSearch, bool normalize);
    virtual cv::Mat getTransform();

    virtual void resetTracking();
    virtual bool isReseted() { return resetAction; };
    virtual void resetDone() { resetAction = false; };

    virtual bool isFaceFound() { return faceDetected; };
    virtual bool isRegionsReady() { return isGlassesFound; };

    virtual void setFaceTrackedData(cv::Rect faceRect, std::vector<cv::Point> landmarks);

protected:
    /*!
     * \brief Last processed frame number in sequence of frames transmitted to face detector
     */
    int frameNum;

    /*!
     * \brief Vector with landmarks
     */
    std::vector<cv::Point> landmarks;

    /*!
     * \brief Whether face is detected
     */
    bool faceDetected;

    /*!
     * \brief Cached transformation matrix
     * \sa getTransform
     */
    cv::Mat cachedTransformation;

    /*!
     * \brief Enable RealSense improvement using a-priory information about face and the subset of canonical landmarks
     */
    bool useTemplateRegions;

    /*!
     * \brief Last processed frame index in playlist
     */
    VideoPlaylistFrame cachedFrame;

    /*!
     * \brief Process detected face element locations information and generate face bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateFace(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate left eyebrow bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateLEyebrow(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate right eyebrow bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateREyebrow(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate mouth bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateMouth(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate nose bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateNose(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate left eye bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateLEye(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate right eye bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateREye(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate left cheek bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateLCheek(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Process detected face element locations information and generate right cheek bounding rectangle
     * \param[in] face Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \return Bounding rectangle of detected region
     */
    cv::Rect locateRCheek(const cv::Rect& face, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Apply a workaround for eyebrow location
     */
    cv::Rect fixEyebrow(bool isLeft, cv::Rect eyebrowRect, const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Face movement stabilization threshold
     */
    float stabThreshold;

    /*!
     * \brief Face movement stabilization frame number
     */
    unsigned int stabFrames;

    /*!
     * \brief Face markers locations for movement stabilization
     */
    std::deque<StabMarker> stabData;

    /*!
     * \brief Check whether face is changed or moved enough to update cached faceData
     * \return In case the face moved changed enough the function returns true and returns false otherwise
     */
    bool checkFaceDataUpdate(const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Face rectangle, detected by RealSense
     */
    cv::Rect faceRect;

    /*
     * \brief A map from region's type to region's size via cv::Rect (for stabThreshold)
     */
    std::map<RegionType, cv::Rect> regionsLocation;

    /*!
     * \brief Whether is face data updated and we need to recalculate regions
     */
    bool isFaceDataUpdated;

    /*!
     * \brief Enable glasses detection
     */
    bool glassesFixEnabled;

    /*!
     * \brief Similarity transformation matrix for improved detection
     */
    cv::Mat similarityTransformation;

    /*!
     * \brief Map with coordinates of canonical landmarks
     */
    std::map<int, cv::Point2i> canonicalLandmark;

    /*!
     * \brief Map with coordinates of regions in template mask
     */
    std::map<ivcp::RegionType, std::pair<cv::Point2i, cv::Point2i>> templateMask;

    /*!
     * \brief Whether use auto glasses search workaround
     */
    bool useAutoGlassesSearch;

    /*!
     * \brief Function for correction eyebrow regions based on auto glasses search option result
     */
    cv::Rect autoGlassesSearch(cv::Rect eyebrowRect);

    /*!
     * \brief Current eyes centers position
     */
    std::pair<cv::Point, cv::Point> eyeCenters;

    /*!
     * \brief Get current eyes centers position
     */
    std::pair<cv::Point, cv::Point> getEyeCenters();

    /*!
     * \brief Ratio of length between center of left eye and glasses to left eyebrow width
     */
    double leftGlassRatio;

    /*!
     * \brief Ratio of length between center of right eye and glasses to right eyebrow width
     */
    double rightGlassRatio;

    /*!
     * \brief Whether recalculate current glasses position
     */
    bool recalculateGlasses;

    /*!
     * \brief Ratio of length between center of left eye and glasses down rim to left eye width
     */
    double leftRimRatio;

    /*!
     * \brief Ratio of length between center of right eye and glasses down rim to right eye width
     */
    double rightRimRatio;

    /*!
     * \brief Function for correction cheeks regions based on auto glasses down rim search option result
     */
    cv::Rect autoGlassesDownRimSearch(cv::Rect eyebrowRect);

    /*!
     * \brief Pair with coordinates of eyes in glasses template (for auto glasses search - useExhaustiveSearch)
     */
    std::pair<cv::Point2i, cv::Point2i> templateEyePos;

    /*!
     * \brief Map with coordinates of points in glasses template (for auto glasses search - not useExhaustiveSearch)
     */
    std::map<int, cv::Point2d> templateGlasses;

    /*!
     * \brief Get eyebrows width (for auto glasses search)
     */
    std::pair<int, int> getEyebrowRegionsWidth();

    /*!
     * \brief glassesFixStrength value used to tune position of eyebrow
     */
    float glassesFixStrength;

    /*!
     * \brief Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     */
    bool cutBackground;

    /*!
     * \brief Function for correction cheek and eyebrow regions based on auto face border search
     */
    cv::Rect cutBackgroundFromRegions(cv::Rect regionRect, ivcp::RegionType type);

    /*!
     * \brief Calculate lines of face border (for auto face border search)
     */
    void calculateFaceBorder();

    /*!
     * \brief Slope of the line of the left face border for cheek region (for auto face border search)
     */
    double kLeftCheekFaceBorder;

    /*!
     * \brief Shift factor of the line of the left face border for cheek region (for auto face border search)
     */
    double bLeftCheekFaceBorder;

    /*!
     * \brief Slope of the line of the right face border for cheek region (for auto face border search)
     */
    double kRightCheekFaceBorder;

    /*!
     * \brief Shift factor of the line of the right face border for cheek region (for auto face border search)
     */
    double bRightCheekFaceBorder;

    /*!
     * \brief Slope of the line of the left face border for eyebrow region (for auto face border search)
     */
    double kLeftEyebrowFaceBorder;

    /*!
     * \brief Shift factor of the line of the left face border for eyebrow region (for auto face border search)
     */
    double bLeftEyebrowFaceBorder;

    /*!
     * \brief Slope of the line of the right face border for eyebrow region (for auto face border search)
     */
    double kRightEyebrowFaceBorder;

    /*!
     * \brief Shift factor of the line of the right face border for eyebrow region (for auto face border search)
     */
    double bRightEyebrowFaceBorder;

    /*!
     * \brief Mutex for sinchronization access to the settings structure and RealSense data structures
     */
    std::recursive_mutex mutex;

    /*!
     * \brief Compute image transformation matrix suitable for normalization of the detected face
     */
    void getTransform(const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Calculate similarity transformation matrix
     */
    void calculateSimilarityTransformation();

    /*!
     * \brief Construct rectangle of RealSense region
     * \param[in] regionType Type of face region to search
     * \param[in] faceRect Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \param[in] isFaceDataUpdated Whether is face data updated and we need to recalculate regions
     * \param[in] isWAImplemented Whether is function called for glasses or cheeck workaround
     * \return Bounding rectangle of detected region
     */
    cv::Rect getRegionRect(ivcp::RegionType regionType, const cv::Rect& faceRect, const std::vector<cv::Point>& landmarks, bool isFaceDataUpdated, bool isWAImplemented = false);

    /*!
     * \brief Apply RealSense workarounds for eyebrow and cheek regions
     */
    cv::Rect applyWorkarounds(cv::Rect inputRect, ivcp::RegionType regionType);

    /*!
     * \brief Glasses thread status: running, signaled to exit, finalized
     */
    enum ThreadStatus
    {
        THREAD_RUNNING = 0,
        THREAD_EXITING = 1,
        THREAD_FINALIZED = 2
    };

    /*
     * \brief A thread for searching glasses position
     */
    std::thread searcher;

    /*!
     * \brief Whether searcher thread is ended
     */
    ThreadStatus searcherStatus;

    /*!
     * \brief Condition variable for the main searcher thread - whether or not it is time to exit
     */
    std::condition_variable_any cvTimeToExit;

    /*!
     * \brief Whether glasses position is found
     */
    bool isGlassesFound;

    /*!
     * \brief Copy of frame for searcher thread
     */
    cv::Mat searcherImage;

    /*!
     * \brief A thread function which search glasses position
     */
    static void searcherThread(RealSense* pRs);

    /*!
     * \brief Close searcher thread anyway
     */
    void closeSearcherThread();

    /*!
     * \brief Estimate threshold using stabilization markers
     */
    double estimateThreshold(StabMarker newMarkers, StabMarker refMarkers);

    /*!
     * \brief Whether RealSenseVideoSource is created
     */
    bool isRealSenseVideoSourceCreated;

    /*!
     * \brief Whether reset action for RealSence face tracking is need to be done
     */
    bool resetAction;

};
};
#endif

/** @}*/
#endif
