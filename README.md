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