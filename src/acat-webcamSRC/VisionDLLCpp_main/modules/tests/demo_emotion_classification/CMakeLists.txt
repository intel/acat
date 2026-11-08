project (ivcp_demo_emotion_classification)

file(GLOB ${PROJECT_NAME}_headers "include/*.hpp")
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

# Link test executable against OpenCV libraries
target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
# Link test executable against IVCP library
target_link_libraries(${PROJECT_NAME} libivcp)
# Link test executable against IVCP Pipeline library
target_link_libraries(${PROJECT_NAME} libpipeline)
# Link test executable against IVCP Infrastructure library
target_link_libraries(${PROJECT_NAME} libinfra)

file(GLOB ${PROJECT_NAME}_blink_files ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/blink/*.avi)
file(COPY ${${PROJECT_NAME}_blink_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/blink)
file(GLOB ${PROJECT_NAME}_smile_files ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/smile/*.avi)
file(COPY ${${PROJECT_NAME}_smile_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/smile)
file(GLOB ${PROJECT_NAME}_amaze_files ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/amaze/*.avi)
file(COPY ${${PROJECT_NAME}_amaze_files} DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res/amaze)

file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/amaze_test_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/amaze_train_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/smile_test_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/smile_train_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/emotion_test_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/emotion_train_session.xml DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/res)

install(FILES ${${PROJECT_NAME}_smile_files} DESTINATION bin/res/blink)
install(FILES ${${PROJECT_NAME}_smile_files} DESTINATION bin/res/smile)
install(FILES ${${PROJECT_NAME}_amaze_files} DESTINATION bin/res/amaze)

install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/amaze_test_session.xml DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/amaze_train_session.xml DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/smile_test_session.xml DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/smile_train_session.xml DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/emotion_test_session.xml DESTINATION bin/res)
install(FILES ${CMAKE_CURRENT_SOURCE_DIR}/../../../data/emotion_train_session.xml DESTINATION bin/res)
    
install(TARGETS ${PROJECT_NAME}
        RUNTIME DESTINATION bin
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib/static)

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()