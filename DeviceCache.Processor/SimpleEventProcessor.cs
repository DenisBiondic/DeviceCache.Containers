using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DeviceCache.Processor
{
    public class SimpleEventProcessor : IEventProcessor
    { 
        // very ugly code but gets the job done in proof of concept case
        private static ConnectionMultiplexer _multiplexer;
        
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            ConfigurationOptions config = ConfigurationOptions.Parse("devicecache-cache:6379");

            DnsEndPoint addressEndpoint = config.EndPoints.First() as DnsEndPoint;
            int port = addressEndpoint.Port;

            IPHostEntry ip = Dns.GetHostEntryAsync(addressEndpoint.Host).Result;
            config.EndPoints.Remove(addressEndpoint);
            config.EndPoints.Add(ip.AddressList.First(), port);
            
            _multiplexer = ConnectionMultiplexer.Connect(config);
            Console.WriteLine($"SimpleEventProcessor initialized. Redis connected. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Dictionary<RedisKey, RedisValue> map = new Dictionary<RedisKey, RedisValue>();
            
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var message = JsonConvert.DeserializeObject<Message>(data);
                map[message.Key] = message.Data;
            }

            var client = _multiplexer.GetDatabase();
            client.StringSet(map.ToArray());
            
            Console.WriteLine($"Processed { map.Count } at: { DateTime.Now }, Partition: '{context.PartitionId}'");

            return context.CheckpointAsync();
        }
    }
}