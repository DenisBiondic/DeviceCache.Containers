using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;

namespace DeviceCache.Processor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var connectionString = configuration["EVENTHUB_READER_CONNECTION_STRING"];
            int entityPathIndex = connectionString.IndexOf("EntityPath=", StringComparison.Ordinal);
            connectionString = connectionString.Substring(0, entityPathIndex);

            var eventHubPath = configuration["EVENTHUB_READER_PATH"];
            var storageAccountName = configuration["STORAGE_ACCOUNT_NAME"];
            var storageAccountKey = configuration["STORAGE_ACCOUNT_KEY"];

            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};" +
                          $"AccountKey={storageAccountKey};EndpointSuffix=core.windows.net";
            
            Console.WriteLine($"Connecting with connString: { connectionString }");
            Console.WriteLine($"Connecting with eventHubPath: { eventHubPath }");
            Console.WriteLine($"Connecting with storageAccName: { storageAccountName }");
            Console.WriteLine($"Connecting with storageAccKey: { storageAccountKey }");
            Console.WriteLine($"Connecting with storageConnString: { storageConnectionString }");

            var eventProcessorHost = new EventProcessorHost(
                eventHubPath,
                PartitionReceiver.DefaultConsumerGroupName,
                connectionString,
                storageConnectionString,
                "hostsync");

            // Registers the Event Processor Host and starts receiving messages
            var eventProcessorOptions = EventProcessorOptions.DefaultOptions;
            eventProcessorOptions.MaxBatchSize = 2000;
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(eventProcessorOptions);
            await Task.Delay(Timeout.Infinite);
        }
    }
}