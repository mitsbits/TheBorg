using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.Media.Models
{
    public class NewVersionViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public IFormFile File
        {
            get; set;
        }
    }

    public class RestoreVersionViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int Version
        {
            get; set;
        }
    }
}