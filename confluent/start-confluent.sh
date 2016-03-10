# Start Zookeeper
(/usr/bin/zookeeper-server-start /etc/kafka/zookeeper.properties) &

# Start Kafka
(sleep 2; /usr/bin/kafka-server-start /etc/kafka/server.properties) &

# Start REST server
(sleep 4; /usr/bin/kafka-rest-start /etc/kafka-rest/kafka-rest.properties) &

(echo confluent started)