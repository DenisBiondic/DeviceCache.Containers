using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DeviceCache.Frontend.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "Works", "Works too!", "Works as well" };
        }
    }
}
