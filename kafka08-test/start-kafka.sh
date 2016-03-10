cd /home/vagrant/kafka

# Start Zookeeper
(bin/zookeeper-server-start.sh config/zookeeper.properties) &

# Start Kafka
(sleep 2; bin/kafka-server-start.sh config/server.properties) &

(echo kafka 0.8 started)