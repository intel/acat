add_subdirectory(libivcp)
add_subdirectory(libpipeline)
add_subdirectory(libinfra)
add_subdirectory(tests)

if (${CREATE_INSTALLER})
    INCLUDE(CPack)
endif()
