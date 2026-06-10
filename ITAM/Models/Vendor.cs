namespace ITAM.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public ICollection<Contract> Contracts { get; set; } = new HashSet<Contract>();
    }
}
