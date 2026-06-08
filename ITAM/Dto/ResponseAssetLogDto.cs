namespace ITAM.Dto
{
    public class ResponseAssetLogDto
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = string.Empty;

        // Cukup ambil ID dan Nama Aset untuk UI tabel
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;

        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;

        public int ChangedBy { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }
    }
}
