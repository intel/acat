

#define NSEQUENCES 6 // # of regions

enum {NONE=-1, RAW_16=0, AVI_24=1, AVI_8=2};

typedef enum  {ACTION_LOG=0, ACTION_TRIGGER=2} ASTER_ACTION;
typedef enum  {EVENT_DOWN=0, EVENT_UP=1, EVENT_TRIGGER=2, EVENT_TRIGGER_LOGONLY=3, EVENT_SYNC=4} ASTER_EVENT;
typedef enum  {CT=1, ER=2, MP=3} GESTURE_ID;

void alarm();
int computeFps();

void sourceCallback(Mat mat, void* param);
void classifierCallback(Mat mat, void* param);
void classifierHMMCallback(Mat mat, void* param);
void edFeatureCallback(cv::Mat mat, void *param);
void checkEDFeatureFileOpen();
void edFeatureCallback_2(cv::Mat mat, void *param);
void normBlurFeatureCallback(cv::Mat mat, void *param);
void checkNormBlurFeatureFileOpen();
void normBlurFeatureCallback_2(cv::Mat mat, void *param);
void myCameraCallback(bool status, void* context);
void regionCallback(Mat mat, void* param);
void processCommandLineArgs(int argc, char **argv);
void setupVideoSource(void);
#ifdef USE_FACEAPI
void setupFaceAPITracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 int pollTimeout, bool realtime, float framerate, float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled,
                 bool useFaceAPITemplateRegions, float glassesFixStrength, TypeOfTransform transformType, bool useAutoGlassesSearch, int triggerWA);
#elif USE_REALSENSE
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
#else
void setupASMTracker(float regionWidthExpansionMultiplier, float regionHeightExpansionMultiplier, bool normalize,
                 float stabThreshold, unsigned int stabFrames, 
                 int regionOffsetX, int regionOffsetY, int normFrames, int noFaceFrames, bool noFaceClockwiseRotation, bool glassesFixEnabled,
                 bool useTemplateRegions, float glassesFixStrength, bool useAutoGlassesSearch, 
				 bool cutBackground, double maxTrackLifeTime, int minFaceSize, bool useGeneralTracker);
#endif
void setupFaceRegionTracker(void);
void setupFeatureComputation_ED();
void setupFeatureComputation_NormBlur();
void setupClassifier();
void setupClassifier_ED_NormBlur();
void setupClassifier_ED_ER_MP();
void setupClassifier_MG();
void setupHMMClassifier();
void setupGUI_singlePipeline();
void setupGUI_parallelPipeline();
void setupSinglePipeline();
void setupParallelPipeline();
void setupVideoRecording();
void stopVideoRecording();

class CheekTwitchDetectionClassifier: public Classifier
{
public:
    CheekTwitchDetectionClassifier(int nRegions);

    virtual vector<float> getDistribution(Mat feature) { (void)feature; return vector<float>(); }
    virtual void addTrainFeature(Mat feature, ClassType response) { (void)feature; (void)response; }
    virtual void trainModel() { }
    virtual ClassifierType getType() { return NEW_CLASSIFIER; }
    virtual bool isTrained() { return true; }
    virtual int getVariablesCount() { return NSEQUENCES; }
    virtual int getClassCount() { return 2; };
	
	virtual void setFps(int fps) ;

	virtual void setDetectionThreshold(float thresh) ;

	virtual void setNoiseThreshold(float thresh) ;

	virtual void setGestureLen(int lenMax, int lenMin);

	virtual void setGestureLenMsec(int lenMsecMax, int lenMsecMin, int spacingMsec);

	virtual void reset();

	virtual void setParamsBasedOnFps();

protected:
    virtual ClassType clasify(Mat feature);

private:
    
	//constants
	int STEADY_FRAMES;
	int NOISE_FLOOR_FRAMECOUNT;
	float NOISE_STD_THRESHOLD;
	int MIN_CT_DURATION;
	//int SKIP_FRAMES_MAX;
	//int SKIP_FRAMES_MIN;
	int GESTURE_LEN_MSEC_MAX;
	int GESTURE_LEN_MSEC_MIN;
	int GESTURE_SPACING_MSEC;
	int GESTURE_LEN_MAX;
	int GESTURE_LEN_MIN;
	int GESTURE_SPACE;
	int GESTURE_LEN_MAX_60FPS;
	int GESTURE_LEN_MIN_60FPS;
	int TOO_FAST; 
	int SECOND_PEAK_START;
	float MOUTH_THRESH_UP;
	float MOUTH_THRESH_DOWN;
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

	//state machine parameters
	int startFrame;
	int peak_up;
	int peaks;

	//lock
	mutex m_classify;
	mutex m_reset;
	mutex m_fps;

	enum {R_EYEBROW=0, L_EYEBROW=1, R_CHEEK=2, L_CHEEK=3, MOUTH=4, NOSE=5};

	enum {SM_COMPUTE_NOISEFLOOR=0, SM_SEARCH_START, SM_POSSIBLE_START, SM_START_CONFIRMED_FIND_END, SM_WAIT_BETWEEN_GESTURES};

    enum { NO_MOTION = 0, HAS_MOTION };

	enum { NOISE_FLOOR_IN_PROGRESS=-1, NO_GESTURE=0, CHEEK_TWITCH=1};

	int classifySM(Mat feature);

}; //class CheekTwitchDetectionClassifier


class CheekTwitchDetectionClassifier_NormBlur: public Classifier
{
public:
    CheekTwitchDetectionClassifier_NormBlur(int nRegions);

    virtual vector<float> getDistribution(Mat feature) { (void)feature; return vector<float>(); }
    virtual void addTrainFeature(Mat feature, ClassType response) { (void)feature; (void)response; }
    virtual void trainModel() { }
    virtual ClassifierType getType() { return NEW_CLASSIFIER; }
    virtual bool isTrained() { return true; }
    virtual int getVariablesCount() { return NSEQUENCES; }
    virtual int getClassCount() { return 2; };
	
	virtual void setFps(int fps) ;

	virtual void setDetectionThreshold(float thresh) ;

	virtual void setNoiseThreshold(float thresh) ;

	virtual void setGestureLen(int lenMax, int lenMin);

	virtual void setGestureLenMsec(int lenMsecMax, int lenMsecMin, int spacingMsec);

	virtual void reset();

	virtual void setParamsBasedOnFps();

protected:
    virtual ClassType clasify(Mat feature);

private:
    
	//constants
	int STEADY_FRAMES;
	int NOISE_FLOOR_FRAMECOUNT;
	float NOISE_STD_THRESHOLD;
	int MIN_CT_DURATION;
	//int SKIP_FRAMES_MAX;
	//int SKIP_FRAMES_MIN;
	int GESTURE_LEN_MSEC_MAX;
	int GESTURE_LEN_MSEC_MIN;
	int GESTURE_SPACING_MSEC;
	int GESTURE_LEN_MAX;
	int GESTURE_LEN_MIN;
	int GESTURE_SPACE;
	int GESTURE_LEN_MAX_60FPS;
	int GESTURE_LEN_MIN_60FPS;
	int TOO_FAST; 
	int SECOND_PEAK_START;
	float MOUTH_THRESH_UP;
	float MOUTH_THRESH_DOWN;
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

	//state machine parameters
	int startFrame;
	int peak_up;
	int peaks;

	//lock
	mutex m_classify;
	mutex m_reset;
	mutex m_fps;

	enum {R_EYEBROW=0, L_EYEBROW=1, R_CHEEK=2, L_CHEEK=3, MOUTH=4, NOSE=5};

	enum {R_EYEBROW_POS=0, L_EYEBROW_POS=3, R_CHEEK_POS=6, L_CHEEK_POS=9, MOUTH_POS=12, NOSE_POS=15};

	enum {SM_COMPUTE_NOISEFLOOR=0, SM_SEARCH_START, SM_POSSIBLE_START, SM_START_CONFIRMED_FIND_END,SM_WAIT_BETWEEN_GESTURES};

    enum { NO_MOTION = 0, HAS_MOTION };

	enum { NOISE_FLOOR_IN_PROGRESS=-1, NO_GESTURE=0, CHEEK_TWITCH=1};

	int classifySM(Mat feature);

}; //class CheekTwitchDetectionClassifier


class EyebrowraiseMouthpullDetectionClassifier: public Classifier
{
public:
    EyebrowraiseMouthpullDetectionClassifier(int nRegions);

    virtual vector<float> getDistribution(Mat feature) { (void)feature; return vector<float>(); }
    virtual void addTrainFeature(Mat feature, ClassType response) { (void)feature; (void)response; }
    virtual void trainModel() { }
    virtual ClassifierType getType() { return NEW_CLASSIFIER; }
    virtual bool isTrained() { return true; }
    virtual int getVariablesCount() { return NSEQUENCES; }
    virtual int getClassCount() { return 2; };
	
	virtual void setFps(int fps) ;

	virtual void setDetectionThreshold_er(float thresh_er) ;

	virtual void setDetectionThreshold_mp(float thresh_mp) ;

	virtual void setNoiseThreshold(float thresh) ;

	//virtual void setGestureLen(int lenMax, int lenMin);

	virtual void setGestureLenMsec_er(int lenMsecMax, int lenMsecMin, int spacingMsec);

	virtual void setGestureLenMsec_mp(int lenMsecMax, int lenMsecMin, int spacingMsec);

	virtual void reset();

	virtual void setParamsBasedOnFps();

protected:
    virtual ClassType clasify(Mat feature);

private:
    
	//constants
	int STEADY_FRAMES;
	int NOISE_FLOOR_FRAMECOUNT;
	float NOISE_STD_THRESHOLD;
	int MIN_CT_DURATION;

	int ER_GESTURE_LEN_MSEC_MAX;
	int ER_GESTURE_LEN_MSEC_MIN;
	int ER_GESTURE_SPACING_MSEC;
	int ER_GESTURE_LEN_MAX;
	int ER_GESTURE_LEN_MIN;
	int ER_GESTURE_SPACE;
	int ER_GESTURE_LEN_MAX_60FPS;
	int ER_GESTURE_LEN_MIN_60FPS;
	float ER_THRESH_UP;
	float ER_THRESH_DOWN;

	int MP_GESTURE_LEN_MSEC_MAX;
	int MP_GESTURE_LEN_MSEC_MIN;
	int MP_GESTURE_SPACING_MSEC;
	int MP_GESTURE_LEN_MAX;
	int MP_GESTURE_LEN_MIN;
	int MP_GESTURE_SPACE;
	int MP_GESTURE_LEN_MAX_60FPS;
	int MP_GESTURE_LEN_MIN_60FPS;
	float MP_THRESH_UP;
	float MP_THRESH_DOWN;

	int TOO_FAST; 
	int SECOND_PEAK_START;
	
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

	//state machine parameters
	int startFrame;
	int peak_up;
	int peaks;

	//lock
	mutex m_classify;
	mutex m_reset;
	mutex m_fps;

	enum {R_EYEBROW=0, L_EYEBROW=1, R_CHEEK=2, L_CHEEK=3, MOUTH=4, NOSE=5};

	enum {R_EYEBROW_POS=0, L_EYEBROW_POS=3, R_CHEEK_POS=6, L_CHEEK_POS=9, MOUTH_POS=12, NOSE_POS=15};

	enum {
		SM_COMPUTE_NOISEFLOOR=0, 
		SM_SEARCH_START, 
		SM_ER_POSSIBLE_START, 
		SM_ER_END,
		SM_MP_POSSIBLE_START, 
		SM_MP_END
	};

    enum { NO_MOTION = 0, HAS_MOTION };

	enum { NOISE_FLOOR_IN_PROGRESS=-1, NO_GESTURE=0, CHEEK_TWITCH=1, EYEBROW_RAISE=2, MOUTH_PULL=3};

	int classifySM(Mat feature);

}; //class CheekTwitchDetectionClassifier

class MultiGestureSMClassifier: public Classifier
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
	
	virtual void setFps(int fps) ;

	virtual void setDetectionThreshold_ct(float thresh) ;
	virtual void setDetectionThreshold_er(float thresh) ;
	virtual void setDetectionThreshold_mp(float thresh) ;

	virtual void setNoiseThreshold(float thresh) ;

	virtual void setGestureLen(int lenMax, int lenMin);

	virtual void setGestureLenMsec_ct(int lenMsecMax, int lenMsecMin, int spacingMsec);
	virtual void setGestureLenMsec_er(int lenMsecMax, int lenMsecMin, int spacingMsec);
	virtual void setGestureLenMsec_mp(int lenMsecMax, int lenMsecMin, int spacingMsec);

	virtual void reset();

	virtual void setParamsBasedOnFps();

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
	int CT_GESTURE_LEN_MAX;
	int CT_GESTURE_LEN_MIN;
	int CT_GESTURE_SPACE;
	int CT_GESTURE_LEN_MAX_60FPS;
	int CT_GESTURE_LEN_MIN_60FPS;
	int CT_TOO_FAST; 
	int CT_SECOND_PEAK_START;
	float CT_MOUTH_THRESH_UP;
	float CT_MOUTH_THRESH_DOWN;

	int ER_GESTURE_LEN_MSEC_MAX;
	int ER_GESTURE_LEN_MSEC_MIN;
	int ER_GESTURE_SPACING_MSEC;
	int ER_GESTURE_LEN_MAX;
	int ER_GESTURE_LEN_MIN;
	int ER_GESTURE_SPACE;
	int ER_GESTURE_LEN_MAX_60FPS;
	int ER_GESTURE_LEN_MIN_60FPS;
	float ER_THRESH_UP;
	float ER_THRESH_DOWN;

	int MP_GESTURE_LEN_MSEC_MAX;
	int MP_GESTURE_LEN_MSEC_MIN;
	int MP_GESTURE_SPACING_MSEC;
	int MP_GESTURE_LEN_MAX;
	int MP_GESTURE_LEN_MIN;
	int MP_GESTURE_SPACE;
	int MP_GESTURE_LEN_MAX_60FPS;
	int MP_GESTURE_LEN_MIN_60FPS;
	float MP_THRESH_UP;
	float MP_THRESH_DOWN;


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

	//state machine parameters
	int startFrame;
	int peak_up;
	int peaks;

	//lock
	mutex m_classify;
	mutex m_reset;
	mutex m_fps;

	enum {R_EYEBROW=0, L_EYEBROW=1, R_CHEEK=2, L_CHEEK=3, MOUTH=4, NOSE=5};

	enum {R_EYEBROW_POS=0, L_EYEBROW_POS=3, R_CHEEK_POS=6, L_CHEEK_POS=9, MOUTH_POS=12, NOSE_POS=15};

	//enum {SM_COMPUTE_NOISEFLOOR=0, SM_SEARCH_START, SM_POSSIBLE_START, SM_START_CONFIRMED_FIND_END,SM_WAIT_BETWEEN_GESTURES};
	enum {
		SM_COMPUTE_NOISEFLOOR=0, 
		SM_SEARCH_START, 
		SM_CT_POSSIBLE_START, 
		SM_CT_START_CONFIRMED_FIND_END,
		SM_ER_POSSIBLE_START, 
		SM_ER_END,
		SM_CT_WAIT_BETWEEN_GESTURES
	};

    enum { NO_MOTION = 0, HAS_MOTION };

	enum { NOISE_FLOOR_IN_PROGRESS=-1, NO_GESTURE=0, CHEEK_TWITCH=1, EYEBROW_RAISE=2, MOUTH_PULL=3};

	int classifySM(Mat feature);

}; //class CheekTwitchDetectionClassifier