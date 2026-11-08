project(ivcp_demo_lr_classification)

file(GLOB ${PROJECT_NAME}_sources "src/*.cpp")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include)
include_directories(${libpipeline_SOURCE_DIR}/include)
include_directories(${libinfra_SOURCE_DIR}/include)

add_executable(${PROJECT_NAME} ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
set_target_properties(${PROJECT_NAME} PROPERTIES PROJECT_LABEL "[demo] ${PROJECT_NAME}")
if (ENABLE_SOLUTION_FOLDERS)
  set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER demo)
endif() 

target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
target_link_libraries(${PROJECT_NAME} libivcp)
target_link_libraries(${PROJECT_NAME} libinfra)

install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)
            
if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()
