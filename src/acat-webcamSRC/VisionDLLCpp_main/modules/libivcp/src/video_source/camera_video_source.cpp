///////////////////////////////////////////////////////////////////////////
// <copyright file="camera_video_source.cpp" company="Intel Corporation">
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



#include <sstream>
#include "libivcp/video_source/camera_video_source.hpp"

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#include <windows.h>
#include <dshow.h>

#pragma comment(lib, "strmiids")
#endif

using namespace std;
using namespace cv;

namespace ivcp
{

CameraVideoSource::CameraVideoSource():
    cameraIndex(0)
{
}

CameraVideoSource::~CameraVideoSource()
{
	//cout << "SSdeb releasing camera\n" << endl;
    release();
}

void CameraVideoSource::seek(int frame) 
{
    (void)frame;
}

void CameraVideoSource::openCamera(int index) 
{
    release();

    if (index >= 0)
    {
        setCameraIndex(index);
    }

    // MSMF (the OpenCV 5 default) is slow to open; DirectShow matches 2.4 behavior.
    cap.open(cameraIndex, cv::CAP_DSHOW);
    if (!cap.isOpened())
    {
        cap.open(cameraIndex);
    }

    if (!cap.isOpened())
    {
        stringstream ss; 
        ss << "Unable to open camera";
        IVCP_EXCEPTION(NoCameraException, ss.str());
    }
    else
    {
        if ((output_resolution.width > 0) && (output_resolution.height > 0))
        {
            if (!cap.set(cv::CAP_PROP_FRAME_WIDTH, output_resolution.width) || 
                !cap.set(cv::CAP_PROP_FRAME_HEIGHT, output_resolution.height))
            {
                IVCP_EXCEPTION(IncorrectParamException, "Camera does not support given resolution");
            }
        }
    }
	//SS read 1 frame to get teh camera ready
	Mat imageOriginal;
    cap >> imageOriginal;
	cap >> imageOriginal;
	cap >> imageOriginal;
	cap >> imageOriginal;
	cap >> imageOriginal;

}

Mat CameraVideoSource::getNextFrame() 
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

    Mat imageOriginal;
    cap >> imageOriginal;

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

Size CameraVideoSource::getOutputResolution()
{
    Size resolution(0,0);
    if ((output_resolution.width > 0) && (output_resolution.height > 0))
    {
        resolution = VideoSource::getOutputResolution();
    }
    else 
    {
        if (cap.isOpened()) 
        {
            resolution.width = (int) cap.get(cv::CAP_PROP_FRAME_WIDTH);
            resolution.height = (int) cap.get(cv::CAP_PROP_FRAME_HEIGHT);
        }
    }
    return resolution;
}

void CameraVideoSource::setOutputResolution(int width, int height)
{
    VideoSource::setOutputResolution(width, height);
    if (isOpen())
    {
        if (!cap.set(cv::CAP_PROP_FRAME_WIDTH, output_resolution.width) || 
            !cap.set(cv::CAP_PROP_FRAME_HEIGHT, output_resolution.height))
        {
            IVCP_EXCEPTION(IncorrectParamException, "Camera does not support given resolution");
        }
    }
}

VideoSourceType CameraVideoSource::getType()
{
    return CAMERA;
}

int CameraVideoSource::getCameraIndex()
{
    return cameraIndex;
}

void CameraVideoSource::setCameraIndex(int _cameraIndex)
{
    cameraIndex = _cameraIndex;
    release();
}

bool CameraVideoSource::isOpen()
{
    return cap.isOpened();
}

void CameraVideoSource::open()
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

void CameraVideoSource::release()
{
	//cout << "SSdeb In CameraVideoSource::release: before cap.isOpened" << endl;
    if (cap.isOpened())
    {
#ifndef ACATDLL //cap.release() hangs when accesses through DLL.
		//cout << "SSdeb In CameraVideoSource::release: cap.isOpened - before release " << GetCurrentThreadId() << endl;
		cap.release();	
		//cout << "SSdeb In CameraVideoSource::release: cap.isOpened - released" << endl;
#else
		ExitThread(GetCurrentThreadId());				
#endif //ACATDLL
    }
	//cout << "SSdeb In CameraVideoSource::release: outside cap.isOpened" << endl;
}

vector<pair<int, string>> CameraVideoSource::listDevices()
{
    vector<pair<int, string>> allDevices;
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32

    ICreateDevEnum *pDevEnum = NULL;
    IEnumMoniker *pEnum = NULL;
    int deviceCounter = 0;

	CoInitialize(nullptr); 
    HRESULT hr = CoCreateInstance(CLSID_SystemDeviceEnum, NULL,
        CLSCTX_INPROC_SERVER, IID_ICreateDevEnum,
        reinterpret_cast<void**>(&pDevEnum));

    if (SUCCEEDED(hr))
    {
        // Create an enumerator for the video capture category.
        hr = pDevEnum->CreateClassEnumerator( CLSID_VideoInputDeviceCategory,&pEnum, 0);
        if(hr == S_OK)
        {
            //printf("SETUP: Looking For Capture Devices\n");
            IMoniker *pMoniker = NULL;
            while (pEnum->Next(1, &pMoniker, NULL) == S_OK)
            {
                IPropertyBag *pPropBag;
                hr = pMoniker->BindToStorage(0, 0, IID_IPropertyBag, (void**)(&pPropBag));
                if (FAILED(hr))
                {
                    pMoniker->Release();
                    continue;  // Skip this one, maybe the next one will work.
                }
                // Find the description or friendly name.
                VARIANT varName;
                VariantInit(&varName);
                hr = pPropBag->Read(L"Description", &varName, 0);

                if (FAILED(hr))
                {
                    hr = pPropBag->Read(L"FriendlyName", &varName, 0);
                }

                if (SUCCEEDED(hr))
                {
                    hr = pPropBag->Read(L"FriendlyName", &varName, 0);

                    int count = 0;
                    int maxLen = 255;
                    char deviceName[255]={0};
                    while( varName.bstrVal[count] != 0x00 && count < maxLen)
                    {
                        deviceName[count] = (char)varName.bstrVal[count];
                        count++;
                    }
                    //printf("SETUP: %i) %s \n", deviceCounter, deviceName);
                    allDevices.push_back(pair<int, string>(deviceCounter, deviceName));
                }
                pPropBag->Release();
                pPropBag = NULL;

                pMoniker->Release();
                pMoniker = NULL;

                deviceCounter++;
            }
            pDevEnum->Release();
            pDevEnum = NULL;

            pEnum->Release();
            pEnum = NULL;
        }
        //printf("SETUP: %i Device(s) found\n\n", deviceCounter);
    }
	CoUninitialize();
#else
    IVCP_LOG_WARNING("Not implemented for Linux\n");
#endif
    return allDevices;
}


}
