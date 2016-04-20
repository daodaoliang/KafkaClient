namespace RdKafka
{
    namespace RdKafka
    {
        public enum ErrorCode
        {
            _BEGIN = -200,
            _BAD_MSG = -199,
            _BAD_COMPRESSION = -198,
            _DESTROY = -197,
            _FAIL = -196,
            _TRANSPORT = -195,
            _CRIT_SYS_RESOURCE = -194,
            _RESOLVE = -193,
            _MSG_TIMED_OUT = -192,
            _PARTITION_EOF = -191,
            _UNKNOWN_PARTITION = -190,
            _FS = -189,
            _UNKNOWN_TOPIC = -188,
            _ALL_BROKERS_DOWN = -187,
            _INVALID_ARG = -186,
            _TIMED_OUT = -185,
            _QUEUE_FULL = -184,
            _ISR_INSUFF = -183,
            _NODE_UPDATE = -182,
            _SSL = -181,
            _WAIT_COORD = -180,
            _UNKNOWN_GROUP = -179,
            _IN_PROGRESS = -178,
            _PREV_IN_PROGRESS = -177,
            _EXISTING_SUBSCRIPTION = -176,
            _ASSIGN_PARTITIONS = -175,
            _REVOKE_PARTITIONS = -174,
            _CONFLICT = -173,
            _STATE = -172,
            _UNKNOWN_PROTOCOL = -171,
            _NOT_IMPLEMENTED = -170,
            _AUTHENTICATION = -169,
            _NO_OFFSET = -168,
            _END = -100,

            Unknown = -1,
            NoError = 0,
            OffsetOutOfRange = 1,
            InvalidMsg = 2,
            UnknownTopicOrPart = 3,
            InvalidMsgSize = 4,
            LeaderNotAvailable = 5,
            NotLeaderForPartition = 6,
            RequestTimedOut = 7,
            BrokerNotAvailable = 8,
            ReplicaNotAvailable = 9,
            MsgSizeTooLarge = 10,
            StaleCtrlEpoch = 11,
            OffsetMetadataTooLarge = 12,
            NetworkException = 13,
            GroupLoadInProgress = 14,
            GroupCoordinatorNotAvailable = 15,
            NotCoordinatorForGroup = 16,
            TopicException = 17,
            RecordListTooLarge = 18,
            NotEnoughReplicas = 19,
            NotEnoughReplicasAfterAppend = 20,
            InvalidRequiredAcks = 21,
            IllegalGeneration = 22,
            InconsistentGroupProtocol = 23,
            InvalidGroupId = 24,
            UnknownMemberId = 25,
            InvalidSessionTimeout = 26,
            RebalanceInProgress = 27,
            InvalidCommitOffsetSize = 28,
            TopicAuthorizationFailed = 29,
            GroupAuthorizationFailed = 30,
            ClusterAuthorizationFailed = 31
        }
    }

    public enum RdKafkaType
    {
        Producer,
        Consumer
    }

    public enum MsgFlags
    {
        None = 0,
        MsgFree = 1,
        MsgCopy = 2
    }
}
