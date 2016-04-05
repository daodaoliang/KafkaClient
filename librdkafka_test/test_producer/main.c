#include <string.h>
#include "rdkafka.h"
#include <stdlib.h>
#include <stdbool.h>

//void produce(const char *host, const char *topic_name, const char *message)
//{
//    
//}

//void consume(const char *host, const char *topic_name)
//{
//    int partition = 0;
//    int64_t start_offset = 0;
//    char errstr[512];
//    rd_kafka_conf_t *conf = rd_kafka_conf_new();
//    rd_kafka_t *kafka = rd_kafka_new(RD_KAFKA_CONSUMER, conf, errstr, sizeof(errstr));
//    //rd_kafka_set_log_level(kafka, LOG_DEBUG);
//    rd_kafka_brokers_add(kafka, host);
//    rd_kafka_topic_conf_t *topic_conf = rd_kafka_topic_conf_new();
//    rd_kafka_topic_t *topic = rd_kafka_topic_new(kafka, topic_name, topic_conf);
//    
//    //rd_kafka_consume_start(topic, partition, start_offset);
//    if (rd_kafka_consume_start(topic, partition, start_offset) == -1){
//        rd_kafka_resp_err_t err = rd_kafka_last_error();
//        fprintf(stderr, "%% Failed to start consuming: %s\n",
//            rd_kafka_err2str(err));
//        if (err == RD_KAFKA_RESP_ERR__INVALID_ARG)
//            fprintf(stderr,
//            "%% Broker based offset storage "
//            "requires a group.id, "
//            "add: -X group.id=yourGroup\n");
//        exit(1);
//    }
//
//    while (1)
//    {
//        rd_kafka_poll(kafka, 0);
//        rd_kafka_message_t *message = rd_kafka_consume(topic, partition, 1000);
//        if (!message)
//            continue;
//
//        printf("%d", message->len);
//        rd_kafka_message_destroy(message);
//    }
//    rd_kafka_consume_stop(topic, partition);
//    while (rd_kafka_outq_len(kafka) > 0)
//        rd_kafka_poll(kafka, 10);
//
//    rd_kafka_topic_destroy(topic);
//    rd_kafka_destroy(kafka);
//}

int main(int argc, char *argv[])
{
    char *host;
    char *topic_name;
    int count;
    int size;
    if (argc < 5)
    {
        host = "192.168.33.12:9092";
        topic_name = "test";
        count = 50000000;
        size = 100;
    }
    else
    {
        host = argv[1];
        topic_name = argv[2];
        count = atoi(argv[3]);
        size = atoi(argv[4]);
    }
    
    char *payload = (char *)calloc(size, sizeof(char));

    int partition = RD_KAFKA_PARTITION_UA;
    char errstr[512];
    rd_kafka_conf_t *conf = rd_kafka_conf_new();
    rd_kafka_t *kafka = rd_kafka_new(RD_KAFKA_PRODUCER, conf, errstr, sizeof(errstr));
    rd_kafka_brokers_add(kafka, host);
    rd_kafka_topic_conf_t *topic_conf = rd_kafka_topic_conf_new();
    rd_kafka_topic_t *topic = rd_kafka_topic_new(kafka, topic_name, topic_conf);
    printf("%s, %d", payload, size);
    
    int sent = 0;
    while (true)
    {
        if (rd_kafka_produce(topic, partition, RD_KAFKA_MSG_F_COPY, payload, size, NULL, 0, NULL) == -1)
        {
            printf("Failed to produce to topic %s partition %i: %s\n", topic_name, partition, rd_kafka_err2str(rd_kafka_last_error()));
            rd_kafka_poll(kafka, 0);
            continue;
        }

        sent++;
        if (sent >= count)
            break;
        rd_kafka_poll(kafka, 0);
    }

    rd_kafka_poll(kafka, 0);

    while (rd_kafka_outq_len(kafka) > 0)
        rd_kafka_poll(kafka, 100);

    rd_kafka_topic_destroy(topic);
    rd_kafka_destroy(kafka);
}
