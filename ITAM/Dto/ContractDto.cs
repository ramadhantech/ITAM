namespace ITAM.Dto
{
    public class ContractDto
    {
        public int Id { get; set; }

        public string ContractNumber { get; set; } = string.Empty;

        public int VendorId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
