using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.GateKeeping.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class CreateUserViewModel : RegisterViewModel
    {
        [Display(Name = "Roles")]
        public RoleOption[] Roles { get; set; }

        [Display(Name = "Enable")]
        public bool EnableOnCreate { get; set; } = true;

        public class RoleOption
        {
            public string Name { get; set; }
            public bool Selected { get; set; }
        }
    }
}