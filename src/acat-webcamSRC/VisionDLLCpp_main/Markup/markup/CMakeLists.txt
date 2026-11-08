project(ivcp_markup)

file(GLOB ${PROJECT_NAME}_headers "include/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include ${libivcp_SOURCE_DIR}/include/exceptions ${libivcp_SOURCE_DIR}/include/data_source ${libivcp_SOURCE_DIR}/include/classifier ${libivcp_SOURCE_DIR}/include/feature ${libivcp_SOURCE_DIR}/include/performance ${libivcp_SOURCE_DIR}/include/region_creator ${libivcp_SOURCE_DIR}/include/video_source ${gtest_SOURCE_DIR})
include_directories (${libpipeline_SOURCE_DIR}/include)
include_directories (${libinfra_SOURCE_DIR}/include ${libinfra_SOURCE_DIR}/include/data_source)

add_executable(${PROJECT_NAME} ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
set_target_properties(${PROJECT_NAME} PROPERTIES PROJECT_LABEL "[app] ${PROJECT_NAME}")
if (ENABLE_SOLUTION_FOLDERS)
  set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER app)
endif()

# Link test executable against OpenCV libraries
target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
# Link test executable against IVCP library
target_link_libraries(${PROJECT_NAME} libivcp)
# Link test executable against IVCP Pipeline library
target_link_libraries(${PROJECT_NAME} libpipeline)
# Link test executable against IVCP Infrastructure library
target_link_libraries(${PROJECT_NAME} libinfra)

file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../data/session_markup.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../data/lines.avi DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../data/points.avi DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../data/session_markup.xml DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../data/lines.avi DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../data/points.avi DESTINATION bin/res)

install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)

if (${CREATE_INSTALLER})            
    INCLUDE(CPack)
endif()