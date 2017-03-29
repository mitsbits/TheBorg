using Borg.Framework.GateKeeping.Commands;
using Borg.Framework.GateKeeping.Models;
using Borg.Framework.GateKeeping.Models.AccountViewModels;
using Borg.Framework.GateKeeping.Models.UserViewModels;
using Borg.Framework.GateKeeping.Queries;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Framework.GateKeeping
{
    [Area("backoffice")]
    public class UsersController : BackofficeController
    {
        private readonly UserManager<BorgUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IBackofficeService<BorgSettings> systemService, UserManager<BorgUser> userManager, RoleManager<IdentityRole> roleManager) : base(systemService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Route("[area]/users")]
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

        [Route("[area]/users/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) throw new FileNotFoundException(nameof(BorgUser));

            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var userClaim in userClaims)
            {
                user.Claims.Add(new IdentityUserClaim<string>() { ClaimType = userClaim.Type, ClaimValue = userClaim.Value });
            }

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
                //Roles = (await _roleManager.Roles.ToArrayAsync()).Select(
                //    x => new CreateUserViewModel.RoleOption() { Name = x.Name, Selected = false }).ToArray()
                Roles = System.BorgHost.IdentityDescriptor().RoleNames.Select(x => new CreateUserViewModel.RoleOption() { Name = x, Selected = false }).ToArray()
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
                    var selectedRoles = await EnsureRolesExistInDb(model);
                    if (model.EnableOnCreate)
                    {
                        result = await _userManager.SetLockoutEnabledAsync(user, false);
                        if (result.Succeeded)
                        {
                            result = await _userManager.AddToRolesAsync(user, selectedRoles);
                            if (result.Succeeded)
                            {
                                Logger.LogInformation(3, "{@User} created a new account with password.", user);
                                return RedirectToAction("Index", new { q = user.Email });
                            }
                            AddErrors(result);
                        }
                        AddErrors(result);
                    }
                    else
                    {
                        result = await _userManager.AddToRolesAsync(user, selectedRoles);
                        if (result.Succeeded)
                        {
                            Logger.LogInformation(3, "{@User} created a new account with password.", user);
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

        [HttpPost]
        [Route("[area]/users/Avatar")]
        public async Task<IActionResult> Avatar(UserAvatarViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Backoffice.Process(new UserAvatarCommand(model));
            }
            return RedirectToAction("Details", new { id = model.UserId });
        }

        private async Task<string[]> EnsureRolesExistInDb(CreateUserViewModel model)
        {
            var selectedRoles = model.Roles.Where(x => x.Selected).Select(x => x.Name).ToArray();
            foreach (var role in selectedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var systemClaims = System.BorgHost.IdentityDescriptor().Claims(role);
                    var dbRole = new IdentityRole(role);
                    foreach (var systemClaim in systemClaims)
                    {
                        dbRole.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = systemClaim.Type, ClaimValue = systemClaim.Value });
                    }
                    await _roleManager.CreateAsync(dbRole);
                }
            }
            return selectedRoles;
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