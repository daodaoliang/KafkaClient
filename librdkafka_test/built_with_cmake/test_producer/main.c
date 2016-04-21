#include <string.h>
#include "rdkafka.h"
#include <stdlib.h>
#include <stdbool.h>
#include <stdio.h>
#include <time.h>

long get_seconds()
{
    time_t t = time(0);
    return t;
}
int i = 0;
void f(rd_kafka_t *rk, const rd_kafka_message_t *rkmessage, void *opaque)
{
    if (rkmessage->_private != NULL)
        ((void( *)())rkmessage->_private)();
    printf("%d: %d\n", i++, rkmessage->len);
}

void f1()
{
    printf("Opaque");
}

int main(int argc, char *argv[])
{
    
    char *host;
    char *topic_name;
    int count;
    int size;
    if (argc < 5)
    {
        host = "192.168.33.12:9092";
        topic_name = "test2";
        count = 100;
        size = 100;
    }
    else
    {
        host = argv[1];
        topic_name = argv[2];
        count = atoi(argv[3]);
        size = atoi(argv[4]);
    }
    
    char *payload = (char *)malloc(size * sizeof(char));
    for (int i = 0; i < size; i++)
        payload[i] = 'A';


    int partition = RD_KAFKA_PARTITION_UA;
    int errsize = 512;
    char *errstr = calloc(errsize, sizeof(char));

    rd_kafka_conf_t *conf = rd_kafka_conf_new();
    

    rd_kafka_conf_set_dr_msg_cb(conf, f);

    rd_kafka_conf_set(conf, "batch.num.messages", "100", errstr, errsize);
    printf("batch.num.messages:error: %s\n", errstr);
    
    rd_kafka_conf_set(conf, "queue.buffering.max.ms", "100", errstr, errsize);
    printf("queue.buffering.max.ms: error: %s\n", errstr);
    
    rd_kafka_conf_set(conf, "queue.buffering.max.messages", "10000000", errstr, errsize);
    printf("queue.buffering.max.messages:error: %s\n", errstr);

    rd_kafka_t *kafka = rd_kafka_new(RD_KAFKA_PRODUCER, conf, errstr, errsize);
    rd_kafka_brokers_add(kafka, host);
    rd_kafka_topic_conf_t *topic_conf = rd_kafka_topic_conf_new();
    rd_kafka_topic_t *topic = rd_kafka_topic_new(kafka, topic_name, topic_conf);
    printf("%s, %d\n", payload, size);
    long t = get_seconds();
    int sent = 0;
    int enobufs = 0;
    while (true)
    {
        if (rd_kafka_produce(topic, partition, 0, payload, size, NULL, 0, f1) == -1)
        {
            enobufs++;
            //printf("Failed to produce to topic %s partition %d: %d %s\n", topic_name, partition, errno, rd_kafka_err2str(rd_kafka_last_error()));
            rd_kafka_poll(kafka, 10);
            continue;
        }

        sent++;
        if (sent >= count)
            break;

        rd_kafka_poll(kafka, 0);
    }
    long elapsed = get_seconds() - t;

    printf("enobufs: %d\n", enobufs);
    printf("%f\n", (float)count/elapsed);

    rd_kafka_poll(kafka, 0);

    while (rd_kafka_outq_len(kafka) > 0)
        rd_kafka_poll(kafka, 100);

    rd_kafka_topic_destroy(topic);
    rd_kafka_destroy(kafka);
}
