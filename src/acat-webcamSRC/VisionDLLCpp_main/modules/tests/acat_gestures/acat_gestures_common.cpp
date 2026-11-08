///////////////////////////////////////////////////////////////////////////
// <copyright file="acat_gestures_common.cpp" company="Intel Corporation">
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
#include <sstream>
#include "ivcp.hpp"

#include <windows.h>

#include <sys/stat.h> 
#include <sys/types.h> 
#include <direct.h>


using namespace cv;
using namespace std;
using namespace ivcp;

#include "acat_gestures_common.hpp"
#include "../acat_gestures_dll/acat_gestures_dll.h"
VisionEventCallback callbackHandler = 0;

string strGUIWindowTitle = "WebcamDLLGUI"; // SS 05/12/2023 - changed title to help with ACAT control of window
//string strGUIWindowTitle = "WebcamDLLGUI";
//-------------------------
//#define NSEQUENCES 6 // # of regions

bool isNormBlurFeaActive=false; // by default it is not active
bool bRunMultiGestureSMClassifier = false;

//-------------------------
//arguments set from the command-line parameters
bool c_useCamera=false; 
int c_cameraId = -1;    //no camera selected - user will be prompted if more than 1 cameras present

int c_videoFF=NONE;
int c_outVideoFF = AVI_8; //SS 05/23 AVI_8;
std::string videoFileName;
string outputFeafileName;
stringstream outputNormBlurFeafileName;
ofstream outputNormBlurFeafile;
string captureFilename = "queueVideoRec.avi";

//-------------------------
//pipeline arguments
SPtr<Pipeline> pipeline_single;
SPtr<Session> session;
SPtr<VideoPlayer> videoPlayer;
SPtr<VideoPlaylist> playlist;
SPtr<IVideoSource> source_avi8;
SPtr<VideofileVideoSource> source_avi24;
SPtr<RawfileVideoSource> source_raw16;
SPtr<MotionEstimationStdDevFeature> motionStdDevFeature;
SPtr<NormalFlowBlurredFeature> normBlurFeature;
SPtr<IGUIFramework> gui = 0;
#ifdef USE_REALSENSE
SPtr<RealSenseVideoSource> mycamera;
#else
SPtr<CameraVideoSource> mycamera;
#endif

#ifdef USE_REALSENSE
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
#endif

//-------------------------
//fps computation
std::deque<int> fpses;
int64 prevFrameTickTime=-1;
int fps;
mutex m_sendEvent;
//-------------------------
//keep track of missedframes due to face not getting detected
int framenumber = 0;
int framenumber_lastfaceNotFound = 0;
int framenumber_last_ct_gesturestart = 0;
int framenumber_last_er_gesturestart = 0;

bool c_FeaturesOut = false;
mutex m_fea;

int normBlurFeatureFramenumber = 0;
int normBlurFeatureFramenumber_cur = 0;
int normBlurFeatureFilenumber = 0;
int regionFramenumber = 0;
bool bIsStartRecord = false;
float STD_FOR_NO_REGIONS = -1.0F;
float thresh_mg = 0.2F;
Rect prevRegion;

//-------------------------
bool c_bIsBeep                    = DEFAULT_BEEP;

bool  c_cutBG                     = DEFAULT_REGION_PARAM_CUT_BACKGROUND;
float c_stabThreshold             = DEFAULT_REGION_PARAM_STAB_THRESHOLD;
double c_dlibResizeFactor         = DEFAULT_DLIBRESIZEFACTOR;

int   c_ct_gestureLenMsec_min     = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTURELENMSEC_MIN;
int   c_ct_gestureLenMsec_max     = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTURELENMSEC_MAX;
int   c_ct_gestureSpacingMsec     = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTURESPACINGMSEC;
int	  c_ct_gestureHoldMsec        = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTUREHOLDMSEC;

int   c_er_gestureLenMsec_max     = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_ER_GESTURELENMSEC_MAX;
int   c_er_gestureLenMsec_min     = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_ER_GESTURELENMSEC_MIN;
int   c_er_gestureHoldMsec        = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_ER_GESTUREHOLDMSEC;

float c_noiseThreshold            = DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_NOISE_THRESHOLD;


bool   c_videoOut                 = DEFAULT_VIDEOOUT;
int    c_outMaxFramesPerFile      = DEFAULT_VIDEOOUT_PARAM_MAX_FRAMES_PER_FILE;
string c_outVideoFileName         = DEFAULT_VIDEOOUT_PARAM_VIDEO_FILENAME;
int    c_videoOutresolutionWidth  = DEFAULT_VIDEOOUT_PARAM_RESOLUTION_WIDTH;
int    c_videoOutresolutionHeight = DEFAULT_VIDEOOUT_PARAM_RESOLUTION_HEIGHT;
string c_outDir                   = DEFAULT_VIDEOOUT_PARAM_OUTDIR;

ASTER_ACTION c_action             = ASTER_ACTION::ACTION_TRIGGER;
int          c_asterSyncFrames    = DEAFULT_ASTERSYNCFRAMES;
int          c_acatPort           = DEAFULT_ACATPORT;

SPtr<MultiGestureSMClassifier> classifierMG;

int64  gestureCount=0;



//============================================================================

void sendCommandToGUI(char *str, double value)
{
	if (gui != 0)
		gui->sendCommandToGUI(str, value);
}

void showVideo()
{
	if (gui != 0)
		gui->showVideo();
}

void hideVideo()
{
	if (gui != 0)
		gui->hideVideo();
}

bool isVideoVisible()
{
	return (gui) ? gui->isVideoVisible() : false;
}

//============================================================================
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
		if ( (stat(inName.c_str(), &statusBuffer) == 0) && ((statusBuffer.st_mode & S_IFMT) == S_IFDIR))
			return true;
	}
    
    return false;
}

//============================================================================

void processCommandLineArgs(int argc, char **argv)
{
	if ((argc > 1) && (strcmp(argv[1], "--help") == 0))
	{
		printf("Need arguments\n");
		
		printf("\nAvailable options are:\n");
		printf("\t--------------------------------------\n");
		printf("\n\t Video Input Options\n");
		printf("\t\t  --usecamera [default] : for camera input\n\n");
		printf("\t\t  --cameraId [-1] : to preselect camera-id, default no selection, user will be prompted\n\n");

		printf("\t\tFor file input use one of:\n");
		printf("\t\t --raw16 filename:  for 16-bit raw format (data collected using Intel tool\n");
		printf("\t\t --avi24 filename: for 16-bit data collected as 24-bit AVI format (data collected using the pipeline)\n");
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
		printf("\t\t --gestureLenMsec int int [default: 150 500] : minimum maximum cheek-twitch gesture len in MSec\n");
		printf("\t\t --gestureSpacingMsec int [default: 0] : ??\n"); 
		printf("\t\t--------------------------------------\n");
		printf("\n\t\t State machine based ER gesture Options\n");
		printf("\t\t --ERgestureLenMsec int int [default: 150 1000]: min max length of ER gesture in msec\n");
		printf("\t\t--------------------------------------\n");
		printf("\t --aster LOG|TRIGGER : how to actuate aster: deafult:TRIGGER\n");
		printf("\t --beep : turns on the beep for cheeck twitch detection]\n");
		printf("\t --asterSyncFrames int [for aster sync frame number recording: default [5000]\n");
		printf("\t --port int [port to talk to ACAT: default [27015]\n");

		printf("\t--------------------------------------\n");
		printf("\n\tOptions mainly for debugging are:\n");
		printf("\t --stabThreshold float [default: 30] : to enable cut background for tracker\n");
		printf("\t --noiseThreshold float [default: 10.0 for --ed OR 0.3 for --ednorm] : for calibrating noise floor\n");
			
		printf("\t--------------------------------------\n");
		printf("\n\tOptions for tracker\n");
		printf("\t --resizeFactor double [default: 1.0]: factor to resize image for dlib face detector\n");		

		exit(1);
	}

	
	bool isNoiseThreshold=false;
	bool isGestureLenMsec=false;

	bool isCommandErr = false;
	std::stringstream strCommandErr;
	strCommandErr.str("");
	strCommandErr.clear();

	int i = 1;
	try {
		while (i < argc)
		{
			//Video input arguments
			if (strcmp(argv[i], "--usecamera") == 0)
			{
				if (c_videoFF == NONE)
				{
					//cout << "Using camera\n";
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
			if (strcmp(argv[i], "--cameraId") == 0) 
			{
				i++;
				c_cameraId = atoi(argv[i]);
				printf("If using camera input, use camera Id: %d\n", c_cameraId);
				i++;
				continue;
			}
			if ( (strcmp(argv[i], "--raw16") == 0)  ||  (strcmp(argv[i], "--avi24") == 0) || (strcmp(argv[i], "--avi8") == 0))
			{
				if (c_useCamera == true)
				{
					c_useCamera = false;
					cout << "**File input detected... Camera input has been disabled\n";

				}
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
				i++;
				continue;
			}
			if (strcmp(argv[i], "--outFramesPerFile") == 0) 
			{
				i++;
				c_outMaxFramesPerFile = atoi(argv[i]);
				
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
			if (strcmp(argv[i], "--gestureLenMsec") == 0) 
			{
				isGestureLenMsec = true;
				i++;
				c_ct_gestureLenMsec_min = atoi(argv[i]);
				i++;
				c_ct_gestureLenMsec_max = atoi(argv[i]);
				i++;

				continue;
			}
			if (strcmp(argv[i], "--gestureSpacingMsec") == 0) 
			{
				i++;
				c_ct_gestureSpacingMsec = atoi(argv[i]);
				printf("Spacing Between gestures to %d\n", c_ct_gestureSpacingMsec);
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
						

			//----------------------------------------------------------
			//arguments - mainly for debugging

			if (strcmp(argv[i], "--stabThreshold") == 0) 
			{
				i++;
				c_stabThreshold = (float) atof(argv[i]);
				printf("tracker: stabThreshold set to %f\n", c_stabThreshold);
				i++;
				continue;
			}
			if (strcmp(argv[i], "--noiseThreshold") == 0) 
			{
				i++;
				isNoiseThreshold = true;
				c_noiseThreshold = (float) atof(argv[i]);
				printf("ED classifier: noiseThreshold set to %f\n", c_noiseThreshold);
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
			if (strcmp(argv[i], "--port") == 0) 
			{
				i++;
				c_acatPort = atoi(argv[i]);
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
			cout << strCommandErr.str();
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
		//cout << "Using camera\n";
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

		struct tm timeinfo;
		time (&rawtime);
		localtime_s (&timeinfo,&rawtime);
		strftime (buffer,80,"_%m%d%Y-%H-%M-%S_",&timeinfo);

		if (c_videoOut == true)
		{
			string outVideoFolderName = c_outDir + "\\Video" + string(buffer) + "\\";
			_mkdir(outVideoFolderName.c_str());

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
			_mkdir(outFeaFolderName.c_str());

			if (isNormBlurFeaActive==true) 
			{
				outputNormBlurFeafileName << outFeaFolderName << "fea_" << buffer << ".normBlur.txt";
				outputFeafileName = outputNormBlurFeafileName.str();
				outputNormBlurFeafile.open(outputNormBlurFeafileName.str(), ios::out | ios::trunc);
			}
		}

		
	}
	//----------------------------------------------------------------------------------------

	if ( (bRunMultiGestureSMClassifier) 
		&& (isNoiseThreshold == false)) 
	{
		c_noiseThreshold = 0.1F; //default
	}
	if ( (isNoiseThreshold == true) )
	{
		if ( (bRunMultiGestureSMClassifier) && (c_noiseThreshold > 1.0F))
		{
			cout << "Please check the value of noiseThreshold: expected to be < 1.0 \n";
			cout << "specified noiseThreshold is " << c_noiseThreshold << "\n";
			cout << "suggested value is 0.1F\n";
			exit (-1);
		}

	}		
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
		int frameFps = cvRound(frequency / ((double)(curFrameTickTime - prevFrameTickTime)));
		fpses.push_back(frameFps);
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

void sendAsterEventsToLog(ASTER_EVENT eventID, GESTURE_ID gestureID)
{
	lock_guard<mutex> lk(m_sendEvent);
	gestureCount++;

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
	else if (gestureID == GESTURE_ID::CALIBRATE)
	{
		//asterSendGestureString << "gesture=CALIB_START;";
	}
	else if (gestureID == GESTURE_ID::INIT)
	{
		//asterSendGestureString << "gesture=INIT;";
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
	else if (eventID == ASTER_EVENT::EVENT_CALIBRATION_START)
	{
		asterSendGestureString << "gesture=CALIB_START;action=up;conf=100;time=12345678;actuate=false;tag=calib_start";
	}
	else if (eventID == ASTER_EVENT::EVENT_CALIBRATION_END)
	{
		asterSendGestureString << "gesture=CALIB_END;action=up;conf=100;time=12345678;actuate=false;tag=calib_end";
	}
	else if (eventID == ASTER_EVENT::EVENT_CALIBRATION_INPROGRESS)
	{
	}
	else if (eventID == ASTER_EVENT::EVENT_INIT_DONE)
	{
		asterSendGestureString << "gesture=INIT_DONE;action=up;conf=100;time=12345678;actuate=false;tag=init_done";
	}
	else if (eventID == ASTER_EVENT::EVENT_EXCEPTION)
	{
		asterSendGestureString.clear();
		asterSendGestureString << "gesture=EXCEPTION;action=up;conf=100;time=12345678;actuate=false;tag=exception";
	}
	else
	{
		cout << "Error in gestureID...exiting\n";
		exit(1);
	}
	asterSendGestureString << "~fps~" << fps;
	asterSendGestureString << "~cnt~" << gestureCount;
	if (gestureID == GESTURE_ID::CT)
		asterSendGestureString << "~thresh~" << classifierMG->getDetectionThreshold_ct();
	else if (gestureID == GESTURE_ID::ER)
		asterSendGestureString << "~thresh~" << classifierMG->getDetectionThreshold_er();

	if (c_videoOut)
	{
		asterSendGestureString << "~frame~" << framenumber << "~fileframe~" << gui->getOutputFramesCount();
		asterSendGestureString << "~filename~" << gui->getCurOutputFilename();
	}
	
	asterSendGestureString << ";" ;

	std::string tmpstring = asterSendGestureString.str();
	const char* tmpChar = tmpstring.c_str();
	
	if (callbackHandler != 0)
	{
		callbackHandler(tmpChar);
	}

#ifdef ASTER_SUPPORT
	gui->sendGesture(tmpChar);
#endif
}

//============================================================================
void sourceCallback(Mat, void* param)
{
    (void)param;
	framenumber++;

	 fps = computeFps();

	if (c_useCamera) 
	{
		if (((framenumber-1) % 100) == 0)
		{
			//printf("\tFPS=%d\tFrame=%d\n", fps,framenumber-1);
			printf("FPS=%d\n", fps);
		}
	}
	else
	{
		printf("\tFPS=%d\tFrame=%d \n", fps,framenumber-1);
	}


#ifdef ASTER_SUPPORT
	//fake aster gestures
	if ( (c_asterSyncFrames > 0) && (((framenumber-1) % c_asterSyncFrames) == 0))
	{
		sendAsterEventsToLog(ASTER_EVENT::EVENT_SYNC,GESTURE_ID::CT);
	}
#endif

	if ((bRunMultiGestureSMClassifier == true) && (c_useCamera == true))
		classifierMG->setFps(fps);

#ifdef  USE_REALSENSE
	if (!rsHandler->isFaceFound())
#elif USE_DLIB
	if (dlibHandler->isFaceSearching() || !dlibHandler->isFaceFound())
#endif
    {

		if ((framenumber_lastfaceNotFound+1) < framenumber)
		{
			printf("Face detection in progress - Please wait\n");
		}

		framenumber_lastfaceNotFound = framenumber;
		
		if (bRunMultiGestureSMClassifier == true) 
		{
			classifierMG->reset();
			prevRegion = Rect(0,0,0,0); //reset nose region
		}
	}
	else if ((framenumber_lastfaceNotFound+1) == framenumber)
	{
		printf("Face Detected!\n");
		//gui->sendCommandToGUI("action=RECALIBRATE", 0);
	}

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
	(void)param;
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

void regionCallback(Mat, void*)
{  
	regionFramenumber++;
	if ( (regionFramenumber < framenumber) )// || dlibHandler->getIsFaceDataUpdated())
	{
		//printf("Region disappeared: Reset Classifier\n");
		if (bRunMultiGestureSMClassifier == true) 
			classifierMG->reset();
		regionFramenumber = framenumber;
	}
	
}

//============================================================================

bool setupCameraSource(char *videoSource)
{
	//OutputDebugString("[VisionDLL] setupCameraSource : enter\n");

	//----reset parameters - testing 12/21/23
	framenumber = 0;
	framenumber_lastfaceNotFound = 0;
	framenumber_last_ct_gesturestart = 0;
	framenumber_last_er_gesturestart = 0;
	normBlurFeatureFramenumber = 0;
	normBlurFeatureFramenumber_cur = 0;
	normBlurFeatureFilenumber = 0;
	regionFramenumber = 0;
	gestureCount = 0;
	//--------------

	videoPlayer = Factory::createVideoPlayer();
    videoPlayer->setId("player");
    playlist = videoPlayer->getPlaylist();

	printf("\n-----------------------------------\n");
	printf("The following camera devices were found on your system:\n");

	std::vector<pair<int, string>> allDevices = CameraVideoSource::listDevices();
	int selectedCameraIdx = 0;
	if (allDevices.size() <= 0)
	{
		printf("ERROR: No camera devices foundon system - Please plug-in a camera and try again\n");
		return (false);
	}
	else 
	{
		for (int i=0; i<allDevices.size();i++)
		{
			printf("\t%d : %s\n", allDevices[i].first, allDevices[i].second.c_str());
		}
#ifdef USE_REALSENSE
			if (foundRealSenseRGBCamera() == false)
			{
				printf("ERROR: RealSense camera device not found on system - Please plug-in a RealSense camera and try again\n");
				return (false);
			}
#endif
	}
	printf("-----------------------------------\n");

#ifndef USE_REALSENSE
		if (videoSource == nullptr)
		{
			cout << "Initializing camera: " << selectedCameraIdx << " " << allDevices[selectedCameraIdx].second  << endl << endl;
		}
		else
		{
			selectedCameraIdx = getCameraIndex(videoSource);
			if (selectedCameraIdx < 0)
			{
				selectedCameraIdx = 0;
			}
		}
		mycamera = Factory::createCameraVideoSource(selectedCameraIdx);
		mycamera->openCamera(); //Sleep(1000);
#else
		cout << "Initializing RealSense camera" << endl << endl;
		mycamera = Factory::createRealSenseVideoSource(selectedCameraIdx);
		mycamera->setOutputResolution(640, 480);
		mycamera->open();		
#endif
		FrameRange videorange;
		playlist->addSourceRange(mycamera, videorange);
		//printf("Initialized camera\n");
	
	session->setVideoPlaylist(playlist);
	
	return (true);
}

//============================================================================

bool setupVideoSource()
{
	videoPlayer = Factory::createVideoPlayer();
    videoPlayer->setId("player");
    playlist = videoPlayer->getPlaylist();

	if (c_useCamera)
	{
		printf("\n-----------------------------------\n");
		printf("The following camera devices were found on your system:\n");

		std::vector<pair<int, string>> allDevices = CameraVideoSource::listDevices();
		int selectedCameraIdx = 0;
		if (allDevices.size() <= 0)
		{
			printf("ERROR: No camera devices found on system - Please plug-in a camera and try again\n");
			return (false);
		}
		else 
		{
			for (int i=0; i<allDevices.size();i++)
			{
				printf("\t%d : %s\n", allDevices[i].first, allDevices[i].second.c_str());
			}
#ifdef USE_REALSENSE
			if (foundRealSenseRGBCamera() == false)
			{
				printf("ERROR: RealSense camera device not found on system - Please plug-in a RealSense camera and try again\n");
				return (false);
			}
#endif
		}
		printf("-----------------------------------\n");
#ifndef USE_REALSENSE
		if (allDevices.size() > 1)
		{
			if ((c_cameraId >= 0) && (c_cameraId < allDevices.size()))
			{
				selectedCameraIdx = c_cameraId;
			}
			else
			{
				string strInput = "";
				while (true) 
				{
					cout << "\nPlease enter the camera ID (number) from the above list: ";
					getline(cin, strInput);
					stringstream strInputStream(strInput);
					if ((strInputStream >> selectedCameraIdx) && ((selectedCameraIdx >= 0) && (selectedCameraIdx < allDevices.size())))
						break;
					cout << "Invalid number, please try again" << endl;
				}
			}
		}
		cout << "Initializing camera: " << selectedCameraIdx << " " << allDevices[selectedCameraIdx].second  << endl << endl;
		mycamera = Factory::createCameraVideoSource(selectedCameraIdx);
		mycamera->openCamera(); //Sleep(1000);
		//printf("\tCamera Resolution = %d : %d",  width,height);
#else
		cout << "Initializing RealSense camera" << endl << endl;
		mycamera = Factory::createRealSenseVideoSource();
		mycamera->setOutputResolution(640, 480);
		mycamera->open();		
#endif
		FrameRange videorange;
		playlist->addSourceRange(mycamera, videorange);
		//printf("Initialized camera\n");
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
	
	return (true);
}

//============================================================================
#ifdef  USE_REALSENSE
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
	//printf("Initializing regions\n");
	//printf("\n========================================================================\n");
	 
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
		//printf("Initialized regions\n");
}

#endif

//============================================================================
void setupFaceRegionTracker(void)
{
//#ifdef  USE_DLIB
	float regionWidthExpansionMultiplier = 1.0f; //1.0f; 
	float regionHeightExpansionMultiplier = 1.0f; //1.0f;	
	
	float glassesFixStrength = 0.0f;
	int normFrames = -1; //10;	
	bool useAutoGlassesSearch=true;
	double resizeDownscale = c_dlibResizeFactor;

	bool cutBackground = c_cutBG; //either default or command-line argument
	float stabThreshold = c_stabThreshold; //either default or command-line argument
	

//#endif
	int regionOffsetX = 0;
	int regionOffsetY = 0;

	bool normalize = true;	

	unsigned int stabFrames = 10U; // 5U;

	int noFaceFrames = 30; 
	bool noFaceClockwiseRotation = false;
	
	
    bool glassesFixEnabled = false;
    bool useTemplateRegions = true;  
	

#ifdef NO_FACE_WORKAROUND
	noFaceFrames = -1;
	printf("\nNO Face workaround will be applied!!!\n\n");
#endif

#ifdef USE_REALSENSE
	setupRealSenseTracker(regionWidthExpansionMultiplier,regionHeightExpansionMultiplier, normalize,
                 stabThreshold, stabFrames, 
                 regionOffsetX, regionOffsetY, glassesFixEnabled,
                 useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, 
				 cutBackground);
#elif USE_DLIB
	setupDlibTracker(regionWidthExpansionMultiplier, regionHeightExpansionMultiplier, normalize,
                 stabThreshold, stabFrames, 
                 regionOffsetX, regionOffsetY, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled,
                 useTemplateRegions, glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);
#endif 

	printf("Face detection in progress...\n");

}

//============================================================================
void setupFeatureComputation_NormBlur()
{
	//feature 
    //normBlurFeature = new NormalFlowBlurredFeature(0.1f, NFBF_ENERGY_DX_DY, true);
	normBlurFeature = new NormalFlowBlurredFeature(0.0f, NFBF_ENERGY_DX_DY);
	normBlurFeature->setId("normFea");
	session->addFeature (normBlurFeature);
}

//============================================================================

void setupClassifier_MG()
{
	if (bRunMultiGestureSMClassifier == true)
	{
		//classifier
		classifierMG = new MultiGestureSMClassifier(6);
		classifierMG->setGestureLenMsec_ct(c_ct_gestureLenMsec_max, c_ct_gestureLenMsec_min,0, c_ct_gestureHoldMsec);
		classifierMG->setGestureLenMsec_er(c_er_gestureLenMsec_max, c_er_gestureLenMsec_min,0, c_er_gestureHoldMsec);
		
		session->addClassifier(classifierMG);
	}
}

//============================================================================

void setupGUI_singlePipeline()
{
	//printf("Setting up GUI framework\n");
	gui = Factory::createGUIFramework(pipeline_single);


/* SS: 05/12/2023 commenting out this code for new ACAT release 
#ifdef USE_REALSENSE
	strGUIWindowTitle += " RealSense Camera";
#else
	strGUIWindowTitle += " Integrated Camera";
#endif
*/


#ifdef USE_REALSENSE
	//strGUIWindowTitle += "";
#elif USE_DLIB
	//strGUIWindowTitle += " DLIB";
#endif
	gui->setWindowName(strGUIWindowTitle);

	if (bRunMultiGestureSMClassifier == true) 
		gui->setSensitivitySettings((int)(thresh_mg*100));
#ifdef ACATDLL //When using DLL 
#ifdef ASTER_SUPPORT
	gui->listenSocket(c_acatPort);
#endif
#endif //ACATDLL
	
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
	pipeline_single->addNodeCallback(classifierMG->getId(), &classifierMGCallback, NULL);

	if (c_FeaturesOut == true)
	{
		if (isNormBlurFeaActive)
			pipeline_single->addNodeCallback(normBlurFeature->getId(), &normBlurFeatureCallback_2, NULL);
	}
	setupGUI_singlePipeline();
}

//============================================================================
void setupVideoRecording()
{
	if (c_videoOut == true)
	{
		// SS 05/23
		captureFilename = c_outVideoFileName;
		if (c_outVideoFF == AVI_24)
			gui->enableVideoOutput(c_outVideoFileName, cv::VideoWriter::fourcc('D','I','B',' '), cv::Size(c_videoOutresolutionWidth, c_videoOutresolutionHeight), 60, c_outMaxFramesPerFile,VIDEOFILE_OUTPUT);
		else if (c_outVideoFF == AVI_8)
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
		//gui->disableVideoOutput();
		if (c_outVideoFF == AVI_24)
			gui->generateVideoOutput(captureFilename, cv::VideoWriter::fourcc('D', 'I', 'B', ' '));
		else if (c_outVideoFF == AVI_8)
			gui->generateVideoOutput(captureFilename, cv::VideoWriter::fourcc('X', 'V', 'I', 'D'));
	}
	else if( gui->haveVideoOutput() )
    {
		cout << "Writing captrured video to file " << captureFilename <<"\n";
		gui->generateVideoOutput(captureFilename, cv::VideoWriter::fourcc('D','I','B',' '));
	}
}

//============================================================================
void classifierMGCallback(Mat mat, void* param)
{  
    (void)param;

    ClassType classID = (ClassType)mat.at<float>(0, 0);
    switch(classID)
    {
	case MultiGestureSMClassifier::INIT_STARTUP:
		sendAsterEventsToLog(ASTER_EVENT::EVENT_INIT_DONE, GESTURE_ID::INIT);
		break;
	case MultiGestureSMClassifier::CALIBRATION_START:
		sendAsterEventsToLog(ASTER_EVENT::EVENT_CALIBRATION_START,GESTURE_ID::CALIBRATE);
        break;
    case MultiGestureSMClassifier::CALIBRATION_IN_PROGRESS:
        break;
    case MultiGestureSMClassifier::CALIBRATION_END:
		sendAsterEventsToLog(ASTER_EVENT::EVENT_CALIBRATION_END,GESTURE_ID::CALIBRATE);
        break;
	case MultiGestureSMClassifier::NOISE_FLOOR_IN_PROGRESS:
        break;
	case MultiGestureSMClassifier::NO_GESTURE:
        break;
	case MultiGestureSMClassifier::CHEEK_TWITCH_DOWN:
		sendAsterEventsToLog(ASTER_EVENT::EVENT_DOWN,GESTURE_ID::CT);
        break;
	case MultiGestureSMClassifier::CHEEK_TWITCH_DOWN_AND_TRIGGER:
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
        break;
	case MultiGestureSMClassifier::CHEEK_TWITCH_TRIGGER:
		if (c_action == ASTER_ACTION::ACTION_TRIGGER)
		{
			sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::CT);
			if (c_bIsBeep) Beep(3000,100);
		}
		else
		{
			sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::CT);
		}
        break;
	case MultiGestureSMClassifier::CHEEK_TWITCH_UP:
		sendAsterEventsToLog(ASTER_EVENT::EVENT_UP,GESTURE_ID::CT);
        break;
	case MultiGestureSMClassifier::EYEBROW_RAISE:
		if (c_action == ASTER_ACTION::ACTION_TRIGGER)
		{
			sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER,GESTURE_ID::ER);
			if (c_bIsBeep) Beep(3000,100);
		}
		else
		{
			sendAsterEventsToLog(ASTER_EVENT::EVENT_TRIGGER_LOGONLY,GESTURE_ID::ER);
		}
        break;
	case MultiGestureSMClassifier::MOUTH_PULL:
        break;
    default:
        break;
    }
};
//===============================================================================
MultiGestureSMClassifier::MultiGestureSMClassifier(int nRegions)
    {
        this->nRegions = nRegions;

		this->NOISE_FLOOR_FRAMECOUNT = 30; //10;	
		this->NOISE_STD_THRESHOLD = 0.1F; //5.0F;
		this->MIN_CT_DURATION = 2;
		
		this->CT_MOUTH_THRESH_UP = 0.2F; //0.3; 
		this->CT_MOUTH_THRESH_DOWN = 0.1F; //this->MOUTH_THRESH_UP - 0.1;
		this->ER_THRESH_UP = 0.1F; //0.3; 
		this->ER_THRESH_DOWN = 0.0F; //this->MOUTH_THRESH_UP - 0.1;
		
		//this->classifyState = SM_COMPUTE_NOISEFLOOR;
		this->classifyState = SM_STARTUP;

		this->noiseFloorDataMat = Mat(nRegions, NOISE_FLOOR_FRAMECOUNT, CV_32FC1, cv::Scalar(0) );
		this->meanNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->stdNoiseFloor = Mat(1,nRegions, CV_32FC1, cv::Scalar(0) );
		this->noiseFloorCurIdx = 0;
		this->curFrameCount = 0;
		this->prevFeature = Mat(1,nRegions*3,CV_32FC1,cv::Scalar(0));
		this->foundNoiseFloor = false;
		this->fps = 60;
		this->isGestureHold = false;

		this->CT_GESTURE_LEN_MSEC_MAX = 500; //420msec???
		this->CT_GESTURE_LEN_MSEC_MIN = 150;
		this->CT_GESTURE_SPACING_MSEC=0;
		this->CT_GESTURE_HOLD_MSEC = 0;
		this->CT_GESTURE_LEN_MAX = (int)(((float)this->CT_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->CT_GESTURE_LEN_MIN = (int)(((float)this->CT_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->CT_GESTURE_SPACE = (int)(((float)this->CT_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);	
		this->CT_GESTURE_HOLD = (int)(((float)this->CT_GESTURE_HOLD_MSEC * this->fps) / 1000.0f + 0.5);
		this->CT_SECOND_PEAK_START = this->CT_GESTURE_LEN_MAX-5;

		this->ER_GESTURE_LEN_MSEC_MAX = 1000; //420msec???
		this->ER_GESTURE_LEN_MSEC_MIN = 250;
		this->ER_GESTURE_SPACING_MSEC=0;
		this->ER_GESTURE_HOLD_MSEC = 0;
		this->ER_GESTURE_LEN_MAX = (int)(((float)this->ER_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->ER_GESTURE_LEN_MIN = (int)(((float)this->ER_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		this->ER_GESTURE_SPACE = (int)(((float)this->ER_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		this->ER_GESTURE_HOLD = (int)(((float)this->ER_GESTURE_HOLD_MSEC * this->fps) / 1000.0f + 0.5);
		
		
		//printf("\tCT: Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->CT_GESTURE_LEN_MSEC_MAX,this->CT_GESTURE_LEN_MAX,this->CT_GESTURE_LEN_MSEC_MIN,this->CT_GESTURE_LEN_MIN);
		//printf("\tCT: Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->CT_GESTURE_SPACING_MSEC, this->CT_GESTURE_SPACE);
		
		//printf("\tER: Default @60FPS Max gesture Len (%d msec, %d frames) / Min gesture len (%d msec, %d frames)\n",this->ER_GESTURE_LEN_MSEC_MAX,this->ER_GESTURE_LEN_MAX,this->ER_GESTURE_LEN_MSEC_MIN,this->ER_GESTURE_LEN_MIN);
		//printf("\tER: Default @60FPS spacing between consecutive gestures (%d msec, %d frames) \n",this->ER_GESTURE_SPACING_MSEC, this->ER_GESTURE_SPACE);

		gestureID = NO_GESTURE;
    }

//============================================================================
void MultiGestureSMClassifier::setFps(int curfps) 
	{
		lock_guard<mutex> lk(m_fps);
		this->fps = curfps;
	}

#ifndef SS_2023 //SS
void MultiGestureSMClassifier::setClassifierParameters(const char* paramName, double paramValue)
{
	lock_guard<mutex> lk(m_changeParams);
	{
		
		string command = paramName;
		if (command.find("CT_MIN_GESTURE_LEN") == 0)
		{
			this->CT_GESTURE_LEN_MSEC_MIN = (int)paramValue;
			setParamsBasedOnFps();
		}
		else if (command.find("HEAD_MOVEMENT_SENSITIVITY") == 0)
		{
			IDlib::Settings dlibSettings = dlibHandler->getHandlerSettings();
			dlibSettings.stabThreshold = static_cast<float>(paramValue);
			dlibHandler->setHandlerSettings(dlibSettings);
		}
		else if (command.find("CHEEK_MOVEMENT_SENSITIVITY") == 0)
		{
			setDetectionThreshold_ct(static_cast<float>(paramValue / 100.0));
		}
		else if (command.find("CHEEK_HOLD_MSEC") == 0)
		{
			this->CT_GESTURE_HOLD_MSEC = (int)paramValue;
			setParamsBasedOnFps();
		}
		else if (command.find("EYEBROW_MOVEMENT_SENSITIVITY") == 0)
		{
			setDetectionThreshold_er(static_cast<float>(paramValue / 100.0));
		}
		else if (command.find("EYEBROW_HOLD_MSEC") == 0)
		{
			this->ER_GESTURE_HOLD_MSEC = (int)paramValue;
			setParamsBasedOnFps();
		}
		
	}
	reset();
	//gui->sendCommandToGUI("action=RECALIBRATE", 0);
}
#endif

//============================================================================
void MultiGestureSMClassifier::setParamsBasedOnFps() 
{
		this->CT_GESTURE_LEN_MAX = (int)(((float)this->CT_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->CT_GESTURE_LEN_MIN = (int)(((float)this->CT_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		CT_SECOND_PEAK_START = CT_GESTURE_LEN_MAX - 5;
		this->CT_GESTURE_SPACE = (int)(((float)this->CT_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		this->CT_GESTURE_HOLD = (int)(((float)this->CT_GESTURE_HOLD_MSEC * this->fps) / 1000.0f + 0.5);
		//printf("CT: Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,CT_GESTURE_LEN_MAX, CT_GESTURE_LEN_MSEC_MAX, CT_GESTURE_LEN_MIN,CT_GESTURE_LEN_MSEC_MIN,this->CT_GESTURE_SPACE,CT_GESTURE_SPACING_MSEC);

		this->ER_GESTURE_LEN_MAX = (int)(((float)this->ER_GESTURE_LEN_MSEC_MAX * this->fps)/1000.0f + 0.5); //30;
		this->ER_GESTURE_LEN_MIN = (int)(((float)this->ER_GESTURE_LEN_MSEC_MIN * this->fps)/1000.0f + 0.5);
		//ER_SECOND_PEAK_START = GESTURE_LEN_MAX - 5;
		this->ER_GESTURE_SPACE = (int)(((float)this->ER_GESTURE_SPACING_MSEC * this->fps)/1000.0f + 0.5);
		this->ER_GESTURE_HOLD = (int)(((float)this->ER_GESTURE_HOLD_MSEC * this->fps) / 1000.0f + 0.5);
		//printf("ER Gesture-length in classifer for fps=%d set to: Max:%d(%d msec) Min:%d(%d msec) Spacing: %d(%d msec)\n", 
		//		fps,ER_GESTURE_LEN_MAX, ER_GESTURE_LEN_MSEC_MAX, ER_GESTURE_LEN_MIN,ER_GESTURE_LEN_MSEC_MIN,this->ER_GESTURE_SPACE,ER_GESTURE_SPACING_MSEC);

}

//============================================================================
void MultiGestureSMClassifier::setDetectionThreshold_ct(float thresh) 
	{
		CT_MOUTH_THRESH_UP = thresh;
		//printf("Detection Threshold set to %f\n", thresh);
		CT_MOUTH_THRESH_DOWN = CT_MOUTH_THRESH_UP - 0.1F;
		/*
		if (CT_MOUTH_THRESH_DOWN < 0.15F)
				CT_MOUTH_THRESH_DOWN = 0.15F;
		*/
		if (CT_MOUTH_THRESH_DOWN < 0.0F)
			CT_MOUTH_THRESH_DOWN = 0.0F;
	}
//============================================================================
string MultiGestureSMClassifier::getDetectionThreshold_ct()
{
	std::stringstream thresh;
	thresh << CT_MOUTH_THRESH_UP;
	thresh << ":";
	thresh << CT_MOUTH_THRESH_DOWN;
	return(thresh.str());
}
//============================================================================
string MultiGestureSMClassifier::getDetectionThreshold_er()
{
	std::stringstream thresh;
	thresh << ER_THRESH_UP;
	thresh << ":";
	thresh << ER_THRESH_DOWN;
	return(thresh.str());
}


//============================================================================
void MultiGestureSMClassifier::setDetectionThreshold_er(float thresh_er) 
{
	ER_THRESH_UP = thresh_er;
	//printf("Detection Threshold set to %f\n", thresh);
	ER_THRESH_DOWN = ER_THRESH_UP - 0.1F;
	/*
	if (ER_THRESH_DOWN < 0.15F)
			ER_THRESH_DOWN = 0.15F;
			*/
	if (ER_THRESH_DOWN < 0.0F)
		ER_THRESH_DOWN = 0.0F;
}

//============================================================================
void MultiGestureSMClassifier::setNoiseThreshold(float thresh) 
{
	NOISE_STD_THRESHOLD = thresh;
	//printf("Detection Threshold set to %f\n", thresh);
}
void MultiGestureSMClassifier::setGestureLen(int lenMax, int lenMin) 
{
	CT_GESTURE_LEN_MAX = lenMax;
	CT_GESTURE_LEN_MIN = lenMin;
	//printf("Gesture len set to %d\n", lenMax);
}

//============================================================================
void MultiGestureSMClassifier::setGestureLenMsec_ct(int lenMsecMax, int lenMsecMin, int spacingMsec, int holdMsec) 
{
	CT_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	CT_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	CT_GESTURE_SPACING_MSEC = spacingMsec;
	CT_GESTURE_HOLD_MSEC = holdMsec;
	//printf("\tCT Gesture len Msec set to %d:%d, spacing set to %d Msec\n", lenMsecMin, lenMsecMax, spacingMsec);
	//printf("\tCT Gesture len Msec set to %d:%d Msec\n", lenMsecMin, lenMsecMax);
}

//============================================================================
void MultiGestureSMClassifier::setGestureLenMsec_er(int lenMsecMax, int lenMsecMin, int spacingMsec, int holdMsec)
{
	ER_GESTURE_LEN_MSEC_MAX = lenMsecMax;
	ER_GESTURE_LEN_MSEC_MIN = lenMsecMin;
	ER_GESTURE_SPACING_MSEC = spacingMsec;
	ER_GESTURE_HOLD_MSEC = holdMsec;
	//printf("\tER Gesture len Msec set to %d:%d Msec\n", lenMsecMin, lenMsecMax);
}

//============================================================================
void MultiGestureSMClassifier::reset()
	{
		lock_guard<mutex> lk(m_reset);
		{
			if (classifyState != SM_STARTUP)
			classifyState = SM_COMPUTE_NOISEFLOOR;
			noiseFloorDataMat.setTo(cv::Scalar(0));
			meanNoiseFloor.setTo(cv::Scalar(0));
			stdNoiseFloor.setTo(cv::Scalar(0));
			noiseFloorCurIdx = 0;
			this->curFrameCount = 0;
		}
	}

//============================================================================
ClassType MultiGestureSMClassifier::clasify(Mat feature)
    {
		ClassType ret = (ClassType)classifySM(feature);
		return (ret);
    }

//============================================================================
int MultiGestureSMClassifier::classifySM(Mat feature)
	{
	try
	{
		lock_guard<mutex> lk(m_classify);

		int classReturnID = NO_GESTURE;
		curFrameCount++;

		float mouthNF = meanNoiseFloor.at<float>(MOUTH);;
		float eyebrowNF = meanNoiseFloor.at<float>(R_EYEBROW);
		float eyebrowNF_L = meanNoiseFloor.at<float>(L_EYEBROW);

		switch (classifyState)
		{
		case (SM_STARTUP):
		{
			gestureID = INIT_STARTUP;
			classReturnID = INIT_STARTUP;

			curFrameCount = 0;
			classifyState = SM_COMPUTE_NOISEFLOOR;
		}
		break;
			case (SM_COMPUTE_NOISEFLOOR):
				{
					foundNoiseFloor = true;
					
					gestureID = NOISE_FLOOR_IN_PROGRESS;
					classReturnID = CALIBRATION_IN_PROGRESS;

					if (curFrameCount == 1)
					{
						classReturnID = CALIBRATION_START;
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
						classReturnID = CALIBRATION_END;

						printf("\n-----------------------------------------------------\n");
						printf("Calibration complete \n");
						printf("-----------------------------------------------------\n");
						/*
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
						*/
					}
				}
				break;
			case (SM_SEARCH_START):
				//search for up transition
				
				//setDetectionThreshold_ct((float)(gui->getCurrentSensitivity())/100.0f);
				//setDetectionThreshold_er((float)(gui->getCurrentSensitivity())/100.0f);
				
				gestureID = NO_GESTURE;
				classReturnID = NO_GESTURE;

				startFrame=-1;
				peak_up=0;
				peaks=0;
				if ( (feature.at<float>(MOUTH_POS) >= (mouthNF+CT_MOUTH_THRESH_UP)) && (prevFeature.at<float>(MOUTH_POS) < (mouthNF+CT_MOUTH_THRESH_UP)) )
				{
					startFrame = curFrameCount;
					peak_up = 1;
					classifyState = SM_CT_POSSIBLE_START;
				}
				else if ( ( (feature.at<float>(R_EYEBROW_POS) >= (eyebrowNF+ER_THRESH_UP)) && (prevFeature.at<float>(R_EYEBROW_POS) < (eyebrowNF+ER_THRESH_UP)) )
						|| ((feature.at<float>(L_EYEBROW_POS) >= (eyebrowNF_L + ER_THRESH_UP)) && (prevFeature.at<float>(L_EYEBROW_POS) < (eyebrowNF_L + ER_THRESH_UP))) )
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
				classReturnID = NO_GESTURE;

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
		   
					   printf("Cheek Gesture detected: #%d %f %f\n", framenumber, mouthNF, mouthNF + CT_MOUTH_THRESH_UP);

					   classReturnID = CHEEK_TWITCH_DOWN;
						if (CT_GESTURE_LEN_MIN <= MIN_CT_DURATION) 
						{
							classReturnID = CHEEK_TWITCH_DOWN_AND_TRIGGER;
						}					   
						if ((CT_GESTURE_HOLD > 0) && isGestureHold)
						{
							//if follow up gesture detected before hold time, then do not declare gesture
							if ((framenumber - framenumber_last_ct_gesturestart) < CT_GESTURE_HOLD)
							{
								printf("Possibly Cheek gesture after hold not declaring: #%d %d %d %d\n", framenumber, startFrame, framenumber_last_ct_gesturestart, CT_GESTURE_HOLD);
								gestureID = NO_GESTURE;
								classReturnID = NO_GESTURE;
							}
							else
								framenumber_last_ct_gesturestart = framenumber;
							isGestureHold = false;
						}
					}           
				}
				break;
			case (SM_ER_POSSIBLE_START):
				//find down transition
				gestureID = NO_GESTURE;
				classReturnID = NO_GESTURE;

				if ((curFrameCount-startFrame+1) <= ER_GESTURE_LEN_MIN)
				{
					if ( (feature.at<float>(R_EYEBROW_POS) < (eyebrowNF+ER_THRESH_UP))
						|| (feature.at<float>(L_EYEBROW_POS) < (eyebrowNF_L + ER_THRESH_UP)) )
					{
					   classifyState = SM_SEARCH_START; //false start
					}
					else if ((curFrameCount-startFrame+1) == ER_GESTURE_LEN_MIN)
					{
					   classifyState = SM_ER_END;
					   //declare found gesture 
					   gestureID = EYEBROW_RAISE;
					   classReturnID = EYEBROW_RAISE;

					   printf("Eyebrow Raise detected: #%d %f %f\n", framenumber, eyebrowNF, eyebrowNF + ER_THRESH_UP);
					   if ((ER_GESTURE_HOLD > 0) && isGestureHold)
					   {
						   //if follow up gesture detected before hold time, then do not declare gesture
						   if ((framenumber - framenumber_last_er_gesturestart) < ER_GESTURE_HOLD)
						   {
							   printf("Possibly Eyebrow gesture after hold not declaring: #%d\n", framenumber);
							   gestureID = NO_GESTURE;
							   classReturnID = NO_GESTURE;
						   }
						   else
							   framenumber_last_er_gesturestart = framenumber;
						   isGestureHold = false;
					   }
					   
					}           
				}
				else
				{
					classifyState = SM_ER_END;
					   //declare found gesture 
					gestureID = EYEBROW_RAISE;
					classReturnID = EYEBROW_RAISE;

					//printf("** Eyebrow Raise:   Start frame=%d\n", framenumber);

					printf("\tEyebrow Raise #%d\n", framenumber);
					if (ER_GESTURE_HOLD > 0)
					{
						//if follow up gesture detected before hold time, then do not declare gesture
						if ((framenumber - framenumber_last_er_gesturestart) < ER_GESTURE_HOLD)
						{
							printf("Possibly Eyebrow gesture after hold not declaring: #%d\n", framenumber);
							gestureID = NO_GESTURE;
							classReturnID = NO_GESTURE;
						}
						else
							framenumber_last_er_gesturestart = framenumber;
					}
				}
				break;
			case (SM_CT_START_CONFIRMED_FIND_END):
				if ( ((curFrameCount-startFrame+1) > CT_GESTURE_LEN_MAX)  && (peak_up == 0)) {
					classifyState = SM_SEARCH_START; //declare end of gesture
					isGestureHold = true;
				}
				else if ( (peak_up==1) && ( (feature.at<float>(MOUTH_POS) < (mouthNF+CT_MOUTH_THRESH_DOWN)) && (prevFeature.at<float>(MOUTH_POS)>=(mouthNF+CT_MOUTH_THRESH_DOWN))) )
				{
					//found crossing
					peak_up=0;
					peaks = peaks+1;					
					if (peaks == 2)
						classifyState = SM_SEARCH_START; //declare end of gesture
					isGestureHold = false;
				}
				else if ( (peak_up==0) && ( (feature.at<float>(MOUTH_POS)>=(mouthNF+CT_MOUTH_THRESH_UP)) & (prevFeature.at<float>(MOUTH_POS)<(mouthNF+CT_MOUTH_THRESH_UP))) ) 
				{
					if ((curFrameCount-startFrame) < CT_SECOND_PEAK_START)
						//second peak
						peak_up=1;
				}
				if (classifyState == SM_SEARCH_START)
				{
					if ((curFrameCount-startFrame+1) > CT_GESTURE_LEN_MIN)
					{
						if (gestureID!=NO_GESTURE)
							classReturnID = CHEEK_TWITCH_UP;
						
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
						if (gestureID != NO_GESTURE)
							classReturnID = CHEEK_TWITCH_TRIGGER;
					}			
				}

				break;
			case SM_ER_END:
				isGestureHold = true;
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
		//return (gestureID);
		return (classReturnID);
	}
	catch(exception&) 
	{
		IVCP_EXCEPTION(IncorrectParamException, "Classifier exception");
	}

	
}

//============================================================================
int getCameraIndex(char *cameraName)
{
	std::vector<pair<int, string>> allDevices = CameraVideoSource::listDevices();
	if (allDevices.size() <= 0)
	{
		return -1;
	}

	for (int i=0; i<allDevices.size();i++)
	{
		if (_strcmpi(cameraName, allDevices[i].second.c_str()) == 0)
		{
			return i;
		}
	}

	return -1;
}

//============================================================================
bool foundRealSenseRGBCamera()
{		
	string cameraRS = "RealSense";

	std::vector<pair<int, string>> allDevices = CameraVideoSource::listDevices();
	if (allDevices.size() <= 0)
	{
		return false;
	}

	for (int i=0; i<allDevices.size();i++)
	{
		if (strstr(allDevices[i].second.c_str(), cameraRS.c_str()) != NULL)
			return (true);
	}

	return false;
}

//============================================================================