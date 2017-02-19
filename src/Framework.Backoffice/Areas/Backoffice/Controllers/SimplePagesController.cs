using System.Threading.Tasks;
using Borg.Framework.Backoffice.Pages.Data;
using Borg.Framework.MVC;
using Borg.Framework.System;
using Borg.Infra.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Authorize][Area("backoffice")]
    public class SimplePagesController : ComponentController<SimplePage>
    {
        public SimplePagesController(ISystemService<BorgSettings> systemService, IDbContextScopeFactory scopeFactory) : base(systemService, scopeFactory)
        {
        }


    }
}