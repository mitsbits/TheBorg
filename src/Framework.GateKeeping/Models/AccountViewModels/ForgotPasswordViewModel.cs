using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.GateKeeping.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}