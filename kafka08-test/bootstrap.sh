#!/usr/bin/env bash

sudo yum -y install java-1.7.0-openjdk-devel
sudo yum -y install git
cd /home/vagrant
git clone https://github.com/jkreps/kafka
cd kafka
./gradlew clean && ./gradlew jar && ./gradlew releaseTarGz -x signArchives
sudo cp /vagrant/config/* config