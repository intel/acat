///////////////////////////////////////////////////////////////////////////
// <copyright file="acat_gestures_common.hpp" company="Intel Corporation">
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


#define NSEQUENCES 6 // # of regions
//--------------Default settings

#define DEFAULT_BEEP						 false
#define DEFAULT_REGION_PARAM_CUT_BACKGROUND  true
#define DEFAULT_REGION_PARAM_STAB_THRESHOLD 40.0F
#define DEFAULT_DLIBRESIZEFACTOR            0.5

#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_NOISE_THRESHOLD       10.0F
#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTURELENMSEC_MIN 150
#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTURELENMSEC_MAX 300 //500
#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTURESPACINGMSEC 0 //sec
#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_CT_GESTUREHOLDMSEC 0 //sec

#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_ER_GESTURELENMSEC_MAX 500
#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_ER_GESTURELENMSEC_MIN 250
#define DEFAULT_MULTI_GESTURE_CLASSIFIER_PARAM_ER_GESTUREHOLDMSEC 0

#define DEFAULT_VIDEOOUT                             false
#define DEFAULT_VIDEOOUT_PARAM_MAX_FRAMES_PER_FILE   0
#define DEFAULT_VIDEOOUT_PARAM_RESOLUTION_WIDTH      1280
#define DEFAULT_VIDEOOUT_PARAM_RESOLUTION_HEIGHT     1024
#define DEFAULT_VIDEOOUT_PARAM_OUTDIR                "C:\\Data\\"
#define DEFAULT_VIDEOOUT_PARAM_VIDEO_FILENAME  "fileVidRec.avi"

//ASTER_ACTION DEFAULT_ASTER_ACTION = ASTER_ACTION::ACTION_TRIGGER;
#define DEAFULT_ASTERSYNCFRAMES  5000
#define DEAFULT_ACATPORT         27015

enum {NONE=-1, RAW_16=0, AVI_24=1, AVI_8=2};

typedef enum  {ACTION_LOG=0, ACTION_TRIGGER=2} ASTER_ACTION;
typedef enum  {EVENT_EXCEPTION=-1,
				EVENT_DOWN=0, 
				EVENT_UP=1, 
				EVENT_TRIGGER=2, 
				EVENT_TRIGGER_LOGONLY=3, 
				EVENT_SYNC=4, 
				EVENT_CALIBRATION_START=5, 
				EVENT_CALIBRATION_END=6,
				EVENT_CALIBRATION_INPROGRESS=7,
				EVENT_INIT_DONE=8
			} ASTER_EVENT;
typedef enum  {INIT=-1, CALIBRATE=0, CT=1, ER=2} GESTURE_ID;


//------------------


void alarm();
int computeFps();

void sourceCallback(Mat mat, void* param);
void checkNormBlurFeatureFileOpen();
void normBlurFeatureCallback_2(cv::Mat mat, void *param);
void regionCallback(Mat mat, void* param);
void classifierMGCallback(Mat mat, void* param);
void processCommandLineArgs(int argc, char **argv);
bool setupVideoSource();
bool setupCameraSource(char *cameraSource = nullptr);
int getCameraIndex(char *cameraName);
bool foundRealSenseRGBCamera();
#ifdef USE_REALSENSE
void setupRealSenseTracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, bool glassesFixEnabled,
                 bool useTemplateRegions, float glassesFixStrength, bool useAutoGlassesSearch, 
				 bool cutBackground);
#elif USE_DLIB
void setupDlibTracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled,
                 bool useTemplateRegions, float glassesFixStrength, bool useAutoGlassesSearch, 
				 bool cutBackground, double resizeDownscale);
#endif
void setupFaceRegionTracker(void);
void setupFeatureComputation_NormBlur();
void setupClassifier_MG();
void setupGUI_singlePipeline();
void setupSinglePipeline();
void setupVideoRecording();
void stopVideoRecording();
void sendAsterEventsToLog(ASTER_EVENT eventID, GESTURE_ID gestureID);
void sendCommandToGUI(char *str, double value);
void showVideo();
void hideVideo();
bool isVideoVisible();


class MultiGestureSMClassifier : public Classifier
{
public:
	MultiGestureSMClassifier(int nRegions);

	virtual vector<float> getDistribution(Mat feature) { (void)feature; return vector<float>(); }
	virtual void addTrainFeature(Mat feature, ClassType response) { (void)feature; (void)response; }
	virtual void trainModel() { }
	virtual ClassifierType getType() { return NEW_CLASSIFIER; }
	virtual bool isTrained() { return true; }
	virtual int getVariablesCount() { return NSEQUENCES; }
	virtual int getClassCount() { return 2; };

	virtual void setFps(int fps);

	virtual void setDetectionThreshold_ct(float thresh);
	virtual void setDetectionThreshold_er(float thresh);

	virtual std::string getDetectionThreshold_ct(void);
	virtual std::string getDetectionThreshold_er(void);

	virtual void setNoiseThreshold(float thresh);

	virtual void setGestureLen(int lenMax, int lenMin);

	virtual void setGestureLenMsec_ct(int lenMsecMax, int lenMsecMin, int spacingMsec, int holdMsec);
	virtual void setGestureLenMsec_er(int lenMsecMax, int lenMsecMin, int spacingMsec, int holdMsec);

	virtual void reset();

	virtual void setParamsBasedOnFps();

#ifndef SS_2023 //SS
	virtual void setClassifierParameters(const char *paramName, double paramValue);
#endif


	enum {
		INIT_STARTUP = -5,
		CALIBRATION_START = -4,
		CALIBRATION_IN_PROGRESS = -3,
		CALIBRATION_END = -2,
		NOISE_FLOOR_IN_PROGRESS = -1,
		NO_GESTURE = 0,
		CHEEK_TWITCH = 1,
		CHEEK_TWITCH_DOWN = 101,
		CHEEK_TWITCH_DOWN_AND_TRIGGER = 102,
		CHEEK_TWITCH_TRIGGER = 103,
		CHEEK_TWITCH_UP = 104,
		EYEBROW_RAISE = 2,
		MOUTH_PULL = 3
	};

protected:
	virtual ClassType clasify(Mat feature);

private:

	//constants
	int STEADY_FRAMES;
	int NOISE_FLOOR_FRAMECOUNT;
	float NOISE_STD_THRESHOLD;
	int MIN_CT_DURATION;

	int CT_GESTURE_LEN_MSEC_MAX;
	int CT_GESTURE_LEN_MSEC_MIN;
	int CT_GESTURE_SPACING_MSEC;
	int CT_GESTURE_HOLD_MSEC;
	int CT_GESTURE_LEN_MAX;
	int CT_GESTURE_LEN_MIN;
	int CT_GESTURE_SPACE;
	int CT_GESTURE_HOLD;
	int CT_GESTURE_LEN_MAX_60FPS;
	int CT_GESTURE_LEN_MIN_60FPS;
	int CT_SECOND_PEAK_START;
	float CT_MOUTH_THRESH_UP;
	float CT_MOUTH_THRESH_DOWN;

	int ER_GESTURE_LEN_MSEC_MAX;
	int ER_GESTURE_LEN_MSEC_MIN;
	int ER_GESTURE_SPACING_MSEC;
	int ER_GESTURE_HOLD_MSEC;
	int ER_GESTURE_LEN_MAX;
	int ER_GESTURE_LEN_MIN;
	int ER_GESTURE_SPACE;
	int ER_GESTURE_HOLD;
	int ER_GESTURE_LEN_MAX_60FPS;
	int ER_GESTURE_LEN_MIN_60FPS;
	float ER_THRESH_UP;
	float ER_THRESH_DOWN;

	int gestureID;

	//variables
	int nRegions;
	int classifyState;
	Mat noiseFloorDataMat;
	int noiseFloorCurIdx;
	int curFrameCount;
	Mat meanNoiseFloor;
	Mat stdNoiseFloor;
	Mat prevFeature;
	bool foundNoiseFloor;
	int fps;
	bool isGestureHold;

	//state machine parameters
	int startFrame;
	int peak_up;
	int peaks;

	//lock
	mutex m_classify;
	mutex m_reset;
	mutex m_fps;
	mutex m_changeParams;


	enum { R_EYEBROW = 0, L_EYEBROW = 1, R_CHEEK = 2, L_CHEEK = 3, MOUTH = 4, NOSE = 5 };

	enum { R_EYEBROW_POS = 0, L_EYEBROW_POS = 3, R_CHEEK_POS = 6, L_CHEEK_POS = 9, MOUTH_POS = 12, NOSE_POS = 15 };

	enum {
		SM_STARTUP = -1,
		SM_COMPUTE_NOISEFLOOR = 0,
		SM_SEARCH_START,
		SM_CT_POSSIBLE_START,
		SM_CT_START_CONFIRMED_FIND_END,
		SM_ER_POSSIBLE_START,
		SM_ER_END,
		SM_CT_WAIT_BETWEEN_GESTURES
	};

	enum { NO_MOTION = 0, HAS_MOTION };

	int classifySM(Mat feature);

}; //class 