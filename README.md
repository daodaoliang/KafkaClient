# KafkaClient
A client for Kafka REST Proxy

Quickstart
----------

Bring up test environment:

    $ cd ./confluent
    $ vagrant up

Run benchmark:

    $ cd ./KafkaClient/bin
    $ start Consumer.exe
    $ start Publisher.exe

To see what kind of performance we should aim for in confluent platform tests I've replicated [this](https://engineering.linkedin.com/kafka/benchmarking-apache-kafka-2-million-writes-second-three-cheap-machines) Kafka 0.8 performance test. To run it on your machine, use 'vagrant up' in kafka08-test and then use [these](https://gist.github.com/jkreps/c7ddb4041ef62a900e6c#file-benchmark-commands-txt) commands.

Run Kafka 0.8 benchmark:

	$ cd ./kafka08-test
	$ vagrant up
	$ vagrant ssh
	$ cd /vagrant
	$ ./bench-create-topic.sh topic-name
	$ ./bench-producer.sh topic-name
	$ ./bench-consumer.sh topic-name
	$ ./bench-end-to-end-latency.sh topic-name
	$ ./bench-producer-consumer.sh topic-name
