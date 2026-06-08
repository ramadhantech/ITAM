namespace ITAM.Models
{
    public class AssetHistory
    {
        public int Id {  get; set; }
        public string FieldName { get; set; } = string.Empty;
        public int AssetId { get; set; }
        public Asset Asset { get; set; } = null!;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public int ChangedBy { get; set; }
        public User ChangedByUser { get; set; } = null!;
        public DateTime ChangedAt { get; set; }
    }
}
