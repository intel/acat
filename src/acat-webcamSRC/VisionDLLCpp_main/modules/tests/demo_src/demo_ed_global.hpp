// Copyright (C) 2013, Intel Corporation, all rights reserved.

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
extern bool isStdFeaActive;
extern bool isNormBlurFeaActive;


//-------------------------
extern bool c_bRunEDClassifier;
extern bool c_bRunEDClassifier_NormBlur;
extern bool bRunEDClassifier_ER_MP;
extern bool bRunHMMClassifier;
extern bool bRunMultiGestureSMClassifier;

//-------------------------
//arguments set from the command-line parameters
extern bool c_useCamera; //true;
extern int c_videoFF;
extern int c_outVideoFF;

extern std::string videoFileName;
extern string outputFeafileName;
extern stringstream outputEDFeafileName;
extern ofstream outputEDFeafile;
extern stringstream outputNormBlurFeafileName;
extern ofstream outputNormBlurFeafile;
extern stringstream outputEDResfileName;
extern ofstream outputEDResfile;

extern int c_AeMaxLimit;
extern string captureFilename;

//-------------------------
//pipeline arguments
extern SPtr<ParallelPipeline> pipeline_parallel;
extern SPtr<Pipeline> pipeline_single;
extern SPtr<Session> session;
extern SPtr<VideoPlayer> videoPlayer;
extern SPtr<VideoPlaylist> playlist;
extern SPtr<IVideoSource> source_avi8;
extern SPtr<VideofileVideoSource> source_avi24;
extern SPtr<RawfileVideoSource> source_raw16;
extern SPtr<MotionEstimationStdDevFeature> motionStdDevFeature;
extern SPtr<NormalFlowBlurredFeature> normBlurFeature;;
extern SPtr<IGUIFramework> gui;
#ifdef USE_XIMEA
extern SPtr<XimeaVideoSource> mycamera;
#elif USE_REALSENSE
extern SPtr<RealSenseVideoSource> mycamera;
#else
extern SPtr<CameraVideoSource> mycamera;
#endif

#ifdef USE_FACEAPI
extern SPtr<IFaceAPI> faceAPIHandler;
extern SPtr<IRegionCreator> regionREyebrow;
extern SPtr<IRegionCreator> regionLEyebrow;
extern SPtr<IRegionCreator> regionRCheek;
extern SPtr<IRegionCreator> regionLCheek;
extern SPtr<IRegionCreator> regionMouth;
extern SPtr<IRegionCreator> regionNose;
#elif USE_REALSENSE
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

#else
extern SPtr<IASM> asmHandler ;
extern SPtr<ASMRegionCreator> regionREyebrow;
extern SPtr<ASMRegionCreator> regionLEyebrow;
extern SPtr<ASMRegionCreator> regionRCheek;
extern SPtr<ASMRegionCreator> regionLCheek;
extern SPtr<ASMRegionCreator> regionMouth;
extern SPtr<ASMRegionCreator> regionNose;
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
extern int edFeatureFramenumber;
extern int edFeatureFramenumber_cur;
extern int edFeatureFilenumber;
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
extern float thresh_normBlur;
extern float thresh_er_mp;
extern float thresh_mg;
extern float c_noiseThreshold; 
//-------------------------
extern bool prevAdjustStatus;
extern double prevManualExposureSetting;
//-------------------------

extern bool c_cutBG; //ASM tracker default setting
extern float c_stabThreshold; //ASM tracker setting

#ifdef USE_ASM
extern int c_minFace; //ASM tracker default setting
extern bool c_useGeneralTracker; //ASM tracker default setting
#endif

extern float c_fileIdx;

extern bool c_videoOut;
extern string c_outVideoFileName;
extern int c_outMaxFramesPerFile;
extern int c_gestureLen_max;
extern int c_gestureLen_min;

extern int c_videoOutresolutionWidth;
extern int c_videoOutresolutionHeight;

extern int c_asterSyncFrames;

extern bool isRegionMoved;

#endif //ED_GLOBAL_H