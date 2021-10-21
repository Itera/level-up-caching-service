using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MadLevelUpCachingService.API.ScheduledService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _projectId;
        private readonly string _dataset;
        private static string _cacheRaw;
        private static List<string> _cache;
        //private static Node _treeCache;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _projectId = "y2nkns2q";
            _dataset = "production";
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int overrideWaitInterval = -1;
            while (!stoppingToken.IsCancellationRequested)
            {
                var minuteInterval = _configuration.GetValue<int>("MinuteInterval");
                await Task.Delay(
                    overrideWaitInterval >= 0 ? 
                      TimeSpan.FromMinutes(minuteInterval) 
                    : TimeSpan.FromMinutes(overrideWaitInterval), 
                    stoppingToken);

                using var client = new HttpClient();

                var response = await client.GetAsync($"https://{_projectId}.api.sanity.io/v2021-06-07/data/export/{_dataset}");

                if(response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully retrieved data for project: {_projectId}, dataset: {_dataset}.");
                    overrideWaitInterval = -1;

                    _cacheRaw = await response.Content.ReadAsStringAsync();
                    _cache = _cacheRaw.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    _logger.LogInformation($"Failed to retrieve data for project: {_projectId}, dataset: {_dataset}.");
                    overrideWaitInterval = 5;
                }
            }
        }

        public static string getCache()
        {
            return _cacheRaw;
        }

        public static string getCacheEntry(string id)
        {
            var metaData = GetMetaData(_cache);
            var matches = metaData.Where(x => x.Id == id);
            if (matches.Any())
            {
                return _cache[matches.First().LineNr];
            }
            else
            {
                return null;
            }
        }

        public static List<MetaData> GetMetaData(List<string> strings)
        {
            var ret = new List<MetaData>();
            for (int i = 0; i < strings.Count(); i++)
            {
                ret.Add(GetMetaData(strings[i], i));
            }
            return ret;
        }

        public static MetaData GetMetaData(string str, int lineNr)
        {
            var meta = new MetaData();
            string currStr = "";
            string name = "";

            bool strStarted = false;
            foreach (var c in str.ToCharArray())
            {
                if (c == '"')
                {
                    strStarted = !strStarted;

                    if (strStarted)
                    {
                        currStr = "";
                    }
                    else
                    {
                        if (name == "")
                        {
                            name = currStr;
                        }
                        else
                        {
                            if(meta.CreatedAt == null && name == "_createdAt")
                            {
                                meta.CreatedAt = currStr;
                            }
                            else if (meta.Id == null && name == "_id")
                            {
                                meta.Id = currStr;
                            }
                            else if (meta.Rev == null && name == "_rev")
                            {
                                meta.Rev = currStr;
                            }
                            else if (meta.Type == null && name == "_type")
                            {
                                meta.Type = currStr;
                            }
                            else if (meta.UpdatedAt == null && name == "_updatedAt")
                            {
                                meta.UpdatedAt = currStr;
                            }

                            name = "";
                        }
                    }
                }
                else if (!strStarted)
                {
                    // Do nothing inbetween strings and lists.
                }
                else
                {
                    currStr += c;
                }
            }

            return meta;
        }

        public class MetaData
        {
            public string Id { get; set; } = null;
            public string CreatedAt { get; set; } = null;
            public string Rev { get; set; } = null;
            public string Type { get; set; } = null;
            public string UpdatedAt { get; set; } = null;
            public int LineNr { get; set; }
        }

        /*
        public static List<Node> SplitStringByCurlyNested(List<string> strings)
        {
            var ret = new List<Node>();
            foreach (var str in strings)
            {
                ret.Add(SplitStringByCurlyNested(str));
            }
            return ret;
        }

        public static Node SplitStringByCurlyNested(string str)
        {
            Node ret = null;
            Node cursor = null;
            string currStr = "";
            string name = "";

            bool strStarted = false;
            foreach (var c in str.ToCharArray())
            {
                if (c == '{')
                {
                }
                else if (c == '}')
                {
                }
                else if (c == '"')
                {
                    strStarted = !strStarted;
                    if (strStarted)
                    {
                        currStr = "";
                    }
                    else
                    {
                        if(name == "")
                        {
                            name = currStr;
                        }
                        else
                        {
                            if(ret == null)
                            {
                                cursor = new Node(name, currStr);
                                ret = cursor;
                            }
                            else
                            {
                                cursor = new Node(name, currStr, cursor);
                            }

                            name = "";
                        }
                    }
                }
                else if (!strStarted && c == '[')
                {
                    ;
                }
                else if (!strStarted)
                {
                    // Do nothing inbetween strings and lists.
                }
                else
                {

                }
            }

            return ret;
        }
        */
    }
}
