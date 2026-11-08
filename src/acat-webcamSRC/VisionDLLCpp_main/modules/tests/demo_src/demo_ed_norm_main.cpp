// Copyright (C) 2013, Intel Corporation, all rights reserved.

#include <time.h>
#include <iostream>
#include <fstream>
#include "ivcp.hpp"

#include <windows.h>

using namespace cv;
using namespace std;
using namespace ivcp;

#include "demo_ed_global.hpp"
#include "demo_ed_common.hpp"

//============================================================================
//============================================================================
int main(int argc, char **argv)
{

	isStdFeaActive = true;
	isNormBlurFeaActive = true;
	processCommandLineArgs(argc, argv);

	
	// Initialize pipeline
	printf("Initializing pipeline\n");
	ivcpInit();
	printf("Initialized pipeline\n");
	//----------------------------------

	session = Factory::createSession("session");
	
	setupVideoSource();

	setupFaceRegionTracker();

	setupFeatureComputation_ED();
	setupFeatureComputation_NormBlur();

	if (c_bRunEDClassifier)
		setupClassifier();
	else if (c_bRunEDClassifier_NormBlur)
		setupClassifier_ED_NormBlur();
	
	//setupGUIFramework();
	
	setupParallelPipeline();

	setupVideoRecording();

	Log::setVerbosityLevel(LOG_ERROR);

	pipeline_parallel->run();
	stopVideoRecording();

	if (c_FeaturesOut == true)
	{
		outputEDFeafile.close();
		outputNormBlurFeafile.close();
		outputEDResfile.close();
	}
	
    return 0;
}
//============================================================================