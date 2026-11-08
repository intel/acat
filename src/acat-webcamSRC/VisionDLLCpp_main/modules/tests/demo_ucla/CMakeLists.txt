project(ivcp_demo_ucla)

file(GLOB ${PROJECT_NAME}_headers "include/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(./include)
include_directories(${libivcp_SOURCE_DIR}/include)
include_directories(${libpipeline_SOURCE_DIR}/include)
include_directories(${libinfra_SOURCE_DIR}/include)

add_executable(${PROJECT_NAME} ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
set_target_properties(${PROJECT_NAME} PROPERTIES PROJECT_LABEL "[demo] ${PROJECT_NAME}")
if (ENABLE_SOLUTION_FOLDERS)
  set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER demo)
endif()

# Link test executable against OpenCV libraries
target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
# Link test executable against IVCP library
target_link_libraries(${PROJECT_NAME} libivcp)
# Link test executable against IVCP Pipeline library
target_link_libraries(${PROJECT_NAME} libpipeline)
# Link test executable against IVCP infrastructure library
target_link_libraries(${PROJECT_NAME} libinfra)

file(GLOB ${PROJECT_NAME}_ucla2_files ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/ucla2/*.avi)
file(COPY ${${PROJECT_NAME}_ucla2_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/ucla2)

install(FILES ${${PROJECT_NAME}_ucla2_files} DESTINATION bin/res/ucla2)

install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)
            
if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()
