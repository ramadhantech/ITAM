namespace ITAM.Dto
{
    public class CreateInspectionReportDto
    {
        public string Title { get; set; } = string.Empty;

        public DateTime InspectionDate { get; set; }

        public int LocationId { get; set; }
        public int? AssignedToUserId { get; set; }

        public string? Notes { get; set; }

        public List<CreateInspectionDetailDto> Details { get; set; }
           = new();
    }
}
