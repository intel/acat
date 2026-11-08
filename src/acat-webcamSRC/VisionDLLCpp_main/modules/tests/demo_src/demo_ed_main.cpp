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

int main(int argc, char **argv)
{

	isStdFeaActive = true;

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

	setupClassifier();
	
	setupSinglePipeline();

	setupVideoRecording();

    pipeline_single->run();

	stopVideoRecording();

	if (c_FeaturesOut == true)
	{
		outputEDFeafile.close();
		outputEDResfile.close();
	}
    return 0;
}
//============================================================================