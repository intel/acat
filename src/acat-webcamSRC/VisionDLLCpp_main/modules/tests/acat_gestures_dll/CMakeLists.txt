project (acat_gestures_dll)

file(GLOB ${PROJECT_NAME}_sources "*.cpp" "../acat_gestures/acat_gestures_common.cpp" "*.hpp" "*.h")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include)
include_directories(${libpipeline_SOURCE_DIR}/include)
include_directories(${libinfra_SOURCE_DIR}/include)
include_directories("../acat_gestures")

file(COPY ${DLIB_FACE_LANDMARKS_FILE} DESTINATION ${CMAKE_BINARY_DIR}/modules/tests/acat_gestures_dll)

if (BUILD_SHARED_LIBS)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DACAT_GESTURES_DLL_EXPORTS")
    add_library(${PROJECT_NAME} SHARED  ${${PROJECT_NAME}_sources})
else()
    add_library(${PROJECT_NAME} STATIC  ${${PROJECT_NAME}_sources})
endif()

target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS} ${${PROJECT_NAME}_libraries})
target_link_libraries(${PROJECT_NAME} libivcp)
target_link_libraries(${PROJECT_NAME} libpipeline)
target_link_libraries(${PROJECT_NAME} libinfra)

# Also stage the model when this DLL is built as an individual target.
add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
    COMMAND "${CMAKE_COMMAND}" -E copy_if_different
        "${DLIB_FACE_LANDMARKS_FILE}" "$<TARGET_FILE_DIR:${PROJECT_NAME}>"
    VERBATIM)

if(UNIX)
    set_target_properties(libivcp PROPERTIES PREFIX "")
endif()
set_target_properties( ${PROJECT_NAME} PROPERTIES DEBUG_POSTFIX _d)
if (HAVE_REALSENSE)
set_target_properties(${PROJECT_NAME} PROPERTIES OUTPUT_NAME "${PROJECT_NAME}_RS")
endif()


if(MSVC)
if (HAVE_REALSENSE)
	install(FILES "${CMAKE_BINARY_DIR}/bin/Debug/acat_gestures_dll_RS_d.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Release/acat_gestures_dll_RS.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Debug/acat_gestures_dll_RS_d.lib" DESTINATION lib/static)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Release/acat_gestures_dll_RS.lib" DESTINATION lib/static)
else()
    install(FILES "${CMAKE_BINARY_DIR}/bin/Debug/acat_gestures_dll_d.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Release/acat_gestures_dll.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Debug/acat_gestures_dll_d.lib" DESTINATION lib/static)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Release/acat_gestures_dll.lib" DESTINATION lib/static)
endif()
else()
    install(TARGETS ${PROJECT_NAME}
            RUNTIME DESTINATION bin
            LIBRARY DESTINATION lib
            ARCHIVE DESTINATION lib/static)
endif()

install(FILES "acat_gestures_dll.h" DESTINATION include/acatVision)

if (HAVE_DLIB)
    install(FILES ${DLIB_FACE_LANDMARKS_FILE} DESTINATION bin)
endif()

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()

