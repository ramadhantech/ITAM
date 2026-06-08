using ITAM.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITAM.Models
{
    public class InspectionReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReportNumber { get; set; }
            = string.Empty;

        [Required]
        public string Title { get; set; }
            = string.Empty;

        public DateTime InspectionDate { get; set; }

        /* =========================
           FOREIGN KEYS
        ========================= */

        // LOCATION
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }
            = null!;

        // CREATOR
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User Creator { get; set; }
            = null!;

        // ASSIGNED PIC
        public int? AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }

        // ICT APPROVER
        public int? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public User? Approver { get; set; }

        /* =========================
           REPORT INFO
        ========================= */

        public string ApprovalStatus { get; set; }
       = InspectionStatus.PendingUser;

        public DateTime? ApprovedAt { get; set; }

        // LOCATION APPROVAL
        public int? ApprovedByLocationUserId { get; set; }

        [ForeignKey("ApprovedByLocationUserId")]
        public User? ApprovedByLocationUser { get; set; }

        public DateTime? LocationApprovedAt { get; set; }

        public string? Notes { get; set; }

        public string? SignaturePath { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        /* =========================
           RELATION
        ========================= */

        public ICollection<InspectionDetail> InspectionDetails
        { get; set; }
            = new List<InspectionDetail>();
    }
}