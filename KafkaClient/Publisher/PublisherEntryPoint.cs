using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KafkaClient;
using log4net;
using Newtonsoft.Json;
using KafkaClient.Logging;

namespace Publisher
{
    public class PublisherEntryPoint
    {
        public static readonly ILog Log = LogManager.GetLogger("Publisher");

        public static int TaskNumber = 0;
        public static object TaskNumberLock = new object();

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
                    Description = new string(description)
                };

                lock (TaskNumberLock)
                {
                    tasks[i].Name = $"Task #{TaskNumber++}";
                }
            }
            
            for (int i = 0; i < count; i++)
                tasks[i].Timestamp = DateTime.UtcNow;

            return tasks;
        }

        public static void Publish(XTask[] tasks)
        {
            Thread.Sleep(10);
            var contentType = "application/vnd.kafka.json.v1+json";
            var accept = "application/vnd.kafka.v1+json, application/vnd.kafka+json, application/json";
            var data = JsonConvert.SerializeObject(new { records = tasks.Select(x => new { value = x }) });

            try
            {
                WebRequestHelper.Post(Config.HostName, $"topics/{Config.TopicName}", data, contentType, accept);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        
        public static void Main(string[] args)
        {
            Log4NetConfigurator.Configure();

            var tasksPerThread = TasksToSend/ThreadCount;
            var tasks = new Task[ThreadCount];

            var time = DateTime.UtcNow;
            while (DateTime.UtcNow.Subtract(time).TotalMinutes < 5)
            {
                var elapsed = DateTime.UtcNow.Subtract(time).TotalSeconds;
                if ((int) (elapsed*1000)%250 == 0)
                {
                    Log.Info($"Time elapsed: {elapsed}. Messages sent: {TaskNumber}.");
                }

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
}
