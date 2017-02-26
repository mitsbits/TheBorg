using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Borg.Framework.Backoffice.Assets.Models
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

    public class AssetNameViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required][MaxLength(512)]
        public string Name { get; set; }

    }
}
