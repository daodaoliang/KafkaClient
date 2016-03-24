@echo off
start ..\bin\windows\kafka-run-class.bat org.apache.kafka.clients.tools.ProducerPerformance %2 50000000 100 -1 acks=1 bootstrap.servers=%1:9092 buffer.memory=67108864 batch.size=8196
start ..\bin\windows\kafka-consumer-perf-test.bat --zookeeper %1:2181 --messages 50000000 --topic %2 --threads 1
