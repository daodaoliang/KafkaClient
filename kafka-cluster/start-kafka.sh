cd /vagrant

if [ ! -d "kafka-logs" ]; then
	mkdir kafka-logs
fi

for ((i = 1; i < 4; i++)); do
    if [ ! -d "kafka-logs/kafka$i" ]; then
        mkdir kafka-logs/kafka$i
    fi
done

cd /home/vagrant/kafka

# Start Zookeeper
(bin/zookeeper-server-start.sh config/zookeeper.properties > /vagrant/kafka-logs/kafka$1/zookeper-log.txt) &

# Start Kafka
(sleep 2; bin/kafka-server-start.sh config/server.properties > /vagrant/kafka-logs/kafka$1/server-log.txt) & 

(echo Kafka 0.9.1 started, see logs at /vagrant/kafka-logs)
