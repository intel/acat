///////////////////////////////////////////////////////////////////////////
// <copyright file="acat_gestures_main.cpp" company="Intel Corporation">
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
#include "ivcp.hpp"

#include <windows.h>

using namespace cv;
using namespace std;
using namespace ivcp;

#include "acat_gestures_global.hpp"
#include "acat_gestures_common.hpp"

//============================================================================
int main(int argc, char **argv)
{

	isNormBlurFeaActive = true;
	bRunMultiGestureSMClassifier = true;

	processCommandLineArgs(argc, argv);

	Log::setVerbosityLevel(LOG_ERROR);
	
	// Initialize pipeline
	ivcpInit();
	
	session = Factory::createSession("session");
	
	if (setupVideoSource()== false)
		exit(0);

	setupFaceRegionTracker();

	setupFeatureComputation_NormBlur();

	setupClassifier_MG();
	
	setupSinglePipeline();

	setupVideoRecording();
	pipeline_single->run();
	stopVideoRecording();

	if (c_FeaturesOut == true)
	{
		outputNormBlurFeafile.close();
	}
	
    return 0;
}
//============================================================================