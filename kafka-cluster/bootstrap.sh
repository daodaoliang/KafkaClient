#!/usr/bin/env bash

sudo yum -y install java-1.7.0-openjdk

wget "http://apache-mirror.rbc.ru/pub/apache/kafka/0.9.0.1/kafka_2.11-0.9.0.1.tgz" -O /home/vagrant/kafka.tgz

mkdir -p /home/vagrant/kafka && cd /home/vagrant/kafka

tar -xvzf /home/vagrant/kafka.tgz --strip 1

sudo cat > /home/vagrant/kafka/config/server.properties << EOF
broker.id=$1
listeners=PLAINTEXT://:9092
host.name=192.168.33.2$1
advertised.host.name=192.168.33.2$1
num.network.threads=3
num.io.threads=8
socket.send.buffer.bytes=102400
socket.receive.buffer.bytes=102400
socket.request.max.bytes=104857600
log.dirs=/tmp/kafka-logs
num.partitions=1
num.recovery.threads.per.data.dir=1
log.retention.hours=168
log.segment.bytes=1073741824
log.retention.check.interval.ms=300000
zookeeper.connect=192.168.33.21:2181,192.168.33.22:2181,192.168.33.23:2181
zookeeper.connection.timeout.ms=1200000
EOF

sudo cat > /home/vagrant/kafka/config/zookeeper.properties << EOF
dataDir=/tmp/zookeeper
clientPort=2181
maxClientCnxns=0
tickTime=2000
initLimit=20
syncLimit=2
server.1=192.168.33.21:2888:3888
server.2=192.168.33.22:2888:3888
server.3=192.168.33.23:2888:3888
EOF

if [ ! -d "/tmp/zookeeper" ]; then
    mkdir /tmp/zookeeper
fi

sudo echo -n $1 > /tmp/zookeeper/myid
