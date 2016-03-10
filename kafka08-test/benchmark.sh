cd /home/vagrant/kafka
bin/kafka-topics.sh --zookeeper localhost:2181 --create --topic test1 --partitions 6 --replication-factor 1
bin/kafka-run-class.sh org.apache.kafka.clients.tools.ProducerPerformance test1 50000000 100 -1 acks=1 bootstrap.servers=localhost:9092 buffer.memory=67108864 batch.size=8196