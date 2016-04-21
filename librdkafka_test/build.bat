@echo off
cmake_from_kafka_vcxproj.py
mkdir build
cd build
cmake ..
set msbuild="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MsBuild.exe"
%msbuild% librdkafka.sln /target:Rebuild /p:Configuration=Release /verbosity:m
