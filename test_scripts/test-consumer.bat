@echo off
..\bin\windows\kafka-consumer-perf-test.bat --zookeeper %1:2181 --messages 50000000 --topic %2 --threads 1
