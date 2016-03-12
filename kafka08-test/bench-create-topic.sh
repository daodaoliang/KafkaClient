cd /home/vagrant/kafka
bin/kafka-topics.sh --zookeeper localhost:2181 --create --topic $1 --partitions 6 --replication-factor 1