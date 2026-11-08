///////////////////////////////////////////////////////////////////////////
// <copyright file="acat_gestures_dll.h" company="Intel Corporation">
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


#ifndef __ACAT_GESTURES_DLL_H_
#define __ACAT_GESTURES_DLL_H_

#ifdef ACAT_GESTURES_DLL_EXPORTS
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#define ACAT_GESTURES_EXPORT __declspec(dllexport)
#else
#define ACAT_GESTURES_EXPORT
#endif
#else
#define ACAT_GESTURES_EXPORT
#endif

typedef void (__stdcall * VisionEventCallback)(const char* text);

extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall init(void);
}

extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall quit(void);
}


extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall acatVision(void);
}

extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall SetVisionEventHandler(VisionEventCallback handler) ;
}

extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall visionCommand(char *str, double value) ;	
}


extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall showVideoWindow();
}

extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall hideVideoWindow();
}

extern "C"
{
	ACAT_GESTURES_EXPORT bool __stdcall isVideoWindowVisible();
}

extern "C"
{
	ACAT_GESTURES_EXPORT void __stdcall selectCamera(char *str) ;	
}

extern VisionEventCallback callbackHandler;

#endif //__ACAT_GESTURES_DLL_H_