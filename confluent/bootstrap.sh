#!/usr/bin/env bash

sudo apt-get update
sudo apt-get -y install python-software-properties
sudo apt-get -y install openjdk-7-jre-headless
sudo apt-get -y install tmux

wget -qO - http://packages.confluent.io/deb/2.0/archive.key | sudo apt-key add -
sudo add-apt-repository "deb [arch=amd64] http://packages.confluent.io/deb/2.0 stable main"
sudo apt-get update && sudo apt-get -y install confluent-platform-2.11.7
