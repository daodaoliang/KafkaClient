cd /home/vagrant/kafka
bin/kafka-run-class.sh kafka.tools.TestEndToEndLatency localhost:9092 localhost:2181 $1 5000
