cmake_minimum_required (VERSION 2.6)
project(test_consumer)

include_directories(../librdkafka/src)

add_executable(test_consumer
    main.c
)

target_link_libraries(test_consumer 
    librdkafka
)

add_custom_command(TARGET test_consumer POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        $<TARGET_FILE_DIR:librdkafka>/librdkafka.dll
        $<TARGET_FILE_DIR:test_consumer>)


add_custom_command(TARGET test_consumer POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        ${PROJECT_SOURCE_DIR}/../external/dll/zlib.dll
        $<TARGET_FILE_DIR:test_consumer>/zlib.dll)

add_custom_command(TARGET test_consumer POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        ${PROJECT_SOURCE_DIR}/../external/dll/zlib.dll
        $<TARGET_FILE_DIR:test_consumer>/zlib1.dll)
