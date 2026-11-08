///////////////////////////////////////////////////////////////////////////
// <copyright file="gui_framework.hpp" company="Intel Corporation">
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



#ifndef __GUI_FRAMEWORK_HPP__
#define __GUI_FRAMEWORK_HPP__

#include <string>
#include <deque>
#include <mutex>
#include <thread>
#include <atomic>
#include <condition_variable>

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#include <winsock2.h>
#include <Windows.h>
#include <commctrl.h>
#endif

#include "libivcp/video_source/video_playlist.hpp"
#include "libivcp/ipipeline.hpp"
#include "libivcp/igui_framework.hpp"
#include "libivcp/video_writer/ivideo_writer.hpp"

/*! 
 * \defgroup libpipeline IVCP Pipeline Library
 * \brief IVCP pipeline library
 * @{
 */

/*! 
 * \file gui_framework.hpp
 * \brief Declaration of GUI framework class
 * \copyright (C) 2012, Intel Corporation, all rights reserved
 */

namespace ivcp
{
/*!
* \brief GUIFramework implementation
* \details GUIFramework is a simple pipeline visualizer. It allows to display processed playlist frame by frame
* and show detected regions and classification results during playback.
*/
class IVCP_EXPORT GUIFramework : public IGUIFramework
{
public:

    /*!
    * \brief Context structure
    */
    struct Context
    {
        /*!
        * \brief Pointer to GUIFramework
        */
        GUIFramework* pointerGUI;

        /*!
        * \brief Name of region
        */
        std::string name;
    };
    
    /*!
     * \brief Constructor
     * \details GUIFramework should be pointed to pipeline which should be visualized.
     * Provided pipeline will be automatically configured to use this class for visualization
     * \param[in] visPipeline Pipeline that will be visualised
     * \param[in] framerate Quantity of pipeline processed frames to visualize per second.(Please note that real framerate could be lower in case the pipeline couldn't process frames fast enough)
     */
    GUIFramework(SPtr<IPipeline > visPipeline, float framerate);

    /*!
     * \brief Destructor
     */
    virtual ~GUIFramework();

    /*!
     * \brief Set visualization window name
     * \details Frame processing result is displayed in separate window called in accordance with configuration
     * \param[in] name Visualization window name
     */
    virtual void setWindowName(std::string name);

    /*!
     * \brief Get current visualization window name
     * \details Frame processing result is displayed in separate window called in accordance with configuration
     * \return Visualization window name
     */
    virtual std::string getWindowName();

    /*!
     * \brief Check whether there are some pipeline processing results stored in memory that could be stored to a videofile
     * \details Checks that there are some frames processed prior to creation of output videofile
     * \return true if there are some unsaved frames
     */
    virtual bool haveVideoOutput();

    /*!
     * \brief Store pipeline processing result to videofile
     * \details Displayed frame processing result will be stored as a videofile
     * \param[in] filename Output video file name
     * \param[in] fourcc Output video file codec. By default it is 0, that means avi format
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \param[in] type Output video file type
     * \param[in] convertTo8bit Whether need to save video in 8bit format
     * \details For RAWFILE_OUTPUT type fourcc and convertTo8bit parameters are ignored
     * \throw IOException Unable to open the video file
     */
    virtual void generateVideoOutput(std::string filename, int fourcc = 0, cv::Size frameSize = cv::Size(0, 0), int fps = 30, VideoOutputType type = VIDEOFILE_OUTPUT, bool convertTo8bit = false);

    /*!
     * \brief Enable retaining of pipeline processing result in videofile
     * \details Displayed frame processing result will be stored as a videofile
     * \param[in] filename Output video file name
     * \param[in] fourcc Output video file codec
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \param[in] framesThreshold Frame number limit per output file (if non zero)
     * \param[in] type Output video file type
     * \param[in] convertTo8bit Whether need to save video in 8bit format
     * \details For RAWFILE_OUTPUT type fourcc and convertTo8bit parameters are ignored
     * \throw IOException Unable to open the video file
     */
    virtual void enableVideoOutput(std::string filename, int fourcc, cv::Size frameSize, int fps = 30, int framesThreshold = 0, VideoOutputType type = VIDEOFILE_OUTPUT, bool convertTo8bit = false);

    /*!
     * \brief Disable retaining of pipeline processing result in videofile
     */
    virtual void disableVideoOutput();

    virtual int getNormFrames();
    virtual void setNormFrames(int normFrames = -1);
    
    virtual void saveSettings(const std::string& filename) const;
    virtual void loadSettings(const std::string& filename);

    virtual void setDefaultSettings(IGUIFramework::Settings settings);
    virtual void replaceSettingsWithDefaults();

    virtual void setSettings(IGUIFramework::Settings settings);
    virtual Settings getDefaultSettings() const;
    virtual Settings getSettings() const;

    virtual int getCurrentSensitivity() const { return sensitivityCurrent; };
    virtual std::map<std::string, bool> getGesturesSettings() const { return gesturesCurrent; };

    virtual void setSensitivitySettings(int sens);
    virtual void setGesturesSettings(const std::map<std::string, bool>& gestures) { gesturesCurrent = gestures; } ;

    virtual void setVideoCapture(bool isToCapture) { storeFrames = isToCapture; };

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    virtual void listenSocket(int port = 27015);
#ifdef ASTER_SUPPORT
	virtual bool sendGesture(const char *buffer);
#endif
#endif

	virtual void sendCommandToGUI(const char *buffer, double value);
	void showVideo();
	void hideVideo();
	bool isVideoVisible();

	virtual std::string getCurOutputFilename();
	virtual int getOutputFramesCount();
	bool repositionWindow;
	bool forceResetAfterFaceLostAndFound;

#if 1 //SSdebug
	// this is needed when a worker thread does not exit cleanly - dll
	// need to manually set thread status to terminate process.
	virtual void setWorkerThreadStatusToExiting();
#endif

protected:
    /*!
    * \brief Sensitivity level number
    * \details Could take values between 0-10
    */    
    int sensitivityCurrent;

    /*!
    * \brief Gestures settings 
    * \details Every gesture could be enabled or disabled
    */
    std::map <std::string, bool> gesturesCurrent;

    /*!
     * \brief Callback function for source pipeline node
     * \details This function should be called by visulized pipeline during video source processing to display currently processed frame
     * \param[in] mat Current frame image as matrix
     * \param[in] context pointer to context information provided during callback registration
     */
    static void callbackSource(cv::Mat mat, void* context);

    /*!
     * \brief Callback function for region detector pipeline node
     * \details This function should be called by visulized pipeline during region detection to display detected region boundaries
     * \param[in] mat Detected region rect parameters as 2x2 matrix(in accordance with: first row x,y and second row width, height)
     * \param[in] context pointer to context information provided during callback registration
     */
    static void callbackRegionCreator(cv::Mat mat, void* context);

    /*!
     * \brief Callback function for classifier pipeline node
     * \details This function should be called by visulized pipeline during frame classification to display classification result
     * \param[in] mat Current frame mark as 1x1 matrix
     * \param[in] context pointer to context information provided during callback registration
     */
    static void callbackClassifier(cv::Mat mat, void* context);

    /*!
     * \brief Current visualization window name
     */
    std::string windowName;

    /*!
     * \brief Visualized pipeline
     */
    SPtr<IPipeline> pipeline;

    /*!
     * \brief Cached visualized pipeline mode
     */
    PipelineMode pipelineMode;

    /*!
     * \brief Seesion used py visualized pipeline
     */
    SPtr<Session> session;

    /*!
     * \brief Processed playlist
     */
    SPtr<const VideoPlaylist> playlist;

    /*!
     * \brief Cache for currently processed frame (to be saved to a file)
     */
    cv::Mat originalImage;

    /*!
     * \brief Cache for currently processed frame (to be displayed in a Vizualization window)
     */
    cv::Mat drawImage;

    /*!
     * \brief Cache for currently processed frame (copy for worker thread)
     */
    cv::Mat workerImage;

    /*!
     * \brief Cache for transformation matrix of currently processed frame
     */
    cv::Mat transpose;

    /*!
     * \brief Cache for index of currently processed frame
     */
    VideoPlaylistFrame currentFrame;

    /*!
     * \brief Classification result description of currently processed frame
     */
    std::string classificationDescription;

    /*!
     * \brief Probabilities description of currently processed frame
     */
    std::string probabilitiesDescription;

    /*!
     * \brief Last draw time
     * \details Timer is used to emulate correct frame rate during output
     */
    int64 lastDrawTickTime;

    /*!
     * \brief Time in cvWait function on the last iteration in ticks
     * \details Sleep time is used in calculating FPS
     */
    int64 lastSleepTickTime;

    /*!
     * \brief Current time
     * \details Timer is used to emulate correct frame rate during output
     */
    int64 nowTickTime;

    /*!
     * \brief Time between sequential frame display
     * \details Display time is calculated in accordance with emulated frame rate(constant, 30 FPS at the moment)
     */
    double drawDelay;

    /*!
     * \brief A-priori frame mark source for current playlist
     */
    SPtr<IClassifierResultDataSource> markup;

    /*!
     * \brief Frame processing results stored for video file generation
     */
    std::deque<cv::Mat> outputFrames;

    /*!
     * \brief Flag controling frame processing results storage
     */
    bool storeFrames;

    /*!
     * \brief Video file handler used to store processing results
     */
    SPtr<IVideoWriter> output;

    /*!
     * \brief Frame size used during video file generation
     */
    cv::Size outputSize;

    /*!
     * \brief Queue of the latest 30 fps measurements
     */
    std::deque<int> fpses;

    /*!
     * \brief Cache image used for saving video stream to a file
     * \details In case user wants to save video to a file and the original image is 12 bit from the camera
     *  we have to convert it to a color image and store in first 12 bits of 24 bits 
     */
    cv::Mat mergedImage;

    /*!
     * \brief Maximum number of frames per one output file
     * \details In case user wants to save video to a file and wants to limit its size, outputFramesThreshold needs to be set to non zero. This will limit frame number of every output file and will imply creating several files if threshold is achieved
     */
    int outputFramesThreshold;

    /*!
     * \brief Counter for currently outputted frames related to current output
     * \details In case user wants to save video to a file this counter is responsible for number of outputted frames related to current output
     */
    int outputFramesCount;

    /*!
     * \brief Name of file to store output
     * \details In case user wants to save video to a file desired name is stored in filename
     */
    std::string outputFileName;
	std::string curOutputFileName;

    /*!
     * \brief Counter for currently created files related to current output
     * \details In case user wants to save video to a file this counter is responsible for total number of created files related to current output
     */
    int outputFilesCount;

    /*!
     * \brief 4-character code of codec used to compress the frames
     * \details In case user wants to save video to a file outputFourcc contains 4-character code of codec used to compress the frames
     */
    int outputFourcc;

    /*!
     * \brief Framerate of the created video stream
     * \details In case user wants to save video to a file outputFps contains framerate of the created video stream
     */
    int outputFps;

    /*!
     * \brief Type of a video output file
     */
    VideoOutputType outputFileType;

    /*!
     * \brief Per GUIFramework specific callback function for source pipeline node
     * \details This function is called from generic callback function
     * \param[in] mat Current frame image as matrix
     */
    void callbackSource(cv::Mat mat);

    /*!
     * \brief Per GUIFramework specific callback function for region detector pipeline node
     * \details This function is called from generic callback function
     * \param[in] mat Detected region rect parameters as 2x2 matrix(in accordance with: first row x,y and second row width, height)
     * \param[in] name Name of the region creator filter which called this callback
     */
    void callbackRegionCreator(cv::Mat mat, std::string name);

    /*!
     * \brief Per GUIFramework specific callback function for classifier pipeline node
     * \details This function is called from generic callback function
     * \param[in] mat Current frame mark as 1x1 matrix
     */
    void callbackClassifier(cv::Mat mat);

    /*!
     * \brief Internal function used to display generated processing result
     */
    void draw();

    /*!
     * \brief Internal function used to store (or prepare for storage) generated processing result
     * \param[in] mat Frame to save
     * \throw IOException Unable to open videofile if several files are used
     */
    void storeToVideoOutput(cv::Mat mat);

    /*!
     * \brief Save frame to the avi file
     * \param[in] mat Frame to save
     */
    void saveFrame(cv::Mat mat);

    /*!
     * \brief Normalize brightness of given 16 bit image 
     * \details The function search for min/max values of src16bit image (this is done for each normFrames'th frame)
     * The min and the max value are saved and are used to convert the image to 8 bit (ie 0...255)
     * \param[in] src16bit Source 16 bit image which shall be scaled to the 8 bit destanation image
     * \param[out] dst8bit Destination  8 bit image
     */
    void normalizeBrightness(const cv::Mat &src16bit, cv::Mat &dst8bit);

    /*!
     * \brief Whether video source adjusting is on
     */
    bool adjustingOn;

    /*!
     * \brief Min value found in the previous 16bit image (used in brightness normalization)
     */
    unsigned int min16BitValue;

    /*!
     * \brief Max value found in the previous 16bit image (used in brightness normalization)
     */
    unsigned int max16bitValue;

    /*!
     * \brief Frequency of searching for min and max 16 bit values (the values are searched each normFrames'th frames)
     * \details In case normFrames is -1 no automatic brightness normalization tuning (ie calculating max and min values) is performed.
     * Max value is assumed to be 4095, min value is assumed to be 0
     */
    int normFrames;

    /*!
     * \brief 16 bit frames counter (used in brightness normalization)
     */
    unsigned long long frameCounter16Bit;

    /*!
     * \brief Default GUI settings
     */
    Settings defaultSettings;

    /*!
     * \brief Pointers to contexts for region (used in callback to get to know name of the region which sent the message)
     */
    std::list<Context*> contexts;

    /*!
     * \brief Mutex to ensure correct work when access in different threads  
     */
    static std::recursive_mutex locker;

    /*!
     * \brief Worker and captor thread status: running, signaled to exit, finalized
     */
    enum ThreadStatus
    {
        THREAD_RUNNING = 0,
        THREAD_EXITING = 1,
        THREAD_FINALIZED = 2
    };
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32

#ifdef ASTER_SUPPORT
	SOCKET acceptSocket;
#endif

    /*!
     * \brief Whether window is shown
     */
    bool isShown;

    /*!
     * \brief Whether shift button is pressed
     */
    static bool shiftPressed;

    /*!
     * \brief Whether alt button is pressed
     */
    static bool altPressed;

    /*!
     * \brief Whether ctrl button is pressed
     */
    static bool ctrlPressed;

    /*!
     * \brief Whether window for settings is show
     */
    bool isSettingsShow;

    /*!
     * \brief Event handler for window
     */
    int ownWaitKey(int, std::string);

    /*!
     * \brief Callback friend function of WIN API
     */
    friend LRESULT CALLBACK LowLevelKeyboardProc(int, WPARAM, LPARAM);

    /*!
     * \brief Callback friend function of WIN API
     */
    friend LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);

    /*!
     * \brief Hook for working with keyboard pressing
     */
     HHOOK hhook;

    /*!
     * \brief Static function used to create all controls on setting window
     */
    static void createControls(HWND);

    /*!
     * \brief Function used to change sensitivity when slider position is changed
     */
    void updateSensitivityLabel(HWND);

    /*!
     * \brief Function used to change exposure when slider position is changed
     */
    void updateExposureLabel(HWND);

    /*!
     * \brief Static function used to change setting when user makes some changes via settings window
     */
    static void updateControls(HWND, WPARAM);

    /*!
     * \brief Function used to add all settings to setting window when it is created
     */
    void fillSettingWindow(HWND);

    /*!
     * \brief Function used to add region settings to setting window
     */
    void fillRegionWindow(HWND, LRESULT, bool);

#if (defined USE_DLIB || defined USE_REALSENSE)
    /*!
     * \brief Function used when some changes done in region settings
     */
    void changeByRegionWindow(HWND, LRESULT);

    /*!
     * \brief Function used to reset Dlib or RealSense tracking
     */
    void resetTracking();
#endif

    /*!
     * \brief Static function used to create settings window
     */
    static void createSettingWindow(GUIFramework*);
    
    /*
     * \brief A map from Settings window handler to GUIFramework pointer in order to find the GUIFramework which created
     * the window received the message
     */
    static std::map<HWND, GUIFramework*> fromSettingsHandlerToGUI;

    /*
     * \brief A map from window handler to GUIFramework pointer in order to find the GUIFramework which created
     * the window received the message
     */
    static std::map<HWND, GUIFramework*> fromHandlerToGUI;

    /*!
     * \brief Static function used to add mapping between Settings HWND and GUIFramework's example
     */
    static void setMapping(HWND, GUIFramework*);

    /*!
     * \brief Static function used to get mapping between Settings HWND and GUIFramework's example
     */
    static GUIFramework* getMapping(HWND);

    /*!
     * \brief Static function used to add mapping between HWND and GUIFramework's example
     */
    static void setMappingGUI(HWND, GUIFramework*);

    /*!
     * \brief Static function used to get mapping between HWND and GUIFramework's example
     */
    static GUIFramework* getMappingGUI(HWND);

    /*!
     * \brief Static function used to remove mapping between HWND and GUIFramework's example
     */
    static void removeMappingGUI(GUIFramework*);

    /*!
     * \brief Static function used to remove mapping between Settings HWND and GUIFramework's example
     */
    static void removeMapping(HWND);

    /*!
     * \brief Function used to find settings HWND from GUIFramework's example
     */
    static HWND findSettingsHandler(GUIFramework*);

    /*
     * \brief Create a socket on given port
     */
    int createSocket();

    /*
     * \brief Close the socket guiListenSocket
     */ 
    int closeSocket();

    /*
     * \brief A thread that listen for new socket connections
     */
    static void listenMessageThread(GUIFramework* gui);

    /*
     * \brief A thread function which receives and processes messages 
     */
#ifdef ASTER_SUPPORT
	static void receiveMessageThread(SOCKET socket, GUIFramework* gui);
#else
	static void receiveMessageThread(SOCKET socket);
#endif

    /*
     * \brief WinSocket variable
     */
    SOCKET guiListenSocket;

    /*
     * \brief Listener thread for GUI command socket
     */
    std::thread socketListenerThread;

    /*
     * \brief Signal to stop the GUI command socket listener
     */
    std::atomic<bool> stopSocketListener;

    /*
     * \brief Socket port number
     */
    int port;

    /*
     * \brief Name of current selected region
     */
    std::string currentRegion;
    
    /*
     * \brief A map from region's name to region's size via cv::Rect
     */
    std::map<std::string, cv::Rect> allRegions;

    /*!
     * \brief Whether it is the first call of draw() function
     */
    bool isWindowInitialized;

    /*!
     * \brief Counter of GUI Frameworks
     */
    static int countGUIFramework;

    /*!
     * \brief Whether it is the GUI destroyed
     */
    bool isDestroyed;

    /*!
     * \brief Static function used to process mouse click
     */
    static void onMouse( int event, int x, int y, int, void* );

    /*!
     * \brief Functions used to destroy the Settings Window
     */
    void destroySettingsWindow();

    /*
     * \brief A thread for drawing
     */
    std::thread* worker;

    /*!
     * \brief Whether worker thread is ready to update displayed image
     */
    bool isWorkerReady;

    /*!
     * \brief Whether worker thread is ended
     */
    ThreadStatus workerStatus;

    /*!
     * \brief Condition variable for the main worker thread - whether or not it is time to exit
     */
    std::condition_variable_any cvTimeToExit;

    /*!
     * \brief Condition variable for the main worker thread - whether or not the worker thread shall show the image
     */
    std::condition_variable_any cvWorkerReady;

    /*!
     * \brief A thread function which draw displayed image
     */
    static void workerLoop(GUIFramework*);

    /*!
     * \brief Index of changed region
     */
    int intChangedRegion;

    /*!
     * \brief Whether changing actions performed to regions
     */
    bool regionChanged;
#endif
    /*!
     * \brief Sequence of unhandling pressed keys
     */
    std::deque<int> pressedKeys;
    
    /*!
     * \brief Function used to handle pressed keys
     */
    void handleKeyboardPress();
	ClassType gestureId;

    /*!
     * \brief Last processed frame number
     */
    int frameNum;

    /*!
     * \brief Open output video file
     */
    void openVideoOutput();

    /*!
     * \brief Save parameters for output video file
     * \param[in] filename Output video file name
     * \param[in] fourcc Output video file codec
     * \param[in] frameSize Output video file frame size
     * \param[in] fps Output video file frame rate
     * \param[in] framesThreshold Frame number limit per output file (if non zero)
     * \param[in] type Output video file type
     * \param[in] convertTo8bit Whether need to save video in 8bit format
     * \throw IOException Unable to open the video file
     */
    void saveVideoOutputParams(std::string filename, int fourcc, cv::Size frameSize, int fps = 30, int framesThreshold = 0, VideoOutputType type = VIDEOFILE_OUTPUT, bool convertTo8bit = false);

    /*!
     * \brief Whether need to save video in 8bit format
     */
    bool convertTo8bit;

    /*!
     * \brief Close output video file
     */
    void closeVideoOutput();

    /*!
     * \brief A thread for capturing to ouput video file
     */
    std::thread* captor;

    /*!
     * \brief Whether captor thread is ready to write a new frame
     */
    bool isCaptorReady;

    /*!
     * \brief Whether captor thread is ended
     */
    ThreadStatus captorStatus;

    /*!
     * \brief Condition variable for the main captor thread - whether or not it is time to exit
     */
    std::condition_variable_any cvCaptorTimeToExit;

    /*!
     * \brief Condition variable for the main captor thread - whether or not the captor thread shall show the image
     */
    std::condition_variable_any cvCaptorReady;

    /*!
     * \brief A thread function which draw displayed image
     */
    static void captorLoop(GUIFramework*);

    /*!
     * \brief Sequence of unwritten frames to output video file
     */
    std::deque<cv::Mat> captorImages;

private:
    /*!
     * \brief Get current pipeline settings
     * \param[in] pipeline Pointer of the pipeline 
     * \param[out] settings Settings of the pipeline 
     */
    void getPipelineSettings(SPtr<IPipeline> pipeline, Settings& settings) const;

    /*!
     * \brief Change current pipeline settings
     * \param[in] pipeline Pointer of the pipeline 
     * \param[in] settings New settings for the pipeline 
     */
    void setPipelineSettings(SPtr<IPipeline> pipeline, const Settings& settings);

     /*!
     * \brief Save current or default settings to the file storage
     * \param[in] settings Settings to save
     * \param[in] id type of settings (current or default)
     * \param[in] fs File storage where settings should be saved
     */
    void writeSettings(const Settings& settings, const std::string& id, cv::FileStorage& fs) const;

     /*!
     * \brief Load current or default settings from the file storage
     * \param[out] settings Loaded settings
     * \param[in] id type of settings (current or default)
     * \param[in] fs File storage from which settings should be loaded
     */
    void readSettings(Settings& settings, const std::string& id, cv::FileStorage& fs) const;
    
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    /*!
     * \brief Save port number to the file storage
     * \param[in] fs File storage where settings should be saved
     */
    void writeSettingsPort(cv::FileStorage& fs) const;
    /*!
     * \brief Load port number from the file storage
     * \param[in] fs File storage from which settings should be loaded
     */
    void readSettingsPort(cv::FileStorage& fs);
#endif
};

};

/** @}*/
#endif