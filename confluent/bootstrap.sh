#!/usr/bin/env bash

sudo yum -y install tmux
sudo yum -y install java-1.7.0-openjdk

sudo rpm --import http://packages.confluent.io/rpm/2.0/archive.key

sudo cat > /etc/yum.repos.d/confluent.repo << EOF
[confluent-2.0]
name=Confluent repository for 2.0.x packages
baseurl=http://packages.confluent.io/rpm/2.0
gpgcheck=1
gpgkey=http://packages.confluent.io/rpm/2.0/archive.key
enabled=1
EOF

sudo yum -y install confluent-platform-2.11.7