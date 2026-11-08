set(REALSENSE_FOUND)

set(REALSENSE_POSSIBLE_PATHS ${REALSENSE_ROOT} $ENV{REALSENSE_ROOT} "C:/Program Files (x86)/Intel/RSSDK")

if (WIN32)
    if(CMAKE_CL_64)
        set(REALSENSE_LIB_DIR "lib/x64")
        #set(REALSENSE_BIN_DIR "bin/x64")
    else()
        set(REALSENSE_LIB_DIR "lib/Win32")
        #set(REALSENSE_BIN_DIR "bin/Win32")
    endif()

    find_path(RealSense_INCLUDE_DIR NAMES pxcbase.h PATH_SUFFIXES include PATHS ${REALSENSE_POSSIBLE_PATHS})
    find_library(RealSense_LIBRARY NAMES pxc libpxc libpxc.lib PATH_SUFFIXES ${REALSENSE_LIB_DIR} PATHS ${REALSENSE_POSSIBLE_PATHS})

    if (EXISTS ${RealSense_INCLUDE_DIR} AND EXISTS ${RealSense_LIBRARY})
        set(REALSENSE_FOUND 1)
    else()
        set(REALSENSE_FOUND 0)
    endif()
else()
    set(REALSENSE_FOUND 0)
endif()

mark_as_advanced(FORCE REALSENSE_FOUND)
mark_as_advanced(FORCE RealSense_INCLUDE_DIR)
mark_as_advanced(FORCE RealSense_LIBRARY)