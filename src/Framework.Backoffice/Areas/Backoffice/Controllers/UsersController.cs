using Borg.Framework.Backoffice.Identity.Models;
using Borg.Framework.Backoffice.Identity.Models.AccountViewModels;
using Borg.Framework.Backoffice.Identity.Queries;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    public class UsersController : BackofficeController
    {

        private readonly UserManager<BorgUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IBackofficeService<BorgSettings> systemService,  UserManager<BorgUser> userManager, RoleManager<IdentityRole> roleManager) : base(systemService)
        {
 
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
                u => u.Claims, u => u.Logins, u => u.Roles);
            var result = await Backoffice.Fetch(query);

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

        [Route("[area]/users/new")]
        public async Task<IActionResult> NewUser()
        {
            PageContent(new PageContent()
            {
                Title = "Create user"
            });
            var model = new CreateUserViewModel
            {
                Roles = (await _roleManager.Roles.ToArrayAsync()).Select(
                    x => new CreateUserViewModel.RoleOption() { Name = x.Name, Selected = false }).ToArray()
            };
            return View(model);
        }

        [Route("[area]/users/new")]
        [HttpPost]
        public async Task<IActionResult> NewUser(CreateUserViewModel model)
        {
            model.ConfirmPassword = model.Password;
            ModelState.Clear();

            if (TryValidateModel(model))
            {
                var user = new BorgUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (model.EnableOnCreate)
                    {
                        result = await _userManager.SetLockoutEnabledAsync(user, false);
                        if (result.Succeeded)
                        {
                            result = await _userManager.AddToRolesAsync(user,
                                model.Roles.Where(x => x.Selected).Select(x => x.Name));
                            if (result.Succeeded)
                            {
                                Logger.LogInformation(3, "User created a new account with password.");
                                return RedirectToAction("Index", new { q = user.Email });
                            }
                            AddErrors(result);
                        }
                        AddErrors(result);
                    }
                    else
                    {
                        result = await _userManager.AddToRolesAsync(user,
                                model.Roles.Where(x => x.Selected).Select(x => x.Name));
                        if (result.Succeeded)
                        {
                            Logger.LogInformation(3, "User created a new account with password.");
                            return RedirectToAction("Index", new { q = user.Email });
                        }
                        AddErrors(result);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}