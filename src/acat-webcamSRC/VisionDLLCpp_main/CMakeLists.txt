CMAKE_MINIMUM_REQUIRED(VERSION 3.16)

project(ivcp VERSION 0.18.0 LANGUAGES CXX)

option(BUILD_SHARED_LIBS "Build shared libraries (DLLs)." ON)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/lib")

if(WIN32)
    SET(INSTALL_PREFIX "C:/ivcp ${VERSION}" CACHE PATH "Install path prefix")
else()
    SET(INSTALL_PREFIX "/usr/local/ivcp_${VERSION}" CACHE PATH "Install path prefix")
    SET(CMAKE_INSTALL_RPATH "${INSTALL_PREFIX}/lib")
endif()
SET(CMAKE_INSTALL_PREFIX "${INSTALL_PREFIX}" CACHE INTERNAL "Install path prefix" FORCE)

if(DEFINED ENV{OPENCV_DIR})
    set(OpenCV_DIR "$ENV{OPENCV_DIR}" CACHE PATH "Path to OpenCVConfig.cmake")
else()
    set(OpenCV_DIR "./3rdparty/opencv" CACHE PATH "Path to OpenCVConfig.cmake")
endif()

find_package(OpenCV REQUIRED CONFIG)

message(STATUS "OpenCV version: ${OpenCV_VERSION}")
message(STATUS "OpenCV include dirs: ${OpenCV_INCLUDE_DIRS}")

# Make OpenCV targets available to subdirectories
set(OpenCV_LIBRARIES
    OpenCV::opencv_core
    OpenCV::opencv_imgproc
    OpenCV::opencv_highgui
    CACHE INTERNAL "List of OpenCV targets to link"
)

SET(DLIB_LOCATION "../VisionExternalInstalls/dlib-20.0" CACHE PATH "Path where Dlib is located")

SET(DLIB_FACE_LANDMARKS_FILE "$ENV{DLIB_FLM_FILE}" CACHE FILEPATH "Path where Dlib's default face landmarking model file is located")

if(NOT EXISTS "${DLIB_FACE_LANDMARKS_FILE}" OR IS_DIRECTORY "${DLIB_FACE_LANDMARKS_FILE}")
    message(FATAL_ERROR "Dlib face landmark model not found: '${DLIB_FACE_LANDMARKS_FILE}'.\n"
        "Set -DDLIB_FACE_LANDMARKS_FILE=<path to shape_predictor_68_face_landmarks.dat>. "
        "A previous value may be cached; setting DLIB_FLM_FILE alone will not replace it.")
endif()

if(EXISTS "${DLIB_LOCATION}")
    add_definitions(-DDLIB_FACE_LANDMARKS_FILE="${DLIB_FACE_LANDMARKS_FILE}")

    set(HAVE_DLIB TRUE)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DUSE_DLIB")
    option(DLIB_LINK_WITH_LIBPNG "" OFF)
    option(DLIB_LINK_WITH_LIBJPEG "" OFF)
    option(DLIB_USE_BLAS "" OFF)
    option(DLIB_USE_LAPACK "" OFF)
    # Tell cmake we will need dlib.  This command will pull in dlib and compile it
    # into your project.  Note that you don't need to compile or install dlib.  All
    # cmake needs is the dlib source code folder and it will take care of everything.
    add_subdirectory(${DLIB_LOCATION}/dlib dlib_build)
    if(MSVC)
        SET_TARGET_PROPERTIES(dlib PROPERTIES COMPILE_FLAGS -W0)
    endif()
else()
    set(HAVE_DLIB FALSE)
    message(FATAL_ERROR "Dlib is not found.\n"
                            "Set path to Dlib to DLIB_LOCATION\n")
endif()

# For MSVC, CMake sets certain flags to defaults regardless of whether static or dynamic libraries are used.
# So it should be reconfigured here in order to use build options related to configuration.
# This replacement code is taken from sample in the CMake Wiki at
# http://www.cmake.org/Wiki/CMake_FAQ#Dynamic_Replace.
if(MSVC)
foreach (flag_var
            CMAKE_CXX_FLAGS CMAKE_CXX_FLAGS_DEBUG CMAKE_CXX_FLAGS_RELEASE
            CMAKE_CXX_FLAGS_MINSIZEREL CMAKE_CXX_FLAGS_RELWITHDEBINFO)
    if (NOT BUILD_SHARED_LIBS AND NOT gtest_force_shared_crt)
        string(REPLACE "/MD" "-MT" ${flag_var} "${${flag_var}}")
    endif()
endforeach()
if(CMAKE_CXX_FLAGS MATCHES "/W[0-4]")
    string(REGEX REPLACE "/W[0-4]" "/W4" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
else()
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
endif()
if (${CMAKE_CXX_COMPILER_VERSION} VERSION_GREATER 17)
    # MSVS 2012 c++ compiler doesn't have support for variadic templates.
    # Default maximum number of parameters is 5. Need to increase it to 10.
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /D_VARIADIC_MAX=10 /MP")
endif()
endif()

option(ACAT_BUILD_DOCUMENTATION "Build the Doxygen documentation" OFF)
option(ACAT_BUILD_MARKUP "Build the markup applications" OFF)

add_subdirectory(modules)
if(ACAT_BUILD_DOCUMENTATION)
    add_subdirectory(Docs)
endif()
if(ACAT_BUILD_MARKUP)
    add_subdirectory(Markup)
endif()

# ----------------------  OLD OPTIONS ----------------------

# option(WITH_DLIB "Include Dlib library" ON)
# if(WIN32)
#     option(ASTER_SUPPORT "Trigger gestures into Aster" ON)
#     #option(WITH_REALSENSE_1 "Include RealSense library" OFF)
# 	MESSAGE("  CMAKE_SYSTEM_VERSION='${CMAKE_SYSTEM_VERSION}'")
#     #if (${CMAKE_SYSTEM_VERSION} EQUAL 6.2)
# 	if (${CMAKE_SYSTEM_VERSION} GREATER 6.1)
#         option(WITH_REALSENSE "Include RealSense library" OFF)
#     endif()
# endif(WIN32)

# if (MSVC_IDE OR CMAKE_GENERATOR MATCHES Xcode)
#     if (CMAKE_VERSION VERSION_GREATER "2.8.0")
#         option(ENABLE_SOLUTION_FOLDERS "Solution folder in Visual Studio or in other IDEs" ON)
#     endif()
# endif()

# SET(VERSION_MAJOR "0")
# SET(VERSION_MINOR "18")
# SET(VERSION "${VERSION_MAJOR}.${VERSION_MINOR}")
# SET(CPACK_PACKAGE_VERSION_MAJOR "0")
# SET(CPACK_PACKAGE_VERSION_MINOR "18")
# SET(CPACK_PACKAGE_VERSION  "${CPACK_PACKAGE_VERSION_MAJOR}.${CPACK_PACKAGE_VERSION_MINOR}")
# SET(COVERAGE 0 CACHE BOOL "")
# SET(MEMCHECK 0 CACHE BOOL "")
# SET(WARN_NOT_DOCUMENTED 0 CACHE BOOL "")

# if(WIN32)
#     SET(INSTALL_PREFIX "C:/ivcp ${VERSION}" CACHE PATH "Install path prefix")
# else()
#     SET(INSTALL_PREFIX "/usr/local/ivcp_${VERSION}" CACHE PATH "Install path prefix")
#     SET(CMAKE_INSTALL_RPATH "${INSTALL_PREFIX}/lib")
# endif()
# SET(CMAKE_INSTALL_PREFIX "${INSTALL_PREFIX}" CACHE INTERNAL "Install path prefix" FORCE)

# if(WARN_NOT_DOCUMENTED)
# SET(WARN_IF_UNDOCUMENTED "YES")
# else()
# SET(WARN_IF_UNDOCUMENTED "NO")
# endif()

# if(DEFINED ENV{OPENCV_DIR})
#     find_package(OpenCV PATHS "$ENV{OPENCV_DIR}")
# else()
#     find_package(OpenCV)
# endif()

# if(DEFINED OpenCV_FOUND)
#     SET(OpenCV_INCLUDE_DIRS "${OpenCV_INCLUDE_DIRS}" CACHE PATH "Path where OpenCV include files are located")
#     SET(OpenCV_LIB_DIR "${OpenCV_LIB_DIR}" CACHE PATH "Path where OpenCV libraries are located")
#     SET(OpenCV_LIBS "${OpenCV_LIBS}" CACHE PATH "List of OpenCV libraries to link")
# else()
#     if(DEFINED ENV{OPENCV_DIR})
#         SET(OpenCV_LOCATION "$ENV{OPENCV_DIR}")
#     else()
#         SET(OpenCV_LOCATION "./3rdparty")
#     endif()
#     SET(OpenCV_INCLUDE_DIRS "${OpenCV_LOCATION}/include" CACHE PATH "Path where OpenCV include files are located")
#     include_directories(${OpenCV_INCLUDE_DIRS})
#     SET(OpenCV_LIB_DIR "${OpenCV_LOCATION}/lib" CACHE PATH "Path where OpenCV libraries are located")
#     file(GLOB OpenCV_LIBS "${OpenCV_LIB_DIR}/*.lib")
#     SET(OpenCV_LIBS "${OpenCV_LIBS}" CACHE PATH "List of OpenCV libraries to link")
# endif()

# if(ENABLE_SOLUTION_FOLDERS)
#   set_property(GLOBAL PROPERTY USE_FOLDERS ON)
#   set_property(GLOBAL PROPERTY PREDEFINED_TARGETS_FOLDER "CMakeTargets")
# endif()


# SET(INCLUDE_SOURCE "${INCLUDE_SOURCE}" CACHE BOOL "Include sources in installer")
# SET(CREATE_INSTALLER "${CREATE_INSTALLER}" CACHE BOOL "Create installer")

# if(CMAKE_COMPILER_IS_GNUCXX)
#     SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++0x")
#     if(COVERAGE)
#         SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -g -O0 --coverage")
#         SET(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} --coverage")
#     endif()
#     if (MEMCHECK)
#         SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -g")
#     endif()
#     set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Wextra")
# elseif(MSVC)
#     # For MSVC, CMake sets certain flags to defaults regardless of whether static or dynamic libraries are used.
#     # So it should be reconfigured here in order to use build options related to configuration.
#     # This replacement code is taken from sample in the CMake Wiki at
#     # http://www.cmake.org/Wiki/CMake_FAQ#Dynamic_Replace.
#     foreach (flag_var
#              CMAKE_CXX_FLAGS CMAKE_CXX_FLAGS_DEBUG CMAKE_CXX_FLAGS_RELEASE
#              CMAKE_CXX_FLAGS_MINSIZEREL CMAKE_CXX_FLAGS_RELWITHDEBINFO)
#         if (NOT BUILD_SHARED_LIBS AND NOT gtest_force_shared_crt)
#             string(REPLACE "/MD" "-MT" ${flag_var} "${${flag_var}}")
#         endif()
#     endforeach()
#     if(CMAKE_CXX_FLAGS MATCHES "/W[0-4]")
#         string(REGEX REPLACE "/W[0-4]" "/W4" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
#     else()
#         set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W4")
#     endif()
#     if (${CMAKE_CXX_COMPILER_VERSION} VERSION_GREATER 17)
#         # MSVS 2012 c++ compiler doesn't have support for variadic templates.
#         # Default maximum number of parameters is 5. Need to increase it to 10.
#         set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /D_VARIADIC_MAX=10 /MP")
#     endif()
# endif()

# SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin")
# SET(LIBRARY_OUTPUT_PATH  "${CMAKE_BINARY_DIR}/lib")

# if (ASTER_SUPPORT)
# 	SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DASTER_SUPPORT")
# endif()


# if (WITH_DLIB)
#     SET(DLIB_LOCATION "$ENV{DLIB_PATH}" CACHE PATH "Path where Dlib is located")
#     SET(DLIB_FACE_LANDMARKS_FILE "$ENV{DLIB_FLM_FILE}" CACHE FILEPATH "Path where Dlib's default face landmarking model file is located")
#     if(EXISTS ${DLIB_LOCATION})
#         if(EXISTS ${DLIB_FACE_LANDMARKS_FILE})
#             add_definitions(-DDLIB_FACE_LANDMARKS_FILE="${DLIB_FACE_LANDMARKS_FILE}")
#         else()
#             message(FATAL_ERROR, "Dlib's default face landmarking model file is not found\n"
#                                  "You can get it from the following URL: \n"
#                                  "http://sourceforge.net/projects/dclib/files/dlib/v18.10/shape_predictor_68_face_landmarks.dat.bz2 \n"
#                                  "And set path to this file to DLIB_FACE_LANDMARKS_FILE \n")
#         endif()
#         set(HAVE_DLIB TRUE)
#         SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DUSE_DLIB")
#         option(DLIB_LINK_WITH_LIBPNG "" OFF)
#         option(DLIB_LINK_WITH_LIBJPEG "" OFF)
#         option(DLIB_USE_BLAS "" OFF)
#         option(DLIB_USE_LAPACK "" OFF)
#         include("${DLIB_LOCATION}/dlib/cmake")
#         if(MSVC)
#             SET_TARGET_PROPERTIES(dlib PROPERTIES COMPILE_FLAGS -W0)
#         endif()
#     else()
#         set(HAVE_DLIB FALSE)
#         message(FATAL_ERROR, "Dlib is not found.\n"
#                              "Set path to Dlib to DLIB_LOCATION\n")
#     endif()
# else()
#     set(HAVE_DLIB FALSE)
# endif()

# if (WITH_REALSENSE)
# 	MESSAGE("In WITH_REALSENSE")
#     include("cmake/IVCPFindRealSense.cmake")
#     if (REALSENSE_FOUND)
# 		MESSAGE("In REALSENSE_FOUND")
#         include_directories(${RealSense_INCLUDE_DIR})
#         set(HAVE_REALSENSE TRUE)
#         SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DUSE_REALSENSE")
#     else()
#         set(HAVE_REALSENSE FALSE)
#         message(FATAL_ERROR, "RealSense is not found")
#     endif()
# else()
#     set(HAVE_REALSENSE FALSE)
# endif()

# add_subdirectory(modules)
# add_subdirectory(Docs)
# add_subdirectory(Markup)

# list(GET OpenCV_LIB_DIR 0 HEAD)
# file(GLOB OpenCV_DLLS "${HEAD}/../bin/*.dll")
# install(FILES ${OpenCV_DLLS} DESTINATION bin)
# file(GLOB OpenCV_DLLS "${OpenCV_DIR}/bin/Release/*.dll")
# install(FILES ${OpenCV_DLLS} DESTINATION bin)
# file(GLOB OpenCV_DLLS "${OpenCV_DIR}/bin/Debug/*.dll")
# install(FILES ${OpenCV_DLLS} DESTINATION bin)

# install(FILES "${CMAKE_SOURCE_DIR}/include/ivcp.hpp" DESTINATION include)

# if (${INCLUDE_SOURCE})
#     install(DIRECTORY "${CMAKE_SOURCE_DIR}/cmake" DESTINATION src)
#     install(DIRECTORY "${CMAKE_SOURCE_DIR}/data" DESTINATION src)
#     install(DIRECTORY "${CMAKE_SOURCE_DIR}/Docs" DESTINATION src)
#     install(DIRECTORY "${CMAKE_SOURCE_DIR}/include" DESTINATION src)
#     install(DIRECTORY "${CMAKE_SOURCE_DIR}/Markup" DESTINATION src)
#     install(DIRECTORY "${CMAKE_SOURCE_DIR}/modules" DESTINATION src)
#     install(FILES "${CMAKE_SOURCE_DIR}/CMakeLists.txt" DESTINATION src)
# endif()

# if (${CREATE_INSTALLER})
#     SET(CPACK_GENERATOR "NSIS")
#     SET(CPACK_NSIS_INSTALL_ROOT "C:")

#     SET(CPACK_NSIS_EXTRA_INSTALL_COMMANDS "
#             SetOutPath  \\\"$INSTDIR\\\\bin\\\"
#             CreateDirectory \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Demo\\\"
#             CreateDirectory \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Markup\\\"
#             CreateDirectory \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Tests\\\"
#             CreateDirectory \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Docs\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Demo\\\\Demo.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_demo.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Demo\\\\DemoEmotionClassification.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_demo_emotion_classification.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Demo\\\\RuntimeDemo.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_demo_runtime.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Demo\\\\DemoUcla.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_demo_ucla.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Markup\\\\RegionMarkup.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_region_markup.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Markup\\\\Markup.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_markup.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Tests\\\\PerformanceTest.lnk\\\" \\\"$INSTDIR\\\\bin\\\\ivcp_performance_test.exe\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Docs\\\\Help.lnk\\\" \\\"$INSTDIR\\\\doc\\\\index.html\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Docs\\\\IVCP Architecture.lnk\\\" \\\"$INSTDIR\\\\doc\\\\IVCP_Architecture.pdf\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Docs\\\\IVCP User Guide.lnk\\\" \\\"$INSTDIR\\\\doc\\\\IVCP_User_guide.pdf\\\"
#             CreateShortCut \\\"$SMPROGRAMS\\\\$STARTMENU_FOLDER\\\\Docs\\\\IVCP Usage Samples.lnk\\\" \\\"$INSTDIR\\\\doc\\\\IVCP_Usage_samples.pdf\\\"
#         ")
#     SET(CPACK_NSIS_EXTRA_UNINSTALL_COMMANDS "
#             !insertmacro MUI_STARTMENU_GETFOLDER Application $MUI_TEMP
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Demo\\\\Demo.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Demo\\\\DemoEmotionClassification.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Demo\\\\RuntimeDemo.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Demo\\\\DemoUcla.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Markup\\\\RegionMarkup.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Markup\\\\Markup.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Tests\\\\PerformanceTest.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Docs\\\\Help.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Docs\\\\IVCP Architecture.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Docs\\\\IVCP User Guide.lnk\\\"
#             Delete \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Docs\\\\IVCP Usage Samples.lnk\\\"
#             RMDir  \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Markup\\\"
#             RMDir  \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Tests\\\"
#             RMDir  \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Docs\\\"
#         ")

#     SET(CPACK_NSIS_EXTRA_UNINSTALL_COMMANDS ${CPACK_NSIS_EXTRA_UNINSTALL_COMMANDS} "
#             RMDir  \\\"$SMPROGRAMS\\\\$MUI_TEMP\\\\Demo\\\"
#         ")

#     INCLUDE(CPack)
# endif()

