@echo off
..\bin\windows\kafka-run-class.bat org.apache.kafka.clients.tools.ProducerPerformance %2 50000000 100 -1 acks=1 bootstrap.servers=%1:9092 buffer.memory=67108864 batch.size=8196