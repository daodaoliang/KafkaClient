# KafkaClient
A client for Kafka REST Proxy

Quickstart
----------

Get [Ubuntu](http://www.ubuntu.com/download/desktop)

Get latest [Confluent platform](http://www.confluent.io/developer#download) 

Extract Confluent platform, then run the following commands:

    # Start Zookeeper
    $ bin/zookeeper-server-start etc/kafka/zookeeper.properties

    # Start Kafka
    $ bin/kafka-server-start etc/kafka/server.properties

    # Start Schema Registry
    $ bin/schema-registry-start etc/schema-registry/schema-registry.properties

    # Start REST server
    $ bin/kafka-rest-start etc/kafka-rest/kafka-rest.properties

Then create a topic called JsonTestTopic:

    # Create JsonTestTopic topic
    $ bin/kafka-topics --create --zookeeper localhost:2181 --replication-factor 1 \
      --partitions 1 --topic JsonTestTopic

Run 

	$ ifconfig

Then, in your windows environment, run

	>KafkaClient.exe <ip-address-from-ifconfig>:8082

