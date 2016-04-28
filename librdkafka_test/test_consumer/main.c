#include "rdkafka.h"
#include <stdlib.h>
#include <time.h>
#include <stdbool.h>

int recv = 0;
int bytes = 0;

long get_seconds()
{
    time_t t = time(0);
    return t;
}

void msg_consume(rd_kafka_message_t *message, void *opaque)
{
    if (!message) {
        printf("Got null message: %s\n", rd_kafka_err2str(rd_kafka_last_error()));
        return;
    }
    recv++;
    bytes += message->len;
    rd_kafka_message_destroy(message);
}

void consume(const char *host, const char *topic_name, int count)
{

    int partition = 0;
    int64_t start_offset = 0;
    char errstr[512];
    rd_kafka_conf_t *conf = rd_kafka_conf_new();
    
    rd_kafka_conf_set(conf, "auto.commit.enable", "false", errstr, sizeof(errstr));
    rd_kafka_conf_set(conf, "fetch.message.max.bytes", "10000000", errstr, sizeof(errstr));
    rd_kafka_conf_set(conf, "fetch.message.min.bytes", "1000000", errstr, sizeof(errstr));
    rd_kafka_conf_set(conf, "queued.min.messages", "1000000", errstr, sizeof(errstr));
    rd_kafka_t *kafka = rd_kafka_new(RD_KAFKA_CONSUMER, conf, errstr, sizeof(errstr));
    //rd_kafka_set_log_level(kafka, LOG_DEBUG);
    rd_kafka_brokers_add(kafka, host);
    rd_kafka_topic_conf_t *topic_conf = rd_kafka_topic_conf_new();
    rd_kafka_topic_t *topic = rd_kafka_topic_new(kafka, topic_name, topic_conf);
    

    rd_kafka_metadata_t *metadata;
    int err = rd_kafka_metadata(kafka, false, topic, &metadata, 5000);
    if (err != RD_KAFKA_RESP_ERR_NO_ERROR)
    {
        printf("Failed to aquire metadata :%s\n", rd_kafka_err2str(err));
        return;
    }
    int partition_cnt = metadata->topics[0].partition_cnt;
    for (int i = 0; i < partition_cnt; i++) {
        if (rd_kafka_consume_start(topic, i, start_offset) == -1) {
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
    }

    int batch_size = 10000;
    rd_kafka_message_t **rkmessages = malloc(sizeof(rd_kafka_message_t *) * batch_size);
    
    int s = get_seconds();


    while (recv <count)
    {
        //for (int i = 0; i < partition_cnt; i++) {
            rd_kafka_poll(kafka, 0);

            //rd_kafka_consume_callback(topic, i, 1000, msg_consume, NULL);

            int r = rd_kafka_consume_batch(topic, partition, 1000, rkmessages, batch_size);
            if (r > 0)
            {
                //printf("Got %d msgs\n", r);
                for (int j = 0; j < r; j++)
                    msg_consume(rkmessages[j], NULL);
            }
            else
            {
                //printf("Got null: %s\n", rd_kafka_err2str(rd_kafka_last_error()));
            }

            //rd_kafka_message_t *message = rd_kafka_consume(topic, partition, 100);
            //if (!message) {
            //    //printf("Got null: %s\n", rd_kafka_err2str(rd_kafka_last_error()));
            //    //rd_kafka_poll(kafka, 10);
            //    continue;
            //}
            //recv++;
            //bytes += message->len;
            ////printf("%d: %d\n", recv, message->len);
            //
            //rd_kafka_message_destroy(message);
        //}
    }
    
    float mb = bytes / 1024.0 / 1024;
    s = get_seconds() - s;

    printf("%f MB/sec\n", (float)mb / s);

    rd_kafka_consume_stop(topic, partition);
    while (rd_kafka_outq_len(kafka) > 0)
        rd_kafka_poll(kafka, 10);

    rd_kafka_topic_destroy(topic);
    rd_kafka_destroy(kafka);
}

int main(int argc, char *argv[])
{
    char *host;
    char *topic_name;
    int count;
    if (argc < 4)
    {
        host = "192.168.33.12:9092";
        topic_name = "test2";
        count = 1000 * 1000;
    }
    else
    {
        host = argv[1];
        topic_name = argv[2];
        count = atoi(argv[3]);
    }
    consume(host, topic_name, count);
}
