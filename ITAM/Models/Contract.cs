using NuGet.ContentModel;

namespace ITAM.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public ICollection<Asset> Assets { get; set; } = new HashSet<Asset>();
    }
}
