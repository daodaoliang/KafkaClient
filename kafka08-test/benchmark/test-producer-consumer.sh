cd /home/vagrant/kafka

(bin/kafka-run-class.sh org.apache.kafka.clients.tools.ProducerPerformance $1 50000000 100 -1 acks=1 bootstrap.servers=localhost:9092 buffer.memory=67108864 batch.size=8196 > /vagrant/kafka-logs/test-publisher-$1.txt) &

(bin/kafka-consumer-perf-test.sh --zookeeper localhost:2181 --messages 50000000 --topic $1 --threads 1 > /vagrant/kafka-logs/test-consumer-$1.txt) &
