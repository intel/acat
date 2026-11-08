project (ivcp_region_markup)

file(GLOB ${PROJECT_NAME}_headers "include/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include ./include ${libivcp_SOURCE_DIR}/include ${libivcp_SOURCE_DIR}/include/exceptions ${libivcp_SOURCE_DIR}/include/data_source ${libivcp_SOURCE_DIR}/include/classifier ${libivcp_SOURCE_DIR}/include/feature ${libivcp_SOURCE_DIR}/include/performance ${libivcp_SOURCE_DIR}/include/region_creator ${libivcp_SOURCE_DIR}/include/video_source)
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

file(GLOB ${PROJECT_NAME}_smile_files ${CMAKE_CURRENT_SOURCE_DIR}/../../data/smile/*.avi)
file(COPY ${${PROJECT_NAME}_smile_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/smile)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../data/region_markup_in.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)

install(FILES ${${PROJECT_NAME}_smile_files} DESTINATION bin/res/smile)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../data/region_markup_in.xml DESTINATION bin/res)
    
install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()