cd /home/vagrant/kafka
bin/kafka-consumer-perf-test.sh --zookeeper localhost:2181 --messages 50000000 --topic $1 --threads 1
