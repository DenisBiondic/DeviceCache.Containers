using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace DeviceCache.Frontend.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly ConnectionMultiplexer _redis;

        public DataController(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var random = new Random();
            var keys = new List<RedisKey>();

            for (int i = 0; i < 40; i++)
            {
                var randomKey = random.Next(0, 100000);
                keys.Add(randomKey.ToString());
            }

            IDatabase db = _redis.GetDatabase();
            
            var values = db.StringGet(keys.ToArray());
            return values.ToStringArray();
        }
    }
}
