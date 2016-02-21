using System;
using System.Linq;
using System.Threading.Tasks;
using KafkaClient;
using log4net;
using Newtonsoft.Json;

namespace Publisher
{
    public class Program
    {
        public static readonly ILog Log = LogManager.GetLogger("Publisher");

        public static int ThreadCount = 4;
        public static int TaskSize = 1024;
        public static int TasksToSend = 1024;

        public static string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public static Random Random = new Random();

        public static XTask[] GenerateTasks(int count)
        {
            
            var tasks = new XTask[count];
            for (int i = 0; i < count; i++)
            {
                var description = new char[TaskSize];
                for (int j = 0; j < TaskSize; j++)
                    description[j] = Alphabet[Random.Next(Alphabet.Length)];

                tasks[i] = new XTask
                {
                    Name = $"Task #{i}",
                    Description = new string(description),
                    Timestamp = DateTime.Now
                };
            }
            return tasks;
        }

        public static void Publish(XTask[] tasks)
        {
            var contentType = "application/vnd.kafka.json.v1+json";
            var accept = "application/vnd.kafka.v1+json, application/vnd.kafka+json, application/json";
            var data = JsonConvert.SerializeObject(new { records = tasks.Select(x => new { value = x }) });

            Log.Info(WebRequestHelper.Post(Config.HostName, $"topics/{Config.TopicName}", data, contentType, accept));
        }
        
        public static void Main(string[] args)
        {
            var tasksPerThread = TasksToSend/ThreadCount;
            var tasks = new Task[ThreadCount];
            for (int i = 0; i < tasks.Length; i++)
                tasks[i] = Task.Run(() => Publish(GenerateTasks(tasksPerThread)));
            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException e)
            {
                Log.Error(e.InnerException.Message);
            }
        }
    }
}
