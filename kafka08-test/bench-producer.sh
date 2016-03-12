cd /home/vagrant/kafka
bin/kafka-run-class.sh org.apache.kafka.clients.tools.ProducerPerformance $1 50000000 100 -1 acks=1 bootstrap.servers=localhost:9092 buffer.memory=67108864 batch.size=8196