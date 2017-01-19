using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Borg.Infra.Relational;
using Infra.Core.Caching;
using Infra.Core.Relational;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Borg.Client.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var page = random.Next(1,100);
            var records = _db().Skip(page - 1).Take(10);
            var model = new PagedResult<Mod>(records, page, 10, 1000);

            return View(model);
        }



        public class Mod
        {
            public int Id { get; set; }
            public Guid Identifier { get; set; }
            public string Name { get; set; }
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
                    Id =  i,
                    Identifier = Guid.NewGuid(),
                    Name = RandomString(random.Next(15))
                });
            }
            return result.OrderBy(x => x.Id).ToList();
        } 
    }
}
