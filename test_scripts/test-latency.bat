@echo off
..\bin\windows\kafka-run-class.bat kafka.tools.TestEndToEndLatency %1:9092 %1:2181 %2 5000