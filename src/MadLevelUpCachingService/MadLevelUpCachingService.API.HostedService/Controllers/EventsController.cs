using MadLevelUpCachingService.API.ScheduledService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MadLevelUpCachingService.API.HostedService.Controllers
{
    public class EventsController : Controller
    {
        // 
        // GET: /Events/

        public string Index()
        {
            return Worker.getCache();
        }

        // 
        // GET: /Events/
        public ActionResult Index(string id)
        {
            return this.Content(Worker.getCacheEntry(id), "application/json");
        }
    }
}
