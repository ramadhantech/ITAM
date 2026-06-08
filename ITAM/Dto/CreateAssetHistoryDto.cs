namespace ITAM.Dto
{
    public class CreateAssetHistoryDto
    {
        public string FieldName { get; set; } = string.Empty;
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public int ChangedBy { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;
    }
}
