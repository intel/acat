///////////////////////////////////////////////////////////////////////////
// <copyright file="demo_main.cpp" company="Intel Corporation">
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

#include "ivcp.hpp"

using namespace cv;
using namespace std;
using namespace ivcp;

float stabThreshold = 30.0F;
unsigned int stabFrames = 10U;
int normFrames = 10;
int noFaceFrames = 30;
bool noFaceClockwiseRotation = false;
bool glassesFixEnabled = false;
bool useTemplateRegions = true;
float glassesFixStrength = 0.0F;
bool useAutoGlassesSearch = true;
bool cutBackground = false;
double resizeDownscale = 0.5; // 0.25;

int main(int argc, char *argv[])
{
    ivcpInit();

    SPtr<Session> session = Factory::createSession("session");

    SPtr<VideoPlayer> videoPlayer = Factory::createVideoPlayer();
    SPtr<VideoPlaylist> videoPlaylist = Factory::createVideoPlaylist();
    SPtr<IDlib> dlibHandler;

    if (argc != 2)
    {
		//SPtr<CameraVideoSource> camera = Factory::createCameraVideoSource(1);
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
			for (int i = 0; i<allDevices.size(); i++)
			{
				printf("\t%d : %s\n", allDevices[i].first, allDevices[i].second.c_str());
			}
		}
		printf("-----------------------------------\n");
		if (allDevices.size() > 1)
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
		cout << "Initializing camera: " << selectedCameraIdx << " " << allDevices[selectedCameraIdx].second << endl << endl;
		SPtr<CameraVideoSource> camera = Factory::createCameraVideoSource(selectedCameraIdx);

		camera->openCamera();
		FrameRange videorange;
        videoPlaylist->addSourceRange(camera, videorange);
        //cout << "Can not be used with camera. Please, run demo with path to file\n";
        //return 1;

    }
    else
    {
        if (strcmp(argv[1], "--help") == 0)
        {
            cout << "Usage: ivcp_demo_dlib [--help] <file-path>\n";
            cout << "To use demo with camera - run it without arguments\n";
            cout << "To use demo with file - run it with path to file\n";
            return 0;
        }
        string fileName = string(argv[1]);
        SPtr<VideofileVideoSource> videoFile = Factory::createVideofileVideoSource(fileName);
        videoPlaylist->addSourceRange(videoFile, FrameRange(0, videoFile->getFrameCount()));
    }

    dlibHandler = Factory::createDlib(stabThreshold, stabFrames, normFrames, noFaceFrames, noFaceClockwiseRotation, glassesFixEnabled, useTemplateRegions, 
                                      glassesFixStrength, useAutoGlassesSearch, cutBackground, resizeDownscale);

    session->setVideoPlaylist(videoPlaylist);

    bool normalize = true;
    SPtr<DlibRegionCreator> regionFace = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_FACE, 1.0, 1.0, normalize));
        regionFace->setId("regionFace");
        session->addRegionCreator(regionFace);
    SPtr<DlibRegionCreator> regionLEyebrow = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_LEYEBROW, 1.0, 1.0, normalize));
        regionLEyebrow->setId("regionLEyebrow");
        session->addRegionCreator(regionLEyebrow);
    SPtr<DlibRegionCreator> regionREyebrow = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_REYEBROW, 1.0, 1.0, normalize));
        regionREyebrow->setId("regionREyebrow");
        session->addRegionCreator(regionREyebrow);
    SPtr<DlibRegionCreator> regionMouth = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_MOUTH, 1.0, 1.0, normalize));
        regionMouth->setId("regionMouth");
        session->addRegionCreator(regionMouth);
    SPtr<DlibRegionCreator> regionNose = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_NOSE, 1.0, 1.0, normalize));
        regionNose->setId("regionNose");
        session->addRegionCreator(regionNose);
    SPtr<DlibRegionCreator> regionLEye = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_LEYE, 1.0, 1.0, normalize));
        regionLEye->setId("regionLEye");
        session->addRegionCreator(regionLEye);
    SPtr<DlibRegionCreator> regionREye = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_REYE, 1.0, 1.0, normalize));
        regionREye->setId("regionREye");
        session->addRegionCreator(regionREye);
    SPtr<DlibRegionCreator> regionLCheek = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_LCHEEK, 1.0, 1.0, normalize));
        regionLCheek->setId("regionLCheek");
        session->addRegionCreator(regionLCheek);
    SPtr<DlibRegionCreator> regionRCheek = new DlibRegionCreator(dlibHandler, DlibRegionCreator::Settings(REGION_RCHEEK, 1.0, 1.0, normalize));
        regionRCheek->setId("regionRCheek");
        session->addRegionCreator(regionRCheek);

    SPtr<TextPerformance> performance = Factory::createPerformance();
        performance->setId("performance");
        session->addPerformanceFilter(performance);

   SPtr<Pipeline> pipeline = Factory::createPipeline(RUNTIME_PIPELINE);
        pipeline->setSession(session);

    pipeline->addLink(session->getVideoPlayer()->getId(), "regionFace");
    pipeline->addLink("regionFace", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionLEyebrow");
    pipeline->addLink("regionLEyebrow", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionREyebrow");
    pipeline->addLink("regionREyebrow", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionMouth");
    pipeline->addLink("regionMouth", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionNose");
    pipeline->addLink("regionNose", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionLEye");
    pipeline->addLink("regionLEye", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionREye");
    pipeline->addLink("regionREye", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionLCheek");
    pipeline->addLink("regionLCheek", "performance");
    pipeline->addLink(session->getVideoPlayer()->getId(), "regionRCheek");
    pipeline->addLink("regionRCheek", "performance");

    SPtr<IGUIFramework> visualize = Factory::createGUIFramework(pipeline);
    visualize->setNormFrames(normFrames);
    pipeline->run();

    return 0;
}