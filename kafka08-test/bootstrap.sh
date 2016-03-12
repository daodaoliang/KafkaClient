#!/usr/bin/env bash

sudo yum -y install java-1.7.0-openjdk-devel
sudo yum -y install git
cd /home/vagrant
git clone https://github.com/jkreps/kafka
cd kafka
./gradlew clean && ./gradlew jar && ./gradlew releaseTarGz -x signArchives

if [ -d "/vagrant/config" ]; then
	sudo cp /vagrant/config/* config
fi

if [ -f "/vagrant/kafka-run-class.sh" ]; then
	sudo cp /vagrant/kafka-run-class.sh bin
fi
