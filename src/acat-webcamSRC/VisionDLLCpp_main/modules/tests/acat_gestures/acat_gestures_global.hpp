///////////////////////////////////////////////////////////////////////////
// <copyright file="acat_gestures_global.hpp" company="Intel Corporation">
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


#ifndef ED_GLOBAL_H // header guards
#define ED_GLOBAL_H

#include <time.h>
#include <iostream>
#include <fstream>
#include "ivcp.hpp"

#include <windows.h>

using namespace cv;
using namespace std;
using namespace ivcp;

//-------------------------
extern bool isNormBlurFeaActive;
extern bool bRunMultiGestureSMClassifier;

//-------------------------
//arguments set from the command-line parameters
extern bool c_useCamera; //true;
extern int c_videoFF;
extern int c_outVideoFF;

extern std::string videoFileName;
extern string outputFeafileName;
extern stringstream outputNormBlurFeafileName;
extern ofstream outputNormBlurFeafile;

extern string captureFilename;

//-------------------------
//pipeline arguments
extern SPtr<Pipeline> pipeline_single;
extern SPtr<Session> session;
extern SPtr<VideoPlayer> videoPlayer;
extern SPtr<VideoPlaylist> playlist;
extern SPtr<IVideoSource> source_avi8;
extern SPtr<VideofileVideoSource> source_avi24;
extern SPtr<RawfileVideoSource> source_raw16;
extern SPtr<NormalFlowBlurredFeature> normBlurFeature;;
extern SPtr<IGUIFramework> gui;
#ifdef USE_REALSENSE
extern SPtr<RealSenseVideoSource> mycamera;
#else
extern SPtr<CameraVideoSource> mycamera;
#endif

#ifdef USE_REALSENSE
extern SPtr<IRealSense> rsHandler;
extern SPtr<RealSenseRegionCreator> regionREyebrow;
extern SPtr<RealSenseRegionCreator> regionLEyebrow;
extern SPtr<RealSenseRegionCreator> regionRCheek;
extern SPtr<RealSenseRegionCreator> regionLCheek;
extern SPtr<RealSenseRegionCreator> regionMouth;
extern SPtr<RealSenseRegionCreator> regionNose;
#elif USE_DLIB
extern SPtr<IDlib> dlibHandler;
extern SPtr<DlibRegionCreator> regionREyebrow;
extern SPtr<DlibRegionCreator> regionLEyebrow;
extern SPtr<DlibRegionCreator> regionRCheek;
extern SPtr<DlibRegionCreator> regionLCheek;
extern SPtr<DlibRegionCreator> regionMouth;
extern SPtr<DlibRegionCreator> regionNose;
#endif

//-------------------------
//fps computation
extern std::deque<int> fpses;
extern int64 prevFrameTickTime;
extern int fps;

//-------------------------
//keeping track of missedframes due to face not getting detected
extern int framenumber;
extern int framenumber_lastfaceNotFound;
extern bool c_FeaturesOut;
extern mutex m_fea;

extern int normBlurFeatureFramenumber;
extern int normBlurFeatureFramenumber_cur;
extern int normBlurFeatureFilenumber;
//-------------------------

extern Rect prevRegion;

//------------------------

extern bool c_bIsBeep;
extern bool bIsStartRecord;
extern float STD_FOR_NO_REGIONS;
extern float thresh; //default detection threshold
extern float thresh_mg;
extern float c_noiseThreshold; 
//-------------------------

extern bool c_cutBG; 
extern float c_stabThreshold; 


extern bool c_videoOut;
extern string c_outVideoFileName;
extern int c_outMaxFramesPerFile;


extern int c_videoOutresolutionWidth;
extern int c_videoOutresolutionHeight;

extern int c_asterSyncFrames;
extern int c_acatPort;

#endif //ED_GLOBAL_H