cmake_minimum_required (VERSION 2.6)
project(test_producer)

include_directories(../librdkafka)
add_executable(test_producer main.c)
target_link_libraries(test_producer 
    librdkafka
)
