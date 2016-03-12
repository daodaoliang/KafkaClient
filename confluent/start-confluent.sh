cd /vagrant

if [ ! -d "kafka-logs" ]; then
	mkdir kafka-logs
fi

# Start Zookeeper
(/usr/bin/zookeeper-server-start /etc/kafka/zookeeper.properties > /vagrant/kafka-logs/zookeper-log.txt) &

# Start Kafka
(sleep 2; /usr/bin/kafka-server-start /etc/kafka/server.properties > /vagrant/kafka-logs/server-log.txt) &

# Start REST server
(sleep 4; /usr/bin/kafka-rest-start /etc/kafka-rest/kafka-rest.properties > /vagrant/kafka-logs/kafka-rest-log.txt) &

(echo confluent started)