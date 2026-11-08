///////////////////////////////////////////////////////////////////////////
// <copyright file="realsense_video_source.cpp" company="Intel Corporation">
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


#ifdef USE_REALSENSE

#include <sstream>
#include "libivcp/video_source/realsense_video_source.hpp"
#include "libivcp/factory.hpp"

using namespace std;
using namespace cv;

#define DEFAULT_WIDTH  1920
#define DEFAULT_HEIGHT 1080

namespace ivcp
{

RealSenseVideoSource::RealSenseVideoSource():
    cameraIndex(0), isInitialized(false), is3dTracking(true)
{
    output_resolution = Size(DEFAULT_WIDTH, DEFAULT_HEIGHT);
}

RealSenseVideoSource::~RealSenseVideoSource()
{
    release();
}

void RealSenseVideoSource::seek(int frame)
{
    (void)frame;
}

void RealSenseVideoSource::openCamera(int index) 
{
    release();

    if (index >= 0)
    {
        setCameraIndex(index);
    }

    psm = PXCSenseManager::CreateInstance();
    if (!psm)
    {
        IVCP_EXCEPTION(NoCameraException, "Unable to create the PXCSenseManager\n");
    }

    captureMgr = psm->QueryCaptureManager();
    captureMgr->FilterByDeviceInfo(L"RealSense", NULL, index);

    status = psm->EnableStream(PXCCapture::STREAM_TYPE_COLOR, output_resolution.width, output_resolution.height);
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IOException, "Unable to enable PXCSenseManager color stream\n");
    }

    if (is3dTracking)
    {
        status = psm->EnableStream(PXCCapture::STREAM_TYPE_DEPTH, output_resolution.width, output_resolution.height);
        if (status < PXC_STATUS_NO_ERROR)
        {
            IVCP_EXCEPTION(IOException, "Unable to enable PXCSenseManager depth stream\n");
        }
    }

    status = psm->EnableFace();
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IOException, "Unable to enable face analysis\n");
    }

    faceAnalyzer = psm->QueryFace();
    if (!faceAnalyzer)
    {
        IVCP_EXCEPTION(IOException, "Unable to retrieve face results\n");
    }

    status = psm->Init();
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IOException, "Unable to init the PXCSenseManager\n");
    }

    fdata = faceAnalyzer->CreateOutput();
    if (!fdata)
    {
        IVCP_EXCEPTION(IOException, "Unable to create output for face results\n");
    }

    config = faceAnalyzer->CreateActiveConfiguration();
    if (is3dTracking)
    {
        config->SetTrackingMode(PXCFaceConfiguration::TrackingModeType::FACE_MODE_COLOR_PLUS_DEPTH);
    }
    else
    {
        config->SetTrackingMode(PXCFaceConfiguration::TrackingModeType::FACE_MODE_COLOR);
    }
    config->detection.isEnabled = true;
    config->detection.maxTrackedFaces = 1;
    status = config->ApplyChanges();
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IOException, "Unable to apply settings for face detection\n");
    }

    isInitialized = true;
}

Mat RealSenseVideoSource::getNextFrame()
{
    if (!isOpen())
    {
        try
        {
            openCamera();
        }
        catch(IncorrectParamException)
        {
            return Mat();
        }
        catch(NoCameraException)
        {
            return Mat();
        }
    }

    resetAction();

    status = psm->AcquireFrame(true);
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Can't get next frame\n");
    }
    status = fdata->Update();
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Can't update data to latest available output\n");
    }

    Mat imageOriginal = getFrameMat();
    getAllFaceData();

    psm->ReleaseFrame();

    Mat imageConverted;
    try
    {
        convertColorFormat(imageOriginal, imageConverted);
    }
    catch (IncorrectParamException)
    {
        return Mat();
    }

    return imageConverted;
}

Size RealSenseVideoSource::getOutputResolution()
{
    return VideoSource::getOutputResolution();
}

void RealSenseVideoSource::setOutputResolution(int width, int height)
{
    if (!isInitialized)
    {
        output_resolution = Size(width, height);
    }
    else
    {
        IVCP_EXCEPTION(IncorrectParamException, "RealSense camera can not change resolution after it is opened\n");
    }
}

VideoSourceType RealSenseVideoSource::getType()
{
    return RS_CAMERA;
}

int RealSenseVideoSource::getCameraIndex()
{
    return cameraIndex;
}

void RealSenseVideoSource::setCameraIndex(int _cameraIndex)
{
    cameraIndex = _cameraIndex;
    release();
}

bool RealSenseVideoSource::isOpen()
{
    return isInitialized;
}

void RealSenseVideoSource::open()
{
    try
    {
        openCamera();
    }
    catch(IncorrectParamException &ex)
    {
        IVCP_EXCEPTION(IOException, ex.getMessage());
    }
    catch(NoCameraException &ex)
    {
        IVCP_EXCEPTION(IOException, ex.getMessage());
    }
}

void RealSenseVideoSource::release()
{
    if (isInitialized)
    {
        isInitialized = false;
        fdata->Release();
        config->Release();
        psm->Release();
    }
}

Mat RealSenseVideoSource::getFrameMat()
{
    PXCCapture::Sample *sample = psm->QuerySample();
    PXCImage *color_image = sample->color;
    PXCImage::ImageData data;
    status = color_image->AcquireAccess(PXCImage::ACCESS_READ_WRITE, PXCImage::PIXEL_FORMAT_RGB24, &data);
    if (status < PXC_STATUS_NO_ERROR)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Can't lock to access the internal storage\n");
    }

    Mat frame(output_resolution.height, output_resolution.width, CV_8UC3);
    unsigned char *bgr_data = data.planes[0];
    for(int i = 0; i < output_resolution.height; ++i)
    {
        unsigned char* p = frame.ptr<unsigned char>(i);
        memcpy(p, bgr_data + (output_resolution.width * i * 3), sizeof(unsigned char) * 3 * output_resolution.width);
    }
    color_image->ReleaseAccess(&data);

    return frame;
}

void RealSenseVideoSource::getAllFaceData()
{
    if (Factory::isRealSenseHandlerCreated())
    {
        Rect faceRect = Rect();
        vector<Point> faceLandmarks = vector<Point>();

        pxcI32 nfaces = fdata->QueryNumberOfDetectedFaces();
        for (pxcI32 i = 0; i < nfaces; i++)
        {
            PXCFaceData::Face *trackedFace = fdata->QueryFaceByIndex(i);
            if (trackedFace != NULL)
            {
                PXCFaceData::LandmarksData *landmarkData = trackedFace->QueryLandmarks();
                if(landmarkData != NULL)
                {
                    pxcI32 numPoints = landmarkData->QueryNumPoints();
                    PXCFaceData::LandmarkPoint currentPoint;
                    for (pxcI32 j = 0; j < numPoints; j++)
                    {
                        landmarkData->QueryPoint(j, &currentPoint);
                        faceLandmarks.push_back(Point((int)currentPoint.image.x, (int)currentPoint.image.y));
                    }
                }

                PXCFaceData::DetectionData *ddata = trackedFace->QueryDetection();
                if (ddata != NULL)
                {
                    PXCRectI32 rect;
                    ddata->QueryBoundingRect(&rect);
                    faceRect = Rect(rect.x, rect.y, rect.w, rect.h);
                }
            }
        }

        SPtr<IRealSense> rsHandler = Factory::getCachedRealSense();
        rsHandler->setFaceTrackedData(faceRect, faceLandmarks);
    }
}

void RealSenseVideoSource::resetAction()
{
    if (Factory::isRealSenseHandlerCreated())
    {
        SPtr<IRealSense> rsHandler = Factory::getCachedRealSense();
        if (rsHandler->isReseted())
        {
            config = faceAnalyzer->CreateActiveConfiguration();
            config->detection.isEnabled = false;
            config->ApplyChanges();

            config = faceAnalyzer->CreateActiveConfiguration();
            config->SetTrackingMode(PXCFaceConfiguration::TrackingModeType::FACE_MODE_COLOR_PLUS_DEPTH);
            config->detection.isEnabled = true;
            config->detection.maxTrackedFaces = 1;
            config->ApplyChanges();

            rsHandler->resetDone();
        }
    }
}

void RealSenseVideoSource::set3dTracking(bool _is3dTracking)
{
    is3dTracking = _is3dTracking;
}

bool RealSenseVideoSource::get3dTracking()
{
    return is3dTracking;
}

vector<pair<int, string>> RealSenseVideoSource::listDevices()
{
    vector<pair<int, string>> allDevices;
    PXCSession* session = PXCSession_Create();
    if (session == NULL)
    {
        IVCP_EXCEPTION(IncorrectParamException, "Failed to create an SDK session to get all cameras");
    }
    PXCSession::ImplDesc desc;
    memset(&desc, 0, sizeof(desc));
    desc.group = PXCSession::IMPL_GROUP_SENSOR;
    desc.subgroup = PXCSession::IMPL_SUBGROUP_VIDEO_CAPTURE;
    for (int i = 0; ; ++i)
    {
        PXCSession::ImplDesc desc1;
        if (session->QueryImpl(&desc, i, &desc1) < PXC_STATUS_NO_ERROR)
        {
            break;
        }

        PXCCapture *capture;
        if (session->CreateImpl<PXCCapture>(&desc1, &capture) < PXC_STATUS_NO_ERROR)
        {
            continue;
        }

        for (int j = 0; ; ++j)
        {
            PXCCapture::DeviceInfo deviceInfo;
            if (capture->QueryDeviceInfo(j, &deviceInfo) < PXC_STATUS_NO_ERROR)
            {
                break;
            }
            wstring ws(deviceInfo.name);
            string deviceName(ws.begin(), ws.end());
            allDevices.push_back(pair<int, string>(deviceInfo.didx, deviceName));
            cout << "device name: " << deviceName << ", didx: " << deviceInfo.didx << endl;
        }
        capture->Release();
    }
    return allDevices;
}

}

#endif
