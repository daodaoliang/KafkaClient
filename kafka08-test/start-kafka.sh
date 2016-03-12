cd /vagrant

if [ ! -d "kafka-logs" ]; then
	mkdir kafka-logs
fi

cd /home/vagrant/kafka

# Start Zookeeper
(bin/zookeeper-server-start.sh config/zookeeper.properties > /vagrant/kafka-logs/zookeper-log.txt) &

# Start Kafka
(sleep 2; bin/kafka-server-start.sh config/server.properties > /vagrant/kafka-logs/server-log.txt) & 

(echo Kafka 0.8 started, see logs at /vagrant/kafka-logs)