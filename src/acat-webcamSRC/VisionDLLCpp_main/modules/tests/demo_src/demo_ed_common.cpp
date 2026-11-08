// Copyright (C) 2013, Intel Corporation, all rights reserved.

#include <time.h>
#include <iostream>
#include <fstream>
#include "ivcp.hpp"

#include <windows.h>

//#include <stdio.h> 
#include <sys/stat.h> 
#include <sys/types.h> 
#include <direct.h>


using namespace cv;
using namespace std;
using namespace ivcp;

#include "demo_ed_common.hpp"

string strGUIWindowTitle = "ACAT vision: v0.18.31 r2";
//-------------------------
#define NSEQUENCES 6 // # of regions

bool isStdFeaActive=false;
bool isNormBlurFeaActive=false; // by default it is not active

//-------------------------
bool c_bRunEDClassifier = false;
bool c_bRunEDClassifier_NormBlur = false;
bool bRunEDClassifier_ER_MP = false;
bool bRunHMMClassifier = false;
bool bRunMultiGestureSMClassifier = false;

//-------------------------
//arguments set from the command-line parameters
bool c_useCamera=false; //default: true;
//enum {NONE=-1, RAW_16=0, AVI_24=1, AVI_8=2};
int c_videoFF=NONE;
int c_outVideoFF=AVI_8;
std::string videoFileName;
string outputFeafileName;
stringstream outputEDFeafileName;
ofstream outputEDFeafile;
stringstream outputNormBlurFeafileName;
ofstream outputNormBlurFeafile;
stringstream outputEDResfileName;
ofstream outputEDResfile;
stringstream outputHMMResfileName;
ofstream outputHMMResfile;
int c_AeMaxLimit = 16000;
string captureFilename = "queueVideoRec.avi";

//-------------------------
//pipeline arguments
SPtr<ParallelPipeline> pipeline_parallel;
SPtr<Pipeline> pipeline_single;
SPtr<Session> session;
SPtr<VideoPlayer> videoPlayer;
SPtr<VideoPlaylist> playlist;
SPtr<IVideoSource> source_avi8;
SPtr<VideofileVideoSource> source_avi24;
SPtr<RawfileVideoSource> source_raw16;
SPtr<MotionEstimationStdDevFeature> motionStdDevFeature;
SPtr<NormalFlowBlurredFeature> normBlurFeature;
SPtr<IGUIFramework> gui;
#ifdef USE_XIMEA
SPtr<XimeaVideoSource> mycamera;
#elif USE_REALSENSE
SPtr<RealSenseVideoSource> mycamera;
#else
SPtr<CameraVideoSource> mycamera;
#endif

#ifdef USE_FACEAPI
SPtr<IFaceAPI> faceAPIHandler;
SPtr<IRegionCreator> regionREyebrow;
SPtr<IRegionCreator> regionLEyebrow;
SPtr<IRegionCreator> regionRCheek;
SPtr<IRegionCreator> regionLCheek;
SPtr<IRegionCreator> regionMouth;
SPtr<IRegionCreator> regionNose;
#elif USE_REALSENSE
SPtr<IRealSense> rsHandler;
SPtr<RealSenseRegionCreator> regionREyebrow;
SPtr<RealSenseRegionCreator> regionLEyebrow;
SPtr<RealSenseRegionCreator> regionRCheek;
SPtr<RealSenseRegionCreator> regionLCheek;
SPtr<RealSenseRegionCreator> regionMouth;
SPtr<RealSenseRegionCreator> regionNose;
#elif USE_DLIB
SPtr<IDlib> dlibHandler;
SPtr<DlibRegionCreator> regionREyebrow;
SPtr<DlibRegionCreator> regionLEyebrow;
SPtr<DlibRegionCreator> regionRCheek;
SPtr<DlibRegionCreator> regionLCheek;
SPtr<DlibRegionCreator> regionMouth;
SPtr<DlibRegionCreator> regionNose;

#else
SPtr<IASM> asmHandler;
SPtr<ASMRegionCreator> regionREyebrow;
SPtr<ASMRegionCreator> regionLEyebrow;
SPtr<ASMRegionCreator> regionRCheek;
SPtr<ASMRegionCreator> regionLCheek;
SPtr<ASMRegionCreator> regionMouth;
SPtr<ASMRegionCreator> regionNose;
#endif

//-------------------------
//fps computation
std::deque<int> fpses;
int64 prevFrameTickTime=-1;
int fps;

//-------------------------
//keeping track of missedframes due to face not getting detected
int framenumber = 0;
int framenumber_lastfaceNotFound = 0;
int edFeatureFramenumber = 0;
int edFeatureFramenumber_cur = 0;
int edFeatureFilenumber = 0;
bool c_FeaturesOut = false;
mutex m_fea;

int normBlurFeatureFramenumber = 0;
int normBlurFeatureFramenumber_cur = 0;
int normBlurFeatureFilenumber = 0;
int regionFramenumber = 0;

//-------------------------

Rect prevRegion;

//------------------------
bool c_bIsBeep = false;
bool bIsStartRecord=false;
float STD_FOR_NO_REGIONS = -1.0F;
float thresh = 25.0F; //default detection threshold
float thresh_normBlur = 0.2F;
float thresh_er_mp = 0.2F;
float thresh_mg = 0.2F;

//-------------------------
bool prevAdjustStatus = false;
double prevManualExposureSetting = 0.0;
//-------------------------

bool c_cutBG = true; //ASM tracker default setting
//int c_minFace = 500; //60; //500; //ASM tracker default setting
float c_stabThreshold = 40.0F; //30.0f; //ASM tracker setting
float c_noiseThreshold = 10.0F;

#ifdef USE_ASM
bool c_useGeneralTracker=true; //ASM tracker default setting
bool c_useHawkingTracker=false;
#ifdef USE_XIMEA
	int c_minFace = 500;
#else
	int c_minFace = 175;
#endif
#endif //USE_ASM

float c_fileIdx = 0;
int c_regionOffsetY_eye = 0;
int c_skipFrames = 0;
ASTER_ACTION c_action = ASTER_ACTION::ACTION_TRIGGER;
bool c_videoOut = false;
int c_outMaxFramesPerFile = 0;
string c_outVideoFileName = "fileVidRec.avi";
int c_videoOutresolutionWidth = 1280;
int c_videoOutresolutionHeight = 1024;
string c_outDir = "C:\\Data\\";


//int c_gestureLen_max=10;
//int c_gestureLen_min=10;
int c_gestureLenHMM_er=15;
int c_gestureLenHMM_mp=10;


int c_gestureLenMsec_min=150;
int c_gestureLenMsec_max=500;
int c_gestureSpacingMsec=0; //sec

int c_er_gestureLenMsec_max=1000;
int c_er_gestureLenMsec_min=250;
int c_mp_gestureLenMsec_max=1000;
int c_mp_gestureLenMsec_min=250;

std::string c_hmm_path = "res\\hmm";
int c_hmm_bufferLen = 30;

int c_asterSyncFrames = 5000;

bool isRegionMoved = false;

int prevHMMGesture = -1;

SPtr<CheekTwitchDetectionClassifier> classifier;
SPtr<CheekTwitchDetectionClassifier_NormBlur> classifierED_NormBlur;
SPtr<EyebrowraiseMouthpullDetectionClassifier> classifierED_ER_MP;
#ifdef USE_HMM
SPtr<HMMClassifier> classifierHMM;
#endif
SPtr<MultiGestureSMClassifier> classifierMG;

//#ifdef USE_DLIB
double c_dlibResizeFactor = 0.5; 
//#endif
//============================================================================
//bool fileFolderExists(const std::string& inFilename) {
//  ifstream inFile(inFilename.c_str());
//  return inFile;
//}

bool fileFolderExists(const std::string& inName, int file_or_dir)
{
	struct stat statusBuffer;
	if (file_or_dir == 0)
	{
		if ( (stat(inName.c_str(), &statusBuffer) == 0) && ((statusBuffer.st_mode & S_IFMT) == S_IFREG))
			return true;
	}
	else
	{
		int deb1 = stat(inName.c_str(), &statusBuffer);
		int deb2 = statusBuffer.st_mode & S_IFMT;
		if ( (stat(inName.c_str(), &statusBuffer) == 0) && ((statusBuffer.st_mode & S_IFMT) == S_IFDIR))
			return true;
	}
    
    return false;
}

//============================================================================

void processCommandLineArgs(int argc, char **argv)
{
	//if (argc == 1)
	if ((argc > 1) && (strcmp(argv[1], "--help") == 0))
	{
		printf("Need arguments\n");
		
		printf("\nAvailable options are:\n");
		printf("\t--------------------------------------\n");
		printf("\n\t Video Input Options\n");
		printf("\t\t  --usecamera [default] : for camera input\n\n");
		printf("\t\tFor file input use one of:\n");
		printf("\t\t --raw16 filename:  for 16-bit raw format (data collected using Intel tool\n");
		printf("\t\t --avi24 filename: for 24-bit AVI format (data collected using ItSeez pipeline)\n");
		printf("\t\t --avi8 filename:  for 8-bit avi format\n");

		printf("\t--------------------------------------\n");
		printf("\n\tOptions for capturing video and feature files:\n");
		printf("\t --outFeature: turns on capturing features to file\n");
		printf("\t --outVideo format[avi8|raw16|avi24]: turns on video capture in the specified format\n");
		printf("\t --outVideoRes W H :default  W=1280, H=1024\n");
		printf("\t --outDir foldername: provide foldername for storing output data\n");
		printf("\t --outFramesPerFile maxFramesPerFile: data will be recorded in series of files each containing maxFramesPerFile frames\n");
		printf("\t --startRecord : start recording right from the start\n");
		
		

		printf("\t--------------------------------------\n");
		printf("\n\tClassification options are:\n");
		printf("\n\t\t Early detect cheek-twitch gesture Options\n");
		printf("\t\t --ed : to enable early detect classification\n");
		printf("\t\t --ednorm : to enable early detect classification with norm blur features\n");		
		printf("\t\t --gestureLenMsec int int [default: 150 500] : minimum maximum cheek-twitch gesture len in MSec\n");
		printf("\t\t --gestureSpacingMsec int [default: 0] : ??\n"); 
		printf("\t\t--------------------------------------\n");
		printf("\n\t\t State machine based ER and MP gesture Options\n");
		printf("\t\t --ERgestureLenMsec int int [default: 150 1000]: min max length of ER gesture in msec\n");
		printf("\t\t --MPgestureLenMsec int int [default: 150 1000]: min max length of MP gesture in msec\n");		
		printf("\t\t--------------------------------------\n");
		printf("\n\t\t HMM based ER and MP gesture Options\n");
		printf("\t --hmm foldername: provide_path_to_the_folder containing the hmm am/lm models\n");
		printf("\t --hmmBufferLen  int: number of frames to use for classification: default 30\n");
		printf("\t --gestureLenHMM int int [default: ER:15 MP:10]: number of consecutive frames of recognition before trigger\n");
		
		printf("\t--------------------------------------\n");
		printf("\t --aster LOG|TRIGGER : how to actuate aster: deafult:TRIGGER\n");
		printf("\t --beep : turns on the beep for cheeck twitch detection]\n");
		printf("\t --asterSyncFrames int [for aster sync frame number recording: default [5000]\n");

		printf("\t--------------------------------------\n");
		printf("\n\tOptions mainly for debugging are:\n");
		printf("\t --aelimit integer [default: 16000]: auto exposure max limit\n");		
		printf("\t --cutbg [default: disabled] : to enable cut background for ASM tracker\n");
		printf("\t --minFace integer [default: 500] : to enable cut background for ASM tracker\n");
		printf("\t --userGeneral [default] : to enable tracking of general user face for ASM tracker\n");
		printf("\t --userHawking [default: userGaneral] : to enable tracking of Hawking user face for ASM tracker\n");		
		printf("\t --stabThreshold float [default: 30] : to enable cut background for ASM tracker\n");
		printf("\t --fileIdx int [default: 0] : to enable offline processing of ed results\n");
		printf("\t --eyeYoffset int [default: 0] : left-right eye region y-offset\n");
		printf("\t --skip int [default: 0] : skip every nth input frame to simulate lower fps, use only with file input\n");				
		printf("\t --noiseThreshold float [default: 10.0 for --ed OR 0.3 for --ednorm] : for calibrating noise floor\n");
			
		printf("\t--------------------------------------\n");
		printf("\n\tOptions for tracker\n");
		printf("\t --resizeFactor double [default: 1.0]: factor to resize image for dlib face detector\n");		

		exit(1);
	}

	int i=1;
	bool isNoiseThreshold=false;
	bool isGestureLenHMM=false;
	bool isGestureLenMsec=false;

	bool isCommandErr = false;
	std::stringstream strCommandErr;
	strCommandErr.str("");
	strCommandErr.clear();

	try {
		while (i < argc)
		{
			//Video input arguments
			if (strcmp(argv[i], "--usecamera") == 0)
			{
				if (c_videoFF == NONE)
				{
					cout << "Using camera\n";
					c_useCamera = true;
					i++;
					continue;
				}
				else
				{
					isCommandErr = true;
					strCommandErr << "Command-line argument ERROR: cannot use camera input and file input. Please specify just one\n";
					break;
				}
			}
			if ( (strcmp(argv[i], "--raw16") == 0)  ||  (strcmp(argv[i], "--avi24") == 0) || (strcmp(argv[i], "--avi8") == 0))
			{
				if (c_useCamera == true)
				{
					c_useCamera = false;
					cout << "**File input detected... Camera input has been disabled\n";

				}
				//if ((c_useCamera == false) && (c_videoFF == NONE))
				if ((c_videoFF == NONE))
				{
					if (strcmp(argv[i], "--raw16") == 0)
					{
						cout << "Using Raw 16 file format\n";
						c_videoFF = RAW_16;
					} 
					else if (strcmp(argv[i], "--avi24") == 0)
					{
						cout << "Using AVI 24 file format\n";
						c_videoFF = AVI_24;
					}
					else if (strcmp(argv[i], "--avi8") == 0)
					{
						cout << "Using AVI 8 file format\n";
						c_videoFF = AVI_8;
					}
					i++;
					videoFileName = string(argv[i]); i++;

					if (fileFolderExists(videoFileName,0) == false)
					{
						isCommandErr = true;
						strCommandErr << "Command-line argument ERROR: " << videoFileName.c_str() << "  does not exist\n";
						break;
					}
					i++;
					continue;
				}
				else
				{
					isCommandErr = true;
					strCommandErr << "Command-line argument ERROR: cannot use multiple file inputs. Please specify just one\n";
					break;
				}
			}

			//----------------------------------------------------------
			// Output arguments
			if (strcmp(argv[i], "--outVideo") == 0) 
			{
				c_videoOut = true;
				printf("Record every input frame to file");

				i++;
				if (strcmp(argv[i], "raw16") == 0)
				{
					cout << "Recording in  Raw 16 file format\n";
					c_outVideoFF = RAW_16;
				} 
				else if (strcmp(argv[i], "avi24") == 0)
				{
					cout << "Recording in  AVI 24 file format\n";
					c_outVideoFF = AVI_24;
				}
				else if (strcmp(argv[i], "avi8") == 0)
				{
					cout << "Recording in  AVI 8 file format\n";
					c_outVideoFF = AVI_8;
				}				
				else
				{
					isCommandErr = true;
					strCommandErr << "Command-line argument ERROR: Unknown output video file format: " << argv[i] << "Expected either avi8 | raw16 | avi24\n";
					break;
				}
				i++;
				continue;
			}
			if (strcmp(argv[i], "--outVideoRes") == 0) 
			{
				i++;
				c_videoOutresolutionWidth= atoi(argv[i]);
				i++;
				c_videoOutresolutionHeight = atoi(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--outFeature") == 0) 
			{
				c_FeaturesOut = true;
				//outputFeafileName = string(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--outFramesPerFile") == 0) 
			{
				i++;
				c_outMaxFramesPerFile = atoi(argv[i]);
				
				//c_outVideoFileName = string(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--outDir") == 0) 
			{
				i++;
				c_outDir = string(argv[i]);
				if (fileFolderExists(c_outDir,1) == false)
				{
					isCommandErr = true;
					strCommandErr << "Command-line argument ERROR: Output Folder " << c_outDir.c_str() << "  does not exist. Please create it\n";
					break;
				}
				i++;
				continue;
			}
			if (strcmp(argv[i], "--startRecord") == 0) 
			{
				bIsStartRecord = true;
				i++;
				continue;
			}
			
			//----------------------------------------------------------
			//classifier arguments
			if (strcmp(argv[i], "--ed") == 0) 
			{
				c_bRunEDClassifier = true;
				i++;
				continue;
			}
			if (strcmp(argv[i], "--ednorm") == 0) 
			{
				c_bRunEDClassifier_NormBlur = true;
				i++;
				continue;
			}
			if (strcmp(argv[i], "--gestureLenMsec") == 0) 
			{
				isGestureLenMsec = true;
				i++;
				c_gestureLenMsec_min = atoi(argv[i]);
				i++;
				c_gestureLenMsec_max = atoi(argv[i]);
				i++;

				continue;
			}
			if (strcmp(argv[i], "--gestureSpacingMsec") == 0) 
			{
				i++;
				c_gestureSpacingMsec = atoi(argv[i]);
				printf("Spacing Between gestures to %f\n", c_gestureSpacingMsec);
				i++;
				continue;
			}

			if (strcmp(argv[i], "--ERgestureLenMsec") == 0) 
			{
				isGestureLenMsec = true;
				i++;
				c_er_gestureLenMsec_min = atoi(argv[i]);
				i++;
				c_er_gestureLenMsec_max = atoi(argv[i]);
				i++;

				continue;
			}
			if (strcmp(argv[i], "--MPgestureLenMsec") == 0) 
			{
				isGestureLenMsec = true;
				i++;
				c_mp_gestureLenMsec_min = atoi(argv[i]);
				i++;
				c_mp_gestureLenMsec_max = atoi(argv[i]);
				i++;

				continue;
			}
			
			if (strcmp(argv[i], "--hmm") == 0) 
			{
				i++;
				c_hmm_path = string(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--hmmBufferLen") == 0) 
			{
				i++;
				c_hmm_bufferLen = atoi(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--gestureLenHMM") == 0) 
			{
				isGestureLenHMM = true;
				i++;
				c_gestureLenHMM_er = atoi(argv[i]);
				i++;
				c_gestureLenHMM_mp = atoi(argv[i]);

				i++;
				continue;
			}


			//----------------------------------------------------------
			//Debug arguments
			if (strcmp(argv[i], "--aelimit") == 0) 
			{
				i++;
				c_AeMaxLimit = atoi(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--cutbg") == 0) 
			{
				c_cutBG = true;
				printf("cutbackground enabled for ASM tracker\n");
				i++;
				continue;
			}
#ifdef USE_ASM
			if (strcmp(argv[i], "--minFace") == 0) 
			{
				i++;
				c_minFace = atoi(argv[i]);
				printf("ASM tracker: minFaceSize set to %d\n", c_minFace);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--userGeneral") == 0) 
			{
				c_useGeneralTracker = true;
				if (c_useHawkingTracker == true)
				{
					c_useHawkingTracker = false;
					printf("ASM tracker: user changed from Hawking to General\n");
				}
				else
					printf("ASM tracker: user is set to General\n");
				i++;
				continue;
			}
			if (strcmp(argv[i], "--userHawking") == 0) 
			{
				c_useHawkingTracker = true;
				if (c_useGeneralTracker == true) 
				{
					c_useGeneralTracker = false;
					printf("ASM tracker: user changed from General to Hawking\n");
				}
				else
					printf("ASM tracker: user is set to Hawking\n");
				i++;
				continue;
			}
#endif
			if (strcmp(argv[i], "--stabThreshold") == 0) 
			{
				i++;
				c_stabThreshold = atof(argv[i]);
				printf("ASM tracker: stabThreshold set to %f\n", c_stabThreshold);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--noiseThreshold") == 0) 
			{
				i++;
				isNoiseThreshold = true;
				c_noiseThreshold = atof(argv[i]);
				printf("ED classifier: noiseThreshold set to %f\n", c_noiseThreshold);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--fileIdx") == 0) 
			{
				i++;
				c_fileIdx = atoi(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--eyeYoffset") == 0) 
			{
				i++;
				c_regionOffsetY_eye = atoi(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--skip") == 0) 
			{
				i++;
				c_skipFrames = atoi(argv[i]);
				i++;
				continue;
			}
			//----------------------------------------------------------
			if (strcmp(argv[i], "--aster") == 0) 
			{
				i++;
				if  (strcmp(argv[i], "LOG") == 0) 
				{
					c_action = ASTER_ACTION::ACTION_LOG;
				}
				else if  (strcmp(argv[i], "TRIGGER") == 0) 
				{
					c_action = ASTER_ACTION::ACTION_TRIGGER;
				}
				else
				{
					cout << "Unknown Aster option: " << argv[i] << "Expected either LOG|TRIGGER\n";
					exit(1);
				}
				i++;
				continue;
			}
			if (strcmp(argv[i], "--asterSyncFrames") == 0) 
			{
				i++;
				c_asterSyncFrames = atoi(argv[i]);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--beep") == 0) 
			{
				c_bIsBeep = true;
				i++;
				continue;
			}
			//----------------------------------------------------------
			if (strcmp(argv[i], "--resizeFactor") == 0) 
			{
				i++;
				c_dlibResizeFactor = atof(argv[i]);
				printf("Dlib detector: resizeFactor set to %g\n", c_dlibResizeFactor);
				i++;
				continue;
			}
			//----------------------------------------------------------
								
			cout << "Unknown or conflicting commandline option " << argv[i] << " Please check...\n";
			exit(1);
		}
		if (isCommandErr == true)
		{
			cout << strCommandErr;
			exit(1);
		}
	}
	catch (exception& e)
	{
		cout << "Command-line argument error: " << e.what() << '\n';
	}

	//-------------------------
	if ((c_videoFF == NONE) && (c_useCamera == false))
	{
		//default to useCamera
		cout << "Using camera\n";
		c_useCamera = true;
	}
	//----------------------------------------------------------------------------------------
	//Output
	if (c_videoOut || c_FeaturesOut)
	{
		if (fileFolderExists(c_outDir,1) == false)
		{
			cout << "Default Output Folder " << c_outDir.c_str() << "  does not exist. Please create it or specify another folder using the --outDir option\n";
			exit(1);
		}

		char buffer [80];
		time_t rawtime;
		struct tm * timeinfo;
		time (&rawtime);
		timeinfo = localtime (&rawtime);
		strftime (buffer,80,"_%m%d%Y-%H-%M-%S_",timeinfo);

		if (c_videoOut == true)
		{
			string outVideoFolderName = c_outDir + "\\Video" + string(buffer) + "\\";
			mkdir(outVideoFolderName.c_str());

			switch(c_outVideoFF)
			{
			case AVI_8:
			case AVI_24:
				c_outVideoFileName = outVideoFolderName + string("vid_") + string(buffer) + string(".avi");
				break;
			case RAW_16:
				c_outVideoFileName = outVideoFolderName + string("vid_") + string(buffer) + string(".raw");
				break;
			default:
				;//can't come here
			}
			
		}
		if (c_FeaturesOut == true)
		{
			string outFeaFolderName = c_outDir + "\\Fea" + string(buffer) + "\\";
			mkdir(outFeaFolderName.c_str());

			if (isStdFeaActive==true) 
			{
				outputEDFeafileName << outFeaFolderName << "fea_" << buffer << ".ed.txt";
				outputEDFeafile.open(outputEDFeafileName.str(), ios::out | ios::trunc);
			}
			if (isNormBlurFeaActive==true) 
			{
				outputEDFeafileName << outFeaFolderName << "fea_" << buffer << ".normBlur.txt";
				outputNormBlurFeafile.open(outputNormBlurFeafileName.str(), ios::out | ios::trunc);
			}
		}

		//if (bRunHMMClassifier == true)
		//{
		//	outputHMMResfileName << outputFeafileName << ".hmm.txt";
		//	//outputHMMResfile.open(outputHMMResfileName.str(), ios::out | ios::trunc);
		//}
	}
	//----------------------------------------------------------------------------------------
	if ( (c_bRunEDClassifier == true) && (c_bRunEDClassifier_NormBlur == true) )
	{
		cout << "Cannot run two early detect classiifers in parallel\n";
		cout << "Please specify one of --ed or --ednorm, NOT BOTH\n";
		exit(-1);
	}

	if ( (bRunMultiGestureSMClassifier || bRunEDClassifier_ER_MP) && (c_bRunEDClassifier == true) || (c_bRunEDClassifier_NormBlur == true) )
	{
		c_bRunEDClassifier == false;
		c_bRunEDClassifier_NormBlur = false;
		cout << "\nDisabling --ed or --ednorm options\n";
		cout << "These options not supported in this application\n\n";
	}

	if ( (c_bRunEDClassifier_NormBlur || bRunMultiGestureSMClassifier || bRunEDClassifier_ER_MP) 
		&& (isNoiseThreshold == false)) 
	{
		c_noiseThreshold = 0.1F; //default
	}
	if ( (isNoiseThreshold == true) )
	{
		if ( ((c_bRunEDClassifier) && (c_noiseThreshold < 1.0F)) 
				|| ((c_bRunEDClassifier_NormBlur || bRunMultiGestureSMClassifier || bRunEDClassifier_ER_MP) && (c_noiseThreshold > 1.0F))  )
		{
			cout << "Please check the value of noiseThreshold: expected to be >1.0 with --ed and <1.0 with --ednorm\n";
			cout << "specified noiseThreshold is " << c_noiseThreshold << "\n";
			cout << "suggested values are 10.0F or 0.1F\n";
			exit (-1);
		}

	}


#if 0
		if (c_bRunEDClassifier == true)
		{
			outputEDResfileName << outputFeafileName << ".classify_pipe.txt";
			outputEDResfile.open(outputEDResfileName.str(), ios::out | ios::trunc);
		}

		if (c_bRunEDClassifier_NormBlur == true)
		{
			outputEDResfileName << outputFeafileName << ".classify_pipe.txt";
			outputEDResfile.open(outputEDResfileName.str(), ios::out | ios::trunc);
		}
#endif
		
}
//============================================================================
void alarm()
{
     cout << "Sending SOS\n";
     printf("\a");
}

//============================================================================
int computeFps()
{
	int fpssum = 0;

	int64 curFrameTickTime = getTickCount();

	if (prevFrameTickTime > 0)
	{
		double frequency = getTickFrequency();
		int fps = cvRound(frequency / ((double)(curFrameTickTime - prevFrameTickTime)));
		fpses.push_back(fps);
		if (fpses.size() > 30)
		{
			fpses.pop_front();
		}
    
		for (unsigned int q = 0; q < fpses.size(); q++) 
		{
			fpssum += fpses[q];
		}
		fpssum /= (int) fpses.size();
	}
	prevFrameTickTime=curFrameTickTime;

	return(fpssum);
}
//============================================================================
#ifdef ASTER_SUPPORT
#if 1
void sendAsterEventsToLog(ASTER_EVENT eventID, GESTURE_ID gestureID)
{
	std::stringstream asterSendGestureString;	
	asterSendGestureString.str("");
	asterSendGestureString.clear();

	if (gestureID == GESTURE_ID::CT)
	{
		asterSendGestureString << "gesture=CT;";
	}
	else if (gestureID == GESTURE_ID::ER)
	{
		asterSendGestureString << "gesture=ER;";
	}
	else if (gestureID == GESTURE_ID::MP)
	{
		asterSendGestureString << "gesture=MP;";
	}
	else
	{
		cout << "Error in gestureID...exiting\n";
		exit(1);
	}
	if (eventID == ASTER_EVENT::EVENT_DOWN)
	{
		asterSendGestureString << "action=down;conf=100;time=12345678;actuate=false;tag=action_down";
	}
	else if (eventID == ASTER_EVENT::EVENT_TRIGGER)
	{
		asterSendGestureString << "action=trigger;conf=100;time=12345678;actuate=true;tag=action_trig";
	}
	else if (eventID == ASTER_EVENT::EVENT_UP)
	{
		asterSendGestureString << "action=up;conf=100;time=12345678;actuate=false;tag=action_up";
	}
	else if (eventID == ASTER_EVENT::EVENT_TRIGGER_LOGONLY)
	{
		asterSendGestureString << "action=down;conf=100;time=12345678;actuate=false;tag=action_trig";
	}
	else if (eventID == ASTER_EVENT::EVENT_SYNC)
	{
		asterSendGestureString << "action=up;conf=100;time=12345678;actuate=false;tag=action_sync";
	}
	else
	{
		cout << "Error in gestureID...exiting\n";
		exit(1);
	}

	asterSendGestureString << "~fps~" << fps;

	if (c_videoOut)
	{
		asterSendGestureString << "~frame~" << framenumber << "~fileframe~" << gui->getOutputFramesCount();
		asterSendGestureString << "~filename~" << gui->getCurOutputFilename();
	}
	
	asterSendGestureString << ";" ;



	std::string tmpstring = asterSendGestureString.str();
	const char* tmpChar = tmpstring.c_str();
	
	//cout << tmpstring.length() <<  " " << tmpChar <<"\n";
	gui->sendGesture(tmpChar);
}
#else
void sendAsterEventsToLog(ASTER_EVENT eventID, GESTURE_ID gestureID)
{
	std::stringstream asterSendGestureString;	
	asterSendGestureString.str("");
	asterSendGestureString.clear();
	
	if (eventID == ASTER_EVENT::EVENT_SYNC)
	{
		asterSendGestureString << "gesture=CT;action=up;conf=100;time=12345678;actuate=false;tag=action_sync";

		if (c_videoOut)
		{
			asterSendGestureString << "~frame~" << framenumber << "~fileframe~" << gui->getOutputFramesCount();
			asterSendGestureString << "~filename~" << gui->getCurOutputFilename();
		}
	
		asterSendGestureString << ";" ;

		std::string tmpstring = asterSendGestureString.str();
		const char* tmpChar = tmpstring.c_str();
	
		//cout << tmpstring.length() <<  " " << tmpChar <<"\n";
		gui->sendGesture(tmpChar);
	}
}
#endif
#endif
//============================================================================

//============================================================================
void sourceCallback(Mat mat, void* param)
{
    (void)param;
	framenumber++;

	 fps = computeFps();

	if (c_useCamera) 
	{
		if (((framenumber-1) % 100) == 0)
		{
			printf("\tFPS=%d\tFrame=%d\n", fps,framenumber-1);
#ifdef USE_XIMEA
			//float curExposure=mycamera->getProperty(cv::CAP_PROP_EXPOSURE);
			//printf("FPS=%d\tFrame=%d  Exposure=%f  cameraFps=%f\n", fps,framenumber-1,curExposure,mycamera->getProperty(cv::CAP_PROP_FPS));
			
						
			/*			
			printf("===============\n");
			printf("Camera settings:\n");
			printf("\t XI_PRM_AEAG = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG));
			printf("\t XI_PRM_AE_MAX_LIMIT  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AE_MAX_LIMIT));
			printf("\t XI_PRM_EXP_PRIORITY  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_EXP_PRIORITY));
			printf("\t XI_PRM_AEAG_LEVEL  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG_LEVEL));
			printf("\t XI_PRM_BPC  = %d\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_BPC));
			printf("===============\n");
			*/
#endif
		}
	}
	else
	{
		printf("\tFPS=%d\tFrame=%d \n", fps,framenumber-1);
	}

#if 1 //fake aster gestures
#ifdef ASTER_SUPPORT
	if ( (c_asterSyncFrames > 0) && (((framenumber-1) % c_asterSyncFrames) == 0))
	{
		sendAsterEventsToLog(ASTER_EVENT::EVENT_SYNC,GESTURE_ID::CT);
	}
#endif
#endif

	if ((c_bRunEDClassifier == true) && (c_useCamera == true))
		classifier->setFps(fps);

	if ((c_bRunEDClassifier_NormBlur == true) && (c_useCamera == true))
		classifierED_NormBlur->setFps(fps);
	
	if ((bRunEDClassifier_ER_MP == true) && (c_useCamera == true))
		classifierED_ER_MP->setFps(fps);

	if ((bRunMultiGestureSMClassifier == true) && (c_useCamera == true))
		classifierMG->setFps(fps);

#ifdef USE_FACEAPI
	if (faceAPIHandler->isFaceSearching() || !faceAPIHandler->isFaceFound())
    {
		printf("\t\tFace not found - resetting classifier\n");
		if (c_bRunEDClassifier == true)
			classifier->reset();
	}
#elif USE_REALSENSE
	if (!rsHandler->isFaceFound())
#elif USE_DLIB
	if (dlibHandler->isFaceSearching() || !dlibHandler->isFaceFound())
#else
	if (asmHandler->isFaceSearching() || !asmHandler->isFaceFound())
#endif
    {
		if ((framenumber_lastfaceNotFound+1) < framenumber)
		{
			//printf("\t\tFace not found - resetting classifier\n");
			printf("Face detection in progress - Please wait\n");
		}
		framenumber_lastfaceNotFound = framenumber;
		if (c_bRunEDClassifier == true)
		{
			classifier->reset();
			prevRegion = Rect(0,0,0,0); //reset nose region
		}
		if (c_bRunEDClassifier_NormBlur == true)
		{
			classifierED_NormBlur->reset();
			prevRegion = Rect(0,0,0,0); //reset nose region
		}

		if (bRunEDClassifier_ER_MP == true)
		{
			classifierED_ER_MP->reset();
			prevRegion = Rect(0,0,0,0); //reset nose region
		}
		if (bRunMultiGestureSMClassifier == true) 
		{
			classifierMG->reset();
			prevRegion = Rect(0,0,0,0); //reset nose region
		}
	}
	else if ((framenumber_lastfaceNotFound+1) == framenumber)
	{
		printf("Face Detected!\n");
	}

}

//============================================================================
void classifierCallback(Mat mat, void* param)
{
    (void)param;

    ClassType isHasMotion = (int)mat.at<float>(0, 0);

	//put results in a file
	if (isHasMotion > 0) //cheek-twitch
	{
		outputEDResfile << c_fileIdx << " " << framenumber << " " << framenumber << " " << isHasMotion << endl;
	}
}
//============================================================================
void classifierHMMCallback(Mat mat, void* param)
{
    (void)param;

    ClassType isGesture = (int)mat.at<float>(0, 0);

#if 1//SSdebug
	//retHypothesisLabel = 0;
	//add HMM classification here
	if ((isGesture != 0) && (isGesture != prevHMMGesture))
	{
		//send up trigger to ASTER
#ifdef ASTER_SUPPORT
#if 0
		if (isGesture == 1)
		{
			printf("**Cheek Twitch detected at frame=%d\n", framenumber);
			gui->sendGesture("gesture=CT;action=down;conf=100;time=12345678;actuate=false");
			if (c_action == ASTER_ACTION::TRIGGER)
				gui->sendGesture("gesture=CT;action=trigger;conf=100;time=12345678;actuate=true");
			 if (c_bIsBeep) Beep(3000,100);
		}
		else if (isGesture == 2)
		{
			printf("**Eyebrow Raise detected at frame=%d\n", framenumber);
			gui->sendGesture("gesture=ER;action=down;conf=100;time=12345678;actuate=false");
		}
		else if (isGesture == 3)
		{
			printf("**Mouth Pull detected at frame=%d\n", framenumber);
			gui->sendGesture("gesture=MP;action=down;conf=100;time=12345678;actuate=false");
		}
#else
		string asterSendGestureString = "";
		if (isGesture == 1)
		{
			printf("**Cheek Gesture: Start frame=%d\n", framenumber);
			//gui->sendGesture("gesture=CT;action=down;conf=100;time=12345678;actuate=false");

			sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::CT);
			if (c_action == ASTER_ACTION::ACTION_TRIGGER)
			{
				sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
				 if (c_bIsBeep) Beep(3000,100);
			}
			else
			{
				sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
			}
			
		}
		else if (isGesture == 2)
		{
			printf("**Eyebrow Raise: Start frame=%d\n", framenumber);
			//gui->sendGesture("gesture=ER;action=down;conf=100;time=12345678;actuate=false");
			sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::ER);
		}
		else if (isGesture == 3)
		{
			printf("**Mouth Pull: Start frame=%d\n", framenumber);
			//gui->sendGesture("gesture=MP;action=down;conf=100;time=12345678;actuate=false");
			sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::MP);
		}
#endif
#endif
	  
		prevHMMGesture = isGesture;
	}
	else if ((isGesture == 0) && (prevHMMGesture > 0))
	{
		//send end of gesture to aster
#ifdef ASTER_SUPPORT
		if (prevHMMGesture == 1)
		{
			printf("**Cheek Gesture: End frame=%d\n", framenumber);
			//gui->sendGesture("gesture=CT;action=up;conf=100;time=12345678;actuate=false");
			sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::CT);
		}
		else if (prevHMMGesture == 2)
		{
			printf("**Eyebrow Raise: End frame=%d\n", framenumber);
			//gui->sendGesture("gesture=ER;action=up;conf=100;time=12345678;actuate=false");
			sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::ER);
		}
		else if (prevHMMGesture == 3)
		{
			printf("**Mouth Pull: End frame=%d\n", framenumber);
			//gui->sendGesture("gesture=MP;action=up;conf=100;time=12345678;actuate=false");
			sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::MP);
		}
		prevHMMGesture = 0;
#endif
	}
#endif//SSdebug

	//put results in a file
	if (isGesture > 0) //cheek-twitch
	{
		outputEDResfile << c_fileIdx << " " << framenumber << " " << framenumber << " " << isGesture << endl;
	}
}
//============================================================================
//declare a callback function
void edFeatureCallback(cv::Mat mat, void *param)
{
	lock_guard<mutex> lk(m_fea);
	edFeatureFramenumber++;

	if (edFeatureFramenumber < framenumber)
	{
		for (int k=0; k<(framenumber-edFeatureFramenumber); k++)
		{
			for(int j= 0; j< mat.cols; j++)
				outputEDFeafile << STD_FOR_NO_REGIONS  << " ";
			if (isRegionMoved) 
				outputEDFeafile << "1";
			else
				outputEDFeafile << "0";
			outputEDFeafile << endl;
		}
		edFeatureFramenumber = framenumber;
	}
	for(int j= 0; j< mat.cols; j++)
		outputEDFeafile << mat.at<float>(j)  << " ";
	if (isRegionMoved) 
		outputEDFeafile << "1";
	else
		outputEDFeafile << "0";
	outputEDFeafile << endl;
}

//============================================================================
void checkEDFeatureFileOpen()
{
	edFeatureFramenumber_cur++;
	if ( (c_outMaxFramesPerFile > 0) && (edFeatureFramenumber_cur > c_outMaxFramesPerFile) )
	{		
		outputEDFeafile.close();

		edFeatureFilenumber++;
		outputEDFeafileName.str("");
		outputEDFeafileName << outputFeafileName << edFeatureFilenumber << ".ed.txt";
		outputEDFeafile.open(outputEDFeafileName.str(), ios::out | ios::trunc);

		edFeatureFramenumber_cur = 1;
	}
}
//============================================================================
void edFeatureCallback_2(cv::Mat mat, void *param)
{
	lock_guard<mutex> lk(m_fea);
	edFeatureFramenumber++;

	if (edFeatureFramenumber < framenumber)
	{
		for (int k=0; k<(framenumber-edFeatureFramenumber); k++)
		{
			
			checkEDFeatureFileOpen();

			for(int j= 0; j< mat.cols; j++)
				outputEDFeafile << STD_FOR_NO_REGIONS  << " ";
			if (isRegionMoved) 
				outputEDFeafile << "1";
			else
				outputEDFeafile << "0";
			outputEDFeafile << endl;
		}
		edFeatureFramenumber = framenumber;
	}

	checkEDFeatureFileOpen();

	for(int j= 0; j< mat.cols; j++)
		outputEDFeafile << mat.at<float>(j)  << " ";
	if (isRegionMoved) 
		outputEDFeafile << "1";
	else
		outputEDFeafile << "0";
	outputEDFeafile << endl;
}
//============================================================================
void normBlurFeatureCallback(cv::Mat mat, void *param)
{
	normBlurFeatureFramenumber++;

	if (normBlurFeatureFramenumber < framenumber)
	{
		for (int k=0; k<(framenumber-normBlurFeatureFramenumber); k++)
		{
			for(int j= 0; j< mat.cols; j++)
				outputNormBlurFeafile << STD_FOR_NO_REGIONS  << " ";
			outputNormBlurFeafile << endl;
		}
		normBlurFeatureFramenumber = framenumber;
	}
	for(int j= 0; j< mat.cols; j++)
		outputNormBlurFeafile << mat.at<float>(j)  << " ";
	outputNormBlurFeafile << endl;
}

//============================================================================
void checkNormBlurFeatureFileOpen()
{
	normBlurFeatureFramenumber_cur++;
	if ( (c_outMaxFramesPerFile > 0) && (normBlurFeatureFramenumber_cur > c_outMaxFramesPerFile) )
	{		
		outputNormBlurFeafile.close();

		normBlurFeatureFilenumber++;

		outputNormBlurFeafileName.str("");
		outputNormBlurFeafileName << outputFeafileName << normBlurFeatureFilenumber << ".normBlur.txt";
		outputNormBlurFeafile.open(outputNormBlurFeafileName.str(), ios::out | ios::trunc);

		normBlurFeatureFramenumber_cur = 1;
	}
}
//============================================================================
void normBlurFeatureCallback_2(cv::Mat mat, void *param)
{
	normBlurFeatureFramenumber++;

	if (normBlurFeatureFramenumber < framenumber)
	{
		for (int k=0; k<(framenumber-normBlurFeatureFramenumber); k++)
		{
			
			checkNormBlurFeatureFileOpen();

			for(int j= 0; j< mat.cols; j++)
				outputNormBlurFeafile << STD_FOR_NO_REGIONS  << " ";
			outputNormBlurFeafile << endl;
		}
		normBlurFeatureFramenumber = framenumber;
	}

	checkNormBlurFeatureFileOpen();

	for(int j= 0; j< mat.cols; j++)
		outputNormBlurFeafile << mat.at<float>(j)  << " ";
	outputNormBlurFeafile << endl;
}
//============================================================================
void myCameraCallback(bool status, void* context)
{   
    if (status == true)
	{
		if (status != prevAdjustStatus)
			printf("In camera callback --> Please wait...Auto Exposure Ajusting...\n");

		if (c_bRunEDClassifier == true)
			classifier->reset();
		if (c_bRunEDClassifier_NormBlur == true)
			classifierED_NormBlur->reset();
		if (bRunEDClassifier_ER_MP == true)
			classifierED_ER_MP->reset();
		if (bRunMultiGestureSMClassifier == true) 
			classifierMG->reset();
		
	}
	prevAdjustStatus=status;

}
//============================================================================

void regionCallback(Mat mat, void* param)
{  
	regionFramenumber++;
	if (regionFramenumber < framenumber)
	{
		//printf("Region disappeared: Reset Classifier\n");
		if (c_bRunEDClassifier == true)
			classifier->reset();
		if (c_bRunEDClassifier_NormBlur == true)
			classifierED_NormBlur->reset();
		if (bRunEDClassifier_ER_MP == true)
			classifierED_ER_MP->reset();
		if (bRunMultiGestureSMClassifier == true) 
			classifierMG->reset();
		regionFramenumber = framenumber;
	}

#if 0
	isRegionMoved = false;

	Rect curRegion((int)mat.at<double>(0, 0), (int)mat.at<double>(0, 1), (int)mat.at<double>(0, 2), (int)mat.at<double>(0, 3));

	if ( (prevRegion.x != curRegion.x)
		|| (prevRegion.y != curRegion.y)
		|| (prevRegion.width != curRegion.width)
		|| (prevRegion.height != curRegion.height) )
	{
	    isRegionMoved = true;
		prevRegion.x = curRegion.x;
		prevRegion.y = curRegion.y;
		prevRegion.width = curRegion.width;
		prevRegion.height = curRegion.height;

		printf("Region moved: Reset Classifier\n");
		if (c_bRunEDClassifier == true)
			classifier->reset();
		if (c_bRunEDClassifier_NormBlur == true)
			classifierED_NormBlur->reset();
	}
#endif
}

//============================================================================

void setupVideoSource(void)
{
	videoPlayer = Factory::createVideoPlayer();
    videoPlayer->setId("player");
    playlist = videoPlayer->getPlaylist();

	if (c_useCamera)
	{
		printf("Initializing camera\n");
#ifdef USE_XIMEA
		mycamera = Factory::createXimeaVideoSource();

		printf("Camera settings:\n");
			printf("\t XI_PRM_AEAG = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG));
			printf("\t XI_PRM_AE_MAX_LIMIT  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AE_MAX_LIMIT));
			printf("\t XI_PRM_EXP_PRIORITY  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_EXP_PRIORITY));
			printf("\t XI_PRM_AEAG_LEVEL  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG_LEVEL));
			printf("\t XI_PRM_BPC  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_BPC));
			printf("===============\n");
			//mycamera->setProperty(XimeaVideoSource::CV_CAP_PROP_XI_AE_MAX_LIMIT, 33000);
			
			//mycamera->setProperty(XimeaVideoSource::CV_CAP_PROP_XI_BPC, 1);
			mycamera->setProperty(XimeaVideoSource::CV_CAP_PROP_XI_AE_MAX_LIMIT, c_AeMaxLimit);
			printf("\n**Changing XI_AE_MAX_LIMIT to %d\n",c_AeMaxLimit);

			//mycamera->setProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG_LEVEL, AEGLevel);
			//printf("\nChanging CV_CAP_PROP_XI_AEAG_LEVEL to %d\n\n", AEGLevel);
		try
		{
			mycamera->openCamera(0);
			printf("New Camera settings:\n");
			printf("\t XI_PRM_AEAG = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG));
			printf("\t XI_PRM_AE_MAX_LIMIT  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AE_MAX_LIMIT));
			printf("\t XI_PRM_EXP_PRIORITY  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_EXP_PRIORITY));
			printf("\t XI_PRM_AEAG_LEVEL  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_AEAG_LEVEL));
			printf("\t XI_PRM_BPC  = %f\n", mycamera->getProperty(XimeaVideoSource::CV_CAP_PROP_XI_BPC));
			printf("===============\n");
		}
		catch (NoCameraException)
		{
			cout << "Unable to open Ximea camera, exiting" << endl;
			exit(1);
		}
#elif USE_REALSENSE
		mycamera = Factory::createRealSenseVideoSource();
		mycamera->setOutputResolution(640, 480);
		mycamera->open();
#else
		vector<pair<int, string>> allDevicesCV = CameraVideoSource::listDevices();
		mycamera = Factory::createCameraVideoSource(0);
		mycamera->openCamera(); //Sleep(1000);
		int width = mycamera->getOutputResolution().width;
		int height = mycamera->getOutputResolution().height;
		//printf("\Camera Resolution = %d : %d\n",  mycamera->getOutputResolution().width,mycamera->getOutputResolution().height );
		//printf("\Camera Resolution = %d : %d\n",  width,height );

#ifdef USE_ASM
		if (c_minFace == -1)
		{
			float minFaceSize_scaling = 0.49; //(500/1024);
			c_minFace = (int) ( (float) height * minFaceSize_scaling);
		}
		printf("\Camera Resolution = %d : %d, minfaceSize = %d\n",  width,height,c_minFace);
#else
		printf("\Camera Resolution = %d : %d",  width,height);
#endif
		//printf("\tGetProp Output Resolution = %f : %f\n",  mycamera->getProperty(cv::CAP_PROP_FRAME_WIDTH), mycamera->getProperty(cv::CAP_PROP_FRAME_HEIGHT));
		//printf("\tFPS=%f,  EXPOSURE=%f\n",mycamera->getProperty(cv::CAP_PROP_FPS), mycamera->getProperty(cv::CAP_PROP_EXPOSURE));
		//bool mvret = mycamera->setProperty(cv::CAP_PROP_FPS,30);
#endif
		FrameRange videorange;
		playlist->addSourceRange(mycamera, videorange);
		printf("Initialized camera\n");
	}
	else //file
	{
		if (c_videoFF == RAW_16)
		{
			source_raw16 = new RawfileVideoSource(videoFileName);
			//playlist->addSourceRange(source_raw16,FrameRange(0, END_FRAME));
			//playlist->addSourceRange(source_raw16,FrameRange(0, END_FRAME, c_skipFrames));
			playlist->addSourceRange(source_raw16,FrameRange(0, END_FRAME));
			int fileFramerate = source_raw16->getFramerate();
			int fileFramecount = source_raw16->getFrameCount();
			printf("InFile=%s, %d fps, #%d\n", videoFileName.c_str(), fileFramerate, fileFramecount);
		}
		else if (c_videoFF == AVI_24)
		{
			source_avi24 = Factory::createVideofileVideoSource(videoFileName);
			source_avi24->set12BitSource(true);
			source_avi24->setMultipleFiles(true);
			playlist->addSourceRange(source_avi24, FrameRange(0, END_FRAME));
		}
		else if (c_videoFF == AVI_8)
		{
			source_avi8 = Factory::createVideofileVideoSource(videoFileName);
			playlist->addSourceRange(source_avi8, FrameRange(0, END_FRAME));
		}
	} //file

	session->setVideoPlaylist(playlist);
	
}

//============================================================================
#ifdef USE_FACEAPI
void setupFaceAPITracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 int pollTimeout, bool realtime, float framerate, float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled,
                 bool useFaceAPITemplateRegions, float glassesFixStrength, TypeOfTransform transformType, bool useAutoGlassesSearch, int triggerWA)
{
	faceAPIHandler = Factory::createFaceAPI(pollTimeout, 
															realtime, 
															framerate, 
															stabThreshold, 
															stabFrames,
															normFrames,
															noFaceFrames,
															noFaceClockwiseRotation,
															glassesFixEnabled,
															useFaceAPITemplateRegions,
															glassesFixStrength,
															transformType,
															useAutoGlassesSearch);
	printf("Initialized FaceAPI\n");
	// ++++++++++++++++++++++++++++++++++++++++++++++++++
 //DEFAULT
	printf("Initializing region\n");

	regionREyebrow = Factory::createFaceAPIRegionCreator(
														FaceAPIRegionCreator::Settings(REGION_REYEBROW, 
																						regionWidthExpansionMultiplier, 
																						regionHeightExpansionMultiplier, 
																						normalize, 
																						pollTimeout, 
																						realtime, 
																						framerate, 
																						stabThreshold, 
																						stabFrames, 
																						regionOffsetX, 
																						regionOffsetY, 
																						normFrames, 
																						noFaceFrames,
																						noFaceClockwiseRotation,
																						glassesFixEnabled,
																						useFaceAPITemplateRegions,
																						glassesFixStrength,
																						transformType,
																						useAutoGlassesSearch,
																						triggerWA
																						), 
																				"regionREyebrow", 
																				faceAPIHandler);
	session->addRegionCreator(regionREyebrow);


	regionLEyebrow = Factory::createFaceAPIRegionCreator(
														FaceAPIRegionCreator::Settings(REGION_LEYEBROW, 
																						regionWidthExpansionMultiplier, 
																						regionHeightExpansionMultiplier, 
																						normalize, 
																						pollTimeout,
																						realtime, 
																						framerate, 
																						stabThreshold, 
																						stabFrames, 
																						regionOffsetX, 
																						regionOffsetY, 
																						normFrames, 
																						noFaceFrames,
																						noFaceClockwiseRotation,
																						glassesFixEnabled,
																						useFaceAPITemplateRegions,
																						glassesFixStrength,
																						transformType,
																						useAutoGlassesSearch,
																						triggerWA
																						), 
																				"regionLEyebrow", 
																				faceAPIHandler);
	session->addRegionCreator(regionLEyebrow);

	regionRCheek = Factory::createFaceAPIRegionCreator(
														FaceAPIRegionCreator::Settings(REGION_RCHEEK, 
																						regionWidthExpansionMultiplier, 
																						regionHeightExpansionMultiplier, 
																						normalize, 
																						pollTimeout, 
																						realtime, 
																						framerate, 
																						stabThreshold, 
																						stabFrames, 
																						regionOffsetX, 
																						regionOffsetY, 
																						normFrames, 
																						noFaceFrames,
																						noFaceClockwiseRotation,
																						glassesFixEnabled,
																						useFaceAPITemplateRegions,
																						glassesFixStrength,
																						transformType,
																						useAutoGlassesSearch,
																						triggerWA
																						), 
																				"regionRCheek", 
																				faceAPIHandler);
	session->addRegionCreator(regionRCheek);

	regionLCheek = Factory::createFaceAPIRegionCreator(
														FaceAPIRegionCreator::Settings(REGION_LCHEEK, 
																						regionWidthExpansionMultiplier, 
																						regionHeightExpansionMultiplier, 
																						normalize, 
																						pollTimeout,
																						realtime, 
																						framerate, 
																						stabThreshold, 
																						stabFrames, 
																						regionOffsetX, 
																						regionOffsetY, 
																						normFrames, 
																						noFaceFrames,
																						noFaceClockwiseRotation,
																						glassesFixEnabled,
																						useFaceAPITemplateRegions,
																						glassesFixStrength,
																						transformType,
																						useAutoGlassesSearch,
																						triggerWA
																						), 
																				"regionLCheek", 
																				faceAPIHandler);
	session->addRegionCreator(regionLCheek);

	regionMouth = Factory::createFaceAPIRegionCreator(
														FaceAPIRegionCreator::Settings(REGION_MOUTH, 
																						regionWidthExpansionMultiplier, 
																						regionHeightExpansionMultiplier, 
																						normalize, 
																						pollTimeout, 
																						realtime, 
																						framerate, 
																						stabThreshold, 
																						stabFrames, 
																						regionOffsetX, 
																						regionOffsetY, 
																						normFrames, 
																						noFaceFrames,
																						noFaceClockwiseRotation,
																						glassesFixEnabled,
																						useFaceAPITemplateRegions,
																						glassesFixStrength,
																						transformType,
																						useAutoGlassesSearch,
																						triggerWA
																						), 
																				"regionMouth", 
																				faceAPIHandler);
	session->addRegionCreator(regionMouth);
	prevRegion = Rect(0,0,0,0);

	regionNose = Factory::createFaceAPIRegionCreator(
														FaceAPIRegionCreator::Settings(REGION_NOSE, 
																						regionWidthExpansionMultiplier, 
																						regionHeightExpansionMultiplier, 
																						normalize, 
																						pollTimeout,
																						realtime, 
																						framerate, 
																						stabThreshold, 
																						stabFrames, 
																						regionOffsetX, 
																						regionOffsetY, 
																						normFrames, 
																						noFaceFrames,
																						noFaceClockwiseRotation,
																						glassesFixEnabled,
																						useFaceAPITemplateRegions,
																						glassesFixStrength,
																						transformType,
																						useAutoGlassesSearch,
																						triggerWA
																						), 
																				"regionNose", 
																				faceAPIHandler);
	session->addRegionCreator(regionNose);

	printf("Initialized region\n");
}
#elif USE_REALSENSE
void setupRealSenseTracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, bool glassesFixEnabled,
                 bool useTemplateRegions, float glassesFixStrength, bool useAutoGlassesSearch, 
				 bool cutBackground
				 )
{    
	printf("Initializing regions\n");
	printf("\n========================================================================\n");
	
	//SPtr<IRealSense> rsHandler = Factory::createRealSense(stabThreshold, stabFrames, glassesFixEnabled, useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground);
	rsHandler = Factory::createRealSense(stabThreshold, 
												stabFrames, 
												glassesFixEnabled, 
												useTemplateRegions, 
												glassesFixStrength, 
												useAutoGlassesSearch, 
												cutBackground 
												);
	
	//SPtr<RealSenseRegionCreator> regionREyebrow = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(REGION_REYEBROW, 1.0, 1.0, normalize));
	regionREyebrow = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(
																	REGION_REYEBROW, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground)
																	);
        regionREyebrow->setId("regionREyebrow");
        session->addRegionCreator(regionREyebrow);

	regionLEyebrow = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(
																	REGION_LEYEBROW, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground)
																	);

        regionLEyebrow->setId("regionLEyebrow");
        session->addRegionCreator(regionLEyebrow);

	  regionRCheek = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(
																	REGION_RCHEEK, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground)
																	);
        regionRCheek->setId("regionRCheek");
        session->addRegionCreator(regionRCheek);

		regionLCheek = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(
																	REGION_LCHEEK, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground)
																	);
        regionLCheek->setId("regionLCheek");
        session->addRegionCreator(regionLCheek);

		regionMouth = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(
																	REGION_MOUTH, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground)
																	);
        regionMouth->setId("regionMouth");
        session->addRegionCreator(regionMouth);

		regionNose = new RealSenseRegionCreator(rsHandler, RealSenseRegionCreator::Settings(
																	REGION_NOSE, 
																	0.6, //regionWidthExpansionMultiplier, 
																	0.9, //regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	0, //regionOffsetX, 
																	-10, //regionOffsetY,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground)
																	);
        regionNose->setId("regionNose");
        session->addRegionCreator(regionNose);
		prevRegion = Rect(0,0,0,0);
		printf("========================================================================\n\n");
		printf("Initialized regions\n");
}
#elif USE_DLIB
//============================================================================
void setupDlibTracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled,
                 bool useTemplateRegions, float glassesFixStrength, bool useAutoGlassesSearch, 
				 bool cutBackground, double resizeDownscale)
{
	printf("Initializing regions\n");
	printf("\n========================================================================\n");
	 //dlibHandler = Factory::createDlib(stabThreshold, stabFrames, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled, useTemplateRegions, 
                                     // glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);



	dlibHandler = Factory::createDlib(stabThreshold, 
												stabFrames, 
												10, //normFrames, 
												noFaceFrames, 
												noFaceClockwiseRotation, 
												glassesFixEnabled, 
												useTemplateRegions, 
												glassesFixStrength, 
												useAutoGlassesSearch, 
												false, //cutBackground, 
												resizeDownscale);
	
	//SPtr<DlibRegionCreator> regionREyebrow = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_REYEBROW, 1.0, 1.0, normalize));
	regionREyebrow = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(
																	REGION_REYEBROW, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	resizeDownscale)
																	);
        regionREyebrow->setId("regionREyebrow");
        session->addRegionCreator(regionREyebrow);

	regionLEyebrow = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(
																	REGION_LEYEBROW, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	resizeDownscale)
																	);

        regionLEyebrow->setId("regionLEyebrow");
        session->addRegionCreator(regionLEyebrow);

	  regionRCheek = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(
																	REGION_RCHEEK, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	resizeDownscale)
																	);
        regionRCheek->setId("regionRCheek");
        session->addRegionCreator(regionRCheek);

		regionLCheek = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(
																	REGION_LCHEEK, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	resizeDownscale)
																	);
        regionLCheek->setId("regionLCheek");
        session->addRegionCreator(regionLCheek);

		regionMouth = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(
																	REGION_MOUTH, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	resizeDownscale)
																	);
        regionMouth->setId("regionMouth");
        session->addRegionCreator(regionMouth);

		regionNose = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(
																	REGION_NOSE, 
																	0.6f, //regionWidthExpansionMultiplier, 
																	0.9f, //regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	0, //regionOffsetX, 
																	-20, //regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	resizeDownscale)
																	);
        regionNose->setId("regionNose");
        session->addRegionCreator(regionNose);
		prevRegion = Rect(0,0,0,0);
		printf("========================================================================\n\n");
		printf("Initialized regions\n");
}

#else //ASM
//============================================================================
void setupASMTracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled,
                 bool useTemplateRegions, float glassesFixStrength, bool useAutoGlassesSearch, 
				 bool cutBackground, double maxTrackLifeTime, int minFaceSize, bool useGeneralTracker, int regionOffsetY_eye)
{

	printf("Initializing regions\n");
	printf("\n========================================================================\n");
	 //SPtr<IASM> asmHandler = Factory::createASM(30.0f, 10, -1, 30, false, false, true, 0.0f, true);
	asmHandler = Factory::createASM(stabThreshold, 
												stabFrames, 
												normFrames, 
												noFaceFrames, 
												noFaceClockwiseRotation, 
												glassesFixEnabled, 
												useTemplateRegions, 
												glassesFixStrength, 
												useAutoGlassesSearch, 
												cutBackground, 
												maxTrackLifeTime, 
												minFaceSize,
												useGeneralTracker);
	
	 //SPtr<ASMRegionCreator> regionLEyebrow = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(REGION_LEYEBROW, 1.0, 1.0, true));
	regionREyebrow = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(
																	REGION_REYEBROW, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY_eye,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	maxTrackLifeTime, 
																	minFaceSize,
																	useGeneralTracker)
																	);
        regionREyebrow->setId("regionREyebrow");
        session->addRegionCreator(regionREyebrow);

	regionLEyebrow = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(
																	REGION_LEYEBROW, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY_eye,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	maxTrackLifeTime, 
																	minFaceSize,
																	useGeneralTracker)
																	);

        regionLEyebrow->setId("regionLEyebrow");
        session->addRegionCreator(regionLEyebrow);

	  regionRCheek = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(
																	REGION_RCHEEK, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	maxTrackLifeTime, 
																	minFaceSize,
																	useGeneralTracker)
																	);
        regionRCheek->setId("regionRCheek");
        session->addRegionCreator(regionRCheek);

		regionLCheek = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(
																	REGION_LCHEEK, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	maxTrackLifeTime, 
																	minFaceSize,
																	useGeneralTracker)
																	);
        regionLCheek->setId("regionLCheek");
        session->addRegionCreator(regionLCheek);

		regionMouth = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(
																	REGION_MOUTH, 
																	regionWidthExpansionMultiplier, 
																	regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	regionOffsetX, 
																	regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	maxTrackLifeTime, 
																	minFaceSize,
																	useGeneralTracker)
																	);
        regionMouth->setId("regionMouth");
        session->addRegionCreator(regionMouth);

		regionNose = new ASMRegionCreator(asmHandler, ASMRegionCreator::Settings(
																	REGION_NOSE, 
																	0.6f, //regionWidthExpansionMultiplier, 
																	0.9f, //regionHeightExpansionMultiplier,
																	normalize,
																	stabThreshold,
																	stabFrames, 
																	0, //regionOffsetX, 
																	-20, //regionOffsetY,
																	normFrames,
																	noFaceFrames,
																	noFaceClockwiseRotation,
																	glassesFixEnabled, 
																	useTemplateRegions, 
																	glassesFixStrength,
																	useAutoGlassesSearch, 
																	cutBackground, 
																	maxTrackLifeTime, 
																	minFaceSize,
																	useGeneralTracker)
																	);
        regionNose->setId("regionNose");
        session->addRegionCreator(regionNose);
		prevRegion = Rect(0,0,0,0);
		printf("========================================================================\n\n");
		printf("Initialized regions\n");
}

//============================================================================
#endif


void setupFaceRegionTracker(void)
{
#ifdef USE_FACEAPI
	
	float regionWidthExpansionMultiplier = 1.1f; 
	float regionHeightExpansionMultiplier = 1.2f;
	float stabThreshold = 40.0f;
	float glassesFixStrength = 0.5f;
	int normFrames = 10; //1000; //10;	
	//int noFaceFrames = 25; 
	bool useAutoGlassesSearch=true;

	//only used in FaceAPI
	int pollTimeout = 1;
	bool realtime = false; 
	float framerate = 60.0f; 
	TypeOfTransform transformType = ivcp::ROTATION;
	int triggerWA = -1;
#elif USE_REALSENSE
	float regionWidthExpansionMultiplier = 1.0f; //1.0f; 
	float regionHeightExpansionMultiplier = 1.0f; //1.0f;	
	
	float glassesFixStrength = 0.0f;
	bool useAutoGlassesSearch=true;

	 //only used for ASM & Realsense 
	bool cutBackground = c_cutBG; //default is false;
	//int minFaceSize = c_minFace;

	float stabThreshold = 20; //c_stabThreshold; //30.0f; //10;

#elif USE_DLIB
	float regionWidthExpansionMultiplier = 1.0f; //1.0f; 
	float regionHeightExpansionMultiplier = 1.0f; //1.0f;	
	
	float glassesFixStrength = 0.0f;
	int normFrames = -1; //10;	
	bool useAutoGlassesSearch=true;
	//double resizeDownscale = 1.0; //0.25;
	double resizeDownscale = c_dlibResizeFactor;
	//printf("Dlib detector: resizeFactor set to %g\n", c_dlibResizeFactor);

	 //only used for ASM & Realsense 
	bool cutBackground = c_cutBG; //default is false;
	float stabThreshold = c_stabThreshold; //30.0f; //10;
	
#elif USE_ASM
	float regionWidthExpansionMultiplier = 1.0f; //1.0f; 
	float regionHeightExpansionMultiplier = 1.0f; //1.0f;	
	
	float glassesFixStrength = 0.0f;
	int normFrames = -1; //10;	
	bool useAutoGlassesSearch=true;

	 //only used for ASM & Realsense 
	bool cutBackground = c_cutBG; //default is false;
	double maxTrackLifeTime = 30000;
	int minFaceSize = c_minFace;
	bool useGeneralTracker = c_useGeneralTracker;
	float stabThreshold = c_stabThreshold; //30.0f; //10;
	int regionOffsetY_eye = c_regionOffsetY_eye;

	if (c_useCamera && useGeneralTracker)
	{
		//stabThreshold = 0.0f;
		useAutoGlassesSearch=false;
		maxTrackLifeTime = 3000;
		//minFaceSize = 66;
	}
#endif
	int regionOffsetX = 0;
	int regionOffsetY = 0;

	bool normalize = true;	
#ifndef USE_XIMEA
	unsigned int stabFrames = 5U;
#else
	unsigned int stabFrames = 10U;
#endif
	int noFaceFrames = 30; 
	bool noFaceClockwiseRotation = false;
	
	
    bool glassesFixEnabled = false;
    bool useFaceAPITemplateRegions = true;  
	

#ifdef NO_FACE_WORKAROUND
	noFaceFrames = -1;
	printf("\nNO Face workaround will be applied!!!\n\n");
#endif

#ifdef USE_FACEAPI
	setupFaceAPITracker(regionWidthExpansionMultiplier, regionHeightExpansionMultiplier, normalize,
                 pollTimeout, realtime, framerate, stabThreshold, stabFrames, 
                 regionOffsetX, regionOffsetY, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled,
                 useFaceAPITemplateRegions, glassesFixStrength, transformType, useAutoGlassesSearch, triggerWA);

#elif USE_REALSENSE
	setupRealSenseTracker(regionWidthExpansionMultiplier,regionHeightExpansionMultiplier, normalize,
                 stabThreshold, stabFrames, 
                 regionOffsetX, regionOffsetY, glassesFixEnabled,
                 useFaceAPITemplateRegions, glassesFixStrength, useAutoGlassesSearch, 
				 cutBackground);
#elif USE_DLIB
	setupDlibTracker(regionWidthExpansionMultiplier, regionHeightExpansionMultiplier, normalize,
                 stabThreshold, stabFrames, 
                 regionOffsetX, regionOffsetY, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled,
                 useFaceAPITemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);
#elif USE_ASM	
	setupASMTracker(regionWidthExpansionMultiplier, regionHeightExpansionMultiplier, normalize,
                 stabThreshold, stabFrames, 
                 regionOffsetX, regionOffsetY, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled,
                 useFaceAPITemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground, maxTrackLifeTime, minFaceSize,useGeneralTracker,regionOffsetY_eye);
#endif //USE_FACEAPI
	printf("Face detection in progress...\n");
}

//============================================================================
void setupFeatureComputation_ED()
{
	//feature 
    motionStdDevFeature = new MotionEstimationStdDevFeature(NSEQUENCES);
    motionStdDevFeature->setId("mestddev");
    session->addFeature(motionStdDevFeature);
}

//============================================================================
void setupFeatureComputation_NormBlur()
{
	//feature 
    //normBlurFeature = new NormalFlowBlurredFeature(0.1f, NFBF_ENERGY_DX_DY, true);
	normBlurFeature = new NormalFlowBlurredFeature(0.0f, NFBF_ENERGY_DX_DY);
	//normBlurFeature = new NormalFlowBlurredFeature(0.0f, NFBF_ENERGY);
	normBlurFeature->setId("normFea");
	session->addFeature (normBlurFeature);
}

//============================================================================
void setupClassifier()
{
	if (c_bRunEDClassifier == true)
	{
		//classifier
		classifier = new CheekTwitchDetectionClassifier(6);
		classifier->setDetectionThreshold(thresh);
		classifier->setNoiseThreshold(c_noiseThreshold);
		//classifier->setGestureLen(c_gestureLen_max,c_gestureLen_min);
		classifier->setGestureLenMsec(c_gestureLenMsec_max,c_gestureLenMsec_min,c_gestureSpacingMsec);
		session->addClassifier(classifier);
	}
}
//============================================================================
void setupClassifier_ED_NormBlur()
{
	if (c_bRunEDClassifier_NormBlur == true)
	{
		//classifier
		classifierED_NormBlur = new CheekTwitchDetectionClassifier_NormBlur(6);
		classifierED_NormBlur->setDetectionThreshold(thresh_normBlur);
		classifierED_NormBlur->setNoiseThreshold(c_noiseThreshold);
		//classifierED_NormBlur->setGestureLen(c_gestureLen_max, c_gestureLen_min);
		classifierED_NormBlur->setGestureLenMsec(c_gestureLenMsec_max, c_gestureLenMsec_min,c_gestureSpacingMsec);
		session->addClassifier(classifierED_NormBlur);
	}
}
//============================================================================
void setupClassifier_ED_ER_MP()
{
	if (bRunEDClassifier_ER_MP == true)
	{
		//classifier
		classifierED_ER_MP = new EyebrowraiseMouthpullDetectionClassifier(6);
		//classifierED_ER_MP->setDetectionThreshold(thresh_normBlur);
		//classifierED_ER_MP->setNoiseThreshold(c_noiseThreshold);
		//classifierED_ER_MP->setGestureLen(c_gestureLen_max, c_gestureLen_min);
		classifierED_ER_MP->setGestureLenMsec_er(c_er_gestureLenMsec_max, c_er_gestureLenMsec_min,0);
		classifierED_ER_MP->setGestureLenMsec_mp(c_mp_gestureLenMsec_max, c_mp_gestureLenMsec_min,0);
		session->addClassifier(classifierED_ER_MP);
	}
}
//============================================================================
void setupClassifier_MG()
{
	if (bRunMultiGestureSMClassifier == true)
	{
		//classifier
		classifierMG = new MultiGestureSMClassifier(6);
		classifierMG->setGestureLenMsec_ct(c_gestureLenMsec_max, c_gestureLenMsec_min,0);
		classifierMG->setGestureLenMsec_er(c_er_gestureLenMsec_max, c_er_gestureLenMsec_min,0);
		
		session->addClassifier(classifierMG);
	}
}
//============================================================================
#if 0
void setupHMMClassifier()
{
	const int featureSize = 18;
	//const string pathToParamsFolder = "res\\hmm";
	const string pathToParamsFolder = c_hmm_path;
    HMMParameters hmm_params(pathToParamsFolder, featureSize);
	hmm_params.csv_output_filename = outputHMMResfileName.str();
	printf("HMM model folder set to %s\n", hmm_params.configFolder);
	classifierHMM = new HMMClassifier(hmm_params);
	//classifierHMM->setFeaturesBufMaxSize(30);
	classifierHMM->setFeaturesBufMaxSize(c_hmm_bufferLen);
	//classifierHMM->setHMMTriggerLen(c_gestureLen_max);
	classifierHMM->setHMMTriggerLen(c_gestureLenHMM_er,c_gestureLenHMM_mp);
    classifierHMM->loadModel();
	session->addClassifier(classifierHMM);
}
#endif
//============================================================================

#if 0
void setupGUI_singlePipeline(SPtr<Pipeline> pipeline)
{
	gui = Factory::createGUIFramework(pipeline);
	gui->setWindowName("Early Detection Classifier");
	gui->setSensitivitySettings((int)thresh);
#ifdef ASTER_SUPPORT
	gui->listenSocket();
	/*
	while(1)
	{
		printf("\nPress a key to send gesture");
		getchar();
		gui->sendGesture("gesture=CT;action=trigger;conf=100;time=12345678");
	}
	*/
#endif
}
#else
void setupGUI_singlePipeline()
{
	//printf("Setting up GUI framework\n");
	gui = Factory::createGUIFramework(pipeline_single);

	//gui->setWindowName("Early Detection Classifier - v0.18.28.1 clean ASM_webcam_ACAT");
	
#ifdef USE_XIMEA
	strGUIWindowTitle += " Ximea Camera";
#elif USE_REALSENSE
	strGUIWindowTitle += " Realsense Camera";
#else
	strGUIWindowTitle += " Integrated Camera";
#endif

#ifdef USE_ASM
	strGUIWindowTitle += " MSA";
#elif USE_REALSENSE
	strGUIWindowTitle += "";
#elif USE_DLIB
	strGUIWindowTitle += " DLIB";
#endif
	gui->setWindowName(strGUIWindowTitle);

	if (c_bRunEDClassifier)
		gui->setSensitivitySettings((int)thresh);
	else if (c_bRunEDClassifier_NormBlur)
		gui->setSensitivitySettings((int)(thresh_normBlur*100));
	else if (bRunEDClassifier_ER_MP)
		gui->setSensitivitySettings((int)(thresh_er_mp*100));
	else if (bRunMultiGestureSMClassifier == true) 
		gui->setSensitivitySettings((int)(thresh_mg*100));
#ifdef ASTER_SUPPORT
	gui->listenSocket();
	/*
	while(1)
	{
		printf("\nPress a key to send gesture");
		getchar();
		gui->sendGesture("gesture=CT;action=trigger;conf=100;time=12345678");
	}
	*/
#endif
	//printf("GUI framework setup complete\n");
}
#endif
//============================================================================

void setupGUI_singlePipeline_NormBlur(SPtr<Pipeline> pipeline)
{
	gui = Factory::createGUIFramework(pipeline);
	gui->setWindowName("Early Detection Classifier with NormBlur features");
	gui->setSensitivitySettings((int)(thresh_normBlur*100));
#ifdef ASTER_SUPPORT
	gui->listenSocket();
	/*
	while(1)
	{
		printf("\nPress a key to send gesture");
		getchar();
		gui->sendGesture("gesture=CT;action=trigger;conf=100;time=12345678");
	}
	*/
#endif
}
//============================================================================
void setupSinglePipeline()
{
	//pipeline - session
    pipeline_single = Factory::createPipeline(RUNTIME_PIPELINE);

    pipeline_single->setSession(session);

    pipeline_single->connectAll();
   
	pipeline_single->addNodeCallback(videoPlayer->getId(), &sourceCallback, NULL);
	pipeline_single->addNodeCallback(regionNose->getId(), &regionCallback, NULL);

	if (c_FeaturesOut == true)
	{
		if (isNormBlurFeaActive)
			pipeline_single->addNodeCallback(normBlurFeature->getId(), &normBlurFeatureCallback_2, NULL);
		else
			pipeline_parallel->addNodeCallback(motionStdDevFeature->getId(), &edFeatureCallback_2, NULL);
	}
	setupGUI_singlePipeline();
}
//============================================================================
void setupGUI_parallelPipeline()
{
	gui = Factory::createGUIFramework(pipeline_parallel);
	gui->setWindowName("Early Detection Classifier");
	//gui->setSensitivitySettings((int)thresh);
	if (c_bRunEDClassifier)
		gui->setSensitivitySettings((int)thresh);
	else if (c_bRunEDClassifier_NormBlur)
		gui->setSensitivitySettings((int)(thresh_normBlur*100));
	else if (bRunEDClassifier_ER_MP)
		gui->setSensitivitySettings((int)(thresh_er_mp*100));
	else if (bRunMultiGestureSMClassifier == true) 
		gui->setSensitivitySettings((int)(thresh_mg*100));

#ifdef ASTER_SUPPORT
	gui->listenSocket();
	/*
	while(1)
	{
		printf("\nPress a key to send gesture");
		getchar();
		gui->sendGesture("gesture=CT;action=trigger;conf=100;time=12345678");
	}
	*/
#endif

	if( gui->haveVideoOutput() )
    {
		printf("Writing captrured video to file %s\n", captureFilename);
		gui->generateVideoOutput(captureFilename, cv::VideoWriter::fourcc('D','I','B',' '));
	}
}

void setupParallelPipeline()
{
	pipeline_parallel = Factory::createParallelPipeline(RUNTIME_PIPELINE);
	
	pipeline_parallel->setPerformanceRate(60);

	pipeline_parallel->setSession(session);

	for (int r=0; r<6;r++)
	{
		pipeline_parallel->addLink(session->getVideoPlayer()->getId(), session->getRegionCreator(r)->getId());
		
		pipeline_parallel->addLink(session->getRegionCreator(r)->getId(), session->getFeature(0)->getId());
		if (c_bRunEDClassifier == true)
			pipeline_parallel->addLink(session->getFeature(0)->getId(), session->getClassifier(0)->getId());

		pipeline_parallel->addLink(session->getRegionCreator(r)->getId(), session->getFeature(1)->getId());
		if ( (c_bRunEDClassifier_NormBlur == true) || (bRunEDClassifier_ER_MP == true) || (bRunMultiGestureSMClassifier == true))
			pipeline_parallel->addLink(session->getFeature(1)->getId(), session->getClassifier(0)->getId());
	}

	pipeline_parallel->addNodeCallback(videoPlayer->getId(), &sourceCallback, NULL);
	pipeline_parallel->addNodeCallback(regionNose->getId(), &regionCallback, NULL);

	if (c_FeaturesOut == true)
	{
		pipeline_parallel->addNodeCallback(motionStdDevFeature->getId(), &edFeatureCallback_2, NULL);
		pipeline_parallel->addNodeCallback(normBlurFeature->getId(), &normBlurFeatureCallback_2, NULL);
	}

	setupGUI_parallelPipeline();
}
//============================================================================

//============================================================================
void setupVideoRecording()
{
	if (c_videoOut == true)
	{
		if (c_outVideoFF == AVI_24)
			gui->enableVideoOutput(c_outVideoFileName, cv::VideoWriter::fourcc('D','I','B',' '), cv::Size(c_videoOutresolutionWidth, c_videoOutresolutionHeight), 60, c_outMaxFramesPerFile,VIDEOFILE_OUTPUT);
		else if (c_outVideoFF == AVI_8)
			//gui->enableVideoOutput(c_outVideoFileName, cv::VideoWriter::fourcc('X','V','I','D'), cv::Size(c_videoOutresolutionWidth, c_videoOutresolutionHeight), 60, c_outMaxFramesPerFile,VIDEOFILE_OUTPUT, true);
			gui->enableVideoOutput(c_outVideoFileName, cv::VideoWriter::fourcc('X','V','I','D'), cv::Size(c_videoOutresolutionWidth, c_videoOutresolutionHeight), 30, c_outMaxFramesPerFile,VIDEOFILE_OUTPUT, true);
		else
			gui->enableVideoOutput(c_outVideoFileName, 0, cv::Size(c_videoOutresolutionWidth, c_videoOutresolutionHeight), 60, c_outMaxFramesPerFile,RAWFILE_OUTPUT);

		gui->setVideoCapture(bIsStartRecord);
	}
}
//============================================================================
void stopVideoRecording()
{
	if (c_videoOut == true)
	{
		gui->disableVideoOutput();
	}
	else if( gui->haveVideoOutput() )
    {
		cout << "Writing captrured video to file " << captureFilename <<"\n";
		gui->generateVideoOutput(captureFilename, cv::VideoWriter::fourcc('D','I','B',' '));
	}
}


CheekTwitchDetectionClassifier::CheekTwitchDetectionClassifier(int nRegions)
    {
        this->nRegions = nRegions;

		this->NOISE_FLOOR_FRAMECOUNT = 30; //10;
		
		this->NOISE_STD_THRESHOLD = 10.0F; //5.0F;
		this->MIN_CT_DURATION = 2;
		
		this->TOO_FAST = 100; 
		this->SECOND_PEAK_START = 25;
		this->MOUTH_THRESH_UP = 25; 
		this->MOUTH_THRESH_DOWN = 10;

		this->classifyState = SM_COMPUTE_NOISEFLOOR;

		this->noiseFloorDataMat = Mat(nRegions, NOISE_FLOOR_FRAMECOUNT, CV_32FC1, cv::Scalar(0) );
		this->meanNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->stdNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->noiseFloorCurIdx = 0;
		this->curFrameCount = 0;
		this->prevFeature = Mat(1,nRegions,CV_32FC1,cv::Scalar(0));
		this->foundNoiseFloor = false;
		this->fps = 60; //default

		this->GESTURE_LEN_MSEC_MAX = 500;
		this->GESTURE_LEN_MSEC_MIN = 150;
		this->GESTURE_SPACING_MSEC=0;
		this->GESTURE_LEN_MAX = (int)(((float)this->GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->GESTURE_LEN_MIN = (int)(((float)this->GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->GESTURE_SPACE = (int)(((float)this->GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		
		printf("Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->GESTURE_LEN_MSEC_MAX,this->GESTURE_LEN_MAX,this->GESTURE_LEN_MSEC_MIN,this->GESTURE_LEN_MIN);
		printf("Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->GESTURE_SPACING_MSEC, this->GESTURE_SPACE);

		gestureID = NO_GESTURE;
    }

void CheekTwitchDetectionClassifier::setFps(int curfps) 
	{
		lock_guard<mutex> lk(m_fps);
		this->fps = curfps;

#if 0
		int newSkipFrames = (int)((float)fps/2.0 +0.5);
		if (abs(SKIP_FRAMES_MAX-newSkipFrames) > 2)
		{
			SKIP_FRAMES_MAX = newSkipFrames;
			printf("1 adjusted gesture-length in classifer to %d %d for fps=%d\n", SKIP_FRAMES_MAX, SKIP_FRAMES_MIN, fps);
			SKIP_FRAMES_MIN = (int)((float)(fps*GESTURE_LEN_MIN)/60.0 +0.5);
			printf("2 adjusted gesture-length in classifer to %d %d for fps=%d\n", SKIP_FRAMES_MAX, SKIP_FRAMES_MIN, fps);

			//SECOND_PEAK_START = newSkipFrames - 5; //new
		}
#endif
	}

void CheekTwitchDetectionClassifier::setParamsBasedOnFps() 
{
	//GESTURE_LEN_MAX = (int)((float)fps/2.0 +0.5);
	//GESTURE_LEN_MIN = (int)((float)(fps*GESTURE_LEN_MIN)/60.0 +0.5);
	this->GESTURE_LEN_MAX = (int)(((float)this->GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
	this->GESTURE_LEN_MIN = (int)(((float)this->GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
	this->GESTURE_SPACE = (int)(((float)this->GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
	printf("Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
				fps,GESTURE_LEN_MAX, GESTURE_LEN_MSEC_MAX, GESTURE_LEN_MIN,GESTURE_LEN_MSEC_MIN,this->GESTURE_SPACE,GESTURE_SPACING_MSEC);
}

void CheekTwitchDetectionClassifier::setDetectionThreshold(float thresh) 
	{
		MOUTH_THRESH_UP = thresh;
		MOUTH_THRESH_DOWN = MOUTH_THRESH_UP - 15;
		if (MOUTH_THRESH_DOWN < 10)
				MOUTH_THRESH_DOWN = 10;
		//printf("Detection Threshold set to %f\n", thresh);
	}

void CheekTwitchDetectionClassifier::setNoiseThreshold(float thresh) 
{
	NOISE_STD_THRESHOLD = thresh;
	//printf("Detection Threshold set to %f\n", thresh);
}

void CheekTwitchDetectionClassifier::setGestureLen(int lenMax, int lenMin) 
{
	GESTURE_LEN_MAX = lenMax;
	GESTURE_LEN_MIN = lenMin;
	printf("Gesture len set to %d:%d\n", lenMax, lenMin);
}

void CheekTwitchDetectionClassifier::setGestureLenMsec(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	GESTURE_LEN_MSEC_MAX = lenMsecMax;
	GESTURE_LEN_MSEC_MIN = lenMsecMin;
	GESTURE_SPACING_MSEC = spacingMsec;
	printf("Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMax, lenMsecMin, spacingMsec);
}

void CheekTwitchDetectionClassifier::reset()
	{
		lock_guard<mutex> lk(m_reset);
		//if (foundNoiseFloor == false)
		{
			classifyState = SM_COMPUTE_NOISEFLOOR;
			noiseFloorDataMat.setTo(cv::Scalar(0));
			meanNoiseFloor.setTo(cv::Scalar(0));
			stdNoiseFloor.setTo(cv::Scalar(0));
			noiseFloorCurIdx = 0;
			this->curFrameCount = 0;
		}
		//else
		//{
		//	classifyState = SM_SEARCH_START;
		//}
	}

ClassType CheekTwitchDetectionClassifier::clasify(Mat feature)
    {
		ClassType ret = (ClassType)classifySM(feature);
		return (ret);
    }


int CheekTwitchDetectionClassifier::classifySM(Mat feature)
	{
		lock_guard<mutex> lk(m_classify);

		
		curFrameCount++;

		float mouthNF = meanNoiseFloor.at<float>(MOUTH);;

		switch (classifyState)
		{
			case (SM_COMPUTE_NOISEFLOOR):
				{
					foundNoiseFloor = true;
					//gestureID = NO_GESTURE;
					gestureID = NOISE_FLOOR_IN_PROGRESS;
					
					if (curFrameCount == 1)
					{
						printf("\n-------------------------------------------------------------------------------------------\n");
						printf("Please wait and do NO GESTURES...Calibration in progress...\n");	
						printf("-------------------------------------------------------------------------------------------\n");
					}
					//compute noise floor
					for (int i = 0; i < feature.cols; i++)
					{
						 noiseFloorDataMat.at<float>(i,noiseFloorCurIdx) = feature.at<float>(i);
					}
					/*
					if (framenumber == 82) 
					{
						printf("FN: %d  ",framenumber);
							for (int t=0; t<feature.cols; t++)
								printf(" %5.2f",  feature.at<float>(t));
							printf("\n");
					}
					*/
					noiseFloorCurIdx++;
					if ((noiseFloorCurIdx % NOISE_FLOOR_FRAMECOUNT) == 0)
						noiseFloorCurIdx = 0;
					if (curFrameCount < NOISE_FLOOR_FRAMECOUNT)
					{
						foundNoiseFloor = false;
						break;
					}

					classifyState = SM_SEARCH_START;
					
					for (int i = 0; i < nRegions; i++)
					{
						Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
						noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
						Scalar mean;
						Scalar stddev;
						meanStdDev(tmpMat, mean, stddev);
						float deb_mean = (float)mean(0);
						float deb_std = (float)stddev(0);		

						meanNoiseFloor.at<float>(i) = (float)mean(0);
						stdNoiseFloor.at<float>(i) = (float)stddev(0);

						if ((float)stddev(0) > NOISE_STD_THRESHOLD)
						{
							classifyState = SM_COMPUTE_NOISEFLOOR;
							foundNoiseFloor = false;
							break;
						}
					}
					if (foundNoiseFloor == true)
					{
						printf("\n-----------------------------------------------------\n");
						printf("Calibration complete \n");
						printf("-----------------------------------------------------\n");
#if 1
						printf("%d STD: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							stdNoiseFloor.at<float>(0), stdNoiseFloor.at<float>(1), stdNoiseFloor.at<float>(2), 
							stdNoiseFloor.at<float>(3), stdNoiseFloor.at<float>(4),stdNoiseFloor.at<float>(5)); 
						printf("%d MEA: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							meanNoiseFloor.at<float>(0), meanNoiseFloor.at<float>(1), meanNoiseFloor.at<float>(2), 
							meanNoiseFloor.at<float>(3), meanNoiseFloor.at<float>(4),meanNoiseFloor.at<float>(5)); 
						for (int i = 0; i < nRegions; i++)
						{
							Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
							noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
							printf("Fea: %d  ", i);
							for (int t=0; t<NOISE_FLOOR_FRAMECOUNT; t++)
								printf(" %5.2f",  tmpMat.at<float>(t));
							printf("\n");
						}
#endif
					}
				}
				break;
			case (SM_SEARCH_START):
				//search for up transition

				setDetectionThreshold(gui->getCurrentSensitivity());

				gestureID = NO_GESTURE;
				startFrame=-1;
				peak_up=0;
				peaks=0;
				printf("fea: %f, thresh: %f\n", feature.at<float>(MOUTH), (mouthNF+MOUTH_THRESH_UP));
				if ( (feature.at<float>(MOUTH) >= (mouthNF+MOUTH_THRESH_UP)) && (prevFeature.at<float>(MOUTH) < (mouthNF+MOUTH_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					peak_up = 1;
					classifyState = SM_POSSIBLE_START;
				}
				break;
			case (SM_POSSIBLE_START):
				//find down transition
				gestureID = NO_GESTURE;
				if ((curFrameCount-startFrame+1) <= MIN_CT_DURATION)
				{
					if (feature.at<float>(MOUTH) < (mouthNF+MOUTH_THRESH_UP))
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == MIN_CT_DURATION)
					{
					   classifyState = SM_START_CONFIRMED_FIND_END;

					   setParamsBasedOnFps();

					   //declare found gesture 
					   gestureID = CHEEK_TWITCH;   
					   printf("** Cheek Gesture:   Start frame=%d\n", framenumber);
#ifdef ASTER_SUPPORT
					   sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::CT);
					   if (GESTURE_LEN_MIN <= MIN_CT_DURATION)
						{
							printf("** Cheek Gesture: Trigger frame= %d\n", framenumber);
							if (c_action == ASTER_ACTION::ACTION_TRIGGER)
							{
							   sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
							   if (c_bIsBeep) Beep(3000,100);
							}
							else
							{
								sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
							}
						}
#endif					   
					}           
				}
				break;
			case (SM_START_CONFIRMED_FIND_END):
				//SS for writing to file only
				//gestureID = NO_GESTURE;

				if ( ((curFrameCount-startFrame+1) > GESTURE_LEN_MAX)  && (peak_up == 0))
					classifyState = SM_SEARCH_START; //declare end of gesture
				else if ( (peak_up==1) && ( (feature.at<float>(MOUTH) < (mouthNF+MOUTH_THRESH_DOWN)) && (prevFeature.at<float>(MOUTH)>=(mouthNF+MOUTH_THRESH_DOWN))) )
				{
					//found crossing
					peak_up=0;
					peaks = peaks+1;					
					if (peaks == 2)
						classifyState = SM_SEARCH_START; //declare end of gesture
				}
				else if ( (peak_up==0) && ( (feature.at<float>(MOUTH)>=(mouthNF+MOUTH_THRESH_UP)) & (prevFeature.at<float>(MOUTH)<(mouthNF+MOUTH_THRESH_UP))) ) 
				{
					if ((curFrameCount-startFrame) < SECOND_PEAK_START)
						//second peak
						peak_up=1;
				}
#ifdef ASTER_SUPPORT
				if (classifyState == SM_SEARCH_START)
				{
					if ((curFrameCount-startFrame+1) > GESTURE_LEN_MIN)
					{
						sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::CT);
					   printf("** Cheek Gesture:    End frame=%d\n", framenumber);

					   if (GESTURE_SPACE > 0)
					   {
						   classifyState = SM_WAIT_BETWEEN_GESTURES;
						   startFrame = curFrameCount;
					   }
					}
				}
				else 
				{
					//printf("\n** AM here %d %d\n",SKIP_FRAMES_MIN,(curFrameCount-startFrame));
					if ( (GESTURE_LEN_MIN > MIN_CT_DURATION) 
							&& ((curFrameCount-startFrame+1) == GESTURE_LEN_MIN) )
					{
						printf("** Cheek Gesture: Trigger frame= %d\n", framenumber);
						if (c_action == ASTER_ACTION::ACTION_TRIGGER)
						{
						   sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
						   if (c_bIsBeep) Beep(3000,100);
						}
						else
						{
							sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
						}
					}			
				}
#endif
				break;
			case SM_WAIT_BETWEEN_GESTURES:
				if ((curFrameCount-startFrame) >= GESTURE_SPACE)
				{
					classifyState = SM_SEARCH_START;
				}
				break;
		}
		//saveFeatureVector
		feature.copyTo(prevFeature);

		return (gestureID);
	}


//============================================================================
CheekTwitchDetectionClassifier_NormBlur::CheekTwitchDetectionClassifier_NormBlur(int nRegions)
    {
        this->nRegions = nRegions;

		this->NOISE_FLOOR_FRAMECOUNT = 30; //10;
		
		this->NOISE_STD_THRESHOLD = 0.1F; //5.0F;
		this->MIN_CT_DURATION = 2;
		
		this->TOO_FAST = 1.0; 
		this->SECOND_PEAK_START = this->GESTURE_LEN_MAX-5;
		this->MOUTH_THRESH_UP = 0.2; //0.3; 
		this->MOUTH_THRESH_DOWN = 0.15; //this->MOUTH_THRESH_UP - 0.1;

		this->classifyState = SM_COMPUTE_NOISEFLOOR;

		this->noiseFloorDataMat = Mat(nRegions, NOISE_FLOOR_FRAMECOUNT, CV_32FC1, cv::Scalar(0) );
		this->meanNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->stdNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->noiseFloorCurIdx = 0;
		this->curFrameCount = 0;
		this->prevFeature = Mat(1,nRegions*3,CV_32FC1,cv::Scalar(0));
		this->foundNoiseFloor = false;
		this->fps = 60;

		this->GESTURE_LEN_MSEC_MAX = 500; //420msec???
		this->GESTURE_LEN_MSEC_MIN = 150;
		this->GESTURE_SPACING_MSEC=0;
		this->GESTURE_LEN_MAX = (int)(((float)this->GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->GESTURE_LEN_MIN = (int)(((float)this->GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->GESTURE_SPACE = (int)(((float)this->GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		
		printf("Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->GESTURE_LEN_MSEC_MAX,this->GESTURE_LEN_MAX,this->GESTURE_LEN_MSEC_MIN,this->GESTURE_LEN_MIN);
		printf("Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->GESTURE_SPACING_MSEC, this->GESTURE_SPACE);
		gestureID = NO_GESTURE;
    }

#if 0
void CheekTwitchDetectionClassifier_NormBlur::setFps(int fps) 
	{
		lock_guard<mutex> lk(m_fps);
		//int newSkipFrames = (int)((float)fps/2.0 +0.5);
		int newSkipFrames = (int)((float)(fps*GESTURE_LEN_MAX)/60.0 +0.5);
		if (abs(SKIP_FRAMES_MAX-newSkipFrames) > 2)
		{
			SKIP_FRAMES_MAX = newSkipFrames;
			SKIP_FRAMES_MIN = (int)((float)(fps*GESTURE_LEN_MIN)/60.0 +0.5);
			printf("adjusted gesture-length in classifer to %d %d\n", SKIP_FRAMES_MAX, SKIP_FRAMES_MIN);
			SECOND_PEAK_START = newSkipFrames - 5; //new
		}
	}
#endif

void CheekTwitchDetectionClassifier_NormBlur::setFps(int curfps) 
	{
		lock_guard<mutex> lk(m_fps);
		this->fps = curfps;
	}

void CheekTwitchDetectionClassifier_NormBlur::setParamsBasedOnFps() 
{
		this->GESTURE_LEN_MAX = (int)(((float)this->GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->GESTURE_LEN_MIN = (int)(((float)this->GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		SECOND_PEAK_START = GESTURE_LEN_MAX - 5;
		this->GESTURE_SPACE = (int)(((float)this->GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		printf("Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
				fps,GESTURE_LEN_MAX, GESTURE_LEN_MSEC_MAX, GESTURE_LEN_MIN,GESTURE_LEN_MSEC_MIN,this->GESTURE_SPACE,GESTURE_SPACING_MSEC);
}

void CheekTwitchDetectionClassifier_NormBlur::setDetectionThreshold(float thresh) 
	{
		MOUTH_THRESH_UP = thresh;
		//printf("Detection Threshold set to %f\n", thresh);
		MOUTH_THRESH_DOWN = MOUTH_THRESH_UP - 0.1;
		if (MOUTH_THRESH_DOWN < 0.15)
				MOUTH_THRESH_DOWN = 0.15;
	}

void CheekTwitchDetectionClassifier_NormBlur::setNoiseThreshold(float thresh) 
{
	NOISE_STD_THRESHOLD = thresh;
	//printf("Detection Threshold set to %f\n", thresh);
}

void CheekTwitchDetectionClassifier_NormBlur::setGestureLen(int lenMax, int lenMin) 
{
	GESTURE_LEN_MAX = lenMax;
	GESTURE_LEN_MIN = lenMin;
	printf("Gesture len set to %d\n", lenMax);
}

void CheekTwitchDetectionClassifier_NormBlur::setGestureLenMsec(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	GESTURE_LEN_MSEC_MAX = lenMsecMax;
	GESTURE_LEN_MSEC_MIN = lenMsecMin;
	GESTURE_SPACING_MSEC = spacingMsec;
	printf("Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMax, lenMsecMin, spacingMsec);
}

void CheekTwitchDetectionClassifier_NormBlur::reset()
	{
		lock_guard<mutex> lk(m_reset);
		//if (foundNoiseFloor == false)
		{
			classifyState = SM_COMPUTE_NOISEFLOOR;
			noiseFloorDataMat.setTo(cv::Scalar(0));
			meanNoiseFloor.setTo(cv::Scalar(0));
			stdNoiseFloor.setTo(cv::Scalar(0));
			noiseFloorCurIdx = 0;
			this->curFrameCount = 0;
		}
		//else
		//{
		//	classifyState = SM_SEARCH_START;
		//}
	}

ClassType CheekTwitchDetectionClassifier_NormBlur::clasify(Mat feature)
    {
		ClassType ret = (ClassType)classifySM(feature);
		return (ret);
    }


int CheekTwitchDetectionClassifier_NormBlur::classifySM(Mat feature)
	{
		lock_guard<mutex> lk(m_classify);

		
		curFrameCount++;

		float mouthNF = meanNoiseFloor.at<float>(MOUTH);;

		switch (classifyState)
		{
			case (SM_COMPUTE_NOISEFLOOR):
				{
					foundNoiseFloor = true;
					//gestureID = NO_GESTURE;
					gestureID = NOISE_FLOOR_IN_PROGRESS;
					
					if (curFrameCount == 1)
					{
						printf("\n-------------------------------------------------------------------------------------------\n");
						printf("Please wait and do NO GESTURES...Calibration in progress...\n");	
						printf("-------------------------------------------------------------------------------------------\n");
					}

					//compute noise floor
					for (int i=0,j=0; i<feature.cols; i=i+3,j++)
					{
						 noiseFloorDataMat.at<float>(j,noiseFloorCurIdx) = feature.at<float>(i);
					}
					noiseFloorCurIdx++;
					if ((noiseFloorCurIdx % NOISE_FLOOR_FRAMECOUNT) == 0)
						noiseFloorCurIdx = 0;
					if (curFrameCount < NOISE_FLOOR_FRAMECOUNT)
					{
						foundNoiseFloor = false;
						break;
					}

					classifyState = SM_SEARCH_START;
					
					for (int i = 0; i < nRegions; i++)
					{
						Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
						noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
						Scalar mean;
						Scalar stddev;
						meanStdDev(tmpMat, mean, stddev);
						float deb_mean = (float)mean(0);
						float deb_std = (float)stddev(0);		

						meanNoiseFloor.at<float>(i) = (float)mean(0);
						stdNoiseFloor.at<float>(i) = (float)stddev(0);

						if ((float)stddev(0) > NOISE_STD_THRESHOLD)
						{
							classifyState = SM_COMPUTE_NOISEFLOOR;
							foundNoiseFloor = false;
							break;
						}
					}
					if (foundNoiseFloor == true)
					{
						printf("\n-----------------------------------------------------\n");
						printf("Calibration complete \n");
						printf("-----------------------------------------------------\n");
#if 0
						printf("%d STD: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							stdNoiseFloor.at<float>(0), stdNoiseFloor.at<float>(1), stdNoiseFloor.at<float>(2), 
							stdNoiseFloor.at<float>(3), stdNoiseFloor.at<float>(4),stdNoiseFloor.at<float>(5)); 
						printf("%d MEA: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							meanNoiseFloor.at<float>(0), meanNoiseFloor.at<float>(1), meanNoiseFloor.at<float>(2), 
							meanNoiseFloor.at<float>(3), meanNoiseFloor.at<float>(4),meanNoiseFloor.at<float>(5)); 
						for (int i = 0; i < nRegions; i++)
						{
							Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
							noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
							printf("Fea: %d  ", i);
							for (int t=0; t<NOISE_FLOOR_FRAMECOUNT; t++)
								printf(" %5.2f",  tmpMat.at<float>(t));
							printf("\n");
						}
#endif
					}
				}
				break;
			case (SM_SEARCH_START):
				//search for up transition

				setDetectionThreshold((float)(gui->getCurrentSensitivity())/100.0f);

				gestureID = NO_GESTURE;
				startFrame=-1;
				peak_up=0;
				peaks=0;
				if ( (feature.at<float>(MOUTH_POS) >= (mouthNF+MOUTH_THRESH_UP)) && (prevFeature.at<float>(MOUTH_POS) < (mouthNF+MOUTH_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					peak_up = 1;
					classifyState = SM_POSSIBLE_START;
				}
				break;
			case (SM_POSSIBLE_START):
				//find down transition
				gestureID = NO_GESTURE;
				if ((curFrameCount-startFrame+1) <= MIN_CT_DURATION)
				{
					if (feature.at<float>(MOUTH_POS) < (mouthNF+MOUTH_THRESH_UP))
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == MIN_CT_DURATION)
					{
					   classifyState = SM_START_CONFIRMED_FIND_END;

					   setParamsBasedOnFps();

					   //declare found gesture 
					   gestureID = CHEEK_TWITCH;
					   
					   printf("** Cheek Gesture:   Start frame=%d\n", framenumber);
#ifdef ASTER_SUPPORT
					   sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::CT);
					   if (GESTURE_LEN_MIN <= MIN_CT_DURATION)
						{
							printf("** Cheek Gesture: Trigger frame= %d\n", framenumber);
							if (c_action == ASTER_ACTION::ACTION_TRIGGER)
							{
							   sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
							   if (c_bIsBeep) Beep(3000,100);
							}
							else
							{
								sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
							}
						}
#endif					
					   
					}           
				}
				break;
			case (SM_START_CONFIRMED_FIND_END):
				//SS for writing to file only
				//gestureID = NO_GESTURE;

				if ( ((curFrameCount-startFrame+1) > GESTURE_LEN_MAX)  && (peak_up == 0))
					classifyState = SM_SEARCH_START; //declare end of gesture
				else if ( (peak_up==1) && ( (feature.at<float>(MOUTH_POS) < (mouthNF+MOUTH_THRESH_DOWN)) && (prevFeature.at<float>(MOUTH_POS)>=(mouthNF+MOUTH_THRESH_DOWN))) )
				{
					//found crossing
					peak_up=0;
					peaks = peaks+1;					
					if (peaks == 2)
						classifyState = SM_SEARCH_START; //declare end of gesture
				}
				else if ( (peak_up==0) && ( (feature.at<float>(MOUTH_POS)>=(mouthNF+MOUTH_THRESH_UP)) & (prevFeature.at<float>(MOUTH_POS)<(mouthNF+MOUTH_THRESH_UP))) ) 
				{
					if ((curFrameCount-startFrame) < SECOND_PEAK_START)
						//second peak
						peak_up=1;
				}
#ifdef ASTER_SUPPORT
				if (classifyState == SM_SEARCH_START)
				{
					if ((curFrameCount-startFrame+1) > GESTURE_LEN_MIN)
					{
						sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::CT);
					   printf("** Cheek Gesture:    End frame=%d\n", framenumber);

					   
					   if (GESTURE_SPACE > 0)
					   {
						   classifyState = SM_WAIT_BETWEEN_GESTURES;
						   startFrame = curFrameCount;
					   }
					}
				}
				else 
				{
					//printf("\n** AM here %d %d\n",SKIP_FRAMES_MIN,(curFrameCount-startFrame));
					if ( (GESTURE_LEN_MIN > MIN_CT_DURATION) 
							&& ((curFrameCount-startFrame+1) == GESTURE_LEN_MIN) )
					{
						printf("** Cheek Gesture: Trigger frame= %d\n", framenumber);
						if (c_action == ASTER_ACTION::ACTION_TRIGGER)
						{
						   sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
						   if (c_bIsBeep) Beep(3000,100);
						}
						else
						{
							sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
						}
					}			
				}
#endif
				break;
			case SM_WAIT_BETWEEN_GESTURES:
				if ((curFrameCount-startFrame) >= GESTURE_SPACE)
				{
					classifyState = SM_SEARCH_START;
				}
				break;
		}
		//saveFeatureVector
		feature.copyTo(prevFeature);

		return (gestureID);
	}


//============================================================================
EyebrowraiseMouthpullDetectionClassifier::EyebrowraiseMouthpullDetectionClassifier(int nRegions)
    {
        this->nRegions = nRegions;

		this->NOISE_FLOOR_FRAMECOUNT = 30; //10;
		
		this->NOISE_STD_THRESHOLD = 0.1F; //5.0F;
		//this->MIN_CT_DURATION = 2;
		
		//this->TOO_FAST = 1.0; 
		this->ER_THRESH_UP = 0.2; //0.3; 
		this->ER_THRESH_DOWN = 0.15; //this->MOUTH_THRESH_UP - 0.1;

		this->MP_THRESH_UP = 0.2; //0.3; 
		this->MP_THRESH_DOWN = 0.15; //this->MOUTH_THRESH_UP - 0.1;

		this->classifyState = SM_COMPUTE_NOISEFLOOR;

		this->noiseFloorDataMat = Mat(nRegions, NOISE_FLOOR_FRAMECOUNT, CV_32FC1, cv::Scalar(0) );
		this->meanNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->stdNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->noiseFloorCurIdx = 0;
		this->curFrameCount = 0;
		this->prevFeature = Mat(1,nRegions*3,CV_32FC1,cv::Scalar(0));
		this->foundNoiseFloor = false;
		this->fps = 60;

		this->ER_GESTURE_LEN_MSEC_MAX = 1000; //420msec???
		this->ER_GESTURE_LEN_MSEC_MIN = 250;
		this->ER_GESTURE_SPACING_MSEC=0;
		this->ER_GESTURE_LEN_MAX = (int)(((float)this->ER_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->ER_GESTURE_LEN_MIN = (int)(((float)this->ER_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->ER_GESTURE_SPACE = (int)(((float)this->ER_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		
		this->MP_GESTURE_LEN_MSEC_MAX = 1000; //420msec???
		this->MP_GESTURE_LEN_MSEC_MIN = 250;
		this->MP_GESTURE_SPACING_MSEC=0;
		this->MP_GESTURE_LEN_MAX = (int)(((float)this->MP_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->MP_GESTURE_LEN_MIN = (int)(((float)this->MP_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->MP_GESTURE_SPACE = (int)(((float)this->MP_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		
		printf("Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->ER_GESTURE_LEN_MSEC_MAX,this->ER_GESTURE_LEN_MAX,this->ER_GESTURE_LEN_MSEC_MIN,this->ER_GESTURE_LEN_MIN);
		printf("Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->ER_GESTURE_SPACING_MSEC, this->ER_GESTURE_SPACE);
		gestureID = NO_GESTURE;
    }

void EyebrowraiseMouthpullDetectionClassifier::setFps(int curfps) 
	{
		lock_guard<mutex> lk(m_fps);
		this->fps = curfps;
	}

void EyebrowraiseMouthpullDetectionClassifier::setParamsBasedOnFps() 
{
		this->ER_GESTURE_LEN_MAX = (int)(((float)this->ER_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->ER_GESTURE_LEN_MIN = (int)(((float)this->ER_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		//SECOND_PEAK_START = GESTURE_LEN_MAX - 5;
		this->ER_GESTURE_SPACE = (int)(((float)this->ER_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		//printf("ER Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,ER_GESTURE_LEN_MAX, ER_GESTURE_LEN_MSEC_MAX, ER_GESTURE_LEN_MIN,ER_GESTURE_LEN_MSEC_MIN,this->ER_GESTURE_SPACE,ER_GESTURE_SPACING_MSEC);

		this->MP_GESTURE_LEN_MAX = (int)(((float)this->MP_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->MP_GESTURE_LEN_MIN = (int)(((float)this->MP_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		//SECOND_PEAK_START = GESTURE_LEN_MAX - 5;
		this->MP_GESTURE_SPACE = (int)(((float)this->MP_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		//printf("MP Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,MP_GESTURE_LEN_MAX, MP_GESTURE_LEN_MSEC_MAX, MP_GESTURE_LEN_MIN,MP_GESTURE_LEN_MSEC_MIN,this->MP_GESTURE_SPACE,MP_GESTURE_SPACING_MSEC);
}


void EyebrowraiseMouthpullDetectionClassifier::setDetectionThreshold_er(float thresh_er) 
	{
		ER_THRESH_UP = thresh_er;
		//printf("Detection Threshold set to %f\n", thresh);
		ER_THRESH_DOWN = ER_THRESH_UP - 0.1;
		if (ER_THRESH_DOWN < 0.15)
				ER_THRESH_DOWN = 0.15;
	}

void EyebrowraiseMouthpullDetectionClassifier::setDetectionThreshold_mp(float thresh_mp) 
	{
		MP_THRESH_UP = thresh_mp;
		//printf("Detection Threshold set to %f\n", thresh);
		MP_THRESH_DOWN = MP_THRESH_UP - 0.1;
		if (MP_THRESH_DOWN < 0.15)
				MP_THRESH_DOWN = 0.15;
	}


void EyebrowraiseMouthpullDetectionClassifier::setNoiseThreshold(float thresh) 
{
	NOISE_STD_THRESHOLD = thresh;
	//printf("Detection Threshold set to %f\n", thresh);
}

void EyebrowraiseMouthpullDetectionClassifier::setGestureLenMsec_er(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	ER_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	ER_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	ER_GESTURE_SPACING_MSEC = spacingMsec;
	printf("Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMax, lenMsecMin, spacingMsec);
}

void EyebrowraiseMouthpullDetectionClassifier::setGestureLenMsec_mp(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	MP_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	MP_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	MP_GESTURE_SPACING_MSEC = spacingMsec;
	printf("Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMax, lenMsecMin, spacingMsec);
}

void EyebrowraiseMouthpullDetectionClassifier::reset()
	{
		lock_guard<mutex> lk(m_reset);
		//if (foundNoiseFloor == false)
		{
			classifyState = SM_COMPUTE_NOISEFLOOR;
			noiseFloorDataMat.setTo(cv::Scalar(0));
			meanNoiseFloor.setTo(cv::Scalar(0));
			stdNoiseFloor.setTo(cv::Scalar(0));
			noiseFloorCurIdx = 0;
			this->curFrameCount = 0;
		}
		//else
		//{
		//	classifyState = SM_SEARCH_START;
		//}
	}

ClassType EyebrowraiseMouthpullDetectionClassifier::clasify(Mat feature)
    {
		ClassType ret = (ClassType)classifySM(feature);
		return (ret);
    }


int EyebrowraiseMouthpullDetectionClassifier::classifySM(Mat feature)
	{
		lock_guard<mutex> lk(m_classify);

		
		curFrameCount++;

		float mouthNF = meanNoiseFloor.at<float>(MOUTH);

		float eyebrowNF = meanNoiseFloor.at<float>(R_EYEBROW);

		//printf("Frame: %d, mouth: %f, thresh: %f\teye: %f, thresh %f\n", curFrameCount, feature.at<float>(MOUTH), (mouthNF+MP_THRESH_UP), feature.at<float>(R_EYEBROW_POS),(eyebrowNF+ER_THRESH_UP));

		switch (classifyState)
		{
			case (SM_COMPUTE_NOISEFLOOR):
				{
					//printf("ER_MP: SM_COMPUTE_NOISEFLOOR\n");
					foundNoiseFloor = true;
					//gestureID = NO_GESTURE;
					gestureID = NOISE_FLOOR_IN_PROGRESS;
					
					if (curFrameCount == 1)
					{
						printf("\n-------------------------------------------------------------------------------------------\n");
						printf("Please wait and do NO GESTURES...Calibration in progress...\n");	
						printf("-------------------------------------------------------------------------------------------\n");
					}
					//compute noise floor
					for (int i=0,j=0; i<feature.cols; i=i+3,j++)
					{
						 noiseFloorDataMat.at<float>(j,noiseFloorCurIdx) = feature.at<float>(i);
					}
					noiseFloorCurIdx++;
					if ((noiseFloorCurIdx % NOISE_FLOOR_FRAMECOUNT) == 0)
						noiseFloorCurIdx = 0;
					if (curFrameCount < NOISE_FLOOR_FRAMECOUNT)
					{
						foundNoiseFloor = false;
						break;
					}

					classifyState = SM_SEARCH_START;
					
					for (int i = 0; i < nRegions; i++)
					{
						Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
						noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
						Scalar mean;
						Scalar stddev;
						meanStdDev(tmpMat, mean, stddev);
						float deb_mean = (float)mean(0);
						float deb_std = (float)stddev(0);		

						meanNoiseFloor.at<float>(i) = (float)mean(0);
						stdNoiseFloor.at<float>(i) = (float)stddev(0);

						if ((float)stddev(0) > NOISE_STD_THRESHOLD)
						{
							classifyState = SM_COMPUTE_NOISEFLOOR;
							foundNoiseFloor = false;
							break;
						}
					}
					if (foundNoiseFloor == true)
					{
						printf("\n-----------------------------------------------------\n");
						printf("Calibration complete \n");
						printf("-----------------------------------------------------\n");
#if 0
						printf("%d STD: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							stdNoiseFloor.at<float>(0), stdNoiseFloor.at<float>(1), stdNoiseFloor.at<float>(2), 
							stdNoiseFloor.at<float>(3), stdNoiseFloor.at<float>(4),stdNoiseFloor.at<float>(5)); 
						printf("%d MEA: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							meanNoiseFloor.at<float>(0), meanNoiseFloor.at<float>(1), meanNoiseFloor.at<float>(2), 
							meanNoiseFloor.at<float>(3), meanNoiseFloor.at<float>(4),meanNoiseFloor.at<float>(5)); 
						for (int i = 0; i < nRegions; i++)
						{
							Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
							noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
							printf("Fea: %d  ", i);
							for (int t=0; t<NOISE_FLOOR_FRAMECOUNT; t++)
								printf(" %5.2f",  tmpMat.at<float>(t));
							printf("\n");
						}
#endif
					}
				}
				break;
			case (SM_SEARCH_START):
				//search for up transition
				//printf("ER_MP: SM_SEARCH_START\n");

				setDetectionThreshold_er((float)(gui->getCurrentSensitivity())/100.0f);
				setDetectionThreshold_mp((float)(gui->getCurrentSensitivity())/100.0f);

				gestureID = NO_GESTURE;
				startFrame=-1;
				//peak_up=0;
				//peaks=0;
				//printf("mouth: %f, thresh: %f\teye: %f, thresh %f\n", feature.at<float>(MOUTH), (mouthNF+MP_THRESH_UP), feature.at<float>(R_EYEBROW_POS),(eyebrowNF+ER_THRESH_UP));
				if ( (feature.at<float>(MOUTH_POS) >= (mouthNF+MP_THRESH_UP)) && (prevFeature.at<float>(MOUTH_POS) < (mouthNF+MP_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					//peak_up = 1;
					classifyState = SM_MP_POSSIBLE_START;
					setParamsBasedOnFps();
				}
				else if ( (feature.at<float>(R_EYEBROW_POS) >= (eyebrowNF+ER_THRESH_UP)) && (prevFeature.at<float>(R_EYEBROW_POS) < (eyebrowNF+ER_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					//peak_up = 1;
					classifyState = SM_ER_POSSIBLE_START;
					 setParamsBasedOnFps();
				}
				break;
			case (SM_MP_POSSIBLE_START):
				//find down transition
				//printf("ER_MP: SM_MP_POSSIBLE_START: cur=%d, start=%d thresh=%d\n",curFrameCount,startFrame,MP_GESTURE_LEN_MIN);
				gestureID = NO_GESTURE;
				if ((curFrameCount-startFrame+1) <= MP_GESTURE_LEN_MIN)
				{
					if (feature.at<float>(MOUTH_POS) < (mouthNF+MP_THRESH_UP))
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == MP_GESTURE_LEN_MIN)
					{
					   classifyState = SM_MP_END;
					   //declare found gesture 
					   gestureID = MOUTH_PULL;
					   
					   printf("** Mouth Pull:   Start frame=%d\n", framenumber);
#ifdef ASTER_SUPPORT
					   sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::MP);
#endif					
					   
					}           
				}
				break;
			case (SM_ER_POSSIBLE_START):
				//find down transition
				//printf("ER_MP: SM_ER_POSSIBLE_START: cur=%d, start=%d thresh=%d\n",curFrameCount,startFrame,ER_GESTURE_LEN_MIN);
				gestureID = NO_GESTURE;
				if ((curFrameCount-startFrame+1) <= ER_GESTURE_LEN_MIN)
				{
					if (feature.at<float>(R_EYEBROW_POS) < (eyebrowNF+ER_THRESH_UP))
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == ER_GESTURE_LEN_MIN)
					{
					   classifyState = SM_ER_END;
					   //declare found gesture 
					   gestureID = EYEBROW_RAISE;
					   
					   printf("** Eyebrow Raise:   Start frame=%d\n", framenumber);
#ifdef ASTER_SUPPORT
					   sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::ER);
#endif					
					   
					}           
				}
				else
				{
					classifyState = SM_ER_END;
					   //declare found gesture 
					gestureID = EYEBROW_RAISE;
					   
					printf("** Eyebrow Raise:   Start frame=%d\n", framenumber);
#ifdef ASTER_SUPPORT
					sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::ER);
#endif					
				}
				break;
			case SM_MP_END:
				//printf("ER_MP: SM_MP_END\n");
				if ((curFrameCount-startFrame) >= MP_GESTURE_LEN_MAX)
				{
					classifyState = SM_SEARCH_START;
				}
				break;
			case SM_ER_END:
				//printf("ER_MP: SM_ER_END\n");
				if ((curFrameCount-startFrame) >= ER_GESTURE_LEN_MAX)
				{
					classifyState = SM_SEARCH_START;
				}
				break;
		}
		//saveFeatureVector
		feature.copyTo(prevFeature);

		return (gestureID);
	}


	//============================================================================
MultiGestureSMClassifier::MultiGestureSMClassifier(int nRegions)
    {
        this->nRegions = nRegions;

		this->NOISE_FLOOR_FRAMECOUNT = 30; //10;	
		this->NOISE_STD_THRESHOLD = 0.1F; //5.0F;
		this->MIN_CT_DURATION = 2;
		
		this->CT_TOO_FAST = 1.0; 
		

		this->CT_MOUTH_THRESH_UP = 0.2; //0.3; 
		this->CT_MOUTH_THRESH_DOWN = 0.15; //this->MOUTH_THRESH_UP - 0.1;
		this->ER_THRESH_UP = 0.2; //0.3; 
		this->ER_THRESH_DOWN = 0.15; //this->MOUTH_THRESH_UP - 0.1;
		this->MP_THRESH_UP = 0.2; //0.3; 
		this->MP_THRESH_DOWN = 0.15; //this->MOUTH_THRESH_UP - 0.1;

		this->classifyState = SM_COMPUTE_NOISEFLOOR;

		this->noiseFloorDataMat = Mat(nRegions, NOISE_FLOOR_FRAMECOUNT, CV_32FC1, cv::Scalar(0) );
		this->meanNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->stdNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->noiseFloorCurIdx = 0;
		this->curFrameCount = 0;
		this->prevFeature = Mat(1,nRegions*3,CV_32FC1,cv::Scalar(0));
		this->foundNoiseFloor = false;
		this->fps = 60;

		this->CT_GESTURE_LEN_MSEC_MAX = 500; //420msec???
		this->CT_GESTURE_LEN_MSEC_MIN = 150;
		this->CT_GESTURE_SPACING_MSEC=0;
		this->CT_GESTURE_LEN_MAX = (int)(((float)this->CT_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->CT_GESTURE_LEN_MIN = (int)(((float)this->CT_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->CT_GESTURE_SPACE = (int)(((float)this->CT_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);		
		this->CT_SECOND_PEAK_START = this->CT_GESTURE_LEN_MAX-5;

		this->ER_GESTURE_LEN_MSEC_MAX = 1000; //420msec???
		this->ER_GESTURE_LEN_MSEC_MIN = 250;
		this->ER_GESTURE_SPACING_MSEC=0;
		this->ER_GESTURE_LEN_MAX = (int)(((float)this->ER_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->ER_GESTURE_LEN_MIN = (int)(((float)this->ER_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->ER_GESTURE_SPACE = (int)(((float)this->ER_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		
		this->MP_GESTURE_LEN_MSEC_MAX = 1000; //420msec???
		this->MP_GESTURE_LEN_MSEC_MIN = 250;
		this->MP_GESTURE_SPACING_MSEC=0;
		this->MP_GESTURE_LEN_MAX = (int)(((float)this->MP_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->MP_GESTURE_LEN_MIN = (int)(((float)this->MP_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->MP_GESTURE_SPACE = (int)(((float)this->MP_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);

		//printf("\tCT: Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->CT_GESTURE_LEN_MSEC_MAX,this->CT_GESTURE_LEN_MAX,this->CT_GESTURE_LEN_MSEC_MIN,this->CT_GESTURE_LEN_MIN);
		//printf("\tCT: Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->CT_GESTURE_SPACING_MSEC, this->CT_GESTURE_SPACE);
		
		//printf("\tER: Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->ER_GESTURE_LEN_MSEC_MAX,this->ER_GESTURE_LEN_MAX,this->ER_GESTURE_LEN_MSEC_MIN,this->ER_GESTURE_LEN_MIN);
		//printf("\tER: Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->ER_GESTURE_SPACING_MSEC, this->ER_GESTURE_SPACE);

		//printf("\tMP: Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->MP_GESTURE_LEN_MSEC_MAX,this->MP_GESTURE_LEN_MAX,this->MP_GESTURE_LEN_MSEC_MIN,this->MP_GESTURE_LEN_MIN);
		//printf("\tMP: Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->MP_GESTURE_SPACING_MSEC, this->MP_GESTURE_SPACE);

		gestureID = NO_GESTURE;
    }


void MultiGestureSMClassifier::setFps(int curfps) 
	{
		lock_guard<mutex> lk(m_fps);
		this->fps = curfps;
	}

void MultiGestureSMClassifier::setParamsBasedOnFps() 
{
		this->CT_GESTURE_LEN_MAX = (int)(((float)this->CT_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->CT_GESTURE_LEN_MIN = (int)(((float)this->CT_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		CT_SECOND_PEAK_START = CT_GESTURE_LEN_MAX - 5;
		this->CT_GESTURE_SPACE = (int)(((float)this->CT_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		//printf("CT: Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,CT_GESTURE_LEN_MAX, CT_GESTURE_LEN_MSEC_MAX, CT_GESTURE_LEN_MIN,CT_GESTURE_LEN_MSEC_MIN,this->CT_GESTURE_SPACE,CT_GESTURE_SPACING_MSEC);

		this->ER_GESTURE_LEN_MAX = (int)(((float)this->ER_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->ER_GESTURE_LEN_MIN = (int)(((float)this->ER_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		//ER_SECOND_PEAK_START = GESTURE_LEN_MAX - 5;
		this->ER_GESTURE_SPACE = (int)(((float)this->ER_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		//printf("ER Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,ER_GESTURE_LEN_MAX, ER_GESTURE_LEN_MSEC_MAX, ER_GESTURE_LEN_MIN,ER_GESTURE_LEN_MSEC_MIN,this->ER_GESTURE_SPACE,ER_GESTURE_SPACING_MSEC);

		this->MP_GESTURE_LEN_MAX = (int)(((float)this->MP_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->MP_GESTURE_LEN_MIN = (int)(((float)this->MP_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		//MP_SECOND_PEAK_START = GESTURE_LEN_MAX - 5;
		this->MP_GESTURE_SPACE = (int)(((float)this->MP_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		//printf("MP Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,MP_GESTURE_LEN_MAX, MP_GESTURE_LEN_MSEC_MAX, MP_GESTURE_LEN_MIN,MP_GESTURE_LEN_MSEC_MIN,this->MP_GESTURE_SPACE,MP_GESTURE_SPACING_MSEC);
}

void MultiGestureSMClassifier::setDetectionThreshold_ct(float thresh) 
	{
		CT_MOUTH_THRESH_UP = thresh;
		//printf("Detection Threshold set to %f\n", thresh);
		CT_MOUTH_THRESH_DOWN = CT_MOUTH_THRESH_UP - 0.1;
		if (CT_MOUTH_THRESH_DOWN < 0.15)
				CT_MOUTH_THRESH_DOWN = 0.15;
	}



void MultiGestureSMClassifier::setDetectionThreshold_er(float thresh_er) 
{
	ER_THRESH_UP = thresh_er;
	//printf("Detection Threshold set to %f\n", thresh);
	ER_THRESH_DOWN = ER_THRESH_UP - 0.1;
	if (ER_THRESH_DOWN < 0.15)
			ER_THRESH_DOWN = 0.15;
}

void MultiGestureSMClassifier::setDetectionThreshold_mp(float thresh_mp) 
{
	MP_THRESH_UP = thresh_mp;
	//printf("Detection Threshold set to %f\n", thresh);
	MP_THRESH_DOWN = MP_THRESH_UP - 0.1;
	if (MP_THRESH_DOWN < 0.15)
			MP_THRESH_DOWN = 0.15;
}

void MultiGestureSMClassifier::setNoiseThreshold(float thresh) 
{
	NOISE_STD_THRESHOLD = thresh;
	//printf("Detection Threshold set to %f\n", thresh);
}
void MultiGestureSMClassifier::setGestureLen(int lenMax, int lenMin) 
{
	CT_GESTURE_LEN_MAX = lenMax;
	CT_GESTURE_LEN_MIN = lenMin;
	printf("Gesture len set to %d\n", lenMax);
}

void MultiGestureSMClassifier::setGestureLenMsec_ct(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	CT_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	CT_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	CT_GESTURE_SPACING_MSEC = spacingMsec;
	//printf("\tCT Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMin, lenMsecMax, spacingMsec);
	printf("\tCT Gesture len Msec set to %d:%d Msec\n", lenMsecMin, lenMsecMax);
}

void MultiGestureSMClassifier::setGestureLenMsec_er(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	ER_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	ER_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	ER_GESTURE_SPACING_MSEC = spacingMsec;
	printf("\tER Gesture len Msec set to %d:%d Msec\n", lenMsecMin, lenMsecMax);
}

void MultiGestureSMClassifier::setGestureLenMsec_mp(int lenMsecMax, int lenMsecMin, int spacingMsec) 
{
	MP_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	MP_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	MP_GESTURE_SPACING_MSEC = spacingMsec;
	//printf("Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMax, lenMsecMin, spacingMsec);
}

void MultiGestureSMClassifier::reset()
	{
		lock_guard<mutex> lk(m_reset);
		//if (foundNoiseFloor == false)
		{
			classifyState = SM_COMPUTE_NOISEFLOOR;
			noiseFloorDataMat.setTo(cv::Scalar(0));
			meanNoiseFloor.setTo(cv::Scalar(0));
			stdNoiseFloor.setTo(cv::Scalar(0));
			noiseFloorCurIdx = 0;
			this->curFrameCount = 0;
		}
		//else
		//{
		//	classifyState = SM_SEARCH_START;
		//}
	}


ClassType MultiGestureSMClassifier::clasify(Mat feature)
    {
		ClassType ret = (ClassType)classifySM(feature);
		return (ret);
    }


int MultiGestureSMClassifier::classifySM(Mat feature)
	{
		lock_guard<mutex> lk(m_classify);

		
		curFrameCount++;

		float mouthNF = meanNoiseFloor.at<float>(MOUTH);;
		float eyebrowNF = meanNoiseFloor.at<float>(R_EYEBROW);

		switch (classifyState)
		{
			case (SM_COMPUTE_NOISEFLOOR):
				{
					foundNoiseFloor = true;
					//gestureID = NO_GESTURE;
					gestureID = NOISE_FLOOR_IN_PROGRESS;
					
					if (curFrameCount == 1)
					{
						printf("\n-------------------------------------------------------------------------------------------\n");
						printf("Please wait and do NO GESTURES...Calibration in progress...\n");	
						printf("-------------------------------------------------------------------------------------------\n");
					}
					//compute noise floor
					for (int i=0,j=0; i<feature.cols; i=i+3,j++)
					{
						 noiseFloorDataMat.at<float>(j,noiseFloorCurIdx) = feature.at<float>(i);
					}
					noiseFloorCurIdx++;
					if ((noiseFloorCurIdx % NOISE_FLOOR_FRAMECOUNT) == 0)
						noiseFloorCurIdx = 0;
					if (curFrameCount < NOISE_FLOOR_FRAMECOUNT)
					{
						foundNoiseFloor = false;
						break;
					}

					classifyState = SM_SEARCH_START;
					
					for (int i = 0; i < nRegions; i++)
					{
						Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
						noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
						Scalar mean;
						Scalar stddev;
						meanStdDev(tmpMat, mean, stddev);
						float deb_mean = (float)mean(0);
						float deb_std = (float)stddev(0);		

						meanNoiseFloor.at<float>(i) = (float)mean(0);
						stdNoiseFloor.at<float>(i) = (float)stddev(0);

						if ((float)stddev(0) > NOISE_STD_THRESHOLD)
						{
							classifyState = SM_COMPUTE_NOISEFLOOR;
							foundNoiseFloor = false;
							break;
						}
					}
					if (foundNoiseFloor == true)
					{
						printf("\n-----------------------------------------------------\n");
						printf("Calibration complete \n");
						printf("-----------------------------------------------------\n");
#if 0
						printf("%d STD: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							stdNoiseFloor.at<float>(0), stdNoiseFloor.at<float>(1), stdNoiseFloor.at<float>(2), 
							stdNoiseFloor.at<float>(3), stdNoiseFloor.at<float>(4),stdNoiseFloor.at<float>(5)); 
						printf("%d MEA: %5.2f %5.2f %5.2f %5.2f %5.2f %5.2f\n", framenumber, 
							meanNoiseFloor.at<float>(0), meanNoiseFloor.at<float>(1), meanNoiseFloor.at<float>(2), 
							meanNoiseFloor.at<float>(3), meanNoiseFloor.at<float>(4),meanNoiseFloor.at<float>(5)); 
						for (int i = 0; i < nRegions; i++)
						{
							Mat tmpMat = Mat(1,NOISE_FLOOR_FRAMECOUNT,CV_32FC1);
							noiseFloorDataMat.row(i).copyTo(tmpMat.row(0));
							printf("Fea: %d  ", i);
							for (int t=0; t<NOISE_FLOOR_FRAMECOUNT; t++)
								printf(" %5.2f",  tmpMat.at<float>(t));
							printf("\n");
						}
#endif
					}
				}
				break;
			case (SM_SEARCH_START):
				//search for up transition
				//printf("ER_MP: SM_SEARCH_START\n");

				setDetectionThreshold_ct((float)(gui->getCurrentSensitivity())/100.0f);
				setDetectionThreshold_er((float)(gui->getCurrentSensitivity())/100.0f);
				setDetectionThreshold_mp((float)(gui->getCurrentSensitivity())/100.0f);

				gestureID = NO_GESTURE;

				startFrame=-1;
				peak_up=0;
				peaks=0;
				if ( (feature.at<float>(MOUTH_POS) >= (mouthNF+CT_MOUTH_THRESH_UP)) && (prevFeature.at<float>(MOUTH_POS) < (mouthNF+CT_MOUTH_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					peak_up = 1;
					classifyState = SM_CT_POSSIBLE_START;
				}
				else if ( (feature.at<float>(R_EYEBROW_POS) >= (eyebrowNF+ER_THRESH_UP)) && (prevFeature.at<float>(R_EYEBROW_POS) < (eyebrowNF+ER_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					//peak_up = 1;
					classifyState = SM_ER_POSSIBLE_START;
					 setParamsBasedOnFps();
				}
				break;
			case (SM_CT_POSSIBLE_START):
				//find down transition
				gestureID = NO_GESTURE;
				if ((curFrameCount-startFrame+1) <= MIN_CT_DURATION)
				{
					if (feature.at<float>(MOUTH_POS) < (mouthNF+CT_MOUTH_THRESH_UP))
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == MIN_CT_DURATION)
					{
					   classifyState = SM_CT_START_CONFIRMED_FIND_END;

					   setParamsBasedOnFps();

					   //declare found gesture 
					   gestureID = CHEEK_TWITCH;
					   
					   //printf("** Cheek Gesture:   Start frame=%d\n", framenumber);
					   printf("\tCheek Gesture #%d\n", framenumber);
#ifdef ASTER_SUPPORT
					   sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::CT);
					   if (CT_GESTURE_LEN_MIN <= MIN_CT_DURATION)
						{
							//printf("** Cheek Gesture: Trigger frame= %d\n", framenumber);
							if (c_action == ASTER_ACTION::ACTION_TRIGGER)
							{
							   sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
							   if (c_bIsBeep) Beep(3000,100);
							}
							else
							{
								sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
							}
						}
#endif					
					   
					}           
				}
				break;
			case (SM_ER_POSSIBLE_START):
				//find down transition
				//printf("ER_MP: SM_ER_POSSIBLE_START: cur=%d, start=%d thresh=%d\n",curFrameCount,startFrame,ER_GESTURE_LEN_MIN);
				gestureID = NO_GESTURE;
				if ((curFrameCount-startFrame+1) <= ER_GESTURE_LEN_MIN)
				{
					if (feature.at<float>(R_EYEBROW_POS) < (eyebrowNF+ER_THRESH_UP))
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == ER_GESTURE_LEN_MIN)
					{
					   classifyState = SM_ER_END;
					   //declare found gesture 
					   gestureID = EYEBROW_RAISE;
					   
					   //printf("** Eyebrow Raise:   Start frame=%d\n", framenumber);
					   printf("\tEyebrow Raise #%d\n", framenumber);
#ifdef ASTER_SUPPORT
					   //sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::ER);
					   if (c_action == ASTER_ACTION::ACTION_TRIGGER)
						{
							sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::ER);
							if (c_bIsBeep) Beep(3000,100);
						}
						else
						{
							sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::ER);
						}
#endif					
					   
					}           
				}
				else
				{
					classifyState = SM_ER_END;
					   //declare found gesture 
					gestureID = EYEBROW_RAISE;
					   
					//printf("** Eyebrow Raise:   Start frame=%d\n", framenumber);
					printf("\tEyebrow Raise #%d\n", framenumber);
#ifdef ASTER_SUPPORT
					//sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::ER);
					if (c_action == ASTER_ACTION::ACTION_TRIGGER)
					{
						sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::ER);
						if (c_bIsBeep) Beep(3000,100);
					}
					else
					{
						sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::ER);
					}
#endif					
				}
				break;
			case (SM_CT_START_CONFIRMED_FIND_END):
				//SS for writing to file only
				//gestureID = NO_GESTURE;

				if ( ((curFrameCount-startFrame+1) > CT_GESTURE_LEN_MAX)  && (peak_up == 0))
					classifyState = SM_SEARCH_START; //declare end of gesture
				else if ( (peak_up==1) && ( (feature.at<float>(MOUTH_POS) < (mouthNF+CT_MOUTH_THRESH_DOWN)) && (prevFeature.at<float>(MOUTH_POS)>=(mouthNF+CT_MOUTH_THRESH_DOWN))) )
				{
					//found crossing
					peak_up=0;
					peaks = peaks+1;					
					if (peaks == 2)
						classifyState = SM_SEARCH_START; //declare end of gesture
				}
				else if ( (peak_up==0) && ( (feature.at<float>(MOUTH_POS)>=(mouthNF+CT_MOUTH_THRESH_UP)) & (prevFeature.at<float>(MOUTH_POS)<(mouthNF+CT_MOUTH_THRESH_UP))) ) 
				{
					if ((curFrameCount-startFrame) < CT_SECOND_PEAK_START)
						//second peak
						peak_up=1;
				}
#ifdef ASTER_SUPPORT
				if (classifyState == SM_SEARCH_START)
				{
					if ((curFrameCount-startFrame+1) > CT_GESTURE_LEN_MIN)
					{
						sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::CT);
					   //printf("** Cheek Gesture:    End frame=%d\n", framenumber);

					   
					   if (CT_GESTURE_SPACE > 0)
					   {
						   classifyState = SM_CT_WAIT_BETWEEN_GESTURES;
						   startFrame = curFrameCount;
					   }
					}
				}
				else 
				{
					//printf("\n** AM here %d %d\n",SKIP_FRAMES_MIN,(curFrameCount-startFrame));
					if ( (CT_GESTURE_LEN_MIN > MIN_CT_DURATION) 
							&& ((curFrameCount-startFrame+1) == CT_GESTURE_LEN_MIN) )
					{
						//printf("** Cheek Gesture: Trigger frame= %d\n", framenumber);
						if (c_action == ASTER_ACTION::ACTION_TRIGGER)
						{
						   sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
						   if (c_bIsBeep) Beep(3000,100);
						}
						else
						{
							sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
						}
					}			
				}
#endif
				break;
			case SM_ER_END:
				//printf("ER_MP: SM_ER_END\n");
				if ((curFrameCount-startFrame) >= ER_GESTURE_LEN_MAX)
				{
					classifyState = SM_SEARCH_START;
				}

			case SM_CT_WAIT_BETWEEN_GESTURES:
				if ((curFrameCount-startFrame) >= CT_GESTURE_SPACE)
				{
					classifyState = SM_SEARCH_START;
				}
				break;
		}
		//saveFeatureVector
		feature.copyTo(prevFeature);

		return (gestureID);
	}

