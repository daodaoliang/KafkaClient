# KafkaClient
A client for Kafka REST Proxy

Quickstart
----------

Before running the examples, make sure that Zookeeper, Kafka and Schema Registry are
running. In what follows, we assume that Zookeeper, Kafka and Schema Registry are
started with the default settings.

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
