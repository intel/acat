///////////////////////////////////////////////////////////////////////////
// <copyright file="dlib.cpp" company="Intel Corporation">
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
#include "libinfra/dlib.hpp"
#include "libinfra/optim.hpp"
#include "libinfra/face_template.hpp"
#include "libinfra/region_handler_defines.hpp"

using namespace cv;
using namespace std;

//face landmarks
#define LM_FACE_R 0
#define LM_FACE_R_2 1
#define LM_FACE_R_4 3
#define LM_FACE_L 16
#define LM_FACE_L_2 15
#define LM_FACE_L_4 13
#define LM_FACE_D 8
//left eyebrow landmarks
#define LM_LEYEB_R 22
#define LM_LEYEB_L 26
#define LM_LEYEB_U 24
//right eyebrow landmarks
#define LM_REYEB_R 17
#define LM_REYEB_L 21
#define LM_REYEB_U 19
//left eye landmarks
#define LM_LEYE_U 44
#define LM_LEYE_R 42
#define LM_LEYE_D 46
#define LM_LEYE_L 45
//right eye landmarks
#define LM_REYE_U 37
#define LM_REYE_L 39
#define LM_REYE_D 41
#define LM_REYE_R 36
//mouth landmarks
#define LM_MOUTH_R 48
#define LM_MOUTH_L 54
#define LM_MOUTH_U 52
#define LM_MOUTH_D 57
//nose and nose tip landmarks
#define LM_NOSE_L 35
#define LM_NOSE_R 31
#define LM_NOSE_D 33
#define LM_NOSE_U 29
#define LM_NOSET_R 32
#define LM_NOSET_L 34
#define LM_NOSET_U 30
#define LM_NOSET_D 33

#ifdef USE_DLIB

namespace ivcp
{

Dlib::Dlib(float _stabThreshold,
        unsigned int _stabFrames,
        int _normFrames,
        int _noFaceFrames,
        bool _noFaceClockwiseRotation,
        bool _glassesFixEnabled,
        bool _useTemplateRegions,
        float _glassesFixStrength,
        bool _useAutoGlassesSearch,
        bool _cutBackground,
        double _resizeDownscale)
{
    ffd = dlib::get_frontal_face_detector();

    string landmarksPath = DLIB_FACE_LANDMARKS_FILE;
    size_t dot = landmarksPath.find_last_of(".");
    string landmarksFilename = landmarksPath.substr(landmarksPath.find_last_of("\\/") + 1, dot);
    try
    {
        dlib::deserialize(DLIB_FACE_LANDMARKS_FILE) >> pose_model;
    }
    catch(dlib::serialization_error& e)
    {
        IVCP_EXCEPTION(IncorrectParamException, landmarksFilename + " can't be opened\n" + string(e.what()));
    }

    stabThreshold = _stabThreshold;
    stabFrames = _stabFrames;
    normFrames = _normFrames;
    noFaceFrames = _noFaceFrames,
    noFaceClockwiseRotation = _noFaceClockwiseRotation;
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
    resizeDownscale = _resizeDownscale;

    frameCounter16Bit = 0;
    min16BitValue = 0;
    max16bitValue = 4095;

    cachedFrame = VIDEO_PLAYLIST_FRAME_EOF;

    if (useTemplateRegions)
    {
        templateMask = getTemplateMask(DLIB_REGION);
    }

    canonicalLandmark = getCanonicalLandmark();

    isRotationAngleFound = false;
    faceDetected = false;
    searchFace = false;
    noFaceNumFrames = 0;
    frameNum = 0;
    rotationAngle = 0.0;
    prevRotationAngle = 360.0;
    tryAngle = 0.0;
    countAnglesWithDetection = 0;

    recalculateGlasses = true;
    eyeCenters = pair<Point2i, Point2i>();
    searcherStatus = THREAD_FINALIZED;
    isGlassesFound = (!useAutoGlassesSearch) ? true : false;
    if (useAutoGlassesSearch)
    {
        templateGlasses = getTemplateGlasses();
        templateEyePos = getTemplateEyePos(DLIB_REGION);
    }

	//SS 05/14/2023
	stableRegionMarker.noseBottom = cv::Point2d(-DBL_MAX, -DBL_MAX);
	stableRegionMarker.noseLeft = cv::Point2d(-DBL_MAX, -DBL_MAX);
	stableRegionMarker.noseRight = cv::Point2d(-DBL_MAX, -DBL_MAX);
	stableRegionMarker.noseTop = cv::Point2d(-DBL_MAX, -DBL_MAX);

}

Dlib::~Dlib()
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

IDlib::Settings Dlib::getHandlerSettings()
{
    lock_guard<recursive_mutex> lk(mutex);
    return IDlib::Settings(stabThreshold, stabFrames, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled, 
        useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);
}

void Dlib::setHandlerSettings(IDlib::Settings settings)
{
    lock_guard<recursive_mutex> lk(mutex);
    stabThreshold = settings.stabThreshold;
    stabFrames = settings.stabFrames;
    normFrames = settings.normFrames;
    noFaceFrames = settings.noFaceFrames;
    noFaceClockwiseRotation = settings.noFaceClockwiseRotation;
    glassesFixEnabled = settings.glassesFixEnabled;
    useTemplateRegions = settings.useTemplateRegions;
    glassesFixStrength = settings.glassesFixStrength;
    useAutoGlassesSearch = settings.useAutoGlassesSearch;
    cutBackground = settings.cutBackground;
    resizeDownscale = settings.resizeDownscale;
}

Rect Dlib::getRegion(Mat srcImage, VideoPlaylistFrame currentFrame, RegionType regionToSearch, bool normalize)
{
	try {
    lock_guard<recursive_mutex> lkSettings(mutex);

    if(searchFace)
    {
        if (!isRotationAngleFound && tryAngle < 360.0f && tryAngle > 0.0f)
        {
            return Rect();
        }
        else
        {
            if (faceDetected)
            {
                IVCP_LOG_WARNING("The workaround worked! The pipeline will continue in normal mode.");
                eyeCenters = pair<Point2i, Point2i>();
                recalculateGlasses = true;
                isGlassesFound = (!useAutoGlassesSearch) ? true : false;
            }
            else
            {
                IVCP_LOG_WARNING("The workaround did not work. The pipeline will continue in normal mode.");
                IVCP_LOG_WARNING("Will try to find face again after %d frames.", noFaceFrames);
                tryAngle = 0.0;
                rotationAngle = 0.0;
            }
            searchFace = false;
            noFaceNumFrames = 0;
        }
    }
    if(currentFrame != cachedFrame)
    {
        frameNum++;
        cachedFrame = currentFrame;
        Mat mat = rotateImageDlib(srcImage, rotationAngle);
        mat = prepareImageForFaceSearch(mat);

#if 0//SSdebug
		Mat resizeMat;
		//resize(mat, resizeMat, Size(), 0.5, 0.5, cv::INTER_AREA); 
		resize(mat, resizeMat, Size(), 1.2, 1.2, cv::INTER_CUBIC); 
		dlib::cv_image<dlib::bgr_pixel> cimg(resizeMat);
#else
        dlib::cv_image<dlib::bgr_pixel> cimg(mat);
#endif
        vector<dlib::rectangle> faces = ffd(cimg);
        if (faces.size() != 0)
        {
            faceDetected = true;
            noFaceNumFrames = 0;

            getLandmarks(cimg, faces);

            for(auto iter = landmarks.begin(); iter != landmarks.end(); iter++)
            {
                *iter = transformPoint(iter->x, iter->y, rerotationMat);
            }

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

            for(auto iter = landmarks.begin(); iter != landmarks.end(); iter++)
            {
                *iter = transformPoint(iter->x, iter->y, cachedTransformation);
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
                    searcher = thread(Dlib::searcherThread, this);
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
        else
        {
            faceDetected = false;
            noFaceNumFrames++;
            if (noFaceFrames > 0 && ((noFaceNumFrames % noFaceFrames) == 0))
            {
                IVCP_LOG_WARNING("Dlib is unable to find face on the image. Trying to apply a workaround. Please wait...");
                rotationAngle = 0.0;
                tryAngle = (noFaceClockwiseRotation)?(2.0f):(358.0f);
                searchFace = true;
                isRotationAngleFound = false;
                countAnglesWithDetection = 0;
                isFaceDataUpdated = true;
            }
            return Rect();
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
     catch (exception&)
	{
		IVCP_EXCEPTION(IncorrectParamException, "Dlib::getRegion exception");
	}
	
}

Mat Dlib::getTransform()
{
    return cachedTransformation;
}

int Dlib::getNormFrames()
{
    return normFrames;
}

void Dlib::setNormFrames(int _normFrames)
{
    normFrames = _normFrames;
}

int Dlib::getNoFaceFrames()
{
    return noFaceFrames;
}

void Dlib::setNoFaceFrames(int _noFaceFrames)
{
    noFaceFrames = _noFaceFrames;
}

void Dlib::resetTracking()
{
    {
        lock_guard<recursive_mutex> lk(mutex);
        isRotationAngleFound = false;
        rotationAngle = 0.0;
        tryAngle = 0.0;
        faceDetected = false;
        searchFace = false;
        countAnglesWithDetection = 0;
        eyeCenters = pair<Point2i, Point2i>();
        stabData.clear();
        searchFace = false;
        recalculateGlasses = true;
        isGlassesFound = (!useAutoGlassesSearch) ? true : false;
        regionsLocation.clear();
        noFaceNumFrames = 0;
		//SS 05/14/2023
		
		stableRegionMarker.noseBottom = cv::Point2d(-DBL_MAX, -DBL_MAX);
		stableRegionMarker.noseLeft = cv::Point2d(-DBL_MAX, -DBL_MAX);
		stableRegionMarker.noseRight = cv::Point2d(-DBL_MAX, -DBL_MAX);
		stableRegionMarker.noseTop = cv::Point2d(-DBL_MAX, -DBL_MAX);
		
    }
    closeSearcherThread();
}

void Dlib::rotateImageWA(Mat image)
{
    lock_guard<recursive_mutex> lkSettings(mutex);
    Mat mat = rotateImageDlib(image, tryAngle);
    mat = prepareImageForFaceSearch(mat);

    dlib::cv_image<dlib::bgr_pixel> cimg(mat);
    vector<dlib::rectangle> faces = ffd(cimg);

    if (faces.size() != 0)
    {
        faceDetected = true;
        countAnglesWithDetection++;
        if (countAnglesWithDetection >= MIN_FRAME_COUNT_GOOD_ANGLE)
        {
            rotationAngle = tryAngle;
            isRotationAngleFound = true;
        }
    }
    else
    {
        faceDetected = false;
        countAnglesWithDetection = 0;
        changeAngleWA();
    }
}

void Dlib::normalizeBrightness(const Mat &src16bit, Mat &dst8bit)
{
    if (normFrames > 0 && (frameCounter16Bit % normFrames == 0))
    {
        double minVal = 0.0;
        double maxVal = 4095.0;
        minMaxLoc(src16bit, &minVal, &maxVal);

        min16BitValue = (unsigned int)minVal;
        max16bitValue = (unsigned int)maxVal;
    }

    src16bit -= min16BitValue;
    src16bit.convertTo(dst8bit, CV_8UC1, 256.0/(max16bitValue - min16BitValue + 1));

    frameCounter16Bit++;
}

void Dlib::getLandmarks(dlib::cv_image<dlib::bgr_pixel> img, vector<dlib::rectangle> faces)
{
    int biggestFace = 0;
    unsigned long biggestArea = 0;
    for (size_t i = 0; i < faces.size(); i++)
    {
        unsigned long area = faces[i].area();
        if (area > biggestArea)
        {
            biggestArea = area;
            biggestFace = (int)i;
        }
    }

    dlib::full_object_detection shape;
    shape = pose_model(img, faces[biggestFace]);

    //make faceRect and resize it back
    int faceRectX = (int)(faces[biggestFace].left()/resizeDownscale);
    int faceRectY = (int)(faces[biggestFace].top()/resizeDownscale);
    int faceRectW = (int)(faces[biggestFace].width()/resizeDownscale);
    int faceRectH = (int)(faces[biggestFace].height()/resizeDownscale);
    faceRect = Rect(faceRectX, faceRectY, faceRectW, faceRectH);

    //make landmarks and resize them back
    landmarks.clear();
    for (unsigned long i = 0; i < shape.num_parts(); i++)
    {
        landmarks.push_back(Point((int)(shape.part(i).x()/resizeDownscale), (int)(shape.part(i).y()/resizeDownscale)));
    }
}

void Dlib::getTransform(const vector<Point>& points)
{
    if (!points.empty())
    {
        cachedTransformation = Mat();
        Point2d source[10], target[10];

        source[0] = points[LM_REYE_R];
        source[1] = points[LM_REYE_L];
        source[2] = points[LM_LEYE_R];
        source[3] = points[LM_LEYE_L];
        source[4] = points[LM_MOUTH_R];
        source[5] = points[LM_MOUTH_L];
        source[6] = points[LM_NOSET_R];
        source[7] = points[LM_NOSET_L];
        source[8] = points[LM_NOSET_U];
        source[9] = points[LM_NOSET_D];

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

        double rotationAngle_rad = rotationAngle * CV_PI / 180;
        double angle = ((rotationAngle_rad > -CV_PI / 2 && rotationAngle_rad < CV_PI / 2) || (rotationAngle_rad > 3 * CV_PI / 2) || (rotationAngle_rad < -3 * CV_PI / 2)) ? atan(x(1) / x(0)) : CV_PI - atan(x(1) / x(0));

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

Rect Dlib::locateFace(const Rect& face, const vector<Point>& points)
{
    Rect bound;
    bound.x = points[LM_FACE_R].x;
    bound.y = face.y;
    bound.width = points[LM_FACE_L].x - points[LM_FACE_R].x;
    bound.height = points[LM_FACE_D].y - bound.y;

    return bound;
}

Rect Dlib::locateLEyebrow(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_LEYEB_R].x;
    bound.y = points[LM_LEYEB_U].y;
    bound.width = abs(points[LM_LEYEB_L].x - points[LM_LEYEB_R].x);
    bound.height = abs(points[LM_LEYEB_U].y - points[LM_LEYEB_R].y);

    if (glassesFixEnabled)
    {
        bound = fixEyebrow(true, bound, points);
    }

    int eyebrowOffset = (int)(bound.height * glassesFixStrength);
    bound.y -= eyebrowOffset;

    return bound;
}

Rect Dlib::locateREyebrow(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_REYEB_R].x;
    bound.y = points[LM_REYEB_U].y;
    bound.width = abs(points[LM_REYEB_L].x - points[LM_REYEB_R].x);
    bound.height = abs(points[LM_REYEB_U].y - points[LM_REYEB_R].y);

    if (glassesFixEnabled)
    {
        bound = fixEyebrow(true, bound, points);
    }

    int eyebrowOffset = (int)(bound.height * glassesFixStrength);
    bound.y -= eyebrowOffset;

    return bound;
}

Rect Dlib::locateMouth(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_MOUTH_R].x;
    bound.y = points[LM_MOUTH_U].y;
    bound.width = abs(points[LM_MOUTH_L].x - points[LM_MOUTH_R].x);
    bound.height = abs(points[LM_MOUTH_U].y - points[LM_MOUTH_D].y);
    return bound;
}

Rect Dlib::locateNose(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_NOSE_R].x;
    bound.y = points[LM_NOSE_U].y;
    bound.width = abs(points[LM_NOSE_L].x - points[LM_NOSE_R].x);
    bound.height = abs(points[LM_NOSE_U].y - points[LM_NOSE_D].y);
    return bound;
}

Rect Dlib::locateLEye(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_LEYE_R].x;
    bound.y = points[LM_LEYE_U].y;
    bound.width = abs(points[LM_LEYE_L].x - points[LM_LEYE_R].x);
    bound.height = abs(points[LM_LEYE_U].y - points[LM_LEYE_D].y);
    return bound;
}

Rect Dlib::locateREye(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_REYE_R].x;
    bound.y = points[LM_REYE_U].y;
    bound.width = abs(points[LM_REYE_L].x - points[LM_REYE_R].x);
    bound.height = abs(points[LM_REYE_U].y - points[LM_REYE_D].y);
    return bound;
}

Rect Dlib::locateLCheek(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_NOSE_L].x;
    bound.y = points[LM_FACE_L_2].y;
    bound.width = points[LM_FACE_L_4].x - bound.x;
    bound.height = points[LM_FACE_L_4].y - points[LM_FACE_L_2].y;

    bound.height /= 2;
    return bound;
}

Rect Dlib::locateRCheek(const Rect& face, const vector<Point>& points)
{
    (void)face;
    Rect bound;
    bound.x = points[LM_FACE_R_4].x;
    bound.y = points[LM_FACE_R_2].y;
    bound.width = points[LM_NOSE_R].x - bound.x;
    bound.height = points[LM_FACE_R_4].y - points[LM_FACE_R_2].y;

    bound.height /= 2;
    return bound;
}

Rect Dlib::fixEyebrow(bool isLeft, Rect eyebrowRect, const vector<Point>& points)
{
    Rect result = eyebrowRect;

    Point2d headVector = (points[LM_REYEB_U] - points[LM_REYE_U]) + (points[LM_LEYEB_U] - points[LM_LEYE_U]);
    headVector *= 1.0/sqrt(headVector.x * headVector.x + headVector.y * headVector.y);

    vector<int> base;
    base.push_back(LM_REYEB_R);
    base.push_back(LM_REYEB_U);
    base.push_back(LM_REYEB_L);
    Point2d eyePoint = points[LM_REYE_U];
    if (isLeft)
    {
        base.clear();
        base.push_back(LM_LEYEB_R);
        base.push_back(LM_LEYEB_U);
        base.push_back(LM_LEYEB_L);
        eyePoint = points[LM_LEYE_U];
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
            Point2d basePoint = Point2d(points[base[i]].x, points[base[i]].y);
            Point2d diffVec = rectPoints[j] - basePoint;

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

void Dlib::calculateSimilarityTransformation()
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

Mat Dlib::rotateImageDlib(const Mat& source, double angle)
{
    lock_guard<recursive_mutex> lkSettings(mutex);
    Mat dst;
    if (prevRotationAngle != angle)
    {
        prevRotationAngle = angle;

        Point2f rotationCenter = Point2f(source.cols/2.0F+UP_CENTER_ROTATION, source.rows/2.0F);
        currentRotationMat = getRotationMatrix2D(rotationCenter, angle, 1.0);
        warpAffine(source, dst, currentRotationMat, source.size());

        Mat currentTransformMat = (Mat_<double>(3, 3) <<
            currentRotationMat.at<double>(0, 0), currentRotationMat.at<double>(0, 1), currentRotationMat.at<double>(0, 2),
            currentRotationMat.at<double>(1, 0), currentRotationMat.at<double>(1, 1), currentRotationMat.at<double>(1, 2),
            0.0, 0.0, 1.0);

        Point2f rerotationCenter = transformPoint(rotationCenter.x, rotationCenter.y, currentTransformMat);
        Mat currentRerotationMat = getRotationMatrix2D(rerotationCenter, -angle, 1.0);
        rerotationMat = (Mat_<double>(3, 3) <<
                currentRerotationMat.at<double>(0, 0), currentRerotationMat.at<double>(0, 1), currentRerotationMat.at<double>(0, 2),
                currentRerotationMat.at<double>(1, 0), currentRerotationMat.at<double>(1, 1), currentRerotationMat.at<double>(1, 2),
                0.0, 0.0, 1.0);
    }
    else
    {
        warpAffine(source, dst, currentRotationMat, source.size());
    }
    return dst;
}

double Dlib::estimateThreshold(StabMarker newMarkers, StabMarker refMarkers)
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

bool Dlib::checkFaceDataUpdate(const vector<Point>& points)
{
    if(stabThreshold == 0)
    {
        return true;
    }

    StabMarker newMarkers;
    int noseTopPointCount = 0;
    int counter = 0;
    for(auto iter = points.begin(); iter != points.end(); iter++, counter++)
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
#ifndef SS_ORIG
		//SS 05/14/2023
		
		stableRegionMarker.noseBottom = newMarkers.noseBottom;
		stableRegionMarker.noseLeft = newMarkers.noseLeft;
		stableRegionMarker.noseRight = newMarkers.noseRight;
		stableRegionMarker.noseTop = newMarkers.noseTop;
		
		/*
		stableRegionMarker.noseBottom.x = newMarkers.noseBottom.x;
		stableRegionMarker.noseLeft.x = newMarkers.noseLeft.x;
		stableRegionMarker.noseRight.x = newMarkers.noseRight.x;
		stableRegionMarker.noseTop.x = newMarkers.noseTop.x;
		stableRegionMarker.noseBottom.y = newMarkers.noseBottom.y;
		stableRegionMarker.noseLeft.y = newMarkers.noseLeft.y;
		stableRegionMarker.noseRight.y = newMarkers.noseRight.y;
		stableRegionMarker.noseTop.y = newMarkers.noseTop.y;
		*/
#endif
        return true;
    }
#ifndef SS_ORIG
	//SS 05/14/2023
	stabData.push_back(newMarkers);
	if (stabData.size() > stabFrames)
	{
		stabData.pop_front();
	}
#endif
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

	//SS 05/14/2023
#ifdef SS_ORIG
    stabData.push_back(newMarkers);
    if(stabData.size() > stabFrames)
    {
        stabData.pop_front();
    }
	if (estimateThreshold(newMarkers, avgMarkers) > stabThreshold)
	{
		return true;
	}
#else
	//SS 05/14/2023
	StabMarker copyStableMarker;
	copyStableMarker.noseBottom = stableRegionMarker.noseBottom;
	copyStableMarker.noseLeft = stableRegionMarker.noseLeft;
	copyStableMarker.noseRight = stableRegionMarker.noseRight;
	copyStableMarker.noseTop = stableRegionMarker.noseTop;

	if (estimateThreshold(copyStableMarker, avgMarkers) > stabThreshold)
    {
		//SS 05/14/2023
		
		stableRegionMarker.noseBottom = newMarkers.noseBottom;
		stableRegionMarker.noseLeft = newMarkers.noseLeft;
		stableRegionMarker.noseRight = newMarkers.noseRight;
		stableRegionMarker.noseTop = newMarkers.noseTop;
		/*
		stableRegionMarker.noseBottom.x = newMarkers.noseBottom.x;
		stableRegionMarker.noseLeft.x = newMarkers.noseLeft.x;
		stableRegionMarker.noseRight.x = newMarkers.noseRight.x;
		stableRegionMarker.noseTop.x = newMarkers.noseTop.x;
		stableRegionMarker.noseBottom.y = newMarkers.noseBottom.y;
		stableRegionMarker.noseLeft.y = newMarkers.noseLeft.y;
		stableRegionMarker.noseRight.y = newMarkers.noseRight.y;
		stableRegionMarker.noseTop.y = newMarkers.noseTop.y;
		*/
        return true;
    }
#endif //SS_STABLE
    return false;
}

Rect Dlib::getRegionRect(ivcp::RegionType regionType, const Rect& detectedFaceRect, const vector<Point>& points, bool faceDataUpdated, bool isWAImplemented)
{
    Rect bound = Rect();
    if (faceDataUpdated)
    {
        if (!useTemplateRegions)
        {
            switch(regionType)
            {
            case REGION_MOUTH:
                bound = locateMouth(detectedFaceRect, points);
                break;
            case REGION_NOSE:
                bound = locateNose(detectedFaceRect, points);
                break;
            case REGION_LEYE:
                bound = locateLEye(detectedFaceRect, points);
                break;
            case REGION_REYE:
                bound = locateREye(detectedFaceRect, points);
                break;
            case REGION_LCHEEK:
                bound = locateLCheek(detectedFaceRect, points);
                break;
            case REGION_RCHEEK:
                bound = locateRCheek(detectedFaceRect, points);
                break;
            case REGION_FACE:
                bound = locateFace(detectedFaceRect, points);
                break;
            case REGION_LEYEBROW:
                bound = locateLEyebrow(detectedFaceRect, points);
                break;
            case REGION_REYEBROW:
                bound = locateREyebrow(detectedFaceRect, points);
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
                bound = getRegionRect(regionType, detectedFaceRect, points, true, isWAImplemented);
            }
        }
        else
        {
            bound = getRegionRect(regionType, detectedFaceRect, points, true, isWAImplemented);
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

void Dlib::changeAngleWA()
{
    lock_guard<recursive_mutex> lkSettings(mutex);
    if (noFaceClockwiseRotation)
    {
        tryAngle += 2.0f;
    }
    else
    {
        tryAngle -= 2.0f;
    }
}

Rect Dlib::applyWorkarounds(Rect inputRect, RegionType regionType)
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

pair<Point, Point> Dlib::getEyeCenters()
{
    pair<Point, Point> eyes = pair<Point, Point>();
    Rect tmpEye = getRegionRect(REGION_LEYE, faceRect, landmarks, false, true);
    eyes.second = Point(tmpEye.x + (int)(tmpEye.width / 2), tmpEye.y + (int)(tmpEye.height / 2));

    tmpEye = getRegionRect(REGION_REYE, faceRect, landmarks, false, true);
    eyes.first = Point(tmpEye.x + (int)(tmpEye.width / 2), tmpEye.y + (int)(tmpEye.height / 2));

    return eyes;
}

pair<int, int> Dlib::getEyebrowRegionsWidth()
{
    pair<int, int> eyebrowsWidth = pair<int, int>();

    Rect tmpEyebrow = getRegionRect(REGION_LEYEBROW, faceRect, landmarks, false, true);
    eyebrowsWidth.first = tmpEyebrow.width;

    tmpEyebrow = getRegionRect(REGION_REYEBROW, faceRect, landmarks, false, true);
    eyebrowsWidth.second = tmpEyebrow.width;

    return eyebrowsWidth;
}

#if useExhaustiveSearch == 0
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
#endif

void Dlib::searcherThread(Dlib* pDlib)
{
    Mat image;
    pair<Point, Point> eyeCenters;
    pair<Point, Point> templateEyePos;
    map<int, Point2d> templateGlasses;
    int leftEyebrowWidth, rightEyebrowWidth;
    {
        lock_guard<recursive_mutex> lk(pDlib->mutex);
        //get eyebrow regions
        pair<int, int> eyebrowsWidth = pDlib->getEyebrowRegionsWidth();
        leftEyebrowWidth = eyebrowsWidth.first;
        rightEyebrowWidth = eyebrowsWidth.second;

        //copy data from main thread
        image = pDlib->searcherImage.clone();
        eyeCenters = pDlib->eyeCenters;
        templateEyePos = pDlib->templateEyePos;
        templateGlasses = pDlib->templateGlasses;

        //normalize image
        Mat trDraw(image.size(), image.type());
        warpPerspective(image, trDraw, pDlib->cachedTransformation, image.size(), 0);
        if (image.type() == CV_8UC3)
        {
            cvtColor(trDraw, image, cv::COLOR_BGR2GRAY);
        }
        else
        {
            pDlib->normalizeBrightness(trDraw, image);
        }
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
    Point2d f = Point2d(eyeCenters.first.x, eyeCenters.first.y);
    Point2d secondEye = Point2d(eyeCenters.second.x, eyeCenters.second.y);
    pair<Point2d, Point2d> glasses = pair<Point2d, Point2d>(f, secondEye);
    pair<Point2d, Point2d> rim = pair<Point2d, Point2d>(f, secondEye);

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
#if useExhaustiveSearch == 1
    Mat lensHypo = Mat::zeros(gradXY.size(), gradXY.type());
#endif
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

#if useExhaustiveSearch == 1
            for (int dy = MIN_G_DY; dy < MAX_G_DY; dy++)
            {
                for (int dx = MIN_G_DX; dx < MAX_G_DX; dx++)
                {
                    if (pDlib->searcherStatus == THREAD_EXITING)
                    {
                        lock_guard<recursive_mutex> lk(pDlib->mutex);
                        pDlib->isGlassesFound = false;
                        pDlib->searcherStatus = THREAD_FINALIZED;
                        pDlib->cvTimeToExit.notify_one();
                        return;
                    }
                    Mat tryTransf = rotateTransf.clone();
                    tryTransf.at<double>(0, 2) -= dx;
                    tryTransf.at<double>(1, 2) -= dy;

                    map<int, Point> tmpPoints;
                    for (auto iter = tryTemplateGlasses.begin(); iter != tryTemplateGlasses.end(); iter++)
                    {
                        tmpPoints[iter->first] = transformPoint(iter->second.x, iter->second.y, tryTransf);
                    }
                    Mat tryLensHypo = lensHypo.clone();
                    line(tryLensHypo, tmpPoints[1], tmpPoints[2], Scalar(255), 5);
                    line(tryLensHypo, tmpPoints[3], tmpPoints[4], Scalar(255), 5);
                    line(tryLensHypo, tmpPoints[5], tmpPoints[6], Scalar(255), 6);
                    line(tryLensHypo, tmpPoints[7], tmpPoints[8], Scalar(255), 4);
                    line(tryLensHypo, tmpPoints[9], tmpPoints[10], Scalar(255), 4);
                    int weight = (int)((cv::sum(gradXY & tryLensHypo)[0] / s));
                    if (weight > maxWeight)
                    {
                        maxWeight = weight;
                        glasses.first = tmpPoints[FIRST_GLASSES_POINT];
                        glasses.second = tmpPoints[SECOND_GLASSES_POINT];
                        rim.first = tmpPoints[FIRST_DOWN_RIM_POINT];
                        rim.second = tmpPoints[SECOND_DOWN_RIM_POINT];
                    }
                }
            }
#else
            if (pDlib->searcherStatus == THREAD_EXITING)
            {
                lock_guard<recursive_mutex> lk(pDlib->mutex);
                pDlib->searcherStatus = THREAD_FINALIZED;
                pDlib->cvTimeToExit.notify_one();
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
#endif
        }
    }

    //calculate ratio of length between center of eyes and glasses to eyebrows width
    double kGlassesLine = (double)(glasses.second.y - glasses.first.y) / (glasses.second.x - glasses.first.x);
    double bGlassesLine = glasses.first.y - kGlassesLine * glasses.first.x;
    pDlib->rightGlassRatio = (double)(eyeCenters.first.y - (kGlassesLine * eyeCenters.first.x + bGlassesLine)) / rightEyebrowWidth;
    pDlib->leftGlassRatio = (double)(eyeCenters.second.y - (kGlassesLine * eyeCenters.second.x + bGlassesLine)) / leftEyebrowWidth;

    //calculate ratio of length between center of eyes and rim to eyebrows width
    double kRimLine = (double)(rim.second.y - rim.first.y) / (rim.second.x - rim.first.x);
    double bRimLine = rim.first.y - kRimLine * rim.first.x;
    pDlib->rightRimRatio = (double)(eyeCenters.first.y - (kRimLine * eyeCenters.first.x + bRimLine)) / rightEyebrowWidth;
    pDlib->leftRimRatio = (double)(eyeCenters.second.y - (kRimLine * eyeCenters.second.x + bRimLine)) / leftEyebrowWidth;

    unique_lock<recursive_mutex> lk(pDlib->mutex);
    pDlib->isGlassesFound = true;
    pDlib->searcherStatus = THREAD_FINALIZED;
    pDlib->cvTimeToExit.notify_one();
}

void Dlib::closeSearcherThread()
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

Rect Dlib::autoGlassesSearch(Rect eyebrowRect)
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

Rect Dlib::autoGlassesDownRimSearch(Rect cheekRect)
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

void Dlib::calculateFaceBorder()
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

Rect Dlib::cutBackgroundFromRegions(Rect regionRect, RegionType type)
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

Mat Dlib::prepareImageForFaceSearch(Mat srcImage)
{
    Mat mat;
    if (srcImage.depth() == CV_16UC1 && srcImage.channels() == 1)
    {
        normalizeBrightness(srcImage, mat);
    }
    else
    {
        mat = srcImage.clone();
    }
    if (mat.type() == CV_8UC1)
    {
        cvtColor(mat, mat, cv::COLOR_GRAY2BGR);
    }
    resize(mat, mat, Size(), resizeDownscale, resizeDownscale);
    return mat;
}

}

#endif