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
            var eventHubPath = configuration["EVENTHUB_READER_PATH"];
            var storageAccountName = configuration["STORAGE_ACCOUNT_NAME"];
            var storageAccountKey = configuration["STORAGE_ACCOUNT_KEY"];

            var storage = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};" +
                          $"AccountKey={storageAccountKey};EndpointSuffix=core.windows.net";
            
            var eventProcessorHost = new EventProcessorHost(
                eventHubPath,
                PartitionReceiver.DefaultConsumerGroupName,
                connectionString,
                storage,
                "hostsync");

            // Registers the Event Processor Host and starts receiving messages
            var eventProcessorOptions = EventProcessorOptions.DefaultOptions;
            eventProcessorOptions.MaxBatchSize = 2000;
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(eventProcessorOptions);
            await Task.Delay(Timeout.Infinite);
        }
    }
}