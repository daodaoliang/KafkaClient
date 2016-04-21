cmake_minimum_required (VERSION 2.6)
project(test_producer)

include_directories(../librdkafka/src)

add_executable(test_producer
    main.c
)

target_link_libraries(test_producer 
    librdkafka
)

add_custom_command(TARGET test_producer POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        $<TARGET_FILE_DIR:librdkafka>/librdkafka.dll
        $<TARGET_FILE_DIR:test_producer>)


add_custom_command(TARGET test_producer POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        ${PROJECT_SOURCE_DIR}/../external/dll/zlib.dll
        $<TARGET_FILE_DIR:test_producer>/zlib.dll)

add_custom_command(TARGET test_producer POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        ${PROJECT_SOURCE_DIR}/../external/dll/zlib.dll
        $<TARGET_FILE_DIR:test_producer>/zlib1.dll)
