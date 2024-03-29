﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Borg.Framework.GateKeeping.Consent;
using Borg.Framework.MVC;
using Borg.Framework.System;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.GateKeeping
{
    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    //[SecurityHeaders]
    [Area("backoffice")]
    public class ConsentController : BackofficeController
    {
        private readonly ConsentService _consent;

        public ConsentController(
            IBackofficeService<BorgSettings> system,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore) :base(system)
        {
            _consent = new ConsentService(interaction, clientStore, resourceStore, system.CreateLogger<ConsentController>());
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _consent.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await _consent.ProcessConsent(model);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }
    }
}