using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.Media.Models
{
    public class AssetNameViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }
    }
}