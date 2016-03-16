#!/usr/bin/env bash

sudo yum -y install tmux
sudo yum -y install java-1.7.0-openjdk

wget "http://apache-mirror.rbc.ru/pub/apache/kafka/0.9.0.1/kafka_2.11-0.9.0.1.tgz" -O /home/vagrant/kafka.tgz

mkdir -p /home/vagrant/kafka && cd /home/vagrant/kafka

tar -xvzf /home/vagrant/kafka.tgz --strip 1

sudo cp /vagrant/server.properties /home/vagrant/kafka/config