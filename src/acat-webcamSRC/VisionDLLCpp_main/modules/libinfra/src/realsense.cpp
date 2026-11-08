///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense.cpp" company="Intel Corporation">
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

#include <cstdint>
#include "libinfra/realsense.hpp"
#include "libinfra/optim.hpp"
#include "libinfra/face_template.hpp"
#include "libinfra/region_handler_defines.hpp"

using namespace cv;
using namespace std;

//face landmarks
#define LM_FACE_R 53
#define LM_FACE_R_2 54
#define LM_FACE_R_4 56
#define LM_FACE_L 69
#define LM_FACE_L_2 68
#define LM_FACE_L_4 66
#define LM_FACE_D 61
//left eyebrow landmarks
#define LM_LEYEB_R 5
#define LM_LEYEB_L 9
#define LM_LEYEB_U 7
//right eyebrow landmarks
#define LM_REYEB_R 4
#define LM_REYEB_L 0
#define LM_REYEB_U 2
//left eye landmarks
#define LM_LEYE_U 20
#define LM_LEYE_R 18
#define LM_LEYE_D 24
#define LM_LEYE_L 22
#define LM_LEYE_C 77
//right eye landmarks
#define LM_REYE_U 12
#define LM_REYE_L 10
#define LM_REYE_D 16
#define LM_REYE_R 14
#define LM_REYE_C 76
//mouth landmarks
#define LM_MOUTH_R 33
#define LM_MOUTH_L 39
#define LM_MOUTH_U 36
#define LM_MOUTH_D 42
//nose and nose tip landmarks
#define LM_NOSE_L 32
#define LM_NOSE_R 30
#define LM_NOSE_D 31
#define LM_NOSE_U 27
#define LM_NOSET_R 30
#define LM_NOSET_L 32
#define LM_NOSET_U 29
#define LM_NOSET_D 30

#ifdef USE_REALSENSE

namespace ivcp
{

RealSense::RealSense(float _stabThreshold,
        unsigned int _stabFrames,
        bool _glassesFixEnabled,
        bool _useTemplateRegions,
        float _glassesFixStrength,
        bool _useAutoGlassesSearch,
        bool _cutBackground)
{
    stabThreshold = _stabThreshold;
    stabFrames = _stabFrames;
    glassesFixEnabled = _glassesFixEnabled;
    useTemplateRegions = _useTemplateRegions;
    glassesFixStrength = _glassesFixStrength;
    if (glassesFixStrength < 0.f)
    {
        glassesFixStrength = 0.f;
    }
    else if (glassesFixStrength > 1.f)
    {
        glassesFixStrength = 1.f;
    }
    useAutoGlassesSearch = _useAutoGlassesSearch;
    cutBackground = _cutBackground;

    frameNum = 0;
    faceDetected = false;

    isRealSenseVideoSourceCreated = false;
    resetAction = false;

    cachedFrame = VIDEO_PLAYLIST_FRAME_EOF;

    if (useTemplateRegions)
    {
        templateMask = getTemplateMask(RS_REGION);
    }

    canonicalLandmark = getCanonicalLandmark();

    recalculateGlasses = true;
    eyeCenters = pair<Point2i, Point2i>();
    searcherStatus = THREAD_FINALIZED;
    isGlassesFound = (!useAutoGlassesSearch) ? true : false;
    if (useAutoGlassesSearch)
    {
        templateGlasses = getTemplateGlasses();
        templateEyePos = getTemplateEyePos(RS_REGION);
    }
}

RealSense::~RealSense()
{
    closeSearcherThread();
}

static Point2d transformPoint(double x, double y, Mat transform)
{
    const double epsilon = numeric_limits<double>::epsilon();
    Point2d target;
    double w = x * transform.at<double>(2, 0) + y * transform.at<double>(2, 1) + transform.at<double>(2, 2);

    if( fabs(w) > epsilon )
    {
        w = 1./w;
        target.x = ((x * transform.at<double>(0, 0) + y * transform.at<double>(0, 1) + transform.at<double>(0, 2)) * w);
        target.y = ((x * transform.at<double>(1, 0) + y * transform.at<double>(1, 1) + transform.at<double>(1, 2)) * w);
    }
    else
    {
        target.x = target.y = 0;
    }
    return target;
}

IRealSense::Settings RealSense::getHandlerSettings()
{
    lock_guard<recursive_mutex> lk(mutex);
    return RealSense::Settings(stabThreshold, stabFrames, glassesFixEnabled, useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground);
}

void RealSense::setHandlerSettings(IRealSense::Settings settings)
{
    lock_guard<recursive_mutex> lk(mutex);
    stabThreshold = settings.stabThreshold;
    stabFrames = settings.stabFrames;
    glassesFixEnabled = settings.glassesFixEnabled;
    useTemplateRegions = settings.useTemplateRegions;
    glassesFixStrength = settings.glassesFixStrength;
    useAutoGlassesSearch = settings.useAutoGlassesSearch;
    cutBackground = settings.cutBackground;
}

double RealSense::estimateThreshold(StabMarker newMarkers, StabMarker refMarkers)
{
    refMarkers.noseTop -= newMarkers.noseTop;
    refMarkers.noseLeft -= newMarkers.noseLeft;
    refMarkers.noseRight -= newMarkers.noseRight;
    refMarkers.noseBottom -= newMarkers.noseBottom;
    double TopLen = sqrt(refMarkers.noseTop.x * refMarkers.noseTop.x + refMarkers.noseTop.y * refMarkers.noseTop.y);
    double LeftLen = sqrt(refMarkers.noseLeft.x * refMarkers.noseLeft.x + refMarkers.noseLeft.y * refMarkers.noseLeft.y);
    double RightLen = sqrt(refMarkers.noseRight.x * refMarkers.noseRight.x + refMarkers.noseRight.y * refMarkers.noseRight.y);
    double BottomLen = sqrt(refMarkers.noseBottom.x * refMarkers.noseBottom.x + refMarkers.noseBottom.y * refMarkers.noseBottom.y);
    return TopLen + LeftLen + RightLen + BottomLen;
}

bool RealSense::checkFaceDataUpdate(const vector<Point>& landmarks)
{
    if(stabThreshold == 0)
    {
        return true;
    }

    StabMarker newMarkers;
    int noseTopPointCount = 0;
    int counter = 0;
    for(auto iter = landmarks.begin(); iter != landmarks.end(); iter++, counter++)
    {
        if( noseTopPointCount < 2 && (counter == LM_LEYE_R || counter == LM_REYE_L) )
        {
            if( noseTopPointCount )
            {
                newMarkers.noseTop.x = (newMarkers.noseTop.x + iter->x) / 2;
                newMarkers.noseTop.y = (newMarkers.noseTop.y + iter->y) / 2;
            }
            else
            {
                newMarkers.noseTop.x = iter->x;
                newMarkers.noseTop.y = iter->y;
            }
            noseTopPointCount++;
        }
        if(counter == LM_NOSE_R)
        {
            newMarkers.noseRight.x = iter->x;
            newMarkers.noseRight.y = iter->y;
        }
        if(counter == LM_NOSE_L)
        {
            newMarkers.noseLeft.x = iter->x;
            newMarkers.noseLeft.y = iter->y;
        }
        if(counter == LM_NOSE_D)
        {
            newMarkers.noseBottom.x = iter->x;
            newMarkers.noseBottom.y = iter->y;
        }
    }

    StabMarker avgMarkers;
    avgMarkers.noseTop = Point2d(0, 0);
    avgMarkers.noseLeft = Point2d(0, 0);
    avgMarkers.noseRight = Point2d(0, 0);
    avgMarkers.noseBottom = Point2d(0, 0);
    if(stabData.size() == 0)
    {
        stabData.push_back(newMarkers);
        return true;
    }
    for(auto iter = stabData.begin(); iter != stabData.end(); iter++)
    {
        avgMarkers.noseTop += iter->noseTop;
        avgMarkers.noseLeft += iter->noseLeft;
        avgMarkers.noseRight += iter->noseRight;
        avgMarkers.noseBottom += iter->noseBottom;
    }
    avgMarkers.noseTop.x /= stabData.size();
    avgMarkers.noseLeft.x /= stabData.size();
    avgMarkers.noseRight.x /= stabData.size();
    avgMarkers.noseBottom.x /= stabData.size();
    avgMarkers.noseTop.y /= stabData.size();
    avgMarkers.noseLeft.y /= stabData.size();
    avgMarkers.noseRight.y /= stabData.size();
    avgMarkers.noseBottom.y /= stabData.size();

    stabData.push_back(newMarkers);
    if(stabData.size() > stabFrames)
    {
        stabData.pop_front();
    }

    if(estimateThreshold(newMarkers, avgMarkers) > stabThreshold)
    {
        return true;
    }
    return false;
}

Rect RealSense::getRegion(Mat srcImage, VideoPlaylistFrame currentFrame, RegionType regionToSearch, bool normalize)
{
    lock_guard<recursive_mutex> lkSettings(mutex);
    if(currentFrame != cachedFrame)
    {
        frameNum++;
        cachedFrame = currentFrame;

        if (frameNum == 1)
        {
            if (isRealSenseVideoSourceCreated == false)
            {
                IVCP_EXCEPTION(IncorrectParamException, "RealSense regions can be created only with RealSenseVideoSource.\n");
            }
        }
        if (faceDetected == true)
        {
            isFaceDataUpdated = checkFaceDataUpdate(landmarks);

            if (isFaceDataUpdated)
            {
                if (normalize)
                {
                    getTransform(landmarks);
                }
                else
                {
                    cachedTransformation = Mat::eye(3, 3, CV_64FC1);
                }
            }

            if (useTemplateRegions)
            {
                calculateSimilarityTransformation();
            }

            if (glassesFixStrength != 0.0 && useTemplateRegions)
            {
                int eyebrowOffset = (int)(45.f * glassesFixStrength);
                templateMask[REGION_REYEBROW] = pair<Point2i, Point2i>(Point2i(50, 20 - eyebrowOffset), Point2i(215, 65 - eyebrowOffset));
                templateMask[REGION_LEYEBROW] = pair<Point2i, Point2i>(Point2i(265, 20 - eyebrowOffset), Point2i(428, 65 - eyebrowOffset));
            }

            if (useAutoGlassesSearch)
            {
                if (recalculateGlasses)
                {
                    eyeCenters = getEyeCenters();
                    closeSearcherThread();
                    recalculateGlasses = false;
                    searcherImage = srcImage.clone();
                    searcherStatus = THREAD_RUNNING;
                    searcher = thread(RealSense::searcherThread, this);
                }
                else if (isFaceDataUpdated)
                {
                    eyeCenters = getEyeCenters();
                }
            }

            if (cutBackground && isFaceDataUpdated)
            {
                calculateFaceBorder();
            }
        }
    }
    if (!faceDetected || landmarks.empty())
    {
        return Rect();
    }
    Rect bound = getRegionRect(regionToSearch, faceRect, landmarks, isFaceDataUpdated);
    bound = applyWorkarounds(bound, regionToSearch);
    return bound;
}

Mat RealSense::getTransform()
{
    return cachedTransformation;
}

void RealSense::resetTracking()
{
    {
        lock_guard<recursive_mutex> lk(mutex);
        faceDetected = false;
        eyeCenters = pair<Point2i, Point2i>();
        stabData.clear();
        recalculateGlasses = true;
        isGlassesFound = (!useAutoGlassesSearch) ? true : false;
        regionsLocation.clear();
        resetAction = true;
    }
    closeSearcherThread();
}

void RealSense::setFaceTrackedData(cv::Rect _faceRect, std::vector<cv::Point> _landmarks)
{
    faceRect = _faceRect;
    landmarks = _landmarks;
    isRealSenseVideoSourceCreated = true;
    faceDetected = (faceRect != Rect() && landmarks.size() != 0) ? true : false;
}

void RealSense::getTransform(const vector<Point>& landmarks)
{
    if (!landmarks.empty())
    {
        cachedTransformation = Mat();
        Point2d source[10], target[10];

        source[0] = landmarks[LM_REYE_R];
        source[1] = landmarks[LM_REYE_L];
        source[2] = landmarks[LM_LEYE_R];
        source[3] = landmarks[LM_LEYE_L];
        source[4] = landmarks[LM_MOUTH_R];
        source[5] = landmarks[LM_MOUTH_L];
        source[6] = landmarks[LM_NOSET_R];
        source[7] = landmarks[LM_NOSET_L];
        source[8] = landmarks[LM_NOSET_U];
        source[9] = landmarks[LM_NOSET_D];

        int k = 0;
        for(auto iter = canonicalLandmark.begin(); iter != canonicalLandmark.end(); iter++)
        {
            target[k++] = iter->second;
        }

        Mat_<double> A(20, 4, CV_64FC1);
        for (int i = 0; i < 10; i++)
        {
            A(i * 2, 0) = source[i].x;
            A(i * 2, 1) = -source[i].y;
            A(i * 2, 2) = 1.0;
            A(i * 2, 3) = 0.0;
            A(i * 2 + 1, 0) = source[i].y;
            A(i * 2 + 1, 1) = source[i].x;
            A(i * 2 + 1, 2) = 0.0;
            A(i * 2 + 1, 3) = 1.0;
        }

        Mat_<double> B(20, 1, CV_64FC1);
        for (int i = 0; i < 10; i++)
        {
            B(i * 2, 0) = target[i].x;
            B(i * 2 + 1, 0) = target[i].y;
        }

        Mat_<double> x = Mat::zeros(4, 1, CV_64FC1);
        solve(A, B, x, DECOMP_SVD);

        double angle = atan(x(1) / x(0));

        cachedTransformation = (Mat_<double>(3, 3) <<
            cos(angle), -sin(angle), x(2),
            sin(angle), cos(angle), x(3),
            0.0, 0.0, 1.0);

        //corrections for dx and dy, the center of rotation is the tip of the nose
        Point2d dxdy = transformPoint(source[8].x, source[8].y, cachedTransformation);
        cachedTransformation.at<double>(0, 2) -= dxdy.x - source[8].x;
        cachedTransformation.at<double>(1, 2) -= dxdy.y - source[8].y;
    }
}

Rect RealSense::getRegionRect(ivcp::RegionType regionType, const Rect& faceRect, const vector<Point>& landmarks, bool isFaceDataUpdated, bool isWAImplemented)
{
    Rect bound = Rect();
    if (isFaceDataUpdated)
    {
        if (!useTemplateRegions)
        {
            switch(regionType)
            {
            case REGION_MOUTH:
                bound = locateMouth(faceRect, landmarks);
                break;
            case REGION_NOSE:
                bound = locateNose(faceRect, landmarks);
                break;
            case REGION_LEYE:
                bound = locateLEye(faceRect, landmarks);
                break;
            case REGION_REYE:
                bound = locateREye(faceRect, landmarks);
                break;
            case REGION_LCHEEK:
                bound = locateLCheek(faceRect, landmarks);
                break;
            case REGION_RCHEEK:
                bound = locateRCheek(faceRect, landmarks);
                break;
            case REGION_FACE:
                bound = locateFace(faceRect, landmarks);
                break;
            case REGION_LEYEBROW:
                bound = locateLEyebrow(faceRect, landmarks);
                break;
            case REGION_REYEBROW:
                bound = locateREyebrow(faceRect, landmarks);
                break;
            default:
                IVCP_EXCEPTION(IncorrectParamException, "Unknown face region");
                break;
            }
        }
        else
        {
            auto iter = templateMask.find(regionType);
            if (iter != templateMask.end())
            {
                Point2d first = transformPoint(iter->second.first.x, iter->second.first.y, similarityTransformation);
                Point2d last = transformPoint(iter->second.second.x, iter->second.second.y, similarityTransformation);           
                bound = Rect((int)first.x, (int)first.y, (int)(last.x - first.x), (int)(last.y - first.y));
            }
            else
            {
                IVCP_EXCEPTION(IncorrectParamException, "Unknown face region");
            }
        }
        regionsLocation[regionType] = bound;
    }
    else
    {
        if (!regionsLocation.empty())
        {
            auto iter = regionsLocation.find(regionType);
            if (iter != regionsLocation.end())
            {
                bound = iter->second;
            }
            else
            {
                bound = getRegionRect(regionType, faceRect, landmarks, true, isWAImplemented);
            }
        }
        else
        {
            bound = getRegionRect(regionType, faceRect, landmarks, true, isWAImplemented);
        }
    }
    if (isRegionsReady() || isWAImplemented)
    {
        return bound;
    }
    else
    {
        return Rect();
    }
}

Rect RealSense::locateFace(const Rect& face, const vector<Point>& landmarks)
{
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_FACE_R].x;
        bound.y = face.y;
        bound.width = landmarks[LM_FACE_L].x - landmarks[LM_FACE_R].x;
        bound.height = landmarks[LM_FACE_D].y - bound.y;
    }
    return bound;
}

Rect RealSense::locateLEyebrow(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_LEYEB_R].x;
        bound.y = landmarks[LM_LEYEB_U].y;
        bound.width = abs(landmarks[LM_LEYEB_L].x - landmarks[LM_LEYEB_R].x);
        bound.height = abs(landmarks[LM_LEYEB_U].y - landmarks[LM_LEYEB_R].y);

        if (glassesFixEnabled)
        {
            bound = fixEyebrow(true, bound, landmarks);
        }

        int eyebrowOffset = (int)(bound.height * glassesFixStrength);
        bound.y -= eyebrowOffset;
    }
    return bound;
}

Rect RealSense::locateREyebrow(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_REYEB_R].x;
        bound.y = landmarks[LM_REYEB_U].y;
        bound.width = abs(landmarks[LM_REYEB_L].x - landmarks[LM_REYEB_R].x);
        bound.height = abs(landmarks[LM_REYEB_U].y - landmarks[LM_REYEB_R].y);

        if (glassesFixEnabled)
        {
            bound = fixEyebrow(true, bound, landmarks);
        }

        int eyebrowOffset = (int)(bound.height * glassesFixStrength);
        bound.y -= eyebrowOffset;
    }
    return bound;
}

Rect RealSense::locateMouth(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_MOUTH_R].x;
        bound.y = landmarks[LM_MOUTH_U].y;
        bound.width = abs(landmarks[LM_MOUTH_L].x - landmarks[LM_MOUTH_R].x);
        bound.height = abs(landmarks[LM_MOUTH_U].y - landmarks[LM_MOUTH_D].y);
    }
    return bound;
}

Rect RealSense::locateNose(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_NOSE_R].x;
        bound.y = landmarks[LM_NOSE_U].y;
        bound.width = abs(landmarks[LM_NOSE_L].x - landmarks[LM_NOSE_R].x);
        bound.height = abs(landmarks[LM_NOSE_U].y - landmarks[LM_NOSE_D].y);
    }
    return bound;
}

Rect RealSense::locateLEye(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_LEYE_R].x;
        bound.y = landmarks[LM_LEYE_U].y;
        bound.width = abs(landmarks[LM_LEYE_L].x - landmarks[LM_LEYE_R].x);
        bound.height = abs(landmarks[LM_LEYE_U].y - landmarks[LM_LEYE_D].y);
    }
    return bound;
}

Rect RealSense::locateREye(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_REYE_R].x;
        bound.y = landmarks[LM_REYE_U].y;
        bound.width = abs(landmarks[LM_REYE_L].x - landmarks[LM_REYE_R].x);
        bound.height = abs(landmarks[LM_REYE_U].y - landmarks[LM_REYE_D].y);
    }
    return bound;
}

Rect RealSense::locateLCheek(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_NOSE_L].x;
        bound.y = landmarks[LM_FACE_L_2].y;
        bound.width = landmarks[LM_FACE_L_4].x - bound.x;
        bound.height = landmarks[LM_FACE_L_4].y - landmarks[LM_FACE_L_2].y;

        bound.height /= 2;
    }
    return bound;
}

Rect RealSense::locateRCheek(const Rect& face, const vector<Point>& landmarks)
{
    (void)face;
    Rect bound = Rect();
    if (faceDetected)
    {
        bound.x = landmarks[LM_FACE_R_4].x;
        bound.y = landmarks[LM_FACE_R_2].y;
        bound.width = landmarks[LM_NOSE_R].x - bound.x;
        bound.height = landmarks[LM_FACE_R_4].y - landmarks[LM_FACE_R_2].y;

        bound.height /= 2;
    }
    return bound;
}

Rect RealSense::fixEyebrow(bool isLeft, Rect eyebrowRect, const vector<Point>& landmarks)
{
    Rect result = eyebrowRect;

    Point2d headVector = (landmarks[LM_REYEB_U] - landmarks[LM_REYE_U]) + (landmarks[LM_LEYEB_U] - landmarks[LM_LEYE_U]);
    headVector *= 1.0/sqrt(headVector.x * headVector.x + headVector.y * headVector.y);

    vector<int> base;
    base.push_back(LM_REYEB_R);
    base.push_back(LM_REYEB_U);
    base.push_back(LM_REYEB_L);
    Point2d eyePoint = landmarks[LM_REYE_U];
    if (isLeft)
    {
        base.clear();
        base.push_back(LM_LEYEB_R);
        base.push_back(LM_LEYEB_U);
        base.push_back(LM_LEYEB_L);
        eyePoint = landmarks[LM_LEYE_U];
    }

    double minDot = 0;
    for (int i = 0; i < 3; i++)
    {
        Point2d rectPoints[] = {Point2d(result.x, result.y),
                                Point2d(result.x + result.width, result.y),
                                Point2d(result.x, result.y + result.height),
                                Point2d(result.x + result.width, result.y + result.height)};
        for (int j = 0; j < 4; j ++)
        {
            Point2d diffVec = rectPoints[j] - (Point2d)landmarks[base[i]];

            double dot = 0;
            if (((dot = diffVec.dot(headVector)) < 0) && (dot < minDot))
            {
                minDot = dot;
            }
        }
    }

    if (minDot < 0)
    {
        result.x -= cvRound(minDot * headVector.x * 1.15);
        result.y -= cvRound(minDot * headVector.y * 1.15);
    }

    return result;
}

void RealSense::calculateSimilarityTransformation()
{
    map<int, Point2d> tempLandmarks;
    tempLandmarks[0] = landmarks[LM_REYE_R];
    tempLandmarks[1] = landmarks[LM_REYE_L];
    tempLandmarks[2] = landmarks[LM_LEYE_R];
    tempLandmarks[3] = landmarks[LM_LEYE_L];
    tempLandmarks[4] = landmarks[LM_MOUTH_R];
    tempLandmarks[5] = landmarks[LM_MOUTH_L];
    tempLandmarks[6] = landmarks[LM_NOSET_R];
    tempLandmarks[7] = landmarks[LM_NOSET_L];
    tempLandmarks[8] = landmarks[LM_NOSET_U];
    tempLandmarks[9] = landmarks[LM_NOSET_D];

    Mat_<double> B(20, 1, CV_64FC1);
    int count = 0;
    for (auto iter = tempLandmarks.begin(); iter != tempLandmarks.end(); iter++)
    {
        B(count++, 0) = iter->second.x;
        B(count++, 0) = iter->second.y;
    }

    Mat_<double> A(20, 3, CV_64FC1);
    count = 0;
    for (auto iter = canonicalLandmark.begin(); iter != canonicalLandmark.end(); iter++)
    {
        A(count, 0) = iter->second.x;
        A(count, 1) = 1.0;
        A(count, 2) = 0.0;
        count++;
        A(count, 0) = iter->second.y;
        A(count, 1) = 0.0;
        A(count, 2) = 1.0;
        count++;
    }

    Mat_<double> x = Mat::zeros(3, 1, CV_64FC1);
    solve(A, B, x, DECOMP_SVD);

    similarityTransformation = (Mat_<double>(3, 3) <<
        x(0), 0.0, x(1),
        0.0, x(0), x(2),
        0.0, 0.0, 1.0);
}


Rect RealSense::applyWorkarounds(Rect inputRect, RegionType regionType)
{
    Rect outputRect = inputRect;
    if (outputRect != Rect() && eyeCenters.first != Point() && useAutoGlassesSearch && isGlassesFound)
    {
        if (regionType == REGION_LEYEBROW || regionType == REGION_REYEBROW)
        {
            outputRect = autoGlassesSearch(inputRect);
        }
        if (regionType == REGION_LCHEEK || regionType == REGION_RCHEEK)
        {
            outputRect = autoGlassesDownRimSearch(inputRect);
        }
    }
    if (outputRect != Rect() && cutBackground)
    {
        if (regionType == REGION_LCHEEK || regionType == REGION_RCHEEK || regionType == REGION_LEYEBROW || regionType == REGION_REYEBROW)
        {
            outputRect = cutBackgroundFromRegions(inputRect, regionType);
        }
    }
    return outputRect;
}

pair<Point, Point> RealSense::getEyeCenters()
{
    pair<Point, Point> eyes = pair<Point, Point>();
    eyes.second = Point(landmarks[LM_LEYE_C]);
    eyes.first = Point(landmarks[LM_REYE_C]);

    return eyes;
}

pair<int, int> RealSense::getEyebrowRegionsWidth()
{
    pair<int, int> eyebrowsWidth = pair<int, int>();

    Rect tmpEyebrow = getRegionRect(REGION_LEYEBROW, faceRect, landmarks, false, true);
    eyebrowsWidth.first = tmpEyebrow.width;

    tmpEyebrow = getRegionRect(REGION_REYEBROW, faceRect, landmarks, false, true);
    eyebrowsWidth.second = tmpEyebrow.width;

    return eyebrowsWidth;
}

struct Context
{
    map<int, Point2d> templateGlasses;
    Mat gradSearch;
    Mat transf;
    double s;
};

class hypoGlassesWeight:public cv::optim::Solver::Function
{
public:
    Context* con;
    hypoGlassesWeight(Context* _con)
    {
        con = _con;
    }
    double calc(const double* x) const
    {
        double dx = x[0];
        if (dx < MIN_G_DX || dx > MAX_G_DX)
        {
            return 0;
        }
        double dy = x[1];
        if (dy > MAX_G_DY || dy < MIN_G_DY)
        {
            return 0;
        }

        Mat tryTransf = con->transf.clone();
        tryTransf.at<double>(0, 2) -= dx;
        tryTransf.at<double>(1, 2) -= dy;

        map<int, Point2d> tmpPoints;
        for (auto iter = con->templateGlasses.begin(); iter != con->templateGlasses.end(); iter++)
        {
            tmpPoints[iter->first] = transformPoint(iter->second.x, iter->second.y, tryTransf);
        }

        Mat LensHypo = Mat::zeros(con->gradSearch.size(), con->gradSearch.type());
        cv::line(LensHypo, tmpPoints[1], tmpPoints[2], Scalar(255), 5);
        cv::line(LensHypo, tmpPoints[3], tmpPoints[4], Scalar(255), 5);
        cv::line(LensHypo, tmpPoints[5], tmpPoints[6], Scalar(255), 3);
        cv::line(LensHypo, tmpPoints[7], tmpPoints[8], Scalar(255), 3);
        cv::line(LensHypo, tmpPoints[9], tmpPoints[10], Scalar(255), 3);
        cv::line(LensHypo, tmpPoints[11], tmpPoints[12], Scalar(255), 2);
        cv::line(LensHypo, tmpPoints[13], tmpPoints[14], Scalar(255), 2);

        return (-1) * ((cv::sum(con->gradSearch & LensHypo)[0]) / con->s);
    }
};

void RealSense::searcherThread(RealSense* pRS)
{
    Mat image;
    pair<Point, Point> eyeCenters;
    pair<Point, Point> templateEyePos;
    map<int, Point2d> templateGlasses;
    int leftEyebrowWidth, rightEyebrowWidth;
    {
        lock_guard<recursive_mutex> lk(pRS->mutex);
        //get eyebrow regions
        pair<int, int> eyebrowsWidth = pRS->getEyebrowRegionsWidth();
        leftEyebrowWidth = eyebrowsWidth.first;
        rightEyebrowWidth = eyebrowsWidth.second;

        //copy data from main thread
        image = pRS->searcherImage.clone();
        eyeCenters = pRS->eyeCenters;
        templateEyePos = pRS->templateEyePos;
        templateGlasses = pRS->templateGlasses;
    }

    //calculate grad
    Mat gradX, gradY, gradXY;
    cv::Sobel(image, gradX, CV_16S, 1, 0);
    cv::convertScaleAbs(gradX, gradX);
    cv::Sobel(image, gradY, CV_16S, 0, 1);
    cv::convertScaleAbs(gradY, gradY);
    cv::addWeighted(gradX, 0.5, gradY, 0.5, 0, gradXY);

    //search glasses position
    double maxWeight = 0.0;
    pair<Point2d, Point2d> glasses = eyeCenters;
    pair<Point2d, Point2d> rim = eyeCenters;

    Mat B = (Mat_<double>(4, 1) <<
        eyeCenters.first.x,
        eyeCenters.first.y,
        eyeCenters.second.x,
        eyeCenters.second.y);

    Mat A = (Mat_<double>(4, 3) <<
        templateEyePos.first.x, 1.0, 0.0,
        templateEyePos.first.y, 0.0, 1.0,
        templateEyePos.second.x, 1.0, 0.0,
        templateEyePos.second.y, 0.0, 1.0);

    Mat_<double> x = Mat::zeros(3, 1, CV_64FC1);
    cv::solve(A, B, x, DECOMP_SVD);
    Mat similarity = (Mat_<double>(3, 3) <<
        x(0), 0.0, x(1),
        0.0, x(0), x(2),
        0.0, 0.0, 1.0);

    map<int, Point2d> tryTemplateGlasses;
    for (auto iter = templateGlasses.begin(); iter != templateGlasses.end(); iter++)
    {
        tryTemplateGlasses[iter->first] = transformPoint(iter->second.x, iter->second.y, similarity);
    }
    Point2i rotationCenter = Point2i((int)((tryTemplateGlasses[FIRST_GLASSES_POINT].x + tryTemplateGlasses[SECOND_GLASSES_POINT].x) / 2), 
        (int)((tryTemplateGlasses[FIRST_GLASSES_POINT].y + tryTemplateGlasses[SECOND_GLASSES_POINT].y) / 2));

    for (double s = MIN_SCALE; s < MAX_SCALE; s += STEP_SCALE)
    {
        for (double alpha = MIN_ALPHA; alpha < MAX_ALPHA; alpha += STEP_ALPHA)
        {
            double angle = atan(alpha);
            Mat rotateTransf = (Mat_<double>(3, 3) <<
                                s * cos(angle), -s * sin(angle), 0.0,
                                s * sin(angle), s * cos(angle), 0.0,
                                0.0, 0.0, 1.0);
            Point2i tmpCenter = transformPoint(rotationCenter.x, rotationCenter.y, rotateTransf);
            rotateTransf.at<double>(0, 2) = -tmpCenter.x + rotationCenter.x;
            rotateTransf.at<double>(1, 2) = -tmpCenter.y + rotationCenter.y;

            if (pRS->searcherStatus == THREAD_EXITING)
            {
                lock_guard<recursive_mutex> lk(pRS->mutex);
                pRS->searcherStatus = THREAD_FINALIZED;
                pRS->cvTimeToExit.notify_one();
                return;
            }
            Context* context = new Context();
            context->gradSearch = gradXY;
            context->templateGlasses = tryTemplateGlasses;
            context->s = s;
            context->transf = rotateTransf.clone();
            Ptr<optim::Solver::Function> func(new hypoGlassesWeight(context));
            Mat simplex = (Mat_<double>(1, 2) << SIMPLEX_INITIAL_POINT, SIMPLEX_INITIAL_POINT);
            Mat step = (Mat_<double>(2, 1) << SIMPLEX_INITIAL_STEP_DX, SIMPLEX_INITIAL_STEP_DY);
            TermCriteria crit(cv::TermCriteria::COUNT + cv::TermCriteria::EPS, SIMPLEX_MAX_COUNT, SIMPLEX_EPSILON);

            optim::DownhillSolverImpl downhillSolver = optim::DownhillSolverImpl();
            downhillSolver.setFunction(func);
            downhillSolver.setInitStep(step);
            downhillSolver.setTermCriteria(crit);

            double weight  = (-1) * downhillSolver.minimize(simplex);
            if (weight > maxWeight)
            {
                maxWeight = weight;
                Mat tryTransf = rotateTransf.clone();
                tryTransf.at<double>(0, 2) -= simplex.at<double>(0);
                tryTransf.at<double>(1, 2) -= simplex.at<double>(1);
                glasses.first = transformPoint(tryTemplateGlasses[FIRST_GLASSES_POINT].x, tryTemplateGlasses[FIRST_GLASSES_POINT].y, tryTransf);
                glasses.second = transformPoint(tryTemplateGlasses[SECOND_GLASSES_POINT].x, tryTemplateGlasses[SECOND_GLASSES_POINT].y, tryTransf);
                rim.first = transformPoint(tryTemplateGlasses[FIRST_DOWN_RIM_POINT].x, tryTemplateGlasses[FIRST_DOWN_RIM_POINT].y, tryTransf);
                rim.second = transformPoint(tryTemplateGlasses[SECOND_DOWN_RIM_POINT].x, tryTemplateGlasses[SECOND_DOWN_RIM_POINT].y, tryTransf);
            }
            delete context;
        }
    }

    //calculate ratio of length between center of eyes and glasses to eyebrows width
    double kGlassesLine = (double)(glasses.second.y - glasses.first.y) / (glasses.second.x - glasses.first.x);
    double bGlassesLine = glasses.first.y - kGlassesLine * glasses.first.x;
    pRS->rightGlassRatio = (double)(eyeCenters.first.y - (kGlassesLine * eyeCenters.first.x + bGlassesLine)) / rightEyebrowWidth;
    pRS->leftGlassRatio = (double)(eyeCenters.second.y - (kGlassesLine * eyeCenters.second.x + bGlassesLine)) / leftEyebrowWidth;

    //calculate ratio of length between center of eyes and rim to eyebrows width
    double kRimLine = (double)(rim.second.y - rim.first.y) / (rim.second.x - rim.first.x);
    double bRimLine = rim.first.y - kRimLine * rim.first.x;
    pRS->rightRimRatio = (double)(eyeCenters.first.y - (kRimLine * eyeCenters.first.x + bRimLine)) / rightEyebrowWidth;
    pRS->leftRimRatio = (double)(eyeCenters.second.y - (kRimLine * eyeCenters.second.x + bRimLine)) / leftEyebrowWidth;

    unique_lock<recursive_mutex> lk(pRS->mutex);
    pRS->isGlassesFound = true;
    pRS->searcherStatus = THREAD_FINALIZED;
    pRS->cvTimeToExit.notify_one();
}

void RealSense::closeSearcherThread()
{
    if (searcher.joinable())
    {
        if (searcherStatus == THREAD_RUNNING)
        {
            searcherStatus = THREAD_EXITING;
            cvTimeToExit.notify_one();
            if (searcher.joinable())
            {
                unique_lock<recursive_mutex> lk(mutex);
                //Waiting for the worker thread to signal it is exited
                cvTimeToExit.wait(lk, [&] { return searcherStatus == THREAD_FINALIZED; } );
            }
            isGlassesFound = false;
        }
        searcher.join();
    }
}

Rect RealSense::autoGlassesSearch(Rect eyebrowRect)
{
    Rect result = eyebrowRect;

    pair<int, int> eyebrowsWidth = getEyebrowRegionsWidth();
    Point2d glassPoint1 = Point2d(eyeCenters.first.x, eyeCenters.first.y - eyebrowsWidth.second * rightGlassRatio); // right eye and eyebrow
    Point2d glassPoint2 = Point2d(eyeCenters.second.x, eyeCenters.second.y - eyebrowsWidth.first * leftGlassRatio); // left eye and eyebrow
    double kGlassesLine = (double)(glassPoint2.y - glassPoint1.y) / (glassPoint2.x - glassPoint1.x);
    double bGlassesLine = glassPoint1.y - kGlassesLine * glassPoint1.x;

    double leftIntersectionY = kGlassesLine * result.x + bGlassesLine;
    double rightIntersectionY = kGlassesLine * (result.x + result.width) + bGlassesLine;
    int diff = max((int)((result.y + result.height) - leftIntersectionY), (int)((result.y + result.height) - rightIntersectionY));
    int dy = (int)(result.height * MIN_EYEBROW_REGION_PART);
    if (diff > -dy)
    {
        int upStep = min(dy + diff, (int)(result.height * MAX_EYEBROW_REGION_PART));
        result.y -= upStep;
    }
    return result;
}

Rect RealSense::autoGlassesDownRimSearch(Rect cheekRect)
{
    Rect result = cheekRect;

    pair<int, int> eyebrowsWidth = getEyebrowRegionsWidth();
    Point2d rimPoint1 = Point2d(eyeCenters.first.x, eyeCenters.first.y - eyebrowsWidth.second * rightRimRatio); // right eye and eyebrow
    Point2d rimPoint2 = Point2d(eyeCenters.second.x, eyeCenters.second.y - eyebrowsWidth.first * leftRimRatio); // left eye and eyebrow
    double kRimLine = (double)(rimPoint2.y - rimPoint1.y) / (rimPoint2.x - rimPoint1.x);
    double bRimLine = rimPoint1.y - kRimLine * rimPoint1.x;

    double leftIntersectionY = kRimLine * result.x + bRimLine;
    double rightIntersectionY = kRimLine * (result.x + result.width) + bRimLine;
    int diff = min((int)(result.y - leftIntersectionY), (int)(result.y - rightIntersectionY));
    int dy = (int)(result.height * MIN_CHEEK_REGION_PART);
    if (diff < dy)
    {
        int downStep = min(dy - diff, (int)(result.height * MAX_CHEEK_REGION_PART));
        result.y += downStep;
    }
    return result;
}


void RealSense::calculateFaceBorder()
{
    Point firstPoint = landmarks[LM_FACE_R_2];
    Point secondPoint = landmarks[LM_FACE_R_4];
    if (firstPoint.x == secondPoint.x)
    {
        //vertical line x = b
        kRightCheekFaceBorder = 0.0;
        bRightCheekFaceBorder = firstPoint.x;
    }
    else
    {
        //line y = k * x + b
        kRightCheekFaceBorder = (double)(secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x);
        bRightCheekFaceBorder = secondPoint.y - kRightCheekFaceBorder * secondPoint.x;
    }

    firstPoint = landmarks[LM_FACE_L_4];
    secondPoint = landmarks[LM_FACE_L_2];
    if (firstPoint.x == secondPoint.x)
    {
        //vertical line x = b
        kLeftCheekFaceBorder = 0.0;
        bLeftCheekFaceBorder = firstPoint.x;
    }
    else
    {
        //line y = k * x + b
        kLeftCheekFaceBorder = (double)(secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x);
        bLeftCheekFaceBorder = secondPoint.y - kLeftCheekFaceBorder * secondPoint.x;
    }

    firstPoint = landmarks[LM_FACE_R_4];
    secondPoint = Point((landmarks[LM_REYEB_R].x + landmarks[LM_FACE_R].x) / 2, (landmarks[LM_REYEB_R].y + landmarks[LM_FACE_R].y) / 2);
    if (firstPoint.x == secondPoint.x)
    {
        //vertical line x = b
        kRightEyebrowFaceBorder = 0.0;
        bRightEyebrowFaceBorder = firstPoint.x;
    }
    else
    {
        //line y = k * x + b
        kRightEyebrowFaceBorder = (double)(secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x);
        bRightEyebrowFaceBorder = secondPoint.y - kRightEyebrowFaceBorder * secondPoint.x;
    }

    firstPoint = landmarks[LM_FACE_L_4];
    secondPoint = Point((landmarks[LM_LEYEB_L].x + landmarks[LM_FACE_L].x) / 2, (landmarks[LM_LEYEB_L].y + landmarks[LM_FACE_L].y) / 2);
    if (firstPoint.x == secondPoint.x)
    {
        //vertical line x = b
        kLeftEyebrowFaceBorder = 0.0;
        bLeftEyebrowFaceBorder = firstPoint.x;
    }
    else
    {
        //line y = k * x + b
        kLeftEyebrowFaceBorder = (double)(secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x);
        bLeftEyebrowFaceBorder = secondPoint.y - kLeftEyebrowFaceBorder * secondPoint.x;
    }
}

Rect RealSense::cutBackgroundFromRegions(Rect regionRect, RegionType type)
{
    Rect finalRect = regionRect;
    double upIntersectionX = 0.0, downIntersectionX = 0.0;
    int cutLineX = 0;
    switch (type)
    {
    case ivcp::REGION_LEYEBROW:
        if (abs(kLeftEyebrowFaceBorder) < 0.0001)
        {
            cutLineX = min(regionRect.width + regionRect.x, (int)bLeftEyebrowFaceBorder);
        }
        else
        {
            upIntersectionX = (double)((regionRect.y - bLeftEyebrowFaceBorder) / kLeftEyebrowFaceBorder);
            downIntersectionX = (double)((regionRect.y + regionRect.height - bLeftEyebrowFaceBorder) / kLeftEyebrowFaceBorder);

            cutLineX = min((int)upIntersectionX, (int)downIntersectionX);
            cutLineX = min(cutLineX, regionRect.width + regionRect.x);
        }
        finalRect.width = cutLineX - regionRect.x;
        break;
    case ivcp::REGION_REYEBROW:
        if (abs(kRightEyebrowFaceBorder) < 0.0001)
        {
            cutLineX = max(regionRect.x, (int)bRightEyebrowFaceBorder);
        }
        else
        {
            upIntersectionX = (double)((regionRect.y - bRightEyebrowFaceBorder) / kRightEyebrowFaceBorder);
            downIntersectionX = (double)((regionRect.y + regionRect.height - bRightEyebrowFaceBorder) / kRightEyebrowFaceBorder);

            cutLineX = max((int)upIntersectionX, (int)downIntersectionX);
            cutLineX = max(cutLineX, regionRect.x);
        }
        finalRect.width = regionRect.width - (cutLineX - regionRect.x);
        finalRect.x = cutLineX;
        break;
    case ivcp::REGION_LCHEEK:
        if (abs(kLeftCheekFaceBorder) < 0.0001)
        {
            cutLineX = min(regionRect.width + regionRect.x, (int)bLeftCheekFaceBorder);
        }
        else
        {
            upIntersectionX = (double)((regionRect.y - bLeftCheekFaceBorder) / kLeftCheekFaceBorder);
            downIntersectionX = (double)((regionRect.y + regionRect.height - bLeftCheekFaceBorder) / kLeftCheekFaceBorder);

            cutLineX = min((int)upIntersectionX, (int)downIntersectionX);
            cutLineX = min(cutLineX, regionRect.width + regionRect.x);
        }
        finalRect.width = cutLineX - regionRect.x;
        break;
    case ivcp::REGION_RCHEEK:
        if (abs(kRightCheekFaceBorder) < 0.0001)
        {
            cutLineX = min(regionRect.x, (int)bLeftCheekFaceBorder);
        }
        else
        {
            upIntersectionX = (double)((regionRect.y - bRightCheekFaceBorder) / kRightCheekFaceBorder);
            downIntersectionX = (double)((regionRect.y + regionRect.height - bRightCheekFaceBorder) / kRightCheekFaceBorder);

            cutLineX = max((int)upIntersectionX, (int)downIntersectionX);
            cutLineX = max(cutLineX, regionRect.x);
        }
        finalRect.width = regionRect.width - (cutLineX - regionRect.x);
        finalRect.x = cutLineX;
        break;
    default:
        break;
    }
    if (finalRect.width == 0)
    {
        finalRect.width = 10;
    }

    return finalRect;
}

}

#endif