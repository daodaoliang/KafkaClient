using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host;
            string topicName;
            int count;
            if (args.Length < 3)
            {
                host = "192.168.33.12:9092";
                topicName = "test";
                count = 10000000;
            }
            else
            {
                host = args[0];
                topicName = args[1];
                count = int.Parse(args[2]);
            }

            var totalMessagesRead = 0L;
            var totalBytesRead = 0L;


        }
    }
}
