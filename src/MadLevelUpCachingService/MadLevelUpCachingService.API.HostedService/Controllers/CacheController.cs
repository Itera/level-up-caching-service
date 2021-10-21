using MadLevelUpCachingService.API.ScheduledService;
using Microsoft.AspNetCore.Mvc;

namespace MadLevelUpCachingService.API.HostedService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        // 
        // GET: /Events/
        [HttpGet()]
        public string Index()
        {
            return Worker.getCache();
        }

        // 
        // GET: /Events/
        [HttpGet("{id}")]
        public ActionResult Index(string id)
        {
            return this.Content(Worker.getCacheEntry(id), "application/json");
        }
    }
}
