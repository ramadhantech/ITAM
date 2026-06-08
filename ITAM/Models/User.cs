using System.ComponentModel.DataAnnotations;

namespace ITAM.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // NAVIGATION CLEAN (tetap boleh kalau masih dipakai)
        public ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();
        public ICollection<AssetHistory> History { get; set; } = new HashSet<AssetHistory>();

        public ICollection<InspectionReport> CreatedInspectionReport { get; set; }
            = new HashSet<InspectionReport>();

        public ICollection<InspectionReport> ApprovedInspectionReports { get; set; }
            = new HashSet<InspectionReport>();

    }
}
