using System.ComponentModel.DataAnnotations;

namespace ITAM.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public ICollection<User> Users { get; set; }
       = new HashSet<User>();
        public ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();
        public ICollection<InspectionReport> InspectReports { get; set; } = new HashSet<InspectionReport>();
    }
}
