using Microsoft.AspNetCore.Http;

namespace Borg.Framework.GateKeeping.Models.UserViewModels
{
    public class UserAvatarViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatarUrl { get; set; }
        
        public IFormFile File {  get; set;  }
    }
}