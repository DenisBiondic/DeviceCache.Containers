using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace DeviceCache.Processor
{
    public class SimpleEventProcessor : IEventProcessor
    { 
        // very ugly code but gets the job done in proof of concept case
        private static readonly RedisManagerPool Pool = new RedisManagerPool("devicecache-cache:6379");
        
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var message = JsonConvert.DeserializeObject<Message>(data);
                map[message.Key] = message.Data;
            }

            using (var client = Pool.GetClient())
            {
                client.SetAll(map);
            }

            Console.WriteLine($"Processed { map.Count } at: { DateTime.Now }, Partition: '{context.PartitionId}'");

            return context.CheckpointAsync();
        }
    }
}