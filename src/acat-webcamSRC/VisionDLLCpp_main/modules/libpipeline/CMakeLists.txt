project(libpipeline)

file(GLOB ${PROJECT_NAME}_headers "include/libpipeline/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp")

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories (${libpipeline_SOURCE_DIR}/include ${libivcp_SOURCE_DIR}/include)

if (BUILD_SHARED_LIBS)
    add_library(${PROJECT_NAME} SHARED ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_data_source_headers} ${${PROJECT_NAME}_sources})
else()
    add_library(${PROJECT_NAME} STATIC ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_data_source_headers} ${${PROJECT_NAME}_sources})
endif()

# Link test executable against IVCP library
target_link_libraries(${PROJECT_NAME} libivcp)
if(BUILD_SHARED_LIBS)
  SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DIVCP_API_EXPORTS")
  #The following action should be performed unconditionaly in case library is built as SHARED
  #and can be removed completely if library is built as STATIC(As it is at the moment)
  target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
endif()

if(UNIX)
  set_target_properties(libpipeline PROPERTIES PREFIX "")
endif()
set_target_properties( ${PROJECT_NAME} PROPERTIES DEBUG_POSTFIX _d)

if(MSVC)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Debug/libpipeline_d.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Release/libpipeline.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Debug/libpipeline_d.lib" DESTINATION lib/static)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Release/libpipeline.lib" DESTINATION lib/static)
else()
    install(TARGETS ${PROJECT_NAME}
            RUNTIME DESTINATION bin
            LIBRARY DESTINATION lib
            ARCHIVE DESTINATION lib/static)   
endif()
install(DIRECTORY "include/libpipeline" DESTINATION include)        

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()
