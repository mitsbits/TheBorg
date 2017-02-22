using System;
using System.IO;
using System.Linq.Expressions;
using Borg.Framework.Backoffice.Pages.Commands;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Borg.Framework.Backoffice.Identity.Models;
using Borg.Framework.Backoffice.Identity.Queries;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    public class UsersController : BackofficeController
    {
        private readonly ICommandBus _bus;
        private readonly IQueryBus _queries;
        private readonly UserManager<BorgUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ISystemService<BorgSettings> systemService, ICommandBus bus, IQueryBus queries, UserManager<BorgUser> userManager, RoleManager<IdentityRole> roleManager) : base(systemService)
        {
            _bus = bus;
            _queries = queries;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string q)
        {
            var page = new PageContent()
            {
                Title = "Users"
            };
            

            Expression<Func<BorgUser, bool>> where = x => true;
            if (!string.IsNullOrWhiteSpace(q))
            {
                where = x => x.Email.Contains(q.TrimStart().TrimEnd()) || x.UserName.Contains(q.TrimStart().TrimEnd());
                page.Subtitle = $"filter: {q.TrimStart().TrimEnd()}";
            }

            var query = new UsersQueryRequest(where, Pager.Current, Pager.RowCount,
                new[] { new OrderByInfo<BorgUser>() { Ascending = true, Property = u => u.Email } }, 
                u => u.Claims, u => u.Logins, u=>u.Roles);
            var result = await _queries.Fetch(query);

            PageContent(page);
            return View(result);
        }
        [Route("[area]/user/{id}")]
        public async Task<IActionResult> Details(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) throw new FileNotFoundException(nameof(BorgUser));

            var page = new PageContent()
            {
                Title = user.UserName,
                Subtitle = user.Email
            };


            PageContent(page);
            return View(user);
        }
    }
}