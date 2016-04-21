@echo off
cmake ..
set msbuild="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MsBuild.exe"
%msbuild% librdkafka_test.sln /target:Rebuild /p:Configuration=Release /verbosity:m
