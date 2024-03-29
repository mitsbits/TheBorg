﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.GateKeeping.Account
{
    public class LoginInputModel
    {
        [Required]
        [Display(Name = "User")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}