project(ivcp_performance_test)

file(GLOB ${PROJECT_NAME}_headers "include/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp")

enable_testing()
include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include)
include_directories(${libpipeline_SOURCE_DIR}/include)
include_directories(${libinfra_SOURCE_DIR}/include)

add_executable(${PROJECT_NAME} ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
set_target_properties(${PROJECT_NAME} PROPERTIES PROJECT_LABEL "[test] ${PROJECT_NAME}")
if (ENABLE_SOLUTION_FOLDERS)
  set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER test)
endif()

# Link test executable against OpenCV libraries
target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
# Link test executable against IVCP library
target_link_libraries(${PROJECT_NAME} libivcp)
# Link test executable against IVCP Pipeline library
target_link_libraries(${PROJECT_NAME} libpipeline)
# Link test executable against IVCP infrastructure library
target_link_libraries(${PROJECT_NAME} libinfra)

add_test( ${PROJECT_NAME} ${PROJECT_NAME} )

file(GLOB ${PROJECT_NAME}_blink_files ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/blink/*.avi)
file(COPY ${${PROJECT_NAME}_blink_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/blink)
file(GLOB ${PROJECT_NAME}_smile_files ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/smile/*.avi)
file(COPY ${${PROJECT_NAME}_smile_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/smile)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/performance_test_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/performance_train_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)

install(FILES ${${PROJECT_NAME}_blink_files} DESTINATION bin/res/blink)
install(FILES ${${PROJECT_NAME}_smile_files} DESTINATION bin/res/smile)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/performance_test_session.xml ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/performance_train_session.xml DESTINATION bin/res)

install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)
            
if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()