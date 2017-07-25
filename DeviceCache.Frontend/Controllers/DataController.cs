using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;

namespace DeviceCache.Frontend.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly RedisManagerPool _manager;

        public DataController(RedisManagerPool manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            var random = new Random();
            var keys = new List<string>();

            for (int i = 0; i < 40; i++)
            {
                var randomKey = random.Next(0, 100000);
                keys.Add(randomKey.ToString());
            }

            using (var client = _manager.GetClient())
            {
                var values = client.GetValues(keys);
                return values;
            }
        }
    }
}
