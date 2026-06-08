using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITAM.Models
{
    public class InspectionDetail
    {
        [Key]
        public int Id { get; set; }

        /* =========================
           FOREIGN KEYS
        ========================= */

        public int ReportId { get; set; }

        [ForeignKey("ReportId")]
        public InspectionReport Report { get; set; } = null!;
        public int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        /* =========================
           INSPECTION DATA
        ========================= */

        public string Condition { get; set; } = "Good";

        public string Status { get; set; } = "Active";

        public bool Checked { get; set; } = true;

        public string? Notes { get; set; }

        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}
