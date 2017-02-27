using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.Media.Models
{
    public class UploadAssetViewModel
    {
        [MaxLength(512)]
        public string Name { get; set; }

        [Required]
        public IFormFile File
        {
            get; set;
        }
    }
}