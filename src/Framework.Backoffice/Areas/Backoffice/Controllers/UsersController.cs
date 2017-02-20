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

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    public class UsersController : BackofficeController
    {
        private readonly ICommandBus _bus;
        private readonly IQueryBus _queries;

        public UsersController(ISystemService<BorgSettings> systemService, ICommandBus bus, IQueryBus queries) : base(systemService)
        {
            _bus = bus;
            _queries = queries;
        }

        public async Task<IActionResult> Index()
        {
            PageContent(new PageContent()
            {
                Title = "Users"
            });

            var q = new UsersQueryRequest(x => x.UserName.StartsWith("m"), Pager.Current, Pager.RowCount,
                new[] { new OrderByInfo<BorgUser>() { Ascending = false, Property = u => u.Email } }, u => u.Claims,
                u => u.Logins);
            var result = await _queries.Fetch<UsersQueryRequest>(q);

            return View(result);
        }
    }
}