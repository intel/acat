///////////////////////////////////////////////////////////////////////////
// <copyright file="gui_framework.cpp" company="Intel Corporation">
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



#include <stdio.h>
#include <iomanip> 
#include <chrono>
#include "libpipeline/gui_framework.hpp"

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#include <strsafe.h>
#include <winsock2.h>
#include <Ws2tcpip.h>

#pragma comment(lib, "comctl32.lib")
#pragma comment(lib, "ws2_32.lib")
#endif


#include "libivcp/factory.hpp"
#include "libivcp/video_writer/avi_video_writer.hpp"
#include "libivcp/video_writer/rawfile_video_writer.hpp"

#define CURRENT "current"
#define DEFAULT "default"

#define HWND_CAMERA_SETTINGS 10
#define HWND_AUTO_EXPOSURE 11
#define HWND_MANUAL 12
#define HWND_EXPOSURE_LEFT 13
#define HWND_EXPOSURE_RIGHT 14
#define HWND_EXPOSURE_POSITION 15
#define HWND_EXPOSURE_CONTROL 16
#define HWND_GESTURE_LABEL 20
#define HWND_GESTURE_COMBOBOX 21
#define HWND_GESTURE_CHECK_BOX 22
#define HWND_REGION_LABEL 30
#define HWND_REGION_COMBO_BOX 31
#define HWND_REGION_OFFSETX_LABEL 32
#define HWND_REGION_OFFSETX 33
#define HWND_REGION_OFFSETY_LABEL 34
#define HWND_REGION_OFFSETY 35
#define HWND_REGION_SCALEH_LABEL 36
#define HWND_REGION_SCALEH 37
#define HWND_REGION_SCALEW_LABEL 38
#define HWND_REGION_SCALEW 39
#define HWND_REGION_STABFRAME_LABEL 310
#define HWND_REGION_STABFRAME 311
#define HWND_REGION_STABTHRESHOLD_LABEL 312
#define HWND_REGION_STABTHRESHOLD 313
#define HWND_BUTTON_RESET 319
#define HWND_BUTTON_APPLY 320
#define HWND_BUTTON_DEFAULT 321
#define HWND_SENSITIBITY_LABEL 40
#define HWND_SENSITIBITY_LEFT 41
#define HWND_SENSITIBITY_RIGHT 42
#define HWND_SENSITIBITY_POSITION 43
#define HWND_SENSITIBITY_CONTROL 44
#define HWND_BUTTON_SAVE 50
#define HWND_BUTTON_LOAD 51

#define START_FRAME_NORMALIZATION 10

using namespace std;
using namespace cv;

namespace ivcp
{
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    bool GUIFramework::shiftPressed = false;
    bool GUIFramework::altPressed = false;
    bool GUIFramework::ctrlPressed = false;
    int GUIFramework::countGUIFramework = 0;
    std::map<HWND, GUIFramework*> GUIFramework::fromSettingsHandlerToGUI;
    std::map<HWND, GUIFramework*> GUIFramework::fromHandlerToGUI; 
#endif

// OpenCV 5 throws when destroying a window that does not exist (2.4 ignored it).
static void safeDestroyWindow(const std::string& name)
{
    try
    {
        cv::destroyWindow(name);
    }
    catch (const cv::Exception&)
    {
    }
}

std::recursive_mutex GUIFramework::locker;

GUIFramework::GUIFramework(SPtr<IPipeline > visPipeline, float _framerate)
{
    float framerate = _framerate;
    // In case framerate is set in pipeline
    if (visPipeline->getPerformanceRate() > 0)
    {
        framerate = (float) visPipeline->getPerformanceRate();
    }

    if ( !(framerate > 0) )
    {
        IVCP_EXCEPTION(IncorrectParamException, 
            "Pipeline visualization frame rate should be greater than 0.");
    }
    drawDelay = 1000.0f / framerate;
    windowName = "Pipeline visualization";
    pipeline = visPipeline;
    pipelineMode = pipeline->getMode();
    session = pipeline->getSession();
    adjustingOn = false;
    contexts.clear();

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    isWindowInitialized = true;
    isDestroyed = true;
    setSensitivitySettings(0);
    currentRegion = "";
    isShown = true;
    isSettingsShow = false;
    regionChanged = false;
    workerStatus = THREAD_RUNNING;
    isWorkerReady = false;
    worker = 0;
	repositionWindow=true;
	forceResetAfterFaceLostAndFound = false;
#ifdef ASTER_SUPPORT
	acceptSocket = NULL;
#endif
#endif

    captorStatus = THREAD_RUNNING;
    isCaptorReady = false;
    captorImages.clear();
    captor = 0;

    if (!session)
    {
        IVCP_EXCEPTION(IncorrectParamException, 
            "Session is not defined for current pipeline. Couldn't handle pipeline objects due to lack of information.");
    }

    playlist = session->getVideoPlayer()->getPlaylist();
    pipeline->addNodeCallback(session->getVideoPlayer()->getId(), callbackSource, this);


    for (int i = 0; i < session->getNumberOfRegionCreators(); i++)
    {
        try
        {
            Context *context = new GUIFramework::Context();
            context->pointerGUI = this;
            context->name = session->getRegionCreator(i)->getId();
            pipeline->addNodeCallback(session->getRegionCreator(i)->getId(), callbackRegionCreator, context);

            contexts.push_back(context);
        }
        catch(Exception &e)
        {
            IVCP_LOG_INFO(e.message);
            continue;
        }
    }

    for (int i = 0; i < session->getNumberOfClassifiers(); i++)
    {
        try
        {
            pipeline->addNodeCallback(session->getClassifier(i)->getId(), callbackClassifier, this);
        }
        catch(Exception &e)
        {
            IVCP_LOG_INFO(e.message);
            continue;
        }
        break;
    }

    markup = NULL;
    for (int i = 0; session->getNumberOfDataSources(); i++)
    {
        SPtr<IClassifierResultDataSource > temp = session->getDataSource(i);
        if (temp)
        {
            markup = temp;
            break;
        }
    }

    storeFrames = false;
    lastDrawTickTime = 0;
    lastSleepTickTime = 0;
    nowTickTime = getTickCount();

    min16BitValue = 0;
    max16bitValue = 4095;
    normFrames = -1;
    frameCounter16Bit = 0;
    frameNum = 0;
        
	gestureId = 0;

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    guiListenSocket = INVALID_SOCKET;
    stopSocketListener = false;
#endif
    output = new AvifileVideoWriter();
}

GUIFramework::~GUIFramework()
{
	//cout << "In ~GUIFramework\n" << endl;
    pipeline->removeNodeCallback(session->getVideoPlayer()->getId(), callbackSource, this);

    for (auto it = contexts.begin(); it != contexts.end(); ++it)
    {
        try
        {
            pipeline->removeNodeCallback((*it)->name, callbackRegionCreator, (*it));
        }
        catch(Exception &e)
        {
            IVCP_LOG_INFO(e.message);
            continue;
        }
    }

    for (auto it = contexts.begin(); it != contexts.end(); ++it)
    {
        if (*it != 0)
        {
            delete *it;
        }
    }
    contexts.clear();

    for (int i = 0; i < session->getNumberOfClassifiers(); i++)
    {
        try
        {
            pipeline->removeNodeCallback(session->getClassifier(i)->getId(), callbackClassifier, this);
        }
        catch(Exception &e)
        {
            IVCP_LOG_INFO(e.message);
            continue;
        }
        break;
    }

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
	if (workerStatus == THREAD_RUNNING) //SSdebug
	{
		workerStatus = THREAD_EXITING;
		cvWorkerReady.notify_one();
	}
        
    if (worker != 0)
    {		
        unique_lock<recursive_mutex> lk(GUIFramework::locker);  
		//cout << "IVCP: Waiting for Thread_Finalized wait from state: " << workerStatus << endl;
        //Waiting for the worker thread to signal it is exited
        cvTimeToExit.wait(lk, [&] { return (workerStatus == THREAD_FINALIZED); } );
		//cout << "IVCP: Out of wait for Thread_Finalized\n" << endl;
    }

	
    if (countGUIFramework == 1)
    {
        UnhookWindowsHookEx(hhook);
    }
    countGUIFramework--;
    
    closeSocket();
#endif
    if (captor != 0)
    {
        disableVideoOutput();
    }
	//worker->join();
	//cout << "IVCP: Done ~GuiFramework()\n" << endl;
}

void GUIFramework::callbackSource(cv::Mat mat, void* context)
{
    static_cast<GUIFramework*>(context)->callbackSource(mat);
}

void GUIFramework::callbackRegionCreator(cv::Mat mat, void* context)
{
    Context *currentContext = static_cast<Context *>(context);
    currentContext->pointerGUI->callbackRegionCreator(mat, currentContext->name);
}

void GUIFramework::callbackClassifier(cv::Mat mat, void* context)
{
    static_cast<GUIFramework*>(context)->callbackClassifier(mat);
}

void GUIFramework::callbackSource(cv::Mat mat)
{
    nowTickTime = getTickCount();

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isShown)
#endif
    {
        if (drawImage.empty())
        {
            if (mat.depth() == CV_16UC1 && mat.channels() == 1)
            {
                normalizeBrightness(mat, drawImage);
            }
            else
            {
                drawImage = mat.clone();
            }
        }
    }
    if (storeFrames)
    {
        originalImage = mat.clone();
    }
    draw();
    lastDrawTickTime = nowTickTime;    
    currentFrame = playlist->getCurrentFrame();
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isShown) 
#endif
    {
        if (mat.depth() == CV_16UC1 && mat.channels() == 1)
        {
            normalizeBrightness(mat, drawImage);
        }
        else
        {
            drawImage = mat.clone();
        }   
    }
    transpose = Mat();

    frameNum++;
}

void GUIFramework::callbackRegionCreator(cv::Mat mat, std::string name)
{
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isShown)
#endif
    {
        // rect should be put to matrix in accordance with: first row x,y, width, height
        // first three columns of last three rows transformation matrix
        Rect current((int)mat.at<double>(0, 0), (int)mat.at<double>(0, 1), (int)mat.at<double>(0, 2), (int)mat.at<double>(0, 3));
        if(transpose.empty())
        {
            if(mat.rows == 4)
            {
                transpose = mat.rowRange(1, 4).colRange(0, 3);
                Mat trDraw(drawImage.rows, drawImage.cols, CV_8UC3);
                warpPerspective(drawImage, trDraw, transpose, trDraw.size(), 0);
                drawImage = trDraw;
            }
            else
            {
                transpose = Mat::eye(3, 3, CV_64FC1);
            }
        }
        else
        {
            Mat ref;
            if(mat.rows == 4)
            {
                ref = mat.rowRange(1, 4).colRange(0, 3);
            }
            else
            {
                ref = Mat::eye(3, 3, CV_64FC1);
            }
            if( countNonZero(transpose - ref) )
            {
                IVCP_EXCEPTION(IncorrectParamException, "Could not display regions with different transformation matrixes");
            }
        }
    #if (defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32) && (defined USE_DLIB || defined USE_REALSENSE)
		if ( (name.compare("regionRCheek") == 0) || (name.compare("regionLCheek") == 0) || (name.compare("regionREyebrow") == 0) || (name.compare("regionLEyebrow") == 0) || (name.compare("regionMouth") == 0))
		{
			if (name.compare(currentRegion))
			{
				rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(255), 2);
			}
			else
			{
				rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(0, 255, 255), 2);
			}
		}
        allRegions[name] = current;
    #else
        (void)name;
        rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(255), 2);
    #endif

	if (((gestureId == 102) || (gestureId == 103)) && (name.compare("regionRCheek")==0))
		rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(0,255,0),5 );
	if (((gestureId == 102) || (gestureId == 103)) && (name.compare("regionLCheek")==0))
		rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(0,255,0),5 );
	if ((gestureId == 2) && (name.compare("regionREyebrow")==0))
		rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(0,255,0),5 );
	if ((gestureId == 2) && (name.compare("regionLEyebrow")==0))
		rectangle(drawImage, Point(current.x, current.y), Point(current.x + current.width, current.y + current.height), Scalar(0,255,0),5 );

    }
}

void GUIFramework::callbackClassifier(cv::Mat mat)
{
    ClassType mark = (ClassType)mat.at<float>(0, 0);
    stringstream str;
    str << "Classified as ";
    str << mark;
    classificationDescription = str.str(); 

    stringstream probs;
    if (mat.cols > 1)
    {
        probs << "(" << setprecision(3) << fixed;
        for (int i = 1; i < mat.cols; i++)
        {
            float prob = mat.at<float>(0, i);
            probs << prob;
            if (i < mat.cols - 1)
            {
                probs << "; ";
            }
        }
        probs << ")";
        probabilitiesDescription = probs.str();
    }

		gestureId = mark;

}

void GUIFramework::draw()
{
    if(storeFrames && !originalImage.empty())
    {
        if (captor == 0)
        {
            storeToVideoOutput(originalImage);
        }
        else
        {
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
            captorImages.push_back(originalImage);
            isCaptorReady = true;
            cvCaptorReady.notify_one();
        }
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
        if (isShown)
#endif
        {
            if(drawImage.rows > 50)
            {
                putText(drawImage, "capturing...", Point(1, drawImage.rows - 5), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
        }
    }
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isShown)
#endif
    {
        if(pipelineMode == TRAIN_PIPELINE || pipelineMode == TEST_PIPELINE)
        {
            if(markup && markup->isClassified(currentFrame))
            {
                stringstream str;
                ClassType mark = markup->getClassifyResult(currentFrame);
                if(mark >= 0)
                {
                    str << "Marked as ";
                    str << mark;
                }
                else
                {
                    str << "Not marked";
                }
                putText(drawImage, str.str(), Point(1, 10), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
        }

        if(pipelineMode == TRAIN_PIPELINE)
        {
            putText(drawImage, "Classifier training is in progress...", Point(1, 25), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
        }

        if(classificationDescription != "")
        {
            if(pipelineMode == RUNTIME_PIPELINE)
            {

				//SS 05/12/2023 Suppressing messages on image 
				//if (gestureId == 1)
				if ((gestureId == 102) || (gestureId == 103)) {//check trigger
					//putText(drawImage, "Cheek Motion", Point(500, 50), FONT_HERSHEY_TRIPLEX, 2.0, Scalar(255),3 );
					//putText(drawImage, "Cheek Gesture Detected", Point(drawImage.cols/4, 50), FONT_HERSHEY_COMPLEX, 1.0, Scalar(255),3);
				}
				if (gestureId == 2) {
					//putText(drawImage, "Eyebrow Raise Detected", Point(drawImage.cols/4, 50), FONT_HERSHEY_COMPLEX, 1.0, Scalar(255),3);
				}
				if (gestureId == 3) {
					//putText(drawImage, "Mouth Pull Detected", Point(drawImage.cols/4, 50), FONT_HERSHEY_COMPLEX, 1.0, Scalar(255),3);
				} 
				else if (gestureId < 0) {//calibration
					//putText(drawImage, "Please wait... Calibration In Progress", Point(80, 40), FONT_HERSHEY_COMPLEX, 0.8, Scalar(255),2 );
				} 
				else {
					//putText(drawImage, classificationDescription, Point(1, 10), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255) );
				}
            }
            else
            {
                putText(drawImage, classificationDescription, Point(1, 25), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
            classificationDescription = "";
        }  
        if(probabilitiesDescription != "")
        {
            if(pipelineMode == RUNTIME_PIPELINE)
            {
                putText(drawImage, "Distribution:", Point(1, 25), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
                putText(drawImage, probabilitiesDescription, Point(1, 40), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
            else
            {
                putText(drawImage, "Distribution:", Point(1, 40), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
                putText(drawImage, probabilitiesDescription, Point(1, 55), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
            probabilitiesDescription = "";
        }
    }
	
    double frequency = getTickFrequency();
    int fps = cvRound(frequency / ((double)(nowTickTime - lastDrawTickTime)));
    fpses.push_back(fps);
    if (fpses.size() > 30)
    {
        fpses.pop_front();
    }

    int fpssum = 0;
    for (unsigned int q = 0; q < fpses.size(); q++) 
    {
        fpssum += fpses[q];
    }
    fpssum /= (int) fpses.size();
    
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isShown)
#endif
    {
        std::stringstream ss;
        ss << "fps: " << fpssum;
		//ss << "      Frame: " << currentFrame.frameIndex;

        putText(drawImage, ss.str(), Point(1, 70), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255) );
        if (adjustingOn)
        {
			//putText(drawImage, "Auto adjusting....Please wait", Point(drawImage.cols/4, 40), FONT_HERSHEY_COMPLEX, 0.8, Scalar(255),2 );
        }

#ifdef USE_DLIB
        if (Factory::isDlibHandlerCreated())
        {
            SPtr<IDlib> dlibHandler = Factory::getCachedDlib();
            if (dlibHandler->isFaceSearching() || !dlibHandler->isFaceFound())
            {
				forceResetAfterFaceLostAndFound = true;
                //putText(drawImage, "Detecting face, please wait...", Point(1, 85), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
            else if (!dlibHandler->isRegionsReady())
            {
                //putText(drawImage, "Glasses searching is in progress, please wait...", Point(1, 85), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
#ifndef SS_ORIG
			else if (forceResetAfterFaceLostAndFound) //SS to add stability
			{
				forceResetAfterFaceLostAndFound = false;
				try
				{
					resetTracking();
				}
				catch (exception& e)
				{
					cout << "Exception " << e.what() << '\n';
					IVCP_EXCEPTION(IncorrectParamException, "Dlib::getRegion exception");
				}
			}
#endif
        }
#endif
#ifdef USE_REALSENSE
        if (Factory::isRealSenseHandlerCreated())
        {
            SPtr<IRealSense> rsHandler = Factory::getCachedRealSense();
            if (!rsHandler->isFaceFound())
            {
                putText(drawImage, "Detecting face, please wait...", Point(1, 85), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
            else if (!rsHandler->isRegionsReady())
            {
                putText(drawImage, "Glasses searching is in progress, please wait...", Point(1, 85), FONT_HERSHEY_COMPLEX, 0.4, Scalar(255));
            }
        }
#endif
    }
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isWindowInitialized)
    {
        isWindowInitialized = false;
        Settings settings;
        getPipelineSettings(pipeline, settings);
        if (!settings.regions.empty())
        {
            defaultSettings.regions = settings.regions;
        }

		//cout << "Starting worker thread\n" << endl;
        worker = new std::thread(GUIFramework::workerLoop, this);
		//cout << "\nworker is :" << worker << endl;
    }

    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    if (isShown)
    {
        drawImage.copyTo(workerImage);
    }
    isWorkerReady = true;
    cvWorkerReady.notify_one();

#else
    imshow(windowName, drawImage);
    int sleepTime = max( cvRound(drawDelay - fabs(1000 * ((double)(nowTickTime - lastDrawTickTime - lastSleepTickTime)) / frequency)), 1 );
    lastSleepTickTime = (int64)(((double)sleepTime / 1000.0) * frequency);
    int key = waitKey(sleepTime);
    pressedKeys.push_back(key);
#endif

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
    if (isShown)
#endif
    {
        handleKeyboardPress();
    }
}

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
void GUIFramework::workerLoop(GUIFramework* pGui)
{ 
	//cout <<"ENtering GUIFramework::workerLoop with workerStatus as " << pGui->workerStatus << endl;
    {
        lock_guard<recursive_mutex> lk(GUIFramework::locker);
        if (countGUIFramework == 0)
        {
            HINSTANCE instance = GetModuleHandle(NULL);
            HOOKPROC keyboardProc = (HOOKPROC)LowLevelKeyboardProc;
            pGui->hhook = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, instance, 0);
        }
        countGUIFramework++;
    }  

    createSettingWindow(pGui);

    while (true)
    {
		//cout <<"In GUIFramework::workerLoop in while true workerStatus = " << pGui->workerStatus << endl;
        if (pGui->workerStatus == THREAD_EXITING)
        {
			//cout << "In GUIFramework::workerLoop workerStatus is THREAD_EXITING return\n" << endl;
            safeDestroyWindow(pGui->windowName);
            pGui->destroySettingsWindow();

            lock_guard<recursive_mutex> lk(GUIFramework::locker);  
            pGui->workerStatus = THREAD_FINALIZED;
            pGui->cvTimeToExit.notify_one();
			//cout << "In GUIFramework::workerLoop: workerStatus changed THREAD_FINALIZED return\n" << endl;
            return;
        } 
 
		//cout << "In GUIFramework::workerLoop - workerStatus == NOT THREAD_EXITING\n" << endl;
        unique_lock<recursive_mutex> lk(GUIFramework::locker); 
        if (!pGui->isWorkerReady)
        {
            pGui->cvWorkerReady.wait(lk, [&] { 
                if (pGui->workerStatus == THREAD_RUNNING)
                {
					//cout << "In GUIFramework::workerLoop - return (pGui->isWorkerReady == true\n" << endl;
                    return (pGui->isWorkerReady == true);
                }
                else 
				{
					//cout << "In GUIFramework::workerLoop - return true\n" << endl;
                    return true;
				}
                }); 
        }

        if (pGui->isWorkerReady)
        {  
            if (pGui->isShown)
            {          
                if (pGui->isDestroyed)
                {
					//cout << "In GUIFramework::workerLoop - calling removeMappingGUI\n" << endl;
                    pGui->isDestroyed = false;
                    removeMappingGUI(pGui);
                    namedWindow(pGui->windowName, cv::WINDOW_NORMAL);  
#if (defined USE_DLIB || defined USE_REALSENSE)
                    setMouseCallback(pGui->windowName, onMouse, pGui);                 
#endif
                }  
                imshow(pGui->windowName, pGui->workerImage);
				if (pGui->repositionWindow)
				{
					//moveWindow(pGui->windowName, 0, 0);  //SS 05/12/12023 changed so it is not visible on screen but ACAT can grab it
					moveWindow(pGui->windowName, 0, 0);
					pGui->repositionWindow = false;
				}
				//moveWindow(pGui->windowName,0,0);
                HWND hwnd = FindWindow(NULL, (LPCSTR)(pGui->windowName.c_str()));
                GUIFramework::setMappingGUI(hwnd, pGui);
            }
            else
            {
				pGui->repositionWindow = true;
                if (!pGui->isDestroyed)
                {
					//cout << "In GUIFramework::workerLoop - pGui->isDestroyed\n" << endl;
                    pGui->isDestroyed = true;
                    safeDestroyWindow(pGui->windowName);

                }           
            }

            double frequency = getTickFrequency();
            int sleepTime = max( cvRound(pGui->drawDelay - fabs(1000 * ((double)(pGui->nowTickTime - pGui->lastDrawTickTime - pGui->lastSleepTickTime)) / frequency)), 1 );
            pGui->lastSleepTickTime = (int64)(((double)sleepTime / 1000.0) * frequency);
            if (pGui->isShown)
            {
                pGui->pressedKeys.push_back(pGui->ownWaitKey(sleepTime, pGui->windowName));
            } 
            else 
            {
                pGui->ownWaitKey(sleepTime, pGui->windowName);
            }

            if (pGui->regionChanged)
            {
                HWND hwnd = findSettingsHandler(pGui);
                pGui->fillRegionWindow(hwnd, pGui->intChangedRegion, false);
                pGui->regionChanged = false;
            }
        
            pGui->isWorkerReady = false;   
        }
    }
	//cout << "Exiting workerloop\n" << endl;
}
#endif

void GUIFramework::handleKeyboardPress()
{
    while (!pressedKeys.empty())
    {
        int key = pressedKeys.front();
        pressedKeys.pop_front();
        if (((unsigned char)key == 'v') || ((unsigned char)key == 'V'))
        {
            storeFrames = !storeFrames;
			IVCP_LOG_INFO("\nKey V pressed - data capture is %d\n", (int)storeFrames);
        }  
        if ((unsigned char)key == 27)
        {
#ifdef ACATNODLL
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
			//cout << "In handleKeyboardPress - calling removeMappingGUI\n" << endl;
            removeMappingGUI(this);
#endif
            this->session->getVideoPlayer()->stop();
#else
			hideVideo();
#endif //ACATDLL
        }  
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
#if defined USE_DLIB || defined USE_REALSENSE
        if ((key == 'r') || (key == 'R'))
        {
            resetTracking();
        }
#endif
        if ((key == 'w') || (key == 'W'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    iter->second.regionOffsetY --;
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
        if ((key == 's') || (key == 'S'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    iter->second.regionOffsetY ++;
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
        if ((key == 'd') || (key == 'D'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    iter->second.regionOffsetX ++;
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
        if ((key == 'a') || (key == 'A'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    iter->second.regionOffsetX --;
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
        if ((key == 'T') || (key == 't'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    if (iter->second.regionExpansionHScale > 0.005)
                    {
                        iter->second.regionExpansionHScale -= (float)0.005;
                    }
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
        if ((key == 'G') || (key == 'g'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    if (iter->second.regionExpansionWScale > 0.005)
                    {
                        iter->second.regionExpansionWScale -= (float)0.005;
                    }
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
        if ((key == 'H') || (key == 'h'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    iter->second.regionExpansionWScale += (float)0.005;
                    break;
                } 
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;    
        }
        if ((key == 'Y') || (key == 'y'))
        {
            Settings settings;
            getPipelineSettings(pipeline, settings);
            int tmp = 0;
            for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
            {
                if (iter->first == currentRegion)
                {
                    iter->second.regionExpansionHScale += (float)0.005;
                    break;
                }   
            }
            setPipelineSettings(pipeline, settings);
            regionChanged = true;
            intChangedRegion = tmp;
        }
#endif
    }
}
void GUIFramework::setWindowName(string name)
{
    safeDestroyWindow(windowName);
    windowName = name;
}

string GUIFramework::getWindowName()
{
    return windowName;
}

bool GUIFramework::haveVideoOutput()
{
    return !outputFrames.empty();
}

void GUIFramework::generateVideoOutput(string filename, int fourcc, Size frameSize, int fps, VideoOutputType type, bool _convertTo8bit)
{
    Size max(0, 0);
    if(frameSize != Size(0, 0))
    {
        max = frameSize;
    }
    else
    {
        for(auto iter = outputFrames.begin(); iter != outputFrames.end(); iter++ )
        {
            if( max.height < iter->rows ) 
            {
                max.height = iter->rows;
            }
            if( max.width < iter->cols )
            {
                max.width = iter->cols;
            }
        }
    }

    closeVideoOutput();
    saveVideoOutputParams(filename, fourcc, max, fps, 0, type, _convertTo8bit);
    openVideoOutput();
        
    while(!outputFrames.empty())
    {
        outputFramesCount++;
        saveFrame(outputFrames.front());
        outputFrames.pop_front();
    }

    closeVideoOutput();
}

void GUIFramework::enableVideoOutput(std::string filename, int fourcc, cv::Size frameSize, int fps, int framesThreshold, VideoOutputType type, bool _convertTo8bit)
{
    saveVideoOutputParams(filename, fourcc, frameSize, fps, framesThreshold, type, _convertTo8bit);
	//cout << "Starting captor thread\n" << endl;
    captor = new std::thread(GUIFramework::captorLoop, this);
}

void GUIFramework::saveVideoOutputParams(std::string filename, int fourcc, cv::Size frameSize, int fps, int framesThreshold, VideoOutputType type, bool _convertTo8bit)
{
    outputSize = frameSize;
    outputFileName = filename;
    outputFourcc = fourcc;
    outputFps = fps;
    outputFramesThreshold = framesThreshold;
    outputFramesCount = 0;
    outputFilesCount = 0;
    outputFileType = type;
    storeFrames = false;
    convertTo8bit = _convertTo8bit;

	curOutputFileName = outputFileName;
}

void GUIFramework::openVideoOutput()
{
    if (!output->isOpened() && (outputFilesCount == 0) && !outputFileName.empty())
    {
        switch (outputFileType)
        {
        case ivcp::VIDEOFILE_OUTPUT:
            output = new AvifileVideoWriter();
            break;
        case ivcp::RAWFILE_OUTPUT:
            output = new RawfileVideoWriter();
            break;
        default:
            stringstream ss;
            ss << "Nonexistent video output file type: " << outputFileType;
            IVCP_EXCEPTION(IOException, ss.str());
            break;
        }
        output->open(outputFileName, outputFourcc, outputSize, outputFps);
        if(!output->isOpened())
        {
            stringstream ss;
            ss << "Unable to open video file: " << outputFileName;
            IVCP_EXCEPTION(IOException, ss.str());
        }
		else
			curOutputFileName = outputFileName;
    }
}

void GUIFramework::disableVideoOutput()
{
    captorStatus = THREAD_EXITING;
    cvCaptorReady.notify_one();

    if (captor != 0)
    {
        unique_lock<recursive_mutex> lk(GUIFramework::locker);
        cvCaptorTimeToExit.wait(lk, [&] { return captorStatus == THREAD_FINALIZED; } );
    }
    captor = 0;
    captorImages.clear();
}

void GUIFramework::closeVideoOutput()
{
    if (output->isOpened())
    {
        output->release();
        storeFrames = false;
    }    
}

void GUIFramework::storeToVideoOutput(Mat mat)
{
    string temp_filename = outputFileName;
    size_t dot_position = string::npos;

    openVideoOutput();
    
    if(!output->isOpened())
    {
        outputFrames.push_back(mat);
    }
    else
    {
        if ((outputFramesThreshold > 0 ) && (outputFramesCount == outputFramesThreshold))
        {
            outputFilesCount++;
            outputFramesCount = 0;

            dot_position = temp_filename.find_last_of('.');
            if (dot_position != string::npos)
            {
                temp_filename.insert(dot_position,std::to_string((unsigned long long)outputFilesCount));
            }
            else
            {
                temp_filename.append(std::to_string((unsigned long long)outputFilesCount));
            }
            
            if (output->isOpened())
            {
                output->release();
            }
            output->open(temp_filename, outputFourcc, outputSize, outputFps);
            if(!output->isOpened())
            {
                stringstream ss;
                ss << "Unable to open video file: " << temp_filename;
                IVCP_EXCEPTION(IOException, ss.str());
            }
			curOutputFileName = temp_filename;
//#ifndef JUST_RECORD
			cout << "Video capture to file: " << curOutputFileName << "\n";
//#endif
        }
        outputFramesCount++;
        saveFrame(mat);
    }
}

void GUIFramework::saveFrame(Mat mat)
{
    Mat temp;
    if (mat.size() != outputSize)
    {
        resize(mat, temp, outputSize);
    }
    else
    {
        temp = mat;
    }
    //If 16 bit split by channels and store as a color image for avi output
    if (temp.depth() == CV_16UC1 && temp.channels() == 1 && outputFileType == VIDEOFILE_OUTPUT)
    {
        if (convertTo8bit)
        {
            Mat temp8bit(outputSize, CV_8UC1);
            temp.convertTo(temp8bit, CV_8UC1, 256.0/(max16bitValue - min16BitValue + 1));
            cvtColor(temp8bit, temp8bit, cv::COLOR_GRAY2BGR);
            output->write(temp8bit);
        }
        else
        {
            if (mergedImage.type() != CV_8UC3 && temp.size() != mergedImage.size())
            {
                mergedImage = Mat(temp.size(), CV_8UC3);
            }

            for (int r = 0; r < temp.rows; r++)
            {
                for (int c = 0; c < temp.cols; c++)
                {
                    unsigned short val = temp.at<unsigned short>(r, c);
                    unsigned char b0 = 0, b1 = 0, b2 = 0;

                    b0 = val & 255;
                    b1 = (val & 3840) >> 8;

                    mergedImage.at<cv::Vec3b>(r, c)[0] = b0;
                    mergedImage.at<cv::Vec3b>(r, c)[1] = b1;
                    mergedImage.at<cv::Vec3b>(r, c)[2] = b2;
                }
            }
            output->write(mergedImage);
        }
    }
    else
    {
        output->write(temp);
    }
}

void GUIFramework::captorLoop(GUIFramework* pGui)
{
    deque<Mat> currentCaptorImages;
    while (true)
    {
        if (pGui->captorStatus == THREAD_EXITING)
        {
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
            while (!pGui->captorImages.empty())
            {
                pGui->storeToVideoOutput(pGui->captorImages.front());
                pGui->captorImages.pop_front();
            }

            pGui->closeVideoOutput();

            pGui->captorStatus = THREAD_FINALIZED;
            pGui->cvCaptorTimeToExit.notify_one();
            return;
        }
        {
            unique_lock<recursive_mutex> lk(GUIFramework::locker);
            if (!pGui->isCaptorReady)
            {
                pGui->cvCaptorReady.wait(lk, [&] {
                if (pGui->captorStatus == THREAD_RUNNING)
                    {
                        return (pGui->isCaptorReady == true);
                    }
                    else
                        return true;
                    });
            }
            if (pGui->isCaptorReady)
            {
                if (!pGui->captorImages.empty())
                {
                    currentCaptorImages = pGui->captorImages;
                    pGui->captorImages.clear();
                }
                pGui->isCaptorReady = false;
            }
        }
        while (!currentCaptorImages.empty())
        {
            pGui->storeToVideoOutput(currentCaptorImages.front());
            currentCaptorImages.pop_front();
        }
    }
}

void GUIFramework::normalizeBrightness(const Mat &src16bit, Mat &dst8bit)
{
    if (frameNum >= START_FRAME_NORMALIZATION)
    {
        if (normFrames > 0 && (frameCounter16Bit % normFrames == 0))
        {
            double minVal = 0.0;
            double maxVal = 4095.0;
            minMaxLoc(src16bit, &minVal, &maxVal);

            min16BitValue = (unsigned int)minVal;
            max16bitValue = (unsigned int)maxVal;
        }


        src16bit -= min16BitValue;
        src16bit.convertTo(dst8bit, CV_8UC1, 256.0/(max16bitValue - min16BitValue + 1));

        frameCounter16Bit++;
    }
    else
    {
        dst8bit = src16bit;
    }
}

int GUIFramework::getNormFrames()
{
    return normFrames;
}

std::string GUIFramework::getCurOutputFilename()
{
	return curOutputFileName;
}
int GUIFramework::getOutputFramesCount()
{
	return outputFramesCount;
}

#if 1//SSdebug
void GUIFramework::setWorkerThreadStatusToExiting()
{
	lock_guard<recursive_mutex> lk(GUIFramework::locker);  
	workerStatus = THREAD_EXITING; //THREAD_FINALIZED;
	//cvTimeToExit.notify_one();
	cvWorkerReady.notify_one();
	//cout << "workerStatus = THREAD_EXITING\n";
}
#endif

void GUIFramework::setNormFrames(int _normFrames)
{
    normFrames = _normFrames;
}
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32

int GUIFramework::ownWaitKey(int delay, std::string name)
{
    int time0 = GetTickCount();

    for(;;)
    {
        HWND hwnd = FindWindow(NULL, (LPCSTR)name.c_str());
        MSG message;
        int isProcessed = 0;

        if( (delay > 0 && abs((int)(GetTickCount() - time0)) >= delay))
        {
            return -1;
        }

        if( delay <= 0 )
        {
            GetMessage(&message, 0, 0, 0);
        }
        else if( PeekMessage(&message, 0, 0, 0, PM_REMOVE) == FALSE )
        {
            Sleep(1);
            continue;
        }

        if( hwnd == message.hwnd)
        {
            isProcessed = 1;
            switch(message.message)
            {
            case WM_DESTROY:
            case WM_CHAR:
                DispatchMessage(&message);
                return (int)message.wParam;

            case WM_SYSKEYDOWN:
                if( message.wParam == VK_F10 )
                {
                    isProcessed = 1;
                    return (int)(message.wParam << 16);
                }
                break;

            case WM_KEYDOWN:
                TranslateMessage(&message);
                if( (message.wParam >= VK_F1 && message.wParam <= VK_F24) ||
                    message.wParam == VK_HOME || message.wParam == VK_END ||
                    message.wParam == VK_UP || message.wParam == VK_DOWN ||
                    message.wParam == VK_LEFT || message.wParam == VK_RIGHT ||
                    message.wParam == VK_INSERT || message.wParam == VK_DELETE ||
                    message.wParam == VK_PRIOR || message.wParam == VK_NEXT )
                {
                    DispatchMessage(&message);
                    isProcessed = 1;
                    return (int)(message.wParam << 16);
                }
            default:
                DispatchMessage(&message);
                isProcessed = 1;
                break;
            }    
        }

        if( !isProcessed )
        {
            TranslateMessage(&message);
            DispatchMessage(&message);
        }
    }
}

#endif 

void GUIFramework::getPipelineSettings(SPtr<IPipeline> settingsPipeline, Settings& settings) const
{
    SPtr<Session> settingsSession = settingsPipeline->getSession();
    settings.gestures.clear();
    settings.regions.clear();


#ifdef USE_DLIB
    if (Factory::isDlibHandlerCreated())
    {
        bool isAnyDlibRegions = false;
        for (int i = 0; i < settingsSession->getNumberOfRegionCreators(); i++)
        {
            SPtr<RegionCreator> nregion = settingsSession->getRegionCreator(i);
            DlibRegionCreator* region = dynamic_cast<DlibRegionCreator*>((RegionCreator*)nregion);
            if(region)
            {
                isAnyDlibRegions = true;
                string name = region->getId();
                DlibRegionCreator::Settings regionSettings = region->getSearchRegionSettings();
                settings.regions[name].regionExpansionHScale = regionSettings.regionExpansionHScale;
                settings.regions[name].regionExpansionWScale = regionSettings.regionExpansionWScale;
                settings.regions[name].regionOffsetX = regionSettings.regionOffsetX ;
                settings.regions[name].regionOffsetY = regionSettings.regionOffsetY;
            }
        }
        if (isAnyDlibRegions)
        {
            SPtr<IDlib> dlibH = Factory::getCachedDlib();
            IDlib::Settings dlibSettings = dlibH->getHandlerSettings();
            settings.stabFrames = dlibSettings.stabFrames;
            settings.stabThreshold = dlibSettings.stabThreshold;
        }
    }
#endif // USE_DLIB

#ifdef USE_REALSENSE
    if (Factory::isRealSenseHandlerCreated())
    {
        bool isAnyRsRegions = false;
        for (int i = 0; i < settingsSession->getNumberOfRegionCreators(); i++)
        {
            SPtr<RegionCreator> nregion = settingsSession->getRegionCreator(i);
            RealSenseRegionCreator* region = dynamic_cast<RealSenseRegionCreator*>((RegionCreator*)nregion);
            if(region)
            {
                isAnyRsRegions = true;
                string name = region->getId();
                RealSenseRegionCreator::Settings regionSettings = region->getSearchRegionSettings();
                settings.regions[name].regionExpansionHScale = regionSettings.regionExpansionHScale;
                settings.regions[name].regionExpansionWScale = regionSettings.regionExpansionWScale;
                settings.regions[name].regionOffsetX = regionSettings.regionOffsetX ;
                settings.regions[name].regionOffsetY = regionSettings.regionOffsetY;
            }
        }
        if (isAnyRsRegions)
        {
            SPtr<IRealSense> rsH = Factory::getCachedRealSense();
            IRealSense::Settings rsSettings = rsH->getHandlerSettings();
            settings.stabFrames = rsSettings.stabFrames;
            settings.stabThreshold = rsSettings.stabThreshold;
        }
    }
#endif // USE_REALSENSE

    settings.sensitivity = getCurrentSensitivity();
    settings.gestures = getGesturesSettings();
}

void GUIFramework::setPipelineSettings(SPtr<IPipeline> settingsPipeline, const Settings& settings)
{
    SPtr<Session> settingsSession = settingsPipeline->getSession();



#ifdef USE_DLIB
    if (Factory::isDlibHandlerCreated())
    {
        std::set<string> names;
        bool isAnyDlibRegions = false;
        for (int i = 0; i < settingsSession->getNumberOfRegionCreators(); i++)
        {
            SPtr<RegionCreator> region = settingsSession->getRegionCreator(i);
            names.insert(region->getId());
        }

        for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter)
        {
            auto namesIt = names.find(iter->first);
            if(namesIt != names.end())
            {
                SPtr<DlibRegionCreator> region = settingsSession->getRegionCreator(iter->first);
                if(region)
                {
                    isAnyDlibRegions = true;
                    DlibRegionCreator::Settings regionSettings = region->getSearchRegionSettings();
                    regionSettings.regionExpansionHScale = iter->second.regionExpansionHScale;
                    regionSettings.regionExpansionWScale = iter->second.regionExpansionWScale;
                    regionSettings.regionOffsetX = iter->second.regionOffsetX;
                    regionSettings.regionOffsetY = iter->second.regionOffsetY;
                    regionSettings.stabFrames = settings.stabFrames;
                    regionSettings.stabThreshold = settings.stabThreshold;
                    region->setSearchRegionSettings(regionSettings);
                }
                else
                {
                    IVCP_LOG_WARNING("Can't change settings for region \"%s\" because it is not DlibRegionCreator.", iter->first.c_str());
                }
            }
        }
        if (isAnyDlibRegions)
        {
            SPtr<IDlib> dlibH = Factory::getCachedDlib();
            IDlib::Settings dlibSettings = dlibH->getHandlerSettings();
            dlibSettings.stabFrames = settings.stabFrames;
            dlibSettings.stabThreshold = settings.stabThreshold;
            dlibH->setHandlerSettings(dlibSettings);
        }
    }
#endif // USE_DLIB

#ifdef USE_REALSENSE
    if (Factory::isRealSenseHandlerCreated())
    {
        std::set<string> names;
        bool isAnyRsRegions = false;
        for (int i = 0; i < settingsSession->getNumberOfRegionCreators(); i++)
        {
            SPtr<RegionCreator> region = settingsSession->getRegionCreator(i);
            names.insert(region->getId());
        }

        for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter)
        {
            auto namesIt = names.find(iter->first);
            if(namesIt != names.end())
            {
                SPtr<RealSenseRegionCreator> region = settingsSession->getRegionCreator(iter->first);
                if(region)
                {
                    isAnyRsRegions = true;
                    RealSenseRegionCreator::Settings regionSettings = region->getSearchRegionSettings();
                    regionSettings.regionExpansionHScale = iter->second.regionExpansionHScale;
                    regionSettings.regionExpansionWScale = iter->second.regionExpansionWScale;
                    regionSettings.regionOffsetX = iter->second.regionOffsetX;
                    regionSettings.regionOffsetY = iter->second.regionOffsetY;
                    regionSettings.stabFrames = settings.stabFrames;
                    regionSettings.stabThreshold = settings.stabThreshold;
                    region->setSearchRegionSettings(regionSettings);
                }
                else
                {
                    IVCP_LOG_WARNING("Can't change settings for region \"%s\" because it is not RealSenseRegionCreator.", iter->first.c_str());
                }
            }
        }
        if (isAnyRsRegions)
        {
            SPtr<IRealSense> rsH = Factory::getCachedRealSense();
            IRealSense::Settings rsSettings = rsH->getHandlerSettings();
            rsSettings.stabFrames = settings.stabFrames;
            rsSettings.stabThreshold = settings.stabThreshold;
            rsH->setHandlerSettings(rsSettings);
        }
    }
#endif // USE_REALSENSE
    
    setSensitivitySettings(settings.sensitivity);
    setGesturesSettings(settings.gestures);
}

void GUIFramework::writeSettings(const Settings& settings, const string& id, cv::FileStorage& fs) const
{
    fs << id;
    fs << "{" << "auto_exposure" << ((int)settings.autoExposure);
    fs << "exposure" << settings.exposure;
    fs << "sensitivity" << settings.sensitivity;
    fs << "gestures" << "[";
    
    for (auto iter = settings.gestures.begin(); iter != settings.gestures.end(); ++iter)
    {
        fs << "{" <<"id" << iter->first;
        fs        << "value"<< ((int)iter->second);
        fs << "}";
    }
    fs << "]";
    fs << "stab_frames" <<(int)settings.stabFrames; 
    fs << "stab_threshold" <<settings.stabThreshold;
    fs << "regions" << "[";
    for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter)
    {
        fs << "{" << "id" << iter->first;
        fs        << "offsetX" << iter->second.regionOffsetX;
        fs        << "offsetY" << iter->second.regionOffsetY;
        fs        << "scaleH" << iter->second.regionExpansionHScale;
        fs        << "scaleW" << iter->second.regionExpansionWScale;        
        fs << "}";
    }
    fs << "]";
    fs<<"}";
}

void GUIFramework::readSettings(Settings& settings, const string& id, cv::FileStorage& fs) const
{
    FileNode n = fs[id];
    settings.autoExposure = (((int)(n["auto_exposure"])) != 0);
    settings.exposure = n["exposure"];
    settings.sensitivity = n["sensitivity"];
    settings.gestures.clear();
    FileNode gn = n["gestures"];
    for(FileNodeIterator  it = gn.begin(); it != gn.end(); ++it)
    {
        FileNode ign = (FileNode)*it;
        string name = ign["id"];
        settings.gestures[name] = (((int)(ign["value"])) != 0);
    }
    int stabFrames = 0;
    stabFrames = n["stab_frames"];
    settings.stabFrames = stabFrames;
    settings.stabThreshold = n["stab_threshold"];
    settings.regions.clear();
    FileNode rn = n["regions"];
    for(FileNodeIterator  it = rn.begin(); it != rn.end(); ++it)
    {
        FileNode irn = (FileNode)*it;
        string name = irn["id"];
        settings.regions[name].regionOffsetX = irn["offsetX"];
        settings.regions[name].regionOffsetY = irn["offsetY"];
        settings.regions[name].regionExpansionHScale = irn["scaleH"];
        settings.regions[name].regionExpansionWScale = irn["scaleW"];        
    }
}

void GUIFramework::setDefaultSettings(IGUIFramework::Settings settings)
{
    defaultSettings = settings;
}

void GUIFramework::setSettings(IGUIFramework::Settings settings)
{
    setPipelineSettings(pipeline, settings);
}

IGUIFramework::Settings GUIFramework::getDefaultSettings() const
{
    return defaultSettings;
}

IGUIFramework::Settings GUIFramework::getSettings() const
{
    IGUIFramework::Settings settings; 
    getPipelineSettings(pipeline, settings);
    return settings;
}

void GUIFramework::replaceSettingsWithDefaults()
{
    setPipelineSettings(pipeline, this->defaultSettings);
    setGesturesSettings(defaultSettings.gestures);
    setSensitivitySettings(defaultSettings.sensitivity);
}

void GUIFramework::setSensitivitySettings(int sens)
{
    sensitivityCurrent = sens;
    if(sensitivityCurrent > 50)
    {
        sensitivityCurrent = 50;
    }
    if(sensitivityCurrent < 5)
    {
        sensitivityCurrent = 5;
    }
}

void GUIFramework::saveSettings(const std::string& filename) const
{
    try
    {
        FileStorage fs(filename, FileStorage::WRITE);

        if (fs.isOpened())
        {
            Settings currentSettings;
            getPipelineSettings(pipeline, currentSettings);
            writeSettings(currentSettings, CURRENT, fs);
            writeSettings(defaultSettings, DEFAULT, fs);
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
            if (guiListenSocket != INVALID_SOCKET)
            {
                writeSettingsPort(fs);
            }
#endif
            fs.release();
        }
        else
        {
            stringstream ss;
            ss << "Unable to save GUI settings to file: " << filename;
            IVCP_EXCEPTION(IOException, ss.str());
        }
    }
    catch(Exception &ex)
    {
        stringstream ss;
        ss << "Error occured when tried to save GUI settings to file: " << filename << ". Exception message: " << ex.what();
        IVCP_EXCEPTION(IOException, ss.str());
    }
}

void GUIFramework::loadSettings(const std::string& filename) 
{
    try
    {
        FileStorage fs(filename, FileStorage::READ);

        if (fs.isOpened())
        {
            Settings newSettings;
            readSettings(newSettings, CURRENT, fs);
            setPipelineSettings(pipeline, newSettings);
            readSettings(defaultSettings, DEFAULT, fs);
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
            readSettingsPort(fs);
#endif
            fs.release();
        }
        else
        {
            stringstream ss;
            ss << "Unable to read GUI settings from file: " << filename;
            IVCP_EXCEPTION(IOException, ss.str());
        }
    }
    catch (Exception &ex)
    {
        stringstream ss;
        ss << "Error occured when tried to read GUI settings from file: " << filename << ". Exception message: " << ex.what();
        IVCP_EXCEPTION(IOException, ss.str());
    }
}

#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32

void GUIFramework::writeSettingsPort(cv::FileStorage& fs) const
{
    fs << "Port" << port;
}

void GUIFramework::readSettingsPort(cv::FileStorage& fs)
{
    FileNode n = fs["Port"];
    this->port = (int)n;
}

LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    char pressedKey = ' ';
    KBDLLHOOKSTRUCT *pKeyBoard = (KBDLLHOOKSTRUCT *)lParam;
    switch(wParam)
    {
    case WM_KEYUP:
    {
        pressedKey = (char)pKeyBoard->vkCode;
        if (pKeyBoard->vkCode == VK_LCONTROL || pKeyBoard->vkCode == VK_RCONTROL)
        {
            GUIFramework::ctrlPressed = false;
        }

        if (pKeyBoard->vkCode == VK_LMENU || pKeyBoard->vkCode == VK_RMENU)
        {
            GUIFramework::altPressed = false;
        }

        if (pKeyBoard->vkCode == VK_LSHIFT || pKeyBoard->vkCode == VK_RSHIFT)
        {
            GUIFramework::shiftPressed = false;
        }
    }
    break;
    
    case WM_KEYDOWN:
    {
        if (pKeyBoard->vkCode == VK_LCONTROL || pKeyBoard->vkCode == VK_RCONTROL)
        {
            GUIFramework::ctrlPressed = true;
        }

        if (pKeyBoard->vkCode == VK_LMENU || pKeyBoard->vkCode == VK_RMENU)
        {
            GUIFramework::altPressed = true;
        }

        if (pKeyBoard->vkCode == VK_LSHIFT || pKeyBoard->vkCode == VK_RSHIFT)
        {
            GUIFramework::shiftPressed = true;
        }
    }
    break;
    default:
        return CallNextHookEx(NULL, nCode, wParam, lParam);
    break;
    }

    //Show/hide GUIFramework window
    if (GUIFramework::ctrlPressed && GUIFramework::shiftPressed && GUIFramework::altPressed && (pressedKey == 'W'))
    {
        HWND hwnd = GetForegroundWindow();
        GUIFramework* thisGUI = GUIFramework::getMappingGUI(hwnd);
        if (thisGUI != NULL)
        {
            thisGUI->isShown = false;
        }
        else
        {
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
            bool isEveryWindowHide = true;
            std::map<HWND, GUIFramework*>::iterator iter;
            for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
            {
                if (iter->second->isShown == true)
                {
                    isEveryWindowHide = false;
                    break;
                }
            }

            //all windows are hiden -> show them
            if (isEveryWindowHide == true)             
            {
                for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
                {
                    iter->second->isShown = true;
                }
            }
        }
 	}

    //Show/hide settings window
    if (GUIFramework::ctrlPressed && GUIFramework::shiftPressed && GUIFramework::altPressed && (pressedKey == 'K'))
    {
        HWND hwnd = GetForegroundWindow();
        GUIFramework* thisGUI = GUIFramework::getMappingGUI(hwnd);

        if (thisGUI != NULL) // GUIFramework is in focus
        {
            thisGUI->isSettingsShow = !thisGUI->isSettingsShow;
            HWND hwndSet = GUIFramework::findSettingsHandler(thisGUI);
            if (thisGUI->isSettingsShow == true)
            {
                PostMessage(hwndSet, WM_SYSCOMMAND, SC_RESTORE, NULL);
                thisGUI->fillSettingWindow(hwndSet);
            }
            else
            {
                PostMessage(hwndSet, WM_CLOSE, NULL, NULL);
            }
        }
        else
        {
            GUIFramework* settingsGUI = GUIFramework::getMapping(hwnd);
            if (settingsGUI != NULL) // Settings GUI is in focus
            {
                settingsGUI->isSettingsShow = false;
                PostMessage(hwnd, WM_CLOSE, NULL, NULL);
            }
        }
    }

    return CallNextHookEx(NULL, nCode, wParam, lParam);
}

void GUIFramework::createSettingWindow(GUIFramework* gui)
{
    WNDCLASSW wc = {0};
    wc.lpszClassName = L"ColorWindows";
    wc.hbrBackground = GetSysColorBrush(COLOR_3DFACE);
    wc.lpfnWndProc   = WndProc;
    wc.hCursor       = LoadCursor(0, IDC_ARROW);
  
    RegisterClassW(&wc);

    
    std::string tmp("Settings ");
    tmp.append(gui->getWindowName());
    const char* charArray = tmp.c_str();
    wchar_t wString[4096];
    MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, 4096);
    HWND hwnd = CreateWindowExW(NULL, wc.lpszClassName, wString, WS_OVERLAPPEDWINDOW & ~WS_THICKFRAME & ~WS_MAXIMIZEBOX,
        100, 100, 400, 500, 0, 0, NULL, 0);

    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    GUIFramework::setMapping(hwnd, gui);

    gui->fillSettingWindow(hwnd);
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{    
    switch(msg)  
    {
        case WM_CREATE:
        {
            GUIFramework::createControls(hwnd);
        }
        break;

        case WM_HSCROLL:
        {
            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
            if ((HWND)lParam == GetDlgItem(hwnd, HWND_SENSITIBITY_CONTROL))
            {
                thisGUI->updateSensitivityLabel(hwnd);
            }
            else 
            {
                if ((HWND)lParam == GetDlgItem(hwnd, HWND_EXPOSURE_CONTROL))
                {
                    thisGUI->updateExposureLabel(hwnd);
                }
            }
        }
        break;

        case WM_COMMAND:
        {
            GUIFramework::updateControls(hwnd, wParam);
        }
        break;

        
        case WM_DESTROY:
        {
            PostQuitMessage(0);

            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
            thisGUI->isSettingsShow = false;
            GUIFramework::removeMapping(hwnd);

            thisGUI->createSettingWindow(thisGUI);
        }
        break;
        
    }
    return DefWindowProcW(hwnd, msg, wParam, lParam);
}

void GUIFramework::createControls(HWND hwnd)
{
    CreateWindowW(L"STATIC", L"Camera setting", WS_CHILD | WS_VISIBLE | SS_LEFT,
        0, 0, 110, 20, hwnd, (HMENU)HWND_CAMERA_SETTINGS, NULL, NULL);
    CreateWindowW(L"button", L"ROI-based auto exposure", WS_VISIBLE | WS_CHILD | BS_CHECKBOX,
        0, 20, 200, 20, hwnd, (HMENU)HWND_AUTO_EXPOSURE, NULL, NULL);
    CheckDlgButton(hwnd, HWND_AUTO_EXPOSURE, BST_CHECKED);
    CreateWindowW(L"STATIC", L"Manual", WS_CHILD | WS_VISIBLE | SS_LEFT,
        0, 40, 60, 20, hwnd, (HMENU)HWND_MANUAL, NULL, NULL);
    HWND hLeftLabel1 = CreateWindowW(L"STATIC", L"10", WS_CHILD | WS_VISIBLE, 
        70, 40, 30, 30, hwnd, (HMENU)HWND_EXPOSURE_LEFT, NULL, NULL);
	HWND hRightLabel1 = CreateWindowW(L"STATIC", L"90", WS_CHILD | WS_VISIBLE, 
        70, 40, 30, 30, hwnd, (HMENU)HWND_EXPOSURE_RIGHT, NULL, NULL);
    CreateWindowW(L"STATIC", L"10", WS_CHILD | WS_VISIBLE, 
        290, 40, 170, 30, hwnd, (HMENU)HWND_EXPOSURE_POSITION, NULL, NULL);
    INITCOMMONCONTROLSEX icex1;
    icex1.dwSize = sizeof(INITCOMMONCONTROLSEX);
    icex1.dwICC  = ICC_LISTVIEW_CLASSES;
    InitCommonControlsEx(&icex1); 
    HWND hTrack1 = CreateWindowW(L"msctls_trackbar32", L"Trackbar Control", WS_CHILD | WS_VISIBLE,
        90, 40, 170, 30, hwnd, (HMENU)HWND_EXPOSURE_CONTROL, NULL, NULL);
    SendMessageW(hTrack1, TBM_SETRANGE,  TRUE, MAKELONG(10, 90)); 
    SendMessageW(hTrack1, TBM_SETPAGESIZE, 10, 90); 
    SendMessageW(hTrack1, TBM_SETTICFREQ, 10, 0); 
    SendMessageW(hTrack1, TBM_SETPOS, FALSE, 10); 
    SendMessageW(hTrack1, TBM_SETBUDDY, TRUE, (LPARAM) hLeftLabel1); 
    SendMessageW(hTrack1, TBM_SETBUDDY, FALSE, (LPARAM) hRightLabel1); 
    
    CreateWindowW(L"STATIC", L"Gesture", WS_CHILD | WS_VISIBLE | SS_LEFT,
        0, 70, 110, 20, hwnd, (HMENU)HWND_GESTURE_LABEL, NULL, NULL);
    HWND hwndComboGest = CreateWindow(TEXT("combobox"), NULL, WS_CHILD | WS_VISIBLE | CBS_DROPDOWNLIST | WS_VSCROLL,
        0, 90, 140, 300, hwnd, (HMENU)HWND_GESTURE_COMBOBOX, NULL, NULL);
    SendMessage(hwndComboGest, CB_SETCURSEL, (WPARAM)0, (LPARAM)0);
    CreateWindowW(L"button", L"Enable", WS_VISIBLE | WS_CHILD | BS_CHECKBOX,
        150, 90, 120, 20, hwnd, (HMENU)HWND_GESTURE_CHECK_BOX, NULL, NULL);

    CreateWindowW(L"STATIC", L"Region settings", WS_CHILD | WS_VISIBLE | SS_LEFT,
        0, 120, 110, 20, hwnd, (HMENU)HWND_REGION_LABEL, NULL, NULL);
    HWND hwndComboRegion = CreateWindow(TEXT("combobox"), NULL, WS_CHILD | WS_VISIBLE | CBS_DROPDOWNLIST | WS_VSCROLL,
        0, 140, 140, 110, hwnd, (HMENU)HWND_REGION_COMBO_BOX, NULL, NULL);
    SendMessage(hwndComboRegion, CB_SETCURSEL, (WPARAM)0, (LPARAM)0);
    CreateWindowW(L"STATIC", L"Offset X", WS_CHILD | WS_VISIBLE | SS_LEFT,
        150, 140, 100, 20, hwnd, (HMENU)HWND_REGION_OFFSETX_LABEL, NULL, NULL);
    CreateWindowW(L"Edit", L"0", WS_CHILD | WS_VISIBLE | WS_BORDER,
        280, 140, 100, 20, hwnd, (HMENU)HWND_REGION_OFFSETX, NULL, NULL);
    CreateWindowW(L"STATIC", L"Offset Y", WS_CHILD | WS_VISIBLE | SS_LEFT,
        150, 160, 100, 20, hwnd, (HMENU)HWND_REGION_OFFSETY_LABEL, NULL, NULL);
    CreateWindowW(L"Edit", L"0", WS_CHILD | WS_VISIBLE | WS_BORDER,
        280, 160, 100, 20, hwnd, (HMENU)HWND_REGION_OFFSETY, NULL, NULL);
    CreateWindowW(L"STATIC", L"Scale H", WS_CHILD | WS_VISIBLE | SS_LEFT,
        150, 180, 100, 20, hwnd, (HMENU)HWND_REGION_SCALEH_LABEL, NULL, NULL);
    CreateWindowW(L"Edit", L"0", WS_CHILD | WS_VISIBLE | WS_BORDER,
        280, 180, 100, 20, hwnd, (HMENU)HWND_REGION_SCALEH, NULL, NULL);
    CreateWindowW(L"STATIC", L"Scale W", WS_CHILD | WS_VISIBLE | SS_LEFT,
        150, 200, 100, 20, hwnd, (HMENU)HWND_REGION_SCALEW_LABEL, NULL, NULL);
    CreateWindowW(L"Edit", L"0", WS_CHILD | WS_VISIBLE | WS_BORDER,
        280, 200, 100, 20, hwnd, (HMENU)HWND_REGION_SCALEW, NULL, NULL);
    CreateWindowW(L"STATIC", L"Stabframe", WS_CHILD | WS_VISIBLE | SS_LEFT,
        150, 220, 100, 20, hwnd, (HMENU)HWND_REGION_STABFRAME_LABEL, NULL, NULL);
    CreateWindowW(L"Edit", L"0", WS_CHILD | WS_VISIBLE | WS_BORDER,
        280, 220, 100, 20, hwnd, (HMENU)HWND_REGION_STABFRAME, NULL, NULL);
    CreateWindowW(L"STATIC", L"Stabthreshold", WS_CHILD | WS_VISIBLE | SS_LEFT,
        150, 240, 100, 20, hwnd, (HMENU)HWND_REGION_STABTHRESHOLD_LABEL, NULL, NULL);
    CreateWindowW(L"Edit", L"0", WS_CHILD | WS_VISIBLE | WS_BORDER,
        280, 240, 100, 20, hwnd, (HMENU)HWND_REGION_STABTHRESHOLD, NULL, NULL);
    CreateWindowW(L"button", L"Reset tracking", WS_VISIBLE | WS_CHILD,
        10, 320, 360, 25, hwnd, (HMENU)HWND_BUTTON_RESET, NULL, NULL);
    CreateWindowW(L"button", L"Apply", WS_VISIBLE | WS_CHILD, 
        195, 350, 175, 25, hwnd, (HMENU)HWND_BUTTON_APPLY, NULL, NULL);
    CreateWindowW(L"button", L"Default", WS_VISIBLE | WS_CHILD,
        10, 350, 175, 25, hwnd, (HMENU)HWND_BUTTON_DEFAULT, NULL, NULL);

    CreateWindowW(L"STATIC", L"Sensitivity", WS_CHILD | WS_VISIBLE | SS_LEFT,
        0, 260, 70, 30, hwnd, (HMENU)HWND_SENSITIBITY_LABEL, NULL, NULL);

	HWND hLeftLabel = CreateWindowW(L"STATIC", L"5", WS_CHILD | WS_VISIBLE, 
        0, 280, 30, 30, hwnd, (HMENU)HWND_SENSITIBITY_LEFT, NULL, NULL);
	HWND hRightLabel = CreateWindowW(L"STATIC", L"50", WS_CHILD | WS_VISIBLE, 
        0, 280, 30, 30, hwnd, (HMENU)HWND_SENSITIBITY_RIGHT, NULL, NULL);
    CreateWindowW(L"STATIC", L"5", WS_CHILD | WS_VISIBLE, 
        270, 280, 30, 30, hwnd, (HMENU)HWND_SENSITIBITY_POSITION, NULL, NULL);

    INITCOMMONCONTROLSEX icex;
    icex.dwSize = sizeof(INITCOMMONCONTROLSEX);
    icex.dwICC  = ICC_LISTVIEW_CLASSES;
    InitCommonControlsEx(&icex); 
	HWND hTrack = CreateWindowW(L"msctls_trackbar32", L"Trackbar Control", WS_CHILD | WS_VISIBLE | TBS_AUTOTICKS,
    30, 280, 170, 30, hwnd, (HMENU)HWND_SENSITIBITY_CONTROL, NULL, NULL);
	SendMessageW(hTrack, TBM_SETRANGE,  TRUE, MAKELONG(5, 50)); 
    SendMessageW(hTrack, TBM_SETPAGESIZE, 5, 50); 
    SendMessageW(hTrack, TBM_SETTICFREQ, 5, 0);     
    SendMessageW(hTrack, TBM_SETPOS, FALSE, 0); 
    SendMessageW(hTrack, TBM_SETBUDDY, TRUE, (LPARAM) hLeftLabel); 
    SendMessageW(hTrack, TBM_SETBUDDY, FALSE, (LPARAM) hRightLabel); 

    CreateWindowW(L"button", L"Save settings", WS_VISIBLE | WS_CHILD, 
        10, 390, 360, 25, hwnd, (HMENU)HWND_BUTTON_SAVE, NULL, NULL);
    CreateWindowW(L"button", L"Load settings", WS_VISIBLE | WS_CHILD, 
        10, 420, 360, 25, hwnd, (HMENU)HWND_BUTTON_LOAD, NULL, NULL);
}

void GUIFramework::updateSensitivityLabel(HWND hwnd)
{
    HWND hwndSlider = GetDlgItem(hwnd, HWND_SENSITIBITY_CONTROL);
    LRESULT pos = SendMessage (hwndSlider, TBM_GETPOS, 0, 0);
    wchar_t buf[4];
    wsprintfW(buf, L"%ld", pos);
    HWND hlbl = GetDlgItem(hwnd, HWND_SENSITIBITY_POSITION);
    SetWindowTextW(hlbl, buf);
    setSensitivitySettings((int)pos);
}

void GUIFramework::updateExposureLabel(HWND hwnd)
{
    HWND hwndSlider = GetDlgItem(hwnd, HWND_EXPOSURE_CONTROL);
    LRESULT pos = SendMessage (hwndSlider, TBM_GETPOS, 0, 0);
    wchar_t buf[4];
    wsprintfW(buf, L"%ld", pos);
    HWND hlbl = GetDlgItem(hwnd, HWND_EXPOSURE_POSITION);
    SetWindowTextW(hlbl, buf);
    GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
    Settings settings;
    thisGUI->getPipelineSettings(thisGUI->pipeline, settings);
    settings.exposure = (int)pos;
    thisGUI->setPipelineSettings(thisGUI->pipeline, settings);
}

void GUIFramework::updateControls(HWND hwnd, WPARAM wParam)
{
    switch LOWORD(wParam)
    {
        case HWND_AUTO_EXPOSURE:
        {
            HWND hwndCheck = GetDlgItem(hwnd, HWND_AUTO_EXPOSURE);
            LRESULT res = SendMessage (hwndCheck, BM_GETCHECK, 0, 0);

            Settings settings;
            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
            thisGUI->getPipelineSettings(thisGUI->pipeline, settings);
            if (res == BST_CHECKED)
            {
                CheckDlgButton(hwnd, HWND_AUTO_EXPOSURE, BST_UNCHECKED); //manual settings
                settings.autoExposure = false;
            }
            else
            {
                CheckDlgButton(hwnd, HWND_AUTO_EXPOSURE, BST_CHECKED); //autoexposure
                settings.autoExposure = true;
            }
            
            thisGUI->setPipelineSettings(thisGUI->pipeline, settings);
        }
        break;

        case HWND_GESTURE_CHECK_BOX:
        {
            HWND hwndCheck = GetDlgItem(hwnd, HWND_GESTURE_CHECK_BOX);
            LRESULT res = SendMessage (hwndCheck, BM_GETCHECK, 0, 0);
            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
            std::map<std::string, bool> gest = thisGUI->getGesturesSettings();
            LRESULT sel = SendMessage(GetDlgItem(hwnd, HWND_GESTURE_COMBOBOX), CB_GETCURSEL, 0, 0);
            int tmp = 0;
            for (auto iter = gest.begin(); iter != gest.end(); ++iter, tmp++)
            {
                if (tmp == sel)
                {
                    if (res == BST_UNCHECKED)
                    {
                        CheckDlgButton(hwnd, HWND_GESTURE_CHECK_BOX, BST_CHECKED);
                        iter->second = true;
                    }
                    else
                    {
                        CheckDlgButton(hwnd, HWND_GESTURE_CHECK_BOX, BST_UNCHECKED);
                        iter->second = false;
                    }
                    break;
                }
            }
            //the current gesture changes its value
            thisGUI->setGesturesSettings(gest);
        }
        break;

        case HWND_GESTURE_COMBOBOX:
        {
            if (HIWORD(wParam) == CBN_SELCHANGE) 
            {  
                HWND hwndCheck = GetDlgItem(hwnd, HWND_GESTURE_COMBOBOX);
                LRESULT sel = SendMessage(hwndCheck, CB_GETCURSEL, 0, 0);
                GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
                std::map<std::string, bool> gest = thisGUI->getGesturesSettings();
                int tmp = 0;
                for (auto iter = gest.begin(); iter != gest.end(); ++iter, tmp++)
                {
                    if (tmp == sel)
                    {
                        if (iter->second == true)
                        {
                            CheckDlgButton(hwnd, HWND_GESTURE_CHECK_BOX, BST_CHECKED);
                        }
                        else
                        {
                            CheckDlgButton(hwnd, HWND_GESTURE_CHECK_BOX, BST_UNCHECKED);
                        }
                        break;
                    }
                }
                //selected gesture is changed
            }
        }
        break;

        case HWND_REGION_COMBO_BOX:
        {
            if (HIWORD(wParam) == CBN_SELCHANGE) 
            {  
                HWND hwndCheck = GetDlgItem(hwnd, HWND_REGION_COMBO_BOX);
                LRESULT sel = SendMessage(hwndCheck, CB_GETCURSEL, 0, 0);
                GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
                thisGUI->fillRegionWindow(hwnd, sel, false);
                //selected region is changed
            }
        }
        break;

        case HWND_BUTTON_APPLY:
        {
            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
#if (defined USE_DLIB || defined USE_REALSENSE)
            LRESULT sel = SendMessage(GetDlgItem(hwnd, HWND_REGION_COMBO_BOX), CB_GETCURSEL, 0, 0);
            thisGUI->changeByRegionWindow(hwnd, sel);
#endif
            LRESULT res = SendMessage(GetDlgItem(hwnd, HWND_AUTO_EXPOSURE), BM_GETCHECK, 0, 0);
            Settings settings;
            thisGUI->getPipelineSettings(thisGUI->pipeline, settings);
            settings.autoExposure = (res == BST_CHECKED) ? true : false;
            thisGUI->setPipelineSettings(thisGUI->pipeline, settings);
        }
        break;

        case HWND_BUTTON_DEFAULT:
        {
            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
            thisGUI->replaceSettingsWithDefaults();
            thisGUI->fillSettingWindow(hwnd);
        }
        break;

        case HWND_BUTTON_SAVE:
        {
            TCHAR szFilePathName[_MAX_PATH] = "";
            OPENFILENAME ofn = {0};
            ofn.lStructSize = sizeof(OPENFILENAME);
            ofn.hwndOwner = hwnd;
            ofn.lpstrFile = szFilePathName;
            ofn.nMaxFile = _MAX_PATH;
            ofn.lpstrTitle = "Save File";
            ofn.Flags = OFN_OVERWRITEPROMPT;
            GetSaveFileName(&ofn);
           
            if (szFilePathName[0] != L'\0')
            {
                GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
                try
                {
                    thisGUI->saveSettings(szFilePathName);
                }
                catch (IOException)
                {
                    MessageBox(NULL, "Unable to save settings. See the log for more details", "Error", MB_OK | MB_ICONEXCLAMATION);
                }
            }
        }
        break;

        case HWND_BUTTON_LOAD:
        {
            TCHAR szFilePathName[_MAX_PATH] = "";
            OPENFILENAME ofn = {0};
            ofn.lStructSize = sizeof(OPENFILENAME);
            ofn.hwndOwner = hwnd;
            ofn.lpstrFile = szFilePathName;
            ofn.nMaxFile = _MAX_PATH;
            ofn.lpstrTitle = "Open File";
            ofn.Flags = OFN_FILEMUSTEXIST;
            GetOpenFileName(&ofn);

            if (szFilePathName[0] != L'\0')
            {
                GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
                try 
                {
                    thisGUI->loadSettings(szFilePathName);
                    thisGUI->fillSettingWindow(hwnd);
                }
                catch (IOException)
                {
                    MessageBox(NULL, "Unable to read settings. See the log for more details", "Error", MB_OK | MB_ICONEXCLAMATION);
                }
            }
        }
        break;

        case HWND_BUTTON_RESET:
        {
#if defined USE_DLIB || defined USE_REALSENSE
            GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
            thisGUI->resetTracking();
#endif
        }
        break;
    }
}

void GUIFramework::fillSettingWindow(HWND hwnd)
{
    GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
       
    HWND hwndSlider = GetDlgItem(hwnd, HWND_SENSITIBITY_CONTROL);
    SendMessage(hwndSlider, TBM_SETPOS, TRUE, thisGUI->getCurrentSensitivity());
    SetWindowText(GetDlgItem(hwnd, HWND_SENSITIBITY_POSITION), (LPCSTR)(std::to_string(thisGUI->getCurrentSensitivity()).c_str()));

    SendMessage(GetDlgItem(hwnd, HWND_GESTURE_COMBOBOX), CB_RESETCONTENT, 0, 0);
    std::map<std::string, bool> gest = thisGUI->getGesturesSettings();
    if (!gest.empty())
    {
        for (auto iter = gest.begin(); iter != gest.end(); ++iter)
        {
            SendMessage(GetDlgItem(hwnd, HWND_GESTURE_COMBOBOX), CB_ADDSTRING, 0, (LPARAM)(iter->first).c_str());
        }
        if (gest.begin()->second == true)
        {
            CheckDlgButton(hwnd, HWND_GESTURE_CHECK_BOX, BST_CHECKED);
        }
        else
        {
            CheckDlgButton(hwnd, HWND_GESTURE_CHECK_BOX, BST_UNCHECKED);
        }
        SendMessage(GetDlgItem(hwnd, HWND_GESTURE_COMBOBOX), CB_SETCURSEL, 0, 0);
    }

    thisGUI->fillRegionWindow(hwnd, 0, true);

}


void GUIFramework::fillRegionWindow(HWND hwnd, LRESULT pos, bool check)
{
    GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
    Settings settings;
    HWND hwndCombo = GetDlgItem(hwnd, HWND_REGION_COMBO_BOX);
    if (check)
    {
        SendMessage(hwndCombo, CB_RESETCONTENT, 0, 0);
    }
    thisGUI->getPipelineSettings(thisGUI->pipeline, settings);
    int tmp = 0;
    int tmpPos = 0;
    for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
    {
        if (check)
        {
            SendMessage(hwndCombo, CB_ADDSTRING, 0, (LPARAM)(iter->first).c_str());
            
            if (thisGUI->currentRegion == iter->first)
            {
                tmpPos = tmp;
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETX), (LPCSTR)(std::to_string(iter->second.regionOffsetX).c_str()));
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETY), (LPCSTR)(std::to_string(iter->second.regionOffsetY).c_str()));
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEH), (LPCSTR)(std::to_string(iter->second.regionExpansionHScale).c_str()));
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEW), (LPCSTR)(std::to_string(iter->second.regionExpansionWScale).c_str()));
            }
        }
        else 
        {
            if (tmp == pos)
            {
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETX), (LPCSTR)(std::to_string(iter->second.regionOffsetX).c_str()));
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETY), (LPCSTR)(std::to_string(iter->second.regionOffsetY).c_str()));
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEH), (LPCSTR)(std::to_string(iter->second.regionExpansionHScale).c_str()));
                SetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEW), (LPCSTR)(std::to_string(iter->second.regionExpansionWScale).c_str()));
                
                thisGUI->currentRegion = iter->first;
            }
        }
    }

    SetWindowText(GetDlgItem(hwnd, HWND_REGION_STABFRAME), (LPCSTR)(std::to_string(settings.stabFrames).c_str()));
    SetWindowText(GetDlgItem(hwnd, HWND_REGION_STABTHRESHOLD), (LPCSTR)(std::to_string(settings.stabThreshold).c_str()));
    if (pos >= tmp)
    {
        thisGUI->currentRegion = "";
    }
    if (check)
    {
        SendMessage(hwndCombo, CB_ADDSTRING, 0, (LPARAM)(""));
        SendMessage(hwndCombo, CB_SETCURSEL, tmpPos, 0);
    }
    else
    {
        SendMessage(hwndCombo, CB_SETCURSEL, pos, 0);
    }
    if (thisGUI->currentRegion == "")
    {
        SendMessage(hwndCombo, CB_SETCURSEL, tmp, 0);
        
        SetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETX), (LPCSTR)(""));
        SetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETY), (LPCSTR)(""));
        SetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEH), (LPCSTR)(""));
        SetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEW), (LPCSTR)(""));
    }
}

#if (defined USE_DLIB || defined USE_REALSENSE)
void GUIFramework::changeByRegionWindow(HWND hwnd, LRESULT pos)
{
    GUIFramework* thisGUI = GUIFramework::getMapping(hwnd);
    Settings settings;
    thisGUI->getPipelineSettings(thisGUI->pipeline, settings);
    TCHAR tmpString[256];
    if ((unsigned int)pos < settings.regions.size())
    {
        int tmp = 0;
        for (auto iter = settings.regions.begin(); iter != settings.regions.end(); ++iter, tmp++)
        {
            if (tmp == pos)
            {
                GetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETX), tmpString, 256);
                iter->second.regionOffsetX = atoi(tmpString);
                GetWindowText(GetDlgItem(hwnd, HWND_REGION_OFFSETY), tmpString, 256);
                iter->second.regionOffsetY = atoi(tmpString);
                GetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEH), tmpString, 256);
                iter->second.regionExpansionHScale = (float)atof(tmpString);
                GetWindowText(GetDlgItem(hwnd, HWND_REGION_SCALEW), tmpString, 256);
                iter->second.regionExpansionWScale = (float)atof(tmpString);

                break;
            }
        }
    }
    GetWindowText(GetDlgItem(hwnd, HWND_REGION_STABFRAME), tmpString, 256);
    settings.stabFrames = (unsigned)atoi(tmpString);
    GetWindowText(GetDlgItem(hwnd, HWND_REGION_STABTHRESHOLD), tmpString, 256);
    settings.stabThreshold = (float)atof(tmpString);

    thisGUI->setPipelineSettings(thisGUI->pipeline, settings);
}
#endif //  USE_DLIB || USE_REALSENSE

template<class V>
struct value_finder_t
{
    V v_;
    value_finder_t(V v) : v_(v) {}
    template<class Pair> bool operator()(const Pair& p) const 
    { 
        return p.second == v_; 
    }
};
template<class V>
value_finder_t<V> value_finder(V v) 
{ 
    return value_finder_t<V>(v); 
}

void GUIFramework::setMapping(HWND hwnd, GUIFramework* pG)
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    GUIFramework::fromSettingsHandlerToGUI[hwnd] = pG;
}

GUIFramework* GUIFramework::getMapping(HWND hwnd)
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator it;
    it = GUIFramework::fromSettingsHandlerToGUI.find(hwnd);
    if (it == GUIFramework::fromSettingsHandlerToGUI.end())
        return NULL;
    return it->second;  
}

HWND GUIFramework::findSettingsHandler(GUIFramework* pGui)
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter;
    iter = find_if(fromSettingsHandlerToGUI.begin(), fromSettingsHandlerToGUI.end(), value_finder(pGui));
    if (iter != fromSettingsHandlerToGUI.end())
    {
        return iter->first;
    }
    return NULL;
}

void GUIFramework::removeMapping(HWND hwnd)
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter; 
    iter = GUIFramework::fromSettingsHandlerToGUI.find(hwnd);
    if (iter != fromSettingsHandlerToGUI.end())
    {
        GUIFramework::fromSettingsHandlerToGUI.erase(iter);
    }
}

void GUIFramework::setMappingGUI(HWND hwnd, GUIFramework* pG)
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    GUIFramework::fromHandlerToGUI[hwnd] = pG;
}

GUIFramework* GUIFramework::getMappingGUI(HWND hwnd)
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator it;
    it = GUIFramework::fromHandlerToGUI.find(hwnd);
    if (it == GUIFramework::fromHandlerToGUI.end())
        return NULL;
    return it->second;  
}

void GUIFramework::removeMappingGUI(GUIFramework* pG)
{
	//cout << "In GUIFramework::removeMappingGUI\n" << endl;
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter;
    iter = find_if(fromHandlerToGUI.begin(), fromHandlerToGUI.end(), value_finder(pG));
    while (iter != fromHandlerToGUI.end())
    {
        GUIFramework::fromHandlerToGUI.erase(iter);
        iter = find_if(fromHandlerToGUI.begin(), fromHandlerToGUI.end(), value_finder(pG));
    } 
}

void GUIFramework::destroySettingsWindow()
{
    lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter;
    iter = find_if(fromSettingsHandlerToGUI.begin(), fromSettingsHandlerToGUI.end(), value_finder(this));
    if (iter != fromSettingsHandlerToGUI.end())
    {
        if (iter->second == this)
        {
            PostMessage(iter->first, WM_CLOSE, NULL, NULL);
        }
    }
}

int GUIFramework::createSocket()
{
    if (guiListenSocket != INVALID_SOCKET)
    {
        closesocket(guiListenSocket);
    }

    WSADATA wsaData;
    int iResult = 0;
    guiListenSocket = INVALID_SOCKET;
    
    iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != NO_ERROR)
    {
        wprintf(L"WSAStartup() failed with error: %d\n", iResult);
        return 1;
    }

    guiListenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (guiListenSocket == INVALID_SOCKET) 
    {
        wprintf(L"socket function failed with error: %ld\n", WSAGetLastError());
        //WSACleanup();
        return 1;
    }

    sockaddr_in service;
    service.sin_family = AF_INET;
    service.sin_addr.s_addr = htonl(INADDR_ANY);
    service.sin_port = htons((u_short)port);
   
    iResult = ::bind(guiListenSocket, (LPSOCKADDR)&service, sizeof(service));

    if (iResult == SOCKET_ERROR) 
    {
        wprintf(L"connect function failed with error %d\n", WSAGetLastError());
        iResult = closesocket(guiListenSocket);
        if (iResult == SOCKET_ERROR)
        {
            wprintf(L"closesocket function failed with error %d\n", WSAGetLastError());
        }
        //WSACleanup();
        return 1;
    }

    return 0;
}

#ifdef ASTER_SUPPORT
void GUIFramework::receiveMessageThread(SOCKET socket, GUIFramework* gui)
#else
void GUIFramework::receiveMessageThread(SOCKET socket)
#endif
{
    int iResult = 0;
    char buffer[256] = {0};
    string message = "";
    bool cmdReceived = false;
    do {
        iResult = recv(socket, buffer, 256, 0);
        message += buffer;

        if (message.find("action=SHOWGUI") == 0)
        {
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
            std::map<HWND, GUIFramework*>::iterator iter;
            for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
            {
                iter->second->isShown = true;
            }
            cmdReceived = false;
        }
        else if (message.find("action=HIDEGUI") == 0)
        {
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
            std::map<HWND, GUIFramework*>::iterator iter;
            for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
            {
                iter->second->isShown = false;
            }
            cmdReceived = false;
        }
#ifdef ASTER_SUPPORT
		else if (message.find("action=EXITAPP") == 0)
        {
            gui->setWorkerThreadStatusToExiting();
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
            {
                lock_guard<recursive_mutex> lk(GUIFramework::locker);
                //cout << "message action=EXITAPP\n" << endl;
                removeMappingGUI(gui);
            }
#endif
            gui->session->getVideoPlayer()->stop();
            gui->closeSocket();

            {
                unique_lock<recursive_mutex> lk(GUIFramework::locker);
                if (gui->worker != 0)
                {
                    gui->cvTimeToExit.wait_for(lk, std::chrono::milliseconds(2000), [&] { return gui->workerStatus == THREAD_FINALIZED; });
                }
                if (gui->captor != 0)
                {
                    gui->captorStatus = THREAD_EXITING;
                    gui->cvCaptorReady.notify_one();
                    gui->cvCaptorTimeToExit.wait_for(lk, std::chrono::milliseconds(2000), [&] { return gui->captorStatus == THREAD_FINALIZED; });
                }

                if (GUIFramework::countGUIFramework > 0)
                {
                    UnhookWindowsHookEx(gui->hhook);
                    GUIFramework::countGUIFramework--;
                }
            }
			
			cmdReceived = true;
        }
		message.clear();
		message = "";

#endif //ASTER_SUPPORT
    } while( (iResult > 0) && (message.size() < 256) && (!cmdReceived));
#ifdef ASTER_SUPPORT
	if (!iResult)
	{
		IVCP_LOG_INFO("TCP connection closed");
	}
	else 
#endif
    if (!cmdReceived)
    {
        IVCP_LOG_WARNING("GUIFramework received unknown message: %s, ignoring", message.c_str());
    }
#ifdef ASTER_SUPPORT
	gui->acceptSocket = NULL;
#endif
    shutdown(socket, SD_BOTH);
    closesocket(socket);
	//cout << "Exiting ReceiveMessageThread\n" << endl;
}

void GUIFramework::listenMessageThread(GUIFramework* gui)
{
    for(; !gui->stopSocketListener.load(); )
    {
        if (gui->guiListenSocket == INVALID_SOCKET)
        {
            break;
        }

        int iResult = listen(gui->guiListenSocket, SOMAXCONN);

        if (iResult == SOCKET_ERROR)
        {
            int socketError = WSAGetLastError();
            if (gui->stopSocketListener.load() || socketError == WSAENOTSOCK || socketError == WSAEINVAL || socketError == WSAESHUTDOWN)
            {
                break;
            }

            Sleep(100);
            continue;
        }

        SOCKET acceptSocket = accept(gui->guiListenSocket, NULL, NULL);

        if (acceptSocket != INVALID_SOCKET)
        {
#ifdef ASTER_SUPPORT
            gui->acceptSocket = acceptSocket;
            std::thread sockT(GUIFramework::receiveMessageThread, acceptSocket, gui);
#else
            std::thread sockT(GUIFramework::receiveMessageThread, acceptSocket);
#endif
            sockT.detach();
        }
        else
        {
            int socketError = WSAGetLastError();
            if (gui->stopSocketListener.load() || socketError == WSAENOTSOCK || socketError == WSAEINVAL || socketError == WSAESHUTDOWN)
            {
                break;
            }
        }

        Sleep(100);
    }
}

int GUIFramework::closeSocket()
{
    stopSocketListener = true;

    if (guiListenSocket != INVALID_SOCKET)
    {
        shutdown(guiListenSocket, SD_BOTH);
        closesocket(guiListenSocket);
        guiListenSocket = INVALID_SOCKET;
    }

#ifdef ASTER_SUPPORT
    if (acceptSocket != NULL && acceptSocket != INVALID_SOCKET)
    {
        shutdown(acceptSocket, SD_BOTH);
        closesocket(acceptSocket);
        acceptSocket = NULL;
    }
#endif

    if (socketListenerThread.joinable() && socketListenerThread.get_id() != std::this_thread::get_id())
    {
        socketListenerThread.join();
    }

    //WSACleanup();
    return 0;
}

void GUIFramework::listenSocket(int _port)
{
    port = _port;
    stopSocketListener = false;
    if (!createSocket())
    {
        if (socketListenerThread.joinable())
        {
            socketListenerThread.join();
        }

        socketListenerThread = std::thread(GUIFramework::listenMessageThread, this);
    }
    else
    {
        IVCP_EXCEPTION(IOException, "Unable to create a socket");
    }
}
#ifdef ASTER_SUPPORT
bool GUIFramework::sendGesture(const char *buffer)
{
	bool retVal = true;

	if (acceptSocket != NULL)
	{
		int len = strlen(buffer);
		//cout << "sendgesture buffer length " << len << " " << buffer << "\n";
		char *buf = (char *) malloc(len);
		memcpy(buf, buffer, len);
		if (send(acceptSocket , buf, len, 0) != len)
		{
			IVCP_EXCEPTION(IOException, "Unable to send gesture data socket");
		}
	}
	return retVal;
}
#endif

void GUIFramework::sendCommandToGUI(const char *buffer, double value)
{
	string command = buffer;
	bool cmdReceived = false;
	if (command.find("action=SHOWGUI") == 0)
    {
		showVideo();
        cmdReceived = false;
    }
    else if (command.find("action=HIDEGUI") == 0)
    {
		hideVideo();
        cmdReceived = false;
    }
	else if (command.find("action=EXITAPP") == 0)
    {
		//SS debug
		setWorkerThreadStatusToExiting();

        lock_guard<recursive_mutex> lk(GUIFramework::locker);
#if defined WIN32 || defined _WIN32 || defined WINCE || defined MINGW32
		//cout << "sendCommandToGUI - removeMappingGUI " << GetCurrentThreadId() << endl;
        {
            lock_guard<recursive_mutex> lk(GUIFramework::locker);
            removeMappingGUI(this);
        }
#endif
        this->session->getVideoPlayer()->stop();
        this->closeSocket();

        {
            unique_lock<recursive_mutex> lk(GUIFramework::locker);
            if (worker != 0)
            {
                cvTimeToExit.wait_for(lk, std::chrono::milliseconds(2000), [&] { return workerStatus == THREAD_FINALIZED; });
            }
            if (captor != 0)
            {
                captorStatus = THREAD_EXITING;
                cvCaptorReady.notify_one();
                cvCaptorTimeToExit.wait_for(lk, std::chrono::milliseconds(2000), [&] { return captorStatus == THREAD_FINALIZED; });
            }

            if (GUIFramework::countGUIFramework > 0)
            {
                UnhookWindowsHookEx(hhook);
                GUIFramework::countGUIFramework--;
            }
        }
		cmdReceived = true;
		
    }
	else if (command.find("action=RECALIBRATE") == 0)
	{
		resetTracking();
		cmdReceived = false;
	}
	else if (command.find("action=SET_CT_MIN_GESTURE_LEN") == 0)
	{
		this->session->getClassifier(0)->setClassifierParameters("CT_MIN_GESTURE_LEN", value);
		cmdReceived = false;
	}
	else if (command.find("action=HEAD_MOVEMENT_SENSITIVITY") == 0)
	{
		this->session->getClassifier(0)->setClassifierParameters("HEAD_MOVEMENT_SENSITIVITY", value);
		cmdReceived = false;
	}
	else if (command.find("action=CHEEK_MOVEMENT_SENSITIVITY") == 0)
	{
		this->session->getClassifier(0)->setClassifierParameters("CHEEK_MOVEMENT_SENSITIVITY", value);
		cmdReceived = false;
	}
	else if (command.find("action=CHEEK_HOLD_MSEC") == 0)
	{
		this->session->getClassifier(0)->setClassifierParameters("CHEEK_HOLD_MSEC", value);
		cmdReceived = false;
	}
	else if (command.find("action=EYEBROW_MOVEMENT_SENSITIVITY") == 0)
	{
		this->session->getClassifier(0)->setClassifierParameters("EYEBROW_MOVEMENT_SENSITIVITY", value);
		cmdReceived = false;
	}
	else if (command.find("action=EYEBROW_HOLD_MSEC") == 0)
	{
		this->session->getClassifier(0)->setClassifierParameters("EYEBROW_HOLD_MSEC", value);
		cmdReceived = false;
	}
	/*
	else 
	{
		this->session->getClassifier(0)->setClassifierParameters((char *)buffer, value);
		cmdReceived = false;
	}
	*/
	
	command.clear();
	command = "";
}

void GUIFramework::showVideo()
{
	lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter;
    for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
    {
        iter->second->isShown = true;
    }
}

void GUIFramework::hideVideo()
{
	lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter;
    for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
    {
        iter->second->isShown = false;
    }
}


bool GUIFramework::isVideoVisible()
{
	lock_guard<recursive_mutex> lk(GUIFramework::locker);
    std::map<HWND, GUIFramework*>::iterator iter;
    for (iter = GUIFramework::fromHandlerToGUI.begin(); iter != GUIFramework::fromHandlerToGUI.end(); ++iter)
    {
		if (iter->second->isShown)
		{
			return true;
		}
    }

	return false;
}

#if (defined USE_DLIB || defined USE_REALSENSE)
void GUIFramework::onMouse( int event, int x, int y, int, void* gui )
{
    if( event != EVENT_LBUTTONDOWN )
        return;

    GUIFramework* thisGUI = (GUIFramework*) gui;
    HWND hwnd = thisGUI->findSettingsHandler(thisGUI);
    std::map<std::string, cv::Rect> fitRegions;
    for (auto iter = thisGUI->allRegions.begin(); iter != thisGUI->allRegions.end(); ++iter)
    {
        if ((x >= iter->second.x) && (x <= iter->second.x + iter->second.width) && (y >= iter->second.y) && (y <= iter->second.y + iter->second.height))
        {
            fitRegions[iter->first] = iter->second;
        }
    }
    if (fitRegions.empty())
    {
        thisGUI->currentRegion = "";
        if (!thisGUI->allRegions.empty())
        {
            thisGUI->fillRegionWindow(hwnd, thisGUI->allRegions.size() + 1, false);
        }
        return;
    }
    if (fitRegions.size() > 1)
    {
        for (auto iterFirst = fitRegions.begin(); iterFirst != fitRegions.end(); iterFirst++)
        {
            for (auto iterSecond = fitRegions.begin(); iterSecond != fitRegions.end(); iterSecond++)
            {
                if (iterFirst != iterSecond)
                {
                    if ((iterFirst->second.x >= iterSecond->second.x) && (iterFirst->second.x + iterFirst->second.width <= iterSecond->second.x + iterSecond->second.width)
                        && (iterFirst->second.y >= iterSecond->second.y) && (iterFirst->second.y + iterFirst->second.height <= iterSecond->second.y + iterSecond->second.height))
                    {
                        fitRegions.erase(iterSecond);
                        break;
                    }
                }
            }
        }
    }
    if (fitRegions.size() > 1) 
    {
        for (auto iterFirst = fitRegions.begin(); iterFirst != fitRegions.end(); iterFirst++)
        {
            for (auto iterSecond = fitRegions.begin(); iterSecond != fitRegions.end(); iterSecond++)
            {
                if (iterFirst != iterSecond)
                {
                    double centerFirstX = (iterFirst->second.x + iterFirst->second.width) / 2.0;
                    double centerFirstY = (iterFirst->second.y + iterFirst->second.height) / 2.0;
                    double centerSecondX = (iterSecond->second.x + iterSecond->second.width) / 2.0;
                    double centerSecondY = (iterSecond->second.y + iterSecond->second.height) / 2.0;
                    double firstLen = (x - centerFirstX) * (x - centerFirstX) + (y - centerFirstY) * (y - centerFirstY);
                    double secondLen = (x - centerSecondX) * (x - centerSecondX) + (y - centerSecondY) * (y - centerSecondY);
                    if (firstLen > secondLen)
                    {
                        fitRegions.erase(iterSecond);
                        break;
                    }
                }
            }
        }
    }
    if (fitRegions.size() == 1)
    {
        thisGUI->currentRegion = fitRegions.begin()->first;
        Settings settings;
        thisGUI->getPipelineSettings(thisGUI->pipeline, settings);
        int tmp = 0;
        for (auto iterS = settings.regions.begin(); iterS != settings.regions.end(); ++iterS, tmp++)
        {
            if (iterS->first == thisGUI->currentRegion)
            {
                break;
            }
        }
        SendMessage(GetDlgItem(hwnd, HWND_REGION_COMBO_BOX), CB_SETCURSEL, tmp, 0);
        thisGUI->fillRegionWindow(hwnd, tmp, false);
    }
}
void GUIFramework::resetTracking()
{
	try 
	{ 
    
    #ifdef USE_DLIB
    if (Factory::isDlibHandlerCreated())
    {
        for (int i = 0; i < session->getNumberOfRegionCreators(); i++)
        {
            SPtr<RegionCreator> nregion = session->getRegionCreator(i);
            DlibRegionCreator* region = dynamic_cast<DlibRegionCreator*>((RegionCreator*)nregion);
            if(region)
            {
                SPtr<IDlib> dlibHandler = Factory::getCachedDlib();
                dlibHandler->resetTracking();

                Settings settings;
                getPipelineSettings(pipeline, settings);
                settings.regions = defaultSettings.regions;
                setPipelineSettings(pipeline, settings);

                if (allRegions.size() != 0)
                {
                    intChangedRegion = (int)allRegions.size() + 1;
                    allRegions.clear();
                }

                currentRegion = "";
                if (isSettingsShow)
                {
                    HWND hwnd = findSettingsHandler(this);
                    fillRegionWindow(hwnd, settings.regions.size(), false);
                }

                break;
            }
        }
    }
    #endif  //USE_DLIB
    #ifdef USE_REALSENSE
    if (Factory::isRealSenseHandlerCreated())
    {
        for (int i = 0; i < session->getNumberOfRegionCreators(); i++)
        {
            SPtr<RegionCreator> nregion = session->getRegionCreator(i);
            RealSenseRegionCreator* region = dynamic_cast<RealSenseRegionCreator*>((RegionCreator*)nregion);
            if(region)
            {
                SPtr<IRealSense> rsHandler = Factory::getCachedRealSense();
                rsHandler->resetTracking();

                Settings settings;
                getPipelineSettings(pipeline, settings);
                settings.regions = defaultSettings.regions;
                setPipelineSettings(pipeline, settings);

                if (allRegions.size() != 0)
                {
                    intChangedRegion = (int)allRegions.size() + 1;
                    allRegions.clear();
                }

                currentRegion = "";
                if (isSettingsShow)
                {
                    HWND hwnd = findSettingsHandler(this);
                    fillRegionWindow(hwnd, settings.regions.size(), false);
                }

                break;
            }
        }
    }
    #endif  //USE_REALSENSE
	} //try
     catch (Exception &)
	{
		IVCP_EXCEPTION(IncorrectParamException, "reset tracking failed");
	}
}
#endif // USE_DLIB || USE_REALSENSE

#endif //WIN32 || defined _WIN32 || defined WINCE || defined MINGW32

};

