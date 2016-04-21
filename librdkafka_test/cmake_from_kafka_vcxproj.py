import re

header_re = re.compile(r'<ClInclude Include="(.+?)" />')
source_re = re.compile(r'<ClCompile Include="(.+?)" />')

with open('librdkafka\win32\librdkafka.vcxproj', 'r') as f:
    vcxproj = f.read()

rep = lambda x: x.replace('..', '../librdkafka').replace('\\', '/')
src_files = (
    list(map(rep, header_re.findall(vcxproj)))
    + list(map(rep, source_re.findall(vcxproj)))
)

cmake_file = """
cmake_minimum_required (VERSION 2.6)
project(librdkafka)

include_directories(../external/include)

add_library(librdkafka SHARED
   """ + '\n'.join(src_files) + """
)

target_link_libraries(librdkafka 
    ../../external/lib/ssleay32MT
    ../../external/lib/libeay32MT
    ../../external/lib/zdll
)
"""

with open('librd/CMakeLists.txt', 'w+') as f:
    f.write(cmake_file)
