namespace ITAM.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty ;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string AssetType { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;
        public string Condition {  get; set; } = string.Empty;
        public string Note {  get; set; } = string.Empty;
        public string AssetSubtype { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;    
        public DateTime DeletedAt { get; set; }
        public ICollection<AssetHistory> History { get; set; } = new HashSet<AssetHistory>();
        public ICollection<InspectionReport> InspectionReports { get; set; } = new HashSet<InspectionReport>();
    }
}
