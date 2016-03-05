call %~dp0cleanup-dir.cmd %~dp0bin
set msbuild="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe"
%msbuild% KafkaClient.sln /target:Rebuild /p:Configuration=Release /verbosity:m