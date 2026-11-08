////////////////////////////////////////////////////////////////////////////
// <copyright file="dlib.hpp" company="Intel Corporation">
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
#ifndef __DLIB_HPP__
#define __DLIB_HPP__

#include <deque>
#include <mutex>
#include <thread>
#include <condition_variable>

#include "libivcp/idlib.hpp"
#include "libivcp/sptr.hpp"
#include "libivcp/exceptions/io_exception.hpp"

/*!
 * \defgroup libinfra IVCP Infrastructure Library
 * \brief IVCP infrastructure library
 * @{
 */

/*!
 * \file dlib.hpp
 * \brief Declaration of Dlib face detector handler
 * \copyright (C) 2015, Intel Corporation, all rights reserved
 */

#ifdef USE_DLIB

#ifndef MSVC
#pragma warning(push, 0)
#include <dlib/image_processing/frontal_face_detector.h>
#include <dlib/image_processing/render_face_detections.h>
#include <dlib/image_processing/shape_predictor.h>
#include "libinfra/dlib_cv_image.hpp"
#pragma warning(pop)
#pragma warning(disable: 4505)
#endif

namespace ivcp
{
/*!
* \brief Dlib face detector handler implementation
*/
class IVCP_EXPORT Dlib: public IDlib
{
public:

    virtual IDlib::Settings getHandlerSettings();
    virtual void setHandlerSettings(IDlib::Settings settings);

    /*!
     * \brief Constructor
     * \param[in] stabThreshold Movement threshold to start changing regions on face movement
     * \param[in] stabFrames Number of previous frames to check for face movement during face stabilization
     * \param[in] normFrames Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames)
     * In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed
     * \param[in] noFaceFrames Frequency of applying the "no face" Dlib workaround
     * \param[in] noFaceClockwiseRotation If true in the "no face" workaround we rotate the image clockwise, otherwise we rotate counter-clockwise
     * \param[in] glassesFixEnabled If true the "glasses" workaround is enabled
     * \param[in] useTemplateRegions If true Dlib improvement will be performed
     * \param[in] glassesFixStrength glassesFixStrength value used to tune position of eyebrow
     * \param[in] useAutoGlassesSearch Whether use auto glasses search actions
     * \param[in] cutBackground Whether remove background for cheek and eyebrow regions if regions doesn't contain only face using face bounding landmarks
     * \param[in] resizeDownscale Downscale for every frame image to decrease face detection time
     * \throw IncorrectParamException Dlib is not found
     * \throw IOException Dlib internal error
     */
    Dlib(float stabThreshold = 0.0f,
        unsigned int stabFrames = 10,
        int normFrames = -1,
        int noFaceFrames = -1,
        bool noFaceClockwiseRotation = false,
        bool glassesFixEnabled = false,
        bool useTemplateRegions = false,
        float glassesFixStrength = 0.0f,
        bool useAutoGlassesSearch = false,
        bool cutBackground = false,
        double resizeDownscale = 1.0);

    virtual ~Dlib();
    virtual cv::Rect getRegion(cv::Mat image, VideoPlaylistFrame currentFrame, RegionType regionToSearch, bool normalize);
    virtual cv::Mat getTransform();

    virtual int getNormFrames();
    virtual void setNormFrames(int normFrames = -1);
    virtual int getNoFaceFrames();
    virtual void setNoFaceFrames(int numFrames = -1);

    virtual void resetTracking();
    virtual void rotateImageWA(cv::Mat image);
    virtual bool isFaceFound() { return faceDetected; };
    virtual bool isFaceSearching() { return searchFace; };
    virtual bool isRegionsReady() { return isGlassesFound; };
	virtual bool getIsFaceDataUpdated() { return isFaceDataUpdated; };

protected:

    /*!
     * \brief Normalize brightness of given 16 bit image
     * \details The function search for min/max values of src16bit image (this is done for each normFrames'th frame)
     * The min and the max value are saved and are used to convert the image to 8 bit (ie 0...255)
     * \param[in] src16bit Source 16 bit image which shall be scaled to the 8 bit destanation image
     * \param[out] dst8bit Destination  8 bit image
     */
    void normalizeBrightness(const cv::Mat &src16bit, cv::Mat &dst8bit);

    /*!
     * \brief Vector with landmarks
     */
    std::vector<cv::Point> landmarks;

    /*!
     * \brief Whether face is detected
     */
    bool faceDetected;

    /*!
     * \brief 16 bit frames counter (used in brightness normalization)
     */
    unsigned long long frameCounter16Bit;

    /*!
     * \brief Min value found in the previous 16bit image (used in brightness normalization)
     */
    unsigned int min16BitValue;

    /*!
     * \brief Max value found in the previous 16bit image (used in brightness normalization)
     */
    unsigned int max16bitValue;

    /*!
     * \brief Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames)
     * \details In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed.
     * Max value is assumed to be 4095, min value is assumed to be 0
     */
    int normFrames;

    /*!
     * \brief Cached transformation matrix
     * \sa getTransform
     */
    cv::Mat cachedTransformation;

    /*!
     * \brief Enable Dlib improvement using a-priory information about face and the subset of canonical landmarks
     */
    bool useTemplateRegions;

    /*!
     * \brief Rotate source image for given angle
     */
    cv::Mat rotateImageDlib(const cv::Mat& source, double angle);

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
     * \return In case the face moved changed enough the function returns true and returns  false otherwise
     */
    bool checkFaceDataUpdate(const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Rotation angle
     */
    double rotationAngle;

    /*!
     * \brief Face rectangle, detected by Dlib
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
    std::map<ivcp::RegionType, std::pair<cv::Point2i, cv::Point2i> > templateMask;

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
     * \brief Current temp rotation angle (for searching face)
     */
    double tryAngle;

    /*!
     * \brief Counter for angles where face is detected (for searching face)
     */
    int countAnglesWithDetection;

    /*!
     * \brief Whether rotation angle found (for searching face)
     */
    bool isRotationAngleFound;

    /*!
     * \brief Dlib frontal face detector (for searching face)
     */
    dlib::frontal_face_detector ffd;

    /*!
     * \brief Dlib shape predictor (for landmarks)
     */
    dlib::shape_predictor pose_model;

    /*!
     * \brief Last processed frame number in sequence of frames transmitted to face detector
     */
    int frameNum;

    /*!
     * \brief Previous rotation angle (for image rotation)
     */
    double prevRotationAngle;

    /*!
     * \brief Current 2x3 affine transformation matrix for the planar rotation (for image rotation)
     */
    cv::Mat currentRotationMat;

    /*!
     * \brief glassesFixStrength value used to tune position of eyebrow
     */
    float glassesFixStrength;

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
     * \brief Current number of frames when face is not detected
     */
    int noFaceNumFrames;

    /*!
     * \brief Change angle value (for searching face)
     */
    void changeAngleWA();

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
     * \brief Mutex for sinchronization access to the settings structure and Dlib data structures
     */
    std::recursive_mutex mutex;

    /*!
     * \brief Whether face workaround is working
     */
    bool searchFace;

    /*!
     * \brief Compute image transformation matrix suitable for normalization of the detected face
     */
    void getTransform(const std::vector<cv::Point>& landmarks);

    /*!
     * \brief Calculate landmarks position on the biggest detected face
     */
    void getLandmarks(dlib::cv_image<dlib::bgr_pixel> img, std::vector<dlib::rectangle> faces);

    /*!
     * \brief Rerotation transformation matrix (needed to rerotate landmarks if face is detected only using rotated image)
     */
    cv::Mat rerotationMat;

    /*!
     * \brief Calculate similarity transformation matrix
     */
    void calculateSimilarityTransformation();

    /*!
     * \brief Construct rectangle of Dlib region
     * \param[in] regionType Type of face region to search
     * \param[in] faceRect Face rectangle
     * \param[in] landmarks Vector with landmarks
     * \param[in] isFaceDataUpdated Whether is face data updated and we need to recalculate regions
     * \param[in] isWAImplemented Whether is function called for glasses or cheeck workaround
     * \return Bounding rectangle of detected region
     */
    cv::Rect getRegionRect(ivcp::RegionType regionType, const cv::Rect& faceRect, const std::vector<cv::Point>& landmarks, bool isFaceDataUpdated, bool isWAImplemented = false);

    /*!
     * \brief Apply Dlib workarounds for eyebrow and cheek regions
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
    static void searcherThread(Dlib* pDlib);

    /*!
     * \brief Close searcher thread anyway
     */
    void closeSearcherThread();

    /*!
     * \brief Estimate threshold using stabilization markers
     */
    double estimateThreshold(StabMarker newMarkers, StabMarker refMarkers);

    /*!
     * \brief Prepare image for face searching actions: normalize brightness, convert colors, resize
     */
    cv::Mat prepareImageForFaceSearch(cv::Mat srcImage);

    /*!
     * \brief Downscale for every frame image to decrease face detection time
     */
    double resizeDownscale;

	//SS 05/14/2023
	StabMarker stableRegionMarker;

};
};
#endif

/** @}*/
#endif
