project (acat_gestures)

file(GLOB ${PROJECT_NAME}_sources "*.cpp" "*.hpp" "*.h" "*.rc" "../acat_gestures_dll/acat_gestures_dll.h")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include)
include_directories(${libpipeline_SOURCE_DIR}/include)
include_directories(${libinfra_SOURCE_DIR}/include)
include_directories("../acat_gestures_dll")

file(COPY ${DLIB_FACE_LANDMARKS_FILE} DESTINATION ${CMAKE_BINARY_DIR}/modules/tests/acat_gestures)

add_executable(${PROJECT_NAME} ${${PROJECT_NAME}_sources})
set_target_properties(${PROJECT_NAME} PROPERTIES PROJECT_LABEL "[demo] ${PROJECT_NAME}")  
if (HAVE_REALSENSE)
set_target_properties(${PROJECT_NAME} PROPERTIES OUTPUT_NAME "${PROJECT_NAME}_RS")
endif()
if (ENABLE_SOLUTION_FOLDERS)
  set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER demo)
endif()

target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
target_link_libraries(${PROJECT_NAME} libivcp)
target_link_libraries(${PROJECT_NAME} libpipeline)
target_link_libraries(${PROJECT_NAME} libinfra)
    
install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)
if (HAVE_DLIB)
    install(FILES ${DLIB_FACE_LANDMARKS_FILE} DESTINATION bin)
endif()

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()