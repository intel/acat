cmake_minimum_required(VERSION 3.16)

project(acat_webcam LANGUAGES CXX)

# The native CMake files use CMAKE_SOURCE_DIR for their public headers. Make the
# native include directory available when they are built from the repository root.
include_directories("${CMAKE_CURRENT_SOURCE_DIR}/VisionDLLCpp_main/include")

if(DEFINED ENV{OPENCV_DIR})
    set(_acat_opencv_default "$ENV{OPENCV_DIR}")
elseif(WIN32)
    set(_acat_opencv_default "${CMAKE_CURRENT_SOURCE_DIR}/VisionExternalInstalls/opencv/build")
else()
    set(_acat_opencv_default "") # Search the system for OpenCV on non-Windows hosts.
endif()
set(OpenCV_DIR "${_acat_opencv_default}" CACHE PATH "Directory containing OpenCVConfig.cmake")
set(DLIB_LOCATION "${CMAKE_CURRENT_SOURCE_DIR}/VisionExternalInstalls/dlib-20.0"
    CACHE PATH "Directory containing the dlib sources")

# The bundled OpenCV 4.11 pack predates the Visual Studio 2026 compiler.
# Its vc16 binaries are usable with newer MSVC toolsets, but its config file
# does not recognize their compiler version unless the runtime is specified.
if(MSVC AND MSVC_VERSION GREATER_EQUAL 1950 AND NOT DEFINED OpenCV_RUNTIME)
    string(TOLOWER "${OpenCV_DIR}" _acat_opencv_dir)
    string(TOLOWER "${CMAKE_CURRENT_SOURCE_DIR}/VisionExternalInstalls/opencv/build"
        _acat_bundled_opencv_dir)
    if(_acat_opencv_dir STREQUAL _acat_bundled_opencv_dir)
        if(CMAKE_SIZEOF_VOID_P EQUAL 8)
            set(OpenCV_ARCH x64)
        else()
            set(OpenCV_ARCH x86)
        endif()
        set(OpenCV_RUNTIME vc16)
    endif()
endif()

# OpenCV 2.4.13.6's package config only recognizes older MSVC versions.
# Select its available vc14 binaries with VS2022/VS2026 unless overridden.
if(MSVC AND NOT DEFINED OpenCV_RUNTIME)
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        set(_acat_opencv_arch x64)
    else()
        set(_acat_opencv_arch x86)
    endif()
    if(EXISTS "${OpenCV_DIR}/${_acat_opencv_arch}/vc14/lib/OpenCVConfig.cmake"
            AND NOT EXISTS "${OpenCV_DIR}/${_acat_opencv_arch}/vc16/lib/OpenCVConfig.cmake")
        set(OpenCV_ARCH "${_acat_opencv_arch}")
        set(OpenCV_RUNTIME vc14)
    endif()
endif()

option(ACAT_BUILD_DOCUMENTATION "Build the Doxygen documentation" OFF)
if(ACAT_BUILD_DOCUMENTATION)
    find_program(ACAT_DOXYGEN_EXECUTABLE NAMES doxygen)
    if(NOT ACAT_DOXYGEN_EXECUTABLE)
        message(FATAL_ERROR
            "Doxygen is required to build the documentation. Install it or set "
            "ACAT_BUILD_DOCUMENTATION=OFF to build without documentation.")
    endif()
endif()

add_subdirectory(VisionDLLCpp_main)

# Keep the landmark model next to the native executables and the Windows Forms
# application for each build configuration (Debug, Release, etc.). Also stage it
# when the gesture DLL is built by itself rather than through the default target.
add_custom_target(StageLandmarkModel ALL
    COMMAND "${CMAKE_COMMAND}" -E copy_if_different
        "${DLIB_FACE_LANDMARKS_FILE}" "$<TARGET_FILE_DIR:acat_gestures_dll>"
    DEPENDS acat_gestures_dll
    VERBATIM
    COMMENT "Copying dlib face landmark model beside the executables")
add_dependencies(acat_gestures StageLandmarkModel)

option(ACAT_BUILD_WINDOWS_APP "Build the .NET Framework Windows Forms application" ${WIN32})
if(ACAT_BUILD_WINDOWS_APP)
    if(NOT WIN32)
        message(FATAL_ERROR "VisionWindowsAppCS requires Windows and .NET Framework 4.8.1.")
    endif()

    # Visual Studio launches the real C# project from its own bin/<config>
    # directory, not from CMake's bin/<config>. Stage its P/Invoke dependencies
    # there *before* building the external project so Build/F5 both work.
    set(_acat_windows_app_bin
        "${CMAKE_CURRENT_SOURCE_DIR}/VisionWindowsAppCS/bin/$<IF:$<CONFIG:Debug>,Debug,Release>")
    if(OpenCV_LIB_DIR_OPT)
        get_filename_component(_acat_opencv_bin "${OpenCV_LIB_DIR_OPT}/../bin" ABSOLUTE)
    elseif(OpenCV_LIB_PATH)
        get_filename_component(_acat_opencv_bin "${OpenCV_LIB_PATH}/../bin" ABSOLUTE)
    endif()
    if(_acat_opencv_bin)
        file(GLOB _acat_opencv_dlls CONFIGURE_DEPENDS "${_acat_opencv_bin}/opencv*.dll")
        if(NOT _acat_opencv_dlls)
            message(FATAL_ERROR "No OpenCV runtime DLLs found in ${_acat_opencv_bin}")
        endif()
    else()
        message(FATAL_ERROR "OpenCV runtime DLL directory not found; set OpenCV_DIR to a Windows OpenCV package")
    endif()
    add_custom_target(VisionWindowsAppCSRuntime
        COMMAND "${CMAKE_COMMAND}" -E make_directory "${_acat_windows_app_bin}"
        COMMAND "${CMAKE_COMMAND}" -E copy_if_different
            "$<TARGET_FILE:acat_gestures_dll>"
            "$<TARGET_FILE:libivcp>"
            "$<TARGET_FILE:libpipeline>"
            "$<TARGET_FILE:libinfra>"
            "${DLIB_FACE_LANDMARKS_FILE}"
            ${_acat_opencv_dlls}
            "${_acat_windows_app_bin}"
        COMMAND "${CMAKE_COMMAND}" -E copy_if_different
            ${_acat_opencv_dlls} "$<TARGET_FILE_DIR:acat_gestures_dll>"
        DEPENDS acat_gestures_dll libivcp libpipeline libinfra
        VERBATIM
        COMMENT "Staging native runtime dependencies for VisionWindowsAppCS")
    if(CMAKE_GENERATOR MATCHES "^Visual Studio")
        # Show the actual WinForms .csproj in the root Visual Studio solution.
        include_external_msproject(VisionWindowsAppCS
            "${CMAKE_CURRENT_SOURCE_DIR}/VisionWindowsAppCS/VisionWindowsAppCS.csproj"
            TYPE "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
            GUID "{2B492231-D197-45F8-A943-DB8B4A0A0FEC}"
            PLATFORM x64)
        set_target_properties(VisionWindowsAppCS PROPERTIES
            MAP_IMPORTED_CONFIG_MINSIZEREL Release
            MAP_IMPORTED_CONFIG_RELWITHDEBINFO Release)
        # VisionContracts and VisionHost must be in the solution: the IDE does not
        # build project references that are missing from the loaded solution.
        include_external_msproject(VisionContracts
            "${CMAKE_CURRENT_SOURCE_DIR}/VisionContracts/VisionContracts.csproj"
            TYPE "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
            GUID "{A2F7E00A-6BDF-4F30-9C5F-5B1D7724E7A3}"
            PLATFORM "Any CPU")
        include_external_msproject(VisionHost
            "${CMAKE_CURRENT_SOURCE_DIR}/VisionHost/VisionHost.csproj"
            TYPE "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
            GUID "{3C72B34F-4E1B-4B16-A3B7-E2F0D9C6AE5D}"
            PLATFORM x64)
        set_target_properties(VisionContracts VisionHost PROPERTIES
            MAP_IMPORTED_CONFIG_MINSIZEREL Release
            MAP_IMPORTED_CONFIG_RELWITHDEBINFO Release)
        add_dependencies(VisionHost VisionContracts)
        add_dependencies(VisionWindowsAppCS VisionHost VisionWindowsAppCSRuntime StageLandmarkModel)
        add_custom_target(StageWindowsApp ALL
            COMMAND "${CMAKE_COMMAND}" -E copy_if_different
                "${_acat_windows_app_bin}/VisionWindowsAppCS.exe"
                "${_acat_windows_app_bin}/VisionWindowsAppCS.exe.config"
                "${_acat_windows_app_bin}/VisionContracts.dll"
                "${_acat_windows_app_bin}/VisionHost.exe"
                "${_acat_windows_app_bin}/VisionHost.exe.config"
                "$<TARGET_FILE_DIR:acat_gestures_dll>"
            DEPENDS VisionWindowsAppCS VisionHost VisionWindowsAppCSRuntime StageLandmarkModel
            VERBATIM
            COMMENT "Copying VisionWindowsAppCS and VisionHost beside the native DLLs")
    else()
        find_program(ACAT_MSBUILD NAMES MSBuild.exe MSBuild)
        if(NOT ACAT_MSBUILD)
            message(FATAL_ERROR "MSBuild is required for VisionWindowsAppCS; use a Visual Studio generator or set ACAT_BUILD_WINDOWS_APP=OFF.")
        endif()
        add_custom_target(VisionWindowsAppCS ALL
            COMMAND "${ACAT_MSBUILD}"
                "${CMAKE_CURRENT_SOURCE_DIR}/VisionWindowsAppCS/VisionWindowsAppCS.csproj"
                /t:Build /p:Configuration=$<CONFIG> /p:Platform=AnyCPU
                "/p:OutputPath=$<TARGET_FILE_DIR:acat_gestures_dll>/"
            DEPENDS VisionWindowsAppCSRuntime StageLandmarkModel
            VERBATIM)
    endif()
endif()