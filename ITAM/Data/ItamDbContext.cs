using ITAM.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Data
{
    public class ItamDbContext : DbContext
    {
        public ItamDbContext(DbContextOptions options) : base(options)
        {
        }

       public DbSet<User> Users { get; set; }
       public DbSet<Asset> Assets { get; set; }
       public DbSet<Location> Locations { get; set; }
       public DbSet<Category> Categories { get; set; }
       public DbSet<Department> Departments { get; set; }
       public DbSet<AssetHistory> AssetHistory { get; set; }
       public DbSet<InspectionReport> InspectionReport { get; set; }
      public DbSet<InspectionDetail> InspectionDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.User)
                .WithMany(u => u.Assets)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(a => a.ChangedByUser)
                .WithMany()
                .HasForeignKey(a => a.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionReport>()
     .HasOne(x => x.Creator)
     .WithMany(x => x.CreatedInspectionReport)
     .HasForeignKey(x => x.CreatedBy)
     .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionReport>()
    .HasOne(x => x.Approver)
    .WithMany(x => x.ApprovedInspectionReports)
    .HasForeignKey(x => x.ApprovedBy)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionReport>()
                .HasOne(x => x.ApprovedByLocationUser)
                .WithMany()
                .HasForeignKey(x => x.ApprovedByLocationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionReport>()
                .HasOne(x => x.AssignedToUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
