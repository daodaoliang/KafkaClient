@echo off
..\bin\windows\kafka-topics.bat --zookeeper %1:2181 --create --topic %2 --partitions 6 --replication-factor 1