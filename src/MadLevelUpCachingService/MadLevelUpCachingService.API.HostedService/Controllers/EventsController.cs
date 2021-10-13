using System;
using System.Collections.Generic;
using System.Linq;
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
            return "TODO: Get data from CMS";
        }

        // 
        // GET: /Events/
        public string Index(string id)
        {
            return "TODO: Get data from CMS";
        }

        // 
        // POST: /Events/
        [HttpPost]
        public string Post(string id)
        {
            return "TODO: Get data from CMS";
        }

        // 
        // PUT: /Events/
        [HttpPut]
        public string Put(string id)
        {
            return "TODO: Get data from CMS";
        }
    }
}
