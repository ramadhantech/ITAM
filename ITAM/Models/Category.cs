using System.ComponentModel.DataAnnotations;

namespace ITAM.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public  ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();
    }
}
