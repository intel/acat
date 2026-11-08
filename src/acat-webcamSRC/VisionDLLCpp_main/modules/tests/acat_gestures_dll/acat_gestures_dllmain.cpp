///////////////////////////////////////////////////////////////////////////
// <copyright file="acat_gestures_main.cpp" company="Intel Corporation">
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


#include <time.h>
#include <iostream>
#include <fstream>
#include "ivcp.hpp"
#include <windows.h>

using namespace cv;
using namespace std;
using namespace ivcp;

#include "acat_gestures_dll.h"
#include "acat_gestures_global.hpp"
#include "acat_gestures_common.hpp"

static char selectedCamera[256];

//============================================================================

void __stdcall SetVisionEventHandler(VisionEventCallback handler) 
{
  callbackHandler = handler;
}

//============================================================================
void __stdcall visionCommand(char *str, double value)
{
    sendCommandToGUI(str, value);
}


//============================================================================
void __stdcall showVideoWindow()
{
	showVideo();
}

//============================================================================
void __stdcall hideVideoWindow()
{
	hideVideo();
}

//============================================================================
bool __stdcall isVideoWindowVisible()
{
	return isVideoVisible();
}

//============================================================================
void __stdcall selectCamera(char *camera)
{
	strncpy_s(selectedCamera, camera, _TRUNCATE);
}

void __stdcall init()
{
	selectedCamera[0] = 0;
}

void __stdcall quit()
{
	//cout << "DLL quit()" << std::endl;
	visionCommand("action=EXITAPP", 0);
}

//============================================================================

void __stdcall acatVision(void)
{
	try
	{
	isNormBlurFeaActive = true;
	bRunMultiGestureSMClassifier = true;
	processCommandLineArgs(0, NULL);

	Log::setVerbosityLevel(LOG_ERROR);
	
	// Initialize pipeline
	ivcpInit();

	session = Factory::createSession("session");
	
	if (setupCameraSource(selectedCamera)== false)
	{
		if (callbackHandler != nullptr)
			callbackHandler("gesture=EXCEPTION;reason=No camera found;");
		return;
	}

	setupFaceRegionTracker();

	
	//setupFeatureComputation_ED();
	setupFeatureComputation_NormBlur();

	
	setupClassifier_MG();
	
	setupSinglePipeline();

	setupVideoRecording();
	
	pipeline_single->run();
	stopVideoRecording();

	if (c_FeaturesOut == true)
	{
		outputNormBlurFeafile.close();
	}
	}
	catch (const std::exception& e)
	{
		IVCP_LOG_ERROR("Vision pipeline exception: %s", e.what());
		if (callbackHandler != nullptr)
			callbackHandler("gesture=EXCEPTION;reason=Vision pipeline failed;");
	}
	catch (...)
	{
		IVCP_LOG_ERROR("Unknown vision pipeline exception");
		if (callbackHandler != nullptr)
			callbackHandler("gesture=EXCEPTION;reason=Unknown vision pipeline failure;");
	}
}

//============================================================================
HINSTANCE hInstance;
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
						   LPVOID
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		//cout << "DLL_PROCESS_ATTACH " << GetCurrentProcessId() << std::endl;
		break;
	case DLL_THREAD_ATTACH:
		//cout << "DLL_THREAD_ATTACH " << GetCurrentThreadId() << std::endl;
		break;
	case DLL_THREAD_DETACH:
		//cout << "DLL_THREAD_DETACH " << GetCurrentThreadId() << endl;
		break;
	case DLL_PROCESS_DETACH:
		//cout << "DLL_PROCESS_DETACH " << GetCurrentProcessId() << endl;
		//if (lpReserved != NULL)
		{
			//cout << "process exiting: set worker thread status to Finalized\n";
			//gui->setWorkerThreadStatusToFinished();
		}
		//std::this_thread::sleep_for(std::chrono::seconds(1000)); //SSdebug
		hInstance = hModule;
		break;
	}
	return TRUE;
}
