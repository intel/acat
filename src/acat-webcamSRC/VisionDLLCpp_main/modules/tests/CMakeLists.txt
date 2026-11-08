add_subdirectory(acat_gestures_dll)
add_subdirectory(acat_gestures)

option(ACAT_BUILD_DEMOS "Build native demo applications" OFF)
option(ACAT_BUILD_PERFORMANCE_TEST "Build the native performance test" OFF)

if(ACAT_BUILD_DEMOS)
	add_subdirectory(demo_dlib)
	add_subdirectory(demo_emotion_classification)
	add_subdirectory(demo_lr_classification)
	add_subdirectory(demo_motion_estimation_feature)
	add_subdirectory(demo_runtime_mode)
	add_subdirectory(demo_test_mode)
	add_subdirectory(demo_ucla)
	if(HAVE_REALSENSE)
		add_subdirectory(demo_realsense)
	endif()
endif()

if(ACAT_BUILD_PERFORMANCE_TEST)
	add_subdirectory(performance_test)
endif()

