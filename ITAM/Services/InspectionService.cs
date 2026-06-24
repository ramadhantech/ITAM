using ITAM.Constants;
using ITAM.Data;
using ITAM.Dto;
using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Services
{
    public class InspectionService
    {
        private readonly ItamDbContext _context;

        public InspectionService(ItamDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<InspectionReport>> GetAllAsync(int currentUserId)
        {
            return await _context.InspectionReport
      .AsNoTracking()
      .Include(x => x.Location)
      .Include(x => x.Creator)
          .ThenInclude(x => x.Department)
      .Include(x => x.AssignedToUser)
      .Include(x => x.Approver)
      .Where(x => x.CreatedBy == currentUserId)
      .OrderByDescending(x => x.CreatedAt)
      .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<InspectionReport?> GetByIdAsync(int id)
        {
            return await _context.InspectionReport
                .AsSplitQuery()
                .Include(x => x.Location)
                .Include(x => x.Creator)
                .Include(x => x.AssignedToUser)
                .Include(x => x.Approver)


                // INI YANG WAJIB TAMBAH
                .Include(x => x.InspectionDetails)
                    .ThenInclude(x => x.Asset)
                        .ThenInclude(x => x.User)

                .Include(x => x.InspectionDetails)
                    .ThenInclude(x => x.Asset)
                        .ThenInclude(x => x.Category)

                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // =========================
        // CREATE
        // =========================
        public async Task<InspectionReport> CreateAsync(CreateInspectionReportDto dto, int createdBy)
        {

            if (dto.Details == null || !dto.Details.Any())
                throw new Exception("Detail tidak boleh kosong");

            var mor3 = await _context.Locations
      .FirstOrDefaultAsync(x => x.Name.Trim().ToUpper() == "MOR III");

            if (mor3 == null)
                throw new Exception("Lokasi MOR III tidak ditemukan");

            var approver = await _context.Users
             .FirstOrDefaultAsync(x => x.LocationId == mor3.Id && x.Role == "Admin");

            if (approver == null)
                throw new Exception("User approver MOR III tidak ditemukan");

            var report = new InspectionReport
            {
                ReportNumber =
                    $"BA-{DateTime.UtcNow:yyyy}-{await _context.InspectionReport.CountAsync() + 1:D4}",

                Title = dto.Title,
                InspectionDate = DateTime.SpecifyKind(dto.InspectionDate, DateTimeKind.Utc),
                LocationId = dto.LocationId,
                AssignedToUserId = approver.Id,
                Notes = dto.Notes,
                CreatedBy = createdBy,

                ApprovalStatus = InspectionStatus.PendingUser,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in dto.Details)
            {
                report.InspectionDetails.Add(new InspectionDetail
                {
                    AssetId = item.AssetId,
                    Condition = item.Condition,
                    Status = item.Status,
                    Notes = item.Notes,
                    Checked = true,
                    CheckedAt = DateTime.UtcNow
                });
            }

            _context.InspectionReport.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }

        // =========================
        // LOCATION APPROVAL
        // =========================
        public async Task<bool> ApproveByLocationAsync(int id, int userId)
        {
            var report = await _context.InspectionReport
                .FirstOrDefaultAsync(x => x.Id == id);

            if (report == null)
                throw new Exception("Report tidak ditemukan");

            if (report.AssignedToUserId != userId)
                throw new Exception("User tidak berhak approve");

            report.ApprovalStatus = InspectionStatus.Approved;

            report.ApprovedBy = userId;
            report.ApprovedAt = DateTime.UtcNow;

            report.ApprovedByLocationUserId = userId;
            report.LocationApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

       
        // =========================
        // PENDING APPROVAL
        // =========================
        public async Task<List<InspectionReport>> GetPendingApprovalAsync(int currentUserId)
        {
            return await _context.InspectionReport
                .Include(x => x.Location)
                .Include(x => x.Creator)
                .Include(x => x.AssignedToUser)
                .Where(x =>
                    x.AssignedToUserId == currentUserId &&
                    x.ApprovalStatus == InspectionStatus.PendingUser)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================
        // DELETE
        // =========================
        public async Task<bool> DeleteAsync(int id)
        {
            var data = await _context.InspectionReport.FindAsync(id);

            if (data == null)
                return false;

            _context.InspectionReport.Remove(data);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}