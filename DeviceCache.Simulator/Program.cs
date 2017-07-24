using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DeviceCache.Simulator
{
    public class Program
    {
        private static EventHubClient _client;

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var connectionString = configuration["EVENTHUB_SENDER_CONNECTION_STRING"];
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString);

            _client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            
            await SendBatchesToEventHub(100000, 1000);

            await _client.CloseAsync();
        }

        private static async Task SendBatchesToEventHub(int numBatchesToSend, int messagesPerBatch)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var random = new Random();

            for (var i = 0; i < numBatchesToSend; i++)
            {
                var batch = new List<EventData>();
                    
                for (int j = 0; j < messagesPerBatch; j++)
                {
                    var message = new Message
                    {
                        Key = random.Next(0, 100000).ToString(),
                        Data = $"Message {i}, Some big payload: {Guid.NewGuid()}, Time: {DateTime.Now}"
                    };
                    var eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
                    batch.Add(eventData);
                }

                Console.WriteLine($"Sending batch ({messagesPerBatch} items (actually: {batch.Count})): { i }");
                await _client.SendAsync(batch);

                if (i % 20 == 0 && i != 0)
                {
                    var totalMessages = 20 * messagesPerBatch;
                    var elapsedTotalSeconds = stopwatch.Elapsed.TotalSeconds;
                    Console.WriteLine($"{totalMessages} messages sent in {elapsedTotalSeconds} seconds. " +
                                      $"Msg per sec: {totalMessages / elapsedTotalSeconds}");
                    stopwatch.Restart();
                }
            }
        }
    }
}