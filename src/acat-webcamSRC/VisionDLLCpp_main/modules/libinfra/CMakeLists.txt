project(libinfra)

file(GLOB ${PROJECT_NAME}_headers "include/libinfra/*.hpp" "include/libinfra/repository/*.hpp" "include/libinfra/data_source/*.hpp")
file(GLOB ${PROJECT_NAME}_sources "src/*.cpp" "src/repository/*.cpp" "src/data_source/*.cpp")

file(GLOB ${PROJECT_NAME}_repository_headers "include/libinfra/repository/*.hpp")
file(GLOB ${PROJECT_NAME}_repository_sources "src/repository/*.cpp")

file(GLOB ${PROJECT_NAME}_data_source_headers "include/libinfra/data_source/*.hpp")
file(GLOB ${PROJECT_NAME}_data_source_sources "src/data_source/*.cpp")

source_group("repository" FILES ${${PROJECT_NAME}_repository_headers} ${${PROJECT_NAME}_repository_sources})
source_group("data_source" FILES ${${PROJECT_NAME}_data_source_headers} ${${PROJECT_NAME}_data_source_sources})

include_directories(${CMAKE_SOURCE_DIR}/include)
include_directories(${libinfra_SOURCE_DIR}/include)
include_directories(${libpipeline_SOURCE_DIR}/include)
include_directories(${libivcp_SOURCE_DIR}/include)

# Public infrastructure headers include <dlib/...>; consumers need this path too.
include_directories(${DLIB_LOCATION})

if (HAVE_DLIB)
    SET(DLIB_SOURCE_FILE ${DLIB_LOCATION}/dlib/all/source.cpp)
    SET(${PROJECT_NAME}_sources ${${PROJECT_NAME}_sources} ${DLIB_SOURCE_FILE})
    if (WIN32)
        set_source_files_properties(${DLIB_SOURCE_FILE} PROPERTIES COMPILE_FLAGS "/W0 /bigobj")
    endif()
endif()

if(BUILD_SHARED_LIBS)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -DIVCP_API_EXPORTS")
    #The following action should be performed unconditionaly in case library is built as SHARED
    #and can be removed completely if library is built as STATIC(As it is at the moment)
    add_library(${PROJECT_NAME} SHARED ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
    target_link_libraries(${PROJECT_NAME} ${OpenCV_LIBS})
else()
    add_library(${PROJECT_NAME} STATIC ${${PROJECT_NAME}_headers} ${${PROJECT_NAME}_sources})
endif()


target_link_libraries(${PROJECT_NAME} libivcp libpipeline)
if (UNIX AND HAVE_DLIB)
    target_link_libraries(${PROJECT_NAME} -lpthread -lX11)
endif()

if(UNIX)
    set_target_properties(libinfra PROPERTIES PREFIX "")
endif()
set_target_properties( ${PROJECT_NAME} PROPERTIES DEBUG_POSTFIX _d)

if(MSVC)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Debug/libinfra_d.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/bin/Release/libinfra.dll" DESTINATION bin)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Debug/libinfra_d.lib" DESTINATION lib/static)
    install(FILES "${CMAKE_BINARY_DIR}/lib/Release/libinfra.lib" DESTINATION lib/static)
else()
    install(TARGETS ${PROJECT_NAME}
            RUNTIME DESTINATION bin
            LIBRARY DESTINATION lib
            ARCHIVE DESTINATION lib/static)   
endif()
install(FILES "include/libinfra/ivcp_infra.hpp" DESTINATION include/libinfra) 

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()

