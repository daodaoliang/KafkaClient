#!/usr/bin/env bash

sudo yum -y install git

sudo yum -y install gcc
sudo yum -y install gcc-c++

cd /home/vagrant
git clone https://github.com/edenhill/librdkafka
cd librdkafka

./configure
make
sudo make install
