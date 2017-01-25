using Autofac;
using Borg.Framework.Redis;
using Borg.Infra.BuildingBlocks;
using Borg.Infra.Caching;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Borg.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDepedencyCacheClient _cache;
        private readonly IBroadcaster _broadcaster;

        public HomeController(IDepedencyCacheClient cache, IBroadcaster broadcaster)
        {
            _cache = cache;
            _broadcaster = broadcaster;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var page = random.Next(1, 100);
            var records = _db().Skip(page - 1).Take(10);
            var model = new PagedResult<Mod>(records, page, 10, 1000);

            HttpContext.Session.SetString("name", "mitsbits");
            HttpContext.Session.SetInt32("answer", 42);
            var keys = model.Records.Select(x => x.Key).ToArray();
            var ddd = RandomString(12);
            await _cache.Add<Mod>(ddd, keys);

            var publisher = Startup.ApplicationContainer.ResolveNamed<IMessageBus>(CacheConstants.CACHE_DEPEDENCY_TOPIC);

            publisher.PublishAsync(new EntityCacheDepedencyEvictionEvent(typeof(Mod), keys.Take(4).ToArray()));

            return View(model);
        }

        public IActionResult SessionNameYears()
        {
            var name = HttpContext.Session.GetString("name");
            var yearsMember = HttpContext.Session.GetInt32("answer");

            return Content($"Name: \"{name}\",  Membership years: \"{yearsMember}\"");
        }

        public class Mod : IHavePartitionedKey
        {
            public int Id { get; set; }
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public PartitionedKey Key => new PartitionedKey(Id.ToString());
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static List<Mod> _db()
        {
            var result = new List<Mod>();
            for (int i = 1000 - 1; i >= 0; i--)
            {
                result.Add(new Mod()
                {
                    Id = i,
                    Identifier = Guid.NewGuid(),
                    Name = RandomString(random.Next(15))
                });
            }
            return result.OrderBy(x => x.Id).ToList();
        }
    }
}