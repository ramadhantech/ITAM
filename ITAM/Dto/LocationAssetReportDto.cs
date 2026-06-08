namespace ITAM.Dto
{
    public class LocationAssetReportDto
    {
        public string LocationName { get; set; }
        public int TotalAsset { get; set; }

        public List<AssetReportItemDto> Assets { get; set; }
    }

    public class AssetReportItemDto
    {
        public string AssetTag { get; set; }
        public string AssetName { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Condition { get; set; }
    }
}
