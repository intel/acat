project(libivcp)


file(GLOB ${PROJECT_NAME}_headers "include/libivcp/*.hpp" "include/libivcp/data_source/*.hpp" "include/libivcp/exceptions/*.hpp"  "include/libivcp/classifier/*.hpp" "include/libivcp/feature/*.hpp"  "include/libivcp/performance/*.hpp"  "include/libivcp/region_creator/*.hpp"  "include/libivcp/video_source/*.hpp"  "include/libivcp/video_writer/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp" "src/exceptions/*.cpp" "src/classifier/*.cpp" "src/data_source/*.cpp" "src/feature/*.cpp" "src/performance/*.cpp" "src/region_creator/*.cpp" "src/video_source/*.cpp" "src/video_writer/*.cpp")

# These files contain embedded copies of libsvm/liblinear algorithms. Their
# original shadowed identifiers are intentional; avoid editing the algorithms
# solely to accommodate newer MSVC shadow diagnostics.
if(MSVC)
    set_source_files_properties(src/classifier/libsvm.cpp PROPERTIES COMPILE_OPTIONS "/wd4458")
    set_source_files_properties(src/classifier/liblr.cpp PROPERTIES COMPILE_OPTIONS "/wd4458;/wd4456")
endif()

file(GLOB ${PROJECT_NAME}_exceptions_headers "include/libivcp/exceptions/*.hpp")
file(GLOB ${PROJECT_NAME}_exceptions_sources "src/exceptions/*.cpp")

file(GLOB ${PROJECT_NAME}_classifier_headers "include/libivcp/classifier/*.hpp")
file(GLOB ${PROJECT_NAME}_classifier_sources "src/classifier/*.cpp")

file(GLOB ${PROJECT_NAME}_feature_headers "include/libivcp/feature/*.hpp")
file(GLOB ${PROJECT_NAME}_feature_sources "src/feature/*.cpp")

file(GLOB ${PROJECT_NAME}_performance_headers "include/libivcp/performance/*.hpp")
file(GLOB ${PROJECT_NAME}_performance_sources "src/performance/*.cpp")

file(GLOB ${PROJECT_NAME}_region_creator_headers "include/libivcp/region_creator/*.hpp")
file(GLOB ${PROJECT_NAME}_region_creator_sources "src/region_creator/*.cpp")

file(GLOB ${PROJECT_NAME}_video_source_headers "include/libivcp/video_source/*.hpp")
file(GLOB ${PROJECT_NAME}_video_source_sources "src/video_source/*.cpp")

file(GLOB ${PROJECT_NAME}_data_source_headers "include/libivcp/data_source/*.hpp")
file(GLOB ${PROJECT_NAME}_data_source_sources "src/data_source/*.cpp")

file(GLOB ${PROJECT_NAME}_video_writer_headers "include/libivcp/video_writer/*.hpp")
file(GLOB ${PROJECT_NAME}_video_writer_sources "src/video_writer/*.cpp")

source_group("exceptions" FILES ${${PROJECT_NAME}_exceptions_headers} ${${PROJECT_NAME}_exceptions_sources})
source_group("data_source" FILES ${${PROJECT_NAME}_data_source_headers} ${${PROJECT_NAME}_data_source_sources})
source_group("classifier" FILES ${${PROJECT_NAME}_classifier_headers} ${${PROJECT_NAME}_classifier_sources})
source_group("feature" FILES ${${PROJECT_NAME}_feature_headers} ${${PROJECT_NAME}_feature_sources})
source_group("performance" FILES ${${PROJECT_NAME}_performance_headers} ${${PROJECT_NAME}_performance_sources})
source_group("region_creator" FILES ${${PROJECT_NAME}_region_creator_headers} ${${PROJECT_NAME}_region_creator_sources})
source_group("video_source" FILES ${${PROJECT_NAME}_video_source_headers} ${${PROJECT_NAME}_video_source_sources})
source_group("video_writer" FILES ${${PROJECT_NAME}_video_writer_headers} ${${PROJECT_NAME}_video_writer_sources})


include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories (${libivcp_SOURCE_DIR}/include)

if (BUILD_SHARED_LIBS)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DIVCP_API_EXPORTS")
    add_library(${PROJECT_NAME} SHARED ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
else()
    add_library(${PROJECT_NAME} STATIC ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
endif()

target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS} ${${PROJECT_NAME}_libraries})


if (HAVE_REALSENSE)
    set_target_properties(${PROJECT_NAME} PROPERTIES LINK_FLAGS "/NODEFAULTLIB:LIBCMT.lib") 
    target_link_libraries(${PROJECT_NAME} ${RealSense_LIBRARY})
endif()


if(UNIX)
    set_target_properties(libivcp PROPERTIES PREFIX "")
endif()
set_target_properties( ${PROJECT_NAME} PROPERTIES DEBUG_POSTFIX _d)

if(MSVC)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Debug/libivcp_d.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Release/libivcp.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Debug/libivcp_d.lib" DESTINATION lib/static)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Release/libivcp.lib" DESTINATION lib/static)
else()
    install(TARGETS ${PROJECT_NAME}
            RUNTIME DESTINATION bin
            LIBRARY DESTINATION lib
            ARCHIVE DESTINATION lib/static)   
endif()
        
install(DIRECTORY "include/libivcp" DESTINATION include)

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()

