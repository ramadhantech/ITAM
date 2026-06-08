namespace ITAM.Dto
{
    public class AssetLookupDto
    {
        public int Id { get; set; }
        public string AssetName { get; set; }
        public string AssetTag { get; set; }
        public int CategoryId { get; set; }
        public string AssetSubtype { get; set; }
    }
}
