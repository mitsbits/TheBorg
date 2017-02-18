using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.Backoffice.Identity.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}