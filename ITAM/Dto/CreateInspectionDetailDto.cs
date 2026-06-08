namespace ITAM.Dto
{
    public class CreateInspectionDetailDto
    {
        public int AssetId { get; set; }

        public string Condition { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}
