#include <string.h>
#include "rdkafka.h"

void produce(const char *host, const char *topic_name, const char *message)
{
    int partition = RD_KAFKA_PARTITION_UA;
    char errstr[512];
    rd_kafka_conf_t *conf = rd_kafka_conf_new();
    rd_kafka_t *kafka = rd_kafka_new(RD_KAFKA_PRODUCER, conf, errstr, sizeof(errstr));
    rd_kafka_brokers_add(kafka, host);
    rd_kafka_topic_conf_t *topic_conf = rd_kafka_topic_conf_new();
    rd_kafka_topic_t *topic = rd_kafka_topic_new(kafka, topic_name, topic_conf);
    printf("%s, %d", message, sizeof(message));
    char buf[512];
    fgets(buf, sizeof(buf), stdin);
    size_t len = strlen(buf);

    rd_kafka_produce(topic, partition, RD_KAFKA_MSG_F_COPY, buf, len, NULL, 0, NULL);
    rd_kafka_poll(kafka, 0);

    while (rd_kafka_outq_len(kafka) > 0)
        rd_kafka_poll(kafka, 100);

    rd_kafka_topic_destroy(topic);
    rd_kafka_destroy(kafka);
}

void consume(const char *host, const char *topic_name)
{
    int partition = 0;
    int64_t start_offset = 0;
    char errstr[512];
    rd_kafka_conf_t *conf = rd_kafka_conf_new();
    rd_kafka_t *kafka = rd_kafka_new(RD_KAFKA_CONSUMER, conf, errstr, sizeof(errstr));
    //rd_kafka_set_log_level(kafka, LOG_DEBUG);
    rd_kafka_brokers_add(kafka, host);
    rd_kafka_topic_conf_t *topic_conf = rd_kafka_topic_conf_new();
    rd_kafka_topic_t *topic = rd_kafka_topic_new(kafka, topic_name, topic_conf);
    
    //rd_kafka_consume_start(topic, partition, start_offset);
    if (rd_kafka_consume_start(topic, partition, start_offset) == -1){
        rd_kafka_resp_err_t err = rd_kafka_last_error();
        fprintf(stderr, "%% Failed to start consuming: %s\n",
            rd_kafka_err2str(err));
        if (err == RD_KAFKA_RESP_ERR__INVALID_ARG)
            fprintf(stderr,
            "%% Broker based offset storage "
            "requires a group.id, "
            "add: -X group.id=yourGroup\n");
        exit(1);
    }

    while (1)
    {
        rd_kafka_poll(kafka, 0);
        rd_kafka_message_t *message = rd_kafka_consume(topic, partition, 1000);
        if (!message)
            continue;

        printf("%d", message->len);
        rd_kafka_message_destroy(message);
    }
    rd_kafka_consume_stop(topic, partition);
    while (rd_kafka_outq_len(kafka) > 0)
        rd_kafka_poll(kafka, 10);

    rd_kafka_topic_destroy(topic);
    rd_kafka_destroy(kafka);
}

int main(int argc, char *argv[])
{
    produce("192.168.33.11:9092", "test11", "Hello kafka world");
    consume("192.168.33.11:9092", "test11");
}
